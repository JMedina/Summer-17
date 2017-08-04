using Redirection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Rating.cs
//Manages the verification experience and records the user's weirdness rating.
//Once calibration reaches confidence, this script will launch automatically.
//TODO: Currently minGain and maxGain are not se to any particular value.
//      set them to the values collected from the calibration 
//TODO: The type of calibration that has just been finished must be selected
//      mannually from the inspector for the results to be recorded in the right
//      file. Possibly automate this.


public class Rating : Redirector {

    public enum Experiment { Staircase, PEST }
    public Experiment EXPERIMENT;

    public Transform player;
    public Transform target1;
    public Transform target2;
    public Transform target3;

    public static int userID;
    public static int maxGain;
    public static int minGain;

    public UnityEngine.UI.Slider slider;
    private SteamVR_TrackedObject trackedObj;


    /// <summary>
    /// Container for getting input from the vr controller.
    /// </summary>
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    /// <summary>
    /// Uses controller input to adjust the value of the weirdness rating.
    /// When trigger is pressed, writes value to file and launches next phase.
    /// </summary>
    void Update()
    {
        if (Controller.GetAxis().y > 0.0f || Input.GetKey("up"))
        {
            if (slider.value <= 100)
            {
                slider.value += 30 * Time.deltaTime;
            }
        }
        else if (Controller.GetAxis().y < 0.0f || Input.GetKey("down")){

            if (slider.value >= 0)
            {
                slider.value -= 30 * Time.deltaTime;
            }
        }
        else if (Controller.GetHairTrigger() || Input.GetKey("r"))
        {
            SteamVR_Fade.Start(Color.black, 0.1f);
            SteamVR_Fade.Start(Color.clear, 1.2f);
            Utils.writeToFile("Assets/" + userID + Convert.ToString(EXPERIMENT) + "Weirdness.txt", Convert.ToString(slider.value));
            SceneManager.LoadScene("Redirected Walking Scene");
        }
    }

    /*
    public Dictionary<string, string> readGainValues(string path)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();

        dict.Add("min", Utils.getLastLine("Assets/" + userID.ToString() + "_FinalResult.txt")); 
        dict.Add("max", Utils.getLastLine("Assets/" + userID.ToString() + "_FinalResult.txt"));

        return dict;
    }
    */


   /// <summary>
   /// Applies the correct redirection for each frame. When the user is within
   /// 1 meter of any target, ApplyRedirection applies either the minimum
   /// or maximum amount of gain, depending on which target.
   /// </summary>
    public override void ApplyRedirection()
    {
        float distance1 = Vector2.Distance(Utilities.FlattenedPos2D(player.position), Utilities.FlattenedPos2D(target1.position));
        float distance2 = Vector2.Distance(Utilities.FlattenedPos2D(player.position), Utilities.FlattenedPos2D(target2.position));
        float distance3 = Vector2.Distance(Utilities.FlattenedPos2D(player.position), Utilities.FlattenedPos2D(target3.position));

        if (distance1 < 1f)
        {
            InjectRotation(maxGain * redirectionManager.deltaDir);
        }

        else if (distance1 < 1f)
        {
            InjectRotation(minGain * redirectionManager.deltaDir);
        }

        else if (distance1 < 1f)
        {
            InjectRotation(maxGain * redirectionManager.deltaDir);
        }
    }
}
