using UnityEngine;

public class Target : MonoBehaviour
{
    public FoodProperties foodConfig;

    private void Update()
    {
        transform.position += Vector3.down * Time.deltaTime;


        if (transform.transform.position.y < 1f)
        {
            FollowerController.RemoveTargetObjectFromList(gameObject);

            Destroy(gameObject);
        }
    }

}
