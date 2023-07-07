using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Main_menu
{
    public enum MenuCameras
    {
        EDDO,
        NewGame,
        Gallery,
        Settings,
        Credits,
        Tutorial
    }

    public class MenuCamerasController : MonoBehaviour
    {
        [SerializeField] private List<CinemachineVirtualCamera> _cameras;
        private MenuCameras cameraTarget;
        private Vector3 camTargetPos;
        private bool updateCam;
        [SerializeField] private float speed;

        private void Start()
        {
            camTargetPos = Vector3.zero;
            cameraTarget = MenuCameras.Gallery;
        }

        private void Update()
        {
            if (updateCam)
                UpdateCamPos();
        }

        public void SetCamera(MenuCameras camera)
        {
            for (int i = 0; i < _cameras.Count; i++)
                _cameras[i].Priority = i == (int)camera ? 10 : 0;
        }

        private CinemachineVirtualCamera GetCamera(MenuCameras camera)
        {
            return _cameras[(int)camera];
        }

        public void SetCameraNewPosAndLookAt(MenuCameras camera, Vector3 pos, Transform newLookAt)
        {
            GetCamera(camera).LookAt = newLookAt;
            camTargetPos = pos;
            cameraTarget = camera;
            updateCam = true;
        }

        private void UpdateCamPos()
        {
            GetCamera(cameraTarget).transform.position = Vector3.MoveTowards(GetCamera(cameraTarget).transform.position,
                camTargetPos, speed * Time.deltaTime);
            if (Vector3.Distance(GetCamera(cameraTarget).transform.position, camTargetPos) < 0.01f)
            {
                updateCam = false;
            }
        }
    }
}