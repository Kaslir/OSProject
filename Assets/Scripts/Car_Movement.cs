using System.Collections;
using UnityEngine;

public class Car_Movement : MonoBehaviour
{
    public Transform transform;
    public float speed = 5f;
    private static Car_Spawner carSpawner;  // Reference to Car_Spawner to access the car queue

    void Start()
    {
        transform = GetComponent<Transform>();

        // Get reference to Car_Spawner component
        if (carSpawner == null)
        {
            carSpawner = FindObjectOfType<Car_Spawner>();
        }
    }

    void Update()
    {
        transform.position -= new Vector3(0, speed * Time.deltaTime, 0);  // Move the car down

        // If the car reaches the bottom (y = -10), destroy it and remove it from the queue
        if (transform.position.y <= -10)
        {
            // Remove the car from the queue (FIFO)
            if (carSpawner != null && carSpawner.GetCarQueue().Count > 0 && carSpawner.GetCarQueue().Peek() == gameObject)
            {
                carSpawner.GetCarQueue().Dequeue();  // Remove the car from the queue
            }

            Destroy(gameObject);  // Destroy the car
        }
    }
}
