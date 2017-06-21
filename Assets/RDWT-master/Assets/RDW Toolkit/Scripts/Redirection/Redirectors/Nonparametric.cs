using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Redirection;
using System.Text;
using System.IO;
using System.Linq;
using System;


public class Nonparametric : Redirector
{
    public GameObject yesButton;
    public GameObject nobutton;

    public static float minGain = -.99f;
    public static float maxgain = .49f;
    public static float currentGain = .79f;
    public static float stepSize = 0.1f;

    public Text doneText;

    //Staircase specific variables
    public static uint maxReversals = 4;
    public static uint reversalNum = 0;
    public static float Zn = 1.0f;
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
    static int numLevels = 10; //CHANGE THIS

    public void setStimLevel(float stimLevel)
    {
        this.stimLevel = stimLevel;
    }
    public void setResponse(int response)
    {
        this.response = response;
    }


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


    //Staircase to find lower threshold
    public void staircase()
    {
        string lastLine = getLastLine("Assets/test.txt");
        writeToFile("Assets/results.txt", lastLine + Convert.ToString(currentGain));

        float newZ = (lastLine == yesButton.name) ? 1.0f : 0.0f;
        if (newZ != Zn)
        {
            reversalList.Add(currentGain);
            Debug.Log("Reversed at gain: " + Convert.ToString(reversalList.Last()));
            if (reversalList.Count == maxReversals)
            {
                Debug.Log("Done: " + Convert.ToString(reversalList.Average()));
                doneText.text = "DONE!";
            }
        }
        Zn = newZ;
        Debug.Log(currentGain);
        currentGain -= stepSize * (2 * Zn - 1);

        
    }


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
        string lastLine = getLastLine("Assets/test.txt");
        writeToFile("Assets/results.txt", lastLine + Convert.ToString(currentGain));
        
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

        string lastLine = getLastLine("Assets/test.txt");
        writeToFile("Assets/results.txt", lastLine + Convert.ToString(currentGain));

        float newZn = (lastLine == yesButton.name) ? 1.0f : -1.0f;
        if (newZn != Zn)
        {
            ++reversalNum;
        }
    
        Zn = newZn;
        
        currentGain -= stepSize* (Zn - confidence) / (2 + reversalNum);
    }


    public void QUEST()
    {
        
    }

    public void PEST()
    {
        
        PESTSubroutine();
        
        currentGain = stimLevel;

        Debug.Log("testing gain of " + currentGain);

        string lastLine = getLastLine("Assets/test.txt");
        writeToFile("Assets/results.txt", lastLine + Convert.ToString(currentGain));

        response = (lastLine == yesButton.name) ? 1 : -1;

        float sum = 0;

        for (int i = -2; i < 2; i++)
        {
            int iteration = (int)(currentGain * (stimRange / numLevels));
            sum = prob[iteration + i];
        }

    }

    public void PESTInitialization()
    {

        //each iteration of 1 jumps a certain step size which = ( stimRange / numLevels )
        numTrials = 4;

        prob = new float[numLevels * 2]; //cumulative probability that the threshold is at eaech of the possible values of the independent variable based on user responses

        plgit = new float[numLevels * 2]; //psychometric function. probablity of a positive response
        mlgit = new float[numLevels * 2]; //psychometric function. probability of a negative response

        std = stimRange / 5; //estimate slope of psychometric function

        for (int i = 0; i < 2 * numLevels; i++)
        {
            prob[i] = 0;
            float lgit = 1 / (1 + Mathf.Exp((stimRange - (i * stimRange / numLevels)) / std)); //(i*stimRange/numLevels) is the iteration jump for stimulus values for each i
            plgit[i] = Mathf.Log(lgit);
            mlgit[i] = Mathf.Log(1 - lgit);
        }

        response = -1; // initialize as a negative respose 
        stimLevel = stimRange; // initialize 

    }

    public void PESTSubroutine()
    {
        float max = -1000f;
        int p1 = numLevels, p2 = numLevels; //place where the max(s) is stored

        for (int j = 0; j < numLevels; j++)
        {
            int iterator = (int)(stimLevel * numLevels / stimRange);
            if (response == 1)
            {
                prob[j] = prob[j] + plgit[numLevels + iterator - j -1];
            }
            else if (response == -1)
            {
                prob[j] = prob[j] + mlgit[numLevels + iterator -j -1];
            }

            if (prob[j] > max)
            {
                max = prob[j];
                //p1 = j;
            }
            else
            {
                p1 = j; 
            }
            
            if (prob[j] == max)
            {
                p2 = j;
            }

        }

        //Mathf.FloorToInt( )
        stimLevel = (p1 + p2) * (stimRange / numLevels) / 2;
        Debug.Log("stim level = (" + p1 + " + " + p2 + ") * (" + stimRange + "/" + numLevels + ") /2");

    }


    public override void ApplyRedirection()
{
    float    deltaDir = redirectionManager.deltaDir;
        
    InjectRotation((currentGain * deltaDir));

    }
}
