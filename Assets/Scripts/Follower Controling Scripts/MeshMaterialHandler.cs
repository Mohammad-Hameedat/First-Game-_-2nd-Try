using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMaterialHandler : MonoBehaviour
{
    public Material[] materials;

    void Start()
    {

    }

    void Update()
    {

    }

    private void ChangeMaterial()
    {
        bool r = GetComponent<FollowerController>().IsHungry();
    }
}
