using UnityEngine;

public class StateMachine : MonoBehaviour
{
    // The current state of the state machine which is an interface
    public IState currentState { get; private set; }

    // Change the current state to a new state and execute it if the current state is changed
    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }


    private void Update()
    {
        currentState?.Execute(); // If the current state is not null, repeatedly execute it.
    }
}
