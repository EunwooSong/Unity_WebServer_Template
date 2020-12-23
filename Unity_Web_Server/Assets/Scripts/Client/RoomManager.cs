using System.Collections;
using System.Collections.Generic;
//using SocketIO;
//New SocketIO
using SocketIOClient;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//Lobby Scene에서 서버 주소 입력시
//Main Room Scene에서 그 주소로 연결

public class RoomManager : MonoBehaviour
{
    public static RoomManager RoomMgr { get; private set; }
    private string serverURL = "http://15.164.163.141/api/";

    [Header(" - Room Group")]
    public GameObject roomPrefab;
    public Dictionary<string, GameObject> roomInfoGroup;
    public GameObject scrollRect;
    public bool refresh;

    public RoomCard wantToJoin;         //Want To Join Room Info
    public GameObject passwordField;    //GameObjects / Password Field

    [Header(" - Create Room")]
    public InputField roomName;
    public InputField password;

    [Header(" - Message")]
    public Text showBuffer;
    public Text mes;

    [Header(" - UserInfo")]
    public Text userID;

    [Header(" - Buffer")]
    string buffer = null;

    void Awake()
    {
        RoomMgr = this;

        mes = GameObject.Find("Message").GetComponent<Text>();
        showBuffer = GameObject.Find("Buffer").GetComponent<Text>();
        userID = GameObject.Find("Sign ID").GetComponent<Text>();

        userID.text += UserData.Get_userID();
        scrollRect = GameObject.Find("Content");
        refresh = true;

        passwordField = GameObject.Find("passwordField");
        
        //Dictionary 초기화
        roomInfoGroup = new Dictionary<string, GameObject>();
    }

    void Start()
    {
        //Get Room Card Prefabs
        roomPrefab = Resources.Load("Prefabs/RoomInfo_Card") as GameObject;

        //방 데이터 불러오기 소켓 추가
        Socket_Manager.socketMgr.socket.On("sendMainRoom", (data) => {
            buffer = data.Json.args[0].ToString();
        });
    }

    void Update()
    {
        MainRoomLoad();     //방 화면(목록) 로드
                            //룸 삭제 요청 받아옴
                            //룸 생성 API 보내기
    }

    public void Refresh()
    {
        refresh = true;
    }

    void MainRoomLoad()
    {
        Set_Room();

        if (refresh)
        {
            JSONObject mes = new JSONObject();
            mes.AddField("mes", "Hello, World!");

            Debug.Log("Refresh List! " + mes);
            Socket_Manager.socketMgr.socket.Emit("MainLoad", mes.str);
            refresh = false;
        }
    }

    public void CreateRoom()
    {
        StartCoroutine(CreateRoom_Post());
    }

    public void ShowPasswordField()
    {
        if (!wantToJoin.isLock) {
            JoinRoom();
            return;
        }

        passwordField.SetActive(true);
    }

    public void JoinRoom()
    {
        //방 참가 요청
        StartCoroutine(JoinRoom_Post(passwordField.transform.GetChild(0).GetComponent<InputField>().text));

        passwordField.transform.GetChild(0).GetComponent<InputField>().text = "";
        passwordField.SetActive(false);
    }

    IEnumerator CreateRoom_Post()
    {
        //프레임 종료시 서버 요청
        yield return new WaitForEndOfFrame();

        //Set Send Data
        WWWForm form = new WWWForm();
        form.AddField("roomname", roomName.text);
        form.AddField("nickname", UserData.Get_userID());
        form.AddField("userdata", "test123");
        form.AddField("password", password.text);
        form.AddField("personnel", UnityEngine.Random.Range(3, 10));

        using (var w = UnityWebRequest.Post(serverURL + "room/create", form))
        {
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
            {
                mes.text = w.error;
                Debug.LogError(w.error);
            }

            else
            {
                JSONObject json = new JSONObject(w.downloadHandler.text);
                mes.text = json.GetField("mes").str;

                if (json.GetField("result").b)
                {
                    //방으로 접속
                    StartCoroutine(JoinRoom_Post(password.text));
                }
                else
                {
                    //실패
                    Debug.Log("Create Failed... ");
                }
            }
        }
    }

    IEnumerator JoinRoom_Post(string userinput_PW)
    {
        string inputPassword = userinput_PW;

        Debug.Log("passwordField : " + inputPassword);
        yield return new WaitForEndOfFrame();

        //Set Send Data
        WWWForm form = new WWWForm();
        form.AddField("_id", wantToJoin.GetRoomID());
        form.AddField("password", inputPassword);
        form.AddField("nickname", UserData.Get_userNickname());
        form.AddField("userData", UserData.Get_UserData());

        using(var w = UnityWebRequest.Post(serverURL + "room/join", form))
        {
            yield return w.SendWebRequest();

            if(w.isNetworkError || w.isHttpError)
            {
                mes.text = w.error;
                Debug.Log(w.error);
            }

            else
            {
                JSONObject json = new JSONObject(w.downloadHandler.text);

                if(json.GetField("result").b)
                {
                    UserData.Set_roomID(wantToJoin.GetRoomID());

                    Debug.Log("Connecting Room. . .");
                    UnityEngine.SceneManagement.SceneManager.LoadScene("RoomIn");
                }
                else
                {
                    Debug.Log("Join Failed. . .");
                    mes.text = json.GetField("mes").str;
                }
            }
        }
    }

    void Set_Room()
    {
        if (buffer == null)
            return;

        showBuffer.text = buffer;

        JSONObject d = new JSONObject(buffer);

        RoomCard tmp = null;
        GameObject obj = null;

        for (int i = 0; i < d.GetField("value").list.Count; i++)
        {
            JSONObject room = d.GetField("value").list[i];

            //If Has Key -> Fixing Room Info
            if (roomInfoGroup.TryGetValue(room.GetField("_id").str, out obj))
            {
                tmp = obj.GetComponent<RoomCard>();

                tmp.SetRoomInfo(room.GetField("connectedUsers").str, room.GetField("progress").b);
                Debug.Log("Fixing Room . . . Succeeded / " + room.GetField("_id").str);
                continue;
            }

            //Else -> Creating Room Card
            //Create Room Card GameObject
            obj = Instantiate(roomPrefab) as GameObject;
            obj.transform.parent = scrollRect.GetComponent<RectTransform>();
            obj.GetComponent<RectTransform>().localScale = new Vector2(1.0f, 1.0f);

            //Set Each Room Info
            tmp = obj.GetComponent<RoomCard>();
            tmp.SetRoomInfo(room.GetField("roomname").str, room.GetField("personnel").str,
                                    room.GetField("connectedUsers").str, room.GetField("passwordLock").b,
                                    room.GetField("progress").b, room.GetField("_id").str);

            //Add Room Card At RoomInfoGroup Dictionary
            roomInfoGroup.Add(room.GetField("_id").str, obj);
            Debug.Log("Creating Room . . . Succeeded / " + room.GetField("_id").str);
        }

        //Clear Buffer
        buffer = null;
    }
}
