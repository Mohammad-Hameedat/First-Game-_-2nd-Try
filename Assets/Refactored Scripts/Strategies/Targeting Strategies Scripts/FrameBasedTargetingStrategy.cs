using System.Collections.Generic;
using UnityEngine;


/*
public class FrameBasedTargetingStrategy : ITargetingStrategy
{
    private int frameInterval = 3; // Interval to distribute nearest object checks
    private int currentObjectIndex = 0; // Tracks the current object index being processed across frames

    public Transform GetNearestTarget(IEnumerable<GameObject> targetObjectsList, Transform lastNearestObject, Vector3 currentPosition)
    {
        if (Time.frameCount % frameInterval != 0 && lastNearestObject != null)
        {
            return lastNearestObject;
        }
        else
        {
            Transform nearestTarget = null;
            float minDistance = Mathf.Infinity;


            int index = 0;

            foreach (GameObject targetObject in targetObjectsList)
            {
                if (index % frameInterval == currentObjectIndex)
                {
                    Vector3 directionToTarget = targetObject.transform.position - currentPosition;
                    float distanceSqr = directionToTarget.sqrMagnitude;

                    if (distanceSqr < minDistance)
                    {
                        minDistance = distanceSqr;
                        nearestTarget = targetObject.transform;
                    }
                }
                index++;
            }

            currentObjectIndex = ( currentObjectIndex + 1 ) % frameInterval;
            return nearestTarget;
        }
    }
}
*/


/*
 * Optimized version of FrameBasedTargetingStrategy with a forced "full" check
 * when there's no current target or the update interval has expired.
 * This ensures newly spawned objects quickly find a target.
*/
public class FrameBasedTargetingStrategy : ITargetingStrategy
{
    [Tooltip("Seconds between full target sweeps")]
    [SerializeField] private float updateInterval = 0.15f;

    [Tooltip("Maximum checks allowed per second")]
    [SerializeField] private int maxChecksPerSecond = 300;

    // Time after which we do a forced full update
    private float nextFullUpdateTime;

    // Tracks how far into the target collection we got with partial checks
    private int lastProcessedIndex;

    // Dynamically adjusted number of checks per frame
    private float checksPerFrame;

    // Factor controlling how quickly checksPerFrame responds to FPS changes
    private float fpsStabilityFactor = 10f;

    // Static fallback list in case the target collection is neither List<GameObject> nor GameObject[]
    private static readonly List<GameObject> _fallbackList = new(512);

    public FrameBasedTargetingStrategy()
    {
        // Stagger the first update across multiple objects
        nextFullUpdateTime = Time.time + Random.Range(0f, updateInterval);

        // Start with a small partial check count; adapt quickly
        checksPerFrame = 1f;
    }

    /*
     * Returns the nearest target from the given collection of GameObjects,    
     * using time-sliced iteration unless an urgent (full) update is needed.

     * - targets:
     * - - A collection of potential targets (e.g., from GameManager.currentActiveFoodTargetObjectsList)

     * - currentBest:
     * - - The currently known best target, if any

     * - searcherPosition:
     * - - The position of the agent searching for targets

     * - Returns:
     * - - The nearest target transform
    */
    public Transform GetNearestTarget(IEnumerable<GameObject> targets,
                                      Transform currentBest,
                                      Vector3 searcherPosition)
    {
        // 1) Quick null check
        if (targets == null)
        {
            ResetState();
            return null;
        }

        // 2) Adapt the partial-check rate based on FPS
        UpdateRateParameters();

        // 3) Determine if we need a forced (full) update
        bool isUrgentUpdate = RequiresUrgentUpdate(currentBest, Time.time);
        if (isUrgentUpdate)
        {
            // If urgent, reset so we can do a full iteration
            nextFullUpdateTime = Time.time;
        }

        // 4) Convert/cast targets to a usable array or list
        int targetCount;
        List<GameObject> listRef = targets as List<GameObject>;
        GameObject[] arrayRef = null;

        if (listRef != null)
        {
            targetCount = listRef.Count;
        }
        else
        {
            arrayRef = targets as GameObject[];
            if (arrayRef != null)
            {
                targetCount = arrayRef.Length;
            }
            else
            {
                // Fallback approach for any other IEnumerable<T>
                _fallbackList.Clear();
                foreach (var obj in targets)
                {
                    _fallbackList.Add(obj);
                }
                listRef = _fallbackList;
                targetCount = listRef.Count;
            }
        }

        if (targetCount == 0)
        {
            ResetState();
            return null;
        }

        // 5) Perform either full or partial iteration
        Transform nearest = ProcessIteration(
            listRef, arrayRef, targetCount,
            currentBest, searcherPosition,
            isUrgentUpdate
        );

        return nearest;
    }

    /*
     * Processes targets either partially or fully, depending on isUrgentUpdate.
     * If isUrgentUpdate is true, we check all targets this frame.
     * Otherwise, we check up to checksPerFrame targets in a circular fashion.
    */
    private Transform ProcessIteration(
        List<GameObject> listRef,
        GameObject[] arrayRef,
        int targetCount,
        Transform currentBest,
        Vector3 searcherPosition,
        bool isUrgentUpdate
    )
    {
        Transform nearest = currentBest;
        float bestSqrDist = ( nearest != null )
            ? ( nearest.position - searcherPosition ).sqrMagnitude
            : float.MaxValue;

        // Decide how many targets to check
        int checksAllowed = isUrgentUpdate
            ? targetCount
            : Mathf.FloorToInt(checksPerFrame);

        int processedCount = 0;
        int startIndex = lastProcessedIndex;

        // Do the partial or full iteration
        for (int i = 0; i < targetCount && processedCount < checksAllowed; i++)
        {
            int index = ( startIndex + i ) % targetCount;
            GameObject candidate = ( listRef != null )
                ? listRef[index]
                : arrayRef[index];

            // Null check for destroyed objects
            if (!candidate)
                continue;

            // Simple broad-phase distance check
            float sqrDist = ( candidate.transform.position - searcherPosition ).sqrMagnitude;
            if (sqrDist < bestSqrDist)
            {
                bestSqrDist = sqrDist;
                nearest = candidate.transform;
            }

            processedCount++;
        }

        // Update lastProcessedIndex for next frame (partial iteration)
        lastProcessedIndex = ( startIndex + processedCount ) % targetCount;

        // If we've processed everything or forced a full iteration,
        // reset for the next pass
        if (isUrgentUpdate || processedCount == targetCount)
        {
            lastProcessedIndex = 0;
            nextFullUpdateTime = Time.time + updateInterval;
        }

        return nearest;
    }

    #region Adaptive Checks Tuning

    /*
     * Smoothly adjusts checksPerFrame to limit total checks to maxChecksPerSecond
     * based on current framerate.
    */
    private void UpdateRateParameters()
    {
        float currentFPS = ( Application.targetFrameRate > 0 )
            ? Application.targetFrameRate
            : ( 1f / Time.deltaTime );

        // Compare the difference between actual FPS and
        // the "theoretical" FPS used by checksPerFrame
        float fpsDelta = Mathf.Abs(
            currentFPS - ( maxChecksPerSecond / Mathf.Max(checksPerFrame, 0.001f) )
        );

        float adaptSpeed = Mathf.Clamp(
            1f / ( fpsDelta + 1f ), 0.01f, 1f
        ) * fpsStabilityFactor;

        float desiredChecksPerFrame = maxChecksPerSecond / currentFPS;

        // Smoothly interpolate checksPerFrame
        checksPerFrame = Mathf.Lerp(checksPerFrame, desiredChecksPerFrame, Time.deltaTime * adaptSpeed);
    }

    /*
     * Determines if a full iteration should happen this frame:
     * - There's no current best target
     * - The scheduled time for a full sweep has arrived
    */
    private bool RequiresUrgentUpdate(Transform currentBest, float timeNow)
    {
        // If we have no current target, do a full pass
        if (!currentBest)
            return true;

        // Check if it's time for the next full sweep
        if (timeNow > nextFullUpdateTime)
            return true;

        return false;
    }

    #endregion

    #region Helpers

    private void ResetState()
    {
        lastProcessedIndex = 0;
        nextFullUpdateTime = Time.time + updateInterval;
    }

    #endregion
}