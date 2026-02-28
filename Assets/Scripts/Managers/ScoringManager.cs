using UnityEngine;
using TMPro;

public class ScoringManager : MonoBehaviour
{
    [SerializeField] private RectTransform ScoringPanel;
    [SerializeField] private int scoreToAdd = 0;
    [SerializeField] private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    private int scoreMultiplier = 1;

    public void ResetScore() {
        score = 0;
        scoreMultiplier = 1;
        scoreText.text = "Score: " + score;
    }

    public void AddScore(int multiplier) {
        score += scoreToAdd * multiplier;
        scoreText.text = "Score: " + score;
    }

    public void OpenPanel(){
      ScoringPanel.localScale = Vector3.one;
    }

    public void ClosePanel(){
      ScoringPanel.localScale = Vector3.zero;
    }
}
