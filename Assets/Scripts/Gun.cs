//using UnityEngine;

//public class Gun : MonoBehaviour
//{
//    [Header("Gun Settings")]
//    public float damage = 10;
//    public float range = 100f;
//    private float nextFireTime = 0f;

//    public Camera fpsCam;

//    public float volume = 1.0f; // Assuming you have a volume variable
//    [Header("Audio Settings")]
//    public AudioClip[] gunShotSounds; // Assign your gunshot sounds in the Inspector

//    [Header("Audio Settings")]
//    private AudioSource m_AudioSource;
//    private AudioClip lastPlayedClip;

//    public ParticleSystem shellParticle; // Reference to the casing shell particle system

//    void Start()
//    {
//        m_AudioSource = GetComponent<AudioSource>();

//        if (m_AudioSource == null)
//        {
//            Debug.LogError("No AudioSource found");
//        }

//        if (gunShotSounds.Length == 0)
//        {
//            Debug.LogError("No gunshot sounds assigned.");
//        }
//    }

//    void Update()
//    {
//        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
//        {
//            PlayShootingSound();
//            Shoot();
//            nextFireTime = Time.time + 60f / fireRate; // Calculate next fire time based on RPM
//        }
//    }

//    //void Shoot()
//    //{
//    //    RaycastHit hit;
//    //    if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
//    //    {
//    //        Debug.Log(hit.transform.name);

//    //        var hitBox = hit.collider.GetComponent<HitBox>(); // Access the collider of the hit RaycastHit
//    //        if (hitBox)
//    //        {
//    //            hitBox.OnRaycastHit(this, fpsCam.transform.forward); // Pass the gun reference and direction to the HitBox
//    //        }

//    //        var rb = hit.collider.GetComponent<Rigidbody>();
//    //        if (rb)
//    //        {
//    //            Vector3 forceDirection = transform.forward * 20f; // Assuming you want to apply force in the forward direction of the object
//    //            rb.AddForceAtPosition(forceDirection, hit.point, ForceMode.Impulse);
//    //        }

//    //    }

//    //    // Emit particles from the shell particle system
//    //    if (shellParticle != null)
//    //    {
//    //        shellParticle.Emit(1);
//    //    }
//    //}

//    void PlayShootingSound()
//    {
//        if (gunShotSounds.Length > 0)
//        {
//            int n;
//            do
//            {
//                n = Random.Range(0, gunShotSounds.Length);
//            } while (gunShotSounds[n] == lastPlayedClip); // Ensure a different sound is picked

//            m_AudioSource.clip = gunShotSounds[n];
//            m_AudioSource.volume = volume;
//            m_AudioSource.PlayOneShot(m_AudioSource.clip);

//            lastPlayedClip = gunShotSounds[n];
//        }
//        else
//        {
//            Debug.LogError("No gunshot sounds assigned.");
//        }
//    }
//}
