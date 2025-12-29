using NUnit.Framework;
using Fara.FaraVRMMultiExporter.Editor;

namespace Fara.FaraVRMMultiExporter.Tests.Editor
{
    [TestFixture]
    public class LocalizationManagerTests
    {
        private bool _originalIsJapanese;

        [SetUp]
        public void SetUp()
        {
            // テスト開始時の言語設定を退避
            _originalIsJapanese = LocalizationManager.IsJapanese;
        }

        [TearDown]
        public void TearDown()
        {
            // テスト終了時に元の言語設定に戻す
            LocalizationManager.IsJapanese = _originalIsJapanese;
        }

        [Test]
        public void IsJapanese_ToggleTest()
        {
            LocalizationManager.IsJapanese = true;
            Assert.IsTrue(LocalizationManager.IsJapanese);

            LocalizationManager.IsJapanese = false;
            Assert.IsFalse(LocalizationManager.IsJapanese);
        }

        [Test]
        public void Get_ReturnsCorrectLanguage()
        {
            const string jp = "日本語";
            const string en = "English";

            LocalizationManager.IsJapanese = true;
            Assert.AreEqual(jp, LocalizationManager.Get(jp, en));

            LocalizationManager.IsJapanese = false;
            Assert.AreEqual(en, LocalizationManager.Get(jp, en));
        }

        [Test]
        public void L10N_AllProperties_ReturnExpectedValues()
        {
            // --- 日本語の検証 ---
            LocalizationManager.IsJapanese = true;

            Assert.AreEqual("VRMMultiExporter", L10N.WindowTitle);
            Assert.AreEqual("OK", L10N.Ok);
            Assert.AreEqual("エラー", L10N.Error);
            Assert.AreEqual("完了", L10N.Complete);

            Assert.AreEqual("言語 / Language", L10N.LanguageHeader);
            Assert.AreEqual("使い方", L10N.HowToUseHeader);
            StringAssert.Contains("ExportしたいVRM", L10N.Instructions);
            StringAssert.Contains("アバター名.unitypackage", L10N.Instructions);

            Assert.AreEqual("アバター選択", L10N.AvatarSelectionHeader);
            Assert.AreEqual("アバター数", L10N.AvatarCount);
            Assert.AreEqual("VRM 1体目", L10N.AvatarElement(0));
            Assert.AreEqual("VRM 2体目", L10N.AvatarElement(1));

            Assert.AreEqual("Export設定", L10N.ExportSettingsHeader);
            Assert.AreEqual("Export先フォルダ", L10N.ExportFolder);
            Assert.AreEqual("フォルダ選択", L10N.SelectFolder);
            Assert.AreEqual("Export先フォルダを選択", L10N.SelectFolderDialogTitle);

            Assert.AreEqual("依存関係も含める（推奨）", L10N.IncludeDependencies);
            Assert.AreEqual("フォルダ内も再帰的に含める", L10N.Recurse);
            Assert.AreEqual("Interactive（Unityの確認UIを出す）", L10N.Interactive);
            Assert.AreEqual("通常は「依存関係も含める」ONがおすすめです。", L10N.ExportSettingsNote);

            Assert.AreEqual("実行", L10N.ActionsHeader);
            Assert.AreEqual("全件を1体ずつExport（連続）", L10N.ExportAllOneByOne);

            StringAssert.Contains("Prefabアセットではない可能性", L10N.NotPrefabAssetWarning);

            Assert.AreEqual("Export中", L10N.Exporting);
            Assert.AreEqual("TestAvatar をExport中... (1/5)", L10N.ExportingProgress("TestAvatar", 1, 5));

            Assert.AreEqual("Export先フォルダが未設定です。", L10N.ExportFolderNotSet);
            Assert.AreEqual("Assetパスを解決できませんでした。", L10N.CannotResolveAssetPath);
            Assert.AreEqual("ExportできるPrefabが見つかりません。", L10N.NoValidAvatarSelected);
            Assert.AreEqual("全件のExport処理が完了しました。", L10N.ExportAllComplete);

            Assert.AreEqual("指定フォルダをExport対象から除外する", L10N.ExcludeFolderEnabled);
            Assert.AreEqual("除外フォルダ一覧", L10N.ExcludeFoldersHeader);
            Assert.AreEqual("除外フォルダ数", L10N.ExcludeFolderCount);
            Assert.AreEqual("除外フォルダ 1", L10N.ExcludeFolderElement(0));
            Assert.AreEqual("除外フォルダ 3", L10N.ExcludeFolderElement(2));

            // --- 英語の検証 ---
            LocalizationManager.IsJapanese = false;

            Assert.AreEqual("VRMMultiExporter", L10N.WindowTitle);
            Assert.AreEqual("OK", L10N.Ok);
            Assert.AreEqual("Error", L10N.Error);
            Assert.AreEqual("Complete", L10N.Complete);

            Assert.AreEqual("Language / 言語", L10N.LanguageHeader);
            Assert.AreEqual("How to use", L10N.HowToUseHeader);
            StringAssert.Contains("Add VRM prefabs", L10N.Instructions);
            StringAssert.Contains("AvatarName.unitypackage", L10N.Instructions);

            Assert.AreEqual("Avatar selection", L10N.AvatarSelectionHeader);
            Assert.AreEqual("Avatar count", L10N.AvatarCount);
            Assert.AreEqual("VRM #1", L10N.AvatarElement(0));
            Assert.AreEqual("VRM #2", L10N.AvatarElement(1));

            Assert.AreEqual("Export settings", L10N.ExportSettingsHeader);
            Assert.AreEqual("Export folder", L10N.ExportFolder);
            Assert.AreEqual("Select folder", L10N.SelectFolder);
            Assert.AreEqual("Select export folder", L10N.SelectFolderDialogTitle);

            Assert.AreEqual("Include dependencies (recommended)", L10N.IncludeDependencies);
            Assert.AreEqual("Recurse into folders", L10N.Recurse);
            Assert.AreEqual("Interactive (show Unity confirmation UI)", L10N.Interactive);
            Assert.AreEqual("Usually, enabling “Include dependencies” is recommended.", L10N.ExportSettingsNote);

            Assert.AreEqual("Actions", L10N.ActionsHeader);
            Assert.AreEqual("Export all one-by-one", L10N.ExportAllOneByOne);

            StringAssert.Contains("may not be a prefab asset", L10N.NotPrefabAssetWarning);

            Assert.AreEqual("Exporting", L10N.Exporting);
            Assert.AreEqual("Exporting TestAvatar... (1/5)", L10N.ExportingProgress("TestAvatar", 1, 5));

            Assert.AreEqual("Export folder is not set.", L10N.ExportFolderNotSet);
            Assert.AreEqual("Could not resolve asset path.", L10N.CannotResolveAssetPath);
            Assert.AreEqual("No exportable prefab found.", L10N.NoValidAvatarSelected);
            Assert.AreEqual("All exports completed.", L10N.ExportAllComplete);

            Assert.AreEqual("Exclude specified folder from export", L10N.ExcludeFolderEnabled);
            Assert.AreEqual("Excluded folders", L10N.ExcludeFoldersHeader);
            Assert.AreEqual("Excluded folder count", L10N.ExcludeFolderCount);
            Assert.AreEqual("Excluded folder 1", L10N.ExcludeFolderElement(0));
            Assert.AreEqual("Excluded folder 3", L10N.ExcludeFolderElement(2));
        }
    }
}