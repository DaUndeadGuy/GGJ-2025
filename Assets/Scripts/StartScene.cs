using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene(1);
    }
    public void EndScene()
    {
        SceneManager.LoadScene(2);
    }

    public void ManiMenuScene()
    {
        SceneManager.LoadScene(0);
    }
}
