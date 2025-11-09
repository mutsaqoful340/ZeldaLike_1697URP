using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuSelector : MonoBehaviour
{
    public GameObject panelSaves;
    public GameObject panelConfirm;

    public TextMeshProUGUI[] slotTexts = new TextMeshProUGUI[3];
    //private int selectedSlot = -1;

    private void Start()
    {
        panelSaves.SetActive(false);
        panelConfirm.SetActive(false);
    }

    public void OnClick_Play(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void OnClick_Load()
    {
        panelSaves.SetActive(true);
    }

    public void OnClick_ConfirmYES()
    {
        panelConfirm.SetActive(false);
    }

    public void OnClick_ConfirmNO()
    {
        panelConfirm.SetActive(false);
    }
}
