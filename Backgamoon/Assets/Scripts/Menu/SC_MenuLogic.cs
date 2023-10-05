using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_MenuLogic : MonoBehaviour
{
    #region Appwarp keys
    private string apiKey = "f42bdc57492ecc042a328453971bc5d84c10869bf6b2685f283057d27837c787";
    private string secretKey = "1a288d908544b255ff10f5ca9c257319662b816f1319a7cae57b9dc293c257a1";

    #endregion
    private Listener listner;
    private string userId=string.Empty;
    public Dictionary<string, GameObject> unityObjects;
    private Dictionary<string, object> passedParams;
    private List<string> roomIds;
    private int maxRoomUsers = 2;
    private int roomIndex;
    public int TurnTime = 20;
    string roomId;
    int connectionTrys = 0;
    int createRoomTrys = 0;
    int joinRoomTrys = 0;

    #region Singleton

    private static SC_MenuLogic instance;
    public static SC_MenuLogic Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_MenuLogic").GetComponent<SC_MenuLogic>();

            return instance;
        }

    }

    #endregion
    #region MonoBehaviour


    void Awake()
    {
        InitAwake();
    }
    void Start()
    {
        InitStart();
    }

    private void InitStart()
    {
        unityObjects["Btn_Play"].GetComponent<Button>().interactable = true;
        StartCoroutine(CR_SetGameScreenInactive());
    }

    private void OnEnable()
    {
        Listener.OnConnect += OnConnect;
        Listener.OnRoomsInRange += OnRoomsInRange;
        Listener.OnCreateRoom += OnCreateRoom;
        Listener.OnJoinRoom += OnJoinRoom;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        Listener.OnUserJoinRoom += OnUserJoinRoom;
        Listener.OnGameStarted += OnGameStarted;
    }


    private void OnDisable()
    {
        Listener.OnConnect -= OnConnect;
        Listener.OnRoomsInRange -= OnRoomsInRange;
        Listener.OnCreateRoom -= OnCreateRoom;
        Listener.OnJoinRoom -= OnJoinRoom;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        Listener.OnUserJoinRoom -= OnUserJoinRoom;
        Listener.OnGameStarted -= OnGameStarted;


    }



    #endregion

    #region Logic

    IEnumerator CR_SetGameScreenInactive()
    {
        yield return new WaitForSeconds(1f);

        unityObjects["Screen_Game"].SetActive(false);
    }
    void JoinRoom()
    {
        joinRoomTrys++;
        WarpClient.GetInstance().JoinRoom(roomId);
        WarpClient.GetInstance().SubscribeRoom(roomId);
    }
    private void SearchRooms()
    {
        if (roomIndex < roomIds.Count)
        {
            UpdateStatus("Searching Room " + roomIds[roomIndex] + " Info");
            WarpClient.GetInstance().GetLiveRoomInfo(roomIds[roomIndex]);
        }
        else
        {
            CreateRoom();
        }
    }

    private void CreateRoom()
    {
        createRoomTrys++;
        UpdateStatus("Creating Room...");
        WarpClient.GetInstance().CreateTurnRoom(
                        "Backgamoon" + (roomIds.Count + 1),
                        userId,
                        maxRoomUsers,
                        passedParams,
                        TurnTime
                        );

    }
    #endregion
    private void InitAwake()
    {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _obj = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _obj)
            unityObjects.Add(g.name, g);
        passedParams = new Dictionary<string, object>()
        {{"Password","Shenkar2023"}};


        if (listner == null)
            listner = new Listener();

        WarpClient.initialize(apiKey, secretKey);
        WarpClient.GetInstance().AddConnectionRequestListener(listner);
        WarpClient.GetInstance().AddChatRequestListener(listner);
        WarpClient.GetInstance().AddUpdateRequestListener(listner);
        WarpClient.GetInstance().AddLobbyRequestListener(listner);
        WarpClient.GetInstance().AddNotificationListener(listner);
        WarpClient.GetInstance().AddRoomRequestListener(listner);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listner);
        WarpClient.GetInstance().AddZoneRequestListener(listner);

        userId = System.Guid.NewGuid().ToString();
        unityObjects["Txt_UserId"].GetComponent<TextMeshProUGUI>().text = "UserId: " + userId;
        connectServer();

    }

    void connectServer()
    {
        connectionTrys++;
        WarpClient.GetInstance().Connect(userId);
        UpdateStatus("Try To Connect Server...");
    }
    private void UpdateStatus(string _Str)
    {
        unityObjects["Txt_Status"].GetComponent<TextMeshProUGUI>().text = _Str;
    }

    #region ServerCallbacks
    private void OnConnect(bool _IsSuccess)
    {
        Debug.Log("OnConnect " + _IsSuccess);
        if (_IsSuccess)
        {
            UpdateStatus("Connected.");
            unityObjects["Btn_Play"].GetComponent<Button>().interactable = true;
        }
        else
        {
            UpdateStatus("Failed to Connect.");
            if (connectionTrys < 5)
                connectServer();
            else
                Application.Quit();
        }
    }

    private void OnRoomsInRange(bool _IsSuccess, MatchedRoomsEvent eventObj)
    {
        Debug.Log("OnRoomsInRange " + _IsSuccess);
        if (_IsSuccess)
        {
            UpdateStatus("Getting Rooms...");
            roomIds = new List<string>();
            foreach (var RoomData in eventObj.getRoomsData())
            {
                Debug.Log("Room Id: " + RoomData.getId());
                Debug.Log("Room Owner: " + RoomData.getRoomOwner());
                roomIds.Add(RoomData.getId());
            }
                roomIndex = 0;
                SearchRooms();
        }
    }


    private void OnJoinRoom(bool _IsSuccess, string _RoomId)
    {
        if(_IsSuccess)
        {
            UpdateStatus("Joined Room " + roomId);
            unityObjects["Txt_RoomId"].GetComponent<TextMeshProUGUI>().text = "Room ID: "+ _RoomId;
        }
        else
        {

            UpdateStatus("Failed to Join Room.");
            if (joinRoomTrys < 5)
                JoinRoom();
            else
                Application.Quit();
        }
    }

    private void OnUserJoinRoom(RoomData eventObj, string _UserName)
    {
        if (eventObj.getRoomOwner() == userId && userId != _UserName)
        {
            UpdateStatus("User  " + _UserName + " Joined room");
            UpdateStatus("Starting game...");
            WarpClient.GetInstance().startGame();
        }
    }

    private void OnCreateRoom(bool _IsSuccess, string _RoomId)
    {
        Debug.Log("OnCreateRoom " + _IsSuccess + " " + _RoomId);
        if (_IsSuccess)
        {
            roomId = _RoomId;
            UpdateStatus("Room created! Room Id= " + roomId);
            JoinRoom();
        }
        else
        {
            UpdateStatus("Failed to Create Room.");
            if (createRoomTrys < 5)
                CreateRoom();
            else
                Application.Quit();
        }
    }

    private void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
    {
        Debug.Log("OnGetLiveRoomInfo ");
        UpdateStatus("OnGetLiveRoomInfo Recieved room data");
        Dictionary<string, object> room_prop = eventObj.getProperties();
        string curr_pass = passedParams["Password"].ToString();
        if (eventObj != null && room_prop != null
            && (room_prop.ContainsKey("Password") && curr_pass == room_prop["Password"].ToString()))
        {
            roomId = eventObj.getData().getId();
            UpdateStatus("Found Room! joining room: "+ roomId);
            JoinRoom();
        }
        else
        {
            roomIndex++;
            SearchRooms();
        }
    }

    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        UpdateStatus("Game Started! Turn: "+ _NextTurn);
        unityObjects["Screen_Menu"].SetActive(false);
        unityObjects["Screen_Game"].SetActive(true);


    }

    #endregion

    #region Controller

    public void Btn_PlayLogic()
    {
        Debug.Log("Btn_PlayLogic");
        unityObjects["Btn_Play"].GetComponent<Button>().interactable = false;
        WarpClient.GetInstance().GetRoomsInRange(1, 2);
        UpdateStatus("Seraching for available rooms...");
        
    }

    #endregion
}
