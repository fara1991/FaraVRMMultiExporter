using System.Collections.Generic;
using NUnit.Framework;
using Fara.FaraVRMMultiExporter.Editor;

namespace Fara.FaraVRMMultiExporter.Tests.Editor
{
    [TestFixture]
    public class VrmListUtilityTests
    {
        [Test]
        public void RemoveLastOrSelected_ShouldRemoveAtSpecifiedIndex()
        {
            var list = new List<string> { "Item0", "Item1", "Item2" };
            var focusedIndex = 1;

            VrmListUtility.RemoveLastOrSelected(list, ref focusedIndex);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Item0", list[0]);
            Assert.AreEqual("Item2", list[1]);
            Assert.AreEqual(-1, focusedIndex, "削除後はフォーカスがリセットされること");
        }

        [Test]
        public void RemoveLastOrSelected_ShouldRemoveEmptyString_WhenNoIndexFocused()
        {
            var list = new List<string> { "Valid", "", "Valid2" };
            var focusedIndex = -1;

            VrmListUtility.RemoveLastOrSelected(list, ref focusedIndex);

            Assert.AreEqual(2, list.Count);
            Assert.IsFalse(list.Contains(""), "空文字の要素が優先的に削除されること");
        }

        [Test]
        public void RemoveLastOrSelected_ShouldRemoveLastElement_WhenNoEmptyElementsFound()
        {
            var list = new List<string> { "A", "B" };
            var focusedIndex = -1;

            VrmListUtility.RemoveLastOrSelected(list, ref focusedIndex);

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("A", list[0]);
        }
    }
}