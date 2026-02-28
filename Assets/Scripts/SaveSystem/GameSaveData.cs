using System;

/// <summary>
/// Serializable data model that captures the full game state for JSON persistence.
/// </summary>
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

/// <summary>
/// Serializable data model for an individual card's state in the grid.
/// </summary>
[Serializable]
public class CardSaveData
{
    /// <summary>Position index in the grid (row * columns + col).</summary>
    public int gridIndex;

    /// <summary>The card type number used for matching.</summary>
    public int cardNumber;

    /// <summary>Whether this card has already been matched and removed.</summary>
    public bool isMatched;
}
