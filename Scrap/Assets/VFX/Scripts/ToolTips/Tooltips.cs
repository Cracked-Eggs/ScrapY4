using UnityEngine;

[System.Serializable]
public class Tooltips : MonoBehaviour
{
    public string id; // Unique ID for the tooltip
    public float duration; // How long the tooltip stays visible
    public bool hasBeenShown; // Track if the tooltip has already been displayed
    public Animator Animator;
}