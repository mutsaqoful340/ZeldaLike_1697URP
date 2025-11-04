using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class GuiScript : MonoBehaviour {
    public PlayerScript Player;
    public InputUiScript Input;
    public List<Item> Inventory;
    public Animator Anim;
    public Button ButtonFirst;
    public Transform PanelHealth;
    public Transform PanelEnergy;
    public Transform PanelQuick;
    public Transform PanelCoin;
    public Transform PanelLevel;
    public Transform PanelExp;
    public Transform ItemContent;
    public Image ItemCast;
    public Sprite[] Hearts = new Sprite[5];
    public Sprite[] Numbers = new Sprite[10];
    public int HeartCount;
    public int SlotSelected;

    [Serializable]
    public struct Item {
        public string Name;
        public float Slot;
        public Sprite Icon;
    }

    public Transform UpdateInventory {
        get {
            return ItemContent;
        }
        set {
            var item = new Item {
                Name = value.name.Replace("Slot_", ""),
                Icon = value.GetChild(0).GetComponent<Image>().sprite,
                Slot = 0,
            };
            Inventory.Add(item);
            for (int i = 0; i < ItemContent.childCount; i++) {
                if (value.name == ItemContent.GetChild(i).name) {
                    var text = ItemContent.GetChild(i).Find("Text");
                    var qty = Convert.ToInt32(text.GetComponent<TextMeshProUGUI>().text.Substring(1));
                    text.GetComponent<TextMeshProUGUI>().text = $"x{++qty}";
                    Destroy(value.gameObject);
                    return;
                }
            }
            value.SetParent(ItemContent);
            value.GetComponent<Button>().onClick.AddListener(ButtonItem_Handler);
            value.gameObject.SetActive(true);
        }
    }

    public bool UpdateCameraUI {
        get {
            var cameraPos = Camera.main.transform.position;
            var newpos = new Vector3(0, 0, cameraPos.z < 0 ? 10 : -10);
            Camera.main.transform.position = cameraPos + newpos;
            return true;
        }
    }

    public int ButtonQuick_Handler {
        set {
            var name = ButtonFirst.name.Replace("Slot_", "");
            for (int i = 0; i < Inventory.Count; i++) {
                if (Inventory[i].Slot == value) {
                    Inventory[i] = new Item { Name = Inventory[i].Name, Icon = Inventory[i].Icon, Slot = 0, };
                }

                if (name == Inventory[i].Name) {
                    Inventory[i] = new Item { Name = Inventory[i].Name, Icon = Inventory[i].Icon, Slot = value, };
                }
            }
            Player.OnEquipSlot = EquipCastSlot;
            RefreshQuickSlot();
        }
    }

    private const int N_HEART = 4;

    private string EquipCastSlot(int slot) {
        foreach (var item in Inventory) {
            if (item.Slot == slot) {
                ItemCast.sprite = PanelQuick.GetChild(slot - 1).GetChild(0).GetComponent<Image>().sprite;
                ItemCast.enabled = true;
                return item.Name;
            } else {
                ItemCast.enabled = false;
            }
        }
        return string.Empty;
    }

    private void ButtonItem_Handler() {
        var selected = EventSystem.current.currentSelectedGameObject;
        ButtonFirst = selected.GetComponent<Button>();
    }

    private void RefreshQuickSlot() {
        for (int i = 0; i < 4; i++) {
            PanelQuick.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
            PanelQuick.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;
            //ItemCast.sprite = null;
            //ItemCast.enabled = false;
        }
        
        for (int i = 0; i < Inventory.Count; i++) {
            if (Inventory[i].Slot == 1) {
                PanelQuick.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Inventory[i].Icon;
                PanelQuick.GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;
            } else if (Inventory[i].Slot == 2) {
                PanelQuick.GetChild(1).GetChild(0).GetComponent<Image>().sprite = Inventory[i].Icon;
                PanelQuick.GetChild(1).GetChild(0).GetComponent<Image>().enabled = true;
            } else if (Inventory[i].Slot == 3) {
                PanelQuick.GetChild(2).GetChild(0).GetComponent<Image>().sprite = Inventory[i].Icon;
                PanelQuick.GetChild(2).GetChild(0).GetComponent<Image>().enabled = true;
            } else if (Inventory[i].Slot == 4) {
                PanelQuick.GetChild(3).GetChild(0).GetComponent<Image>().sprite = Inventory[i].Icon;
                PanelQuick.GetChild(3).GetChild(0).GetComponent<Image>().enabled = true;
            }
        }
    }

    private void SwitchUIView_Handler(bool state) {
        Anim.SetBool("IsSliding", Input.enabled = state);
        Player.Input.enabled = !state;
        if (ButtonFirst == null) {
            ButtonFirst = PanelQuick.GetChild(0).GetComponent<Button>();
        }
        ButtonFirst.Select();
    }

    private bool EquipDismiss_Handler(string name) {
        var state = true;
        foreach (var item in Inventory) {
            if (item.Name == name) {
                for (int i = 0; i < ItemContent.childCount; i++) {
                    if (ItemContent.GetChild(i).name.Replace("Slot_", "") == name) {
                        var text = ItemContent.GetChild(i).Find("Text");
                        var qty = Convert.ToInt32(text.GetComponent<TextMeshProUGUI>().text.Substring(1));
                        if (--qty <= 0) {
                            ItemCast.sprite = null;
                            ItemCast.enabled = false;
                            Destroy(ItemContent.GetChild(i).gameObject);
                            state = false;
                        } else {
                            text.GetComponent<TextMeshProUGUI>().text = $"x{qty}";
                        }
                    }
                }
                Inventory.Remove(item);
                RefreshQuickSlot();
                break;
            }
        }
        return state;
    }

    private void Action(ActionState State) {
        switch (State) {
            case ActionState.Slot1:
                ButtonItem_Handler();
                ButtonQuick_Handler = 1;
                break;
            case ActionState.Slot2:
                ButtonItem_Handler();
                ButtonQuick_Handler = 2;
                break;
            case ActionState.Slot3:
                ButtonItem_Handler();
                ButtonQuick_Handler = 3;
                break;
            case ActionState.Slot4:
                ButtonItem_Handler();
                ButtonQuick_Handler = 4;
                break;
            case ActionState.Status:
                Player.IsDashboard = !Player.IsDashboard;
                SwitchUIView_Handler(false);
                break;
            case ActionState.Interaction:
                ButtonFirst.Select();
                break;
        }
    }

    private void UpdateUILevel() {
        if (Player.Level > 10) {
            var num1 = Player.Level.ToString().Substring(0, 1);
            var num2 = Player.Level.ToString().Substring(1, 1);
            for (int i = 0; i < Numbers.Length; i++) {
                if (i == Convert.ToSByte(num1)) {
                    PanelLevel.Find("ImgLevel (1)").GetComponent<Image>().sprite = Numbers[i];
                }

                if (i == Convert.ToSByte(num2)) {
                    PanelLevel.Find("ImgLevel (2)").GetComponent<Image>().sprite = Numbers[i];
                }
            }
            PanelLevel.Find("ImgLevel (1)").gameObject.SetActive(true);
            PanelLevel.Find("ImgLevel (2)").gameObject.SetActive(true);
        } else {
            for (int i = 0; i < Numbers.Length; i++) {
                if (i == Player.Level) {
                    PanelLevel.Find("ImgLevel (1)").GetComponent<Image>().sprite = Numbers[i];
                }
            }
            PanelLevel.Find("ImgLevel (1)").gameObject.SetActive(true);
            PanelLevel.Find("ImgLevel (2)").gameObject.SetActive(false);
        }
        for (int i = 0; i < 10; i++) {
            PanelExp.Find($"Image ({i})").gameObject.SetActive(false);
        }
    }

    private void MonitorLevel() {
        for (int i = 0; i < Player.Progress + 1; i++) {
            PanelExp.Find($"Image ({i})").gameObject.SetActive(true);
        }
    }

    private void MonitorHealth() {
        if (Player.Health > 0) {
            for (int i = 0; i < HeartCount; i++) {
                if (!PanelHealth.Find($"ImgHeart ({i})").gameObject.activeInHierarchy) {
                    PanelHealth.Find($"ImgHeart ({i})").gameObject.SetActive(true);
                }
                PanelHealth.Find($"ImgHeart ({i})").GetComponent<Image>().sprite = Hearts[N_HEART];
            }
            var heartCount = Player.Health % N_HEART;
            var heartActive = heartCount == 0 ? Player.Health / N_HEART : (Player.Health / N_HEART) + 1;
            for (int i = 0; i < heartActive; i++) {
                PanelHealth.Find($"ImgHeart ({i})").GetComponent<Image>().sprite = Hearts[0];
            }
            var n = N_HEART - heartCount;
            n = n == N_HEART ? 0 : n;
            PanelHealth.Find($"ImgHeart ({heartActive - 1})").GetComponent<Image>().sprite = Hearts[n];
        } else {
            PanelHealth.Find($"ImgHeart (0)").GetComponent<Image>().sprite = Hearts[N_HEART];
        }
    }

    private void MonitorEnergy() {
        for (int i = 0; i < Player.Energy; i++) {
            PanelEnergy.Find($"ImgSoul ({i})").gameObject.SetActive(true);
        }
    }

    private void MonitorCoin() {
        var strDigit = Player.Coin.ToString();
        for (int i = 0; i < 4; i++) {
            if (i <= strDigit.Length - 1) {
                var digit = strDigit.Substring(i, 1);
                PanelCoin.Find($"ImgCoin ({i})").gameObject.SetActive(true);
                PanelCoin.Find($"ImgCoin ({i})").GetComponent<Image>().sprite = Numbers[Convert.ToInt32(digit)];
            } else {
                PanelCoin.Find($"ImgCoin ({i})").gameObject.SetActive(false);
            }
        }
    }

    private void Start() {
        Anim = GetComponent<Animator>();
        Player.OnStatus = SwitchUIView_Handler;
        Player.OnEquipDismiss = EquipDismiss_Handler;
        Input.OnAction = Action;
    }

    private void LateUpdate() {
        if (!PanelLevel.Find("ImgLevel (1)").gameObject.activeInHierarchy) {
            Player.OnLevelUp = UpdateUILevel;
            HeartCount = Player.Health / N_HEART;
            UpdateUILevel();
            MonitorEnergy();
        }
        MonitorHealth();
        MonitorLevel();
        MonitorCoin();
    }
}