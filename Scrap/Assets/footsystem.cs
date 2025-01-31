using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footsystem : MonoBehaviour
{
    public LayerMask terrainLayer;
    public footsystem otherFoot;
    public float stepDistance, stepHeight, stepLength, footSpacing, speed;
    public Transform body;
    public Vector3 footOffset;
    Vector3 oldPosition, newPosition, currentPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;


    private void Start()
    {
        footSpacing = transform.localPosition.x;
        oldPosition = newPosition = currentPosition = transform.position;
        oldNormal = currentNormal = newNormal = transform.up;
        lerp = 1;
    }

    private void Update()
    {
        transform.position = currentPosition;
        transform.up = currentNormal;

        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);
        if(Physics.Raycast(ray,out RaycastHit hit,10,terrainLayer.value))
        {
            if(Vector3.Distance(newPosition, hit.point) < stepDistance && !otherFoot.isMoving() && lerp >=1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = hit.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = hit.normal;
            }
        }
        if (lerp < 1)
        {
            Vector3 tempPos = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPos.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            currentPosition = tempPos;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;

        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }
    public bool isMoving()
    {
        return lerp < 1;
    }
}
