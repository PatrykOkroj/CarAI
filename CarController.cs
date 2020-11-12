﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Vector3 startPosition, startRotation;
    [Range(-1f, 1f)]
    public float a, t; //Acceleration, turning value
    public float timeSinceStart = 0f;

    [Header("Fitness")]
    public float overallFitness;
    public float distanceMultiplier = 1.4f;
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;

    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    private float aSensor, bSensor, cSensor;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


}

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    public void Reset()
    {
        timeSinceStart = 0f;
        totalDistanceTravelled = 0f;
        avgSpeed = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;

    }

    private void OnCollisionEnter(Collision collision)
    {
        Reset();
    }

    private void CalculateFitness()
    {
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled / timeSinceStart;

        overallFitness = (totalDistanceTravelled * distanceMultiplier) +
            (avgSpeed * avgSpeedMultiplier) + (((aSensor + bSensor + cSensor) / 3) * sensorMultiplier);

        if (timeSinceStart > 20 && overallFitness < 40)
        {
            Reset();
        }

        if (overallFitness >= 1000)
        {
            //TODO Saves network to a JSON
            Reset();
        }

    }

    public void FixedUpdate()
    {
        InputSensors();
        lastPosition = transform.position;
        //TODO Neural network code here
        MoveCar(a, t);

        timeSinceStart += Time.deltaTime;

        CalculateFitness();

    }






    private void InputSensors()
    {
        Vector3 a = (transform.forward + transform.right);
        Vector3 b = (transform.forward);
        Vector3 c = (transform.forward - transform.right);

        Ray r = new Ray(transform.position, a);
        RaycastHit hit; 

        if (Physics.Raycast(r, out hit))
        {
            aSensor = hit.distance / 20;
            print("A: " + aSensor);
        }

        r.direction = b;

        if (Physics.Raycast(r, out hit))
        {
            bSensor = hit.distance / 20;
            print("B: " + aSensor);
        }

        r.direction = c;

        if (Physics.Raycast(r, out hit))
        {
            cSensor = hit.distance / 20;
            print("C: " + aSensor);
        }


    }







    private Vector3 inp; //input
    public void MoveCar(float v, float h) //v = vertical , h = horizontal position
    {
        inp = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, v * 11.4f), 0.02f);
        inp = transform.TransformDirection(inp);
        transform.position += inp;

        transform.eulerAngles += new Vector3(0, (h * 90) * 0.02f, 0);
    }



}
