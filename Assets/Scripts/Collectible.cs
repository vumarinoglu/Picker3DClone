using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private bool collected = false;

    private void OnEnable()
    {
        GameManager.OnEndOfChapter += PushObject;
    }

    private void OnDisable()
    {
        GameManager.OnEndOfChapter -= PushObject;
    }

    public void PushObject()
    {
        if (!collected) return;
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
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

}
