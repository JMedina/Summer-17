using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Redirection;
using System.Text;
using System.IO;
using System.Linq;
using System;


public class NegativeTests : Redirector
{
    public GameObject yesButton;
    public GameObject noButton;

    public static float minGain = -.99f;
    public static float maxgain = .49f;
    public static float currentGain = -.95f;
    public static float stepSize = 0.1f;

    //Staircase specific variables
    public static uint maxReversals = 4;
    public static uint reversalNum = 0;
    public static float Zn = 0.0f;
    public static List<float> reversalList = new List<float>();

    //Stochastic specific variables
    public static float confidence = 0.5f;
    public static uint trialNum = 1;

    //PEST specific variables
    float stimRange = .79f;
    int numTrials;
    float[] prob;
    float[] plgit;
    float[] mlgit;
    float std;
    int response;
    float stimLevel;
    static int numLevels = 20;


    private string getLastLine(string path)
    {
        string lastLine;
        using (StreamReader reader = new StreamReader("Assets/test.txt", Encoding.Default))
        {

            string[] lineList = File.ReadAllLines("Assets/test.txt");
            for (int i = 0; i < lineList.Length; ++i)
            {
                //Debug.Log(lineList[i]);
            }
            lastLine = lineList.Last();
        }
        return lastLine;
        //return "bai";
    }

    void writeToFile(string path, string text)
    {
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(text);
        writer.Close();
    }




    public void PESTInitialization()
    {
        numLevels = 10;             // # of stimulus intervals to test.
        stimRange = currentGain;    //each iteration jumps a step size of ( stimRange / numLevels )
        std = stimRange / 5;        //estimated slope of psychometric function

        prob  = new float[numLevels * 2]; //cumulative probability that the threshold is at each possible value of the independent variable based on user responses
        plgit = new float[numLevels * 2]; //psychometric function. probablity of a positive response
        mlgit = new float[numLevels * 2]; //psychometric function. probability of a negative response


        //For each possible stimulus level, find the logarithmic probabilty
        //and populate the arrays with probabilities of positive and negative responses.
        float lgit;
        for (int i = 0; i < 2 * numLevels; ++i)
        {
            prob[i] = 0;
            lgit = 1 / (1 + Mathf.Exp((stimRange - ((i + 1) * stimRange / numLevels)) / std));
            plgit[i] = Mathf.Log(lgit);
            mlgit[i] = Mathf.Log(1 - lgit);
        }

        //Set initial response and stimulus level for subroutine
        response = 1;
        stimLevel = stimRange;
        PESTSubroutine();

        //Set them again cause yolo
        stimLevel = 0;
        response = -1;
    }


    public void PESTSubroutine()
    {
        float max = float.MinValue; //value of max probability
        int p1 = 0;                 //current index
        int p2 = 0;                 //index of max probability
        int iterator;               //index in probability array


        // For each possible stimulus level, update the probability 
        // for that threshold level according to the user's response.
        for (int j = 0; j < numLevels; ++j)
        {
            iterator = (int)(stimLevel * numLevels / stimRange);
            if (response == 1)
            {
                prob[j] += plgit[numLevels + iterator - j - 1];
            }
            else if (response == -1)
            {
                prob[j] += mlgit[numLevels + iterator - j - 1];
            }

            //If the probability for this level of gain is greater
            //than the previously recorded maximum, set new max.
            if (prob[j] > max)
            {
                max = prob[j];
                p2 = j;
            }
            else
            {
                p1 = j;
            }
        }

        stimLevel = (p1 + p2) * (stimRange / numLevels) / 2;
        Debug.Log("gain = "+stimLevel+" = (" + p1 + "+" + p2 + ") * (" + stimRange + "/" + numLevels + ")/2");
        //Debug.Log("gain = " + stimLevel);
    }


    //This function updates the level of gain and records the user's response.
    // it also checks the confidence level to end the test when ready.
    public void PEST()
    {
        PESTSubroutine();
        currentGain = stimLevel;

        string lastLine = getLastLine("Assets/test.txt");
        writeToFile("Assets/results.txt", lastLine + " " + Convert.ToString(currentGain));
        response = (lastLine == yesButton.name) ? 1 : -1;

        float fullSum = 0;
        float sum = 0;

        for (int i = 0; i < numLevels; ++i)
        {
            fullSum += prob[i];
        }

        //Gets the standard deviation 
        int index;
        for (int i = -2; i < 2; ++i)
        {
            index = (int)(currentGain * (numLevels / stimRange));
            sum += prob[index + i];
        }

        if (fullSum * 0.95 <= sum)
        {
            //DONE
        }
    }


    //Staircase Method
    //The staircase adjusts the stimulus level according to the formula
    //which steps at a fixed step size, and changes direction if the
    //response changes.  It stops after a predetermined number of 
    //reversals. The threshold estimate is the average of the reveral points.
    //Stepping rule:  Xn+1 = Xn - d(2 * Zn - 1)
    //Xn is the stimulus level at trial n,
    //d is a fixed step size
    //Zn is the response at trial n.
    //Zn = 1 -> stimulus detected/correct
    //Zn = 0 -> stimulus not detected
    public void staircase()
    {
        string lastLine = getLastLine("Assets/test.txt");
        writeToFile("Assets/results.txt", lastLine + Convert.ToString(currentGain));

        float newZ = (lastLine == noButton.name) ? 1.0f : 0.0f;
        if (newZ != Zn)
        {
            reversalList.Add(currentGain);
            Debug.Log("Reversed at gain: " + Convert.ToString(reversalList.Last()));
            if (reversalList.Count == maxReversals)
            {
                Debug.Log("Done: " + Convert.ToString(reversalList.Average()));
            }
        }
        Zn = newZ;
        Debug.Log(currentGain);
        currentGain -= stepSize * (2 * Zn - 1);
    }

    public void ApplyRotation()
    {
        Debug.Log("neg");
        InjectRotation(currentGain* redirectionManager.deltaDir);
    }

    public override void ApplyRedirection()
    {
    }

}

    