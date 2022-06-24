using UnityEngine;

namespace APMapMod.Util {
    /// <summary>
    /// Class for GameObject extensions.
    /// </summary>
    internal static class TupleExtensions {
        /// <summary>
        /// Find a GameObject with the given name in the children of the given GameObject.
        /// </summary>
        /// <param name="color">Color to convert</param>
        /// <returns>tuple of (R,G,B)</returns>
        public static Color ToColor(
            this (int r, int g, int b) color
        )
        {
            return new Color(color.r / 255f, color.g / 255f, color.b / 255f);
        }
    }
}