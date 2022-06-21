using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Collectible : MonoBehaviour
{
    private bool collected = false;

    private bool push = false;

    private void OnEnable()
    {
        GameManager.OnEndOfChapter += PushObject;
    }

    private void OnDisable()
    {
        GameManager.OnEndOfChapter -= PushObject;
    }

    private void Update()
    {
        if(collected && push)
        {
            var rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.forward * 7f;
        }
    }

    public void PushObject()
    {
        if (!collected) return;

        var rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.forward * 7f;
        rb.AddForce(Vector3.forward * 7f, ForceMode.Impulse);
        push = true;
        StartCoroutine(PushObjectAsync());
    }

    IEnumerator PushObjectAsync()
    {
        yield return new WaitForSeconds(0.25f);
        push = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collected = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("EndingGround"))
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<PooledObject>().pool.ReturnObject(gameObject);
        }
    }
}
