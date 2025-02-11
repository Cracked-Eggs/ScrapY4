using UnityEngine;
using UnityEngine.AI;

public class ForceReceiver : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float drag = 0.3f;

    Vector3 dampingVelocity;
    Vector3 impact;
    float verticalVelocity;

    public Vector3 Movement => impact + Vector3.up * verticalVelocity;

    void Update()
    {
        if (verticalVelocity < 0f && controller.isGrounded)
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        else
            verticalVelocity += Physics.gravity.y * Time.deltaTime;

        impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, drag);

        if (agent != null)
        {
            if (impact.sqrMagnitude < 0.2f * 0.2f)
            {
                impact = Vector3.zero;
                agent.enabled = true;
            }
        }

        if (impact.sqrMagnitude < 0.2f * 0.2f)
            impact = Vector3.zero;
    }

    public void AddForce(Vector3 force)
    {
        impact += force;
        if (agent != null)
            agent.enabled = false;
    }

    public void Jump(float jumpForce) => verticalVelocity += jumpForce;
}