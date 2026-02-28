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
        if (LevelManager.Instance.HasSaveFile()) {
            mainMenu.ShowContinueButton(true);
        }
    }

    public void StartLevel() {
        mainMenu.CloseMainMenu();
        LevelData levelData = LevelManager.Instance.GetCurrentLevelData();
        gridGenerator.GenerateGrid(levelData, this);
        scoringManager.OpenPanel();
        scoringManager.ResetScore();
        ResetGame();
        LevelManager.Instance.DeleteSave();
        mainMenu.ShowContinueButton(false);
    }

    private void ResetGame() {
        totalMatches = 0;
        consecutiveMatches = 0;
    }

    public void NextLevel() {
        LevelManager.Instance.currentLevel++;
        LevelManager.Instance.SaveLevelProgress();
        StartLevel();
    }

    public void SaveGame() {
        GameSaveData saveData = new GameSaveData
        {
            currentLevel = LevelManager.Instance.currentLevel,
            rows = gridGenerator.GetRows(),
            columns = gridGenerator.GetColumns(),
            score = scoringManager.GetScore(),
            consecutiveMatches = this.consecutiveMatches,
            totalMatches = this.totalMatches,
            cards = gridGenerator.GetCardStates()
        };

        LevelManager.Instance.SaveGame(saveData);
        mainMenu.ShowContinueButton(true);
    }


    public void LoadGame() {
        GameSaveData saveData = LevelManager.Instance.LoadGame();
        if (saveData == null) {
            Debug.LogWarning("[GameManager] No save data to load.");
            return;
        }

        mainMenu.CloseMainMenu();

        LevelManager.Instance.currentLevel = saveData.currentLevel;

        CardProperties[] cardData = gridGenerator.BuildCardDataFromSave(saveData);
        bool[] matchedStates = gridGenerator.BuildMatchedStatesFromSave(saveData);
        gridGenerator.GenerateGrid(saveData.rows, saveData.columns, cardData, this, matchedStates);

        scoringManager.OpenPanel();
        scoringManager.SetScore(saveData.score);
        consecutiveMatches = saveData.consecutiveMatches;
        totalMatches = saveData.totalMatches;

        Debug.Log($"[GameManager] Game loaded — Level {saveData.currentLevel}, Score {saveData.score}, Matches {saveData.totalMatches}");
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
            SaveGame();
            if(totalMatches == (LevelManager.Instance.GetCurrentLevelData().rows * LevelManager.Instance.GetCurrentLevelData().columns) / 2){
                OnLevelComplete();
                SoundManager.Instance.PlaySoundEffect(SoundEnum.LevelCompleteSound);
                LevelManager.Instance.DeleteSave();
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
        LevelManager.Instance.DeleteSave();
        mainMenu.ShowContinueButton(false);

        scoringManager.ClosePanel();
        mainMenu.ChangeThemeToNextLevel();
        mainMenu.OpenMainMenu();
        LevelManager.Instance.currentLevel++;
        LevelManager.Instance.SaveLevelProgress();
    }
}
