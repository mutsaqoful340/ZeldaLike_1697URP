using UnityEngine;

public class ItemScript : MonoBehaviour {
    private void PickUp() {
        if (transform.name.Replace("(Clone)", "") == "Coin") {
            PlayerScript.Instance.Coin++;
            if (PlayerScript.Instance.Coin >= 9999) {
                PlayerScript.Instance.Coin = 9999;
            }
        } else if (transform.name.Replace("(Clone)", "") == "Heart(Clone)") {
            PlayerScript.Instance.Damage = -4;
        } else {
            var inventory = GameObject.FindWithTag("HUD").GetComponent<GuiScript>();
            inventory.UpdateInventory = transform.GetChild(0);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !PlayerScript.Instance.IsCarry) {
            if (transform.name != "Coin(Clone)" && transform.name != "Heart(Clone)") {
                PlayerScript.Instance.transform.Find("Notice").gameObject.SetActive(true);
            }
            PlayerScript.Instance.OnInteraction = PickUp;
        }    
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !PlayerScript.Instance.IsCarry) {
            PlayerScript.Instance.transform.Find("Notice").gameObject.SetActive(false);
            PlayerScript.Instance.OnInteraction = null;
        }   
    }
}
