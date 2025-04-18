using TMPro;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageTxt;


    public void ReceiveMessage(string _msg)
    {
        messageTxt.text = $"{_msg}";
    }
}