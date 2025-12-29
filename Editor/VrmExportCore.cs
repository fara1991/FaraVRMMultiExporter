using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fara.FaraVRMMultiExporter.Editor
{
    public static class VrmExportCore
    {
        public static void ExportSingle(
            GameObject prefab,
            string exportFolder,
            bool includeDependencies,
            bool recurse,
            bool interactive,
            string[] excludeFolders
        )
        {
            var assetPath = AssetDatabase.GetAssetPath(prefab);
            if (string.IsNullOrEmpty(assetPath)) throw new InvalidOperationException(L10N.CannotResolveAssetPath);
            if (string.IsNullOrEmpty(exportFolder)) throw new InvalidOperationException(L10N.ExportFolderNotSet);

            if (!Directory.Exists(exportFolder)) Directory.CreateDirectory(exportFolder);

            var fileName = SanitizeFileName(prefab.name) + ".unitypackage";
            var savePath = Path.Combine(exportFolder, fileName);

            var exportAssetPaths = CollectExportAssetPaths(assetPath, includeDependencies, excludeFolders);

            var options = ExportPackageOptions.Default;
            if (recurse) options |= ExportPackageOptions.Recurse;
            if (interactive) options |= ExportPackageOptions.Interactive;

            AssetDatabase.ExportPackage(exportAssetPaths, savePath, options);
        }

        private static string[] CollectExportAssetPaths(
            string rootAssetPath,
            bool includeDependencies,
            string[] excludeFolders
        )
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var paths = includeDependencies
                ? AssetDatabase.GetDependencies(rootAssetPath, true)
                : new[] {rootAssetPath};

            var normalizedExcludes = excludeFolders.Select(NormalizeExcludeToken).Where(t => !string.IsNullOrEmpty(t))
                .ToArray();

            return paths
                .Where(p => !string.IsNullOrEmpty(p) && !ShouldExclude(p, normalizedExcludes))
                .Where(p => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(p) != null)
                .Distinct()
                .ToArray();
        }

        private static string NormalizeExcludeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return "";
            var t = token.Trim().Replace('\\', '/');
            if (t.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase)) t = t[7..];
            if (t.StartsWith("Packages/", StringComparison.OrdinalIgnoreCase)) t = t[9..];
            return t.Trim('/');
        }

        private static bool ShouldExclude(string path, string[] tokens) => tokens.Any(path.StartsWith);

        private static string SanitizeFileName(string name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? "VRM"
                : Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_')).Trim();
        }
    }
}