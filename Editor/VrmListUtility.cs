using System.Collections.Generic;

namespace Fara.FaraVRMMultiExporter.Editor
{
    public static class VrmListUtility
    {
        /// <summary>
        /// フォーカスされたインデックス、または末尾の無効な要素を削除する
        /// </summary>
        public static void RemoveLastOrSelected<T>(List<T> list, ref int lastFocusedIndex) where T : class
        {
            if (list.Count == 0) return;

            if (lastFocusedIndex >= 0 && lastFocusedIndex < list.Count)
            {
                list.RemoveAt(lastFocusedIndex);
                lastFocusedIndex = -1;
                return;
            }

            // 無効な要素（nullや空文字）を優先して削除
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] is string s && !string.IsNullOrWhiteSpace(s)) continue;
                if (list[i] is not null && list[i] is not string) continue;

                list.RemoveAt(i);
                return;
            }

            list.RemoveAt(list.Count - 1);
        }
    }
}