using System.Collections.Generic;
using UnityEngine;

public class ProximityTargetTargetingStrategy : ITargetingStrategy
{
    public Transform GetNearestTarget(IEnumerable<GameObject> targetObjectsList, Transform lastNearestTarget, Vector3 currentPosition)
    {
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject targetObject in targetObjectsList /*as List<GameObject>*/)
        {
            if (targetObject != null && targetObject.activeSelf)
            {
                Vector3 directionToTarget = targetObject.transform.position - currentPosition;
                float distanceSqr = directionToTarget.sqrMagnitude;

                if (distanceSqr < minDistance)
                {
                    minDistance = distanceSqr;
                    nearestTarget = targetObject.transform;
                }
            }
            else
            {
                continue;
            }
        }

        return nearestTarget;
    }


    /*
        public List<Transform> GetNearestTargets_ReturnList(IEnumerable<GameObject> targetObjectsList, Transform lastNearestTarget, Vector3 currentPosition, int numberOfTargets)
        {
            List<Transform> nearestTargets = new List<Transform>();
            List<float> distances = new List<float>();

            foreach (GameObject targetObject in targetObjectsList)
            {
                if (targetObject != null && targetObject.activeSelf)
                {
                    Vector3 directionToTarget = targetObject.transform.position - currentPosition;
                    float distanceSqr = directionToTarget.sqrMagnitude;

                    if (nearestTargets.Count < numberOfTargets)
                    {
                        nearestTargets.Add(targetObject.transform);
                        distances.Add(distanceSqr);
                    }
                    else
                    {
                        for (int i = 0; i < nearestTargets.Count; i++)
                        {
                            if (distanceSqr < distances[i])
                            {
                                nearestTargets[i] = targetObject.transform;
                                distances[i] = distanceSqr;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            return nearestTargets;
        }
    */
}