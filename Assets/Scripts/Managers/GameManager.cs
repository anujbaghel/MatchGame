using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour, ICardListener
{
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private ScoringManager scoringManager;
    private int consecutiveMatches = 0;
    private int totalMatches = 0;

    private Card[] cardsToMatch = new Card[2];

    private void Start() {
        SoundManager.Instance.PlayBackgroundMusic(SoundEnum.BackgroundMusic);
    }

    public void StartLevel() {
        mainMenu.CloseMainMenu();
        LevelData levelData = LevelManager.Instance.GetCurrentLevelData();
        gridGenerator.GenerateGrid(levelData, this);
        scoringManager.OpenPanel();
        scoringManager.ResetScore();
        ResetGame();
    }

    private void ResetGame() {
        totalMatches = 0;
        consecutiveMatches= 0;
    }

    public void NextLevel() {
        LevelManager.Instance.currentLevel++;
        StartLevel();
    }

    public void OnCardClicked(Card card) {
        if(cardsToMatch[0] == null){
            cardsToMatch[0] = card;
        }
        else{
            cardsToMatch[1] = card;
            StartCoroutine(CheckAndMatchCards());
        }
    }

    public IEnumerator CheckAndMatchCards() {
        Card card1 = cardsToMatch[0];
        Card card2 = cardsToMatch[1];
        ClearCardsToMatch();

        yield return new WaitWhile(() => card1.isFlipping || card2.isFlipping);
        yield return new WaitForSeconds(0.5f);
        
        if(card1.cardProperties.cardNumber == card2.cardProperties.cardNumber){
            consecutiveMatches++;
            totalMatches++;
            card1.OnMatchFound();
            card2.OnMatchFound();
            scoringManager.AddScore(consecutiveMatches);
            SoundManager.Instance.PlaySoundEffect(SoundEnum.MatchFoundSound);
            if(totalMatches == (LevelManager.Instance.GetCurrentLevelData().rows * LevelManager.Instance.GetCurrentLevelData().columns) / 2){
                OnLevelComplete();
                SoundManager.Instance.PlaySoundEffect(SoundEnum.LevelCompleteSound);
            }
        }
        else{
            card1.Flip();
            card2.Flip();
            consecutiveMatches = 0;
        }
    }

    private void ClearCardsToMatch() {
        cardsToMatch[0] = null;
        cardsToMatch[1] = null;
    }

    private void OnLevelComplete() {
        scoringManager.ClosePanel();
        mainMenu.ChangeThemeToNextLevel();
        mainMenu.OpenMainMenu();
        LevelManager.Instance.currentLevel++;
    }
}
