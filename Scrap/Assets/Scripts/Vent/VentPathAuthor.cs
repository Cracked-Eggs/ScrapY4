using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VentPathAuthor : MonoBehaviour
{
    public VentPathData pathData;
    public List<Transform> pointTransforms = new List<Transform>();

    public bool saveToAsset = false;

    void OnDrawGizmos()
    {
        if (pointTransforms == null || pointTransforms.Count < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < pointTransforms.Count - 1; i++)
        {
            Gizmos.DrawLine(pointTransforms[i].position, pointTransforms[i + 1].position);
            Gizmos.DrawSphere(pointTransforms[i].position, 0.1f);
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (saveToAsset && pathData != null)
        {
            saveToAsset = false;
            pathData.pathPoints.Clear();
            foreach (var point in pointTransforms)
            {
                pathData.pathPoints.Add(point.position);
            }

            EditorUtility.SetDirty(pathData);
            Debug.Log("Vent path saved to ScriptableObject.");
        }
    }
#endif
}
