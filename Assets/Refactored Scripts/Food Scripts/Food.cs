using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Food : MonoBehaviour
{
    public FoodProperties foodConfig;
    private Rigidbody rb;


    #region Returning Variables
    [Tooltip("Assign the speed curve for the food object")]
    [SerializeField] private AnimationCurve timeReversedSpeedCurve;
    [SerializeField] private AnimationCurve pushedSpeedCurve;


    public bool isPushed = false;
    private Vector3 returnHeightPosition;
    private Vector3 returnStartPosition;
    #endregion


    private static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Add this GameObject to the active food list
        GameManager.currentActiveFoodTargetObjectsList.Add(gameObject);
    }


    private void Start()
    {
        returnHeightPosition = Camera.main.ViewportToWorldPoint(new Vector2(
            Random.Range(0.20f, 0.80f),
            Random.Range(0.70f, 0.80f)
            ));

        returnHeightPosition.z = -2f;

        returnStartPosition = transform.position;

        StartCoroutine(ReturnMotionCoroutinue());
    }


    private void Update()
    {
        // Check if the object has gone below the threshold
        if (transform.position.y < 1f)
        {
            Destroy(gameObject);
        }
    }


    private IEnumerator ReturnMotionCoroutinue()
    {
        while (true)
        {
            #region Move down to the bottom of the screen while not pushed or NTMR Pet's ability is active
            while (!GameManager.NTMRpetUniqueAbility && !isPushed)
            {
                rb.AddForce(Vector3.down, ForceMode.Acceleration);

                yield return waitForFixedUpdate;
            }
            #endregion

            // Make sure the pushed flag is false if the ability is active.
            if (GameManager.NTMRpetUniqueAbility)
            {
                isPushed = false;
            }

            // Capture the starting position.
            returnStartPosition = transform.position;

            // Select the appropriate curve.
            AnimationCurve currentCurve = GameManager.NTMRpetUniqueAbility ? timeReversedSpeedCurve : pushedSpeedCurve;

            float returnDuration = isPushed ? 1.5f : 2f; // Determine the return duration based on the return mode.
            float interpolationFactor = 0f; // Use a normalized interpolation parameter that goes from 0 to 1.

            while (interpolationFactor < 1f)
            {
                // Increment isCollectiblesLimitReached consistently using fixedDeltaTime.
                interpolationFactor += Time.fixedDeltaTime / returnDuration;
                float curveValue = currentCurve.Evaluate(interpolationFactor);

                Vector3 newPosition = Vector3.Lerp(
                    returnStartPosition,
                    returnHeightPosition,
                    curveValue
                    );

                rb.MovePosition(newPosition);

                if (Vector3.Distance(newPosition, returnHeightPosition) == 0f)
                {
                    isPushed = false;

                    break;
                }

                yield return waitForFixedUpdate;
            }

            // Finalize the return motion: snap to the target.
            rb.velocity = Vector3.zero;

            while (GameManager.NTMRpetUniqueAbility)
            {
                yield return null;
            }

            returnHeightPosition.x = transform.position.x;

            yield return null;
        }
    }


    private void OnDisable()
    {
        // Remove this GameObject from the active food list
        GameManager.currentActiveFoodTargetObjectsList.Remove(gameObject);
    }
}