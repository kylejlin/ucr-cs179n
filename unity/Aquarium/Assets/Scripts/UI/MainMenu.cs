using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    GameObject mainMenu;
    GameObject settingsMenu;
    GameObject creditsMenu;

    Button playButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMenu = gameObject.transform.Find("MainMenu").gameObject;
        settingsMenu = gameObject.transform.Find("SettingsMenu").gameObject;
        creditsMenu = gameObject.transform.Find("CreditsMenu").gameObject;

        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        playButton = mainMenu.transform.Find("PlayButton").GetComponent<Button>();
        playButton.onClick.AddListener(() => StartGame());
        mainMenu.transform.Find("SettingsButton").GetComponent<Button>().onClick.AddListener(() => OpenSettings());
        mainMenu.transform.Find("CreditsButton").GetComponent<Button>().onClick.AddListener(OpenCredits);
        settingsMenu.transform.Find("BackButton").GetComponent<Button>().onClick.AddListener(BackToMain);
        creditsMenu.transform.Find("BackButton").GetComponent<Button>().onClick.AddListener(BackToMain);
        mainMenu.transform.Find("QuitButton").GetComponent<Button>().onClick.AddListener(QuitGame);
    }


    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    public void OpenCredits()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }
    public void BackToMain()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}