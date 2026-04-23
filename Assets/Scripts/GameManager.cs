using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Condições de vitória")]
    public int totalEnemies = 3;
    public int totalCoins = 16;

    private int score = 0;
    private int enemiesKilled = 0;
    private int coinsCollected = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UIManager.Instance.UpdateScore(score);
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        AddScore(50);

        if (enemiesKilled >= totalEnemies)
            LevelComplete();
    }

    public void CoinCollected()
    {
        coinsCollected++;
        AddScore(10);

        if (coinsCollected >= totalCoins)
            LevelComplete();
    }

    public void LevelComplete()
    {
        Invoke(nameof(LoadNextLevel), 1f);
    }

    public void GameOver()
    {
        Invoke(nameof(LoadGameOver), 0.5f);
    }

    public void RestartGame()
    {
        enemiesKilled = 0;
        coinsCollected = 0;
        score = 0;
        SceneManager.LoadScene("Level01");
    }

    void LoadGameOver() => SceneManager.LoadScene("GameOver");
    void LoadNextLevel() => SceneManager.LoadScene("MainMenu");
}