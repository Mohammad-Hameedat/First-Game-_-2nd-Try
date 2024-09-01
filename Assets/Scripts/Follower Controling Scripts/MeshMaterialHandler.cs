using UnityEngine;

public class MeshMaterialHandler : MonoBehaviour
{

    public bool isHungry;

    public Material[] materials;

    ChildMainFishFollowerController followerController;

    private MeshRenderer meshRenderer;

    void Start()
    {
        followerController = GetComponent<ChildMainFishFollowerController>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = materials[0];
    }

    void Update()
    {
        isHungry = followerController.IsHungry();
        ChangeMaterial();
    }

    private void ChangeMaterial()
    {
        if (isHungry)
        {
            meshRenderer.material = materials[1];
        }
        else
        {
            meshRenderer.material = materials[0];
        }
    }
}
