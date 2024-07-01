//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class test : MonoBehaviour
//{

//    public Camera fpsCam;
//    public float range = 100f;

//    // Start is called before the first frame update
//    void Start()
//    {
//        Debug.DrawLine(fpsCam.transform.position, fpsCam.transform.position + fpsCam.transform.forward * range, Color.red, 0.1f);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetMouseButtonDown(0)) // 0 represents the left mouse button
//        {
//            Shoot();
//        }
//    }

//    private void Shoot()
//    {

//        RaycastHit hit;
//        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
//        {
//            // Apply damage to the hit object if applicable
//            var hitBox = hit.collider.GetComponent<HitBox>();
//            if (hitBox)
//            {
//                hitBox.OnRaycastHit(this, fpsCam.transform.forward);
//            }

//            // Handle other hit effects such as adding force to rigidbodies
//            var rb = hit.collider.GetComponent<Rigidbody>();
//            if (rb)
//            {
//                Vector3 forceDirection = transform.forward * 20f;
//                rb.AddForceAtPosition(forceDirection, hit.point, ForceMode.Impulse);
//            }
//        }
//    }


//}
