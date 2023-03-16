using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public Transform player;
    private PlayerScript playerScript;
    public float detectionRadius = 10f;
    private float fireRate = 2.5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float turnSpeed = 5f;
    public float maxShootAngle = 20;
    public Transform gunPoint;

    public int damage = 10;

    private float nextFireTime;

    [SerializeField] Animator animator;

    private int health = 5;


    private void Start()
    {
        playerScript = player.GetComponent<PlayerScript>();
    }

    private void Update()
    {
        CheckIfThePlayerIsInSight();
    }

    private void CheckIfThePlayerIsInSight()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            // this rotates even if the player is not visible but whatever
            Vector3 direction = player.position - transform.position;
            this.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, detectionRadius))
            {

                // Fire at player if enough time has passed
                if (Time.time >= nextFireTime)
                {
                    nextFireTime = Time.time + 1f / fireRate;

                    Quaternion randomRotation = Quaternion.Euler(Random.Range(-maxShootAngle, maxShootAngle), Random.Range(-maxShootAngle, maxShootAngle), Random.Range(-maxShootAngle, maxShootAngle));
                    Vector3 shootDirection = randomRotation * direction;

                    if (Physics.Raycast(transform.position, shootDirection, out hit, detectionRadius))
                    {
                        GameObject newRef = Instantiate(playerScript.bulletPrefab);

                        newRef.transform.position = hit.point;
                        newRef.transform.parent = hit.transform;

                        animator.SetTrigger("Shoot");
                        nextFireTime = Time.time + 1f / fireRate;
                        if (hit.transform == player)
                        {
                            playerScript.TakeDamage(10);
                        }
                        else if (hit.transform.GetComponent<MarchingSquare>())
                        {
                            var marchComp = hit.transform.GetComponent<MarchingSquare>();

                            marchComp.ImpactReceiver(newRef.transform.localPosition, 1, shootDirection, 1f, playerScript.bulletDamage);
                        }

                        Instantiate(playerScript.effect, hit.point, playerScript.effect.transform.rotation);
                        var obj = Instantiate(PlayerScript.instance.muzzleFlashObj[Random.Range(0, PlayerScript.instance.muzzleFlashObj.Count)], firePoint.transform.position, firePoint.transform.rotation);
                        obj.transform.parent = this.transform;
                    }
                }

            }
        }
    }

    public void HearNoiseAt(Vector3 position)
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            // this rotates even if the player is not visible but whatever
            Vector3 direction = position - transform.position;
            this.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            RaycastHit hit;

            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;

                Quaternion randomRotation = Quaternion.Euler(Random.Range(-maxShootAngle, maxShootAngle), Random.Range(-maxShootAngle, maxShootAngle), 0f);
                Vector3 shootDirection = randomRotation * direction;

                if (Physics.Raycast(transform.position, shootDirection, out hit, detectionRadius))
                {
                    GameObject newRef = Instantiate(playerScript.bulletPrefab);

                    newRef.transform.position = hit.point;
                    newRef.transform.parent = hit.transform;

                    animator.SetTrigger("Shoot");
                    nextFireTime = Time.time + 1f / fireRate;
                    if (hit.transform == player)
                    {
                        playerScript.TakeDamage(10);
                    }
                    else if (hit.transform.GetComponent<MarchingSquare>())
                    {
                        var marchComp = hit.transform.GetComponent<MarchingSquare>();

                        marchComp.ImpactReceiver(newRef.transform.localPosition, 1, shootDirection, 1f, playerScript.bulletDamage);
                    }

                    Instantiate(playerScript.effect, hit.point, Quaternion.LookRotation(hit.normal));
                    var obj = Instantiate(PlayerScript.instance.muzzleFlashObj[Random.Range(0, PlayerScript.instance.muzzleFlashObj.Count)], firePoint.transform.position, firePoint.transform.rotation);
                    obj.transform.parent = this.transform;
                }
            }
        }
    }

    public void TakeDamage()
    {
        health--;

        if (health == 0)
            Destroy(gameObject);
    }

}
