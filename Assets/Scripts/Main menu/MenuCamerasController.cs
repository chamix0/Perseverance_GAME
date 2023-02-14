using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MenuCamerasController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private List<CinemachineVirtualCamera> _cameras;
    private int currentCamera = 0;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetCamera(int index)
    {
        for (int i = 0; i < _cameras.Count; i++)
        {
            if (i == index)
                _cameras[i].Priority = 10;
            else
                _cameras[i].Priority = 0;
        }
    }
}
