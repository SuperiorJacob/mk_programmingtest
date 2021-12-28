using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Superior.StroopTest
{
    [System.Serializable]
    public struct Language
    {
        // Language definition
        public string name;

        // Translate too
        public string translation;
    }

    [CreateAssetMenu(fileName = "New Localization", menuName = "StroopTest/Localization", order = 2)]
    public class Localization : ScriptableObject
    {
        public static Localization Instance;

        // The current language used in the localization.
        public string languageName = "English";

        // Array the language rules so we can support different languages.
        public Language[] definitions;

        private Dictionary<string, string> _languages;

        /// <summary>
        /// Setup the languages from settings.
        /// </summary>
        public void Setup()
        {
            if (string.IsNullOrEmpty(languageName))
                languageName = "English";

            _languages = new Dictionary<string, string>();

            LoadLanguages();

            Instance = this;
        }

        /// <summary>
        /// Load all the localization information into the language look up table.
        /// </summary>
        public void LoadLanguages()
        {
            foreach (Language localization in definitions)
            {
                _languages[localization.name] = localization.translation;
            }
        }

        /// <summary>
        /// Grab the localization definition.
        /// </summary>
        /// <param name="definition">The definition to query.</param>
        /// <returns>The found definition or the definition on failure.</returns>
        public string GetLanguage(string definition)
        {
            return _languages.TryGetValue(definition, out var translation) 
                ? translation : definition;
        }
    }
}
