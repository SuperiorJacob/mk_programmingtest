using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Superior.StroopTest
{
    public static class Utility
    {
        public static void SetFonts(Font font, params UnityEngine.UI.Text[] texts)
        {
            for (int i = 0; i < texts.Length; i++)
                texts[i].font = font;
        }
    }
}
