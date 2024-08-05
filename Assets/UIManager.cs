using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button spawnObject;
    public Button spawnFood;

    private void Start()
    {
        spawnObject.onClick.AddListener(() => GameEvents.eventsChannelInstance.SpawnObject(1));
        spawnFood.onClick.AddListener(() => GameEvents.eventsChannelInstance.SpawnObject(2));
    }
}
