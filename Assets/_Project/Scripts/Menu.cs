using UnityEngine;
using UnityEngine.SceneManagement;

namespace keijo
{
    public class Menu : MonoBehaviour
    {
        public GameObject pauseMenu;
        public GameManager gameManager;
        public void StartGame()
        {
            SceneManager.LoadScene("MainScene");
        }

        public void Settings()
        {
        }

        public void MainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void ClosePauseMenu()
        {
            pauseMenu.SetActive(false);
            gameManager.Unpause();
        }
    }
}
