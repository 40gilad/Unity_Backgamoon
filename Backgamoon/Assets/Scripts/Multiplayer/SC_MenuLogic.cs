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
    public Dictionary<string, GameObject> unityObjects;
    private Dictionary<string, object> passedParams;
    private List<string> roomIds;
    private int maxRoomUsers = 2;
    private int roomIndex;
    string roomId;
    int connectionTrys = 0;
    int createRoomTrys = 0;
    int joinRoomTrys = 0;
    int multiplayer_volume=0;

    public GameObject Curr_Screen;
    private Dictionary<string, GameObject> Screen;
    Stack<GameObject> ScreenStack;
    SC_Board board;


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
        //InitAwake();
    }
    
    void Start()
    {
        InitStart();
    }
    

    private void InitStart()
    {
        StartCoroutine(CR_InitAwake());
        //unityObjects["Btn_Play"].GetComponent<Button>().interactable = true;
        //StartCoroutine(CR_SetGameScreenInactive());
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

    #region Server Connection Logic

    IEnumerator CR_SetGameScreenInactive()
    {
        yield return new WaitForSeconds(1f);

        unityObjects["Screen_Game"].SetActive(false);
        Debug.Log("Screen_Game is off");
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
        Debug.Log("Passord will be: Shenkar202" + multiplayer_volume.ToString());
        passedParams["Password"] = "Shenkar202" + multiplayer_volume.ToString();
        createRoomTrys++;
        UpdateStatus("Creating Room...");
        WarpClient.GetInstance().CreateTurnRoom(
                        "Backgamoon" + (roomIds.Count + 1),
                        GlobalVars.userId,
                        maxRoomUsers,
                        passedParams,
                        GlobalVars.TurnTime
                        );

    }
    #endregion
    private IEnumerator CR_InitAwake()
    {
        board=GameObject.Find("Board").GetComponent<SC_Board>();
        yield return StartCoroutine(init_objects_dict());
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

        GlobalVars.userId = System.Guid.NewGuid().ToString();
        unityObjects["Txt_UserId"].GetComponent<TextMeshProUGUI>().text = "UserId: " + GlobalVars.userId;
        Screen["Screen_Game"].SetActive(false);
        unityObjects["Canvas_Multiplayer"].SetActive(false);
        //connectServer();

    }

    private IEnumerator init_objects_dict()
    {
        Debug.Log("init_objects_dict");
        yield return new WaitForSeconds(1f);
        ScreenStack = new Stack<GameObject>();
        unityObjects = new Dictionary<string, GameObject>();
        Screen = new Dictionary<string, GameObject>();
        GameObject[] _obj = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _obj)
            unityObjects.Add(g.name, g);
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Screen"))
        {
            if (g.name != "Screen_MainMenu")
                g.SetActive(false);
            if (Screen.ContainsKey(g.name) == false)
                Screen.Add(g.name, g);
            else Debug.LogError("Key name " + g.name + "is already exist in dictionary");
        }
    }
    void connectServer()
    {
        connectionTrys++;
        WarpClient.GetInstance().Connect(GlobalVars.userId);
        UpdateStatus("Try To Connect Server...");
    }
    private void UpdateStatus(string _Str)
    {
        unityObjects["Txt_Status"].GetComponent<TextMeshProUGUI>().text = _Str;
    }

    #region ServerCallbacks
    private void OnConnect(bool _IsSuccess)
    {
        Debug.Log("try to connect " + connectionTrys);
        Debug.Log("OnConnect " + _IsSuccess);
        if (_IsSuccess)
        {
            UpdateStatus("Connected.");
            unityObjects["Btn_PlayMulti"].GetComponent<Button>().interactable = true;
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
        GlobalVars.orange=eventObj.getRoomOwner();
        if (eventObj.getRoomOwner() == GlobalVars.userId && GlobalVars.userId != _UserName)
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
        GlobalVars.orange=eventObj.getData().getRoomOwner();
        Debug.Log("OnGetLiveRoomInfo , room owner= "+ GlobalVars.orange);
        UpdateStatus("OnGetLiveRoomInfo Recieved room data");
        Dictionary<string, object> room_prop = eventObj.getProperties();
        passedParams["Password"] = "Shenkar202" + multiplayer_volume.ToString();
        string curr_pass = passedParams["Password"].ToString();
        Debug.Log("curr_pass= " + curr_pass);
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
        Debug.Log("SC_MenuLogic: OnGameStarted Orange= " + GlobalVars.orange + " MyId= " + GlobalVars.userId);
        UpdateStatus("Game Started! Turn: "+ _NextTurn);
        unityObjects["Screen_Multiplayer"].SetActive(false);
        Screen["Screen_Game"].SetActive(true);
        Debug.Log("Screen Game on");
        unityObjects["Board"].GetComponent<SC_Board>().StartGame(_NextTurn);
    }

    #endregion

    #region Controller

    public void Btn_Logic(string Screen_Name)
    {
        Debug.Log("Curr_Screen=(" + Curr_Screen.name + ")");
        Debug.Log("Btn_Logic(" + Screen_Name + ")");
        ScreenStack.Push(Curr_Screen);
        Curr_Screen.SetActive(false);
        if (Screen_Name != "Screen_Singleplayer" 
            && Screen_Name!= "MutiPlay"
            && Screen_Name!= "PsudoPlay")
        {
            Curr_Screen = Screen[Screen_Name];
            Curr_Screen.SetActive(true);
        }
        else
        {
            unityObjects["Img_LoadingBack"].SetActive(false);
            switch (Screen_Name)
            {
                case ("Screen_Singleplayer"):
                    Curr_Screen = Screen["Screen_Game"];
                    Curr_Screen.SetActive(true);
                    board.psudo_multiplayer = false;
                    board.multiplayer = false;
                    board.rotate_camera();
                    break;
                    
                case ("MutiPlay"):
                    Curr_Screen = unityObjects["Canvas_Multiplayer"];
                    Curr_Screen.SetActive(true);
                    board.psudo_multiplayer = false;
                    board.multiplayer = true;
                    connectServer();
                    break;
                case ("PsudoPlay"):
                    Curr_Screen = Screen["Screen_Game"];
                    Curr_Screen.SetActive(true);
                    board.psudo_multiplayer = true;
                    board.multiplayer = false;
                    break;
                default:
                    Curr_Screen = Screen["Screen_MainMenu"];
                    Curr_Screen.SetActive(true);
                    break;

            }
        }
    }

    public void Exit_Logic()
    {
        Application.Quit();
    }
    public void Btn_BackLogic()
    {
        Debug.Log("Btn_BackLogic");
        Debug.Log("Curr_Screen=(" + Curr_Screen.name + ")");
        Curr_Screen.SetActive(false);
        Curr_Screen = ScreenStack.Pop();
        Debug.Log("Prev_Screen=(" + Curr_Screen.name + ")");
        Curr_Screen.SetActive(true);
    }

    public void VolumeLogic()
    {
        GameObject Vol = GameObject.Find("Volume");
        GameObject Vol_Val_Slider = GameObject.Find("Txt_Vol_Val");
        multiplayer_volume = int.Parse(Vol.GetComponent<Slider>().value.ToString());
        Vol_Val_Slider.GetComponent<TextMeshProUGUI>().text = Vol.GetComponent<Slider>().value.ToString();
    }

    public void Music_VolumeLogic()
    {
        GameObject Vol = GameObject.Find("Volume_Music");
        GameObject Vol_Val_Slider = GameObject.Find("Txt_Music_Val");
        GameObject Fishbone_vol = GameObject.Find("Fishbone");
        float vol_val = Vol.GetComponent<Slider>().value;
        Fishbone_vol.GetComponent<AudioSource>().volume = vol_val;
        Vol_Val_Slider.GetComponent<TextMeshProUGUI>().text = vol_val.ToString();
    }



    public void Sfx_VolumeLogic()
    {
        GameObject Vol = GameObject.Find("Volume_Sfx");
        GameObject Vol_Val_Slider = GameObject.Find("Txt_Sfx_Val");
        GameObject Fishbone_vol = GameObject.Find("Fishbone");
        float vol_val = Vol.GetComponent<Slider>().value;
        Fishbone_vol.GetComponent<AudioSource>().volume = vol_val;
        Vol_Val_Slider.GetComponent<TextMeshProUGUI>().text = vol_val.ToString();
    }

    public void Btn_PlayMultiLogic()
    {
        unityObjects["Btn_PlayMulti"].GetComponent<Button>().interactable = false;
        WarpClient.GetInstance().GetRoomsInRange(1, 2);
        UpdateStatus("Seraching for available rooms...");
        
    }

    #endregion
}
