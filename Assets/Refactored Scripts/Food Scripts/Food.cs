using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Food : MonoBehaviour
{
    public FoodProperties foodConfig;
    private Rigidbody rb;

    private void Awake()
    {
        // Add this GameObject to the active food list
        GameManager.currentActiveFoodTargetObjectsList.Add(gameObject);

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Debug.Log(rb.velocity.magnitude);
        //rb.velocity = new Vector3(0, -1f, 0);
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude < 1f)
        {
            rb.AddForce(Vector3.down, ForceMode.Acceleration);
        }
    }


    private void Update()
    {
        // Check if the object has gone below the threshold
        if (transform.position.y < 1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        // Remove this GameObject from the active food list
        GameManager.currentActiveFoodTargetObjectsList.Remove(gameObject);
    }
}