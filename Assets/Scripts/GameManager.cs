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
        LoadDeck("Card_Objects");
        Shuffle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DrawCard();
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

    void DrawCard()
    {
        if (deck.Count > 0)
        {
            Card drawnCard = deck[0];
            deck.RemoveAt(0);
            player_hand.Add(drawnCard);

            // Set the position of the drawn card based on its index in the player's hand
            int cardIndex = player_hand.Count - 1;
            drawnCard.transform.position = new Vector3(cardIndex * 2.0f, 0, 0); // Adjust the multiplier as needed
            Debug.Log("Card drawn: " + drawnCard.name);
        }
        else
        {
            Debug.Log("Deck is empty!");
        }
    }
}
