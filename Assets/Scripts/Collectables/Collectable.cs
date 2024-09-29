using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableProperties collectableConfig;


    #region Destroying Managers
    [Header("Destroying Managers")]
    [SerializeField] float elapsedTime = 0f;

    float lastPositionBeforeDestruction = 1f;
    #endregion

    private void Update()
    {
        if (transform.transform.position.y > lastPositionBeforeDestruction)
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
}
