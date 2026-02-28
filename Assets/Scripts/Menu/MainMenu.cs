using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private RectTransform MainMenuPanel;
    [SerializeField] private TextMeshProUGUI mainButtonText;

    public void OpenMainMenu(){
        MainMenuPanel.localScale = Vector3.one;
    }

    public void CloseMainMenu(){
      MainMenuPanel.localScale = Vector3.zero;
    }

    public void ChangeThemeToNextLevel() {
      mainButtonText.text = "Next Level";
    }
}
