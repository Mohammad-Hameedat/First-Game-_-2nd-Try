using System.Collections.Generic;
using UnityEngine;

public class MainFishDeathStrategy : IDeathStrategy
{
    private GameObject associatedGameObject;
    private MonoBehaviour mainControllerScript;

    private List<GameObject>[] associatedGameObjectLists;
    private bool isLastListHandledDifferently = false;

    private bool isDead = false;

    public bool IsDead => isDead;

    public MainFishDeathStrategy(
        GameObject _gameObject,
        MonoBehaviour _mainControllerScript,
        bool _isLastListHandledDifferently,
        params IEnumerable<GameObject>[] inputLists
        )
    {
        associatedGameObject = _gameObject;
        mainControllerScript = _mainControllerScript;

        isLastListHandledDifferently = _isLastListHandledDifferently;
        associatedGameObjectLists = new List<GameObject>[inputLists.Length];

        for (int i = 0; i < inputLists.Length; i++)
        {
            associatedGameObjectLists[i] = inputLists[i] as List<GameObject>;
        }
    }


    public void TriggerDeathState()
    {
        isDead = true;

        foreach (List<GameObject> currentList in associatedGameObjectLists as List<GameObject>[])
        {
            // Differently handle the last currentList if the flag is set to true
            /* Important Note:
             * The flag logic might be changed in the future to allow for more flexibility.
             */
            if (isLastListHandledDifferently && currentList == associatedGameObjectLists[associatedGameObjectLists.Length - 1])
            {
                if (!currentList.Contains(associatedGameObject))
                    currentList.Add(associatedGameObject);
            }
            else if (currentList.Contains(associatedGameObject))
            {
                currentList.Remove(associatedGameObject);
            }
        }

        ToggleAttachedScripts(false);
    }


    public void TriggerRevivalState()
    {
        isDead = false;

        foreach (List<GameObject> currentList in associatedGameObjectLists as List<GameObject>[])
        {
            // Differently handle the last currentList if the flag is set to true
            /* Important Note:
             * The flag logic might be changed in the future to allow for more flexibility.
            */
            if (isLastListHandledDifferently && currentList == associatedGameObjectLists[associatedGameObjectLists.Length - 1])
            {
                currentList.Remove(associatedGameObject);
            }
            else if (!currentList.Contains(associatedGameObject))
            {
                currentList.Add(associatedGameObject);
            }
        }

        ToggleAttachedScripts(true);
    }


    public void ToggleAttachedScripts(bool toggleScriptState)
    {
        // Get all the MonoBehaviour scripts attached to the associated GameObject.
        MonoBehaviour[] attachedMonoBehaviours = associatedGameObject.GetComponents<MonoBehaviour>();

        /* Note:
         * Loop through each script and enable/disable it based on the toggleScriptState,
         * but skip the main controller script,
         * and the scripts that are not relevant to the death state.
         */
        foreach (MonoBehaviour script in attachedMonoBehaviours)
        {
            if (script == mainControllerScript ||
                script is MovementController ||
                script is StateMachine ||
                script is DeathAndRevivalSystem ||
                script is BoundsAndPositioningManager)
                continue;

            script.enabled = toggleScriptState;
        }

        GameEvents.EventsChannelInstance.RefresheMainFishesNumber(
            GameManager.currentActiveMainFishObjectsList.Count);
    }


    public void ClearFromAllLists()
    {
        foreach (List<GameObject> currentList in associatedGameObjectLists as List<GameObject>[])
        {
            if (currentList.Contains(associatedGameObject))
                currentList.Remove(associatedGameObject);
        }
    }
}
