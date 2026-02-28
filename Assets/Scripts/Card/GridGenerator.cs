using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{
    public CardScriptableObject cardScriptableObject;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private int rows = 2;
    [SerializeField] private int columns = 2;
    [SerializeField] private float spacing = 0.2f;
    [SerializeField] private float cardsShowDuration = 1f;

    private List<Card> cardsPool = new List<Card>();
    
    public void GenerateGrid(LevelData levelData, ICardListener cardListener)
    {
        CardProperties[] cardData = GenerateCardsData(levelData.rows, levelData.columns);
        GenerateGrid(levelData.rows, levelData.columns, cardData, cardListener);
    }

    public void GenerateGrid(int rows, int columns, CardProperties[] cardData, ICardListener cardListener, bool[] matchedStates = null)
    {
        this.rows = rows;
        this.columns = columns;

        Camera cam = Camera.main;
        float cameraHeight = cam.orthographicSize * 2f;
        float cameraWidth = cameraHeight * cam.aspect;

        float availableWidth = cameraWidth - (columns - 1) * spacing;
        float availableHeight = cameraHeight - (rows - 1) * spacing;

        SpriteRenderer prefabSR = cardPrefab.cardSpriteReneder;
        Vector2 spriteSize = prefabSR.sprite.bounds.size;

        float targetWidth = availableWidth / columns;
        float targetHeight = availableHeight / rows;

        float scaleX = targetWidth / spriteSize.x;
        float scaleY = targetHeight / spriteSize.y;
        float scaleFactor = Mathf.Min(scaleX, scaleY);

        float scaledCardWidth = spriteSize.x * scaleFactor;
        float scaledCardHeight = spriteSize.y * scaleFactor;

        float totalGridWidth = columns * scaledCardWidth + (columns - 1) * spacing;
        float totalGridHeight = rows * scaledCardHeight + (rows - 1) * spacing;

        Vector2 startPos = new Vector2(
            -totalGridWidth / 2f + scaledCardWidth / 2f,
            totalGridHeight / 2f - scaledCardHeight / 2f
        );

        int cardIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector2 position = new Vector2(
                    startPos.x + col * (scaledCardWidth + spacing),
                    startPos.y - row * (scaledCardHeight + spacing)
                );

                Card card = null;
                //Reusing existing cards
                if(cardIndex < cardsPool.Count){
                    card = cardsPool[cardIndex];
                }
                if(card == null){
                    card = Instantiate(cardPrefab, position, Quaternion.identity);
                    cardsPool.Add(card);
                }   
                card.transform.position = position;
                cardsPool[cardIndex].gameObject.SetActive(true);
                card.gameObject.transform.localScale = Vector3.one * scaleFactor;
                card.SetUpCard(cardData[row * columns + col], cardListener);

                // If saved matched state exists and this card was matched, deactivate it
                if (matchedStates != null && matchedStates[row * columns + col])
                {
                    card.OnMatchFound();
                }

                cardIndex++;
            }
        }

        //Disabling unused cards
        if(cardIndex < cardsPool.Count){
            for(int i = cardIndex; i < cardsPool.Count; i++){
                cardsPool[i].gameObject.SetActive(false);
            }
        }

        StartCoroutine(ShowAllCards());
    }

    CardProperties[] GenerateCardsData(int rows, int columns)
    {
        int totalCards = rows * columns;
        CardProperties[] cardPropertiesList =
            new CardProperties[totalCards];

        int uniqueCardCount = cardScriptableObject.cardPropertiesArray.Length;

        for (int i = 0; i < totalCards; i += 2)
        {
            int index = (i / 2) % uniqueCardCount;

            cardPropertiesList[i] = cardScriptableObject.cardPropertiesArray[index];
            cardPropertiesList[i + 1] = cardScriptableObject.cardPropertiesArray[index];
        }
        Shuffle(cardPropertiesList);
        return cardPropertiesList;
    }

    void Shuffle(CardProperties[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            CardProperties temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    private CardProperties FindCardPropertiesByNumber(int cardNumber)
    {
        foreach (CardProperties cp in cardScriptableObject.cardPropertiesArray)
        {
            if (cp.cardNumber == cardNumber)
                return cp;
        }
        Debug.LogWarning($"[GridGenerator] CardProperties not found for cardNumber: {cardNumber}");
        return cardScriptableObject.cardPropertiesArray[0];
    }

    public CardSaveData[] GetCardStates()
    {
        int totalCards = rows * columns;
        CardSaveData[] cardStates = new CardSaveData[totalCards];

        for (int i = 0; i < totalCards && i < cardsPool.Count; i++)
        {
            Card card = cardsPool[i];
            cardStates[i] = new CardSaveData
            {
                gridIndex = i,
                cardNumber = card.cardProperties.cardNumber,
                isMatched = !card.gameObject.activeSelf
            };
        }

        return cardStates;
    }

    public CardProperties[] BuildCardDataFromSave(GameSaveData saveData)
    {
        CardProperties[] cardData = new CardProperties[saveData.cards.Length];
        for (int i = 0; i < saveData.cards.Length; i++)
        {
            cardData[i] = FindCardPropertiesByNumber(saveData.cards[i].cardNumber);
        }
        return cardData;
    }

    public bool[] BuildMatchedStatesFromSave(GameSaveData saveData)
    {
        bool[] matched = new bool[saveData.cards.Length];
        for (int i = 0; i < saveData.cards.Length; i++)
        {
            matched[i] = saveData.cards[i].isMatched;
        }
        return matched;
    }

    public int GetRows() { return rows; }
    public int GetColumns() { return columns; }

    IEnumerator ShowAllCards() {
        foreach (Card card in cardsPool) {
            card.Flip();
        }

        yield return new WaitForSeconds(cardsShowDuration);
    
        foreach (Card card in cardsPool) {
            card.Flip();
        }
    }
}
