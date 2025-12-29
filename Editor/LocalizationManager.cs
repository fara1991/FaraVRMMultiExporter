using UnityEditor;

namespace Fara.FaraVRMMultiExporter.Editor
{
    public static class LocalizationManager
    {
        private const string LanguagePrefKey = "Fara_Language";
        private static bool? _isJapanese;

        public static bool IsJapanese
        {
            get
            {
                _isJapanese ??= EditorPrefs.GetBool(LanguagePrefKey, true);
                return _isJapanese.Value;
            }
            set
            {
                _isJapanese = value;
                EditorPrefs.SetBool(LanguagePrefKey, value);
            }
        }

        public static string Get(string japanese, string english) => IsJapanese ? japanese : english;
    }

    public static class L10N
    {
        private static string Get(string japanese, string english) => LocalizationManager.Get(japanese, english);

        public static string WindowTitle => "VRMMultiExporter";
        public static string Ok => "OK";
        public static string Error => Get("エラー", "Error");
        public static string Complete => Get("完了", "Complete");

        public static string LanguageHeader => Get("言語 / Language", "Language / 言語");
        public static string HowToUseHeader => Get("使い方", "How to use");

        public static string Instructions => Get(
            "1. ExportしたいVRM（Project上のPrefab）をリストに追加\n" +
            "2. Export先フォルダを選択\n" +
            "3. 「全件を1体ずつExport（連続）」で実行\n\n" +
            "※ 出力ファイル名は「アバター名.unitypackage」になります",
            "1. Add VRM prefabs (Project assets) to the list\n" +
            "2. Choose export folder\n" +
            "3. Run via “Export all one-by-one”\n\n" +
            "* Output file name = AvatarName.unitypackage"
        );

        public static string AvatarSelectionHeader => Get("アバター選択", "Avatar selection");
        public static string AvatarCount => Get("アバター数", "Avatar count");
        public static string AvatarElement(int index) => Get($"VRM {index + 1}体目", $"VRM #{index + 1}");

        public static string ExportSettingsHeader => Get("Export設定", "Export settings");
        public static string ExportFolder => Get("Export先フォルダ", "Export folder");
        public static string SelectFolder => Get("フォルダ選択", "Select folder");
        public static string SelectFolderDialogTitle => Get("Export先フォルダを選択", "Select export folder");

        public static string IncludeDependencies => Get("依存関係も含める（推奨）", "Include dependencies (recommended)");
        public static string Recurse => Get("フォルダ内も再帰的に含める", "Recurse into folders");

        public static string Interactive =>
            Get("Interactive（Unityの確認UIを出す）", "Interactive (show Unity confirmation UI)");

        public static string ExportSettingsNote => Get("通常は「依存関係も含める」ONがおすすめです。",
            "Usually, enabling “Include dependencies” is recommended.");

        public static string ActionsHeader => Get("実行", "Actions");
        public static string ExportAllOneByOne => Get("全件を1体ずつExport（連続）", "Export all one-by-one");

        public static string NotPrefabAssetWarning => Get(
            "これはPrefabアセットではない可能性があります（Scene上のオブジェクト等）。Project上のPrefabを指定してください。",
            "This may not be a prefab asset (e.g., a scene object). Please select a prefab asset from Project."
        );

        public static string Exporting => Get("Export中", "Exporting");

        public static string ExportingProgress(string name, int current, int total) =>
            Get($"{name} をExport中... ({current}/{total})", $"Exporting {name}... ({current}/{total})");

        public static string ExportFolderNotSet => Get("Export先フォルダが未設定です。", "Export folder is not set.");
        public static string CannotResolveAssetPath => Get("Assetパスを解決できませんでした。", "Could not resolve asset path.");
        public static string NoValidAvatarSelected => Get("ExportできるPrefabが見つかりません。", "No exportable prefab found.");
        public static string ExportAllComplete => Get("全件のExport処理が完了しました。", "All exports completed.");

        public static string ExcludeFolderEnabled =>
            Get("指定フォルダをExport対象から除外する", "Exclude specified folder from export");

        public static string ExcludeFoldersHeader => Get("除外フォルダ一覧", "Excluded folders");
        public static string ExcludeFolderCount => Get("除外フォルダ数", "Excluded folder count");

        public static string ExcludeFolderElement(int index) =>
            Get($"除外フォルダ {index + 1}", $"Excluded folder {index + 1}");
    }
}