using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UIManagement4ShadowContrastExp : MonoBehaviour {


    public List<Transform> MainMenuChildren;
    public SetWorldAnchor4ShadowContrastExp WorldAnchor;
    public AudioManager audioManager;
    public ShadowContrastDataManager dataManager; 
    private bool isInitialInput;

    private Transform CameraTransform; 
    public Transform ShadowManager;

    bool isExperimentStarted; 
    public bool isLooking;

    // TRY ME 
    [SerializeField] private readonly float _timeToWait = 0.2f; //0.1f;

    private float _nextStep;
    bool _canclick;
    private int cur_subtrial;



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
        _canclick = false;
        isExperimentStarted = false;
        cur_subtrial = 0; 
    }

    private void Update()
    {

        if (isExperimentStarted)
        {

            if (_nextStep > Time.time)
            {
                //Debug.Log("Not enough time! ( " + _nextStep + " > " + Time.time + ")");
                _canclick = false;
            }
            else if (_nextStep <= Time.time && !_canclick)
            {
                _canclick = true;
                HideCube();
            }

            // If absolute time is later than the timestamp, we know .1 seconds have passed
            else if (_nextStep <= Time.time && Input.anyKeyDown && _canclick)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Debug.Log("left arrow");
                    _canclick = false;
                    cur_subtrial += 1;
                    dataManager.UpdateRecordedValues(0);
                    ShowCube();
                }
                else if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Debug.Log("right arrow");
                    _canclick = false;
                    cur_subtrial += 1; 
                    dataManager.UpdateRecordedValues(1);
                    ShowCube();
                }
            }

        } // end if (isExperimentStarted) 
    } // end Update() 



    private void Toggle_OnTimer()
    {
        ToggleCube();
        
    }


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

        ShadowManager.GetChild(0).gameObject.SetActive(isLooking); // cubeA
        ShadowManager.GetChild(1).gameObject.SetActive(isLooking); // cubeB
        ShadowManager.GetChild(2).gameObject.SetActive(isLooking); // shadow

        // SetActive Input and position in front of player's camera 
        transform.GetChild(1).gameObject.SetActive(!isLooking); // (1) User Input  
        transform.GetChild(1).transform.position = CameraTransform.position + CameraTransform.forward * 4.0f;
        transform.GetChild(1).transform.LookAt(CameraTransform);        

    }

    
    public void StartExperiment()
    {
        Debug.Log("StartExperiment()");
        audioManager.MuteBackgroundMusic(true); 
        isExperimentStarted = true;
        cur_subtrial = 0; 
        ShowCube(); 
    }


    public void ShowCube() {
        // test if last trial 
        if (cur_subtrial >= 64) //> 64)
            return; 

        Debug.Log("ShowCube(): " + cur_subtrial);
        isLooking = isExperimentStarted;
        

        ShadowManager.GetChild(0).gameObject.SetActive(true); // cubeA
        ShadowManager.GetChild(1).gameObject.SetActive(true); // cubeB
        ShadowManager.GetChild(2).gameObject.SetActive(true); // shadow 

        transform.GetChild(0).gameObject.SetActive(false); // (0) Main 
        transform.GetChild(1).gameObject.SetActive(false); // (1) User Input
        transform.GetChild(2).gameObject.SetActive(false); // (2) Break

        transform.GetChild(1).transform.position = CameraTransform.position + CameraTransform.forward * 4.0f;
        transform.GetChild(1).transform.LookAt(CameraTransform);

        _nextStep = Time.time + _timeToWait;

    }

    public void HideCube()
    {
        //isExperimentStarted = true;
        //isLooking = isExperimentStarted;

        ShadowManager.GetChild(0).gameObject.SetActive(false); // cubeA
        ShadowManager.GetChild(1).gameObject.SetActive(false); // cubeB
        ShadowManager.GetChild(2).gameObject.SetActive(false); // shadow 

        transform.GetChild(0).gameObject.SetActive(false); // (0) Main 
        transform.GetChild(1).gameObject.SetActive(true); // (1) User Input
        transform.GetChild(2).gameObject.SetActive(false); // (2) Break

        transform.GetChild(1).transform.position = CameraTransform.position + CameraTransform.forward * 4.0f;
        transform.GetChild(1).transform.LookAt(CameraTransform);
    }

    public void BreakMenu()
    {
        Debug.Log("UI Manager BreakMenu called");
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
        MainMenuChildren[2].gameObject.SetActive( true); // show (2) Instruction Text
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
