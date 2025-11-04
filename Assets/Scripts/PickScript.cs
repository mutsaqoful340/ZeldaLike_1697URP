using UnityEngine;

public class PickScript : MonoBehaviour {
    public PlayerScript Player;
    public Animator Anim;
    public GameObject ItemDrop;
    public float Chance;

    public bool Crush {
        set {
            if (value) {
                if (Chance < 100) {
                    if (Random.Range(1, 10) <= (Chance / 10)) {
                        Instantiate(ItemDrop, transform.position, Quaternion.identity);
                    }
                } else {
                    Instantiate(ItemDrop, transform.position, Quaternion.identity);
                }
                Anim.SetTrigger("Crush");
                Destroy(gameObject, 1f);
            }
        }
    }

    private void PickThrow() {
        if (!Player.IsCarry) {
            Player.IsCarry = true;
            Player.Anim.SetFloat("Action", 0);
            Player.Anim.SetTrigger("DoAction");

            if (Chance < 100) {
                if (Random.Range(1, 10) <= (Chance / 10)) {
                    Instantiate(ItemDrop, transform.position, Quaternion.identity);
                }
            } else {
                Instantiate(ItemDrop, transform.position, Quaternion.identity);
            }

            transform.SetParent(Player.transform);
            transform.SetLocalPositionAndRotation(new Vector3(0, 0.6f, 0), Quaternion.identity);
            gameObject.layer = LayerMask.NameToLayer("Player");
            GetComponent<Renderer>().sortingOrder = 1;
            GetComponent<PickScript>().enabled = false;
        } else {
            Player.Anim.SetFloat("Action", 2f);
            Player.Anim.SetTrigger("DoAction");

            var bullet = GetComponent<BulletScript>();
            bullet.transform.SetParent(null);
            bullet.Body.bodyType = RigidbodyType2D.Dynamic;
            bullet.enabled = true;
            Player.IsCarry = false;
            Player.OnInteraction = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !Player.IsCarry) {
            Player.OnInteraction = PickThrow;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !Player.IsCarry) {
            Player.OnInteraction = null;
        }
    }
}
