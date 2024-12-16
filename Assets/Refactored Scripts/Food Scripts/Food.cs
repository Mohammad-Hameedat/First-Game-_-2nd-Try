using UnityEngine;

public class Food : MonoBehaviour
{
    public FoodProperties foodConfig;

    private void Update()
    {
        transform.position += Vector3.down * Time.deltaTime;


        if (transform.transform.position.y < 1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        GameManager.currentActiveFoodTargetObjectsList.Remove(gameObject);
    }

}
