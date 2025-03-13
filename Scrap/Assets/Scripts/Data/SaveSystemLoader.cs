using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemLoader : MonoBehaviour
{
    public SaveSystem systemToLoad;

    void Awake()
    {
        Instantiate(systemToLoad).SetupInstance();
    }
}
