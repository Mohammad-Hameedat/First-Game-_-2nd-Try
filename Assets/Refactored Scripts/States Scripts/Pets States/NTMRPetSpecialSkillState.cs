using UnityEngine;

public class NTMRPetSpecialSkillState : IState
{
    private MovementController movementController;

    #region Deceleration Variables

    private float decelerationTimer = 0f;

    // Keep the same deceleration duration
    private readonly float decelerationDuration = 1f;
    private Vector3 initialVelocityAtDeceleration;

    #endregion


    public NTMRPetSpecialSkillState(
        MovementController _movementController
        )
    {
        movementController = _movementController;
    }


    public void Enter()
    {
        // >>>>>> An Animation can be played here <<<<<<

        movementController.SetMovementStrategy(null);

        initialVelocityAtDeceleration = movementController.rb.velocity;
        decelerationTimer = 0f;
    }


    public void Execute()
    {
        if (movementController.rb.velocity.magnitude > 0)
        {
            // Increase deceleration timer and calculate isCollectiblesLimitReached for Lerp (0 to 1 over decelerationDuration)
            decelerationTimer += Time.deltaTime;
            float t = Mathf.Clamp01(decelerationTimer / decelerationDuration);

            movementController.rb.velocity = Vector3.Lerp(initialVelocityAtDeceleration, Vector3.zero, t);

            // If velocity is nearly zero, we set the unique ability flag
            if (movementController.rb.velocity.magnitude <= 0.2f)
            {
                GameManager.NTMRpetUniqueAbility = true;
            }
        }
    }


    public void Exit()
    {
        GameManager.NTMRpetUniqueAbility = false;
    }
}