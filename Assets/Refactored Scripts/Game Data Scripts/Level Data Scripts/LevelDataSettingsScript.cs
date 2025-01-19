using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Game Data/Level Data", order = 1)]
public class LevelDataSettingsScript : ScriptableObject
{
    [Header("Level Data")]
    #region Level Data

    [Tooltip("The amount of coins that the player has when the level starts.")]
    public int initialAvailableCoins;

    [Tooltip("Assign here the enemy prefabs that can be spawned in scene 'X'.\nX = Scene number.")]
    public List<GameObject> spawnableEnemyPrefabsList = new();
    #endregion


    [Header("Prefabs")]
    #region Game Objects References
    [Tooltip("Assign here the primary fish prefab.")]
    public GameObject primaryFishPrefab;

    [Tooltip("Assign here the default food prefab.")]
    public GameObject foodPrefab;
    #endregion


    [Header("Purchasable Costs")]
    #region Purchasable Costs

    [Tooltip("The cost of buying new primary fish.")]
    public int primaryFishPurchaseCost;
    #endregion


    [Header("Spawning Managers")]
    #region Spawn Objects Managers

    [Tooltip("The minimum delay between each enemy spawn in this scene.")]
    public float minEnemySpawnDelay;
    [Tooltip("The maximum delay between each enemy spawn in this scene.")]
    public float maxEnemySpawnDelay;


    [Tooltip("The delay (in milliseconds) between each food spawn in this scene.")]
    public float foodSpawnDelay;
    #endregion


    [Header("Upgrading Managers")]
    #region Upgradables

    [Tooltip("Assign here the food types (Scriptable Objects) that the player can upgrade the food to.")]
    public FoodProperties[] foodQualityTypes;

    [Tooltip("Assign here the weapon types (Scriptable Objects) that the player can upgrade the weapon to.")]
    public WeaponProperties[] weaponTypes = new WeaponProperties[10];
    #endregion


    [Header("Upgrading Costs")]
    #region Upgrading Costs

    [Tooltip("Assign here the costs of upgrading the egg to the next egg level.")]
    public List<int> eggUpgradeCostList = new();


    public List<int> foodQualityUpgradeCostsList = new();
    #endregion


    [Header("Pets Data")]
    #region Pets Data

    //[Header("Protective Pets Prefabs")]
    //#region Protective Pets Prefabs
    //[Tooltip("Assign here the protective pets prefabs.")]
    //public List<ProtectivePetType> protectivePetsPrefabsList = new();
    //#endregion

    [Header("Niko's Pet Data")]
    #region Niko's Pets
    [Tooltip("Assign here the position of Niko's pet.\nNote: This position may change depending on scene's environment.")]
    public Vector3 nikoPetPosition;

    [Tooltip("Assign here the value of the spawnable collectible for this level.")]
    public int nikoPetCollectableValue;
    #endregion

    #endregion
}