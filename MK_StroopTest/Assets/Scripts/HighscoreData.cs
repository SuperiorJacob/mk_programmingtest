using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Superior.StroopTest
{
    // Found here: https://www.raywenderlich.com/418-how-to-save-and-load-a-game-in-unity
    // I didn't use a class, we love micro-optimization.

    [System.Serializable]
    public struct HighscoreData
    {
        public int correct;
        public float speed;

        /// <summary>
        /// Save all of the highscore information to the local data storage.
        /// </summary>
        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/highscore.save");

            bf.Serialize(file, this);

            file.Close();
        }

        /// <summary>
        /// Load all of the highscore information from the local data storage.
        /// </summary>
        public void Load()
        {
            string path = Application.persistentDataPath + "/highscore.save";

            if (!File.Exists(path))
                return;

            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(path, FileMode.Open);

            this = (HighscoreData)bf.Deserialize(file);

            file.Close();
        }
    }
}
