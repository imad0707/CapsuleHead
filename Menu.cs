using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public GameObject mainMenuHolder;
    public GameObject optieMenuholder;
    public GameObject uitlegMenuholder;

    public Toggle[] resToggels;
    public int[] schermbreedte;
    int actiefSchermResIndex;
    public Toggle fullScreenToggle;

    void Start()
    {
        for (int i = 0; i < resToggels.Length; i++)
        {
            resToggels[i].isOn = i == actiefSchermResIndex;
        }

        //fullScreenToggle.isOn = isFullScreen;
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }
    public void Stop()
    {
        Application.Quit();

    }
    public void OptieMenu()
    {
        mainMenuHolder.SetActive(false);
        uitlegMenuholder.SetActive(false);
        optieMenuholder.SetActive(true);
    }
    public void UitlegMenu()
    {
        mainMenuHolder.SetActive(false);
        optieMenuholder.SetActive(false);
        uitlegMenuholder.SetActive(true);
    }
    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optieMenuholder.SetActive(false);
        uitlegMenuholder.SetActive(false);
    }
    public void SchermRes(int i)
    {
        if (resToggels[i].isOn){
            actiefSchermResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(schermbreedte[i], (int)(schermbreedte[i] / aspectRatio), false);
        }
            
    }
    public void SetFullScreen(bool isFullScreen)
    {
        for (int i = 0; i< resToggels.Length; i++)
        {
            resToggels [i].interactable = !isFullScreen;
            if (isFullScreen)
            {
                Resolution[] alleRes = Screen.resolutions;
                Resolution maxRes = alleRes[alleRes.Length - 1];
                Screen.SetResolution(maxRes.width, maxRes.height, true);
            }
            else
            {
                SchermRes(actiefSchermResIndex);
            }
        }
    }
}
