using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public BoosterType boosterType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBoosts.OnBoosterTaken?.Invoke(boosterType);
            GetComponent<PooledObject>().pool.ReturnObject(gameObject);
        }
    }
}
