using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeCollectible : Collectible
{
    [SerializeField]
    private CollectibleType smallVersionType;

    private bool fall;

    private Rigidbody rb;

    private void OnEnable()
    {
        transform.position += Vector3.up;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (fall) return;

        var offset = transform.position;
        offset.y = 2f;
        transform.position = offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checker"))
        {
            rb.isKinematic = false;
            rb.velocity = new Vector3(0.0f, -.5f, 0.0f);
            fall = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Road"))
        {
            var objPool = GameManager.Instance.gamePools;
            GameObject smallObj;
            Vector3 randPos;

            for (int i = 0; i < 6; i++)
            {
                switch (smallVersionType)
                {
                    case CollectibleType.S_SPHERE:
                        randPos = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));
                        smallObj = objPool.smallSpherePool.GetObject(transform.position + randPos, transform.rotation);
                        smallObj.transform.parent = collision.transform.parent;
                        break;
                    case CollectibleType.S_CUBE:
                        randPos = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));
                        smallObj = objPool.smallCubePool.GetObject(transform.position + randPos, transform.rotation);
                        smallObj.transform.parent = collision.transform.parent;
                        break;
                    case CollectibleType.S_CAPSULE:
                        randPos = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));
                        smallObj = objPool.smallCapsulePool.GetObject(transform.position + randPos, transform.rotation);
                        smallObj.transform.parent = collision.transform.parent;
                        break;
                    default:
                        break;
                }
            }

            rb.isKinematic = true;
            GetComponent<PooledObject>().pool.ReturnObject(gameObject);
        }
    }
}
