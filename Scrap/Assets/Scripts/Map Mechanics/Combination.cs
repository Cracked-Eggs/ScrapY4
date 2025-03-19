using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombinationPuzzleManager : MonoBehaviour
{
    public List<PressurePlate> Plates = new List<PressurePlate>(); 
    public List<int> CorrectSequence = new List<int>();

    private List<int> pressedSequence = new List<int>(); 

    [SerializeField] private UnityEvent onCorrectSequence; 
    [SerializeField] private UnityEvent onWrongSequence; 

    private void Start()
    {
        foreach (PressurePlate plate in Plates)
        {
            plate.magnetEvent.AddListener(() => OnPlatePressed(Plates.IndexOf(plate)));
        }
    }

    private void OnPlatePressed(int index)
    {
        if (pressedSequence.Count >= CorrectSequence.Count) return;

        pressedSequence.Add(index);
        Debug.Log("Pressed Sequence: " + string.Join(", ", pressedSequence));

        if (pressedSequence.Count == CorrectSequence.Count)
        {
            if (IsSequenceCorrect())
            {
                Debug.Log("Puzzle Solved!");
                onCorrectSequence.Invoke();
            }
            else
            {
                Debug.Log("Wrong Order! Resetting...");
               
                pressedSequence.Clear();
                onWrongSequence.Invoke();
            }
        }
    }
   
    private bool IsSequenceCorrect()
    {
        for (int i = 0; i < CorrectSequence.Count; i++)
        {
            if (pressedSequence[i] != CorrectSequence[i]) return false;
        }
        return true;
    }
}
