using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public int firstLevel;

    public Vector3 optionsPosition;
    Vector3 normalPosition;
    Vector3 targetPos;
    public float loadSpeed;
    public float distError;
    bool loadingOptions;

    public GameObject bindingMenu;

    public AudioMixer audioMix;

    GameObject mainCamera;

    public Dropdown dropdown;
    Resolution[] resolutions;
    List<Resolution> usableResolutions = new List<Resolution>();

    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Dropdown qualityDropdown;
    public Button loadSaveButton;
    public GameObject saveMenu;
    public Button[] saveSlots;

    public int numSaves;

    bool showingBindings = false;

    InputManager input;
    SettingsData storedSettings;
    PlayerSaveSystem saveSystem;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        storedSettings = GameObject.FindGameObjectWithTag("Stored Settings").GetComponent<SettingsData>();
        saveSystem = GameObject.FindGameObjectWithTag("Save System").GetComponent<PlayerSaveSystem>();
        input = gameObject.GetComponent<InputManager>();

        normalPosition = mainCamera.transform.position;
        Time.timeScale = 1;

        if (SaveSettingsProcess.LoadSettings() != null)
        {
            SavedSettings data = SaveSettingsProcess.LoadSettings();

            storedSettings.resolution.height = data.resHeight;
            storedSettings.resolution.width = data.resWidth;
            storedSettings.fullscreen = data.fullscreen;
            storedSettings.quality = data.quality;
            storedSettings.volume = data.volume;
            storedSettings.keyBindings = data.bindings;

            for (int i = 0; i < input.buttons.Count; i++)
            {
                input.buttons[i].name = data.bindings[i][0];
                input.buttons[i].code = (KeyCode)System.Enum.Parse(typeof(KeyCode), data.bindings[i][1]);

                if (i >= input.buttons.Count)
                {
                    int j = i - input.buttons.Count;

                    input.hotkeys[j].name = data.bindings[i][0];
                    input.hotkeys[j].code = (KeyCode)System.Enum.Parse(typeof(KeyCode), data.bindings[i][1]);
                }
            }
        } else
        {
            storedSettings.resolution.height = Screen.currentResolution.height;
            storedSettings.resolution.width = Screen.currentResolution.width;
        }

        bool hasSave = false;

        for(int i = 0; i < numSaves; i++)
        {
            if(PlayerSaveLoad.LoadPlayer(i + 1) != null)
            {
                hasSave = true;
            } else
            {
                saveSlots[i].interactable = false;
            }
        }

        if(!hasSave)
        {
            loadSaveButton.interactable = false;
        }
        

        int screenRefreshRate = Screen.currentResolution.refreshRate;

        resolutions = Screen.resolutions;
        dropdown.ClearOptions();

        List<string> resolutionStrings = new List<string>();

        int currResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == screenRefreshRate || resolutions[i].refreshRate == screenRefreshRate - 1)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                resolutionStrings.Add(option);
                usableResolutions.Add(resolutions[i]);
            }

            if (resolutions[i].width == storedSettings.resolution.width && resolutions[i].height == storedSettings.resolution.height)
            {
                currResIndex = i;
            }
        }

        dropdown.AddOptions(resolutionStrings);
        dropdown.value = currResIndex;
        dropdown.RefreshShownValue();

        volumeSlider.value = storedSettings.volume;
        fullscreenToggle.isOn = storedSettings.fullscreen;
        qualityDropdown.value = storedSettings.quality;
    }

    void Update()
    {
        if(loadingOptions)
        {
            mainCamera.transform.position = new Vector3(Mathf.Lerp(mainCamera.transform.position.x, targetPos.x, loadSpeed), Mathf.Lerp(mainCamera.transform.position.y, targetPos.y, loadSpeed), normalPosition.z);

            if(Vector3.Distance(mainCamera.transform.position, targetPos) <= distError)
            {
                mainCamera.transform.position = targetPos;
                targetPos = Vector3.zero;
                loadingOptions = false;
            }
        }

        if(input.ButtonDown("Escape") && showingBindings)
        {
            showingBindings = false;
            bindingMenu.SetActive(false);

            storedSettings.UpdateBindings();
        }

        if(saveMenu.activeSelf && input.ButtonDown("Escape"))
        {
            saveMenu.SetActive(false);
        }
    }

    public void ShowSaves()
    {
        saveMenu.SetActive(!saveMenu.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadOptions()
    {
        targetPos = optionsPosition;
        loadingOptions = true;
    }

    public void BackToMenu()
    {
        targetPos = normalPosition;
        loadingOptions = true;

        SaveSettingsProcess.SaveSettings(storedSettings);
    }

    public void SetVolume(float volume)
    {
        audioMix.SetFloat("MasterVolume", volume);
        storedSettings.volume = volume;
    }

    public void SetQuality(int quality)
    {
        storedSettings.quality = quality;
        QualitySettings.SetQualityLevel(quality);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        storedSettings.fullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resIndex)
    {
        Resolution res = usableResolutions[resIndex];

        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void ShowBindings()
    {
        showingBindings = true;
        bindingMenu.SetActive(true);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(firstLevel);
    }
}
