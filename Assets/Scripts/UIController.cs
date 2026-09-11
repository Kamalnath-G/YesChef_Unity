using System;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _gameTimerText; // Reference to the UI text element for displaying the Game time

    #region Unity Functions

    private void Awake()
    {
        #region Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        #endregion


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
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
    private void OnGameOver()
    {
        _gameTimerText.text = "Game Over!"; //Temp
    }

}
