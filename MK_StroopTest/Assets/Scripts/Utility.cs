using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Superior.StroopTest
{
    public static class Utility
    {
        /// <summary>
        /// A helper function used to easily set a mass amount of fonts at runtime.
        /// </summary>
        public static void SetFonts(Font font, params UnityEngine.UI.Text[] texts)
        {
            for (int i = 0; i < texts.Length; i++)
                texts[i].font = font;
        }

        /// <summary>
        /// A helper function to remove all listeners from a bunch of buttons.
        /// </summary>
        public static void RemoveListeners(params ButtonInfo[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
                buttons[i].button.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// A helper function to set the activity of a bunch of objects.
        /// </summary>
        public static void SetActive(bool active, params GameObject[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
                objects[i].SetActive(active);
        }
    }
}
