#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Yumineko.Character.SpriteCutter.Editor
{
    public readonly struct ImporterSettingsSetter
    {
        private readonly TextureImporter _importer;
        private readonly string _path;
        private readonly ImporterSettings _settings;
        private readonly Texture2D _texture2D;

        public ImporterSettingsSetter(Texture2D texture2D, ImporterSettings settings)
        {
            //  引数で受け取ったテクスチャオブジェクトのパスと、テクスチャインポート設定を取得
            _texture2D = texture2D;
            _path = AssetDatabase.GetAssetPath(texture2D);
            _importer = AssetImporter.GetAtPath(_path) as TextureImporter;
            _settings = settings;
        }

        public void SetImportSettings()
        {
            //  読み込み・書き込み情報を一時保存
            var isReadableTemp = _importer.isReadable;
            
            /* テスクチャインポートプロパティを編集 */
            //  読み込み・書き込みを有効化
            _importer.isReadable = true; 
            //  テクスチャモードをスプライトに設定
            _importer.textureType = TextureImporterType.Sprite;
            //  スプライトモードを複数に（スライス時に必要）
            _importer.spriteImportMode = SpriteImportMode.Multiple;
            //  最大サイズを8192に変更
            _importer.maxTextureSize = 8192;
            //  画像圧縮を変更
            _importer.textureCompression = _settings.Compression;
            //  MeshTypeを変更
            var textureSettings = new TextureImporterSettings();
            _importer.ReadTextureSettings( textureSettings );
            textureSettings.spriteMeshType = SpriteMeshType.FullRect;
            _importer.SetTextureSettings( textureSettings );
            // PPUを設定
            _importer.spritePixelsPerUnit = _settings.PixelPerUnit;

            /* SpriteEditorのスライス操作 */
            var newData = new List<SpriteMetaData>();

            var sliceWidth = _texture2D.width / Mathf.Max(_settings.SliceCount.x, 1f);
            var sliceHeight = _texture2D.height / Mathf.Max(_settings.SliceCount.y, 1f);
            var fileNo = 0;
            for (var j = 0; j < _settings.SliceCount.y; j++)
            {
                for (var i = 0; i < _settings.SliceCount.x; i++)
                {
                    var smd = new SpriteMetaData
                    {
                        pivot = _settings.Pivot,
                        alignment = 9,
                        name = fileNo.ToString(),
                        rect = new Rect(i * sliceWidth, j * sliceHeight, sliceWidth, sliceHeight)
                    };
                    newData.Add(smd);
                    fileNo++;
                }
            }

            _importer.spritesheet = newData.ToArray();

            /* 編集した内容をMetaファイルに反映 */
            // 読み込み、書き込み情報を復元
            _importer.isReadable = isReadableTemp;
            AssetDatabase.ImportAsset(_path, ImportAssetOptions.ForceUpdate);
        }
    }
}

#endif