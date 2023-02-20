using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Main_menu
{
    public class MenuCamerasController : MonoBehaviour
    {
        [SerializeField] private List<CinemachineVirtualCamera> _cameras;


        public void SetCamera(int index)
        {
            for (int i = 0; i < _cameras.Count; i++)
                _cameras[i].Priority = i == index ? 10 : 0;
        }
    }
}