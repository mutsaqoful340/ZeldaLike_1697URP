using System;
using UnityEngine.Events;
using UnityEngine;

public class ChestScript : MonoBehaviour {
    public Loot[] LootItems;
    public bool HasOpened;
    public Animator Anim;

    [Serializable]
    public struct Loot {
        public GameObject ItemDrop;
        public float Chance;
    }

    private Vector3 RandomPosition() {
        var x = UnityEngine.Random.Range(-1.3f, 1.3f);
        if (-0.3f < x && x < 0.3f) {
            x = 1f;
        }

        var y = UnityEngine.Random.Range(-1.3f, 1.3f);
        if (-0.3f < y && y < 0.3f) {
            y = 1f;
        }
        return new Vector3(x, y);
    }

    private void SpawnItem(GameObject lootOrigin) {
        var item = Instantiate(lootOrigin, transform);
        item.transform.SetLocalPositionAndRotation(RandomPosition(), Quaternion.identity);
    }

    private void ItemDropChecker() {
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName("AnimChestEmpty") && !HasOpened) {
            for (int i = 0; i < LootItems.Length; i++) {
                if (LootItems[i].Chance < 100) {
                    if (UnityEngine.Random.Range(1, 10) <= (LootItems[i].Chance / 10)) {
                        SpawnItem(LootItems[i].ItemDrop);
                    }
                } else {
                    SpawnItem(LootItems[i].ItemDrop);
                }
            }
            HasOpened = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player") && !HasOpened) {
            PlayerScript.Instance.OnInteraction = () => Anim.SetTrigger("DoOpen");
            PlayerScript.Instance.transform.Find("Notice").gameObject.SetActive(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            PlayerScript.Instance.OnInteraction = null;
            PlayerScript.Instance.transform.Find("Notice").gameObject.SetActive(false);
        }
    }

    private void LateUpdate() {
        ItemDropChecker();
    }
}