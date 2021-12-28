using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Superior.StroopTest
{
    [System.Serializable]
    public struct ColorInformation
    {
        // Allows editor array index declaration (instead of index 0, 1, 2, it's {name} in the editor).
        [Tooltip("The name of the colour, example: Red.")]
        public string name;

        [Tooltip("The color's representing RGB.")]
        public Color color;
    }

    [CreateAssetMenu(fileName = "New Settings", menuName = "StroopTest/Settings", order = 1)]
    public class Settings : ScriptableObject
    {
        public Localization localization;

        [Header("Fonts"),
            Tooltip("The main font used by everything other then the menu.")]
        public Font mainFont;

        [Tooltip("A font used for most menu items outside of the game.")]
        public Font menuFont;

        public ColorInformation[] stroopColours;
    }
}
