using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Windows;

public class InputUiScript : MonoBehaviour {
    public UnityAction<ActionState> OnAction { get; set; }

    private GameInput input;
    
    private void OnEnable() {
        input = new();
        input.Enable();

        input.GameUI.Slot1.performed += (e) => {
            OnAction?.Invoke(ActionState.Slot1);
        };

        input.GameUI.Slot2.performed += (e) => {
            OnAction?.Invoke(ActionState.Slot2);
        };

        input.GameUI.Slot3.performed += (e) => {
            OnAction?.Invoke(ActionState.Slot3);
        };

        input.GameUI.Slot4.performed += (e) => {
            OnAction?.Invoke(ActionState.Slot4);
        };

        input.GameUI.Exit.performed += (e) => {
            OnAction?.Invoke(ActionState.Status);
        };

        input.GameUI.Select.performed += (e) => {
            OnAction?.Invoke(ActionState.Interaction);
        };
    }

    private void OnDisable() {
        input.Disable();
    }
}
