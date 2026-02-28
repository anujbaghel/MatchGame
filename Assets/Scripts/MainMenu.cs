using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private RectTransform MainMenuPanel;

    public void OpenMainMenu(){
        MainMenuPanel.localScale = Vector3.one;
    }

    public void CloseMainMenu(){
      MainMenuPanel.localScale = Vector3.zero;
    }
    
}
