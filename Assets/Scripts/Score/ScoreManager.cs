using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    private const string SCORE_KEY = "HighScores";
    private const int MAX_HIGH_SCORES = 3;

    [SerializeField] private PlayerScore[] highScores;

    private void Awake()
    {
        LoadHighScores();
    }

    private void LoadHighScores()
    {
        string jsonString = PlayerPrefs.GetString(SCORE_KEY);
        if (!string.IsNullOrEmpty(jsonString))
        {
            highScores = JsonUtility.FromJson<PlayerScore[]>(jsonString);
        }
        else
        {
            highScores = new PlayerScore[MAX_HIGH_SCORES];
        }
    }

    public void SaveHighScore(string playerName, int score)
    {
        // Create a new PlayerScore object
        PlayerScore newScore = new PlayerScore();
        newScore.playerName = playerName;
        newScore.score = score;

        // Insert the new score into the list of high scores
        List<PlayerScore> scoreList = new List<PlayerScore>(highScores);
        scoreList.Add(newScore);

        // Sort the list by score in descending order
        scoreList.Sort((x, y) => y.score.CompareTo(x.score));

        // Keep only the top MAX_HIGH_SCORES scores
        scoreList = scoreList.GetRange(0, Mathf.Min(scoreList.Count, MAX_HIGH_SCORES));

        // Update the highScores array
        highScores = scoreList.ToArray();

        // Save the high scores back to PlayerPrefs
        string jsonString = JsonUtility.ToJson(highScores);
        PlayerPrefs.SetString(SCORE_KEY, jsonString);
        PlayerPrefs.Save();
    }

    public PlayerScore[] GetHighScores()
    {
        return highScores;
    }
}
