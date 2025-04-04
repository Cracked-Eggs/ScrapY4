using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombinationPuzzleManager : MonoBehaviour
{
    public List<TogglePressurePlate> Plates = new List<TogglePressurePlate>();
    public List<int> CorrectSequence = new List<int>();

    private List<int> pressedSequence = new List<int>();

    [SerializeField] private UnityEvent onCorrectSequence;
    [SerializeField] private UnityEvent onWrongSequence;

    private void Start()
    {
        foreach (TogglePressurePlate plate in Plates)
        {
            plate.magnetEvent.AddListener(() => OnPlatePressed(Plates.IndexOf(plate)));
        }
    }

    private void OnPlatePressed(int index)
    {
        if (pressedSequence.Count >= CorrectSequence.Count) return;

        pressedSequence.Add(index);
        Debug.Log("Pressed Sequence: " + string.Join(", ", pressedSequence));

        // Reset just the individual plate after a small delay
        StartCoroutine(ResetSinglePlateAfterDelay(index));

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
                ResetPlates(); // Reset all plates
                pressedSequence.Clear();
                onWrongSequence.Invoke();
            }
        }
    }

    private IEnumerator ResetSinglePlateAfterDelay(int index)
    {
        yield return new WaitForSeconds(0.3f); // Small delay to show interaction
        Plates[index].ResetPlate(); // Reset only the pressed plate
    }

    private void ResetPlates()
    {
        StartCoroutine(ResetPlatesAfterDelay());
    }

    private IEnumerator ResetPlatesAfterDelay()
    {
        yield return new WaitForSeconds(0.6f);

        foreach (TogglePressurePlate plate in Plates)
        {
            plate.ResetPlate();
        }

        pressedSequence.Clear();
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
