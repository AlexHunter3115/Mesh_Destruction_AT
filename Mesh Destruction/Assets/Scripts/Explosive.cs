using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Explosive : MonoBehaviour
{

    [SerializeField]
    public AnimationCurve weightDistribution = new AnimationCurve();


    public GameObject explosion;

    private void Start()
    {
        StartCoroutine(Explode());
    }

    IEnumerator Explode() 
    {
        yield return new WaitForSeconds(2f);
        SpawnRandomRaycasts(8);

        Instantiate(explosion, this.transform.position, explosion.transform.rotation);
        Destroy(gameObject);
    }

    void SpawnRandomRaycasts(float maxDistance)
    {
        bool firtHit = false;

        int num = Random.Range(100, 150);

        for (int i = 0; i < num; i++)
        {
            Vector3 direction = Random.insideUnitSphere;
            RaycastHit outHit;

            if (Physics.Raycast(transform.position, direction, out outHit, maxDistance))
            {
                if (outHit.transform.GetComponent<MarchingSquare>() != null)
                {
                    
                    Vector3 hitDirection = transform.TransformDirection(outHit.point - transform.position);

                    GameObject newRef = Instantiate(PlayerScript.instance.bulletPrefab);

                    newRef.transform.position = outHit.point;
                    newRef.transform.parent = outHit.transform;
                    var marchComp = outHit.transform.GetComponent<MarchingSquare>();

                    if (!firtHit)
                    {
                        marchComp.ImpactReceiver(newRef.transform.localPosition, 8, hitDirection, 1.2f, weightDistribution);
                        firtHit = true;
                    }
                    else 
                    {
                        marchComp.ImpactReceiver(newRef.transform.localPosition, 2, hitDirection, 1f, weightDistribution);
                    }
                }
            }
        }
    }
}
