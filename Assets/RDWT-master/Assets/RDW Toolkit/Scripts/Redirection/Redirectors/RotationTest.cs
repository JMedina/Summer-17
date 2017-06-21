using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redirection;
using System.Text;
using System.IO;
using System.Linq;
using System;

public class RotationTest : Redirector
{
    public RotationTest rotationTest;

    public static float minGain = -.99f;
    public static float maxGain = .49f;
    static float currentGain = .5f;

    public static uint maxTrials;
    static uint trialNum;

    public GameObject yesButton;
    public GameObject noButton;

    //Pentland specific variables
    public static int numLevels;
    public static float range = maxGain - minGain;
    public static float stepSize = range / (float)numLevels;
    public static float[] prob = new float[numLevels];
    public static float[] mlgit = new float[numLevels];
    public static float[] plgit = new float[numLevels];
    public static float std = numLevels / 10.0f;


    //Staircase specific variables
    //static float stepSize = 0.5f;
    static bool lowering = true;
    /////////////////////////////

    //Constant Stimuli specific variables
    //static int numLevels;
    ////////////////////////////


    /*private string getLastLine(string path)
    {
        string lastLine;
        Debug.Log("Entering getlastLine");
        using (StreamReader reader = new StreamReader("Assets/test.txt", Encoding.Default))
        {
            Debug.Log("Initialized streamreader");
            lastLine = reader.ReadAllLines().Last();
        }
        Debug.Log("Last line is: " + lastLine);
        return lastLine;
    }*/

    private string getLastLine(string path)
    {
        string lastLine;
        using (StreamReader reader = new StreamReader("Assets/test.txt", Encoding.Default))
        {

            string[] lineList = File.ReadAllLines("Assets/test.txt"); 
            for(int i = 0; i < lineList.Length; ++i)
            {
                Debug.Log(lineList[i]);
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

    /// <summary>
    /// A set of equally spaced levels of the stimulus intensities is chosen
    /// (usually 5-9). Each level is repeated large number of times. The
    /// subject is asked to report whether the presented stimulus can be
    /// detected(when AL is measured), or whether the intensity of the
    /// presented test stimulus is greater than that of the reference stimulus
    /// (when DL is measured).
    /// </summary>
    /*public void constantStimuli()
    {
        string lastLine = getLastLine("Assets/test.txt");
        //TODO: Print old gain and whether person recognized it into separate file

        float [] possibleLevels = new float[numLevels];
        for(int i = 0; i < numLevels; ++i)
        {
            possibleLevels[i] = minGain + (maxGain - minGain) * (i / numLevels);
        }
    }*/

/*
    public void initializePentland()
    {
        for (int i = 0; i < numLevels; ++i)
        {
            float currentLevel = minGain + (stepSize * i);
            prob[i] = 0;
            float lgit = (float)(1.0f / (1.0f + Math.Exp(maxGain - (stepSize * i) / std)));

            plgit[i] = (float)Math.Log(lgit);
            mlgit[i] = (float)Math.Log(1 - lgit);
        }
        currentGain = minGain;
    }


    public void pentland()
    {
        for (int i = 0; i < numLevels; ++i)
        {
            float maxVal = float.MinValue;
            for (int j = 0; j < numLevels; ++j)
            {
                string lastLine = getLastLine("Assets/test.txt");

                if (lastLine == yesButton.name)
                {
                    prob[j] += plgit(maxGain + currentGain - currentGain)
                }

            }
        }
    }
    */


    public void staircase() {
        Debug.Log("Im in the staircase");
        string lastLine = getLastLine("test.txt");

        writeToFile("Assets/results.txt", lastLine + Convert.ToString(currentGain)); //Todo: copy gain amount

        Debug.Log("Last line of file was: " + lastLine);

        if (lastLine == yesButton.name) {
            stepSize = (lowering == true) ? stepSize : (stepSize / 2);
            lowering = true;
        }

        if (lastLine == noButton.name) {
            stepSize = (lowering == false) ? stepSize : (stepSize / 2);
            lowering = false;
        }

        currentGain = (lowering == true) ? (currentGain - stepSize) : (currentGain + stepSize);
        Debug.Log("Exiting the staircase");
    }


    //Accelerated stochastic approximation
    //the accelerated stochastic approximation works by choosing stimulus according
    //to the stepping rule
    // Xn1 = Xn - c * (Zn- phi) / (2 + m)
    //Xn is stimulus level at trial n
    //Zn is response at trial n. 
    // Zn = 1 -> stimulus detected/correct
    // Zn = 0 -> stimulus not detected
    //Phi is the targe probability
    //For 2AFC the target probability is 75%
    public void stochastic()
    {
        float initialStepSize = 0.5f;
        string lastLine = getLastLine("Assets/test.txt");




    }



    public override void ApplyRedirection()
    {
        //rotationTest.staircase();
        float deltaDir;
        deltaDir = redirectionManager.deltaDir;
        InjectRotation(currentGain * redirectionManager.deltaDir);
    }
}
