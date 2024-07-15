using UnityEngine;
using TMPro;

public class ScoreboardManager : MonoBehaviour
{
    public TextMeshProUGUI latestScoreText; // Điểm mới nhất


    void Start()
    {
        // Load and display the latest score
        int currentScore = PlayerPrefs.GetInt("PlayerScore", 0);
        UpdateScoreText(currentScore);


    }

    public void UpdateScoreText(int score)
    {
        if (latestScoreText != null)
        {
            latestScoreText.text = " Score: " + score;
        }
        else
        {
            Debug.LogError("LatestScoreText is not assigned in the Inspector.");
        }
    }
}