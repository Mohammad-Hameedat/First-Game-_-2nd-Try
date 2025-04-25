using System.Collections.Generic;
using UnityEngine;


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


    #region Targeting & Range Targeting Methods

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
    public Transform GetNearestTarget(IEnumerable<GameObject> targets, Transform currentBest, Vector3 searcherPosition)
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

    #endregion


    #region Process Iteration Methods

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

    #endregion


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



#region Jobs System & Burst Compiler support version

/* Note: Read the following note before you proceed with the Burst-compiled version of the script.
 * 
 * This script's version uses the Unity Jobs System and Burst Compiler 
 * for the distance calculations and is optimized for performance and multi-threading,
 * and it works as expected, and from my testing, it increases the CPU's utilization
 * from areound 30% to 60% (stable).
 * 
 * Note 2:
 * I'm not experienced with the Unity Jobs System and Burst Compiler yet,
 * but everything seems to be working as expected.
 */


//using System.Collections.Generic;
//using UnityEngine;

//// -----------------------------------
//// Additional namespaces for Jobs/Burst
//// -----------------------------------
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Jobs;
//using Unity.Mathematics;


///// <summary>
///// A Burst-compiled job struct that calculates squared distances 
///// from a 'searcherPosition' to each element in 'Positions'.
///// </summary>
//[BurstCompile]
//public struct DistanceCheckJob : IJobParallelFor
//{
//    [ReadOnly] public NativeArray<float3> Positions;
//    [ReadOnly] public float3 SearcherPosition;

//    // Each index corresponds to the squared distance for that element.
//    [WriteOnly] public NativeArray<float> Distances;

//    public void Execute(int index)
//    {
//        float3 diff = Positions[index] - SearcherPosition;
//        Distances[index] = math.lengthsq(diff); // squared distance
//    }
//}


///*
// * Optimized version of FrameBasedTargetingStrategy with a forced "full" check
// * when there's no current target or the update interval has expired.
// * 
// * Refactored to use the Jobs System + Burst for the distance calculations, 
// * preserving the original partial iteration "frame-based" logic.
//*/
//public class FrameBasedTargetingStrategy : ITargetingStrategy
//{
//    [Tooltip("Seconds between full target sweeps")]
//    [SerializeField] private float updateInterval = 0.15f;

//    [Tooltip("Maximum checks allowed per second")]
//    [SerializeField] private int maxChecksPerSecond = 300;

//    // Time after which we do a forced full update
//    private float nextFullUpdateTime;

//    // Tracks how far into the target collection we got with partial checks
//    private int lastProcessedIndex;

//    // Dynamically adjusted number of checks per frame
//    private float checksPerFrame;

//    // Factor controlling how quickly checksPerFrame responds to FPS changes
//    private float fpsStabilityFactor = 10f;

//    // Static fallback list in case the target collection is neither List<GameObject> nor GameObject[]
//    private static readonly List<GameObject> _fallbackList = new(512);


//    public FrameBasedTargetingStrategy()
//    {
//        // Stagger the first update across multiple objects
//        nextFullUpdateTime = Time.time + UnityEngine.Random.Range(0f, updateInterval);

//        // Start with a small partial check count; adapt quickly
//        checksPerFrame = 1f;
//    }


//    #region Targeting & Range Targeting Methods

//    /*
//     * Returns the nearest target from the given collection of GameObjects,    
//     * using time-sliced iteration unless an urgent (full) update is needed.
//     * 
//     * - targets:
//     *   A collection of potential targets (e.g., from GameManager.currentActiveFoodTargetObjectsList)
//     * 
//     * - currentBest:
//     *   The currently known best target, if any
//     * 
//     * - searcherPosition:
//     *   The position of the agent searching for targets
//     * 
//     * - Returns:
//     *   The nearest target transform
//    */
//    public Transform GetNearestTarget(IEnumerable<GameObject> targets, Transform currentBest, Vector3 searcherPosition)
//    {
//        // 1) Quick null check
//        if (targets == null)
//        {
//            ResetState();
//            return null;
//        }

//        // 2) Adapt the partial-check rate based on FPS
//        UpdateRateParameters();

//        // 3) Determine if we need a forced (full) update
//        bool isUrgentUpdate = RequiresUrgentUpdate(currentBest, Time.time);
//        if (isUrgentUpdate)
//        {
//            // If urgent, reset so we can do a full iteration
//            nextFullUpdateTime = Time.time;
//        }

//        // 4) Convert/cast targets to a usable array or list
//        int targetCount;
//        List<GameObject> listRef = targets as List<GameObject>;
//        GameObject[] arrayRef = null;

//        if (listRef != null)
//        {
//            targetCount = listRef.Count;
//        }
//        else
//        {
//            arrayRef = targets as GameObject[];
//            if (arrayRef != null)
//            {
//                targetCount = arrayRef.Length;
//            }
//            else
//            {
//                // Fallback approach for any other IEnumerable<T>
//                _fallbackList.Clear();
//                foreach (var obj in targets)
//                {
//                    _fallbackList.Add(obj);
//                }
//                listRef = _fallbackList;
//                targetCount = listRef.Count;
//            }
//        }

//        if (targetCount == 0)
//        {
//            ResetState();
//            return null;
//        }

//        // 5) Perform either full or partial iteration (now uses a Burst job for distance checks)
//        Transform nearest = ProcessIteration(
//            listRef, arrayRef, targetCount,
//            currentBest, searcherPosition,
//            isUrgentUpdate
//        );

//        return nearest;
//    }

//    ///* 
//    /// NOTE: This method is commented out in the original script. 
//    /// If you need it, you can adapt the same Jobs pattern from ProcessIterationForRange. 
//    /// 
//    /// Returns a list of targets (up to 'maxNumberOfTargets') that fall within 
//    /// the specified 'rangeThreshold' from 'searcherPosition', using a frame-based 
//    /// partial iteration approach similar to GetNearestTarget.
//    /// 
//    /// public List<Transform> GetTargetsWithinRange(IEnumerable<GameObject> targets, Vector3 searcherPosition, int maxNumberOfTargets, float rangeThreshold)
//    /// {
//    ///     // 1) Quick null check
//    ///     if (targets == null)
//    ///     {
//    ///         ResetState();
//    ///         return new List<Transform>();
//    ///     }
//    ///
//    ///     // 2) Adapt the partial-check rate based on FPS (separate from main checksPerFrame)
//    ///     UpdateRateParameters();
//    ///
//    ///     // 3) Determine if we need a forced (full) update for this range-based method
//    ///     bool isUrgentUpdate = RequiresUrgentUpdateForRange(Time.time);
//    ///     if (isUrgentUpdate)
//    ///     {
//    ///         // If urgent, reset so we can do a full iteration
//    ///         nextFullUpdateTime = Time.time;
//    ///     }
//    ///
//    ///     // 4) Convert/cast targets to a usable array or list
//    ///     int targetCount;
//    ///     List<GameObject> listRef = targets as List<GameObject>;
//    ///     GameObject[] arrayRef = null;
//    ///
//    ///     if (listRef != null)
//    ///     {
//    ///         targetCount = listRef.Count;
//    ///     }
//    ///     else
//    ///     {
//    ///         arrayRef = targets as GameObject[];
//    ///         if (arrayRef != null)
//    ///         {
//    ///             targetCount = arrayRef.Length;
//    ///         }
//    ///         else
//    ///         {
//    ///             // Fallback approach for any other IEnumerable<T>
//    ///             _fallbackList.Clear();
//    ///             foreach (var obj in targets)
//    ///             {
//    ///                 _fallbackList.Add(obj);
//    ///             }
//    ///             listRef = _fallbackList;
//    ///             targetCount = listRef.Count;
//    ///         }
//    ///     }
//    ///
//    ///     if (targetCount == 0)
//    ///     {
//    ///         ResetState();
//    ///         return new List<Transform>();
//    ///     }
//    ///
//    ///     // 5) Perform either full or partial iteration
//    ///     List<Transform> targetsInRange = ProcessIterationForRange(
//    ///         listRef, arrayRef, targetCount,
//    ///         searcherPosition, rangeThreshold,
//    ///         maxNumberOfTargets, isUrgentUpdate
//    ///     );
//    ///
//    ///     return targetsInRange;
//    /// }
//    //*/

//    #endregion


//    #region Process Iteration Methods

//    /*
//     * Processes targets either partially or fully, depending on isUrgentUpdate.
//     * If isUrgentUpdate is true, we check all targets this frame.
//     * Otherwise, we check up to checksPerFrame targets in a circular fashion.
//     * 
//     * The main difference in this refactoring is that the actual distance computation 
//     * is offloaded to a Burst-compiled Job (DistanceCheckJob) for potential performance gains.
//    */
//    private Transform ProcessIteration(
//        List<GameObject> listRef,
//        GameObject[] arrayRef,
//        int targetCount,
//        Transform currentBest,
//        Vector3 searcherPosition,
//        bool isUrgentUpdate
//    )
//    {
//        Transform nearest = currentBest;
//        float bestSqrDist = ( nearest != null )
//            ? ( nearest.position - searcherPosition ).sqrMagnitude
//            : float.MaxValue;

//        // Decide how many targets to check
//        int checksAllowed = isUrgentUpdate
//            ? targetCount
//            : Mathf.FloorToInt(checksPerFrame);

//        // We only process as many as checksAllowed or targetCount, whichever is smaller
//        checksAllowed = Mathf.Min(checksAllowed, targetCount);

//        int processedCount = 0;
//        int startIndex = lastProcessedIndex;

//        // -----------------------------------------------------------
//        // Gather a subset of target positions for this frame's checks
//        // -----------------------------------------------------------
//        NativeArray<float3> positions = new NativeArray<float3>(checksAllowed, Allocator.TempJob);
//        NativeArray<int> indices = new NativeArray<int>(checksAllowed, Allocator.TempJob);

//        int validCount = 0;
//        for (int i = 0; i < targetCount && validCount < checksAllowed; i++)
//        {
//            int index = ( startIndex + i ) % targetCount;
//            GameObject candidate = ( listRef != null ) ? listRef[index] : arrayRef[index];

//            // Null check for destroyed objects
//            if (!candidate)
//                continue;

//            // Store the position and the "original" index so we can map back later
//            positions[validCount] = candidate.transform.position;
//            indices[validCount] = index;
//            validCount++;
//        }

//        // -----------------------------------------------------
//        // Schedule the distance check job for those valid items
//        // -----------------------------------------------------
//        NativeArray<float> distanceResults = new NativeArray<float>(validCount, Allocator.TempJob);
//        var distanceJob = new DistanceCheckJob
//        {
//            Positions = positions,
//            SearcherPosition = searcherPosition,
//            Distances = distanceResults
//        };

//        // We batch the parallel-for in chunks of 64 (arbitrary choice).
//        JobHandle distanceHandle = distanceJob.Schedule(validCount, 64);
//        distanceHandle.Complete();

//        // ----------------------------
//        // Find the minimal distance
//        // ----------------------------
//        float bestLocalDist = bestSqrDist;
//        int bestLocalIndex = -1;

//        for (int j = 0; j < validCount; j++)
//        {
//            float dist = distanceResults[j];
//            if (dist < bestLocalDist)
//            {
//                bestLocalDist = dist;
//                bestLocalIndex = indices[j];
//            }
//        }

//        // -------------------------------
//        // If we found a better candidate
//        // -------------------------------
//        if (bestLocalIndex != -1)
//        {
//            GameObject betterCandidate = ( listRef != null )
//                ? listRef[bestLocalIndex]
//                : arrayRef[bestLocalIndex];

//            if (betterCandidate != null)
//            {
//                nearest = betterCandidate.transform;
//                bestSqrDist = bestLocalDist;
//            }
//        }

//        // Cleanup the NativeArrays
//        positions.Dispose();
//        indices.Dispose();
//        distanceResults.Dispose();

//        // The actual number of processed objects for partial iteration
//        processedCount = validCount;

//        // Update lastProcessedIndex for next frame (partial iteration)
//        lastProcessedIndex = ( startIndex + processedCount ) % targetCount;

//        // If we've processed everything or forced a full iteration,
//        // reset for the next pass
//        if (isUrgentUpdate || processedCount == targetCount)
//        {
//            lastProcessedIndex = 0;
//            nextFullUpdateTime = Time.time + updateInterval;
//        }

//        return nearest;
//    }

//    ///*
//    // * Processes the targets either partially or fully, collecting any that fall
//    // * within 'rangeThreshold' until 'maxNumberOfTargets' is reached.
//    // *
//    // * If you wish to use a Jobs + Burst approach here as well, you can use a
//    // * pattern similar to the ProcessIteration method—collect valid positions
//    // * into a NativeArray, schedule a DistanceCheckJob, then filter or collect
//    // * results on the main thread.
//    // */
//    //private List<Transform> ProcessIterationForRange(
//    //    List<GameObject> listRef,
//    //    GameObject[] arrayRef,
//    //    int targetCount,
//    //    Vector3 searcherPosition,
//    //    float rangeThreshold,
//    //    int maxNumberOfTargets,
//    //    bool isUrgentUpdate
//    //)
//    //{
//    //    List<Transform> results = new List<Transform>(maxNumberOfTargets);
//    //
//    //    // Decide how many targets to check this frame
//    //    int checksAllowed = isUrgentUpdate
//    //        ? targetCount
//    //        : Mathf.FloorToInt(fpsStabilityFactor);
//    //
//    //    int processedCount = 0;
//    //    int startIndex = lastProcessedIndex;
//    //    float rangeThresholdSqr = rangeThreshold * rangeThreshold;
//    //
//    //    // Using a similar pattern to the single-target approach,
//    //    // gather a subset of positions, run the DistanceCheckJob,
//    //    // then pick which are within range.
//    //
//    //    // (Implementation would parallel the approach shown above.)
//    //
//    //    // Update lastProcessedIndexRange for next frame (partial iteration)
//    //    lastProcessedIndex = (startIndex + processedCount) % targetCount;
//    //
//    //    if (isUrgentUpdate || processedCount == targetCount)
//    //    {
//    //        lastProcessedIndex = 0;
//    //        nextFullUpdateTime = Time.time + updateInterval;
//    //    }
//    //
//    //    return results;
//    //}

//    #endregion


//    #region Adaptive Checks Tuning

//    /*
//     * Smoothly adjusts checksPerFrame to limit total checks to maxChecksPerSecond
//     * based on current framerate.
//    */
//    private void UpdateRateParameters()
//    {
//        float currentFPS = ( Application.targetFrameRate > 0 )
//            ? Application.targetFrameRate
//            : ( 1f / Time.deltaTime );

//        // Compare the difference between actual FPS and
//        // the "theoretical" FPS used by checksPerFrame
//        float fpsDelta = Mathf.Abs(
//            currentFPS - ( maxChecksPerSecond / Mathf.Max(checksPerFrame, 0.001f) )
//        );

//        float adaptSpeed = Mathf.Clamp(
//            1f / ( fpsDelta + 1f ), 0.01f, 1f
//        ) * fpsStabilityFactor;

//        float desiredChecksPerFrame = maxChecksPerSecond / currentFPS;

//        // Smoothly interpolate checksPerFrame
//        checksPerFrame = Mathf.Lerp(checksPerFrame, desiredChecksPerFrame, Time.deltaTime * adaptSpeed);
//    }

//    /*
//     * Determines if a full iteration should happen this frame:
//     * - There's no current best target
//     * - The scheduled time for a full sweep has arrived
//    */
//    private bool RequiresUrgentUpdate(Transform currentBest, float timeNow)
//    {
//        // If we have no current target, do a full pass
//        if (!currentBest)
//            return true;

//        // Check if it's time for the next full sweep
//        if (timeNow > nextFullUpdateTime)
//            return true;

//        return false;
//    }

//    ///*
//    // * Determines if a full iteration should happen this frame for the
//    // * range-based method (no current "best" needed, just time-based).
//    //*/
//    //private bool RequiresUrgentUpdateForRange(float timeNow)
//    //{
//    //    // Check if it's time for the next full sweep
//    //    return (timeNow > nextFullUpdateTime);
//    //}

//    #endregion


//    #region Helpers

//    private void ResetState()
//    {
//        lastProcessedIndex = 0;
//        nextFullUpdateTime = Time.time + updateInterval;
//    }

//    #endregion
//}

#endregion




/* The original version of FrameBasedTargetingStrategy (below) + Notes.
 * 
 * This version was optimized to distribute the nearest object checks across frames.
 * However, it had a potential issue that could prevent or reduce the chances of drop-frame from occurring.
 * 
 * The newly optimized version of FrameBasedTargetingStrategy (above) addresses this issue by 
 * ensuring that checks are distributed across frames while still allowing for a full update when
 * necessary, this, made the frames to be as more stable as possible.

public class FrameBasedTargetingStrategy : ITargetingStrategy
{
    private int frameInterval = 3; // Interval to distribute nearest object checks
    private int currentObjectIndex = 0; // Tracks the current object index being processed across frames

    public Transform GetNearestTarget(IEnumerable<GameObject> targetedEnemyObjectsList, Transform lastNearestObject, Vector3 currentPosition)
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

            foreach (GameObject targetObject in targetedEnemyObjectsList)
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