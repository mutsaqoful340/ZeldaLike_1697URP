using UnityEngine.Events;
using UnityEngine;

public class InputPlayScript : MonoBehaviour {
    public UnityAction<ActionState> OnAction { get; set; }

    public Vector3 MoveHandler {
        get {
            var axisX = input.GamePlay.Movement.ReadValue<Vector2>().x;
            var axisY = input.GamePlay.Movement.ReadValue<Vector2>().y;
            return new Vector3(axisX, axisY);
        }
    }

    private GameInput input;

    private void OnEnable() {
        input = new();
        input.Enable();

        input.GamePlay.Interaction.performed += (e) => {
            OnAction?.Invoke(ActionState.Interaction);
        };

        input.GamePlay.Attack.performed += (e) => {
            OnAction?.Invoke(ActionState.Attack);
        };

        input.GamePlay.Special.performed += (e) => {
            OnAction?.Invoke(ActionState.Special);
        };

        input.GamePlay.Cast.performed += (e) => {
            OnAction?.Invoke(ActionState.Cast);
        };

        input.GamePlay.Slot1.performed += (e) => {
            OnAction?.Invoke(ActionState.Slot1);
        };

        input.GamePlay.Slot2.performed += (e) => {
            OnAction?.Invoke(ActionState.Slot2);
        };

        input.GamePlay.Slot3.performed += (e) => {
            OnAction?.Invoke(ActionState.Slot3);
        };

        input.GamePlay.Slot4.performed += (e) => {
            OnAction?.Invoke(ActionState.Slot4);
        };

        input.GamePlay.Pause.performed += (e) => {
            OnAction?.Invoke(ActionState.Pause);
        };

        input.GamePlay.Status.performed += (e) => {
            OnAction?.Invoke(ActionState.Status);
        };
    }

    private void OnDisable() {
        input.Disable();
    }
}

public enum ActionState {
    Interaction, Attack, 
    Special, Cast,
    Slot1, Slot2, Slot3, Slot4,
    Status, Pause,
}