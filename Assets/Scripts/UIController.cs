using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI _gameTimerText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _gameUIQuitButton;

    [Header("MainMenu Panel")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _mainMenuQuitButton;
    [SerializeField] private TextMeshProUGUI _mainMenuHighScoreText;

    [Header("Pause Panel")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _pausePanelQuitButton;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Button _gameOverRestartButton;
    [SerializeField] private Button _gameOverQuitButton;
    [SerializeField] private TextMeshProUGUI _gameOverScoreText;
    [SerializeField] private TextMeshProUGUI _gameOverHighScoreText;

    #region Unity Functions

    private void Awake()
    {
        #region Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        #endregion

        _mainMenuPanel.SetActive(true);
        _pausePanel.SetActive(false);
        _gameOverPanel.SetActive(false);

        //Game UI Buttons
        _pauseButton.onClick.AddListener(OnPauseButtonClicked);
        _gameUIQuitButton.onClick.AddListener(OnQuitButtonClicked);

        //Main Menu Panel Buttons
        _playButton.onClick.AddListener(OnPlayButtonClicked);
        _mainMenuQuitButton.onClick.AddListener(OnQuitButtonClicked);

        //Pause Menu Panel Buttons
        _resumeButton.onClick.AddListener(OnResumeButtonClicked);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
        _pausePanelQuitButton.onClick.AddListener(OnQuitButtonClicked);

        //Game Over Panel Buttons
        _gameOverRestartButton.onClick.AddListener(OnRestartButtonClicked);
        _gameOverQuitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnEnable()
    {
        GameManager.OnScoreChanged += OnScoreChanged;
        GameManager.OnGameTimeChanged += UpdateGameTimer;
        GameManager.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameTimeChanged -= UpdateGameTimer;
        GameManager.OnGameOver -= OnGameOver;
    }

    #endregion

    private void UpdateGameTimer(float m_CurrentTime)
    {
        _gameTimerText.text = $"{TimeSpan.FromSeconds(m_CurrentTime).ToString(@"m\:ss")}";
    }

    private void OnScoreChanged(int m_score)
    {
        _scoreText.text = "Score: " + m_score; //Temp
    }

    private void OnGameOver()
    {
        _gameTimerText.text = "Game Over!"; //Temp
    }

    private void OnPlayButtonClicked()
    {
        _mainMenuPanel.gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }
    private void OnPauseButtonClicked()
    {
        Time.timeScale = 0f;
    }

    private void OnResumeButtonClicked()
    {
        Time.timeScale = 1f;
    }
    private void OnRestartButtonClicked()
    {
        GameManager.Instance.RestartGame();
    }


    private void OnQuitButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }

}
