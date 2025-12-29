using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.IO;
using Fara.FaraVRMMultiExporter.Editor;

namespace Fara.FaraVRMMultiExporter.Tests.Editor
{
    [TestFixture]
    public class VrmExportSettingsTests
    {
        [Test]
        public void CreateNew_ReturnsInstanceWithDefaultValues()
        {
            var settings = VrmExportSettings.CreateNew();

            Assert.IsNotNull(settings);
            Assert.IsNotNull(settings.vrmList);
            Assert.IsTrue(settings.isExcludeFolderEnabled);
            Assert.Contains("Packages", settings.excludeFolders);
            Assert.IsTrue(settings.includeDependencies);
            Assert.IsTrue(settings.recurse);

            Object.DestroyImmediate(settings);
        }

        [Test]
        public void Settings_DataCanBeModified()
        {
            var settings = VrmExportSettings.CreateNew();
            
            settings.exportFolderPath = "Assets/Test";
            settings.includeDependencies = false;
            
            Assert.AreEqual("Assets/Test", settings.exportFolderPath);
            Assert.IsFalse(settings.includeDependencies);

            Object.DestroyImmediate(settings);
        }
    }
}