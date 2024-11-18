using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Spawner : MonoBehaviour
{
    public GameObject[] car; // Array of car prefabs
    private Queue<GameObject> carQueue = new Queue<GameObject>(); // Queue for FIFO destruction
    private int currentCarIndex = 0; // Index for Round-Robin scheduling

    private Game_Controller gameController; // Reference to the Game_Controller script
    private float lastSpawnTime;

    void Start()
    {
        gameController = FindObjectOfType<Game_Controller>();
        StartCoroutine(SpawnCars());
    }

    void SpawnCar()
    {
        float randXPos = Random.Range(-1.25f, 1.25f);

        // Round-Robin: Spawn car based on the current index
        GameObject newCar = Instantiate(car[currentCarIndex], new Vector3(randXPos, transform.position.y, transform.position.z), Quaternion.Euler(0, 0, 90));

        carQueue.Enqueue(newCar); // Add car to FIFO queue
        lastSpawnTime = Time.time;

        // Log the spawning process
        gameController.LogProcess(lastSpawnTime, Time.time);

        // Move to the next car index
        currentCarIndex = (currentCarIndex + 1) % car.Length;
    }

    IEnumerator SpawnCars()
    {
        while (true)
        {
            yield return new WaitForSeconds(2); // Fixed time slice for Round-Robin
            SpawnCar();
        }
    }

    public Queue<GameObject> GetCarQueue()
    {
        return carQueue;
    }
}
