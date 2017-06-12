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
    public static float currentGain = .49f;
    public static float stepSize = -.2f;
    
    //Staircase specific variables
    public static uint maxReversals;
    public static uint reversalNum = 0;
    public static float Zn = 1.0f;
    public static list<float> reversalList;
    
    //Stochastic specific variables
    public static float confidence = 0.5f;
    public static uint trialNum = 1;
    
    
    
    
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
    public void staircase(){
        string lastLine = getLastLine("Assets/test.txt");
        writeToFile("Assets/results.txt", lastLine + Convert.ToString(currentgain));
        
        float newZn = (lastLine == yesButton.name) ? 1.0f : -1.0f;
        if(newZn != Zn){
            ++reversalNum;
            reversalList.Add(currentGain);
            if(reversalNum == maxReversals){
                Debug.Log("Done: " + Convert.ToString(reversalList.Average()));
            }
        }
        Zn = newZn;
    
        currentGain -= stepSize * (2*Zn -1);
    }
    
    
    
    //Stochastic approximation
    //a type of modified binary search
    //Step size decreases from trial to trial
    //Stepping rule: Xn+1 = Xn - d * (Zn - phi) / n
    //d is the initial step size
    //Zn is response at trial n.
    //Zn = 1 -> stimulus detected/correct
    //Zn = 0 -> stimulus not detected
    public void stochastic(){
        string lastLine = getLastLine("Assets/test.txt");
        writeToFile("Assets/results.txt", lastLine + Convert.ToString(currentGain));
        
        float Zn = (lastLine == yesButton.name) ? 1.0f : -1.0f;
        
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
    public void acceleratedStochastic(){
        if(trialNum <= 2){
            stochastic();
            return;
        }
        
        string lastLine = getLastLine("Assets/test.txt");
        writeToFile("Assets/results.txt", lastLine + Convert.ToString(currentGain));
                
        float newZn = (lastLine == yesButton.name) ? 1.0f : -1.0f;
        if(newZn != Zn){
            ++reversalNum;
            }
        }
        Zn = newZn;
        
        currentGain -= stepSize * (Zn - confidence) / (2 + reversalNum);
    }

 
    
    public override void ApplyRedirection()
    {
        float deltaDir;
        deltaDir = redirectionManager.deltaDir;
        InjectRotation(currentGain * redirectionManager.deltaDir);
    }
}
