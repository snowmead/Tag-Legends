using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoinRoomFromListHandler : MonoBehaviour
{
    public TextMeshProUGUI roomName;

    public void JoinRoomFromList()
    {
        NetworkManager.instance.JoinRoom(roomName.text);
    }
}
