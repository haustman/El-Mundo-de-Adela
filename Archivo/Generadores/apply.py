"""
Aplica el cambio de personaje completo. Ejecutar DESDE LA RAIZ DEL PROYECTO:

    python _staging/apply.py --check     # solo valida, no toca nada
    python _staging/apply.py             # aplica los cambios

Pasos:
  1. Se niega a ejecutarse si Unity Editor esta abierto.
  2. Archiva Assets/Animations -> Archivo/CharacterAntiguo
  3. Mueve _staging/Character  -> Assets/Sprites/Character
  4. Mueve _staging/Animations -> Assets/Animations
  5. Edita SampleScene.unity (sprite, escala, controller y campos del script)
     localizando los componentes por su fileID, no por numero de linea.
  6. Mueve los generadores a Archivo/Generadores y limpia _staging
"""

import os
import re
import shutil
import subprocess
import sys

SCENE = 'Assets/Scenes/SampleScene.unity'
BACKUP = 'Archivo/SampleScene.unity.bak'
ARCHIVE = 'Archivo/CharacterAntiguo'

SPRITE_GUID = '01985a12499f19b680e277f4b46b68b0'      # Idle_0.png
CONTROLLER_GUID = 'b80ea324bbc15e5e6a25acadaecbdfaa'  # Character.controller

ID_SPRITE_RENDERER = '1578567568'   # SpriteRenderer del GameObject "Circle"
ID_TRANSFORM = '1578567569'         # Transform del GameObject "Circle"
ID_ANIMATOR = '1578567570'          # Animator del GameObject "Circle"
ID_SCRIPT = '1462819167'            # CharacterController del GameObject "Character"

CHECK_ONLY = '--check' in sys.argv


def unity_is_running():
    """True si hay un Unity.exe activo. Muestra que procesos ha encontrado."""
    try:
        out = subprocess.run(['tasklist'], capture_output=True, text=True,
                             encoding='utf-8', errors='replace').stdout or ''
    except Exception as exc:
        print(f'  aviso: no se pudo comprobar tasklist ({exc}); se asume que Unity esta cerrado')
        return False

    found = [ln.strip() for ln in out.splitlines() if ln.strip().lower().startswith('unity.exe')]
    if found:
        print('  procesos encontrados:')
        for ln in found:
            print(f'    {ln}')
    lock = os.path.isfile('Temp/UnityLockfile')
    print(f'  Temp/UnityLockfile: {"PRESENTE (proyecto abierto)" if lock else "ausente"}')
    return bool(found)


def split_blocks(text):
    """Devuelve [(inicio, fin, fileID)] de cada documento YAML del .unity."""
    doc = re.compile(r'^--- !u!\d+ &(-?\d+)[ \t]*\r?$', re.M)
    hits = list(doc.finditer(text))
    return [(m.start(), hits[i + 1].start() if i + 1 < len(hits) else len(text), m.group(1))
            for i, m in enumerate(hits)]


def find_block(blocks, block_id):
    for start, end, bid in blocks:
        if bid == block_id:
            return start, end
    raise SystemExit(f'ERROR: no se encontro el bloque {block_id} en la escena')


def replace_in_block(text, blocks, block_id, pattern, repl, label):
    start, end = find_block(blocks, block_id)
    seg = text[start:end]
    new_seg, n = re.subn(pattern, repl, seg, count=1)
    if n == 0:
        raise SystemExit(f'ERROR: no se encontro el patron esperado en "{label}" (bloque {block_id})')
    print(f'  ok  {label}')
    return text[:start] + new_seg + text[end:]


def insert_after_in_block(text, blocks, block_id, anchor, inserted_lines, label):
    """Inserta lineas tras la linea que coincide con 'anchor', reusando su salto de linea."""
    start, end = find_block(blocks, block_id)
    seg = text[start:end]
    m = re.search(anchor + r'(\r?\n)', seg, re.M)
    if not m:
        raise SystemExit(f'ERROR: no se encontro el ancla en "{label}" (bloque {block_id})')
    nl = m.group(1)
    payload = ''.join(f'{line}{nl}' for line in inserted_lines)
    print(f'  ok  {label}')
    return text[:start] + seg[:m.end()] + payload + seg[m.end():] + text[end:]


def edit_scene_text(text):
    blocks = split_blocks(text)
    print(f'  bloques YAML en la escena: {len(blocks)}')
    if not blocks:
        raise SystemExit('ERROR: no se encontro ningun bloque YAML. La escena no esta en el formato esperado.')

    text = replace_in_block(
        text, blocks, ID_SPRITE_RENDERER,
        r'm_Sprite: \{fileID: -?\d+, guid: [0-9a-f]{32}, type: 3\}',
        'm_Sprite: {fileID: 21300000, guid: ' + SPRITE_GUID + ', type: 3}',
        'sprite del Circle -> Idle_0')

    text = replace_in_block(
        text, blocks, ID_TRANSFORM,
        r'm_LocalScale: \{x: 1\.2, y: 1\.2, z: 1\}',
        'm_LocalScale: {x: 1.44, y: 1.44, z: 1}',
        'escala del Circle 1.2 -> 1.44')

    text = replace_in_block(
        text, blocks, ID_ANIMATOR,
        r'm_Controller: \{fileID: 9100000, guid: [0-9a-f]{32}, type: 2\}',
        'm_Controller: {fileID: 9100000, guid: ' + CONTROLLER_GUID + ', type: 2}',
        'controller del Circle -> Character.controller')

    text = insert_after_in_block(
        text, blocks, ID_SCRIPT,
        r'^  maxJumpCount: 2',
        ['  runSpeedMultiplier: 1.6', '  runKey: 304'],
        'campos runSpeedMultiplier y runKey en el script')

    return text


def move(src, dst):
    os.makedirs(os.path.dirname(dst), exist_ok=True)
    if os.path.exists(dst):
        raise SystemExit(f'ERROR: ya existe {dst}, abortando para no sobreescribir')
    shutil.move(src, dst)
    print(f'  ok  {src}  ->  {dst}')


def main():
    if not os.path.isfile('ProjectSettings/ProjectVersion.txt'):
        raise SystemExit('ERROR: ejecuta este script desde la raiz del proyecto (no encuentro ProjectSettings/)')

    print('[1/6] Estado de Unity')
    if CHECK_ONLY:
        print('  modo --check: no se comprueba')
    elif unity_is_running():
        raise SystemExit('\n  ERROR: Unity Editor sigue abierto. Guarda la escena, cierra Unity y vuelve a intentar.')
    else:
        print('  ok  Unity cerrado')

    print('\n[2/6] Preparando la edicion de la escena')
    with open(SCENE, encoding='utf-8', newline='') as f:
        text = f.read()
    new_text = edit_scene_text(text)
    print(f'  texto resultante: {len(text)} -> {len(new_text)} caracteres')

    if CHECK_ONLY:
        print('\nCHECK OK: todos los patrones coinciden. Nada se ha modificado.')
        return

    print('\n[3/6] Archivando los assets del personaje viejo')
    move('Assets/Animations', ARCHIVE)
    if os.path.exists('Assets/Animations.meta'):
        move('Assets/Animations.meta', ARCHIVE + '.meta')

    print('\n[4/6] Moviendo los assets nuevos')
    move('_staging/Character', 'Assets/Sprites/Character')
    move('_staging/Animations', 'Assets/Animations')

    print('\n[5/6] Guardando la escena')
    os.makedirs('Archivo', exist_ok=True)
    with open(BACKUP, 'w', encoding='utf-8', newline='') as f:
        f.write(text)
    print(f'  ok  copia de seguridad en {BACKUP}')
    with open(SCENE, 'w', encoding='utf-8', newline='') as f:
        f.write(new_text)
    print('  ok  SampleScene.unity actualizada')

    print('\n[6/6] Limpiando staging')
    move('_staging/generate.py', 'Archivo/Generadores/generate_sprites.py')
    move('_staging/generate_anims.py', 'Archivo/Generadores/generate_anims.py')
    move('_staging/apply.py', 'Archivo/Generadores/apply_character_swap.py')
    shutil.rmtree('_staging/__pycache__', ignore_errors=True)
    try:
        os.rmdir('_staging')
        print('  ok  _staging eliminado')
    except OSError:
        print('  aviso: _staging no esta vacio, revisalo')

    print('\nLISTO. Abre Unity: reimportara los assets y la escena tendra el personaje nuevo.')


if __name__ == '__main__':
    main()
