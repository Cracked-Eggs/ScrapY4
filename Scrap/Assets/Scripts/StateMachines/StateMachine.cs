using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    State currentState;
    public Vector3 Momentum { get; private set; }
    public void SetMomentum(Vector3 momentum) => Momentum = momentum;

    public void SwitchState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    void Update() => currentState?.Tick(Time.deltaTime);
}
