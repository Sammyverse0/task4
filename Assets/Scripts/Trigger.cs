using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    [Header("UI to show when player wins")]
    public GameObject winnerUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowWinnerUI();
        }
    }

    void ShowWinnerUI()
    {

        if (winnerUI != null)
        {
            winnerUI.SetActive(true);
            Time.timeScale = 0f; // Optional: pause game
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Call this from the Restart Button's OnClick event
    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
