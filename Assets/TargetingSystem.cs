using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    private ITargetingStrategy targetingStrategy;

    private IEnumerable<GameObject> targetObjectsList;
    private static List<GameObject> searchableTargetObjectsList = new List<GameObject>();
    private Transform lastNearestTarget;



    private void Awake()
    {
        // Determine which targeting strategy to use
        if (GetComponent<EnemyController>() != null)
        {
            targetingStrategy = new EnemyTargetingStrategy();
        }
        else
        {
            targetingStrategy = new MainFishTargetingStrategy();
        }
    }


    // Set the type of target objects to detect and interact with
    public void SetEatableTargetsList(IEnumerable<GameObject> targets)
    {
        targetObjectsList = targets;
    }

    public Transform GetNearestTarget()
    {
        if (targetObjectsList == null)
        {
            return null;
        }

        Vector3 currentPosition = transform.position;
        lastNearestTarget = targetingStrategy.GetNearestTarget(targetObjectsList, lastNearestTarget, currentPosition);

        return lastNearestTarget;
    }

    // Get the last nearest target object
    public Transform GetlastNearestTarget()
    {
        return lastNearestTarget;
    }
}







/* The following code is the original code after the refactoring
*/

//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class TargetingSystem : MonoBehaviour
//{
//    private IEnumerable<GameObject> targetObjectsList;
//    private static List<GameObject> searchableTargetObjectsList = new List<GameObject>();
//    private Transform lastNearestTarget;

//    private int frameInterval = 5; // Interval to distribute nearest object checks
//    private int currentObjectIndex = 0; // Tracks the current object being processed across frames

//    private FollowerProperties properties;
//    //private float sqrLargerDetectionRadius;

//    EnemyController detectIfEnemy;

//    private void Awake()
//    {
//        properties = GetComponent<MovementController>().properties;
//    }

//    private void Start()
//    {
//        //sqrLargerDetectionRadius = properties.closeTargetsRangeThreshold * properties.closeTargetsRangeThreshold;

//        detectIfEnemy = gameObject.GetComponent<EnemyController>();
//    }

//    // Set the type of target objects to detect and interact with
//    public void SetEatableTargetsList(IEnumerable<GameObject> targets)
//    {
//        targetObjectsList = targets;
//    }

//    // Get the nearest object from the list of target objects
//    public Transform GetNearestTarget()
//    {
//        //Debug.Log(targetObjectsList.Count);


//        //Only run the check every X frames for performance reasons | X = frameInterval(5)
//        if (Time.frameCount % frameInterval != 0 && lastNearestTarget != null /*||
//            (lastNearestTarget != null && (lastNearestTarget.position - transform.position).sqrMagnitude <= sqrLargerDetectionRadius)*/)
//        {
//            return lastNearestTarget;
//        }
//        else
//        {
//            if (detectIfEnemy != null)
//            {
//                Debug.Log("Enemy Type: " + detectIfEnemy);
//            }
//            else
//            {
//                Debug.Log("Enemy Type: null");
//            }


//            searchableTargetObjectsList = targetObjectsList.ToList();

//            Transform nearestTarget = null;
//            float minDistance = Mathf.Infinity;

//            Vector3 currentPosition = transform.position;

//            for (int i = 0; i < searchableTargetObjectsList.Count; i++)
//            {
//                if (i % frameInterval == currentObjectIndex)
//                {
//                    GameObject targetObject = searchableTargetObjectsList[i];
//                    if (targetObject == null)
//                    {
//                        searchableTargetObjectsList.RemoveAt(i);
//                        Debug.Log("Removed null object from the list");
//                        continue;
//                    }

//                    Vector3 directionToTarget = targetObject.transform.position - currentPosition;
//                    float distanceSqr = directionToTarget.sqrMagnitude;

//                    if (distanceSqr < minDistance)
//                    {
//                        minDistance = distanceSqr;
//                        nearestTarget = targetObject.transform;
//                    }
//                }
//            }

//            // Cycle through the target objects list across frames
//            currentObjectIndex = (currentObjectIndex + 1) % frameInterval;

//            lastNearestTarget = nearestTarget;
//            return nearestTarget;
//        }
//    }

//    // Get the last nearest target object
//    public Transform GetlastNearestTarget()
//    {
//        return lastNearestTarget;
//    }
//}
