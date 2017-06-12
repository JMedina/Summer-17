using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redirection;
using System.Text;
using System.IO;
using System.Linq;

public class RotationTest : Redirector
{
    public static float minGain = -.99f;
    public static float maxGain = .49f;
    static float currentGain = .5f;

    public static uint maxTrials;
    static uint trialNum;

    public GameObject yesButton;
    public GameObject noButton;

    //Staircase specific variables
    static float adjustmentAmount = 0.5f;
    static bool lowering = true;
    /////////////////////////////

    //Constant Stimuli specific variables
    static int numLevels;
    ////////////////////////////

    //PEST specific variables

    ////////////////////////////

    private string getLastLine(string path)
    {
        string lastLine;
        using (StreamReader reader = new StreamReader("Assets/test.txt", Encoding.Default))
        {
            lastLine = File.ReadAllLines("file.txt").Last();
        }
        return lastLine;
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
    public void constantStimuli()
    {
        string lastLine = getLastLine("Assets/test.txt");
        //TODO: Print old gain and whether person recognized it into separate file

        float[] possibleLevels = new float[numLevels];
        for (int i = 0; i < numLevels; ++i)
        {
            possibleLevels[i] = minGain + (maxGain - minGain) * (i / numLevels);
        }




    }

    public void PEST()
    {

        //INITIALIZATION//
        //max stim value that it is surely detected
        float stimRange = 2.0f; 
        //each iteration of 1 jumps a certain step size which = ( stimRange / numLevels )
        int numTrials = 4;

        float[] prob = new float[numLevels * 2]; //cumulative probability that the threshold is at eaech of the possible values of the independent variable based on user responses

        float[] plgit = new float[numLevels * 2]; //psychometric function. probablity of a positive response
        float[] mlgit = new float[numLevels * 2]; //psychometric function. probability of a negative response

        float std = stimRange / 5; //estimate slope of psychometric function

        for ( int i = 0; i < 2*numLevels; i++)
        {
            prob[i] = 0;
            float lgit = 1 / (1 + Mathf.Exp((stimRange - (i*stimRange/numLevels)  ) / std)); //(i*stimRange/numLevels) is the iteration jump for stimulus values for each i
            plgit[i] = Mathf.Log(lgit);
            mlgit[i] = Mathf.Log(1 - lgit);
        }

        int response = -1; // initialize as a negative respose 
        float stimLevel = stimRange; // initialize 

        //SUBRUITINE
        float max = -1000f;
        int p1 = numLevels, p2 = numLevels; //place where the max(s) is stored

        for (int j = 0; j < numLevels; j++)
        {
            int iterator = (int)(stimLevel * numLevels / stimRange);
            if (response == 1)
            {
                prob[j] = prob[j] + plgit[numLevels + iterator - j];
            }
            else if (response == -1)
            {
                prob[j] = prob[j] + mlgit[numLevels + iterator - j];
            }

            if (prob[j] > max)
            {
                max = prob[j];
                p1 = j;
            }
            else if (prob[j] == max)
            {
                p2 = j;
            }

        }

        stimLevel = Mathf.FloorToInt((p1 + p2) * (stimRange / numLevels) / 2);
        ////////////////////

        stimLevel = 0;
        response = -1;
        ////////////////////

        //MAIN  PROGRAM
        for ( int i = 0; i < numTrials; i++)
        {
            //SUBRUITINE
            max = -1000f;
            p1 = numLevels;
            p2 = numLevels; //place where the max(s) is stored

            for (int j = 0; j < numLevels; j++)
            {
                int iterator = (int)(stimLevel * numLevels / stimRange);
                if (response == 1)
                {
                    prob[j] = prob[j] + plgit[numLevels + iterator - j];
                }
                else if (response == -1)
                {
                    prob[j] = prob[j] + mlgit[numLevels + iterator - j];
                }

                if (prob[j] > max)
                {
                    max = prob[j];
                    p1 = j;
                }
                else if (prob[j] == max)
                {
                    p2 = j;
                }

            }


            stimLevel = Mathf.FloorToInt((p1 + p2) * (stimRange / numLevels) / 2);
            ////////////////////

            //record response and write to file
            //1 if correct and -1 if incorrect
            currentGain = stimLevel;
            ApplyRedirection();

        }

        

    }


    public override void ApplyRedirection()
    {
        float deltaDir;
        deltaDir = redirectionManager.deltaDir;
        InjectRotation(currentGain * redirectionManager.deltaDir);
    }
}