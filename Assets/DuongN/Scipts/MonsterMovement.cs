using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float moveSpeed;
    public int patrolDestination;

    private GameObject hero; // Reference to the hero GameObject
    private bool heroDetected; // Flag to track if hero is detected

    void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player"); // Assuming "Player" is tagged appropriately
        heroDetected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!heroDetected) // Only patrol if hero is not detected
        {
            if (patrolDestination == 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, patrolPoints[0].position, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, patrolPoints[0].position) < .2f)
                {
                    transform.localScale = new Vector3(3, 3, 3);
                    patrolDestination = 1;
                }
            }

            if (patrolDestination == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, patrolPoints[1].position, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, patrolPoints[1].position) < .2f)
                {
                    transform.localScale = new Vector3(-3, 3, 3);
                    patrolDestination = 0;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Assuming "Player" has appropriate tag
        {
            heroDetected = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Assuming "Player" has appropriate tag
        {
            heroDetected = false;
        }
    }
}

