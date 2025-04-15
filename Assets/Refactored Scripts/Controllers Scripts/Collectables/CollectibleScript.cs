using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    #region Components References
    public CollectibleProperties collectibleProperties;
    private Rigidbody rb;
    private GameObject sttPetInstance;
    #endregion

    private float movementSpeed;

    #region Destruction Managers
    [Header("Destruction Managers")]
    [SerializeField]
    private float destructionCountdownTimer = 0f;
    private float destructionYPosition = 1f;
    private float slowModeMovementThreshold = 0.8f;
    private float slowModeDestructionTimeScaler = 0.5f;
    #endregion

    #region Returning Variables
    [Tooltip("Assign the speed curve for the Collectible object.")]
    [SerializeField] private AnimationCurve timeReversedSpeedCurve;
    [SerializeField] private AnimationCurve pushedSpeedCurve;

    public bool isPushed = false;

    private float returnDuration = 2f;

    private Vector3 topCircleCenterPosition;
    private float radius = 3f;
    private Vector3 inCircleDestinationPosition;

    private Vector3 returnHeightPosition;
    #endregion

    private static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GameManager.currentActiveCollectiblesList.Add(gameObject);
    }

    private void Start()
    {
        movementSpeed = collectibleProperties.collectableMovementSpeed;

        sttPetInstance = GameManager.cAPPetsDictionary.ContainsKey(PetType.STTPet)
            ? GameManager.cAPPetsDictionary[PetType.STTPet]
            : null;


        float randomWidthFactor = Random.Range(0.40f, 0.60f);
        float randomHeightFactor = Random.Range(0.60f, 0.80f);
        Vector2 randomCircleOffset = Random.insideUnitCircle * radius;

        topCircleCenterPosition = Camera.main.ViewportToWorldPoint(new Vector2(0.50f, 0.60f));
        topCircleCenterPosition.z = -2f;

        inCircleDestinationPosition = new Vector3(
            topCircleCenterPosition.x + randomCircleOffset.x,
            topCircleCenterPosition.y + randomCircleOffset.y,
            transform.position.z
            );


        returnHeightPosition = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0.80f));
        returnHeightPosition.x = transform.position.x;
        returnHeightPosition.z = -2f;


        // Start the coroutine to handle return motion.
        StartCoroutine(ReturnMotionCoroutine());
    }




    private IEnumerator ReturnMotionCoroutine()
    {
        while (true)
        {
            // Only update normal movement and destruction if not in return mode.
            while (!GameManager.NTMRpetUniqueAbility && !isPushed)
            {
                UpdateMovement(Time.deltaTime);
                CheckAndHandleDestruction(Time.deltaTime);

                yield return null;
            }

            if (GameManager.NTMRpetUniqueAbility)
            {
                isPushed = false;

                // If the ability is active and the collectible is still moving, slow it down.
                if (rb.velocity != Vector3.zero && transform.position.y >= destructionYPosition)
                {
                    float randomLerpDuration = Random.Range(1f, 3f);

                    float randomDuration = randomLerpDuration;

                    while (randomDuration > 0f)
                    {
                        randomDuration -= Time.fixedDeltaTime;

                        // If the collectible is still above the destruction threshold, slow it down.
                        if (transform.position.y >= destructionYPosition)
                        {
                            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime / randomLerpDuration);
                        }
                        // Otherwise, immediately break the loop and proceed to the next iteration.
                        else
                        {
                            break;
                        }

                        yield return waitForFixedUpdate;
                    }
                }
            }

            // Capture the starting position.
            Vector3 returnStartPosition = transform.position;

            // Determine the target position based on the pushed state.
            Vector3 returnTargetPosition = isPushed ? returnHeightPosition : inCircleDestinationPosition;

            // Select the appropriate curve.
            AnimationCurve currentCurve = GameManager.NTMRpetUniqueAbility ? timeReversedSpeedCurve : pushedSpeedCurve;

            // Use a normalized interpolation parameter that goes from 0 to 1.
            float interpolationFactor = 0f;

            while (interpolationFactor < 1f)
            {
                // Increment isCollectiblesLimitReached consistently using fixedDeltaTime.
                interpolationFactor += Time.fixedDeltaTime / returnDuration;
                float curveValue = currentCurve.Evaluate(interpolationFactor);

                Vector3 newPosition = Vector3.Lerp(
                    returnStartPosition,
                    returnTargetPosition,
                    curveValue
                    );

                rb.MovePosition(newPosition);

                // If the collectible is pushed and has reached the return height, break the loop.
                if (Vector3.Distance(newPosition, returnTargetPosition) == 0f)
                {
                    isPushed = false;

                    break;
                }

                yield return waitForFixedUpdate;
            }

            Vector3 finalDestinationPosition = GameManager.NTMRpetUniqueAbility
                ? inCircleDestinationPosition
                : transform.position;

            // Finalize the return motion: snap to the target.
            rb.velocity = Vector3.zero;
            rb.MovePosition(finalDestinationPosition);

            // If the ability is still active, hold at topCircleCenterPosition until it resets.
            while (GameManager.NTMRpetUniqueAbility)
            {
                yield return null;
            }

            returnHeightPosition.x = transform.position.x;

            // Allow one frame for changes before restarting the loop.
            yield return null;
        }
    }


    private void UpdateMovement(float deltaTime)
    {
        // Adjust movement speed if the pet is active.
        float effectiveSpeed = !sttPetInstance ?
            movementSpeed :
            movementSpeed * slowModeMovementThreshold;

        // Move downward until reaching the destruction threshold.
        if (transform.position.y > destructionYPosition && rb.velocity.y < 1f)
        {
            rb.AddForce(Vector3.down * effectiveSpeed, ForceMode.Acceleration);
            destructionCountdownTimer = 0f;
        }
    }

    private void CheckAndHandleDestruction(float deltaTime)
    {
        if (transform.position.y <= destructionYPosition && !GameManager.NTMRpetUniqueAbility)
        {
            rb.velocity = Vector3.zero;

            // Slow down the destruction process if the STT pet is active.
            destructionCountdownTimer += deltaTime * ( sttPetInstance != null ? slowModeDestructionTimeScaler : 1f );

            if (destructionCountdownTimer >= collectibleProperties.TimeBeforeDestroy)
            {
                BombScript bombScriptInstance = gameObject.GetComponent<BombScript>();
                if (bombScriptInstance != null)
                {
                    bombScriptInstance.DestroyNearbyTargets();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnDestroy()
    {
        // Avoid removal during scene unloading.
        if (!this.gameObject.scene.isLoaded)
            return;

        // Unregister from the active collectibles list.
        GameManager.currentActiveCollectiblesList.Remove(gameObject);
    }
}
