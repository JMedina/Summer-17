using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redirection;
using System.Text;
using System.IO;
using System.Linq;

public class RotationTest : Redirector
{
    public GameObject yesButton;
    public GameObject noButton;

    //public static float minGain = -.99f;
    //public static float maxGain = .49f;
    static float currentGain = .5f;
    public static uint maxTrials;
    static int numLevels;

    

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


    

    public void pest()
    {

        //INITIALIZATION//
        //Easily detectable maximum stimulus value
        float stimRange = 2.0f;
        //each iteration of I jumps ( stimRange / numLevels )
        int numTrials = 4;

        //cumulative probability that threshold is at each of the possible values of the independent variable based on user responses
        float[] prob = new float[numLevels * 2]; 

        float[] plgit = new float[numLevels * 2]; //probablity of a positive response
        float[] mlgit = new float[numLevels * 2]; //probability of a negative response

        //estimate slope of psychometric function
        float std = stimRange / 5;

        for (int i = 0; i < 2 * numLevels; ++i)
        {
            prob[i] = 0;
            float lgit = 1 / (1 + Mathf.Exp((stimRange - (i * stimRange / numLevels)) / std)); //(i*stimRange/numLevels) is the iteration jump for stimulus values for each i
            plgit[i] = Mathf.Log(lgit);
            mlgit[i] = Mathf.Log(1 - lgit);
        }
    
        //Initialize response and stimulus level
        int response = -1; 
        float stimLevel = stimRange; 

        //Subroutine
        float max = float.MinValue;
        int p1 = numLevels, p2 = numLevels; //place where the max(s) is stored

        for (int j = 0; j < numLevels; ++j)
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
        for (int i = 0; i < numTrials; ++i)
        {
            //SUBRUITINE
            max = -1000f;
            p1 = numLevels;
            p2 = numLevels; //place where the max(s) is stored

            for (int j = 0; j < numLevels; ++j)
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
        }
    }


    public override void ApplyRedirection()
    {
        float deltaDir;
        deltaDir = redirectionManager.deltaDir;
        InjectRotation(currentGain * redirectionManager.deltaDir);
    }
}
