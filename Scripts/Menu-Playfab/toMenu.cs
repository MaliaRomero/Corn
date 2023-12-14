using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class toMenu : MonoBehaviour
{
    public GameObject signInScreen;
    public GameObject openingScreen;
    public GameObject mainScreen;

    public GameObject HowtoPlayScreen;
    public GameObject LeaderboardScreen;
    void Start()
    {
        openingScreen.SetActive(true);
        signInScreen.SetActive(false);
    }
    public void OnGoToSignInScreen()
    {
        openingScreen.SetActive(false);
        signInScreen.SetActive(true);
    }

    public void OnGoToMenuButton()
    {
        signInScreen.SetActive(false);
        mainScreen.SetActive(true);
    }
    public void OnGoToHowtoPlayButton()
    {
        mainScreen.SetActive(false);
        HowtoPlayScreen.SetActive(true);
    }
    public void OnGoToLeaderboardButton()
    {
        mainScreen.SetActive(false);
        LeaderboardScreen.SetActive(true);
    }
    public void OnBackButton()
    {
        LeaderboardScreen.SetActive(false);
        HowtoPlayScreen.SetActive(false);
        openingScreen.SetActive(true);

    }
}
