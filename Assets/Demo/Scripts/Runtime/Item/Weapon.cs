// Designed by KINEMATION, 2024.
// KINEMATION 4.3.5

using System;
using KINEMATION.FPSAnimationFramework.Runtime.Camera;
using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.FPSAnimationFramework.Runtime.Playables;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using KINEMATION.KAnimationCore.Runtime.Input;

using Demo.Scripts.Runtime.AttachmentSystem;

using System.Collections.Generic;
using Demo.Scripts.Runtime.Character;
using UnityEngine.VFX;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Demo.Scripts.Runtime.Item
{
    public enum OverlayType
    {
        Default,
        Pistol,
        Rifle
    }
    
    public class Weapon : FPSItem
    {
        [Header("General")]
        [SerializeField] [Range(0f, 120f)] private float fieldOfView = 90f;
        
        [Header("Animations")]
        [SerializeField] private FPSAnimationAsset reloadClip;
        [SerializeField] private FPSCameraAnimation cameraReloadAnimation;
        
        [SerializeField] private FPSAnimationAsset grenadeClip;
        [SerializeField] private FPSCameraAnimation cameraGrenadeAnimation;
        [SerializeField] private OverlayType overlayType;

        [Header("Recoil")]
        [SerializeField] private RecoilAnimData recoilData;
        [SerializeField] private RecoilPatternSettings recoilPatternSettings;
        [SerializeField] private FPSCameraShake cameraShake;
        [Min(0f)] [SerializeField] private float fireRate;

        [SerializeField] private bool supportsAuto;
        [SerializeField] private bool supportsBurst;
        [SerializeField] private int burstLength;

        [Header("Attachments")] 
        
        [SerializeField]
        private AttachmentGroup<BaseAttachment> barrelAttachments = new AttachmentGroup<BaseAttachment>();
        
        [SerializeField]
        private AttachmentGroup<BaseAttachment> gripAttachments = new AttachmentGroup<BaseAttachment>();
        
        [SerializeField]
        private List<AttachmentGroup<ScopeAttachment>> scopeGroups = new List<AttachmentGroup<ScopeAttachment>>();

        [Header("Gun Settings")]
        public float damage = 10;
        public float range = 100f;
        public float Rate = 600f;
        public string weaponName;

        [Header("Magazine")]
        public int magazineCapacity = 30;  // Maximum capacity of the magazine
        public int currentAmmoCount;      // Current number of rounds in the magazine\
        public string magazineItemName = "Magazine";
          private InventoryManager inventoryManager;

        [Header("UI")]
        public TextMeshProUGUI ammoText;
        private bool isAmmoTextVisible = false;

        [Header("Gun Effects")]
        public ParticleSystem shellParticle;
        public VisualEffect muzzleFlash;
        public AudioClip[] gunShotSounds;
        public float volume = 1.0f;
        private float nextFireTime = 0f;
        public AudioClip reloadGunSound;
        public AudioClip outOfAmmoSound;
        public Transform casingPoint;


        private AudioSource m_AudioSource;
        public GameObject muzzlePoint;
        // Sound emission
        public GameObject shootingSoundPrefab;

        //~ Controller references

        private FPSController _fpsController;
        private Animator _controllerAnimator;
        private UserInputController _userInputController;
        private IPlayablesController _playablesController;
        private FPSCameraController _fpsCameraController;
        
        private FPSAnimator _fpsAnimator;
        private FPSAnimatorEntity _fpsAnimatorEntity;

        private RecoilAnimation _recoilAnimation;
        private RecoilPattern _recoilPattern;
        
        //~ Controller references
        
        private Animator _weaponAnimator;
        private int _scopeIndex;
        
        private float _lastRecoilTime;
        private int _bursts;
        private FireMode _fireMode = FireMode.Semi;
        
        private static readonly int OverlayType = Animator.StringToHash("OverlayType");
        private static readonly int CurveEquip = Animator.StringToHash("CurveEquip");
        private static readonly int CurveUnequip = Animator.StringToHash("CurveUnequip");

        private void OnActionEnded()
        {
            if (_fpsController == null) return;
            _fpsController.ResetActionState();
        }

        protected void UpdateTargetFOV(bool isAiming)
        {
            float fov = fieldOfView;
            float sensitivityMultiplier = 1f;
            
            if (isAiming && scopeGroups.Count != 0)
            {
                var scope = scopeGroups[_scopeIndex].GetActiveAttachment();
                fov *= scope.aimFovZoom;

                sensitivityMultiplier = scopeGroups[_scopeIndex].GetActiveAttachment().sensitivityMultiplier;
            }

            _userInputController.SetValue("SensitivityMultiplier", sensitivityMultiplier);
            _fpsCameraController.UpdateTargetFOV(fov);
        }

        protected void UpdateAimPoint()
        {
            if (scopeGroups.Count == 0) return;

            var scope = scopeGroups[_scopeIndex].GetActiveAttachment().aimPoint;
            _fpsAnimatorEntity.defaultAimPoint = scope;
        }
        
        protected void InitializeAttachments()
        {
            foreach (var attachmentGroup in scopeGroups)
            {
                attachmentGroup.Initialize(_fpsAnimator);
            }
            
            _scopeIndex = 0;
            if (scopeGroups.Count == 0) return;

            UpdateAimPoint();
            UpdateTargetFOV(false);
        }
        
        public override void OnEquip(GameObject parent)
        {
            if (parent == null) return;
            
            _fpsAnimator = parent.GetComponent<FPSAnimator>();
            _fpsAnimatorEntity = GetComponent<FPSAnimatorEntity>();
            
            _fpsController = parent.GetComponent<FPSController>();
            _weaponAnimator = GetComponentInChildren<Animator>();
            
            _controllerAnimator = parent.GetComponent<Animator>();
            _userInputController = parent.GetComponent<UserInputController>();
            _playablesController = parent.GetComponent<IPlayablesController>();
            _fpsCameraController = parent.GetComponentInChildren<FPSCameraController>();
            
            InitializeAttachments();
            
            _recoilAnimation = parent.GetComponent<RecoilAnimation>();
            _recoilPattern = parent.GetComponent<RecoilPattern>();
            
            _controllerAnimator.SetFloat(OverlayType, (float) overlayType);
            _fpsAnimator.LinkAnimatorProfile(gameObject);
            
            barrelAttachments.Initialize(_fpsAnimator);
            gripAttachments.Initialize(_fpsAnimator);
            
            _recoilAnimation.Init(recoilData, fireRate, _fireMode);

            if (_recoilPattern != null)
            {
                _recoilPattern.Init(recoilPatternSettings);
            }
            
            _controllerAnimator.CrossFade(CurveEquip, 0.15f);

            //Initialise with full magazine
            currentAmmoCount = magazineCapacity;  
        }

        public override void OnUnEquip()
        {
            _controllerAnimator.CrossFade(CurveUnequip, 0.15f);
        }

        public override void OnUnarmedEnabled()
        {
            _controllerAnimator.SetFloat(OverlayType, 0);
            _userInputController.SetValue(FPSANames.PlayablesWeight, 0f);
            _userInputController.SetValue(FPSANames.StabilizationWeight, 0f);
        }

        public override void OnUnarmedDisabled()
        {
            _controllerAnimator.SetFloat(OverlayType, (int) overlayType);
            _userInputController.SetValue(FPSANames.PlayablesWeight, 1f);
            _userInputController.SetValue(FPSANames.StabilizationWeight, 1f);
            _fpsAnimator.LinkAnimatorProfile(gameObject);
        }

        public override bool OnAimPressed()
        {
            _userInputController.SetValue(FPSANames.IsAiming, true);
            UpdateTargetFOV(true);
            _recoilAnimation.isAiming = true;
            
            return true;
        }

        public override bool OnAimReleased()
        {
            _userInputController.SetValue(FPSANames.IsAiming, false);
            UpdateTargetFOV(false);
            _recoilAnimation.isAiming = false;
            
            return true;
        }

        public override bool OnFirePressed()
        {
            Debug.Log("Fire pressed");

            // Prevent firing if the inventory is active
            if (inventoryManager != null && inventoryManager.IsInventoryActive())
            {
                Debug.Log("Cannot fire: Inventory is active.");
                return false;

            }

            // Check if there's ammo left
            if (currentAmmoCount > 0)
            {
                // Do not allow firing faster than the allowed fire rate.
                if (Time.unscaledTime - _lastRecoilTime < 60f / fireRate)
                {
                    return false;
                }


                // Call the shooting method
                Shoot();

                // Call the sound and particle methods
                PlayShootingSound();

                _lastRecoilTime = Time.unscaledTime;
                _bursts = burstLength;

                currentAmmoCount--;  // Decrease ammo count after firing
                Debug.Log("Ammo: " + currentAmmoCount);

                UpdateAmmoText();

                OnFire();

                return true;
            }
            else
            {
                // Trigger click sound when out of ammo
                if (outOfAmmoSound != null && m_AudioSource != null)
                {
                    m_AudioSource.PlayOneShot(outOfAmmoSound);
                }

                // Trigger reload or handle empty magazine scenario
                if (supportsAuto || supportsBurst) 
                {
                    OnReload(); 
                }

                return false;  // Cannot fire due to empty magazine
            }
        }


        public override bool OnFireReleased()
        {
            if (_recoilAnimation != null)
            {
                _recoilAnimation.Stop();
            }
            
            if (_recoilPattern != null)
            {
                _recoilPattern.OnFireEnd();
            }
            
            CancelInvoke(nameof(OnFire));
            return true;
        }

        public override bool OnReload()
        {
            if (currentAmmoCount < magazineCapacity)  // Check if magazine isn't full
            {
                if (!FPSAnimationAsset.IsValid(reloadClip))
                {
                    return false;
                }

                // Find a magazine in the inventory
                itemSlot magazineSlot = inventoryManager.GetItemSlotByName(magazineItemName);

                if (magazineSlot != null && magazineSlot.quantity > 0)
                {
                    // Play reload sound
                    if (m_AudioSource != null && reloadGunSound != null)
                    {
                        m_AudioSource.PlayOneShot(reloadGunSound);
                    }

                    _playablesController.PlayAnimation(reloadClip, 0f);

                    if (_weaponAnimator != null)
                    {
                        _weaponAnimator.Rebind();
                        _weaponAnimator.Play("Reload", 0);
                    }

                    if (_fpsCameraController != null)
                    {
                        _fpsCameraController.PlayCameraAnimation(cameraReloadAnimation);
                    }

                    Invoke(nameof(OnActionEnded), reloadClip.clip.length * 0.85f);

                    // Use the magazine from the inventory
                    magazineSlot.quantity--;
                    if (magazineSlot.quantity <= 0)
                    {
                        magazineSlot.EmptySlot();
                    }
                    else
                    {
                        magazineSlot.UpdateQuantityText();
                    }

                    // Refill the magazine
                    currentAmmoCount = magazineCapacity;

                    UpdateAmmoText();

                    return true;
                }
                else
                {
                    Debug.Log("No magazines available in the inventory.");
                    return false;  // No magazines available in inventory
                }
            }
            else
            {
                return false;  // Magazine is already full, cannot reload
            }
        }



        public override bool OnGrenadeThrow()
        {
            if (!FPSAnimationAsset.IsValid(grenadeClip))
            {
                return false;
            }

            _playablesController.PlayAnimation(grenadeClip, 0f);
            
            if (_fpsCameraController != null)
            {
                _fpsCameraController.PlayCameraAnimation(cameraGrenadeAnimation);
            }
            
            Invoke(nameof(OnActionEnded), grenadeClip.clip.length * 0.8f);
            return true;
        }
        
        private void OnFire()
        {
            if (_weaponAnimator != null)
            {
                _weaponAnimator.Play("Fire", 0, 0f);
            }
            
            _fpsCameraController.PlayCameraShake(cameraShake);

            if (_recoilAnimation != null && recoilData != null)
            {
                _recoilAnimation.Play();
            }

            if (_recoilPattern != null)
            {
                _recoilPattern.OnFireStart();
            }

            if (_recoilAnimation.fireMode == FireMode.Semi)
            {
                Invoke(nameof(OnFireReleased), 60f / fireRate);
                return;
            }
            
            if (_recoilAnimation.fireMode == FireMode.Burst)
            {
                _bursts--;
                
                if (_bursts == 0)
                {
                    OnFireReleased();
                    return;
                }
            }
            
            Invoke(nameof(OnFire), 60f / fireRate);
        }

        public override void OnCycleScope()
        {
            if (scopeGroups.Count == 0) return;
            
            _scopeIndex++;
            _scopeIndex = _scopeIndex > scopeGroups.Count - 1 ? 0 : _scopeIndex;
            
            UpdateAimPoint();
            UpdateTargetFOV(true);
        }

        private void CycleFireMode()
        {
            if (_fireMode == FireMode.Semi && supportsBurst)
            {
                _fireMode = FireMode.Burst;
                _bursts = burstLength;
                return;
            }

            if (_fireMode != FireMode.Auto && supportsAuto)
            {
                _fireMode = FireMode.Auto;
                return;
            }

            _fireMode = FireMode.Semi;
        }
        
        public override void OnChangeFireMode()
        {
            CycleFireMode();
            _recoilAnimation.fireMode = _fireMode;
        }

        public override void OnAttachmentChanged(int attachmentTypeIndex)
        {
            if (attachmentTypeIndex == 1)
            {
                barrelAttachments.CycleAttachments(_fpsAnimator);
                return;
            }

            if (attachmentTypeIndex == 2)
            {
                gripAttachments.CycleAttachments(_fpsAnimator);
                return;
            }

            if (scopeGroups.Count == 0) return;
            scopeGroups[_scopeIndex].CycleAttachments(_fpsAnimator);
            UpdateAimPoint();
        }

        private void Shoot()
        {
            muzzleFlash.Play();

            shellParticle.Stop(); // Ensure the particle system is stopped
            shellParticle.Play(); // Restart the particle system

            RaycastHit hit;
            if (Physics.Raycast(muzzlePoint.transform.position, muzzlePoint.transform.forward, out hit, range))
            {
                //Debug.Log("Hit something: " + hit.collider.gameObject.name);

                // Apply damage to the hit object if applicable
                var hitBox = hit.collider.GetComponent<HitBox>();
                if (hitBox)
                {
                    hitBox.OnRaycastHit(this, muzzlePoint.transform.forward);
                }

                // Handle other hit effects such as adding force to rigidbodies
                var rb = hit.collider.GetComponent<Rigidbody>();
                if (rb)
                {
                    Vector3 forceDirection = muzzlePoint.transform.forward * 20f;
                    rb.AddForceAtPosition(forceDirection, hit.point, ForceMode.Impulse);
                }

                SurfaceManager surfaceManager = hit.collider.GetComponent<SurfaceManager>();
                if (surfaceManager != null)
                {
                    surfaceManager.DetectSurfaceAndTrigger(muzzlePoint.transform.position, muzzlePoint.transform.forward);
                }

                // Emit shooting sound
                EmitShootingSound();
            }
        }

        private void EmitShootingSound()
        {
            if (shootingSoundPrefab)
            {
                Instantiate(shootingSoundPrefab, muzzlePoint.transform.position, Quaternion.identity);
                Debug.Log("Shooting sound emitted.");
            }
        }

        private void PlayShootingSound()
        {
            if (gunShotSounds.Length > 0 && m_AudioSource != null)
            {
                // Choose a random gunshot sound from the array
                int n = UnityEngine.Random.Range(0, gunShotSounds.Length);
                m_AudioSource.clip = gunShotSounds[n];

                // Randomize volume slightly
                m_AudioSource.volume = UnityEngine.Random.Range(volume - 0.1f, volume + 0.1f);

                // Randomize pitch slightly
                m_AudioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);

                // Randomize stereo pan slightly
                m_AudioSource.panStereo = UnityEngine.Random.Range(-0.1f, 0.1f);

                if (Input.GetButtonDown("Fire1"))
                {
                    //m_AudioSource.loop = true;
                    m_AudioSource.Play();
                    //Debug.Log("Semi.");
                }
                // TO BE FIXED FOR SEMI AND FULL AUTO
                //// Play the gunshot sound once if the fire button is initially pressed
                else if (Input.GetButton("Fire1"))
                {
                    //m_AudioSource.loop = false;
                    m_AudioSource.Play();
                    //Debug.Log("Auto.");
                }
            }
        }


        void Update()
        {
            // Check for input to toggle ammo text visibility
            if (Input.GetKeyDown(KeyCode.Y))
            {
                isAmmoTextVisible = !isAmmoTextVisible;
                if (ammoText != null)
                {
                    ammoText.gameObject.SetActive(isAmmoTextVisible);
                }
            }

        }

        private void UpdateAmmoText()
        {
            if (ammoText != null)
            {
                ammoText.text = $"{currentAmmoCount} / {magazineCapacity}";
            }
        }


        void FixedUpdate()
        {
            Debug.DrawLine(muzzlePoint.transform.position, muzzlePoint.transform.position + muzzlePoint.transform.forward * range, Color.red, 0.1f);
        }

        void LateUpdate()
        {
            if (inventoryManager != null && inventoryManager.IsInventoryActive())
            {
                return;
            }

            if (currentAmmoCount > 0 && Input.GetButton("Fire1") && Time.time >= nextFireTime)// Full Auto
            {
                PlayShootingSound();
                Shoot();
                muzzleFlash.Play();
                //shellParticle.Play();
                PlayShell();
                nextFireTime = Time.time + 60f / Rate; // Calculate next fire time based on RPM
            }
            else if (currentAmmoCount > 0 && Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)// Semi Auto
            {
                PlayShootingSound();
                Shoot();
                muzzleFlash.Play();
                //shellParticle.Play();
                PlayShell();
                nextFireTime = Time.time + 60f / Rate; // Calculate next fire time based on RPM
            }
        }

        private void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();
            if (m_AudioSource == null)
            {
                Debug.LogError("No AudioSource component found on the weapon.");
            }
            else
            {
                m_AudioSource.clip = reloadGunSound;  // Assign reload sound clip
            }

            if (ammoText != null)
            {
                // Ensure text starts invisible
                ammoText.gameObject.SetActive(false);
            }

            // Initialize ammo text
            UpdateAmmoText();

            muzzleFlash.Stop();

            inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        }


        private void PlayShell()
        {
            Instantiate(shellParticle, casingPoint.position, transform.rotation);
        }

    }
}