using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using System.IO;
using System;

public class ShadowContrastDataManager : MonoBehaviour
{
    public UIManagement4ShadowContrastExp UIManager;
    public AudioManager audioManager;

    public UpdateText4ShadowContrastExp trialText;
    public UpdateText4ShadowContrastExp breakText;
    public UpdateText4ShadowContrastExp instructText;

    // UI / Filewriter vars
    public string filename;
    public List<GameObject> inputFields;


    // Subject vars to write 
    public string subjectId;
    string deviceId;


    public int[,] randomized_vars;
    public string[,] recorded_vars;


    public int cur_trial;
    public int cur_subtrial;

    bool isExperimentStarted;
    bool isNextTrial;
    private float time;

    // trial vals
    private readonly int totalSubtrials = 64;
    private readonly int totalTrials = 2; 

    public GameObject MyShadowManagerGO; // assign in inspector *shrug* 
    ShadowContrastExperimentManager MyShadowManager;
    ShadowAnchorManager MyShadowAnchorManager;



    void Start()
    {

        subjectId = "";
        deviceId = "";

        MyShadowManager = MyShadowManagerGO.transform.GetComponent<ShadowContrastExperimentManager>();
        MyShadowAnchorManager = MyShadowManagerGO.transform.GetComponent<ShadowAnchorManager>();
        MyShadowManagerGO.SetActive(false); 
        isExperimentStarted = false;
 
        cur_subtrial = 0; // max = totalSubtrials
        cur_trial = 0; // max = totalTrials    


        Debug.Log("RAND TRIALS TESTING");
        RandomizeTrials();
    }


    // Set filename and path.
    // Create file if it doesn't exist.
    // Toss error if file does exist. 
    // ughh what a train wreck. this does more than create a file at this point. 
    public void CreateFile()
    {
        if (!isExperimentStarted)
        {
            SetFields();
            // USE THIS FOR HOLOLENS 
            filename = Path.Combine(Application.persistentDataPath, deviceId + "_" + subjectId  + ".txt");
            // just  for debugging on computer vv 
            //filename = Application.dataPath + "/Data/contact_" + deviceId + "_" + subjectId + ".txt"; // use subj & exp ids

            Debug.Log(filename);

            Debug.Log("Start the Experiment.");
            // Create a file to write to 
            using (StreamWriter sw = File.CreateText(filename))
            {
                // Experiment Information 
                sw.WriteLine("SubjectID, DeviceId");
                sw.WriteLine(subjectId + ", " + deviceId );

                // Data Labels 
                sw.WriteLine("RepetitionIndex, RandomizedTrials [[color_condition, height1, height2, user_response, elapsed_time], ...]");
                //sw.Dispose();
            }
        }
        StartBlockBreak();
        UpdateShaders();
    }


    
    public void StartBlockBreak()
    {
        isNextTrial = false;
        MyShadowManagerGO.SetActive(false);
        audioManager.MuteBackgroundMusic(false);

        trialText.UpdateTrialText(cur_trial, cur_subtrial+1);        
        breakText.StartBreakText(cur_trial);

        if (cur_trial > totalTrials)
            EndExperiment();
    
    }
    

    public void StartTrialBreak()
    {
        audioManager.PlayRoundNumber(cur_trial);

        isNextTrial = false;
        MyShadowManagerGO.SetActive(false); 

        trialText.UpdateTrialText(cur_trial, cur_subtrial+1);
        breakText.UpdateBreakText(cur_trial);

        Debug.Log("TRIAL BREAK"); 
        UIManager.BreakMenu();
    }

    private void StartTrialNoBreak()
    {
        isNextTrial = false;
        trialText.UpdateTrialText(cur_trial, cur_subtrial + 1);
        ResetTrials(); 
    }

    // manually assign inputFields in inspector (lame, i know)
    public void SetFields()
    {
        subjectId = inputFields[0].transform.GetChild(0).GetComponent<Text>().text;
        deviceId = "HoloLens";
        RandomizeTrials();
    }


    public void RandomizeTrials()
    {
        // 16 total combinations [all pairs of heights and reversed orders]
        var ar_combinations = new int[,] {
            {99,0,0},{99,1,0},{99,2,0},{99,3,0},
            {99,0,1},{99,1,1},{99,2,1},{99,3,1},
            {99,0,2},{99,1,2},{99,2,2},{99,3,2},
            {99,0,3},{99,1,3},{99,2,3},{99,3,3},
        };
        randomized_vars = new int[ar_combinations.Length * 4,3];
        var array_str = "";

        int index = 0; 
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                randomized_vars[index, 0] = i;
                randomized_vars[index, 1] = ar_combinations[j,1];
                randomized_vars[index, 2] = ar_combinations[j,2];
                array_str += "[" + randomized_vars[index, 0] + "," + randomized_vars[index, 1] + "," + randomized_vars[index, 2] + "] ";
                index += 1;
            }
        }
        ArrayPrintCheck("OG");
        Shuffle_Matrix(randomized_vars);
        Shuffle_Matrix(randomized_vars);
        ArrayPrintCheck("SHUFFLED");

        recorded_vars = new string[ar_combinations.Length * 4, 2];

    }


    public void ArrayPrintCheck(string label)
    {
        var array_str = "";
        for (int i = 0; i < totalSubtrials; i++)
        {
            array_str += "[" + randomized_vars[i, 0] + "," + randomized_vars[i, 1] + "," + randomized_vars[i, 2] + "] ";
        }
        Debug.Log(label + ": " + array_str);
    }


    public void Shuffle_Matrix(int[,] array)
    {
        for (int i = 0; i < totalSubtrials; i++)
        {
            int j = UnityEngine.Random.Range(i, totalSubtrials);
            List<int> temp = new List<int> { array[i, 0], array[i, 1], array[i,2] };
            array[i, 0] = array[j, 0];
            array[i, 1] = array[j, 1];
            array[i, 2] = array[j, 2];

            array[j, 0] = temp[0];
            array[j, 1] = temp[1];
            array[j, 2] = temp[2];
        }
    }

   void Update()
    {
        time += Time.deltaTime; // reset for every sub trial
        if (Input.anyKeyDown && cur_trial > totalTrials)
        {
            EndExperiment(); 
            return; 
        }
    }

    void  EndExperiment()
    {
        isExperimentStarted = false;
        isNextTrial = false; 

        Debug.Log("You're all done!");
        audioManager.PlayGameOver(); 
        breakText.SetFinalText();
        UIManager.SetFinalInstruction(); 
    }


    public void ResetTrials()
    {
        if (isNextTrial)
            return;

        UpdateRecordedValues(99); // idk
        isExperimentStarted = true;


        RandomizeTrials();
        cur_trial += 1;
        isNextTrial = true;
        cur_subtrial = 0;
        time = 0;


        if (cur_trial > totalTrials)
        {
            EndExperiment();
            return;
        }

        Debug.Log("Trial: " + cur_trial + "/" + totalTrials);

        trialText.UpdateTrialText(cur_trial, cur_subtrial+1);
        breakText.UpdateBreakText(cur_trial);

        if (cur_subtrial == 0)
        {
            MyShadowManagerGO.SetActive(true);
            UpdateShaders();
        }

    }


    public void NextTrial()
    {
        if (!isNextTrial || !isExperimentStarted)
            return;

        if (cur_subtrial >= totalSubtrials-1) //>= 64)
        {
            isNextTrial = false;
            MyShadowManagerGO.SetActive(false);
        }

    }
    

    public void GetAudioPerformanceFeedback(int response_num)
    {
        int first_height = randomized_vars[cur_subtrial, 1];
        int second_height = randomized_vars[cur_subtrial, 2];
        //Debug.Log("User: " + response_num + " [" + first_height + ", " + second_height + "] at trial " + cur_subtrial);
        if ((response_num == 0 && (first_height <= second_height)) ||
             (response_num == 1 && (first_height >= second_height)))
        {
            audioManager.PlayCorrectSound(); }
        else
        {
            audioManager.PlayIncorrectSound(); }   
    }


    public void UpdateRecordedValues(int likert_num)
    {

        if (cur_subtrial >= totalSubtrials) //> 64)
            isNextTrial = false;

        if (likert_num == 99) // delete? 
            return;

        GetAudioPerformanceFeedback(likert_num);        
        Debug.Log("SubTrial: " + cur_subtrial + "/" + totalSubtrials);
        Debug.Log(likert_num + " , " + time); 

        recorded_vars[cur_subtrial, 0] = likert_num.ToString();
        recorded_vars[cur_subtrial, 1] = time.ToString();
        time = 0;
        NextTrial();
        cur_subtrial += 1;

        if (cur_subtrial < totalSubtrials)
        {
            trialText.UpdateTrialText(cur_trial, cur_subtrial+1);
            UpdateShaders();

            /*if (cur_subtrial > 0 && (cur_subtrial % 32 == 0))
            {
                Debug.Log(" mod 32");
            }
            else if (cur_subtrial > 0 && (cur_subtrial % 16 == 0))
            {
                Debug.Log(" mod 16");
            }*/
        }

        if (!isNextTrial && cur_subtrial >= totalSubtrials-1) 
        {
            WriteTrialInformation();

            Debug.Log("WRITE TRIAL INFORMATION bc subtrial is " + cur_subtrial);
            if (cur_trial >= totalTrials)
            { 
                EndExperiment();
                return; 
            }
            else //  
            {
                Debug.Log("TRIAL RESET. StartTrialBreak ");
                StartTrialBreak();
            }

            /*
            if (cur_trial > 0 && (cur_trial % (totalTrials / 3) == 0))
            {
                Debug.Log("START NEXT BLOKKKK");
                StartBlockBreak();
            }
            else if ((cur_trial % 2 == 0)) // even trial _ break 
            {
                Debug.Log("EVEN TRIAL RESET");
                StartTrialBreak();
            }
            else // odd trial _ continue
            {
                Debug.Log("NORMAL TRIAL");
                StartTrialNoBreak(); 
            }*/

            cur_subtrial = 0;
            return;
        }
        else if (!isNextTrial)
            return;

    } 


    // write randomized trial
    void WriteTrialInformation()
    {
        var array_str = "[";
        for (int i = 0; i < totalSubtrials; i++)
        {
            array_str += "[" + randomized_vars[i, 0] + "," + randomized_vars[i, 1] + "," + randomized_vars[i, 2] +
                "," + recorded_vars[i, 0] + "," + recorded_vars[i, 1] +
                "], ";
        }
        array_str += "]";

        Debug.Log(array_str); 

        using (StreamWriter sw = File.AppendText(filename))
        {
            sw.WriteLine(cur_trial + ", " + array_str);
            //sw.WriteLine(" ");
            //sw.WriteLine(condition);
        }
        
    }

    // index into array take subtrial - 1
    public void UpdateShaders()
    {
        MyShadowManager.SetMaterials(randomized_vars[cur_subtrial, 0]);    
        MyShadowManager.SetHeights(randomized_vars[cur_subtrial, 1], 0);
        MyShadowManager.SetHeights(randomized_vars[cur_subtrial, 2], 1);

        ///MyShadowAnchorManager.SetPositionRelativeToAnchor(cur_block); // unnecessary? 
        //MyShadowAnchorManager.UpdateDistance(); // causes anchors to reset 
    }

}