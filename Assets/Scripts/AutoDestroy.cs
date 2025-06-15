using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    void OnEnable()
    {
        Invoke("ReturnToPool", 0.3f);
    }

    void ReturnToPool()
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnObject(gameObject);
        }
    }
}