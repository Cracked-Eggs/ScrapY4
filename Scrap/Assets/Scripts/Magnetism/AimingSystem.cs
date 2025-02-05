using UnityEngine;
using Cinemachine;

public class AimingSystem : MonoBehaviour
{
    private CinemachineBrain cinemachineBrain;

    private void Start()
    {
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>(); // Get Cinemachine Brain
    }

    public Vector3 GetShootDirection(Vector3 shootOrigin)
    {
        if (cinemachineBrain == null || cinemachineBrain.ActiveVirtualCamera == null)
        {
            Debug.LogWarning("Cinemachine Brain or Virtual Camera not found!");
            return transform.forward;
        }

        // Get the forward direction from the active virtual camera
        Transform virtualCamTransform = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.transform;
        Vector3 camForward = virtualCamTransform.forward;

        // Cast a ray from the center of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        // Default direction is forward from the camera
        Vector3 shootDirection = camForward;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            shootDirection = (hit.point - shootOrigin).normalized; // Aim at the target hit

            // Visualize the ray hitting something (Red)
            Debug.DrawRay(shootOrigin, shootDirection * hit.distance, Color.red, 0.1f);
        }
        else
        {
            // Visualize the ray going forward infinitely (Green)
            Debug.DrawRay(shootOrigin, shootDirection * 100f, Color.green, 0.1f);
        }

        return shootDirection;
    }
}
