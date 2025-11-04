using UnityEngine;

public class CameraScript : MonoBehaviour {
    public Vector2 MinPos;
    public Vector2 MaxPos;
    public float Smoothing;

    private void LateUpdate() {
        var Target = PlayerScript.Instance.transform;
        if (transform.position != Target.position) {
            var targetpos = new Vector3(Target.position.x, Target.position.y, transform.position.z);
            targetpos.x = Mathf.Clamp(targetpos.x, MinPos.x, MaxPos.x);
            targetpos.y = Mathf.Clamp(targetpos.y, MinPos.y, MaxPos.y);
            transform.position = Vector3.Lerp(transform.position, targetpos, Smoothing);
        }
    }
}
