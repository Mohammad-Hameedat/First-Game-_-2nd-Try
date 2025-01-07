using System.Collections;
using UnityEngine;

#region Niko Pet Controller - Required Components
[RequireComponent(typeof(StateMachine))]
#endregion
public class NikoPetControllerScript : MonoBehaviour
{
    [Tooltip("Assign here the collectible spawn properties (Scriptable Object) to the follower")]
    public CollectibleSpawnProperties collectibleSpawnProperties;

    private GameManager gameManager;
    private StateMachine stateMachine;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        stateMachine = GetComponent<StateMachine>();
    }

    private void Start()
    {
        transform.position = gameManager.levelData.nikoPetPosition;

        stateMachine.ChangeState(new NikoPetIdleState(
            gameObject,
            collectibleSpawnProperties,
            gameManager
            ));
    }
}
