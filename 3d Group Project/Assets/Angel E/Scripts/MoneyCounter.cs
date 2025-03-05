using UnityEngine;
using System;

public class MoneyCounter : MonoBehaviour
{
    public static MoneyCounter Instance { get; private set; }
    public int TotalScore { get; private set; } = 0;

    public static event Action<int> OnScoreUpdated; // Event to update UI

    private void Awake()
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
        TotalScore += points;
        Debug.Log($"🏆 Points Added: {points} | New Total Score: {TotalScore}");

        OnScoreUpdated?.Invoke(TotalScore); // Notify UI
    }
}
