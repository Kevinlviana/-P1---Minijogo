using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public TextMeshProUGUI scoreText;

    public Image[] hearts;
    public Sprite heartFull;
    public Sprite heartEmpty;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Pontos: " + score;
    }

    public void UpdateHearts(int current, int max)
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].sprite = (i < current) ? heartFull : heartEmpty;
    }
}