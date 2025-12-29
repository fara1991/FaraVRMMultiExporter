using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fara.FaraVRMMultiExporter.Editor
{
    public static class VrmAssetUtility
    {
        public static bool IsPrefabAsset(GameObject go)
        {
            if (!go) return false;
            var type = PrefabUtility.GetPrefabAssetType(go);
            return type != PrefabAssetType.NotAPrefab;
        }

        public static string GetProjectRoot()
        {
            return Directory.GetParent(Application.dataPath)?.FullName ?? Environment.CurrentDirectory;
        }

        /// <summary>
        /// ドラッグ＆ドロップされたオブジェクトから有効なGameObjectを抽出する
        /// </summary>
        public static List<GameObject> GetDroppedVrmPrefabs(UnityEngine.Object[] objects)
        {
            return objects
                .OfType<GameObject>()
                .Where(obj => obj && IsPrefabAsset(obj))
                .ToList();
        }
    }
}