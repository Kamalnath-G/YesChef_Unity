using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string HIGH_SCORE_KEY = "HighScore";

    #region Singleton
    public static SaveManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion


    public void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

        int _score = GameManager.Instance.GetScore();
        if (_score > highScore)
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, _score);
            PlayerPrefs.Save();
        }
    }

    public int LoadHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

}