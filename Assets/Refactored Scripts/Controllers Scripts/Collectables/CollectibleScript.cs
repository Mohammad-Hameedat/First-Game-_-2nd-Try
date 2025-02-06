using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    #region Components References
    public CollectibleProperties collectibleProperties;

    private GameObject sttPetInstance;
    #endregion


    #region Destroying Managers
    [Header("Destruction Managers")]
    [SerializeField]
    private float elapsedTime = 0f;

    private float destructionYPosition = 1f;

    private float slowModeMovementThreshold = 0.8f;
    private float slowModeDestructionTimeScaler = 0.5f;
    #endregion

    private void Start()
    {
        GameManager.currentActiveCollectiblesList.Add(gameObject);

        sttPetInstance = GameManager.cAPPetsDictionary.ContainsKey(PetType.STTPet)
            ? GameManager.cAPPetsDictionary[PetType.STTPet]
            : null;
    }

    private void Update()
    {
        ManageDestruction();
    }


    private void ManageDestruction()
    {
        float movementSpeed = collectibleProperties.collectableMovementSpeed * Time.deltaTime;
        if (sttPetInstance != null)
        {
            // Slow down the movement if the STT pet is active
            movementSpeed *= slowModeMovementThreshold;
        }

        if (transform.position.y > destructionYPosition)
        {
            transform.position += Vector3.down * movementSpeed;
        }
        else
        {
            // Slow down the destruction process if the STT pet is active
            elapsedTime += Time.deltaTime * ( sttPetInstance != null ? slowModeDestructionTimeScaler : 1f );

            if (elapsedTime >= collectibleProperties.TimeBeforeDestroy)
            {
                Destroy(gameObject);
            }
        }
    }


    private void OnDestroy()
    {
        // Return if the game stoped in the editor
        if (!this.gameObject.scene.isLoaded)
            return;

        // Unregister the object from the list of active collectable objects || Remove from GameManager list
        GameManager.currentActiveCollectiblesList.Remove(gameObject);
    }
}
