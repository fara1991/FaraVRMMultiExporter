using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fara.FaraVRMMultiExporter.Editor
{
    public class VrmExportSettings : ScriptableObject
    {
        [FormerlySerializedAs("VrmList")] public List<GameObject> vrmList = new();

        [FormerlySerializedAs("ExportFolderPath")]
        public string exportFolderPath = @"E:\Unity\VCC\fara-avatar-vrm\Assets\Fara\Packages";

        [FormerlySerializedAs("IsExcludeFolderEnabled")]
        public bool isExcludeFolderEnabled = true;

        [FormerlySerializedAs("ExcludeFolders")]
        public List<string> excludeFolders = new() {"Packages"};

        [FormerlySerializedAs("IncludeDependencies")]
        public bool includeDependencies = true;

        [FormerlySerializedAs("Recurse")] public bool recurse = true;

        public static VrmExportSettings CreateNew()
        {
            return CreateInstance<VrmExportSettings>();
        }
    }
}