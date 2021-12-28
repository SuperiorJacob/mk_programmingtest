using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Superior.StroopTest
{
    [System.Serializable]
    public struct ButtonInfo
    {
        public Button button;
        public Text text;
    }

    // I would make this a struct for performance, but for dynamic data changing, a class is required.
    public class RoundInfo
    {
        public int index = 0;
        public float speed = 0f;
        public bool won = false;

        public ColorInformation colorInfo;
        public ColorInformation[] options;
    }

    public class Game : MonoBehaviour
    {
        [Header("References")]
        public Settings settings;

        [Space(10f)]
        [SerializeField] private GameObject _roundManager;
        [SerializeField] private GameObject _endStatistics;

        [Space(10f)]
        [SerializeField] private Text _centralText;
        [SerializeField] private Text _speedText;
        [SerializeField] private ButtonInfo[] _roundButtons;

        private int _currentRound = 0;
        private float _gameStartTimer = -1f;
        private bool _started = false;

        private RoundInfo[] _rounds;

        /// <summary>
        /// Get a stack of shuffled stroop colours for easy game run.
        /// </summary>
        public void LoadShuffledColors()
        {
            _rounds = new RoundInfo[settings.stroopColours.Length];

            int roundButtonCount = _roundButtons.Length;

            // Shuffle the colors.
            var arr = settings.stroopColours.OrderBy(n => System.Guid.NewGuid()).ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                // Randomize for the button options.
                var colorOptions = new List<ColorInformation>(settings.stroopColours.OrderBy(n => System.Guid.NewGuid()));

                colorOptions.Remove(arr[i]); // Cleanse the current color info (just to be sure).

                colorOptions = colorOptions.GetRange(0, roundButtonCount - 1); // Get the options based on how many buttons there are.

                colorOptions.Insert(Random.Range(0, roundButtonCount), arr[i]); // Re-insert the color info.

                var randomColor = colorOptions[Random.Range(0, colorOptions.Count - 1)].color;

                // Push colour info into the round stack.
                _rounds[i] = new RoundInfo
                {
                    index = i,
                    speed = 0,
                    colorInfo = new ColorInformation 
                    { 
                        name = arr[i].name, 
                        color = randomColor
                    },
                    options = colorOptions.ToArray()
                };
            }
        }

        /// <summary>
        /// Create a new game and load it.
        /// </summary>
        public void StartGame()
        {
            _endStatistics.gameObject.SetActive(false);
            _roundManager.gameObject.SetActive(true);

            _centralText.font = settings.mainFont;

            _currentRound = 0;
            _gameStartTimer = -1f;
            _started = false;

            LoadShuffledColors();



            StartRound();
        }

        public void NextRound()
        {
            _currentRound++;

            if (_currentRound >= _rounds.Length)
            {
                EndGame();

                return;
            }

            StartRound();
        }

        public void StartRound()
        {
            var round = _rounds[_currentRound];

            float startTime = Time.realtimeSinceStartup;

            for (int i = 0; i < round.options.Length; i++)
            {
                var option = round.options[i];
                var button = _roundButtons[i];

                button.text.text = Localization.Instance.GetLanguage(option.name);

                button.text.color = Color.black;

                // Cleanup
                button.button.onClick.RemoveAllListeners();

                // Add a win listener at runtime.
                button.button.onClick.AddListener(() => 
                {
                    if (_gameStartTimer == -1f)
                    {
                        _started = true;
                        _gameStartTimer = Time.realtimeSinceStartup;
                    }

                    float speed = Time.realtimeSinceStartup - startTime;

                    round.won = round.colorInfo.name == option.name;
                    round.speed = speed;

                    NextRound();
                });
            }

            _centralText.text = Localization.Instance.GetLanguage(round.colorInfo.name);
            _centralText.color = round.colorInfo.color;
        }

        public void EndGame()
        {
            _endStatistics.gameObject.SetActive(true);
            _roundManager.gameObject.SetActive(false);

            _started = false;
        }

        private void Awake()
        {
            // Check if there are settings (very important).
            if (settings == null)
            {
                Debug.LogError("You do not have a settings object defined. The game will not function at all without one.");

                // Possibly nuke the game :)
                Destroy(gameObject);

                return;
            }

            // We don't want to accidentally wipe the saved settings.
            settings = Instantiate(settings);

            // Setup the settings.
            settings.localization.Setup();
        }

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            if (!_started)
                return;

            _speedText.text = _gameStartTimer != -1f ? $"{Localization.Instance.GetLanguage("#speed")}: {Time.realtimeSinceStartup - _gameStartTimer}s" : "";
        }
    }
}
