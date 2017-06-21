using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public static float currentGain = 2.49f;
    public static float stepSize = 0.1f;

    //Staircase specific variables
    public static uint maxReversals = 10;
    public static uint reversalNum = 0;
    public static float Zn = 1.0f;
    public static List<float> reversalList = new List<float>();

    //Stochastic specific variables
    public static float confidence = 0.5f;
    public static uint trialNum = 1;




    /*private string getLastLine(string path)
    {
        string lastLine;
        using (StreamReader reader = new StreamReader("Assets/test.txt", Encoding.Default))
        {
            lastLine = File.ReadAllLines("file.txt").Last();
        }
        return lastLine;
    }*/

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

        float newZ = (lastLine == yesButton.name) ? 1.0f : -1.0f;
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



public override void ApplyRedirection()
{
    float    deltaDir = redirectionManager.deltaDir;
    InjectRotation((currentGain * deltaDir) + deltaDir);

    }
}
