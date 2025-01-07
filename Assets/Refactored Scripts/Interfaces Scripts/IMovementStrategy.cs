using UnityEngine;

public interface IMovementStrategy
{
    void GetTarget(); // This method is used to get the target object.

    void Move(Rigidbody rb); // This method is used to define the movement strategy of the gameobject.
}
