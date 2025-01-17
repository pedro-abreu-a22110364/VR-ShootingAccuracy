using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Button practiceButton;
    public Button playButton;
    public GameObject mainMenuCanvas;
    public GameObject introCanvas;

    private bool practiceCompleted = false;

    void Start()
    {
        playButton.interactable = false;
    }

    public void StartPractice()
    {
        introCanvas.SetActive(true);
    }

    public void StartGame()
    {
        //
    }
}
