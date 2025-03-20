using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldController : MonoBehaviour
{
    public Canvas canvas;
    public List<TargetIndicator> targetIndicators = new List<TargetIndicator>();
    public Camera MainCamera;
    public GameObject generalIndicatorPrefab;
    public GameObject leftIndicatorPrefab;
    public GameObject rightIndicatorPrefab;

    public enum IndicatorType
    {
        Left,
        Right,
        General
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(targetIndicators.Count > 0)
        {
            for (int i=0; i<targetIndicators.Count; i++)
            {
                targetIndicators[i].UpdateTargetIndicator();
            }
        }
    }

    public void AddTargetIndicator(GameObject target, IndicatorType type)
    {
        GameObject indicatorPrefab;

        // Determine which indicator to use based on type
        switch (type)
        {
            case IndicatorType.Left:
                indicatorPrefab = leftIndicatorPrefab;
                Debug.Log("Adding Left Indicator");
                break;
            case IndicatorType.Right:
                indicatorPrefab = rightIndicatorPrefab;
                Debug.Log("Adding Right Indicator");
                break;
            case IndicatorType.General:
                indicatorPrefab = generalIndicatorPrefab;
                Debug.Log("Adding General Purpose Indicator");
                break;
            default:
                Debug.LogError("Invalid indicator type!", this);
                return;
        }

        if (indicatorPrefab == null)
        {
            Debug.LogError("Indicator prefab is missing!", this);
            return;
        }

        // Instantiate the selected indicator
        GameObject indicatorObject = Instantiate(indicatorPrefab, canvas.transform);

        // Get the TargetIndicator component
        TargetIndicator indicator = indicatorObject.GetComponent<TargetIndicator>();

        if (indicator == null)
        {
            Debug.LogError("TargetIndicator component is missing on the instantiated prefab!", this);
            return;
        }

        // Initialize the indicator
        indicator.InitialiseTargetIndicator(target, MainCamera, canvas);

        // Add to the list of active indicators (if needed)
        targetIndicators.Add(indicator);
    }


}
