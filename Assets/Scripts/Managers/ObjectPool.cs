using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public GameObject prefab;
    public int poolSize = 20;
    private List<GameObject> pool;

    void Awake()
    {
        Instance = this;
        if (prefab == null)
        {
            Debug.LogError("Prefab not assigned to ObjectPool!");
            return;
        }
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }
        }
        Debug.LogWarning("ObjectPool exhausted, expanding pool!");
        GameObject newObj = Instantiate(prefab);
        newObj.transform.position = position;
        newObj.transform.rotation = rotation;
        newObj.SetActive(true);
        pool.Add(newObj);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}