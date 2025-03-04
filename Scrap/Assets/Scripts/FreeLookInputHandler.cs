using Cinemachine;
using UnityEngine;

public class FreeLookInputHandler : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public float xAxisSensitivity = 150f; 
    public float yAxisSensitivity = 2f;   
    public float accelerationTime = 0.2f; 
    public float decelerationTime = 0.2f; 

    private void Start()
    {
        if (freeLookCamera != null)
        {
            freeLookCamera.m_XAxis.m_AccelTime = accelerationTime;
            freeLookCamera.m_XAxis.m_DecelTime = decelerationTime;
            freeLookCamera.m_YAxis.m_AccelTime = accelerationTime;
            freeLookCamera.m_YAxis.m_DecelTime = decelerationTime;
        }
    }

    private void Update()
    {
        if (freeLookCamera != null)
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = xAxisSensitivity;
            freeLookCamera.m_YAxis.m_MaxSpeed = yAxisSensitivity;
        }
    }
}