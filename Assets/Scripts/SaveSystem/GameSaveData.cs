using System;

[Serializable]
public class GameSaveData
{
    public int currentLevel;
    public int rows;
    public int columns;
    public int score;
    public int consecutiveMatches;
    public int totalMatches;
    public CardSaveData[] cards;
}

[Serializable]
public class CardSaveData
{
    public int gridIndex;
    public int cardNumber;
    public bool isMatched;
}
