using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;
using UnityEngine.Rendering;
[DefaultExecutionOrder(3)]
public class CameraDamage : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    private PlayerValues playerValues;
    private CameraChanger cameraChanger;
    [SerializeField] private List<VolumeProfile> profiles;
    private List<Volume> cameraVolumes;
    private List<Camera> camerasList;

    private void Awake()
    {
        camerasList = new List<Camera>();
        cameraVolumes = new List<Volume>();
    }

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        playerValues.AddObserver(this);
        cameraChanger = FindObjectOfType<CameraChanger>();
        camerasList.AddRange(cameraChanger.GetCameras());
        foreach (var camera in camerasList)
        {
            cameraVolumes.Add(camera.GetComponent<Volume>());
        }
    }
    

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.NoDamage)
            SetVolumes(0);
        else if (playerAction is PlayerActions.LowDamage)
            SetVolumes(1);
        else if (playerAction is PlayerActions.MediumDamage)
            SetVolumes(2);
        else if (playerAction is PlayerActions.HighDamage)
            SetVolumes(3);
    }

    private void SetVolumes(int index)
    {
        foreach (var volume in cameraVolumes)
            volume.profile = profiles[index];
    }
}