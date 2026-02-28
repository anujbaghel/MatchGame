using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private RectTransform MainMenuPanel;
    [SerializeField] private TextMeshProUGUI mainButtonText;

    [Header("Save/Load UI")]
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject saveButton;

    public void OpenMainMenu(){
        MainMenuPanel.localScale = Vector3.one;
    }

    public void CloseMainMenu(){
      MainMenuPanel.localScale = Vector3.zero;
    }

    public void ChangeThemeToNextLevel() {
      mainButtonText.text = "Next Level";
    }

    public void ShowContinueButton(bool show) {
        if (continueButton != null) {
            continueButton.SetActive(show);
        }
    }

    public void ShowSaveButton(bool show) {
        if (saveButton != null) {
            saveButton.SetActive(show);
        }
    }
}
