using UnityEngine;

public interface IInteractionStrategy
{
    Transform IdentifyNearestObject();
    void Interact(GameObject interactor, GameObject target);
    int GetInteractedTargetsCount();
}