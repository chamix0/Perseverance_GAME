using UnityEngine;

public class Target : MonoBehaviour
{
    public bool shot = false;
    public bool deployed = false;
    public bool startDisabled;
    [SerializeField] private MeshCollider collider;
    [SerializeField] private DissolveMaterials dissolveMaterials;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int DisableTarget()
    {
        collider.enabled = false;
        deployed = false;
        if (!shot)
        {
            dissolveMaterials.DissolveOut();
        }

        if (shot)
            return 1;
        return 0;
    }

    public void EnableTarget()
    {
        shot = false;
        deployed = true;
        dissolveMaterials.DissolveIn();
        collider.enabled = true;
    }

    public void Shot()
    {
        shot = true;
        collider.enabled = false;
        dissolveMaterials.DissolveOut();
    }
}