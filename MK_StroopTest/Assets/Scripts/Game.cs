using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Superior.StroopTest
{
    /// <summary>
    /// Simplified data object to lesser file lines.
    /// </summary>
    [System.Serializable]
    public struct ButtonInfo
    {
        public Button button;
        public Text text;
    }

    // I would make this a struct for performance, but for dynamic data changing, a class is better.
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

        [Header("Home Menu")]
        [SerializeField] private GameObject _homeMenu;
        [SerializeField] private Text _homeTitle;
        [SerializeField] private Text _homeTitleBottom;
        [SerializeField] private ButtonInfo _homeStart;
        [SerializeField] private ButtonInfo _homeQuit;

        [Header("Rounds")]
        [SerializeField] private GameObject _roundManager;
        [SerializeField] private Text _centralText;
        [SerializeField] private Text _speedText;
        [SerializeField] private Text _highscoreSpeedText;
        [SerializeField] private ButtonInfo[] _roundButtons;

        [Header("End Game")]
        [SerializeField] private GameObject _endGame;
        [SerializeField] private Text _speed;
        [SerializeField] private Text _correctText;
        [SerializeField] private Text _successText;
        [SerializeField] private ButtonInfo _endRestartButton;
        [SerializeField] private ButtonInfo _endHomeButton;

        [Header("Pause Menu")]
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private Text _pauseTitle;
        [SerializeField] private ButtonInfo _homeButton;
        [SerializeField] private ButtonInfo _restartButton;
        [SerializeField] private ButtonInfo _resumeButton;

        private int _currentRound = 0;
        private float _gameStartTimer = -1f;
        private bool _started = false;
        private int _correct = 0;
        private bool _paused = false;
        private float _pauseTime = 0f;

        private HighscoreData _highscore;
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

                // Generate a random colour name from the available options.
                var randomName = colorOptions[Random.Range(0, colorOptions.Count - 1)].name;

                // Push colour info into the round stack.
                _rounds[i] = new RoundInfo
                {
                    index = i,
                    speed = 0,
                    colorInfo = new ColorInformation 
                    { 
                        name = randomName, 
                        color = arr[i].color
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
            ResetGame();

            LoadShuffledColors();

            StartRound();
        }

        /// <summary>
        /// Reset the game memory and data.
        /// </summary>
        public void ResetGame()
        {
            _currentRound = 0;
            _correct = 0;
            _gameStartTimer = -1f;
            _started = false;
            _paused = false;

            List<Text> texts = new List<Text>() { _centralText, _speedText, _correctText, _speed, _pauseTitle, _highscoreSpeedText, _successText,
                _homeButton.text, _restartButton.text, _resumeButton.text, _endRestartButton.text, _endHomeButton.text };

            for (int i = 0; i < _roundButtons.Length; i++)
                texts.Add(_roundButtons[i].text);

            Utility.SetFonts(settings.mainFont, texts.ToArray());
            SetupText();

            Utility.SetActive(false, _endGame, _pauseMenu, _homeMenu);
            _roundManager.SetActive(true);

            Utility.RemoveListeners(_restartButton, _homeButton, _resumeButton);
            SetupButtons();
        }

        /// <summary>
        /// Pause/Unpause the game and resume proccessing.
        /// </summary>
        public void Pause(bool shouldPause)
        {
            _paused = shouldPause;
            _pauseMenu.SetActive(_paused);

            if (_paused)
            {
                // Save the time stamp.
                _pauseTime = Time.realtimeSinceStartup - _gameStartTimer;
            }
            else
            {
                // When we pause our timer gets distorted as in theory it doesn't pause, so lets "simulate"
                // the old time stamp onto the current time to give the same value.
                _gameStartTimer = Time.realtimeSinceStartup - _pauseTime;

                if (_gameStartTimer < 0)
                    _gameStartTimer = -1f;
            }
        }

        /// <summary>
        /// Setup the text languages.
        /// </summary>
        public void SetupText()
        {
            _pauseTitle.text = Localization.Instance.GetLanguage("#pause");
            _homeButton.text.text = Localization.Instance.GetLanguage("#home");
            _restartButton.text.text = Localization.Instance.GetLanguage("#restart");
            _resumeButton.text.text = Localization.Instance.GetLanguage("#resume");

            _endRestartButton.text.text = Localization.Instance.GetLanguage("#restart");
            _endHomeButton.text.text = Localization.Instance.GetLanguage("#home");

            _highscoreSpeedText.text = _highscore.speed > 0 ? $"{Localization.Instance.GetLanguage("#highscore")} {_highscore.speed}s" : "";
            _speedText.text = Localization.Instance.GetLanguage("#speed");

            _successText.text = "";
        }

        /// <summary>
        /// Setup the button listeners.
        /// </summary>
        public void SetupButtons()
        {
            _resumeButton.button.onClick.AddListener(() => Pause(false));
            _restartButton.button.onClick.AddListener(() => StartGame());
            _homeButton.button.onClick.AddListener(() => HomeMenu());

            _endRestartButton.button.onClick.AddListener(() => StartGame());
            _endHomeButton.button.onClick.AddListener(() => HomeMenu());
        }

        /// <summary>
        /// Load the home menu.
        /// </summary>
        public void HomeMenu()
        {
            ResetGame();

            _started = false;

            _roundManager.SetActive(false);

            _homeMenu.SetActive(true);

            Utility.SetFonts(settings.mainFont, new Text[] { _homeTitle, _homeTitleBottom, _homeStart.text, _homeQuit.text });

            _homeQuit.text.text = Localization.Instance.GetLanguage("#quit");
            _homeStart.text.text = Localization.Instance.GetLanguage("#start");

            _homeTitle.text = Localization.Instance.GetLanguage("#main-title");
            _homeTitleBottom.text = Localization.Instance.GetLanguage("#main-author");

            Utility.RemoveListeners(_homeStart, _homeQuit);

            _homeStart.button.onClick.AddListener(() => StartGame());
            _homeQuit.button.onClick.AddListener(() => Application.Quit());
        }

        /// <summary>
        /// Goes to the next round in queue.
        /// </summary>
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

        /// <summary>
        /// Used to start the current round.
        /// </summary>
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
                button.button.onClick.AddListener(() => ColorClickEvent(round, startTime, option));
            }

            // Setup the central round text.
            _centralText.text = Localization.Instance.GetLanguage(round.colorInfo.name);
            _centralText.color = round.colorInfo.color;
        }

        /// <summary>
        /// Used when clicking the correct color in a round.
        /// </summary>
        public void ColorClickEvent(RoundInfo round, float startTime, ColorInformation option)
        {
            if (_paused)
                return;

            if (_gameStartTimer == -1f)
            {
                _started = true;
                _gameStartTimer = Time.realtimeSinceStartup;
            }

            float speed = Time.realtimeSinceStartup - startTime;

            round.won = round.colorInfo.color == option.color;

            if (round.won)
                _correct++;

            round.speed = speed;

            NextRound();
        }

        /// <summary>
        /// End game screen, called when the game is over.
        /// </summary>
        public void EndGame()
        {
            _endGame.gameObject.SetActive(true);
            _roundManager.gameObject.SetActive(false);

            _started = false;

            _speed.text = _speedText.text;
            _correctText.text = $"{Localization.Instance.GetLanguage("#correct")} {_correct}/{_rounds.Length}";

            float endTime = Time.realtimeSinceStartup - _gameStartTimer;
            bool won = _correct == _rounds.Length;
            bool newHighscore = won && endTime < _highscore.speed;

            _successText.text = Localization.Instance.GetLanguage(
                newHighscore ? "#new-highscore" : (won ? "#winner" : "#loser"));

            // Save the new highscore (if it is a highscore).
            // The reason we do this at the end of the scope is because it is quite expensive.

            if (newHighscore)
            {
                _highscore.correct = _correct;
                _highscore.speed = endTime;

                _highscore.Save();
            }
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

            // Load highscore data
            _highscore = new HighscoreData();
            _highscore.Load();
        }

        private void Start()
        {
            HomeMenu();
        }

        private void Update()
        {
            // Pause menu setup.
            if (Input.GetKeyDown(settings.pauseKey))
                Pause(!_paused);

            // Fun rainbow title screen effect.
            if (_homeMenu.activeInHierarchy)
                _homeTitle.color = Color.HSVToRGB(Mathf.PingPong(Time.time * 0.1f, 1), 1, 1);

            // Only update speed text when were playing the game and not paused.
            if (_started && !_paused)
                _speedText.text = _gameStartTimer != -1f ? $"{Localization.Instance.GetLanguage("#speed")} {Time.realtimeSinceStartup - _gameStartTimer}s" : "";
        }
    }
}
