using System.Collections.Generic;
using UnityEngine;

namespace Arcade.Mechanics.Doors
{
    public class TriggerBuyDoor : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private List<GameObject> triggers;

        private ArcadePlayerData _playerData;
        private GuiManager _guiManager;
        [SerializeField] private DoorManager doorManager;
        private bool isIn;

        //values
        [SerializeField] private int prize;
        [SerializeField] private List<ZonesArcade> zonesThatUnlocks;

        void Start()
        {
            _guiManager = FindObjectOfType<GuiManager>();
            _playerData = FindObjectOfType<ArcadePlayerData>();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isIn = true;
                _guiManager.ShowMessage();
                if (CanPay())
                    _guiManager.SetMessageText_("Press E to open \n <color=#0d9146>" + prize + " pts</color>");
                else
                    _guiManager.SetMessageText_("Press E to open \n <color=#911f0d>" + prize + "  pts</color>");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (isIn && CanPay())
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    doorManager.OpenDoor();
                    _playerData.RemovePoints(prize);
                    isIn = false;
                    _guiManager.HideMessage();
                    
                    foreach (var zone in zonesThatUnlocks)
                        _playerData.UnlockZone(zone);

                    foreach (var trigger in triggers)
                        trigger.SetActive(false);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isIn = false;
                _guiManager.HideMessage();
            }
        }

        private bool CanPay()
        {
            return _playerData.GetPoints() >= prize;
        }
    }
}