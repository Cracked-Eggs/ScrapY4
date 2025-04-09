using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Vent Path", menuName = "Scrap/Vent Path")]
public class VentPathData : ScriptableObject
{
    public List<Vector3> pathPoints = new List<Vector3>();
}
