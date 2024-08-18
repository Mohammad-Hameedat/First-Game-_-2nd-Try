using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button spawnObject;
    public Button upgradeFood;
    public TMPro.TextMeshProUGUI currencyText;

    [SerializeField] float moneyAmount = 0;


    void GetMoneyValue(float coinsValue)
    {
        moneyAmount = coinsValue;
        currencyText.text = moneyAmount.ToString() + "$";

    }


    private void OnEnable()
    {
        GameEvents.eventsChannelInstance.onUpdateCoins += GetMoneyValue;

        spawnObject.onClick.AddListener(() => GameEvents.eventsChannelInstance?.SpawnObjects(1));
        //upgradeFood.onClick.AddListener(() => GameEvents.eventsChannelInstance?.SpawnObjects(2));
    }


    private void OnDisable()
    {
        GameEvents.eventsChannelInstance.onUpdateCoins -= GetMoneyValue;

        spawnObject.onClick.RemoveAllListeners();
        //upgradeFood.onClick.RemoveAllListeners();
    }
}
