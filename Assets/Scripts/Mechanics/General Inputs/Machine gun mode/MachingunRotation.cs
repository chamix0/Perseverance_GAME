using UnityEngine;

namespace Mechanics.General_Inputs.Machine_gun_mode
{
    [DefaultExecutionOrder(5)]
    public class MachingunRotation : MonoBehaviour
    {
        private CameraChanger cameraChanger;
        private PlayerValues playerValues;
        [SerializeField] private LayerMask collisionLayer,ignoreRaycast;
        [SerializeField] private float radius, factor;
        [SerializeField] private Transform placeholder;
        private GuiManager _guiManager;


        // Start is called before the first frame update
        void Start()
        {
            cameraChanger = FindObjectOfType<CameraChanger>();
            playerValues = FindObjectOfType<PlayerValues>();
            _guiManager = FindObjectOfType<GuiManager>();
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
            RaycastHit hit;
            Ray ray = cameraChanger.GetActiveCam().ScreenPointToRay(_guiManager.GetCrossHairPosition().position);
            Physics.Raycast(ray, out hit,Mathf.Infinity,ignoreRaycast);
            placeholder.LookAt(hit.point); 
        }

        // private void OnDrawGizmos()
        // {
        //     RaycastHit hit2;
        //     Ray ray = cameraChanger.GetActiveCam().ScreenPointToRay(_guiManager.GetCrossHairPosition().position);
        //     Physics.Raycast(ray, out hit2,Mathf.Infinity,ignoreRaycast);
        //     Gizmos.color=Color.red;
        //     Gizmos.DrawSphere(hit2.point,0.1f);
        // }


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
        
    }
}