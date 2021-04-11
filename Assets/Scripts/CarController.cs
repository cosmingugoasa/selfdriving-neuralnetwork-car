using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform sensorsPosition;
    public Transform spawnPosition;
    float A, B, C, L, R;    //sensors
    float sensorMaxRange = 20f;

    [Range(0, 1)]
    public float acceleration;
    [Range(-1, 1)]
    public float steering;

    Vector3 startPosition, startRotation, lastPosition;
    Rigidbody rb;

    //FITNESS EVALUATOR
    public float fitness;
    public float distance = 0;
    float timeAlive = 0;

    public NN NN;
    public Trainer trainer;
    public bool loaded = false;
    public bool playbackOnly = false;

    CarController() { }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = lastPosition = spawnPosition.position;
        startRotation = spawnPosition.eulerAngles;
        NN = new NN(new int[] { 5,4,2});
        
        //if net is not loaded from file, randomize one
        if (!loaded) {
            NN.Randomize();
        }        
    }

    private void Update()
    {
        if (transform.position.y < 0)
        {
            NN.fitness = 0;
            Debug.Log("OUT OF TRACK");
        }

        Sensors();
        (acceleration, steering) = NN.Run(A, B, C, L, R);
       
        EvaluateFitness();
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        MoveCar(acceleration, steering);
    }

    public void MoveCar(float acc, float steer)
    {
        rb.MovePosition(transform.position + (transform.forward * (acc/3)));
        transform.Rotate(0, steer, 0);
    }

    void Sensors()
    {
        RaycastHit hit;

        //sensor A (front-left)
        if (Physics.Raycast(sensorsPosition.position, sensorsPosition.forward - sensorsPosition.right, out hit, sensorMaxRange))
        {
            A = hit.distance / 10;
            Debug.DrawRay(sensorsPosition.position, (sensorsPosition.forward - sensorsPosition.right) * hit.distance, Color.red);
        }
        else
        {
            A = sensorMaxRange / 10;
            Debug.DrawRay(sensorsPosition.position, (sensorsPosition.forward - sensorsPosition.right) * sensorMaxRange, Color.green);
        }

        //sensor B (front)
        if (Physics.Raycast(sensorsPosition.position, sensorsPosition.forward, out hit, sensorMaxRange))
        {
            B = hit.distance / 10;
            Debug.DrawRay(sensorsPosition.position, sensorsPosition.forward * hit.distance, Color.red);
        }
        else
        {
            B = sensorMaxRange / 10;
            Debug.DrawRay(sensorsPosition.position, sensorsPosition.forward * sensorMaxRange, Color.green);
        }

        //sensor C (front-right)
        if (Physics.Raycast(sensorsPosition.position, sensorsPosition.forward + sensorsPosition.right, out hit, sensorMaxRange))
        {
            C = hit.distance / 10;
            Debug.DrawRay(sensorsPosition.position, (sensorsPosition.forward + sensorsPosition.right) * hit.distance, Color.red);
        }
        else
        {
            C = sensorMaxRange / 10;
            Debug.DrawRay(sensorsPosition.position, (sensorsPosition.forward + sensorsPosition.right) * sensorMaxRange, Color.green);
        }

        //sensor L (left)
        if (Physics.Raycast(sensorsPosition.position, -sensorsPosition.right, out hit, sensorMaxRange))
        {
            L = hit.distance / 10;
            Debug.DrawRay(sensorsPosition.position, -sensorsPosition.right * hit.distance, Color.red);
        }
        else
        {
            L = sensorMaxRange / 10;
            Debug.DrawRay(sensorsPosition.position, -sensorsPosition.right * sensorMaxRange, Color.green);
        }

        //sensor R (right)
        if (Physics.Raycast(sensorsPosition.position, sensorsPosition.right, out hit, sensorMaxRange))
        {
            R = hit.distance / 10;
            Debug.DrawRay(sensorsPosition.position, sensorsPosition.right * hit.distance, Color.red);
        }
        else
        {
            R = sensorMaxRange / 10;
            Debug.DrawRay(sensorsPosition.position, sensorsPosition.right * sensorMaxRange, Color.green);
        }
    }

    //called when car crashes
    public void Reset()
    {       
        distance = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;

        if (!playbackOnly) {
            NN = trainer.AnalyzeNetwork(NN);
        }        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {          
            NN.fitness = this.fitness;
            Reset();
        }

        if (collision.gameObject.CompareTag("ground"))
        {
            NN.fitness = 0;
            Reset();
        }
    }

    //calculate fitness based on distance reached and distance from walls
    void EvaluateFitness()
    {
        distance += Vector3.Distance(lastPosition, transform.position);
        fitness = distance + ((A + B + C + L + R) / 5);

        if (timeAlive > 5 && fitness < 20)
        {
            Reset();
        }
    }
}
