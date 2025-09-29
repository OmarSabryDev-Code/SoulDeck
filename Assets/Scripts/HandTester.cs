using UnityEngine;

public class HandTester : MonoBehaviour
{
    public HandManager handManager;
    public GameObject cardPrefab;

    void Update()
    {
        // Press space to draw a card
        if (Input.GetKeyDown(KeyCode.Space))
        {
            handManager.AddCard(cardPrefab);
        }
    }
}
