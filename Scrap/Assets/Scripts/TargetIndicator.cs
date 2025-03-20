using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{
    public Image TargetIndicatorImage;
    public Image OffScreenTargetIndicator;
    public float OutOfSightOffset = 20f;
    private float outOfSightOffest { get { return OutOfSightOffset /* canvasRect.localScale.x*/; } }

    private GameObject target;
    private Camera mainCamera;
    private RectTransform canvasRect;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    public void InitialiseTargetIndicator(GameObject target, Camera mainCamera, Canvas canvas)
    {
        this.target = target;
        this.mainCamera = mainCamera;
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    public void UpdateTargetIndicator()
    {
        if (target == null || mainCamera == null) return;

        // Check if the target is directly in front of the camera
        Vector3 dirToTarget = (target.transform.position - mainCamera.transform.position).normalized;
        float angle = Vector3.Angle(mainCamera.transform.forward, dirToTarget);

        // If the target is directly in the center of the screen (within 5 degrees), disable the indicator
        bool isTargetInCenter = angle < 3f;

        TargetIndicatorImage.enabled = !isTargetInCenter;
        OffScreenTargetIndicator.enabled = !isTargetInCenter;

        // If the indicator is active, update its position and scale
        if (!isTargetInCenter)
        {
            SetIndicatorPosition();

            float referenceDistance = 10f;
            float distance = Vector3.Distance(mainCamera.transform.position, target.transform.position);
            float scaleFactor = Mathf.Clamp(referenceDistance / distance, 0.2f, 0.5f);

            rectTransform.localScale = Vector3.one * scaleFactor;
            OffScreenTargetIndicator.rectTransform.localScale = Vector3.one * scaleFactor;
        }
    }




    protected void SetIndicatorPosition()
    {

        
        Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.transform.position);
        

        
        if (indicatorPosition.z >= 0f & indicatorPosition.x <= canvasRect.rect.width * canvasRect.localScale.x
         & indicatorPosition.y <= canvasRect.rect.height * canvasRect.localScale.x & indicatorPosition.x >= 0f & indicatorPosition.y >= 0f)
        {

            
            indicatorPosition.z = 0f;

            
            targetOutOfSight(false, indicatorPosition);
        }

        
        else if (indicatorPosition.z >= 0f)
        {
            
            indicatorPosition = OutOfRangeindicatorPositionB(indicatorPosition);
            targetOutOfSight(true, indicatorPosition);
        }
        else
        {
            
            indicatorPosition *= -1f;

            
            indicatorPosition = OutOfRangeindicatorPositionB(indicatorPosition);
            targetOutOfSight(true, indicatorPosition);

        }

        rectTransform.position = indicatorPosition;

    }


    private Vector3 OutOfRangeindicatorPositionB(Vector3 indicatorPosition)
    {
       
        indicatorPosition.z = 0f;

        
        Vector3 canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f, 0f) * canvasRect.localScale.x;
        indicatorPosition -= canvasCenter;

       
        float divX = (canvasRect.rect.width / 2f - outOfSightOffest) / Mathf.Abs(indicatorPosition.x);
        float divY = (canvasRect.rect.height / 2f - outOfSightOffest) / Mathf.Abs(indicatorPosition.y);

        
        if (divX < divY)
        {
            float angle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);
            indicatorPosition.x = Mathf.Sign(indicatorPosition.x) * (canvasRect.rect.width * 0.5f - outOfSightOffest) * canvasRect.localScale.x;
            indicatorPosition.y = Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.x;
        }

        
        else
        {
            float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);

            indicatorPosition.y = Mathf.Sign(indicatorPosition.y) * (canvasRect.rect.height / 2f - outOfSightOffest) * canvasRect.localScale.y;
            indicatorPosition.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.y;
        }

        
        indicatorPosition += canvasCenter;
        return indicatorPosition;
    }



    private void targetOutOfSight(bool oos, Vector3 indicatorPosition)
    {
        
        if (oos)
        {
            
            if (OffScreenTargetIndicator.gameObject.activeSelf == false) OffScreenTargetIndicator.gameObject.SetActive(true);
            if (TargetIndicatorImage.isActiveAndEnabled == true) TargetIndicatorImage.enabled = false;

            
            OffScreenTargetIndicator.rectTransform.rotation = Quaternion.Euler(rotationOutOfSightTargetindicator(indicatorPosition));

           
        }

        
        else
        {
            if (OffScreenTargetIndicator.gameObject.activeSelf == true) OffScreenTargetIndicator.gameObject.SetActive(false);
            if (TargetIndicatorImage.isActiveAndEnabled == false) TargetIndicatorImage.enabled = true;
        }
    }


    private Vector3 rotationOutOfSightTargetindicator(Vector3 indicatorPosition)
    {
        
        Vector3 canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f, 0f) * canvasRect.localScale.x;

      
        float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition - canvasCenter, Vector3.forward);

       
        return new Vector3(0f, 0f, angle);
    }
}