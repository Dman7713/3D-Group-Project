using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; // Reference to the UI text

    private void Start()
    {
        UpdateScoreDisplay(MoneyCounter.Instance.TotalScore);
    }

    private void OnEnable()
    {
        MoneyCounter.OnScoreUpdated += UpdateScoreDisplay;
    }

    private void OnDisable()
    {
        MoneyCounter.OnScoreUpdated -= UpdateScoreDisplay;
    }

    private void UpdateScoreDisplay(int newScore)
    {
        scoreText.text = $"Score: {newScore}";
    }
}
