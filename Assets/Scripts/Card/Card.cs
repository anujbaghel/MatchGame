using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
    public CardProperties cardProperties;
    public SpriteRenderer cardSpriteReneder;
    public Sprite backSprite;
    [SerializeField] private float flipDuration = 0.5f;
    public bool isFlipping = false;
    [SerializeField] private bool isFaceUp = false;
    [SerializeField] private ICardListener cardListener;

    void Start()
    {
        cardSpriteReneder = GetComponent<SpriteRenderer>();
        cardSpriteReneder.sprite = backSprite;
    }

    public void SetUpCard(CardProperties cardProperties, ICardListener cardListener){
        this.cardProperties = cardProperties;
        this.cardListener = cardListener;
        ResetCard();
    }

    private void ResetCard() {
        cardSpriteReneder.sprite = backSprite;
        isFaceUp = false;
        isFlipping = false;
        gameObject.SetActive(true);
    }

    public void Flip()
    {
        isFaceUp = !isFaceUp;
        if (!isFlipping)
            StartCoroutine(FlipRoutine());
    }

    IEnumerator FlipRoutine()
    {
        isFlipping = true;

        float time = 0f;
        float startAngle = transform.eulerAngles.y;
        float targetAngle;

         // Normalize angle (Unity gives 0–360)
        float normalized = startAngle > 180 ? startAngle - 360 : startAngle;
        targetAngle = Mathf.Abs(normalized) < 1f ? 180f : 0f;
        bool spriteSwapped = false;
        while (time < flipDuration)
        {
            float angle = Mathf.Lerp(startAngle, targetAngle, time / flipDuration);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (!spriteSwapped)
            {
                if(isFaceUp && angle >= 90f){
                    cardSpriteReneder.sprite = cardProperties.frontSprite;
                }
                else if(!isFaceUp && angle >= 90f){
                    cardSpriteReneder.sprite = backSprite;
                }
            }
          
            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        isFlipping = false;
    }

    void OnMouseDown()
    {
        if (!isFlipping)
        {
            if(!isFaceUp){
                cardListener.OnCardClicked(this);
                Flip();
            }
        }
    }

    public void OnMatchFound() {
        gameObject.SetActive(false);
    }
}
