#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Yumineko.Character.SpriteCutter.Editor
{
    public class SpriteCutterWindow : EditorWindow
    {
        private  ImporterSettings _settings;
        private readonly string SAVE_KEY = typeof(SpriteCutterWindow).FullName;

        private string _folderPath =  string.Empty;
        private string FolderPath
        {
            get => _folderPath = string.IsNullOrEmpty(_folderPath) ? Application.dataPath : _folderPath;
            set => _folderPath = (string.IsNullOrEmpty(value)) ? FolderPath : value;
        }

        private readonly List<Texture2D> _targetAssets = new List<Texture2D>();


        private void OnEnable()
        {
            FolderPath = EditorUserSettings.GetConfigValue(SAVE_KEY + nameof(FolderPath));
            var settingPath = EditorUserSettings.GetConfigValue(SAVE_KEY + nameof(_settings));
            _settings = AssetDatabase.LoadAssetAtPath<ImporterSettings>(settingPath);
        }

        private void OnDestroy()
        {
            EditorUserSettings.SetConfigValue(SAVE_KEY + nameof(FolderPath), FolderPath);
            if(_settings == null) return;
            var path = AssetDatabase.GetAssetPath(_settings);
            EditorUserSettings.SetConfigValue(SAVE_KEY + nameof(_settings), path);
        }

        private void OnGUI()
        {
            var options = new[]
            {
                GUILayout.Width(96),
                GUILayout.Height(96)
            };

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("画像フォルダ");
                if (GUILayout.Button("選択"))
                {
                    FolderPath = EditorUtility.OpenFolderPanel("画像フォルダ", FolderPath, string.Empty);
                }
            }
            GUILayout.Label(FolderPath);
            GUILayout.Space(20f);
            
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("スライス設定");
                _settings = EditorGUILayout.ObjectField(_settings,typeof(ImporterSettings), false, null) as ImporterSettings;
            }
            GUILayout.Space(40f);
            if (GUILayout.Button("実行"))
            {
                if (_settings == null)
                {
                    Debug.LogError("スライス設定が選択されていません");
                    return;
                }
                GetTextureAssets();
                foreach (var setter in _targetAssets.Select(targetAsset => new ImporterSettingsSetter(targetAsset, _settings)))
                {
                    setter.SetImportSettings();
                }

                Debug.Log("スライスが完了しました");
            }
        }

        [MenuItem("Tools/Sprite Cutter")]
        private static void ShowWindow()
        {
            var window = (SpriteCutterWindow) GetWindow(typeof(SpriteCutterWindow));
            window.titleContent = new GUIContent("Sprite Cutter");

            window.Show();
        }

        private void GetTextureAssets()
        {
            //指定したディレクトリに入っている全ファイルを取得(子ディレクトリも含む)
            var filePathArray = Directory.GetFiles (FolderPath, "*", SearchOption.AllDirectories);

            //取得したファイルの中からアセットだけリストに追加する
            foreach (var filePath in filePathArray.Select(GetDataPath)) {
                var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                if(asset != null){
                    _targetAssets.Add (asset);
                }
            }
        }

        private string GetDataPath(string fullPath)
        {
            var startIndex = fullPath.IndexOf("Assets/", StringComparison.Ordinal);
            if (startIndex == -1) startIndex = fullPath.IndexOf("Assets\\", StringComparison.Ordinal);
            if (startIndex == -1) return string.Empty;
 
            return fullPath.Substring(startIndex);
        }
    }
}

#endif