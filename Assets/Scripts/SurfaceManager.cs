using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceManager : MonoBehaviour
{
    public ParticleSystem woodParticle;
    public ParticleSystem metalParticle;
    public ParticleSystem concreteParticle;
    public ParticleSystem fleshParticle;
    public float range = 100f;

    public AudioClip[] woodHitSounds;
    public AudioClip[] metalHitSounds;
    public AudioClip[] concreteHitSounds;
    public AudioClip[] fleshHitSounds;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // Add an AudioSource component if it doesn't exist
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DetectSurfaceAndTrigger(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, range))
        {
            string surfaceType = "";

            // Check if the hit object is an enemy
            if (hit.collider.CompareTag("ai"))
            {
                // If the hit object is an enemy, consider it as hitting flesh
                surfaceType = "Flesh";
            }
            else
            {
                // If it's not an enemy, use the collider's tag to determine the surface type
                surfaceType = hit.collider.tag;
            }

            Debug.Log("Surface : " + surfaceType);
            ParticleSystem particleEffect = null;
            AudioClip hitSound = null;

            switch (surfaceType)
            {
                case "Wood":
                    Vector3 offsetHitPointWood = hit.point + hit.normal * 0.01f;
                    particleEffect = Instantiate(woodParticle, offsetHitPointWood, Quaternion.LookRotation(hit.normal));
                    hitSound = GetRandomClip(woodHitSounds);
                    break;
                case "Metal":
                    Vector3 offsetHitPointMetal = hit.point + hit.normal * 0.01f;
                    particleEffect = Instantiate(metalParticle, offsetHitPointMetal, Quaternion.LookRotation(hit.normal));
                    hitSound = GetRandomClip(metalHitSounds);
                    break;
                case "Concrete":
                    Vector3 offsetHitPointConcrete = hit.point + hit.normal * 0.01f;
                    particleEffect = Instantiate(concreteParticle, offsetHitPointConcrete, Quaternion.LookRotation(hit.normal));
                    hitSound = GetRandomClip(concreteHitSounds);
                    break;
                case "Flesh":
                    Vector3 offsetHitPointFlesh = hit.point + hit.normal * 0.01f;
                    particleEffect = Instantiate(fleshParticle, offsetHitPointFlesh, Quaternion.LookRotation(hit.normal));
                    hitSound = GetRandomClip(fleshHitSounds);
                    Debug.Log("Instantiated Flesh Particle: " + fleshParticle.name);
                    break;
            }

            // Play the hit sound if available
            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;
        int index = Random.Range(0, clips.Length);
        return clips[index];
    }
}
