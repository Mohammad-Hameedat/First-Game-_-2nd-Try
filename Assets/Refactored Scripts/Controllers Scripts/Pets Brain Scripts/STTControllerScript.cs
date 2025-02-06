using UnityEngine;

#region Seymour The Turtle (STT) Pet Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class STTControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    #endregion


    #region STT Pet Controller - Required Variables
    // The type of the pet (Seymour The Turtle) is defined in Unity Editor
    public PetType PetType;

    public FollowerSettings followerProperties;
    #endregion

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();

        movementController.movementProperties = followerProperties.movementProperties;
    }


    private void OnEnable()
    {
        if (!GameManager.cAPPetsDictionary.ContainsKey(PetType))
        {
            GameManager.cAPPetsDictionary.Add(PetType, gameObject);
        }
    }


    private void Start()
    {
        stateMachine.ChangeState(new STTPetIdleState(movementController));
    }


    private void OnDisable()
    {
        if (GameManager.cAPPetsDictionary.ContainsKey(PetType))
        {
            GameManager.cAPPetsDictionary.Remove(PetType);
        }
    }
}