/* Read the following notes before using this script:
 * 
 * This uncommented script is a refactored version of the original BombScript.cs script
 * that uses Unity's Job System to optimize the search for nearby targets.
 * 
 * And as I'm not experienced enough with Unity's Job System yet, I'm not sure if
 * this script will work as expected but the original script works fine.
 */
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    [SerializeField] private int maxTargets = 10;
    [SerializeField] private float bombingRange = 2f;

    [SerializeField] private float elapsedTime = 0f;
    [SerializeField] private float bombTime;

    private List<GameObject> nearbyTargetsList = new();

    void Start()
    {
        bombTime = Random.Range(5f, 20f);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        SearchForNearbyTargets();
    }

    private void SearchForNearbyTargets()
    {
        List<GameObject> allTargets = new();
        allTargets.AddRange(GameManager.currentActiveMainFishObjectsList);
        allTargets.AddRange(GameManager.currentActiveEnemyObjectsList);

        if (allTargets.Count == 0 && elapsedTime < bombTime)
            return;

        // Allocate native memory
        NativeArray<Vector3> targetPositions = new NativeArray<Vector3>(allTargets.Count, Allocator.TempJob);
        NativeList<int> targetIndices = new NativeList<int>(Allocator.TempJob);

        for (int i = 0; i < allTargets.Count; i++)
        {
            targetPositions[i] = allTargets[i].transform.position;
        }

        BombTargetSearchJob targetSearchJob = new BombTargetSearchJob
        {
            bombPosition = transform.position,
            bombingRangeSqr = bombingRange * bombingRange,
            targetPositions = targetPositions,
            targetIndices = targetIndices.AsParallelWriter() // Use thread-safe writer
        };

        JobHandle jobHandle = targetSearchJob.Schedule(allTargets.Count, 64);
        jobHandle.Complete();

        // Gather valid targets
        nearbyTargetsList.Clear();
        for (int i = 0; i < targetIndices.Length && i < maxTargets; i++)
        {
            nearbyTargetsList.Add(allTargets[targetIndices[i]]);
        }

        // Dispose of Native Arrays
        targetPositions.Dispose();
        targetIndices.Dispose();

        if (nearbyTargetsList.Count > 0 || elapsedTime >= bombTime)
        {
            DestroyNearbyTargets();
        }
    }

    public void DestroyNearbyTargets()
    {
        int targetsCount = 0;

        foreach (GameObject target in nearbyTargetsList)
        {
            if (target != null)
            {
                Destroy(target.gameObject);
                targetsCount++;
            }
        }

        //Debug.Log("Number of targets destroyed: " + targetsCount);
        Destroy(gameObject);
    }

    [BurstCompile]
    private struct BombTargetSearchJob : IJobParallelFor
    {
        [ReadOnly] public Vector3 bombPosition;
        [ReadOnly] public float bombingRangeSqr;
        [ReadOnly] public NativeArray<Vector3> targetPositions;

        [WriteOnly] public NativeList<int>.ParallelWriter targetIndices;

        public void Execute(int index)
        {
            float distanceSqr = ( targetPositions[index] - bombPosition ).sqrMagnitude;

            if (distanceSqr <= bombingRangeSqr)
            {
                targetIndices.AddNoResize(index); // Safe parallel write
            }
        }
    }
}




/* Read the following notes before using this script:
 * 
 * This is the original BombScript.cs script that was refactored to use Unity's Job System.
 * This script works fine without using the Job System, but I don't know if the searching
 * functionality leads to any performance issues when there are many targets in the scene,
 * but I haven't find any issues when tested it with a few scenarios.
 */

/*
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BombScript : MonoBehaviour
{
    [SerializeField]
    private int maxTargets = 10;
    [SerializeField]
    private float bombingRange = 2f;

    [SerializeField]
    private float elapsedTime = 0f;
    [SerializeField]
    private float bombTime;

    List<GameObject> nearbyTargetsList = new();

    void Start()
    {
        bombTime = Random.Range(5f, 20f);
    }


    void Update()
    {
        SearchForNearbyTargets();
    }


    private void SearchForNearbyTargets()
    {
        var nearbyTargets =
            GameManager.currentActiveMainFishObjectsList.Concat(GameManager.currentActiveEnemyObjectsList)
            .Where(x => ( x.transform.position - transform.position ).sqrMagnitude <= bombingRange * bombingRange)
            .OrderBy(x => ( x.transform.position - transform.position ).sqrMagnitude)
            .Take(maxTargets);


        elapsedTime += Time.deltaTime;


        if (nearbyTargets.ToList().Count > 0 || elapsedTime >= bombTime)
        {
            nearbyTargetsList = nearbyTargets.ToList();

            DestroyNearbyTargets();
        }
    }


    public void DestroyNearbyTargets()
    {
        int targetsCount = 0;
        foreach (GameObject currentTarget in nearbyTargetsList)
        {
            Vector3 distanceToTarget = currentTarget.transform.position - transform.position;

            if (currentTarget != null)
            {
                //Debug.Log("Targets detected, launching bomb!\nTargets count: " + nearbyTargetsList.Count);

                Destroy(currentTarget.associatedGameObject);

                targetsCount++;
            }
        }
        Debug.Log("Number of targets destroyed: " + targetsCount);

        Destroy(associatedGameObject);
    }
}*/