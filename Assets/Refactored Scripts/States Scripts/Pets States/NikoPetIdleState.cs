using UnityEngine;

public class NikoPetIdleState : IState
{
    private float nextCollectibleSpawnTime;
    private bool firstSpawnComplete = false;

    #region References
    private GameObject nikoObject;
    private CollectibleSpawnProperties collectibleSpawnProperties;
    private GameManager gameManager;
    #endregion

    public NikoPetIdleState(
        GameObject _nikoObject,
        CollectibleSpawnProperties _collectibleSpawnProperties,
        GameManager _gameManager
        )
    {
        nikoObject = _nikoObject;
        collectibleSpawnProperties = _collectibleSpawnProperties;
        gameManager = _gameManager;
    }

    public void Enter()
    {
        nextCollectibleSpawnTime = Time.time + collectibleSpawnProperties.minCollectableSpwanTime;
    }

    public void Execute()
    {
        if (GameManager.currentActiveEnemyObjectsList.Count == 0)
        {
            if (firstSpawnComplete && Time.time >= nextCollectibleSpawnTime)
            {
                SpawnCollectible();

                // Schedule the next spawn
                nextCollectibleSpawnTime = Time.time + collectibleSpawnProperties.maxCollectableSpwanTime;
            }
            // Handle subsequent spawns
            else if (!firstSpawnComplete && Time.time >= nextCollectibleSpawnTime)
            {
                SpawnCollectible();
                firstSpawnComplete = true;

                // Schedule the next spawn using maxCollectableSpawnTime
                nextCollectibleSpawnTime = Time.time + collectibleSpawnProperties.maxCollectableSpwanTime;
            }
        }
        else
        {
            return;
        }
    }

    public void Exit() { }

    void SpawnCollectible()
    {
        GameObject collectibleInstance = Object.Instantiate(
             collectibleSpawnProperties.collectablePrefab,
           nikoObject.transform.position,
             Quaternion.identity
             );

        CollectibleScript collectibleInstanceScript = collectibleInstance.GetComponent<CollectibleScript>();
        collectibleInstanceScript.collectibleProperties = collectibleSpawnProperties.collectibleProperties[0];
        collectibleInstanceScript.collectibleProperties.collectableValue = gameManager.levelData.nikoPetCollectableValue;


        collectibleInstance.transform.SetParent(nikoObject.transform);
    }
}