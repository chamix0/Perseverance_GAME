using System;

namespace Arcade.Mechanics.Upgrades
{
    public class Upgrade
    {
        private UpgradeType _upgradeType;
        private ArcadePlayerData _playerData;
        private bool used;


        public Upgrade(UpgradeType upgradeType, ArcadePlayerData playerData)
        {
            _upgradeType = upgradeType;
            _playerData = playerData;
            used = false;
        }

        public bool Used
        {
            get => used;
            set => used = value;
        }

        public UpgradeType UpgradeType => _upgradeType;

        public void ApplyUpgrade()
        {
            used = true;
            switch (_upgradeType)
            {
                case UpgradeType.MoreLives:
                    _playerData.AddLive();
                    break;
                case UpgradeType.MoreGears:
                    _playerData.AddGear();
                    break;
                case UpgradeType.IncreaseShootSpeed:
                    _playerData.IncreaseShootSpeed();
                    break;
                case UpgradeType.MaxBullets:
                    _playerData.IncreaseNumCurrentBulletMaxAmmo();
                    break;
                case UpgradeType.MaxGrenades:
                    _playerData.IncreaseNumCurrentGrenadesMaxAmmo();
                    break;
                case UpgradeType.MoreBulletSlots:
                    _playerData.IncreaseNumBulletSlots();
                    break;
                case UpgradeType.MoreGrenadeSlots:
                    _playerData.IncreaseNumGrenadeSlots();
                    break;
                case UpgradeType.PointMultiplier:
                    _playerData.IncreasePointMultiplier();
                    break;
            }
        }

        public string GetText()
        {
            return _upgradeType switch
            {
                UpgradeType.MoreLives => "More Lives",
                UpgradeType.IncreaseShootSpeed => "Increase Shoot Speed",
                UpgradeType.MoreGears => "Increase Number of Gears",
                UpgradeType.MaxBullets => "Increase Bullet Capacity",
                UpgradeType.MaxGrenades => "Increase Grenade Capacity",
                UpgradeType.MoreBulletSlots => "Increase Bullet Slots",
                UpgradeType.MoreGrenadeSlots => "Increase Grenade Slots",
                UpgradeType.PointMultiplier => "Increase Points Multiplier",
            };
        }
    }
}