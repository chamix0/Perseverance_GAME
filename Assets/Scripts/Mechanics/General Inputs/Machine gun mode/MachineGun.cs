using System.Collections;
using System.Diagnostics;
using Mechanics.General_Inputs.Machine_gun_mode.Observers;
using Mechanics.Shoot.Bullets;
using UnityEngine;

namespace Mechanics.General_Inputs.Machine_gun_mode
{
    [DefaultExecutionOrder(6)]
    public class MachineGun : Subject
    {
        [SerializeField] private LayerMask collisionLayers;
        RaycastHit hit;
        private CameraController _cameraController;

        private PlayerValues _playerValues;

        //visibility
        bool _isVisible = false;

        //laser
        [SerializeField] LineRenderer laserRay;
        [SerializeField] private float laserWidth = 0.1f;
        [SerializeField] private float laserMaxDistance = 100;
        private Vector3 laserEndPoint;
        [SerializeField] private Transform aimPoint;
        [SerializeField] private Transform aimSphere;

        //drum
        [SerializeField] private Rigidbody drumRigidbody;
        [SerializeField] private float drumSpeed = 50;

        //shooting
        [SerializeField] float bulletSpeed = 1;
        [SerializeField] private int burstNumber = 3;
        private ShootBullet shooter;
        private ShootingMode shootingMode = ShootingMode.Manual;
        private int modeIndex = 0;

        //automatic shooting
        public bool _isAutomaticShooting;
        private Stopwatch automaticShootTimer;
        [SerializeField] private int automaticShootCooldown = 1250;

        //recoil
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float K = 1000, force = 1;
        [SerializeField] private Transform placeHolder;
        private bool updateHook = false;

        private Stopwatch hookTimer;
        private float hookCooldown = 1f;

        //materials
        private DissolveMaterials dissolveMaterials;

        //observers
        [SerializeField] private MachineGunSounds machineGunSounds;
        private MachineGunParticles machineGunParticles;

        //variables
        private bool aiming;

        //gui
        private GuiManager guiManager;

        //slow time 

        private void Awake()
        {
            automaticShootTimer = new Stopwatch();
            hookTimer = new Stopwatch();
        }

        void Start()
        {
            _playerValues = FindObjectOfType<PlayerValues>();
            dissolveMaterials = GetComponentInChildren<DissolveMaterials>();
            _cameraController = FindObjectOfType<CameraController>();
            guiManager = FindObjectOfType<GuiManager>();
            shooter = GetComponentInChildren<ShootBullet>();
            aimSphere.gameObject.SetActive(false);
            machineGunParticles = GetComponent<MachineGunParticles>();
            AddObserver(FindObjectOfType<RumbleObserver>());
            AddObserver(machineGunParticles);
            AddObserver(machineGunSounds);

            Vector3[] initLaserPos = new Vector3[2] { Vector3.zero, Vector3.zero };
            // laserRay = GetComponent<LineRenderer>();
            laserRay.SetPositions(initLaserPos);
            laserRay.SetWidth(0, 0);
            shootingMode = (ShootingMode)modeIndex;
            _rigidbody.isKinematic = true;
        }

        // Update is called once per frame
        void Update()
        {
            CurrentInput currentInput = _playerValues.GetCurrentInput();

            //show machine guns its its on machine gun mode and hide it if not
            if ((currentInput == CurrentInput.ShootMovement || currentInput == CurrentInput.ArcadeMechanics) &&
                !_isVisible)
            {
                ShowMachineGun();
            }
            else if ((currentInput is not CurrentInput.ShootMovement && currentInput is not CurrentInput.ArcadeMechanics)&&_isVisible)
            {
                HideMachinegun();
            }


            //dont do anything if machinegun is not visible
            if (!_isVisible)
            {
                return;
            }

            //do automatic shooting
            if (_isAutomaticShooting)
            {
                automaticShooting();
            }

            //stop aiming if its paused
            if (_playerValues.GetPaused())
            {
                StopAim();
            }

            //update look at    
            _rigidbody.transform.LookAt(aimPoint);

            //roll the drum
            drumRigidbody.transform.localRotation = Quaternion.Euler(90, 0, drumRigidbody.transform
                .rotation.eulerAngles.z);
        }

        private void FixedUpdate()
        {
            if (!_isVisible)
            {
                return;
            }

            if (updateHook)
            {
                HookPosition();
            }

            if (aiming)
            {
                RotateDrum();
            }
        }

        private void LateUpdate()
        {
            if (_playerValues.GetCurrentInput() is CurrentInput.ShootMovement or CurrentInput.ArcadeMechanics)
            {
                if (!_playerValues.dead && _isVisible && aiming)
                {
                    Laser();
                }
                else
                {
                    StopAim();
                }
            }
        }


        public int Shoot()
        {
            //  if (aiming)
            // {
            hookTimer.Restart();
            if (shootingMode is ShootingMode.Automatic)
            {
                _isAutomaticShooting = true;
                return 100;
            }

            StartCoroutine(ShootingModeCoroutine(shootingMode));
            if (shootingMode is ShootingMode.Manual)
                return 1000;
            if (shootingMode is ShootingMode.Burst)
                return 2000;
            //   }

            return 1000;
        }

        public void StopAutomaticShooting()
        {
            _isAutomaticShooting = false;
        }

        public void ActualShotArcade(BulletType bulletType)
        {
            _cameraController.Shake();
            shooter.Shoot(bulletType, bulletSpeed);
            recoil();
            NotifyObservers(PlayerActions.Shoot);
        }

        public void ActualShotArcadeRandom(BulletType bulletType, float randomRange)
        {
            _cameraController.Shake();
            shooter.ShootRandom(bulletType, bulletSpeed, randomRange);
            recoil();
            NotifyObservers(PlayerActions.Shoot);
        }

        public void ActualShot()
        {
            _cameraController.Shake();
            shooter.Shoot(bulletSpeed);
            recoil();
            NotifyObservers(PlayerActions.Shoot);
        }

        public void Aim()
        {
            if (!aiming)
                NotifyObservers(PlayerActions.Aim);
            aiming = true;
            Laser();
            // SlowTime();
            if (!aimSphere.gameObject.activeSelf)
                aimSphere.gameObject.SetActive(true);
        }

        public void StopAim()
        {
            if (aiming)
            {
                aiming = false;
                StopLaser();
                NotifyObservers(PlayerActions.StopAim);
            }
        }


        public void ShowMachineGun()
        {
            if (!_isVisible)
            {
                dissolveMaterials.DissolveIn();
                _isVisible = true;
                guiManager.SetMachinegun((int)shootingMode);
            }
        }

        public bool HideMachinegun()
        {
            bool aux = _isVisible;
            if (_isVisible)
            {
                dissolveMaterials.DissolveOut();
                _isVisible = false;
                StopLaser();
                guiManager.SetMachinegun(-1);
            }

            return aux;
        }

        public void NextShootingMode()
        {
            _isAutomaticShooting = false;
            modeIndex = (modeIndex + 1) % 3;
            shootingMode = (ShootingMode)modeIndex;
            guiManager.SetMachinegun((int)shootingMode);
            NotifyObservers(PlayerActions.ChangeShootingMode);
        }

        public void ResetShootingMode()
        {
            if (_isVisible)
            {
                _isAutomaticShooting = false;
                shootingMode = ShootingMode.Manual;
                modeIndex = (int)shootingMode;
                guiManager.SetMachinegun((int)shootingMode);
            }
        }

        public void SetShootingMode(ShootingMode mode)
        {
            shootingMode = mode;
            modeIndex = (int)shootingMode;
            NotifyObservers(PlayerActions.ChangeShootingMode);
            guiManager.SetMachinegun((int)shootingMode);
        }

        public ShootingMode GetShootingMode()
        {
            return shootingMode;
        }

        public void PrevShootingMode()
        {
            _isAutomaticShooting = false;
            modeIndex = modeIndex - 1 < 0 ? 2 : modeIndex - 1;
            shootingMode = (ShootingMode)modeIndex;
            guiManager.SetMachinegun((int)shootingMode);
            NotifyObservers(PlayerActions.ChangeShootingMode);
        }

        #region passive actions

        private void automaticShooting()
        {
            Laser();
            if (!automaticShootTimer.IsRunning)
            {
                ActualShot();
                automaticShootTimer.Start();
            }

            if (automaticShootTimer.Elapsed.TotalMilliseconds > automaticShootCooldown)
            {
                ActualShot();
                automaticShootTimer.Restart();
            }
        }

        private void RotateDrum()
        {
            drumRigidbody.AddRelativeTorque(0, 0, drumSpeed, ForceMode.Acceleration);
        }

        private void recoil()
        {
            _rigidbody.isKinematic = false;
            hookTimer.Start();
            _rigidbody.AddRelativeForce(-Vector3.forward * force, ForceMode.Impulse);
            updateHook = true;
        }

        private void HookPosition()
        {
            var position1 = placeHolder.position;
            var centerOfMass = _rigidbody.position;
            float magnitude = (centerOfMass - position1).magnitude;
            Vector3 auxForce = -K * magnitude * (centerOfMass - position1) / magnitude;

            if (!float.IsNaN(auxForce.x) || !float.IsNaN(auxForce.y) || !float.IsNaN(auxForce.z))
                _rigidbody.AddForce(auxForce, ForceMode.Acceleration);

            if (hookTimer.Elapsed.TotalSeconds > hookCooldown && magnitude < 0.3f)
            {
                hookTimer.Stop();
                hookTimer.Reset();
                _rigidbody.isKinematic = true;
            }
        }

        private void Laser()
        {
            if (laserRay.startWidth <= 0)
            {
                laserRay.enabled = true;
                laserRay.startWidth = laserWidth;
                laserRay.endWidth = laserWidth;
            }

            var position = shooter.transform.position;
            Vector3 direction = (aimPoint.position - position).normalized;
            Ray ray = new Ray(position, direction);
            if (Physics.Raycast(ray, out hit, laserMaxDistance,
                    collisionLayers))
            {
                laserEndPoint = hit.point;
                aimSphere.position = ray.GetPoint(hit.distance - 0.1f);
            }
            else
            {
                laserEndPoint = ray.GetPoint(laserMaxDistance);
                aimSphere.position = ray.GetPoint(laserMaxDistance);
            }

            laserRay.SetPosition(0, laserEndPoint);
            laserRay.SetPosition(1, position);
        }

        private void StopLaser()
        {
            laserRay.startWidth = 0;
            laserRay.endWidth = 0;
            laserRay.enabled = false;
            aimSphere.gameObject.SetActive(false);
        }

        #endregion

        public bool isAiming()
        {
            return aiming;
        }

        private void ConstraintLocalAxisRigidbody()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
            localVelocity.y = 0;
            localVelocity.z = 0;
            _rigidbody.velocity = transform.TransformDirection(localVelocity);
        }

        IEnumerator ShootingModeCoroutine(ShootingMode mode)
        {
            switch (mode)
            {
                case ShootingMode.Manual:
                    ActualShot();
                    break;
                case ShootingMode.Burst:
                    for (int i = 0; i < burstNumber; i++)
                    {
                        ActualShot();
                        yield return new WaitForSeconds(0.25f);
                    }

                    break;
            }
        }
    }
}