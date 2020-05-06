using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidEntry : MonoBehaviour
{   
    void Start()
    {
        CFps.Start();
    }
     
    void Update()
    {
        CFps.Update();
    }

    public void OnGUI()
    {
        CFps.OnGUI();
    }
}
