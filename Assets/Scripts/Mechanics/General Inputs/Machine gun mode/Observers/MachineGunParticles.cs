using Player.Observer_pattern;
using UnityEngine;

namespace Mechanics.General_Inputs.Machine_gun_mode.Observers
{
    public class MachineGunParticles : MonoBehaviour, IObserver
    {
        [SerializeField] private ParticleSystem flash;
        public void OnNotify(PlayerActions playerAction)
        {
            if (playerAction is PlayerActions.Shoot)
            {
                flash.Play();
            }
        }
    }
}