using UnityEngine;

public class Collectable : MonoBehaviour
{
    public MoneyProperties moneyConfig;

    float speed = 1.5f;

    #region Destroying Managers
    [Header("Destroying Managers")]

    [SerializeField] float elapsedTime = 0f;
    float timeBeforeDestroy = 3f;

    float positionBeforeDestroy = 1f;
    #endregion

    private void Update()
    {
        if (transform.transform.position.y > positionBeforeDestroy)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= timeBeforeDestroy)
            {
                Destroy(gameObject);
            }


        }
    }

}
