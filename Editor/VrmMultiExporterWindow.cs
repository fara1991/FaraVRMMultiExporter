using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fara.FaraVRMMultiExporter.Editor
{
    [ExcludeFromCodeCoverage]
    public class VrmMultiExporterWindow : EditorWindow
    {
        private VrmExportSettings _settings;
        private Vector2 _scrollPosition;

        // 連続エクスポートの状態管理用
        private List<GameObject> _remainingAvatars;
        private int _totalCount;
        private int _currentExportIndex;
        private string _waitingFileName;

        [MenuItem("FaraScripts/VRMMultiExporter")]
        public static void Open()
        {
            GetWindow<VrmMultiExporterWindow>(L10N.WindowTitle);
        }

        private void OnEnable()
        {
            _settings = VrmExportSettings.CreateNew();
        }

        private void OnGUI()
        {
            HandleDragAndDrop();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawLanguageSettings();
            EditorGUILayout.Space();

            DrawHowToUse();
            EditorGUILayout.Space();

            DrawAvatarSelection();
            EditorGUILayout.Space();

            DrawExportSettings();
            EditorGUILayout.Space();

            DrawActions();

            EditorGUILayout.EndScrollView();
        }

        private void HandleDragAndDrop()
        {
            var ev = Event.current;
            if (ev.type != EventType.DragUpdated && ev.type != EventType.DragPerform) return;
            if (!position.Contains(GUIUtility.GUIToScreenPoint(ev.mousePosition))) return;

            var droppedObjects = DragAndDrop.objectReferences;
            var prefabs = VrmAssetUtility.GetDroppedVrmPrefabs(droppedObjects);

            if (prefabs.Count == 0) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (ev.type != EventType.DragPerform) return;

            DragAndDrop.AcceptDrag();
            foreach (var prefab in prefabs)
            {
                if (_settings.vrmList.Contains(prefab)) continue;

                var nullIndex = _settings.vrmList.IndexOf(null);
                if (nullIndex >= 0) _settings.vrmList[nullIndex] = prefab;
                else _settings.vrmList.Add(prefab);
            }

            ev.Use();
        }

        private static void DrawLanguageSettings()
        {
            EditorGUILayout.LabelField(L10N.LanguageHeader, EditorStyles.boldLabel);
            LocalizationManager.IsJapanese =
                EditorGUILayout.Popup(LocalizationManager.IsJapanese ? 0 : 1, new[] {"Japanese", "English"}) == 0;
        }

        private static void DrawHowToUse()
        {
            EditorGUILayout.LabelField(L10N.HowToUseHeader, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(L10N.Instructions, MessageType.Info);
        }

        private void DrawAvatarSelection()
        {
            EditorGUILayout.LabelField(L10N.AvatarSelectionHeader, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(GUI.skin.box);

            var count = EditorGUILayout.IntField(L10N.AvatarCount, _settings.vrmList.Count);
            if (count != _settings.vrmList.Count)
            {
                while (_settings.vrmList.Count < count) _settings.vrmList.Add(null);
                while (_settings.vrmList.Count > count) _settings.vrmList.RemoveAt(_settings.vrmList.Count - 1);
            }

            var displayCount = _settings.vrmList.Count + 1;
            for (var i = 0; i < displayCount; i++)
            {
                var currentObj = i < _settings.vrmList.Count ? _settings.vrmList[i] : null;
                using (new EditorGUILayout.HorizontalScope())
                {
                    var result = (GameObject) EditorGUILayout.ObjectField(L10N.AvatarElement(i), currentObj,
                        typeof(GameObject), false);
                    if (result != currentObj)
                    {
                        if (i >= _settings.vrmList.Count)
                        {
                            if (result is not null) _settings.vrmList.Add(result);
                        }
                        else _settings.vrmList[i] = result;
                    }

                    if (GUILayout.Button("+", GUILayout.Width(25)))
                    {
                        if (i >= _settings.vrmList.Count) _settings.vrmList.Add(null);
                        else _settings.vrmList.Insert(i + 1, null);
                    }

                    EditorGUI.BeginDisabledGroup(i >= _settings.vrmList.Count);
                    if (GUILayout.Button("-", GUILayout.Width(25))) _settings.vrmList.RemoveAt(i);
                    EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawExportSettings()
        {
            EditorGUILayout.LabelField(L10N.ExportSettingsHeader, EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                _settings.exportFolderPath = EditorGUILayout.TextField(L10N.ExportFolder, _settings.exportFolderPath);
                if (GUILayout.Button(L10N.SelectFolder, GUILayout.Width(100)))
                {
                    var path = EditorUtility.OpenFolderPanel(L10N.SelectFolderDialogTitle, _settings.exportFolderPath,
                        "");
                    if (!string.IsNullOrEmpty(path)) _settings.exportFolderPath = path;
                }
            }

            _settings.includeDependencies =
                EditorGUILayout.ToggleLeft(L10N.IncludeDependencies, _settings.includeDependencies);
            _settings.recurse = EditorGUILayout.ToggleLeft(L10N.Recurse, _settings.recurse);
            EditorGUILayout.Space();
            _settings.isExcludeFolderEnabled =
                EditorGUILayout.ToggleLeft(L10N.ExcludeFolderEnabled, _settings.isExcludeFolderEnabled);
            if (_settings.isExcludeFolderEnabled)
            {
                DrawStringList(_settings.excludeFolders, L10N.ExcludeFoldersHeader, L10N.ExcludeFolderCount,
                    L10N.ExcludeFolderElement);
            }

            EditorGUILayout.HelpBox(L10N.ExportSettingsNote, MessageType.None);
        }

        private static void DrawStringList(List<string> list, string header, string countLabel,
            Func<int, string> elementLabelFunc)
        {
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            var count = EditorGUILayout.IntField(countLabel, list.Count);
            if (count != list.Count)
            {
                while (list.Count < count) list.Add("");
                while (list.Count > count) list.RemoveAt(list.Count - 1);
            }

            for (var i = 0; i < list.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    list[i] = EditorGUILayout.TextField(elementLabelFunc(i), list[i]);
                    if (GUILayout.Button("+", GUILayout.Width(25))) list.Insert(i + 1, "");
                    if (GUILayout.Button("-", GUILayout.Width(25))) list.RemoveAt(i);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawActions()
        {
            EditorGUILayout.LabelField(L10N.ActionsHeader, EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(_remainingAvatars != null);
            if (GUILayout.Button(L10N.ExportAllOneByOne, GUILayout.Height(30)))
            {
                StartSequentialExport();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void StartSequentialExport()
        {
            if (string.IsNullOrEmpty(_settings.exportFolderPath))
            {
                EditorUtility.DisplayDialog(L10N.Error, L10N.ExportFolderNotSet, L10N.Ok);
                return;
            }

            _remainingAvatars = _settings.vrmList.FindAll(v => v is not null);
            if (_remainingAvatars.Count == 0)
            {
                EditorUtility.DisplayDialog(L10N.Error, L10N.NoValidAvatarSelected, L10N.Ok);
                _remainingAvatars = null;
                return;
            }

            _totalCount = _remainingAvatars.Count;
            _currentExportIndex = 0;

            EditorApplication.update += SequentialExportUpdate;
            ExportNext();
        }

        private void SequentialExportUpdate()
        {
            if (string.IsNullOrEmpty(_waitingFileName)) return;
            if (!File.Exists(_waitingFileName) || IsFileLocked(_waitingFileName)) return;

            _waitingFileName = null;
            _currentExportIndex++;
            ExportNext();
        }

        private void ExportNext()
        {
            if (_remainingAvatars == null || _currentExportIndex >= _remainingAvatars.Count)
            {
                FinishExport();
                return;
            }

            var vrm = _remainingAvatars[_currentExportIndex];

            var sanitizedName = Path.GetInvalidFileNameChars()
                .Aggregate(vrm.name, (current, c) => current.Replace(c, '_')).Trim();
            var fileName = Path.Combine(_settings.exportFolderPath, sanitizedName + ".unitypackage");

            if (File.Exists(fileName)) File.Delete(fileName);

            EditorUtility.DisplayProgressBar(L10N.Exporting,
                L10N.ExportingProgress(vrm.name, _currentExportIndex + 1, _totalCount),
                (float) (_currentExportIndex + 1) / _totalCount);

            _waitingFileName = fileName;

            // 重要：ExportPackageOptions.Default を使用せず、必要なフラグのみを指定します。
            // ExportPackageOptions.Default には完了後にフォルダを開く処理が含まれているため、
            // それを避けるために明示的にフラグを組み合わせて呼び出すように VrmExportCore.ExportSingle を調整するか、
            // ここでは直接 AssetDatabase.ExportPackage を呼び出します。

            var assetPath = AssetDatabase.GetAssetPath(vrm);
            var dependencies = _settings.includeDependencies
                ? AssetDatabase.GetDependencies(assetPath, true)
                : new[] {assetPath};

            // 除外フィルタリング
            if (_settings.isExcludeFolderEnabled && _settings.excludeFolders != null)
            {
                var excludes = _settings.excludeFolders.ToArray();
                dependencies = dependencies.Where(p =>
                    !string.IsNullOrWhiteSpace(p) &&
                    !excludes.Any(ex => !string.IsNullOrWhiteSpace(ex) && p.Contains(ex))).ToArray();
            }

            // ExportPackageOptions.None から開始することで、自動でフォルダが開くのを防ぎます
            var options = ExportPackageOptions.Default;
            if (_settings.recurse) options |= ExportPackageOptions.Recurse;
            if (_settings.includeDependencies) options |= ExportPackageOptions.IncludeDependencies;

            AssetDatabase.ExportPackage(dependencies, fileName, options);
        }

        private void FinishExport()
        {
            EditorApplication.update -= SequentialExportUpdate;
            _remainingAvatars = null;
            _waitingFileName = null;

            EditorUtility.ClearProgressBar();

            // 完了ダイアログを出し、OKが押されたらフォルダを開く
            EditorUtility.DisplayDialog(L10N.Complete, L10N.ExportAllComplete, L10N.Ok);
            {
                EditorUtility.RevealInFinder(_settings.exportFolderPath + "/");
            }
        }

        private static bool IsFileLocked(string filePath)
        {
            try
            {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }
    }
}