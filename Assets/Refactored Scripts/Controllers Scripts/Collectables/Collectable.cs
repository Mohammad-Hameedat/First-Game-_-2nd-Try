using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableProperties collectableConfig;


    #region Destroying Managers
    [Header("Destroying Managers")]
    [SerializeField] float elapsedTime = 0f;

    float lastPositionBeforeDestruction = 1f;
    #endregion

    private void Start()
    {
        GameManager.activeCollectablesList.Add(gameObject);
    }

    private void Update()
    {
        if (transform.position.y > lastPositionBeforeDestruction)
        {
            transform.position += Vector3.down * collectableConfig.collectableMovementSpeed * Time.deltaTime;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= collectableConfig.TimeBeforeDestroy)
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
        GameManager.activeCollectablesList.Remove(gameObject);
    }
}
