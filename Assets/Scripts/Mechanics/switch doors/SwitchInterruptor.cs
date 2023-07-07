using System.Diagnostics;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class SwitchInterruptor : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] SwitchDoorManager switchDoorManager;
    private MeshRenderer renderer;
    private Stopwatch timer;
    private float cooldoown = 500;
    private static readonly int BackgroundColor = Shader.PropertyToID("_Background_color");

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        timer = new Stopwatch();
        timer.Start();
    }

    public void SetColor()
    {
        if (switchDoorManager.redBlue)
            renderer.sharedMaterial.SetColor(BackgroundColor, Color.magenta);
        else
            renderer.sharedMaterial.SetColor(BackgroundColor, Color.green);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            if (timer.Elapsed.TotalMilliseconds > cooldoown)
            {
                switchDoorManager.FlickTheSwitch();
                timer.Restart();
            }
        }
    }
}