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
        LoadDeck("Cards/Card_Objects");
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

        // Add card drawing on 'D' key press
        if (Input.GetKeyDown(KeyCode.D))
        {
            DrawCard();
        }

        // Add drag and drop functionality
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Card draggedCard = hit.collider.GetComponent<Card>();
                if (draggedCard != null && playerHand.Contains(draggedCard))
                {
                    // Move card to mouse position
                    Vector3 mouseWorldPos = ray.GetPoint(10f); // 10 units from camera
                    draggedCard.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, draggedCard.transform.position.z);
                }
            }
        }

        // Handle card repositioning on mouse release
        if (Input.GetMouseButtonUp(0) && selectedCard != null)
        {
            ReorderHandBasedOnPosition();
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
        
        if (cards.Length == 0)
        {
            Debug.LogError("No cards found in the specified folder!");
            return;
        }

        for (int i = 0; i < cards.Length && deck.Count < 10; i++)
        {
            Card cardInstance = Instantiate(cards[i]);
            deck.Add(cardInstance);
            cardInstance.gameObject.SetActive(false); // Hide the card until drawn
            Debug.Log("Card added to deck: " + cardInstance.name);
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
        if (deck.Count == 0)
        {
            Debug.Log("Deck is empty!");
            return;
        }

        Card drawnCard = deck[0];
        deck.RemoveAt(0);
        playerHand.Add(drawnCard);
        drawnCard.gameObject.SetActive(true);
        
        UpdateHandPositions();
        Debug.Log("Card drawn: " + drawnCard.name);
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

    private void ReorderHandBasedOnPosition()
    {
        if (selectedCard == null || rectangleTransform == null) return;

        // Get the current position of the selected card
        float cardX = selectedCard.transform.position.x;
        
        // Find the closest valid position in the hand
        float rectWidth = rectangleTransform.localScale.x;
        float cardSpacing = rectWidth / (playerHand.Count + 1);
        float startX = rectangleTransform.position.x - (rectWidth / 2) + cardSpacing;
        
        int closestIndex = 0;
        float minDistance = float.MaxValue;
        
        // Find the closest position index
        for (int i = 0; i < playerHand.Count; i++)
        {
            float posX = startX + (i * cardSpacing);
            float distance = Mathf.Abs(posX - cardX);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }
        
        // Reorder the hand
        int currentIndex = playerHand.IndexOf(selectedCard);
        if (currentIndex != closestIndex)
        {
            playerHand.RemoveAt(currentIndex);
            playerHand.Insert(closestIndex, selectedCard);
            UpdateHandPositions();
        }
    }
}
