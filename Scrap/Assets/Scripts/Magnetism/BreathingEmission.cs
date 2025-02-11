using UnityEngine;

public class BreathingEmission : MonoBehaviour
{
    public Material material; // The material to control
    public Color emissionColor = Color.red; // The color of the emission
    public float minIntensity = 0.5f; // Minimum emission intensity
    public float maxIntensity = 2.0f; // Maximum emission intensity
    public float cycleDuration = 2.0f; // Duration for a full "bop" cycle (up and down)
    public float length = 1f;

    private float timeElapsed = 0f; // Timer to track elapsed time
    private float cycleProgress = 0f; // Progress through the cycle (0 to 1)

    private void Update()
    {
        // Increment the timer by time passed each frame
        timeElapsed += Time.deltaTime;

        // Calculate cycle progress as a value between 0 and 1 based on the timer
        cycleProgress = Mathf.PingPong(timeElapsed / cycleDuration, length);

        // Use a sine wave to make the emission intensity "breathe"
        float emissionIntensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.Sin(cycleProgress * Mathf.PI));

        // Set the emission color and intensity
        material.SetColor("_EmissionColor", emissionColor * emissionIntensity);

        // Update the environment lighting (if needed)
        DynamicGI.UpdateEnvironment();
    }

    private void OnEnable()
    {
        // Enable emission when the script is enabled
        material.EnableKeyword("_EMISSION");
    }

    private void OnDisable()
    {
        // Disable emission when the script is disabled
        material.DisableKeyword("_EMISSION");
    }
}