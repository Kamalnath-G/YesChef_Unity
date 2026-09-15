using System;
using System.Collections;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region Events
    public static event Action OnGameStarted;
    public static event Action OnGameOver;
    public static event Action<float> OnGameTimeChanged;
    public static event Action<int> OnScoreChanged;
    #endregion

    [Header("Player & Other Managers")]
    [SerializeField] private PlayerController _player;
    [SerializeField] private OrderManager _orderManager;
    [SerializeField] private Stove _stove;
    [SerializeField] private Table _table;
    [SerializeField] private Fridge _fridge;

    [Header("Game Timer Settings")]
    [SerializeField] private float _gameTime = 180f; // Total game time in seconds (3 minutes)
    [SerializeField] private float _currentTime; // Current time left in the game

    [Header("Score Settings")]
    private int _score = 0;

    [Header("Ingredient Prefabs")]
    [SerializeField] private GameObject _vegetablePrefab;
    [SerializeField] private GameObject _cutVegetablePrefab;
    [SerializeField] private GameObject _cheesePrefab;
    [SerializeField] private GameObject _meatPrefab;
    [SerializeField] private GameObject _cookedMeatPrefab;

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

#if UNITY_EDITOR
        _gameTime = 30f;
#else
        _gameTime = 180f;
#endif
    }

    public void StartGame()
    {
        _score = 0;

        OnGameStarted?.Invoke();
        StartCoroutine(StartGameTimer());
    }

    private IEnumerator StartGameTimer()
    {
        _currentTime = _gameTime;
        while (_currentTime > 0)
        {
            OnGameTimeChanged?.Invoke(_currentTime);

            yield return new WaitForSeconds(1f);
            _currentTime -= 1f;
        }

        //Game Over Logic
        OnGameOver?.Invoke();
    }

    public void UpdateScore(int m_currentOrderScore)
    {
        _score += m_currentOrderScore;
        OnScoreChanged?.Invoke(_score);
    }

    public int GetScore()
    {
        return _score;
    }

    public GameObject GetIngredient(IngredientType m_ingredientType)
    {
        switch (m_ingredientType)
        {
            case IngredientType.Vegetables:
                return _vegetablePrefab;
            case IngredientType.CutVegetables:
                return _cutVegetablePrefab;
            case IngredientType.Cheese:
                return _cheesePrefab;
            case IngredientType.CookedMeat:
                return _cookedMeatPrefab;
            case IngredientType.Meat:
            default:
                return _meatPrefab;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        StopAllCoroutines();

        _score = 0;
        _currentTime = _gameTime;

        ResetGameplay();

        OnScoreChanged?.Invoke(_score);
        OnGameTimeChanged?.Invoke(_currentTime);

        StartGame();
    }
    private void ResetGameplay()
    {
        _player.ResetPlayer();
        _orderManager.ResetOrders();
        _stove.ResetStoves();
        _fridge.ResetFridge();
        _table.ResetTable();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void StartActionWithDelay(float m_duration, Action m_onTimerComplete)
    {
        StartCoroutine(StartActionWithDelayRoutine(m_duration, m_onTimerComplete));
    }

    private IEnumerator StartActionWithDelayRoutine(float m_duration = 1, Action m_onTimerComplete = null)
    {
        yield return new WaitForSeconds(m_duration);

        m_onTimerComplete?.Invoke();
    }
}