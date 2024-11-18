using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public Transform transform;
    public float speed = 10f; // Increased movement speed
    public GameObject gameOverPanel;
    public GameObject gamePausePanel;

    // Queue for processing inputs (FCFS)
    private Queue<KeyCode> inputQueue = new Queue<KeyCode>();

    private bool isProcessing = false;
    private bool isGamePaused = false;

    // To track timing for average waiting time and turnaround time
    private float lastInputTime;
    private Game_Controller gameController;

    void Start()
    {
        gameOverPanel.SetActive(false);
        gamePausePanel.SetActive(false);
        gameController = FindObjectOfType<Game_Controller>();
        Time.timeScale = 1; // Ensure the game starts running at normal speed
    }

    void Update()
    {
        // Handle game pause toggle
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        if (isGamePaused)
        {
            return; // Skip processing if the game is paused
        }

        // Queue player input for movement
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            inputQueue.Enqueue(KeyCode.RightArrow);
            lastInputTime = Time.time; // Track when the input was received
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            inputQueue.Enqueue(KeyCode.LeftArrow);
            lastInputTime = Time.time; // Track when the input was received
        }

        // Process input queue if not currently processing
        if (!isProcessing && inputQueue.Count > 0)
        {
            StartCoroutine(ProcessInput());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Cars")
        {
            Time.timeScale = 0; // Stop the game
            gameOverPanel.SetActive(true); // Show the Game Over panel
        }
    }

    IEnumerator ProcessInput()
    {
        isProcessing = true;

        KeyCode input = inputQueue.Dequeue(); // Dequeue the next input
        float startProcessingTime = Time.time; // Start time for processing

        Vector3 targetPosition = transform.position;

        // Calculate target position based on input
        if (input == KeyCode.RightArrow)
        {
            targetPosition += new Vector3(1, 0, 0);
        }
        else if (input == KeyCode.LeftArrow)
        {
            targetPosition -= new Vector3(1, 0, 0);
        }

        // Clamp the x position to be within the range -1 to 1
        targetPosition.x = Mathf.Clamp(targetPosition.x, -1f, 1f);

        // Smoothly move to the target position
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < 1f / speed)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime * speed));
            elapsedTime += Time.deltaTime;
            yield return null; // Process in the next frame
        }

        transform.position = targetPosition;

        // Log metrics for FCFS in the Game Controller
        float completionTime = Time.time;
        gameController.LogProcess(lastInputTime, completionTime);

        yield return null; // Minimal delay for smoother processing

        isProcessing = false;
    }

    private void TogglePause()
    {
        // Toggle the game pause state
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0; // Pause the game
            gamePausePanel.SetActive(true); // Show the Pause panel
        }
        else
        {
            Time.timeScale = 1; // Resume the game
            gamePausePanel.SetActive(false); // Hide the Pause panel
        }
    }
}
