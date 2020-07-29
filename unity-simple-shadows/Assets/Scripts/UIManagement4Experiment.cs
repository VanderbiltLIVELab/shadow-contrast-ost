using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UIManagement4Experiment : MonoBehaviour {


    public List<Transform> MainMenuChildren;
    public SetWorldAnchor4Experiment WorldAnchor;
    public AudioManager audioManager; 
    private bool isInitialInput;

    private Transform CameraTransform; 
    public Transform ShadowManager;

    bool isExperimentStarted; 
    public bool isLooking;

    // TRY ME 
    [SerializeField] private readonly float _timeToWait = 0.1f;
    private float _nextStep; 



    // UI Manager
    //      -- (0) Menu
    //      -- (1) User Input
    //      -- (2) Break

    void Start()
    {

        foreach (Transform child in transform.GetChild(0))
            MainMenuChildren.Add(child);

        InitialSetup();

        _nextStep = Time.time + _timeToWait;
    }

    private void Update()
    {
        // If absolute time is later than the timestamp, we know 3 seconds have passed
        if (Input.anyKeyDown && isExperimentStarted) {
            if (_nextStep <= Time.time)
            {
                Debug.Log("Click and timer " + Time.realtimeSinceStartup);
                Toggle_IfNotOnUI();
            }
            else
                Debug.Log("Not enough time!");
        }
    }

   /* public void OnInputClicked()
    {
        // On each tap gesture, toggle whether the user is in placing mode.
        isLooking = !isLooking;
        Toggle_IfNotOnUI();
    }*/


    private void Toggle_IfNotOnUI()
    {
        // Bit shift the index of the layer (5) to get a bit mask
        int layerMask = 1 << 5;

        RaycastHit hit; 
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(CameraTransform.position, CameraTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            //Debug.Log("Hit UI"); 
        }
        else
        {
            //Debug.Log("Not"); 
            isLooking = !isLooking;
            ToggleCube();
            _nextStep = Time.time + _timeToWait;
        }
    }


    // Called on Start 
    private void InitialSetup()
    {
        MainMenuChildren[1].gameObject.SetActive(false); // hide (1) Set Anchor Button
        MainMenuChildren[2].gameObject.SetActive(false); // hide (2) Instruction Text 

        CameraTransform = Camera.main.transform; 
        ToggleMenu(false);
        isInitialInput = true;
        isLooking = true;
        isExperimentStarted = false;
    }

    // Called on Start button & in Data Manager (between each EXP block) 
    // if isExperimentOngoing (user input = active & main menu = inactive)
    // if !isExperimentOngoing (user input = inactive & main menu = active) 
    public void ToggleMenu(bool isExperimentOngoing)
    {
        transform.GetChild(0).gameObject.SetActive(!isExperimentOngoing);
        transform.GetChild(1).gameObject.SetActive(isExperimentOngoing);
        //ToggleCube();
        transform.GetChild(2).gameObject.SetActive(false);

        transform.GetChild(1).transform.position = CameraTransform.position + CameraTransform.forward * 4.0f;
        transform.GetChild(1).transform.LookAt(CameraTransform);
        isLooking = isExperimentOngoing;
    }

    public void ResetAnchorMenu()
    {
        isExperimentStarted = false;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        ShowAnchorResetUI(); 
    }

    // Called after "off UI click" (after viewing stimuli)
    // see Toggle_IfNotOnUI() and Update() 
    public void ToggleCube()
    {        

        ShadowManager.GetChild(0).gameObject.SetActive(isLooking);
        ShadowManager.GetChild(1).gameObject.SetActive(isLooking);
        ShadowManager.GetChild(2).gameObject.SetActive(isLooking);

        // SetActive Input and position in front of player's camera 
        transform.GetChild(1).gameObject.SetActive(!isLooking); // (1) User Input  
        transform.GetChild(1).transform.position = CameraTransform.position + CameraTransform.forward * 4.0f;
        transform.GetChild(1).transform.LookAt(CameraTransform);        

    }


    public void ShowCube() {
        isExperimentStarted = true;
        isLooking = isExperimentStarted;

        ShadowManager.GetChild(0).gameObject.SetActive(true); // (0) Initial UI Manager
        ShadowManager.GetChild(1).gameObject.SetActive(true); // (1) Set Anchor Button
        ShadowManager.GetChild(2).gameObject.SetActive(true); // (2) Instruction Text
        
        transform.GetChild(0).gameObject.SetActive(false); // (0) Main 
        transform.GetChild(1).gameObject.SetActive(false); // (1) User Input
        transform.GetChild(2).gameObject.SetActive(false); // (2) Break

        transform.GetChild(1).transform.position = CameraTransform.position + CameraTransform.forward * 4.0f;
        transform.GetChild(1).transform.LookAt(CameraTransform);
    }


    public void BreakMenu()
    {
        isExperimentStarted = false;
        isLooking = true;

        audioManager.MuteBackgroundMusic(false); 
        transform.GetChild(0).gameObject.SetActive(false); // (0) Main 
        transform.GetChild(1).gameObject.SetActive(false); // (1) User Input
        transform.GetChild(2).gameObject.SetActive(true);  // (2) Break

        transform.GetChild(2).transform.position = (ShadowManager.position + ShadowManager.up * 0.5f) + ShadowManager.forward * 1.0f;
        transform.GetChild(2).transform.LookAt(CameraTransform);

    }


    // Hide Initial UI Manager GO after Experiment has started
    // Show Instructions & Anchor Reset between EXP blocks
    public void ShowAnchorResetUI() 
    {
        isExperimentStarted = false; 
        MainMenuChildren[0].gameObject.SetActive(false); // hide (0) Initial UI Manager
        MainMenuChildren[1].gameObject.SetActive(true); // show (1) Set Anchor Button
        MainMenuChildren[2].gameObject.SetActive(true); // show (2) Instruction Text
        WorldAnchor.SetAnchor(); // unset anchor 
    }

    public void SetFinalInstruction()
    {
        BreakMenu();
        transform.GetChild(2).GetChild(0).gameObject.SetActive(false); // Break -> (0) Start Trial Button


        /*foreach (Transform UIChild in MainMenuChildren)
            UIChild.gameObject.SetActive(false);
        MainMenuChildren[0].gameObject.SetActive(true); // Subtrail counter
        MainMenuChildren[4].gameObject.SetActive(true); // Subtrail counter
        MainMenuChildren[4].transform.GetComponent<Text>().text = "CONGRATULATIONS! \nYOU'RE DONE!";
        */
    }
}
