using UnityEngine.Events;
using UnityEngine;

public delegate string PickSlotAction(int slot);
public delegate bool RemoveEquipAction(string name);

public class PlayerScript : MonoBehaviour {
    public InputPlayScript Input;
    public Rigidbody2D Body;
    public Animator Anim;
    public Vector3 MoveUpdate;
    public float Speed;
    public float Delay;
    public string CastName;
    public bool IsCarry;
    public bool IsDashboard;
    public bool IsPaused;

    public int Health;
    public int Heart;
    public int Energy;
    public int AtkPower;
    public int DefPower;
    public int ExpGain;
    public int Coin;
    public int Level;
    public int Progress;

    public static PlayerScript Instance { get; private set; }
    public PickSlotAction OnEquipSlot { get; set; }
    public RemoveEquipAction OnEquipDismiss { get; set; }
    public UnityAction OnInteraction { get; set; }
    public UnityAction OnLevelUp { get; set; }
    public UnityAction OnSkillCast { get; set; }
    public UnityAction<bool> OnStatus { get; set; }
    public UnityAction<bool> OnPaused { get; set; }
      
    public int Damage {
        set {
            if (value > 0) {
                Health -= value - DefPower;
                if (Health <= 0) {
                    Death();
                }
            } else if (value < 0) {
                Health -= value;
                if (Health / 4 > Heart) {
                    Health = Heart * 4;
                }
            }
        }
    }

    private void Action(ActionState State) {
        switch (State) {
            case ActionState.Interaction:
                OnInteraction?.Invoke();
                break;
            case ActionState.Attack:
                Attack();
                break;
            case ActionState.Special:
                Special();
                break;
            case ActionState.Cast:
                OnSkillCast?.Invoke();
                break;
            case ActionState.Slot1:
                CastName = OnEquipSlot?.Invoke(1);
                OnSkillCast = () => Cast(CastName);
                break;
            case ActionState.Slot2:
                CastName = OnEquipSlot?.Invoke(2);
                OnSkillCast = () => Cast(CastName);
                break;
            case ActionState.Slot3:
                CastName = OnEquipSlot?.Invoke(3);
                OnSkillCast = () => Cast(CastName);
                break;
            case ActionState.Slot4:
                CastName = OnEquipSlot?.Invoke(4);
                OnSkillCast = () => Cast(CastName);
                break;
            case ActionState.Status:
                IsDashboard = true;
                OnStatus?.Invoke(IsDashboard);
                break;
            case ActionState.Pause:
                IsPaused = !IsPaused;
                Time.timeScale = IsPaused ? 0 : 1;
                OnPaused?.Invoke(IsPaused);
                break;
        }
    }

    private void Attack() {
        if (Delay == 0) {
            Delay = 0.5f;
            Anim.SetFloat("Action", 1f);
            Anim.SetTrigger("DoAction");

            var area = Physics2D.CircleCast(transform.position, 0.8f, MoveUpdate, 1.5f, LayerMask.GetMask("Enemy"));
            if (area.collider != null && (area.collider.CompareTag("Enemy") || area.collider.CompareTag("Item"))) {
                if (area.collider.CompareTag("Item")) {
                    area.collider.GetComponent<PickScript>().Crush = true;
                }
                ///HasLevelUp(1, Level); 
                //Damage = 1;
            }
        }
    }

    private void Special() {

    }

    private void Cast(string name) {
        if (Delay == 0 && (name != null && name != "")) {
            var origin = Resources.Load<GameObject>("Prefabs/Bullets/" + name);
            if (name == "LimeStone") {
                Anim.SetFloat("Action", 4f);
                Anim.SetTrigger("DoAction");
                var bullet = Instantiate(origin, transform.position, Quaternion.identity);
                bullet.GetComponent<BulletScript>().enabled = true;
                Delay = 1.5f;
            } else {
                try {
                    var buff = Instantiate(origin);
                    Anim.SetFloat("Action", 4f);
                    Anim.SetTrigger("DoAction");
                    if (name == "IronBall" || name == "MagicPouch") {
                        buff.transform.SetPositionAndRotation(transform.position + (MoveUpdate * 1.5f), Quaternion.identity);
                    } else if (name == "MixedHerb") {
                        buff.transform.SetParent(transform);
                        buff.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
                    }
                    Destroy(buff, 2f);
                    if (OnEquipDismiss?.Invoke(name) == false) {
                        OnSkillCast = null;
                    }
                } catch (System.Exception) {
                    GiveItem(name);
                    return;
                }
                Delay = 3f;
            }
        }
    }

    private void GiveItem(string name) {
        if (transform.Find("Notice").gameObject.activeInHierarchy) {
            OnEquipDismiss?.Invoke(name);
        }
    }

    private void Idle() {
        Anim.SetFloat("Move", IsCarry ? 2 : 0);
        Body.velocity = Vector3.zero;
    }

    private void Movement() {
        var isAction = Anim.GetCurrentAnimatorStateInfo(0).IsName("Action");
        isAction = isAction && Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0;

        MoveUpdate = Input.MoveHandler.normalized;
        Anim.SetFloat("Move", IsCarry ? 3 : 1);
        if (!isAction) {
            Anim.SetFloat("AxisX", MoveUpdate.x);
            Anim.SetFloat("AxisY", MoveUpdate.y);
        }
        Body.velocity = isAction ? Vector3.zero : Input.MoveHandler.normalized * Speed;
    }

    private bool HasLevelUp(int exp, int level) {
        if (level < 25) {
            ExpGain += exp;
            CountLevel(ExpGain);
            if (Level > level) {
                OnLevelUp?.Invoke();
                Anim.SetFloat("Action", 3f);
                Anim.SetTrigger("DoAction");
                var light = Instantiate(Resources.Load<GameObject>("Prefabs/LightOn"), transform);
                light.transform.SetLocalPositionAndRotation(new Vector3(0, -0.25f, 0), Quaternion.identity);
                Destroy(light, 1.05f);
                return true;
            }
        }
        return false;
    }

    private void CountLevel(int exp) {
        if (exp <= 0) {
            exp = 1;
        }
        Level = (int)Mathf.Pow(exp, 0.5f);
        var prev = Mathf.Pow(Level, 2);
        var next = Mathf.Pow(Level + 1, 2);
        Progress = (int)Mathf.Round((exp - prev) / (next - prev) * 10);
    }

    private void Death() {
        var origin = Resources.Load<GameObject>("Prefabs/Tombstone");
        Body.constraints = RigidbodyConstraints2D.FreezeAll;
        Instantiate(origin, transform.position, Quaternion.identity);
        gameObject.SetActive(Input.enabled = false);
    }

    private void Start() {
        Instance = this;
        Input.OnAction = Action;
        CountLevel(ExpGain);
        Heart = Health / 4;
    }

    private void Update() {
        var state = Input.MoveHandler != Vector3.zero;
        if (!state) {
            Idle();
        } else {
            Movement();
        }

        if (Delay > 0) {
            Delay -= Time.deltaTime;
        }

        if (Delay < 0) {
            Delay = 0;
        }
    }
}
