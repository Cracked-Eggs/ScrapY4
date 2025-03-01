using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    public Vector3 gridSize = new Vector3(5, 5, 5);
    public float cellSize = 0.4f;
    public LayerMask obstacleLayer;

    private Vector3[,,] flowField;
    private Vector3 lastTargetPosition;
    private bool needsUpdate = true;

    private void Start()
    {
        obstacleLayer = LayerMask.GetMask("Ground");
    }

    public void GenerateFlowField(Vector3 targetPosition)
    {
        lastTargetPosition = targetPosition;
        flowField = new Vector3[(int)gridSize.x, (int)gridSize.y, (int)gridSize.z];

        Vector3 origin = transform.position - (gridSize * cellSize * 0.5f);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    Vector3 worldPos = origin + new Vector3(x, y, z) * cellSize;
                    if (Physics.CheckSphere(worldPos, cellSize * 0.4f, obstacleLayer))
                    {
                        flowField[x, y, z] = FindNearestValidDirection(worldPos, targetPosition);
                    }
                    else
                    {
                        flowField[x, y, z] = (targetPosition - worldPos).normalized;
                    }

                }
            }
        }
    }
    private Vector3 FindNearestValidDirection(Vector3 startPos, Vector3 targetPosition)
    {
        Vector3 bestDirection = Vector3.zero;
        float bestDistance = float.MaxValue;
        float checkRadius = cellSize * 0.4f;

        // Primary movement directions
        Vector3[] directions = {
        Vector3.forward, Vector3.back, Vector3.right, Vector3.left,
        Vector3.up, Vector3.down,
        (Vector3.forward + Vector3.right).normalized, (Vector3.forward + Vector3.left).normalized,
        (Vector3.back + Vector3.right).normalized, (Vector3.back + Vector3.left).normalized
    };

        // Try all directions to find the closest one
        foreach (Vector3 dir in directions)
        {
            Vector3 checkPos = startPos + dir * cellSize;
            if (!Physics.CheckSphere(checkPos, checkRadius, obstacleLayer))
            {
                float distance = Vector3.Distance(checkPos, targetPosition);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestDirection = dir;
                }
            }
        }

        // 🔥 If still stuck, try a small random push
        if (bestDirection == Vector3.zero)
        {
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            return randomDirection; // Add a small push to break out of being stuck
        }

        return bestDirection.normalized;
    }




    public Vector3 GetFlowDirection(Vector3 position)
    {
        if (flowField == null) return Vector3.zero;

        Vector3 origin = transform.position - (gridSize * cellSize * 0.5f);
        int x = Mathf.Clamp(Mathf.RoundToInt((position.x - origin.x) / cellSize), 0, (int)gridSize.x - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((position.y - origin.y) / cellSize), 0, (int)gridSize.y - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt((position.z - origin.z) / cellSize), 0, (int)gridSize.z - 1);

        return flowField[x, y, z];
    }

    void Update()
    {
        if (needsUpdate)
        {
            GenerateFlowField(lastTargetPosition); // Recalculate path
            needsUpdate = false;
        }
    }

    void OnDrawGizmos()
    {
        if (flowField == null) return;

        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position - (gridSize * cellSize * 0.5f);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    Vector3 worldPos = origin + new Vector3(x, y, z) * cellSize;
                    Vector3 direction = flowField[x, y, z];

                    if (direction != Vector3.zero)
                    {
                        Gizmos.DrawLine(worldPos, worldPos + direction * cellSize * 0.5f);
                        Gizmos.DrawSphere(worldPos + direction * cellSize * 0.1f, 0.05f);
                    }
                }
            }
        }
    }

    public void SetNewTarget(Vector3 newTargetPosition)
    {
        lastTargetPosition = newTargetPosition; // Store the player's position
        needsUpdate = true; // Always mark for update
    }

}
