using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildsHelp : MonoBehaviour
{
    private GameObject b = new GameObject();
    private void Awake() {
        b = GameObject.Find("Builds");
        b.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) 
        {            
            b.SetActive(!b.active);
        }
    }
}
