"""
Genera las AnimationClips (.anim) del personaje nuevo en _staging/Animations/.

Los GUIDs de los sprites se leen de _staging/manifest.json para que no haya
errores de transcripcion. La estructura del YAML replica la que Unity genero
en el antiguo Move.anim (misma version de serializacion).
"""

import hashlib
import json
import os

STAGE = '_staging/Animations'
MANIFEST = '_staging/manifest.json'

# paso en segundos entre frames. Todos son multiplos exactos de 1/60
# para que Unity no tenga que remuestrear las curvas.
CLIPS = {
    'Idle':   {'step': 0.2,       'loop': True,  'desc': '12 ticks (5 fps)'},
    'Walk':   {'step': 0.1,       'loop': True,  'desc': '6 ticks (10 fps)'},
    'Run':    {'step': 0.0833333, 'loop': True,  'desc': '5 ticks (12 fps)'},
    'Jump':   {'step': 0.1,       'loop': False, 'desc': '6 ticks (10 fps)'},
    'Attack': {'step': 0.0666667, 'loop': False, 'desc': '4 ticks (15 fps)'},
}

CLIP_TEMPLATE = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!74 &7400000
AnimationClip:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
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
    script: {{fileID: 0}}
    flags: 2
  m_SampleRate: 60
  m_WrapMode: 0
  m_Bounds:
    m_Center: {{x: 0, y: 0, z: 0}}
    m_Extent: {{x: 0, y: 0, z: 0}}
  m_ClipBindingConstant:
    genericBindings:
    - serializedVersion: 2
      path: 0
      attribute: 0
      script: {{fileID: 0}}
      typeID: 212
      customType: 23
      isPPtrCurve: 1
      isIntCurve: 0
      isSerializeReferenceCurve: 0
    pptrCurveMapping:
__MAPPING__
  m_AnimationClipSettings:
    serializedVersion: 2
    m_AdditiveReferencePoseClip: {{fileID: 0}}
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
  externalObjects: {{}}
  mainObjectFileID: 7400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def make_guid(seed):
    return hashlib.md5(('Unity2DPlatformer_CharClip_' + seed).encode('utf-8')).hexdigest()


def write_crlf(path, text):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, 'w', encoding='utf-8', newline='\r\n') as f:
        f.write(text)


def fmt(value):
    """Formatea el tiempo como lo hace Unity: sin ceros de relleno a la derecha."""
    out = f'{value:.7f}'.rstrip('0').rstrip('.')
    return out if out else '0'


def main():
    with open(MANIFEST, encoding='utf-8') as f:
        manifest = json.load(f)

    guids = {}

    for anim, cfg in CLIPS.items():
        frames = manifest[anim]
        step = cfg['step']

        curve_lines = []
        mapping_lines = []
        for i, frame in enumerate(frames):
            t = fmt(i * step)
            curve_lines.append(f'    - time: {t}')
            curve_lines.append(f'      value: {{fileID: 21300000, guid: {frame["guid"]}, type: 3}}')
            mapping_lines.append(f'    - {{fileID: 21300000, guid: {frame["guid"]}, type: 3}}')

        stop = fmt(len(frames) * step)

        text = (CLIP_TEMPLATE
                .replace('__NAME__', anim)
                .replace('__CURVE__', '\n'.join(curve_lines))
                .replace('__MAPPING__', '\n'.join(mapping_lines))
                .replace('__STOP__', stop)
                .replace('__LOOP__', '1' if cfg['loop'] else '0'))

        clip_path = f'{STAGE}/{anim}.anim'
        write_crlf(clip_path, text)

        g = make_guid(anim)
        write_crlf(clip_path + '.meta',
                   META_TEMPLATE.replace('__GUID__', g))

        guids[anim] = g
        loop = 'loop  ' if cfg['loop'] else 'una vez'
        print(f'{anim:7} {len(frames)} frames x {step:.4f}s = {stop}s  {loop}  [{cfg["desc"]}]  guid={g}')

    with open('_staging/clip_guids.json', 'w', encoding='utf-8') as f:
        json.dump(guids, f, indent=2)

    print('\nClips en', STAGE)
    print('GUIDs: _staging/clip_guids.json')


if __name__ == '__main__':
    main()
