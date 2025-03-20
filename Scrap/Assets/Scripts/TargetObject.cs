using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIWorldController;

public class TargetObject : MonoBehaviour
{
    public UIWorldController ui;
    public Attach attach;
    // Default to false, change in Inspector if needed

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
            if (gameObject.CompareTag("L_Arm"))
            {
                ui.AddTargetIndicator(this.gameObject, IndicatorType.Left);
            }
            if (gameObject.CompareTag("R_Arm"))
            {
                ui.AddTargetIndicator(this.gameObject, IndicatorType.Right);
            }
            if(gameObject.CompareTag("General"))
            {
                ui.AddTargetIndicator(this.gameObject, IndicatorType.General);
            }

        }
        else
        {
            Debug.LogError("UIWorldController reference is still null in Start().", this);
        }
    }
}
