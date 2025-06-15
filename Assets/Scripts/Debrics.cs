using UnityEngine;

public class Debris : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
        else
        {
            Debug.LogError("Renderer not found on Debris: " + name);
        }
    }

    void OnEnable()
    {
        if (rend != null)
        {
            rend.material.color = originalColor; // Reset color when enabled
        }
    }

    public void SetTargeted(bool isTargeted)
    {
        if (rend != null)
        {
            rend.material.color = isTargeted ? new Color(1f, 0.2f, 0.2f) : originalColor;
        }
    }
}