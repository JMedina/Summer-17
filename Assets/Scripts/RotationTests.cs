using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Redirection;
using System.Text;
using System.IO;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class RotationTests : Redirector
{
    public GameObject yesButton;
    public GameObject noButton;

    public string userID;

    public static bool isNegative = false;
    public static bool isDone = false;

    public static float currentGain = .79f;
    public static float stepSize = 0.1f;

    //Staircase specific variables
    public static uint maxReversals = 6;           //Keep this value at 6 or 8. must be an even number for balance.
    public static uint reversalNum = 0;
    public static float Zn = 1.0f;
    public static List<float> reversalList = new List<float>();

    //Stochastic specific variables
    public static float confidence = 0.5f;
    public static uint trialNum = 1;

    //PEST specific variables
    float stimRange;                                //Range of stimulus values to test. Used when changing scale
    static int numLevels = 20;                      //# of stimulus intervals to test = RANGE
    float std;                                      //estimated slope of psychometric function
    int stimLevel;                                  //Level of stimulus to present to user
    int response;                                   //Collect's the user's response

    float[] prob = new float[numLevels * 2 + 1];    //cumulative probability that threshold is at each possible value of independent variable based on user responses
    float[] plgit = new float[numLevels * 2 + 1];   //probability of a positive response
    float[] mlgit = new float[numLevels * 2 + 1];   //probability of a negative response



   /// <summary>
    /// Initializes variables to correct value to measure negative gain.
    /// Called when negative tests are selected in Inspector.
    /// </summary>
    public void setNegative()
    {
        Debug.Log("setting negative");
        currentGain = -.4f;
        Zn = 0.0f;
        reversalList.Clear();
        isNegative = true;
        isDone = false;
    }

    public bool getIsDone()
    {
        return isDone;
    }

    public bool getIsNeg()
    {
        return isNegative;
    }

    /// <summary>
    /// Initializes the arrays and constants needed for the PEST algorithm.
    /// </summary>
    public void PESTInitialization()
    {

        stimRange = currentGain * 2; //Max gain value to try
        std = numLevels / 5;         //Reasonable estimation of slope (can be adjusted)


        //Initialize prob of positive response and negative responses from the psychometric function
        for (int i = 0; i < numLevels * 2; ++i)
        {
            prob[i] = 0;
            float lgit = 1 / (1 + Mathf.Exp((numLevels - i) / std));

            plgit[i] = lgit;
            mlgit[i] = 1 - lgit;
        }

        //Set initial response and stimulus levels. Start at min stimulus value
        stimLevel = 0;
        response = -1;


        PESTSubroutine(); //Initialize probability array for the highest stim value

        //assume that stim = numlevels will be felt
        stimLevel = numLevels;
        response = 1;
    }



    public void PESTSubroutine()
    {
        float max = float.MinValue; //Value of max probability
        float p1 = 0f, p2 = 0f;     //Indices of max probability


        //iterate through posible values and adjust cumulative prob based on response
        for (int i = 0; i < numLevels; ++i)
        {
            //update probabilities based on response
            if (response == 1)
            {
                prob[i] += plgit[numLevels + stimLevel - i];
            }
            else if (response == -1)
            {
                prob[i] += mlgit[numLevels + stimLevel - i];
            }


            //find maximum liklihood
            if (prob[i] > max)
            {
                max = prob[i];
                p1 = i;
            }
            else if (prob[i] == max)
            { //when multiple indices have same max prob set p2
                p2 = i;
            }

        }

        //average in case there are multiple max probs
        stimLevel = (int)Mathf.Floor((p1 + p2) / 2);
        //print("New value: " + stimLevel + " =  " + p1 + " + " + p2 + " / 2");

    }

    /// <summary>
    /// 
    /// </summary>
    public void PEST()
    {

        //get response from last trial
        string lastLine = Utils.getLastLine("Assets/" + userID + "_Test.txt");
        Utils.writeToFile("Assets/" + userID + "_Results.txt", Convert.ToString(currentGain));
        response = (lastLine == yesButton.name) ? 1 : -1;

        //add to the cumulative probability and find new response
        PESTSubroutine();

        //set new value to try
        currentGain = stimLevel * (stimRange / numLevels);
        print("Gain: " + currentGain);

        //TERMINATION CRITERIA//
        float fullSum = 0;
        float sum = 0;

        for (int i = 0; i < numLevels * 2; ++i)
        {
            fullSum += prob[i];
        }

        //Get sum of probabilities 2 standard dev away
        for (int i = -3; i < 3; ++i)
        {
            if (stimLevel - i >= 0)
            {
                sum += prob[stimLevel - i];
            }
        }

        
        if (Mathf.Log(fullSum) * 0.775f <= Mathf.Log(sum))
        {
            print("DONE");
            isDone = true;
            if (isNegative)
            {
                Utils.writeToFile("Assets/" + userID + "_FinalResult.txt", Convert.ToString(currentGain));
                SceneManager.LoadScene("verification experience");
            }
            else
            {
                Utils.writeToFile("Assets/" + userID + "_FinalResult.txt", Convert.ToString(currentGain));
            }
        }

    }

    /// <summary>
    /// Staircase Method
    /// The staircase adjusts the stimulus level according to the formula
    /// which steps at a fixed step size, and changes direction if the
    /// response changes.  It stops after a predetermined number of 
    /// reversals. The threshold estimate is the average of the reveral points.
    /// Stepping rule:  Xn+1 = Xn - d(2 * Zn - 1)
    /// Xn is the stimulus level at trial n,
    /// d is a fixed step size
    /// Zn is the response at trial n.
    /// Zn = 1 -> stimulus detected/correct
    /// Zn = 0 -> stimulus not detected
    /// </summary>
    /// <param name="isPositive"></param>
    public void staircase(bool isPositive)
    {
        string lastLine = Utils.getLastLine("Assets/" + userID + "_Test.txt");
        Utils.writeToFile("Assets/" + userID + "_Results.txt", Convert.ToString(currentGain));

        float newZ;
        if (isPositive)
        {
            newZ = (lastLine == noButton.name) ? 0.0f : 1.0f;
        }
        else
        {
            newZ = (lastLine == yesButton.name) ? 0.0f : 1.0f;
        }

        if (newZ != Zn)
        {
            reversalList.Add(currentGain);
            Debug.Log("Reversed at gain: " + Convert.ToString(reversalList.Last()));
            if (reversalList.Count == maxReversals)
            {
                isDone = true;
                Debug.Log("Done: " + Convert.ToString(reversalList.Average()));
                   if (isNegative)
                {
                    Utils.writeToFile("Assets/" + userID + "_FinalResults.txt", "Negative threshold for Staircase is " + Convert.ToString(reversalList.Average()));
                    SceneManager.LoadScene("verification experience");
                } else
                {
                    Utils.writeToFile("Assets/" + userID + "_FinalResults.txt", "Positive threshold for Staircase is " + Convert.ToString(reversalList.Average()));
                }
            }
        }
        Zn = newZ;
        Debug.Log(currentGain);
        currentGain -= stepSize * (2 * Zn - 1);
    }

    /// <summary>
    /// 
    /// </summary>
    //Stochastic approximation
    //a type of modified binary search
    //Step size decreases from trial to trial
    //Stepping rule: Xn+1 = Xn - d * (Zn - phi) / n
    //d is the initial step size
    //Zn is response at trial n.
    //Zn = 1 -> stimulus detected/correct
    //Zn = 0 -> stimulus not detected
    public void stochastic()
    {
        string lastLine = Utils.getLastLine("Assets/" + userID + "_Test.txt");
        Utils.writeToFile("Assets/" + userID + "_Results.txt", Convert.ToString(currentGain));

        Zn = (lastLine == yesButton.name) ? 1.0f : -1.0f;

        currentGain -= stepSize * (Zn - confidence) / trialNum;
        ++trialNum;
    }


    //Accelerated stochastic approximation
    //improvement on standard stochastic approximation
    //theoretically converges in fewer trials
    //Step rule: Xn+1 = Xn - d *(Zn - phi) / (2 + m)
    //d is initial step size
    //Zn is response at trial n
    //Zn = 1 -> stimulus detected/correct
    //Zn = 0 -> stimulus not detected.
    public void acceleratedStochastic()
    {
        if (trialNum <= 2)
        {
            stochastic();
            return;
        }

        string lastLine = Utils.getLastLine("Assets/" + userID + "_Test.txt");
        Utils.writeToFile("Assets/" + userID + "_Results.txt", Convert.ToString(currentGain));

        float newZn = (lastLine == yesButton.name) ? 1.0f : -1.0f;
        if (newZn != Zn)
        {
            ++reversalNum;
        }

        Zn = newZn;

        currentGain -= stepSize * (Zn - confidence) / (2 + reversalNum);
    }




    public override void ApplyRedirection()
    {
        
        InjectRotation(currentGain * redirectionManager.deltaDir);
    }

}
