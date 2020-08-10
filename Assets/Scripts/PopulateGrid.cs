using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PopulateGrid : MonoBehaviour
{
    public GameObject roomPrefab;
    public static PopulateGrid instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    public void ListRooms()
    {
        NetworkManager.instance.GetListOfRooms();
    }

    public void PopulateRoomList(List<RoomInfo> roomList)
    {

        GameObject roomAdded;
        TextMeshProUGUI roomText;

        foreach (RoomInfo room in roomList)
        {
            roomAdded = Instantiate(roomPrefab, transform);
            roomText = roomAdded.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            roomText.text = room.Name;
        }
    }
}
