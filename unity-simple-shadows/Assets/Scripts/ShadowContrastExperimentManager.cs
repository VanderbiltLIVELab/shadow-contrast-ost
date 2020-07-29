using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowContrastExperimentManager : MonoBehaviour
{

    bool isOnGround;
    int shadowIndex;

    // Object & Shadow Transforms 
    private Transform cubeParentA;
    private Transform cubeParentB;
    private Transform cubeTransformA;
    private Transform cubeTransformB;
    Transform shadowTransform;


    // Object & Shadow Materials
    private Material cubeMaterialA;
    private Material cubeMaterialB;
    private Material shadowMaterial;


    // 4 colors used in experiment
    public Color Gray250;   // light object
    public Color Gray220;   // light shadow
    public Color Gray50;   // dark shadow 
    public Color Gray20;   // dark object


    void Start()
    {
        isOnGround = true;  // default: on ground
        shadowIndex = 1;    // default: ??? 

        // Access 'Cube Parent' Transform to manipulate position
        cubeParentA = transform.GetChild(0);
        cubeParentB = transform.GetChild(1); 
        cubeTransformA = cubeParentA.transform.GetChild(0);
        cubeTransformB = cubeParentB.transform.GetChild(0);
        shadowTransform = transform.GetChild(2);

        // Access 'Cube' -- a child of 'Cube Parent' -- to manipulate material
        cubeMaterialA = cubeTransformA.transform.GetComponent<Renderer>().material;
        cubeMaterialB = cubeTransformB.transform.GetComponent<Renderer>().material;

        shadowMaterial = shadowTransform.transform.GetComponent<Renderer>().material;
    }


    // Do we need the on ground bool? Probably not. 
    public void SetHeights(int _Height_Index, int cube_index)
    {
        Transform temp;
        if (cube_index == 0) 
            temp = cubeParentA;
        else
            temp = cubeParentB;

        //float jitter_z = Random.Range(-0.1f, 0.1f); 

        switch (_Height_Index)
        {
            case 0:                                             // height 0  = 0 degree change                                        
                temp.localPosition = new Vector3(temp.localPosition.x, 0.0f, 0f);
                break;
            case 1:                                             // height 1 = 0.125 degree change
                temp.localPosition = new Vector3(temp.localPosition.x, 0.0018751117468031229f, 0f);
                break;
            case 2:                                            // height 2 = 0.25 degree change
                temp.localPosition = new Vector3(temp.localPosition.x, 0.003743314094901334f, 0f);
                break;
            case 3:                                            // height 3 = 0.5 degree change
                temp.localPosition = new Vector3(temp.localPosition.x, 0.007459213278032227f, 0f);
                break;
            default:                                            // height 0b
                temp.localPosition = new Vector3(temp.localPosition.x, 0.3f, 0f);
                break;
        }
        
    }

    public void SetMaterials(int _Materials_Index)
    {
        switch (_Materials_Index)
        {
            case 0:                                                      
                SetObjectMaterial(0);                           // dark object [20]  
                SetShadowMaterial(0);                           // dark shadow [50]
                break;
            case 1:                                            
                SetObjectMaterial(0);                           // dark object [20]
                SetShadowMaterial(1);                           // light shadow [220]
                break;
            case 2:                                             
                SetObjectMaterial(1);                           // light object [250]
                SetShadowMaterial(0);                           // dark shadow [50]
                break;
            case 3:                                             
                SetObjectMaterial(1);                           // light object [250]
                SetShadowMaterial(1);                           // light shadow [220]
                break;
            default:                                           
                Debug.Log("ERR: MATERIAL NOT SET PROPERLY");
                SetObjectMaterial(99);
                SetShadowMaterial(99);
                break;
        }
    }




    public void SetObjectMaterial(int _ShadowMat_Index)
    {
        //MeshRenderer obj_r = cubeTransformA.GetComponent<MeshRenderer>();
        MeshRenderer obj_r1 = cubeTransformA.GetComponent<MeshRenderer>();
        MeshRenderer obj_r2 = cubeTransformB.GetComponent<MeshRenderer>();

        switch (_ShadowMat_Index)
        {
            case 0:                                             // dark object [20]
                obj_r1.enabled = true;
                cubeMaterialA.SetColor("_Color", Gray20);
                cubeMaterialB.SetColor("_Color", Gray20);
                break;
            case 1:                                             // light object [250]
                obj_r1.enabled = true;
                cubeMaterialA.SetColor("_Color", Gray250);
                cubeMaterialB.SetColor("_Color", Gray250);
                break;
            default:                                            // No render. HAH 
                obj_r1.enabled = false;
                break;
        }
    }


        public void SetShadowMaterial(int _ShadowMat_Index)
    {
        MeshRenderer sh_r = shadowTransform.GetComponent<MeshRenderer>();

        switch (_ShadowMat_Index)
        {
            case 0:                                             // dark shadow [50]
                sh_r.enabled = true;
                sh_r.receiveShadows = true;
                shadowMaterial.SetColor("_Color", Gray50);
                break;
            case 1:                                             // light shadow [220]
                sh_r.enabled = true;
                sh_r.receiveShadows = true;
                shadowMaterial.SetColor("_Color", Gray220);
                break;
            default:                                            // no shadow 
                sh_r.enabled = false;
                sh_r.receiveShadows = true;
                break;
        }
    }


}