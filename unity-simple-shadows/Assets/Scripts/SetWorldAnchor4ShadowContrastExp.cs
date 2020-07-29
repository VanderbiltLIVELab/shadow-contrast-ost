using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.XR.WSA;



public class SetWorldAnchor4ShadowContrastExp : MonoBehaviour
{
    //
    bool isShadowAnchored;
    public static float surfaceHeight;
    //
    ShadowAnchorManager shadowAnchorManager;
    public GameObject mySceneObject; //Shadow Cube
    public GameObject mySceneUI;
    public static bool ischangedText;
    public static GameObject SceneObject;
    public static GameObject SceneUI;

    public static bool active_toggle;
    

    void Start()
    {
        SceneObject = mySceneObject;
        shadowAnchorManager = mySceneObject.GetComponent<ShadowAnchorManager>();

        SceneUI = mySceneUI;
        active_toggle = false;
        SetAnchor();
        isShadowAnchored = false;
        surfaceHeight = 0f; 
    }

    // on click - set world anchor and dependencies  
    // public virtual void OnInputClicked(InputClickedEventData eventData) 
    public void SetAnchor()
    {        
        active_toggle = !active_toggle;
        Debug.Log("is Setting Anchor Bool: " + active_toggle); 
        transform.GetComponent<HoloToolkit.Unity.SpatialMapping.TapToPlace4Experiment>().enabled = active_toggle;
        transform.GetComponentInChildren<MeshRenderer>().enabled = active_toggle;

        UpdateUITransform();
        UpdateObjectsTransform();
    }

    // set position and orientation of UI relative to anchor 
    // SceneUI is the parent to the actual Canvas element
    // Called in TapToPlace.cs by 'My Anchor Manager' object
    public void UpdateUITransform()
    {
        SceneUI.transform.position = new Vector3(transform.position.x, transform.position.y + surfaceHeight, transform.position.z);
        SceneUI.transform.rotation = transform.rotation;
    }

    // Show scene objects (shadow cubes) when active toggle is true 
    // set position and orientation of NEW spatial anchor relative to first world anchor (set at origin) 
    // the NEW spatial anchor is a distance out from origin based on UI settings 
    // and the shadow cubes are displayed at this new spatial anchor. 
    // Using a 2nd spatial anchor stabilizes the shadow cubes GOs. 
    public void UpdateObjectsTransform()
    {
        SceneObject.GetComponent<ShadowAnchorManager>().UpdatePrimaryAnchorTransform(transform);
        SceneObject.transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
        SceneObject.transform.rotation = transform.rotation; //transform.position.y + shadowManager.GetSurfaceHeight()

        SceneObject.GetComponent<ShadowAnchorManager>().UpdateDistance();
        SceneObject.GetComponent<ShadowAnchorManager>().UpdateIntermediateAnchors(!active_toggle);
    }
   

    public void SetSurfaceHeight(float value)
    {
        // variable moves Shadow Manger
        surfaceHeight = (value * 0.2f) - 0.1f;
        // but also need to move anchor manager > (1) anchor cube 
        transform.GetChild(1).localPosition = new Vector3(0, surfaceHeight, 0);

        //UpdateObjectsTransform();
    }

    public float GetSurfaceHeight()
    {
        return surfaceHeight;
    }

  
}
