"""
Genera un tileset de terreno para plataformas a partir de Cambio/TileMap.png.

Que hace:
  1. Elige el mejor parche de muro 32x32 de la textura original (el que mejor se repite).
  2. Crea las 16 piezas de un juego de bordes de 4 vecinos (arriba/abajo/izq/der).
  3. Cada pieza = el muro + un remate (2 px oscuros + 4 px dorados) en cada lado sin vecino.
  4. Escribe PNG + .meta (PPU 33.68 para encajar en la celda de 0.95 del Grid).
  5. Escribe los 16 Tile assets (Tile integrado de Unity, fileID 13312).
  6. Escribe la Rule Tile con las 16 reglas.

Uso:  python Archivo/Generadores/generate_terrain.py
"""

import os
import re
import json
import hashlib
from PIL import Image

RAIZ = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
ORIGEN = os.path.join(RAIZ, 'Cambio', 'TileMap.png')
DESTINO = os.path.join(RAIZ, 'Assets', 'Tiles', 'Terrain')
CARPETA_PNG = os.path.join(DESTINO, 'Sprites')

PLANTILLA_META = os.path.join(RAIZ, 'Assets', 'Sprites', 'Character', 'Idle', 'Idle_0.png.meta')
PLANTILLA_TILE = os.path.join(RAIZ, 'Assets', 'Tiles', 'Tiles', 'tilemap(18X18)_0.asset')
PLANTILLA_TILE_META = os.path.join(RAIZ, 'Assets', 'Tiles', 'Tiles', 'tilemap(18X18)_0.asset.meta')

TAM = 32
PPU = 33.68                      # 32 px / 0.95 unidades de celda
GUID_RULETILE = '9d1514134bc4fbd41bb739b1b9a49231'   # RuleTile.cs del paquete 2d.tilemap.extras (Runtime, el que Unity importa)
GUID_TILE_INTEGRADO = '0000000000000000e000000000000000'  # UnityEngine.Tilemaps.Tile (integrado)
COLLIDER_TYPE = 1                # Tile.ColliderType.Sprite

# Remate: primero el brillo interior, luego la linea oscura exterior (para que las
# esquinas queden limpias, la linea oscura se pinta al final).
DORADO = (252, 214, 128, 255)   # banda clara: contrasta de verdad con el muro naranja
OSCURO = (46, 8, 4, 255)        # linea exterior casi negra
GROSOR_LINEA = 3
GROSOR_BRILLO = 5

# (nombre, arriba, abajo, izquierda, derecha)  -> True = hay vecino de ese lado
PIEZAS = [
    ('Fill',            True,  True,  True,  True),
    ('RimTop',          False, True,  True,  True),
    ('RimBottom',       True,  False, True,  True),
    ('RimLeft',         True,  True,  False, True),
    ('RimRight',        True,  True,  True,  False),
    ('RimTopBottom',    False, False, True,  True),
    ('RimLeftRight',    True,  True,  False, False),
    ('RimTopLeft',      False, True,  False, True),
    ('RimTopRight',     False, True,  True,  False),
    ('RimBottomLeft',   True,  False, False, True),
    ('RimBottomRight',  True,  False, True,  False),
    ('RimTopBottomLeft', False, False, False, True),
    ('RimTopBottomRight', False, False, True, False),
    ('RimTopLeftRight', True,  False, False, False),
    ('RimBottomLeftRight', False, True, False, False),
    ('RimAll',          False, False, False, False),
]

# Orden de vecinos por defecto de RuleTile (no se serializa: lo pone el inicializador,
# que existe justo para esto, compatible hacia atras).
POSICIONES = [(-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (-1, -1), (0, -1), (1, -1)]
THIS, NOT_THIS, DONTCARE = 1, 2, 0


def guid_de(texto):
    """GUID estable a partir del nombre (sin colisiones verificadas aparte)."""
    return hashlib.md5(('terrain:' + texto).encode('utf-8')).hexdigest()


def elegir_parche(im):
    """El parche 32x32 de muro que mejor se azuleja consigo mismo."""
    px = im.load()
    lum = lambda p: 0.299 * p[0] + 0.587 * p[1] + 0.114 * p[2]
    mejor = None
    for y0 in range(0, 128 - TAM + 1, 4):
        for x0 in range(0, 128 - TAM + 1, 4):
            lums = [lum(px[x, y]) for y in range(y0, y0 + TAM) for x in range(x0, x0 + TAM)]
            media = sum(lums) / len(lums)
            # Descartar props: el cuadrado oscuro y la tabla tienen pixeles muy oscuros
            if min(lums) < 95 or not (115 <= media <= 180):
                continue
            costura = 0
            for y in range(y0, y0 + TAM):
                costura += abs(lum(px[x0, y]) - lum(px[x0 + TAM - 1, y]))
            for x in range(x0, x0 + TAM):
                costura += abs(lum(px[x, y0]) - lum(px[x, y0 + TAM - 1]))
            costura /= (2 * TAM)
            if mejor is None or costura < mejor[0]:
                mejor = (costura, (x0, y0), media)
    return mejor


def generar_pieza(muro, arriba, abajo, izq, der):
    """Copia el muro y le pone el remate en los lados sin vecino."""
    pieza = muro.copy()
    px = pieza.load()

    def banda(y0, y1, x0, x1, color):
        for y in range(y0, y1):
            for x in range(x0, x1):
                px[x, y] = color

    brillos = []
    lineas = []
    if not arriba:
        brillos.append((0, GROSOR_BRILLO, 0, TAM))
        lineas.append((0, GROSOR_LINEA, 0, TAM))
    if not abajo:
        brillos.append((TAM - GROSOR_BRILLO, TAM, 0, TAM))
        lineas.append((TAM - GROSOR_LINEA, TAM, 0, TAM))
    if not izq:
        brillos.append((0, TAM, 0, GROSOR_BRILLO))
        lineas.append((0, TAM, 0, GROSOR_LINEA))
    if not der:
        brillos.append((0, TAM, TAM - GROSOR_BRILLO, TAM))
        lineas.append((0, TAM, TAM - GROSOR_LINEA, TAM))

    for y0, y1, x0, x1 in brillos:
        banda(y0, y1, x0, x1, DORADO)
    for y0, y1, x0, x1 in lineas:
        banda(y0, y1, x0, x1, OSCURO)

    return pieza


def codificar_vecinos(arriba, abajo, izq, der):
    """m_Neighbors: 8 enteros (uno por posicion), cada uno como 8 digitos hex
    en orden de bytes little-endian, que es como lo escribe Unity."""
    presentes = {(0, 1): arriba, (-1, 0): izq, (1, 0): der, (0, -1): abajo}
    valores = []
    for pos in POSICIONES:
        if pos not in presentes:
            valores.append(DONTCARE)
        else:
            valores.append(THIS if presentes[pos] else NOT_THIS)
    return ''.join('%08x' % v for v in valores), valores


def escribir_meta_png(ruta, guid, plantilla):
    texto = plantilla.replace(plantilla_guid, guid)
    texto = re.sub(r'(spritePixelsToUnits:\s*)[\d.]+', r'\g<1>' + str(PPU), texto)
    with open(ruta, 'w', encoding='utf-8', newline='') as f:
        f.write(texto.replace('\r\n', '\n').replace('\n', '\r\n'))


def main():
    im = Image.open(ORIGEN).convert('RGBA')
    costura, (x0, y0), media = elegir_parche(im)
    print(f'Parche de muro elegido: ({x0},{y0})  costura {costura:.1f}  brillo medio {media:.0f}')
    muro = im.crop((x0, y0, x0 + TAM, y0 + TAM))

    plantilla_meta = open(PLANTILLA_META, encoding='utf-8').read()
    plantilla_tile = open(PLANTILLA_TILE, encoding='utf-8').read()
    plantilla_tile_meta = open(PLANTILLA_TILE_META, encoding='utf-8').read()

    os.makedirs(CARPETA_PNG, exist_ok=True)

    guids = {}
    reglas = []
    print()
    print('=== Piezas generadas ===')
    for nombre, arriba, abajo, izq, der in PIEZAS:
        pieza = generar_pieza(muro, arriba, abajo, izq, der)

        png = os.path.join(CARPETA_PNG, f'Terrain_{nombre}.png')
        pieza.save(png)
        guid_png = guid_de('png:' + nombre)
        guids[nombre] = guid_png
        escribir_meta_png(png + '.meta', guid_png, plantilla_meta)

        guid_tile = guid_de('tile:' + nombre)
        guids[nombre + ':asset'] = guid_tile
        tile = plantilla_tile.replace('m_Name: tilemap(18X18)_0', f'm_Name: Terrain_{nombre}')
        tile = re.sub(r'm_Sprite: \{[^}]*\}',
                      'm_Sprite: {fileID: 21300000, guid: %s, type: 3}' % guid_png, tile)
        ruta_tile = os.path.join(DESTINO, f'Terrain_{nombre}.asset')
        with open(ruta_tile, 'w', encoding='utf-8', newline='') as f:
            f.write(tile.replace('\r\n', '\n').replace('\n', '\r\n'))
        with open(ruta_tile + '.meta', 'w', encoding='utf-8', newline='') as f:
            f.write(plantilla_tile_meta.replace(plantilla_tile_guid, guid_tile)
                    .replace('\r\n', '\n').replace('\n', '\r\n'))

        vecinos_txt, vecinos = codificar_vecinos(arriba, abajo, izq, der)
        reglas.append((nombre, vecinos_txt, vecinos, guid_png))
        lados = ''.join(n for n, v in [('A', arriba), ('B', abajo), ('I', izq), ('D', der)] if not v) or '-'
        print(f'   Terrain_{nombre:22} remates: {lados:4} vecinos {vecinos}')

    # ---- Rule Tile ----
    guid_rt = guid_de('ruletile:Terrain')
    lineas = [
        '%YAML 1.1',
        '%TAG !u! tag:unity3d.com,2011:',
        '--- !u!114 &11400000',
        'MonoBehaviour:',
        '  m_ObjectHideFlags: 0',
        '  m_CorrespondingSourceObject: {fileID: 0}',
        '  m_PrefabInstance: {fileID: 0}',
        '  m_PrefabAsset: {fileID: 0}',
        '  m_GameObject: {fileID: 0}',
        '  m_Enabled: 1',
        '  m_EditorHideFlags: 0',
        f'  m_Script: {{fileID: 11500000, guid: {GUID_RULETILE}, type: 3}}',
        '  m_Name: Terrain',
        '  m_EditorClassIdentifier: ',
        f'  m_DefaultSprite: {{fileID: 21300000, guid: {guids["Fill"]}, type: 3}}',
        '  m_DefaultGameObject: {fileID: 0}',
        f'  m_DefaultColliderType: {COLLIDER_TYPE}',
        '  m_TilingRules:',
    ]
    for nombre, vecinos_txt, _, guid_png in reglas:
        lineas += [
            f'  - m_Neighbors: {vecinos_txt}',
            '    m_Sprites:',
            f'    - {{fileID: 21300000, guid: {guid_png}, type: 3}}',
            '    m_MinAnimationSpeed: 1',
            '    m_MaxAnimationSpeed: 1',
            '    m_PerlinScale: 0.5',
            '    m_RuleTransform: 0',
            '    m_Output: 0',
            f'    m_ColliderType: {COLLIDER_TYPE}',
            '    m_RandomTransform: 0',
        ]
    ruta_rt = os.path.join(DESTINO, 'Terrain.asset')
    with open(ruta_rt, 'w', encoding='utf-8', newline='') as f:
        f.write('\r\n'.join(lineas) + '\r\n')
    with open(ruta_rt + '.meta', 'w', encoding='utf-8', newline='') as f:
        f.write(plantilla_tile_meta.replace(plantilla_tile_guid, guid_rt)
                .replace('\r\n', '\n').replace('\n', '\r\n'))

    # ---- indice ----
    with open(os.path.join(RAIZ, 'Archivo', 'Generadores', 'terrain_guids.json'), 'w', encoding='utf-8') as f:
        json.dump(guids, f, indent=2)

    print()
    print(f'Rule Tile: {ruta_rt}  (guid {guid_rt})')
    print(f'Piezas   : {len(PIEZAS)} PNG + {len(PIEZAS)} Tile assets')


if __name__ == '__main__':
    # El .meta de la plantilla define su propio guid: lo leemos para sustituirlo
    plantilla_guid = re.search(r'^guid: ([0-9a-f]{32})',
                               open(PLANTILLA_META, encoding='utf-8').read(), re.M).group(1)
    plantilla_tile_guid = re.search(r'^guid: ([0-9a-f]{32})',
                                    open(PLANTILLA_TILE_META, encoding='utf-8').read(), re.M).group(1)
    main()
