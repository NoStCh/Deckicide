using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public List<Card> deck = new List<Card>();
    public List<Card> player_deck = new List<Card>();
    public List<Card> ai_deck = new List<Card>();
    public List<Card> player_hand = new List<Card>();
    public List<Card> ai_hand = new List<Card>();
    public List<Card> discard_pile = new List<Card>();

    private List<Card> playerHand = new List<Card>();
    private Card selectedCard;
    private Camera mainCamera;

    [SerializeField] private GameObject rectanglePrefab; // Reference to the Rectangle
    private Transform rectangleTransform;

    private void Awake()
    {
        if (gm != null && gm != this)
        {
            Destroy(gameObject);
        }
        else
        {
            gm = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadDeck("GitHub/Deckicide/Assets/Cards/Card_Objects");
        Shuffle();
        mainCamera = Camera.main;
        
        // Find the Rectangle in the scene
        rectangleTransform = GameObject.Find("Rectangle").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle card selection
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Card clickedCard = hit.collider.GetComponent<Card>();
                if (clickedCard != null && playerHand.Contains(clickedCard))
                {
                    // Deselect previously selected card
                    if (selectedCard != null)
                        selectedCard.IsSelected = false;

                    // Select new card
                    selectedCard = clickedCard;
                    selectedCard.IsSelected = true;
                }
            }
        }

        // Handle playing selected card
        if (Input.GetKeyDown(KeyCode.Return) && selectedCard != null)
        {
            PlayCard(selectedCard);
        }
    }

    void Deal()
    {
        // Implement your deal logic here
    }

    void LoadDeck(string folderPath)
    {
        Card[] cards = Resources.LoadAll<Card>(folderPath);
        Debug.Log("Cards loaded: " + cards.Length);
        for (int i = 0; i < cards.Length && deck.Count < 10; i++)
        {
            deck.Add(cards[i]);
            Debug.Log("Card added to deck: " + cards[i].name);
        }
    }

    void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(0, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void AI_Turn()
    {
        // Implement AI turn logic here
    }

    public void DrawCard()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Card drawnCard = deck[0];
            deck.RemoveAt(0);
            playerHand.Add(drawnCard);

            // Set the position of the drawn card based on its index in the player's hand
            int cardIndex = playerHand.Count - 1;
            drawnCard.transform.position = new Vector3(cardIndex * 2.0f, 0, 0); // Adjust the multiplier as needed
            Debug.Log("Card drawn: " + drawnCard.name);
            UpdateHandPositions();
        }
        else
        {
            Debug.Log("Deck is empty!");
        }
    }

    private void PlayCard(Card card)
    {
        // Add your logic for playing a card to the table
        playerHand.Remove(card);
        selectedCard = null;
        
        // Move card to the table position
        card.transform.position = new Vector3(0f, 0f, 0f); // Adjust position as needed
        card.transform.rotation = Quaternion.identity;

        UpdateHandPositions();
    }

    private void UpdateHandPositions()
    {
        if (rectangleTransform == null || playerHand.Count == 0) return;

        // Get Rectangle's dimensions and position
        Vector3 rectPosition = rectangleTransform.position;
        Vector3 rectScale = rectangleTransform.localScale;
        
        float cardSpacing = rectScale.x / (playerHand.Count + 1);
        float startX = rectPosition.x - (rectScale.x / 2) + cardSpacing;
        
        for (int i = 0; i < playerHand.Count; i++)
        {
            float xPos = startX + (i * cardSpacing);
            Vector3 position = new Vector3(xPos, rectPosition.y, rectPosition.z);
            
            // Calculate rotation based on position relative to center
            float centerOffset = xPos - rectPosition.x;
            float rotationAngle = centerOffset * -5f; // Adjust multiplier for more/less rotation
            
            playerHand[i].transform.position = position;
            playerHand[i].transform.rotation = Quaternion.Euler(20f, rotationAngle, 0f);
        }
    }
}
