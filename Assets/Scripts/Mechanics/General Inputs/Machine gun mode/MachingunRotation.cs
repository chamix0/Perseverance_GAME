using UnityEngine;

[DefaultExecutionOrder(5)]
public class MachingunRotation : MonoBehaviour
{
    private CameraChanger cameraChanger;
    private PlayerValues playerValues;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float radius, factor;
    [SerializeField] private Transform placeholder;


    // Start is called before the first frame update
    void Start()
    {
        cameraChanger = FindObjectOfType<CameraChanger>();
        playerValues = FindObjectOfType<PlayerValues>();
    }


    private void Update()
    {
        if (!(Col(playerValues.transform.forward) || Col(playerValues.transform.right) ||
              Col(-playerValues.transform.right) ||
              Col((playerValues.transform.right + playerValues.transform.forward).normalized) ||
              Col((-playerValues.transform.right + playerValues.transform.forward).normalized)))
        {
            if (Vector3.Distance(placeholder.position, transform.parent.position) > 0.1f)
                placeholder.position = Vector3.MoveTowards(placeholder.position, transform.parent.position, factor);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraChanger.activeCamera is ActiveCamera.FirstPerson)
        {
            var transform1 = cameraChanger.GetActiveCam().transform;
            Ray ray = new Ray(transform1.position, transform1.forward);
            placeholder.transform.LookAt(ray.GetPoint(20f));
        }
        else
        {
            placeholder.rotation =
                Quaternion.Euler(0, cameraChanger.GetActiveCam().transform.rotation.eulerAngles.y, 0);
        }
    }

    private bool Col(Vector3 dir)
    {
        RaycastHit hit;

        if (Physics.Raycast(playerValues.GetPos(), dir, out hit, radius, collisionLayer))
        {
            float distanceToHit = Vector3.Distance(placeholder.position, hit.point);
            if (distanceToHit < radius)
            {
                placeholder.position += hit.normal * factor;
            }

            return true;
        }

        return false;
    }


    private bool FrontCol()
    {
        RaycastHit hitF;

        if (Physics.Raycast(playerValues.GetPos(), playerValues.transform.forward, out hitF, radius, collisionLayer))
        {
            float distanceToHit = Vector3.Distance(placeholder.position, hitF.point);
            if (distanceToHit < radius)
            {
                placeholder.position += hitF.normal * factor;
            }

            return true;
        }

        return false;
    }

    private bool RightCol()
    {
        RaycastHit hitR;

        if (Physics.Raycast(playerValues.GetPos(), playerValues.transform.right, out hitR, radius, collisionLayer))
        {
            float distanceToHit = Vector3.Distance(placeholder.position, hitR.point);
            if (distanceToHit < radius)
            {
                placeholder.position += hitR.normal * factor;
            }

            return true;
        }

        return false;
    }

    private bool diagLCol()
    {
        RaycastHit hitR;

        if (Physics.Raycast(playerValues.GetPos(),
                (-playerValues.transform.right + playerValues.transform.forward).normalized, out hitR, radius,
                collisionLayer))
        {
            float distanceToHit = Vector3.Distance(placeholder.position, hitR.point);
            if (distanceToHit < radius)
            {
                placeholder.position += hitR.normal * factor;
            }

            return true;
        }

        return false;
    }

    private bool diagRCol()
    {
        RaycastHit hitR;

        if (Physics.Raycast(playerValues.GetPos(),
                (playerValues.transform.right + playerValues.transform.forward).normalized, out hitR, radius,
                collisionLayer))
        {
            float distanceToHit = Vector3.Distance(placeholder.position, hitR.point);
            if (distanceToHit < radius)
            {
                placeholder.position += hitR.normal * factor;
            }

            return true;
        }

        return false;
    }

    private bool LeftCol()
    {
        RaycastHit hitL;
        if (Physics.Raycast(playerValues.GetPos(), -playerValues.transform.right, out hitL, radius,
                collisionLayer))
        {
            float distanceToHit = Vector3.Distance(placeholder.position, hitL.point);
            if (distanceToHit < radius)
            {
                placeholder.position
                    += hitL.normal * factor;
            }

            return true;
        }

        return false;
    }
}