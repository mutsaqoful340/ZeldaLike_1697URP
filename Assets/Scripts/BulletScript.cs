using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour {
    public Rigidbody2D Body;
    public Animator Anim;
    public float MaxDistance;
    public float Speed;
    //public bool IsEnemy;

    private Vector2 direction;
    private Vector3 spawnPos;

    private void Crush() {
        if (Anim != null) {
            Anim.SetTrigger("Crush");
        }
        Body.velocity = Vector2.zero;
        Destroy(gameObject, Anim != null ? 1f : 0);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        var layerIndex = collision.gameObject.layer;
        var layerName = LayerMask.LayerToName(layerIndex);
        if (!collision.collider.CompareTag("Player") && layerName != "Player") {
            Crush();
        }

    }

    private void Start() {
        Body = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        spawnPos = transform.position;

        direction = PlayerScript.Instance.MoveUpdate.normalized;
        if (direction == Vector2.zero) {
            direction = Vector2.right; 
        }
        Body.velocity = direction * Speed;
    }

    private void Update() {
        if (Vector3.Distance(transform.position, spawnPos) > MaxDistance) {
            Crush();
        }
    }
}