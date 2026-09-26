"""
Corrige los 5 Asset/.anim y sus .meta, que se generaron con llaves dobles
({{ }}) por un error de escapado, lo que hacia que Unity no pudiera parsearlos.

Vuelve a escribirlos en Assets/Animations/ y valida el YAML con PyYAML de verdad.

    python Archivo/Generadores/fix_anim_braces.py
"""

import json
import os
import sys

import yaml

MANIFEST = 'Archivo/Generadores/manifest.json'
DEST = 'Assets/Animations'

# Mismos tiempos que la primera generacion
CLIPS = {
    'Idle':   {'step': 0.2,       'loop': True},
    'Walk':   {'step': 0.1,       'loop': True},
    'Run':    {'step': 0.0833333, 'loop': True},
    'Jump':   {'step': 0.1,       'loop': False},
    'Attack': {'step': 0.0666667, 'loop': False},
}

# OJO: llaves simples. La version anterior tenia {{ }} por un error de escapado.
CLIP_TEMPLATE = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!74 &7400000
AnimationClip:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: __NAME__
  serializedVersion: 7
  m_Legacy: 0
  m_Compressed: 0
  m_UseHighQualityCurve: 1
  m_RotationCurves: []
  m_CompressedRotationCurves: []
  m_EulerCurves: []
  m_PositionCurves: []
  m_ScaleCurves: []
  m_FloatCurves: []
  m_PPtrCurves:
  - serializedVersion: 2
    curve:
__CURVE__
    attribute: m_Sprite
    path: 
    classID: 212
    script: {fileID: 0}
    flags: 2
  m_SampleRate: 60
  m_WrapMode: 0
  m_Bounds:
    m_Center: {x: 0, y: 0, z: 0}
    m_Extent: {x: 0, y: 0, z: 0}
  m_ClipBindingConstant:
    genericBindings:
    - serializedVersion: 2
      path: 0
      attribute: 0
      script: {fileID: 0}
      typeID: 212
      customType: 23
      isPPtrCurve: 1
      isIntCurve: 0
      isSerializeReferenceCurve: 0
    pptrCurveMapping:
__MAPPING__
  m_AnimationClipSettings:
    serializedVersion: 2
    m_AdditiveReferencePoseClip: {fileID: 0}
    m_AdditiveReferencePoseTime: 0
    m_StartTime: 0
    m_StopTime: __STOP__
    m_OrientationOffsetY: 0
    m_Level: 0
    m_CycleOffset: 0
    m_HasAdditiveReferencePose: 0
    m_LoopTime: __LOOP__
    m_LoopBlend: 0
    m_LoopBlendOrientation: 0
    m_LoopBlendPositionY: 0
    m_LoopBlendPositionXZ: 0
    m_KeepOriginalOrientation: 0
    m_KeepOriginalPositionY: 1
    m_KeepOriginalPositionXZ: 0
    m_HeightFromFeet: 0
    m_Mirror: 0
  m_EditorCurves: []
  m_EulerEditorCurves: []
  m_HasGenericRootTransform: 0
  m_HasMotionFloatCurves: 0
  m_Events: []
"""

META_TEMPLATE = """fileFormatVersion: 2
guid: __GUID__
NativeFormatImporter:
  externalObjects: {}
  mainObjectFileID: 7400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


class UnityLoader(yaml.SafeLoader):
    """Loader que acepta las etiquetas !u!NN de Unity."""


def _construct_any(loader, tag_suffix, node):
    if isinstance(node, yaml.MappingNode):
        return loader.construct_mapping(node)
    if isinstance(node, yaml.SequenceNode):
        return loader.construct_sequence(node)
    return loader.construct_scalar(node)


# La directiva %TAG expande !u!74 a 'tag:unity3d.com,2011:74', asi que hay que
# registrar el prefijo expandido (y el corto por si no hay directiva).
UnityLoader.add_multi_constructor('tag:unity3d.com,2011:', _construct_any)
UnityLoader.add_multi_constructor('!u!', _construct_any)


def validate_yaml(path):
    """Devuelve (ok, mensaje). Valida sintaxis YAML de verdad, documento a documento."""
    with open(path, encoding='utf-8') as f:
        text = f.read()
    try:
        docs = list(yaml.load_all(text, Loader=UnityLoader))
    except yaml.YAMLError as exc:
        return False, f'YAML invalido: {exc}'
    if not docs:
        return False, 'el archivo no contiene ningun documento'
    return True, f'{len(docs)} documento(s), raiz={type(docs[0]).__name__}'


def fmt(value):
    out = f'{value:.7f}'.rstrip('0').rstrip('.')
    return out if out else '0'


def write_crlf(path, text):
    os.makedirs(os.path.dirname(path) or '.', exist_ok=True)
    with open(path, 'w', encoding='utf-8', newline='\r\n') as f:
        f.write(text)


def main():
    if not os.path.isfile(MANIFEST):
        raise SystemExit(f'ERROR: no encuentro {MANIFEST}. Ejecuta desde la raiz del proyecto.')

    with open(MANIFEST, encoding='utf-8') as f:
        manifest = json.load(f)

    # Los .meta ya existen en disco: reutilizamos sus GUIDs para no romper el controller.
    guids = {}
    for anim in CLIPS:
        meta_path = os.path.join(DEST, anim + '.anim.meta')
        with open(meta_path, encoding='utf-8') as f:
            for line in f:
                if line.startswith('guid:'):
                    guids[anim] = line.split(':', 1)[1].strip()
                    break
        if anim not in guids:
            raise SystemExit(f'ERROR: no pude leer el guid de {meta_path}')

    ok_all = True

    for anim, cfg in CLIPS.items():
        frames = manifest[anim]
        step = cfg['step']

        curve, mapping = [], []
        for i, frame in enumerate(frames):
            curve.append(f'    - time: {fmt(i * step)}')
            curve.append(f'      value: {{fileID: 21300000, guid: {frame["guid"]}, type: 3}}')
            mapping.append(f'    - {{fileID: 21300000, guid: {frame["guid"]}, type: 3}}')

        text = (CLIP_TEMPLATE
                .replace('__NAME__', anim)
                .replace('__CURVE__', '\n'.join(curve))
                .replace('__MAPPING__', '\n'.join(mapping))
                .replace('__STOP__', fmt(len(frames) * step))
                .replace('__LOOP__', '1' if cfg['loop'] else '0'))

        anim_path = os.path.join(DEST, anim + '.anim')
        meta_path = anim_path + '.meta'

        write_crlf(anim_path, text)
        write_crlf(meta_path, META_TEMPLATE.replace('__GUID__', guids[anim]))

        ok_a, msg_a = validate_yaml(anim_path)
        ok_m, msg_m = validate_yaml(meta_path)
        both = ok_a and ok_m
        ok_all = ok_all and both
        print(f'  {"ok  " if both else "FALLO"}  {anim:7} {len(frames)} frames  stop={fmt(len(frames) * step)}s  '
              f'| .anim: {msg_a} | .meta: {msg_m}')

    # Comprobacion directa del bug original
    print()
    leftovers = 0
    for anim in CLIPS:
        p = os.path.join(DEST, anim + '.anim')
        n = open(p, encoding='utf-8').read().count('{{')
        leftovers += n
    print(f'  {"ok  " if leftovers == 0 else "FALLO"}  llaves dobles que quedan: {leftovers}')

    print()
    if ok_all and leftovers == 0:
        print('TODO CORRECTO. Vuelve a Unity para que reimporte Assets/Animations.')
    else:
        sys.exit(1)


if __name__ == '__main__':
    main()
