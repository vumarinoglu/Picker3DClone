using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Movement : MonoBehaviour
{
    [SerializeField] 
    private float speed;
    
    protected bool isActive;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 direction)
    {
        if (!isActive) return;

        Vector3 dirNormalized = direction.normalized;
        dirNormalized.z = 1f;
        dirNormalized.x *= 1.25f;
        Vector3 offset =  dirNormalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + offset);
    }
}
