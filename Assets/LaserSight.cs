using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    public LineRenderer laserLine;
    public float maxLaserDistance = 100f;
    public LayerMask laserLayerMask;

    void Update()
    {
        UpdateLaser();
    }

    void UpdateLaser()
    {
        laserLine.SetPosition(0, transform.position);

        RaycastHit hit;
        Vector3 endPosition;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxLaserDistance, laserLayerMask))
        {
            endPosition = hit.point;
        }
        else
        {
            endPosition = transform.position + transform.forward * maxLaserDistance;
        }

        laserLine.SetPosition(1, endPosition);
    }
}
