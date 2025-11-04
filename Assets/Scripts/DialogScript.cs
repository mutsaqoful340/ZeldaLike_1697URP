using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogScript : MonoBehaviour {
    public GameObject FormX;
    public GameObject FormY;
    public TextMeshProUGUI DialogX;
    public TextMeshProUGUI DialogY;
    public List<ChatState> Sentences;
    public List<Sprite> DialogDir;
    public bool IsWriting;
    public int Index = 0;

    [Serializable]
    public struct ChatState {
        public bool Switch;
        public string Message;
    }

    private IEnumerator WriteSentence(TextMeshProUGUI Dialog) {
        foreach (char text in Sentences[Index].Message.ToCharArray()) {
            if (Dialog.text != Sentences[Index].Message) {
                Dialog.text += text;
                IsWriting = true;
                yield return new WaitForSeconds(0.025f);
            } else {
                break;
            }
        }
        IsWriting = false;
        Index++;
    }

    private void NextSentence() {
        DialogX.text = "";
        DialogY.text = "";
        transform.Find("Notice").gameObject.SetActive(false);
        PlayerScript.Instance.transform.Find("Notice").gameObject.SetActive(false);
        try {
            var Dialog = Sentences[Index].Switch ? DialogX : DialogY;
            if (IsWriting) {
                Dialog.text = Sentences[Index].Message;
                IsWriting = false;
            } else {
                FormX.SetActive(Sentences[Index].Switch);
                FormY.SetActive(!Sentences[Index].Switch);
                StartCoroutine(WriteSentence(Dialog));
            }
        } catch (Exception) {
            FormX.SetActive(false);
            FormY.SetActive(false);
            PlayerScript.Instance.OnInteraction = null;
            Index = 0;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !PlayerScript.Instance.IsCarry) {
            var n = PlayerScript.Instance.MoveUpdate.x;
            if (n < 0) {
                FormX.GetComponent<SpriteRenderer>().sprite = DialogDir[0];
                n = -1f;
            } else if (n > 0) {
                FormX.GetComponent<SpriteRenderer>().sprite = DialogDir[1];
                n = 1f;
            } else {
                FormX.GetComponent<SpriteRenderer>().sprite = DialogDir[2];
                n = 0f;
            }
            FormX.transform.SetLocalPositionAndRotation(new Vector3(n, 0.8f, 0), Quaternion.identity);
            PlayerScript.Instance.transform.Find("Notice").gameObject.SetActive(true);
            PlayerScript.Instance.OnInteraction = NextSentence;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !PlayerScript.Instance.IsCarry) {
            PlayerScript.Instance.transform.Find("Notice").gameObject.SetActive(false);
            PlayerScript.Instance.OnInteraction = null;
            FormX.SetActive(false);
            FormY.SetActive(false);
            Index = 0;
        }
    }
}
