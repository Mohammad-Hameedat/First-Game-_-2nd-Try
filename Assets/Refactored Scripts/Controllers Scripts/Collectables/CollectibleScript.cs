using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    public CollectibleProperties collectibleProperties;


    #region Destroying Managers
    [Header("Destroying Managers")]
    [SerializeField] float elapsedTime = 0f;

    float lastPositionBeforeDestruction = 1f;
    #endregion

    private void Start()
    {
        GameManager.currentActiveCollectiblesList.Add(gameObject);
    }

    private void Update()
    {
        if (transform.position.y > lastPositionBeforeDestruction)
        {
            transform.position += Vector3.down * collectibleProperties.collectableMovementSpeed * Time.deltaTime;
        }
        else
        {
            elapsedTime += Time.deltaTime;
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
