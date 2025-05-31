using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void play()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void exit()
    {
        Debug.Log("Game Exited");
        Application.Quit();
    }
}
