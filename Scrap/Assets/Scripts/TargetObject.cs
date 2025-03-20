using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    public UIWorldController ui;

    private void Awake()
    {
        ui = FindObjectOfType<UIWorldController>();

        if (ui == null)
        {
            Debug.LogError("No UIWorldController component found in the scene.", this);
        }
        else
        {
            Debug.Log("UIWorldController found successfully.", ui);
        }
    }

    private void Start()
    {
        if (ui != null)
        {
            Debug.Log("Adding target indicator to UIWorldController.");
            ui.AddTargetIndicator(this.gameObject);
        }
        else
        {
            Debug.LogError("UIWorldController reference is still null in Start().", this);
        }
    }

}