using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; // Referensi ke UI Text untuk skor
    private int score = 0;

    void Start()
    {
        UpdateScoreUI();
    }

    public void IncreaseScore()
    {
        score += 1; // Menambah skor setiap kali musuh dikalahkan
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
