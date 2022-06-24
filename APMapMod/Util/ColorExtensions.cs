using UnityEngine;

namespace APMapMod.Util {
    /// <summary>
    /// Class for GameObject extensions.
    /// </summary>
    internal static class ColorExtensions {
        /// <summary>
        /// Find a GameObject with the given name in the children of the given GameObject.
        /// </summary>
        /// <param name="color">Color to convert</param>
        /// <returns>tuple of (R,G,B)</returns>
        public static (int r, int g, int b) ToTuple(
            this Color color
        )
        {
            return (Mathf.RoundToInt(color.r*255), Mathf.RoundToInt(color.g*255), Mathf.RoundToInt(color.b*255));
        }
    }
}