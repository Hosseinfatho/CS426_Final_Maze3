using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private struct FullScreenName
    {
        public string name;
        public FullScreenMode value;
    }

    [SerializeField] GameObject resolutionListDropdown;
    [SerializeField] Button buttonFullScreen;

    Resolution[] resolutions;
    TMP_Dropdown dropdown;
    FullScreenName[] fullScreenNames = { new FullScreenName { name = "Exclusive", value = FullScreenMode.ExclusiveFullScreen },
                                        new FullScreenName { name = "Fullscreen", value = FullScreenMode.FullScreenWindow },
                                        new FullScreenName { name = "Windowed", value = FullScreenMode.Windowed }};
    int fullScreenValue = 1; // default will be this + 1


    void Start()
    {
        populateResolutionDropbox();


        buttonFullScreen.onClick.AddListener(fullScreenToggle);
        fullScreenToggle(); //to set the var to false by default and populate the button's text
    }

    void fullScreenToggle()
    {
        fullScreenValue++;
        if (fullScreenValue > fullScreenNames.Length - 1)
        {
            fullScreenValue = 0;
        }

        buttonFullScreen.GetComponentInChildren<TMP_Text>().text = fullScreenNames[fullScreenValue].name;
    }

    public void buttonApplySettings()
    {
        Resolution wanted = resolutions[dropdown.value];
        Screen.SetResolution(wanted.width, wanted.height, fullScreenNames[fullScreenValue].value, wanted.refreshRateRatio);
    }

    void populateResolutionDropbox()
    {
        resolutions = Screen.resolutions;
        dropdown = resolutionListDropdown.GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        List<string> resList = new List<string>();

        foreach (var res in resolutions)
        {
            resList.Add(res.width + "x" + res.height + " @" + res.refreshRateRatio + "hz");
        }

        dropdown.AddOptions(resList);

        string current = Screen.currentResolution.width + "x" + Screen.currentResolution.height + " @" + Screen.currentResolution.refreshRateRatio + "hz";
        //Debug.Log("Current resolution is: " + current);
        int indexInList = resList.FindIndex(x => x.StartsWith(current));

        if (indexInList != 1)
        {
            dropdown.value = indexInList;
        }
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void playerLevel1()
    {
        SceneManager.LoadScene("Peter_Assignment6");
    }
}
