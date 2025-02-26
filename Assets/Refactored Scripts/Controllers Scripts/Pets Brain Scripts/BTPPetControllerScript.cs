using UnityEngine;

#region Blip The Porpoise (BTP) Controller - Required Components
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class BTPPetControllerScript : MonoBehaviour
{
    #region Components References
    private MovementController movementController;
    private StateMachine stateMachine;
    public FollowerSettings petProperties;
    #endregion

    #region BTP Pet - Required Variables
    public PetType petType;
    #endregion


    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        stateMachine = GetComponent<StateMachine>();

        // Assign the properties to the components
        movementController.movementProperties = petProperties.movementProperties;
    }

    private void OnEnable()
    {
        if (!GameManager.cAPPetsDictionary.ContainsKey(PetType.BTPPet))
        {
            GameManager.cAPPetsDictionary.Add(petType, gameObject);
        }
    }

    private void Start()
    {
        stateMachine.ChangeState(new BTPPetIdleState(
            movementController
            ));
    }

    private void OnDisable()
    {
        if (GameManager.cAPPetsDictionary.ContainsKey(petType) && GameManager.cAPPetsDictionary[PetType.BTPPet] == gameObject)
        {
            GameManager.cAPPetsDictionary.Remove(petType);
        }
    }
}
