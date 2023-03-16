using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInteract : MonoBehaviour, IInteractable
{

    [SerializeField]
    public AnimationCurve bulletDamage = new AnimationCurve();

    public float fireRate = 0.2f;
    public int burstCount = 100;
    public float burstDelay = 0.1f;

    private int burstRemaining = 0;
    private float nextFireTime;

    public bool activated = false;

    public int maxShootAngle = 40;
    public GameObject firePoint;

    public void Interact()
    {
        if (!activated)
        {
            burstRemaining = 100;
            activated = true;
        }
    }


    private void FixedUpdate()
    {
        if (burstRemaining > 0 && Time.time >= nextFireTime)
        {
            RaycastHit hit;

            Quaternion randomRotation = Quaternion.Euler(Random.Range(-maxShootAngle*2, maxShootAngle*2), Random.Range(-maxShootAngle * 2, maxShootAngle * 2), Random.Range(-maxShootAngle, maxShootAngle));
            Vector3 shootDirection = randomRotation * firePoint.transform.forward ;
            
            if (Physics.Raycast(firePoint.transform.position, shootDirection, out hit))
            {
                GameObject newRef = Instantiate(PlayerScript.instance.bulletPrefab);

                newRef.transform.position = hit.point;
                newRef.transform.parent = hit.transform;

                if (hit.transform.GetComponent<MarchingSquare>())
                {
                    var marchComp = hit.transform.GetComponent<MarchingSquare>();
                    marchComp.ImpactReceiver(newRef.transform.localPosition, 2, shootDirection, 1f, bulletDamage);
                }

                Debug.DrawRay(firePoint.transform.position, shootDirection * hit.distance, Color.red, 90);
                Instantiate(PlayerScript.instance.effect, hit.point, Quaternion.LookRotation(hit.normal));

                Instantiate(PlayerScript.instance.muzzleFlashObj[Random.Range(0, PlayerScript.instance.muzzleFlashObj.Count)], firePoint.transform.position, firePoint.transform.rotation);
            }

            burstRemaining--;

            if (burstRemaining == 0)
            {
                activated = false;
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                nextFireTime = Time.time + burstDelay;
            }
        }
    }



}
