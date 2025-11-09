using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadUI : MonoBehaviour
{
    public TextMeshProUGUI[] slotTexts = new TextMeshProUGUI[3];
    // public GameObject panelSaveLoadUI;
    // public GameObject panelMenuUI;
    // public GameObject panelconfirmUI;
    private int selectedSlot = -1;

    private void Awake()
    {
        RefreshSlots();
        // panelSaveLoadUI.SetActive(false);
        // panelconfirmUI.SetActive(false);
    }

    // Dipanggil saat tombol slot ditekan
    public void OnClick_SelectSlot(int index)
    {
        selectedSlot = index;
        Debug.Log($"Slot {index + 1} dipilih");
    }

    // Simpan ke slot terpilih
    public void OnClick_SaveToSlot()
    {
        if (selectedSlot == -1)
        {
            Debug.Log("Pilih slot dulu sebelum menyimpan.");
            return;
        }

        PlayerData data = new PlayerData
        {
            playerName = "Hero",
            Health = PlayerScript.Instance.Health,
            Heart = PlayerScript.Instance.Heart,
            Energy = PlayerScript.Instance.Energy,
            AtkPower = PlayerScript.Instance.AtkPower,
            DefPower = PlayerScript.Instance.DefPower,
            ExpGain = PlayerScript.Instance.ExpGain,
            Coin = PlayerScript.Instance.Coin,
            Level = PlayerScript.Instance.Level,
            Progress = PlayerScript.Instance.Progress,
        };

        SaveSystem.Save(data, selectedSlot);
        RefreshSlots();
        Canvas.ForceUpdateCanvases();
    }

    // Load data dari slot
    public void OnClick_LoadFromSlot()
    {
        if (selectedSlot == -1)
        {
            Debug.Log("Pilih slot dulu sebelum load.");
            return;
        }

        var data = SaveSystem.Load(selectedSlot);
        if (data != null)
        {
            Debug.Log($"Loaded: {data.playerName} Lv {data.Level} Health: {data.Health}");

            // Apply loaded data to the player instance
            if (PlayerScript.Instance != null)
            {
                PlayerScript.Instance.Health = data.Health;
                PlayerScript.Instance.Heart = data.Heart;
                PlayerScript.Instance.Energy = data.Energy;
                PlayerScript.Instance.AtkPower = data.AtkPower;
                PlayerScript.Instance.DefPower = data.DefPower;
                PlayerScript.Instance.ExpGain = data.ExpGain;
                PlayerScript.Instance.Coin = data.Coin;
                PlayerScript.Instance.Level = data.Level;
                PlayerScript.Instance.Progress = data.Progress;

                // If you need to update UI or derived state, trigger them here
                // e.g. recalc Heart from Health if that's how your game works:
                // PlayerScript.Instance.Heart = PlayerScript.Instance.Health / 4;

                // Optionally refresh any UI or other systems
                Canvas.ForceUpdateCanvases();
                RefreshSlots();
            }
            else
            {
                Debug.LogWarning("PlayerScript.Instance is null — no player in scene to apply save.");
            }
        }
    }

    // Hapus data di slot
    public void DeleteSlot()
    {
        if (selectedSlot == -1)
        {
            Debug.Log("Pilih slot dulu sebelum menghapus.");
            return;
        }

        SaveSystem.Delete(selectedSlot);
        RefreshSlots();
        Canvas.ForceUpdateCanvases();
    }

    // Perbarui teks UI slot
    private void RefreshSlots()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            if (SaveSystem.HasSave(i))
            {
                slotTexts[i].text = $"Saved Data {i + 1}";
            }
            else
            {
                slotTexts[i].text = "No Data";
            }
        }
    }

    // public void OnClick_Save()
    // {
    //     panelSaveLoadUI.SetActive(true);
    //     panelMenuUI.SetActive(false);
    // }

    // public void OnClick_Load()
    // {

    // }

    // public void OnClick_SaveBack()
    // {
    //     panelSaveLoadUI.SetActive(false);
    //     panelMenuUI.SetActive(true);
    // }

    public void OnClick_BackMainMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // public void OnClick_Confirm_Yes()
    // {
    //     SaveToSlot();
    //     panelSaveLoadUI.SetActive(false);
    //     panelconfirmUI.SetActive(false);
    //     panelMenuUI.SetActive(true);
    // }
    
    // public void OnClick_Confirm_No()
    // {
    //     panelSaveLoadUI.SetActive(false);
    //     panelMenuUI.SetActive(true);
    // }
}
