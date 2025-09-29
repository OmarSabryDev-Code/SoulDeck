using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // DOTween namespace
using UnityEngine.EventSystems;


public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    public Image background;
    public Image iconImage;
    public TMP_Text manaCostText;
    public TMP_Text nameText;
    
    public TMP_Text descriptionText;
    //public GameObject glow;
    public HandManager handManager;

    [Header("Animation Settings")]
    public float spawnDuration = 0.5f;
    public float playDuration = 0.7f;
    public float hoverScale = 1.1f;

    [HideInInspector] public Card cardData;
    private CanvasGroup canvasGroup;
    [Header("Touch Settings")]
    public float pressedScale = 1.1f;  // scale when finger presses down
    public bool isOpponentCard = false;
    public enum CardOwner { Player, Opponent }
    [Header("Card Faces")]
    public GameObject frontFace;   // assign all card visuals here
    public GameObject backFace;    // assign card back object here
    public CardOwner owner = CardOwner.Player;




    void Start()
    {
        handManager = FindObjectOfType<HandManager>();
    }


    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
         if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    // 🔹 Called by DeckSpawner when creating the card
    public void UpdateCardUI(Card newCard)
    {
        cardData = newCard;

        nameText.text = cardData.cardName;
        descriptionText.text = cardData.description;
        manaCostText.text = cardData.manaCost.ToString();
        iconImage.sprite = cardData.cardIcon;

        // background color by type
        switch (cardData.cardType)
        {
            case CardType.Attack:
                //background.color = new Color(1f, 0.6f, 0.6f);
                background.color = Color.white;
                break;
            case CardType.Heal:
                //background.color = new Color(0.6f, 1f, 0.6f);
                background.color = Color.white;
                break;
            case CardType.Defense:
                //background.color = new Color(0.6f, 0.6f, 1f);
                background.color = Color.white;
                break;
            case CardType.Steal:
                //background.color = new Color(1f, 1f, 0.6f);
                background.color = Color.white;
                break;
            default:
                background.color = Color.white;
                break;
        }
    }
    public void SetHidden(bool hidden)
    {
        if (frontFace != null) frontFace.SetActive(!hidden);
        if (backFace != null) backFace.SetActive(hidden);
    }

    // 🔹 Animate when card enters hand
    public void AnimateSpawn(Vector3 startPos)
    {
        transform.position = startPos;
        transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1f, spawnDuration).SetEase(Ease.OutBack));
        seq.Join(canvasGroup.DOFade(1f, spawnDuration));
    }

    // 🔹 Animate when card is played
    public void AnimatePlay(Vector3 targetPos)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(targetPos, playDuration).SetEase(Ease.InOutQuad));
        seq.Join(transform.DOScale(1.2f, playDuration * 0.5f).SetLoops(2, LoopType.Yoyo));
        seq.OnComplete(() =>
        {
            // disable card after playing
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
    }

    // 🔹 Hover/Select effect
    /** public void OnHover(bool isHovering)
    {
        if (isHovering)
        {
            transform.DOScale(hoverScale, 0.2f).SetEase(Ease.OutBack);
            //glow.SetActive(true);
        }
        else
        {
            transform.DOScale(1f, 0.2f).SetEase(Ease.InBack);
            //glow.SetActive(false);
        }
    }**/
    public void OnClick()
    {
        if (handManager == null)
        {
            handManager = FindObjectOfType<HandManager>();
        }

        if (handManager != null)
        {
            handManager.PlayCard(gameObject);
        }
        if (owner == CardOwner.Opponent)
        {
            Debug.Log("Opponent card clicked – ignoring input.");
            return;
        }



    }
    public void TestEvent()
    {
        Debug.Log("EventTrigger working!");
    }
    // Wrapper for click
    public void OnCardClick()
    {
        Debug.Log("Card clicked from EventTrigger!");
        AnimatePlay(new Vector3(0, 0, 0)); // or call HandManager here
    }

    // Wrapper for hover enter
    /** public void OnCardHoverEnter()
     {
         OnHover(true);
     }

     // Wrapper for hover exit
     public void OnCardHoverExit()
     {
         OnHover(false);
     }**/
    // Called on tap down (finger touches)
    public void OnPointerDown(PointerEventData eventData)
{
     Debug.Log($"[CardUI] OnPointerDown: {(cardData != null ? cardData.cardName : gameObject.name)}");
    transform.DOScale(pressedScale, 0.12f).SetEase(Ease.OutBack);
}

public void OnPointerUp(PointerEventData eventData)
{
        Debug.Log($"[CardUI] OnPointerUp: {(cardData != null ? cardData.cardName : gameObject.name)}");
        transform.DOScale(1f, 0.12f).SetEase(Ease.InBack);

    // Instead of hover → call PlayCard on tap
    if (handManager == null) handManager = FindObjectOfType<HandManager>();
    if (handManager != null)
        {
            handManager.PlayCard(gameObject);
        }
        else
        {
            Debug.LogWarning("[CardUI] No HandManager found in scene.");
        }
}


}
