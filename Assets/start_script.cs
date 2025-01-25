using UnityEngine;
using UnityEngine.SceneManagement; // For loading scenes

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the next scene (replace "GameScene" with your actual scene name)
        SceneManager.LoadScene("Office (Tutorial)");
    }

    public void ExitGame()
    {
        // Exit the application
        Debug.Log("Exiting Game...");
        Application.Quit();

        // Note: Application.Quit() will not work in the editor.
        // To test, build the game and run it.
    }
}
