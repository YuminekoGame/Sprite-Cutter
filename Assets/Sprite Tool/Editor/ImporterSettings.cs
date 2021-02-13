#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Yumineko.Character.SpriteCutter.Editor
{
    [CreateAssetMenu]
    public class ImporterSettings : ScriptableObject
    {
        public TextureImporterCompression Compression = TextureImporterCompression.Uncompressed;
        public Vector2Int SliceCount = new Vector2Int(3,4);
        //  Bottomの場合は(0.5f, 0f)
        public Vector2 Pivot = new Vector2(0.5f,0f);
        public SpriteMeshType MeshType = SpriteMeshType.FullRect;
        public float PixelPerUnit = 32f;
    }
}

#endif
