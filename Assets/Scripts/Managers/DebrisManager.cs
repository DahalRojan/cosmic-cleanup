using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DebrisManager : MonoBehaviour
{
    public GameObject debrisPrefab;
    public int debrisCount = 15;
    public float minDistance = 5f;
    public float maxDistance = 20f;
    public TMP_Text debrisCountText;
    public GameObject winMessage;
    public PlayerController player;

    private int zappedCount = 0;
    private List<GameObject> debrisPool;
    private DataLogger dataLogger;

    void Awake()
    {
        debrisPool = new List<GameObject>();
        if (debrisPrefab == null)
        {
            Debug.LogError("DebrisPrefab not assigned in DebrisManager!");
            return;
        }
        for (int i = 0; i < debrisCount; i++)
        {
            GameObject debris = Instantiate(debrisPrefab);
            debris.SetActive(false);
            debrisPool.Add(debris);
        }
    }

    void Start()
    {
        dataLogger = FindFirstObjectByType<DataLogger>();
        if (dataLogger == null)
        {
            Debug.LogWarning("DebrisManager could not find the DataLogger in the scene. Data will not be saved at the end of the game.");
        }

        if (player == null)
        {
            Debug.LogError("Player not assigned to DebrisManager!");
        }

        if (debrisCountText == null)
        {
            Debug.LogError("DebrisCountText not assigned!");
        }

        if (winMessage == null)
        {
            Debug.LogError("WinMessage not assigned!");
        }

        // foreach (GameObject debris in debrisPool)
        // {
        //     Vector3 direction = Random.insideUnitSphere.normalized;
        //     float distance = Random.Range(minDistance, maxDistance);
        //     Vector3 position = player != null ? player.transform.position + direction * distance : direction * distance;
        //     debris.transform.position = position;
        //     debris.transform.rotation = Random.rotation;
        //     debris.SetActive(true);
        // }
        // UpdateCountText();
        SpawnSector();
    }

    public void DebrisZapped(GameObject debris)
    {
        zappedCount++;
        UpdateCountText();
        if (zappedCount >= 12)
        {
            EndGame();
        }
    }

    void UpdateCountText()
    {
        if (debrisCountText != null)
        {
            debrisCountText.text = "Debris Zapped: " + zappedCount + " / 12";
        }
    }

    void EndGame()
    {
        if (winMessage != null) winMessage.SetActive(true);
        if (player != null)
        {
            player.enabled = false;

            // --- NEW --- Hide the zap progress bar UI
            if (player.zapProgress != null)
            {
                player.zapProgress.gameObject.SetActive(false);
            }
        }

        if (dataLogger != null)
        {
            dataLogger.WriteToFile();
        }
        else
        {
            Debug.LogWarning("DataLogger not found in scene! Cannot write data to file.");
        }
    }
    public void SpawnSector()
    {
        // Reset counter & UI
        zappedCount = 0;
        UpdateCountText();

        // Hide win message & re-enable player
        if (winMessage != null) winMessage.SetActive(false);
        if (player != null)
        {
            player.enabled = true;
            if (player.zapProgress != null)
                player.zapProgress.gameObject.SetActive(true);
        }

        // Reposition and reactivate every debris in the pool
        foreach (GameObject debris in debrisPool)
        {
            Vector3 dir = Random.insideUnitSphere.normalized;
            float dist = Random.Range(minDistance, maxDistance);
            Vector3 origin = (player != null) ? player.transform.position : Vector3.zero;

            debris.transform.position = origin + dir * dist;
            debris.transform.rotation = Random.rotation;
            debris.SetActive(true);
        }
    }
    
}