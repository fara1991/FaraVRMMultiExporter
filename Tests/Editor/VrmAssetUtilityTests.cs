using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.IO;
using Fara.FaraVRMMultiExporter.Editor;

namespace Fara.FaraVRMMultiExporter.Tests.Editor
{
    [TestFixture]
    public class VrmAssetUtilityTests
    {
        private const string TestPrefabPath = "Assets/TestVrmAssetUtility.prefab";
        private GameObject _testPrefabAsset;

        [SetUp]
        public void SetUp()
        {
            // テスト用のPrefabアセットを作成
            var go = new GameObject("TestPrefab");
            _testPrefabAsset = PrefabUtility.SaveAsPrefabAsset(go, TestPrefabPath);
            Object.DestroyImmediate(go);
        }

        [TearDown]
        public void TearDown()
        {
            // テスト用のアセットを削除
            AssetDatabase.DeleteAsset(TestPrefabPath);
        }

        [Test]
        public void IsPrefabAsset_VariousInputs_ReturnsExpected()
        {
            // 1. Prefabアセットの場合
            Assert.IsTrue(VrmAssetUtility.IsPrefabAsset(_testPrefabAsset));

            // 2. シーン上のオブジェクトの場合
            var sceneObj = new GameObject("SceneObj");
            Assert.IsFalse(VrmAssetUtility.IsPrefabAsset(sceneObj));
            Object.DestroyImmediate(sceneObj);

            // 3. Nullの場合
            Assert.IsFalse(VrmAssetUtility.IsPrefabAsset(null));
        }

        [Test]
        public void GetProjectRoot_ReturnsCorrectDirectory()
        {
            var root = VrmAssetUtility.GetProjectRoot();
            
            // ルートディレクトリに "Assets" フォルダが存在することを確認
            Assert.IsTrue(Directory.Exists(Path.Combine(root, "Assets")));
            
            // dataPathの親フォルダと一致するか確認
            var expected = Directory.GetParent(Application.dataPath)!.FullName;
            Assert.AreEqual(expected, root);
        }

        [Test]
        public void GetDroppedVrmPrefabs_FiltersCorrectly()
        {
            var sceneObj = new GameObject("SceneObj");
            var material = new Material(Shader.Find("Standard"));
            
            // Prefab, シーンオブジェクト, 別タイプのアセットを混ぜたリスト
            Object[] inputs = { _testPrefabAsset, sceneObj, material, null };

            var results = VrmAssetUtility.GetDroppedVrmPrefabs(inputs);

            // Prefabアセットのみが抽出されているはず
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(_testPrefabAsset, results[0]);

            Object.DestroyImmediate(sceneObj);
            Object.DestroyImmediate(material);
        }
    }
}