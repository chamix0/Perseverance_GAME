using Player.Observer_pattern;
using UnityEngine;

public class MachineGunParticles : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    void Start()
    {
    }

    [SerializeField] private ParticleSystem flash;

    // Update is called once per frame
    void Update()
    {
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.Shoot)
        {
            flash.Play();
        }
    }
}