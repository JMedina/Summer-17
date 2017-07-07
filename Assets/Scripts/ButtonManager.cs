using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class ButtonManager : MonoBehaviour{

    //testing positive or negative thresholds
    public enum Experiment { posStaircase, posPEST, negStaircase, negPEST }
    public Experiment EXPERIMENT;

    public AudioSource soundFile;
    public RotationTests rotationTests;
    public GameObject room;
    public GameObject player;
    public GameObject arrow;
    
    private static bool needSound = false;
    private static string clicked = " ";
    
    void writeToFile(string text)
    {
        StreamWriter writer = new StreamWriter("Assets/test.txt", true);
        writer.WriteLine(text);
        writer.Close();
    }

    private void Awake()
    {
        if(EXPERIMENT == Experiment.negPEST|| EXPERIMENT == Experiment.negStaircase)
        {
            rotationTests.setNegative();
        }

        switch (EXPERIMENT)
        {
            case Experiment.posStaircase:
                Debug.Log("Awake: posStaircase");
                rotationTests.staircase(true);
                break;
            case Experiment.negStaircase:
                Debug.Log("Awake: negStaircase");
                rotationTests.staircase(false);
                break;
            case Experiment.negPEST:
            case Experiment.posPEST:
                Debug.Log("Awake: PEST");
                rotationTests.PESTInitialization();
                break;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown("y"))
        {
            writeToFile("Yes");
            switch (EXPERIMENT)
            {
                case Experiment.posStaircase:
                    rotationTests.staircase(true);
                    break;
                case Experiment.negStaircase:
                    rotationTests.staircase(false);
                    break;
                case Experiment.negPEST:
                case Experiment.posPEST:
                    rotationTests.PEST();
                    break;
            }
            rotateRoom();
        }
        if (Input.GetKeyDown("n"))
        {
            writeToFile("No");
            switch (EXPERIMENT)
            {
                case Experiment.posStaircase:
                    rotationTests.staircase(true);
                    break;
                case Experiment.negStaircase:
                    rotationTests.staircase(false);
                    break;
                case Experiment.negPEST:
                case Experiment.posPEST:
                    rotationTests.PEST();
                    break;
            }
            rotateRoom();
        }
        if (needSound)
        {
            soundFile.Play();
            needSound = false;
        }
    }


    void OnCollisionEnter(Collision col)  //Maybe OnCollisionStay?
    {
        clicked = col.gameObject.name;
    }



    void rotateRoom()
    {
        SteamVR_Fade.Start(Color.black, 0.1f);
        Vector3 axis = new Vector3(0, 1, 0);
        room.transform.RotateAround(player.transform.position, axis, 180f);
        arrow.transform.Rotate(new Vector3(0, 0, 1), 180f);
        SteamVR_Fade.Start(Color.clear, 1.2f);
    }


    public void resetRoom()
    {
        if (clicked == "Yes" || clicked == "No")
        {
            needSound = true;
            Debug.Log(clicked);
            writeToFile(clicked);
            switch (EXPERIMENT)
            {
                case Experiment.posStaircase:
                    rotationTests.staircase(true);
                    break;
                case Experiment.negStaircase:
                    rotationTests.staircase(false);
                    break;
                case Experiment.negPEST:
                case Experiment.posPEST:
                    rotationTests.PEST();
                    break;
            }
            rotateRoom();
        }
    }
}

