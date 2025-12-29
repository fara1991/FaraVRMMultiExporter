using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using Fara.FaraVRMMultiExporter.Editor;

namespace Fara.FaraVRMMultiExporter.Tests.Editor
{
    [TestFixture]
    public class VrmExportCoreTests
    {
        private const string TestPrefabPath = "Assets/TestExportCore.prefab";
        private const string ExportDirPath = "TestExportOutput";
        private GameObject _testPrefab;

        [SetUp]
        public void SetUp()
        {
            var go = new GameObject("Test:Prefab/WithInvalidChars");
            _testPrefab = PrefabUtility.SaveAsPrefabAsset(go, TestPrefabPath);
            UnityEngine.Object.DestroyImmediate(go);
            
            if (Directory.Exists(ExportDirPath)) Directory.Delete(ExportDirPath, true);
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(TestPrefabPath);
            if (Directory.Exists(ExportDirPath)) Directory.Delete(ExportDirPath, true);
        }

        [Test]
        public void ExportSingle_ThrowsWhenAssetPathInvalid()
        {
            // アセットではない（保存されていない）GameObjectを渡す
            var tempGo = new GameObject("NotAnAsset");
            Assert.Throws<InvalidOperationException>(() => 
                VrmExportCore.ExportSingle(tempGo, ExportDirPath, false, false, false, new string[0]));
            UnityEngine.Object.DestroyImmediate(tempGo);
        }

        [Test]
        public void ExportSingle_ThrowsWhenExportFolderEmpty()
        {
            Assert.Throws<InvalidOperationException>(() => 
                VrmExportCore.ExportSingle(_testPrefab, "", false, false, false, new string[0]));
        }

        [Test]
        public void ExportSingle_CreatesDirectoryAndExecutes()
        {
            // 正常系：ディレクトリ作成、サニタイズされたファイル名での書き出し
            // AssetDatabase.ExportPackage は実際には.unitypackageを作成しますが、
            // ユニットテスト環境では副作用を最小限にするため、パスが通ることを確認します。
            Assert.DoesNotThrow(() => 
                VrmExportCore.ExportSingle(_testPrefab, ExportDirPath, false, false, false, new string[0]));
            
            Assert.IsTrue(Directory.Exists(ExportDirPath));
        }

        // --- プライベートメソッドの挙動を間接的にテスト ---

        [Test]
        public void ExportSingle_WithExclusions_HandlesVariousTokens()
        {
            // NormalizeExcludeToken のテストを兼ねる
            // "Assets/Fara" や "\Fara\" など、不揃いな入力を正規化して除外できるか
            string[] excludes = { "Assets/Fara", "Packages\\com.unity", "   /OtherFolder/  " };
            
            // 例外が出ずに実行できることを確認
            Assert.DoesNotThrow(() => 
                VrmExportCore.ExportSingle(_testPrefab, ExportDirPath, true, true, false, excludes));
        }

        [Test]
        public void SanitizeFileName_LogicTest()
        {
            // privateメソッドをリフレクションで取得
            var methodInfo = typeof(VrmExportCore).GetMethod("SanitizeFileName", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            // 1. 不正な文字（: や / など）が含まれる場合
            var input1 = "Test:Prefab/With?Chars";
            var result1 = (string)methodInfo!.Invoke(null, new object[] { input1 });
            Assert.AreEqual("Test_Prefab_With_Chars", result1, "不正な文字がアンダースコアに置換されること");

            // 2. 空文字や空白の場合
            var result2 = (string)methodInfo.Invoke(null, new object[] { "" });
            Assert.AreEqual("VRM", result2, "空文字の場合はデフォルト名 'VRM' になること");

            // 3. 前後の空白がトリムされるか
            const string input3 = "  My Avatar  ";
            var result3 = (string)methodInfo.Invoke(null, new object[] { input3 });
            Assert.AreEqual("My Avatar", result3, "前後の空白がトリムされること");
        }
    }
}