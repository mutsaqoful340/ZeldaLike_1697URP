using TMPro;
using UnityEngine;

public class SaveLoadUI : MonoBehaviour
{
    public TextMeshProUGUI[] slotTexts = new TextMeshProUGUI[3];
    private int selectedSlot = -1;

    private void Start()
    {
        RefreshSlots();
    }

    // Dipanggil saat tombol slot ditekan
    public void SelectSlot(int index)
    {
        selectedSlot = index;
        Debug.Log($"Slot {index + 1} dipilih");
    }

    // Simpan ke slot terpilih
    public void SaveToSlot()
    {
        if (selectedSlot == -1)
        {
            Debug.Log("Pilih slot dulu sebelum menyimpan.");
            return;
        }

        PlayerData data = new PlayerData
        {
            playerName = "Hero",
            Health = Random.Range(5, 20),
            Heart = Random.Range(5, 20),
            Energy = Random.Range(5, 20),
            AtkPower = Random.Range(5, 20),
            DefPower = Random.Range(5, 20),
            ExpGain = Random.Range(5, 20),
            Coin = Random.Range(5, 20),
            Level = Random.Range(5, 20),
            Progress = Random.Range(5, 20),
        };

        SaveSystem.Save(data, selectedSlot);
        RefreshSlots();
        Canvas.ForceUpdateCanvases();
    }

    // Load data dari slot
    public void LoadFromSlot()
    {
        if (selectedSlot == -1)
        {
            Debug.Log("Pilih slot dulu sebelum load.");
            return;
        }

        var data = SaveSystem.Load(selectedSlot);
        if (data != null)
        {
            //Debug.Log($"Loaded: {data.playerName} Lv {data.level}, Exp {data.exp}");
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
}
