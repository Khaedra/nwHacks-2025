using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer swirl;

    [SerializeField]  // Add this so you can assign in inspector
    private GameObject wall;  // Changed to GameObject

    [SerializeField]  // Add this so you can assign in inspector
    private GameObject wall2;  // Changed to GameObject

    public string requiredSign = "A";
    public float dropSpeed = 5f;  // How fast the wall drops
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private Vector3 targetPosition2;
    private Vector3 startPosition2;
    private bool isDropping = false;
    private float lastWalkSoundTime = 0f;
    private float walkSoundCooldown = 7f;

    void Start()
    {
        // Make sure wall reference exists
        if (wall != null && wall2 != null)
        {
            // Store the start position
            startPosition = wall.transform.position;
            // Calculate target position (where wall should drop to)
            targetPosition = startPosition + Vector3.down * 27f; // Drops 5 units down
            
            // Start with wall hidden
            wall.GetComponent<BoxCollider2D>().enabled = false;
            wall.GetComponent<SpriteRenderer>().enabled = false;
            startPosition2 = wall2.transform.position;
            // Calculate target position (where wall should drop to)
            targetPosition2 = startPosition2 + Vector3.down * 27f; // Drops 5 units down
            
            // Start with wall hidden
            wall2.GetComponent<BoxCollider2D>().enabled = false;
            wall2.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            Debug.LogError("Wall not assigned to Stone script!");
        }

        // Hide swirl at start if it exists
        if (swirl != null)
        {
            swirl.enabled = false;
        }
    }

    void Update()
    {
        // If wall is dropping, animate it
        if (isDropping && wall != null && wall2!=null)
        {
            wall.transform.position = Vector3.Lerp(
                wall.transform.position, 
                targetPosition, 
                Time.deltaTime * dropSpeed
            );
            wall2.transform.position = Vector3.Lerp(
                wall2.transform.position, 
                targetPosition2, 
                Time.deltaTime * dropSpeed
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered stone zone!");
            PlayerS player = other.GetComponent<PlayerS>();
    
            if (player != null && wall != null)
            {
                player.SetCurrentStone(this);
                wall.GetComponent<BoxCollider2D>().enabled = true;
                wall.GetComponent<SpriteRenderer>().enabled = true;
                wall2.GetComponent<BoxCollider2D>().enabled = true;
                wall2.GetComponent<SpriteRenderer>().enabled = true;
                isDropping = true;  // Start dropping the wall
                Sound.Instance.doorOpen();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerS player = other.GetComponent<PlayerS>();
            Debug.Log("Player left stone zone!");
            if (player != null)
            {
                player.SetCurrentStone(null);
            }
        }
    }

    public void OnCorrectSign()
    {
        if (swirl != null)
        {
            swirl.enabled = true;
        }

        if (wall != null)
        {
            wall.GetComponent<BoxCollider2D>().enabled = false;
            wall2.GetComponent<BoxCollider2D>().enabled = false;
            // Optional: Make wall fade out or move up
            StartCoroutine(RemoveWall());
            Sound.Instance.rune(); 
            
        
        }
    }

    private IEnumerator RemoveWall()
    {
        float elapsed = 0;
        Vector3 currentPos = wall.transform.position;
        Vector3 currentPos2 = wall2.transform.position;
        
        // Move wall back up over 1 second
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            wall.transform.position = Vector3.Lerp(currentPos, startPosition, elapsed);
            wall2.transform.position = Vector3.Lerp(currentPos2, startPosition2, elapsed);
            yield return null;
        }

        // Hide wall when done
        wall.GetComponent<SpriteRenderer>().enabled = false;
        wall2.GetComponent<SpriteRenderer>().enabled = false;
        isDropping = false;
    }
}