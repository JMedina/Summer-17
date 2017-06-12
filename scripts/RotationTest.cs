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
    static float stepSize = 0.5f;
    static bool lowering = true;
    /////////////////////////////

    //Constant Stimuli specific variables
    static int numLevels;
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

        float [] possibleLevels = new float[numLevels];
        for(int i = 0; i < numLevels; ++i)
        {
            possibleLevels[i] = minGain + (maxGain - minGain) * (i / numLevels);
        }
    }





    public void staircase() {

        string lastLine = getLastLine("Assets/test.txt");

        writeToFile("Assets/results.txt", lastLine); //Todo: copy gain amount

        if (lastLine == yesButton.name){
            stepSize = (lowering == true) ? stepSize : (stepSize / 2);
            lowering = true;
        }

        if (lastLine == noButton.name){
            stepSize = (lowering == false) ? stepSize : (stepSize / 2);
            lowering = false;
        }

        currentGain = (lowering == true) ? (currentGain - stepSize) : (currentGain + stepSize);
    }



    public override void ApplyRedirection()
    {
        float deltaDir;
        deltaDir = redirectionManager.deltaDir;
        InjectRotation(currentGain * redirectionManager.deltaDir);
    }
}
