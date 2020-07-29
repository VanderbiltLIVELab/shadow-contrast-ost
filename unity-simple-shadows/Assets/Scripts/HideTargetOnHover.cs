using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTargetOnHover : MonoBehaviour {

    // If hit object name matches targetName, then reticle is not rendered
    public string targetName;
    private Light lightCursor;
    private Transform handCursor; 
    public Transform playerCamera; 


    // Use this for initialization
    void Start () {
        lightCursor = transform.GetChild(1).GetComponent<Light>(); // (1) CursorOffHolograms
        handCursor = transform.GetChild(3); 
    }

    // raycast out. It target hit == targetName, then turn off renderer
    // else turn renderer on 

    private void Update()
    {

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {            
            HideReticleIfTargetSelected(hit.transform.name);
            //if (hit.transform.name == targetName)
            //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        }
    }


    // Hide reticle if physics raycaster is attatched
    void HideReticleIfTargetSelected(string transformName)
    {
        if (transformName == targetName)
        {

            //Debug.Log("Did Hit " + transformName);
            lightCursor.enabled = false;
            handCursor.gameObject.SetActive(false);
        }
        else
        {
            lightCursor.enabled = true;
            handCursor.gameObject.SetActive(true); 
        }
    }

}
