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
    #endregion
    [Header("Player")]
    [SerializeField] private GameObject _player;

    [Header("Game Timer Settings")]
    [SerializeField] private float _gameTime = 180f; // Total game time in seconds (3 minutes)
    [SerializeField] private float _currentTime; // Current time left in the game


    [SerializeField] GameObject _vegetablePrefab;
    [SerializeField] GameObject _cutVegetablePrefab;
    [SerializeField] GameObject _cheesePrefab;
    [SerializeField] GameObject _meatPrefab;
    [SerializeField] GameObject _cookedMeatPrefab;

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
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {




        StartGame();
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerInteracted += OnPlayerInteracted;
    }


    private void OnDisable()
    {
        PlayerController.OnPlayerInteracted -= OnPlayerInteracted;

    }

    // Update is called once per frame
    void Update()
    {

    }


    void StartGame()
    {
        StartCoroutine(StartGameTimer());
    }


    IEnumerator StartGameTimer()
    {
        OnGameStarted?.Invoke(); //Temp Invoke to start the game, can be used to enable player movement and other game logic.

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

    private void OnPlayerInteracted(GameObject @object)
    {
        //throw new NotImplementedException();
    }

    public GameObject GetIngredient(IngredientType ingredientType)
    {
        switch (ingredientType)
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

}