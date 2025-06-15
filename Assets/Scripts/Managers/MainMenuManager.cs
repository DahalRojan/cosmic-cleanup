using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Essential for controlling TextMeshPro UI elements

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Text Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI startButtonText;
    public TextMeshProUGUI settingsButtonText;
    public TextMeshProUGUI quitButtonText;

    // --- YOUR TEXT SUGGESTIONS STORED IN ARRAYS ---

    private string[] titles = {
        "Cosmic Cleanup Crew",
        "Guardians of the Galaxy Void",
        "Space Debris Eradication"
    };

    private string[] subtitles = {
        "Navigate the Stars, Zap the Trash!",
        "Protect the Cosmos, One Debris at a Time",
        "Explore Infinite Space, Clear the Chaos"
    };

    private string[] startButtons = { "Start Mission", "Launch into Space", "Begin Cleanup" };


    // This function is called as soon as the object is loaded
    void Start()
    {
        // Apply random text to all our UI elements
        ApplyRandomText();
    }

    /// <summary>
    /// Randomly picks text from our arrays and applies it to the UI.
    /// </summary>
    void ApplyRandomText()
    {
        // Pick a random title, subtitle, and start button label
        titleText.text = titles[Random.Range(0, titles.Length)];
        subtitleText.text = subtitles[Random.Range(0, subtitles.Length)];
        startButtonText.text = startButtons[Random.Range(0, startButtons.Length)];

        // We can keep these constant or add them to arrays as well
        settingsButtonText.text = "Settings";
        quitButtonText.text = "Quit";

        // Let's also give the text a random color for fun!
        Color randomColor = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.9f, 1f); // Bright, saturated colors
        titleText.color = randomColor;
    }


    // --- BUTTON LOGIC FUNCTIONS ---

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        // For now, this can just show a message.
        // Later, you could load a settings panel or scene.
        subtitleText.text = "Settings will be implemented soon!";
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}