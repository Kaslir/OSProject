using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour
{
    public TextMeshProUGUI highScoreText; // Highscore display
    public TextMeshProUGUI scoreText; // Current score display
    public TextMeshProUGUI metricsText; // Display for AWT and ATT

    public int score;
    public int highScore;
    public Score_Manager scoreManager;

    public GameObject gamePausePanel;

    private float totalWaitingTime = 0f;
    private float totalTurnaroundTime = 0f;
    private int processCount = 0;

    // Peterson's n Solution Variables for Synchronization
    private static bool[] flag;
    private static int turn;
    private int processId;

    void Start()
    {
        gamePausePanel.SetActive(false);
        metricsText.text = ""; // Initialize metrics display

        // Initialize Peterson's n Solution
        int numProcesses = 2; // For simplicity, assume two processes (one for pause, one for resume)
        flag = new bool[numProcesses];
        turn = 0;

        processId = 0; // The current process (this can be set dynamically based on pause/resume action)
    }

    void Update()
    {
        // Update high score and current score
        highScore = PlayerPrefs.GetInt("high_score");
        score = scoreManager.score;

        highScoreText.text = "Highscore: " + highScore.ToString();
        scoreText.text = "Your Score: " + score.ToString();

        // Display average waiting time and turnaround time
        if (processCount > 0)
        {
            float avgWaitingTime = totalWaitingTime / processCount;
            float avgTurnaroundTime = totalTurnaroundTime / processCount;
            metricsText.text = $"Avg WT: {avgWaitingTime:F2} s, Avg TAT: {avgTurnaroundTime:F2} s";
        }
    }

    // Peterson’s n Solution for mutual exclusion
    private void EnterCriticalSection()
    {
        flag[processId] = true;
        turn = (processId == 0) ? 1 : 0; // Alternate turns between two processes

        while (flag[(processId == 0) ? 1 : 0] && turn == processId)
        {
            // Wait until the other process finishes its critical section
        }
    }

    private void LeaveCriticalSection()
    {
        flag[processId] = false;
    }

    public void PauseGame()
    {
        EnterCriticalSection(); // Ensure mutual exclusion for pausing

        // Pause the game and show the pause panel
        Time.timeScale = 0;
        gamePausePanel.SetActive(true);

        LeaveCriticalSection(); // Leave critical section after the action is done
    }

    public void ResumeGame()
    {
        EnterCriticalSection(); // Ensure mutual exclusion for resuming

        // Resume the game and hide the pause panel
        Time.timeScale = 1;
        gamePausePanel.SetActive(false);

        LeaveCriticalSection(); // Leave critical section after the action is done
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LogProcess(float arrivalTime, float completionTime)
    {
        float turnaroundTime = completionTime - arrivalTime;
        float waitingTime = turnaroundTime - 1f; // Assume each process takes 1 second

        totalTurnaroundTime += turnaroundTime;
        totalWaitingTime += waitingTime;
        processCount++;
    }
}
