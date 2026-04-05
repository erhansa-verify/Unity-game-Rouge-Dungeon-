using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    [Header("Screens")]
    public GameObject settingScreen;
    public GameObject creditsScreen;
    public GameObject extraScreen;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple MainMenu instances detected. There should only be one MainMenu in the scene.");
            Destroy(gameObject);
        }
        
        DisableScreens();
    }

    public void OpenSettings()
    {
        if(!settingScreen.activeSelf)
            settingScreen.SetActive(true);
    }

    void DisableScreens()
    {
        settingScreen.SetActive(false);
        creditsScreen.SetActive(false);
        extraScreen.SetActive(false);
    }

    public void CloseAndOpenMenu(string screenName)
    {
        if(screenName == "Settings")
            if(!settingScreen.activeSelf)
                settingScreen.SetActive(true);
            else
                settingScreen.SetActive(false);
        else if(screenName == "Credits")
            if(!creditsScreen.activeSelf)
                creditsScreen.SetActive(true);
            else
                creditsScreen.SetActive(false);
        else if(screenName == "Extra")
            if(!extraScreen.activeSelf)
                extraScreen.SetActive(true);
            else
                extraScreen.SetActive(false);
    }

    public void CloseExtra()
    {
        if(extraScreen.activeSelf)
            extraScreen.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
