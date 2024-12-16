using System.Collections.Generic;
using UnityEngine;

public interface ITargetingStrategy
{
    Transform GetNearestTarget(IEnumerable<GameObject> targetObjectsList, Transform lastNearestObject, Vector3 currentPosition);
}
