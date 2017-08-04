// ButtonManager.cs
//
// Purpose:	Manage responses to button presses and write to files
// Authors: Julio Medina, Shelby Ziccardi
//          jamedina@hmc.edu
//
// Note:	This component is provided to fade out a single camera layer's
//			scene view.  If instead you want to fade the entire view, use:
//			SteamVR_Fade.View(Color.black, 1);
//			(Does not affect the game view, however.)
//
//=============================================================================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class ButtonManager : MonoBehaviour{

    //testing positive or negative thresholds
    //public enum Experiment { posStaircase, posPEST, negStaircase, negPEST }
    public enum Experiment { Staircase, PEST }
    public Experiment EXPERIMENT;

    public AudioSource soundFile;
    public RotationTests rotationTests;
    public GameObject room;
    public GameObject player;
    public GameObject arrow;
    public GameObject wall;

    public AudioSource fasterVoiceover;
    public AudioSource slowerVoiceover;

    private static bool needSound = false;
    private static string clicked = " ";
   
    /// <summary>
    /// Initializes files to keep track of test results and fades scene in
    /// while playing welcome voiceover.
    /// </summary>
    private void Awake()
    {
        //Initialize files
        Utils.writeToFile("Assets/" + rotationTests.userID + "_Test.txt", "");
        Utils.writeToFile("Assets/" + rotationTests.userID + "_Results.txt", "");
        Utils.writeToFile("Assets/" + rotationTests.userID + "_FinalResults.txt", "");

        //Fade scene in while playing voiceover
        SteamVR_Fade.Start(Color.gray, 0f);
        fasterVoiceover.Play();
        SteamVR_Fade.Start(Color.clear, fasterVoiceover.clip.length);

        //initialize tests
        Initialize();
    }


    private void Initialize()
    {
        if (rotationTests.getIsDone()) { rotationTests.setNegative(); }

        switch (EXPERIMENT)
        {
            case Experiment.Staircase:
                if (rotationTests.getIsNeg())
                {
                    rotationTests.staircase(false);
                }
                else
                {
                    rotationTests.staircase(true);
                }
                break;

            case Experiment.PEST:
                rotationTests.PESTInitialization();
                break;
        }
    }

    void Update()
    {
        // For debugging purposes: if te experimenter presses "s" on the keyboard,
        // simulate what would happen when 
        if (Input.GetKeyDown("s"))
        {
            Utils.writeToFile("Assets/"+ rotationTests.userID +"_Test.txt", "Same");
            switch (EXPERIMENT)
            {
                case Experiment.Staircase:
                    if (rotationTests.getIsNeg())
                    {
                        rotationTests.staircase(false);
                    }
                    else
                    {
                        rotationTests.staircase(true);
                    }
                    break;
               
                case Experiment.PEST:
                    rotationTests.PEST();
                    break;
            }
            Utils.rotateRoom(room, arrow, player);
        }
        if (Input.GetKeyDown("d"))
        {
            Utils.writeToFile("Assets/"+ rotationTests.userID +"_Test.txt", "Different");
            switch (EXPERIMENT)
            {
                case Experiment.Staircase:
                    if (rotationTests.getIsNeg())
                    {
                        rotationTests.staircase(false);
                    }
                    else
                    {
                        rotationTests.staircase(true);
                    }
                    break;
                case Experiment.PEST:
                    rotationTests.PEST();
                    break;
            }
            Utils.rotateRoom(room, arrow, player);

        }

        if(rotationTests.getIsDone())
        {   
            SteamVR_Fade.Start(Color.white, 0f);
            slowerVoiceover.Play();
            SteamVR_Fade.Start(Color.clear, slowerVoiceover.clip.length);
            
            Initialize();
        }

        if (needSound)
        {
            soundFile.Play();
            needSound = false;
        }
        
    }
    

    void OnCollisionEnter(Collision col)
    {
        clicked = col.gameObject.name;
    }


    public void resetRoom()
    {
        if (clicked == "Same" || clicked == "Different")
        {
            needSound = true;
            Debug.Log(clicked);
            Utils.writeToFile("Assets/"+ rotationTests.userID +"_Test.txt", clicked);
            switch (EXPERIMENT)
            {
                case Experiment.Staircase:
                    if (rotationTests.getIsNeg())
                    {
                        Debug.Log("Awake: negStaircase");
                        rotationTests.staircase(false);
                    }
                    else
                    {
                        Debug.Log("Awake: posStaircase");
                        rotationTests.staircase(true);
                    }
                    break;
                
                case Experiment.PEST:
                    rotationTests.PEST();
                    break;
            }
            Utils.rotateRoom(room, arrow, player);
        }
    }
}

