using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Tooltip("How many debris to zap per sector")]
    public int zapsPerSector = 12;

    private int zapsThisSector = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    public void RecordZap()
    {
        zapsThisSector++;
        if (zapsThisSector >= zapsPerSector)
            OnSectorCleared();
    }

    void OnSectorCleared()
    {
        Debug.Log($"Sector cleared ({zapsPerSector} zaps)!");
        zapsThisSector = 0;

        // <-- replace this line:
        // FindObjectOfType<DebrisManager>().SpawnSector();

        // WITH this:
        DebrisManager dm = Object.FindFirstObjectByType<DebrisManager>();
        if (dm != null)
            dm.SpawnSector();
    }

}
