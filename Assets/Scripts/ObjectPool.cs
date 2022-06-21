using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int initialSize;

    private readonly Stack<GameObject> instances = new Stack<GameObject>();

    private void Awake()
    {
        Assert.IsNotNull(prefab);
    }

    private void Start()
    {
        for (var i = 0; i < initialSize; i++)
        {
            var obj = CreateInstance();
            obj.SetActive(false);
            instances.Push(obj);
        }
    }

    public GameObject GetObject()
    {
        var obj = instances.Count > 0 ? instances.Pop() : CreateInstance();
        obj.SetActive(true);
        return obj;
    }

    public GameObject GetObject(Vector3 position, Vector3 velocity, Vector3 angularVelocity)
    {
        var obj = instances.Count > 0 ? instances.Pop() : CreateInstance();
        obj.transform.position = position;
        var objRb = obj.GetComponent<Rigidbody>();
        if (objRb != null)
        {
            objRb.velocity = velocity;
            objRb.angularVelocity = angularVelocity;
        }
        obj.SetActive(true);
        return obj;
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        var obj = instances.Count > 0 ? instances.Pop() : CreateInstance();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        var objRb = obj.GetComponent<Rigidbody>();
        if (objRb != null)
        {
            objRb.velocity = Vector3.zero;
            objRb.angularVelocity = Vector3.zero;
        }
        obj.SetActive(true);
        return obj;
    }

    public GameObject GetObject(Transform parent, Vector3 localPosition, Quaternion rotation)
    {
        var obj = instances.Count > 0 ? instances.Pop() : CreateInstance();
        obj.transform.parent = parent;
        obj.transform.localPosition = localPosition;
        obj.transform.rotation = rotation;
        var objRb = obj.GetComponent<Rigidbody>();
        if (objRb != null)
        {
            objRb.velocity = Vector3.zero;
            objRb.angularVelocity = Vector3.zero;
        }
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        var pooledObject = obj.GetComponent<PooledObject>();
        Assert.IsNotNull(pooledObject);
        Assert.IsTrue(pooledObject.pool == this);

        obj.SetActive(false);
        obj.transform.parent = this.transform;
        instances.Push(obj);
    }

    public void Reset()
    {
        var objectsToReturn = new List<GameObject>();
        foreach (var instance in transform.GetComponentsInChildren<PooledObject>())
        {
            if (instance.gameObject.activeSelf)
            {
                objectsToReturn.Add(instance.gameObject);
            }
        }
        foreach (var instance in objectsToReturn)
        {
            ReturnObject(instance);
        }
    }

    private GameObject CreateInstance()
    {
        var obj = Instantiate(prefab);
        var pooledObject = obj.AddComponent<PooledObject>();
        pooledObject.pool = this;
        obj.transform.SetParent(transform);
        return obj;
    }
}

public class PooledObject : MonoBehaviour
{
    public ObjectPool pool;
}
