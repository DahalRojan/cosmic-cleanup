using UnityEngine;

public class ZapEffectController : MonoBehaviour
{
    [Tooltip("Beam speed in units/sec")]
    public float speed = 200f;
    [Tooltip("Minimum time (sec) beam is visible")]
    public float minTravelTime = 0.05f;

    Vector3 startPos, endPos;
    float travelTime, elapsed;

    /// <summary>
    /// Call immediately after pooling to set endpoints.
    /// </summary>
    public void Initialize(Vector3 from, Vector3 to)
    {
        startPos = from;
        endPos   = to;
        elapsed  = 0f;

        float distance = Vector3.Distance(from, to);
        travelTime = Mathf.Max(distance / speed, minTravelTime);

        transform.position = from;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (travelTime <= 0f) return;
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / travelTime);
        transform.position = Vector3.Lerp(startPos, endPos, t);

        if (t >= 1f)
        {
            ObjectPool.Instance.ReturnObject(gameObject);
        }
    }
}
