using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomInManager : MonoBehaviour
{


    [Header(" - Room Info")]
    public Text roomName;

    [Header(" - PlayerInfo Group")]
    public GameObject userInfo_Card;
    public Dictionary<string, GameObject> userInfoGroup;
    public GameObject content;

    [Header(" - Server Message")]
    public Text serverMessage;

    [Header(" - Chat Message")]
    public Text chatField;
    public InputField messageInputField;

    [Header(" - Buffer")]
    [SerializeField] string buffer = null;

    void Awake()
    {
        roomName = GameObject.Find("RoomName").GetComponent<Text>();

        userInfo_Card = Resources.Load("Prefabs/UserNameInfo_Card") as GameObject;
        userInfoGroup = new Dictionary<string, GameObject>();
        content = GameObject.Find("PlayerInfo_Content");

        serverMessage = GameObject.Find("ServerMessage").GetComponent<Text>();

        chatField = GameObject.Find("ChatField").GetComponent<Text>();
        messageInputField = GameObject.Find("InputMesField").GetComponent<InputField>();
    }

    void Start()
    {
        Socket_Manager.socketMgr.socket.On("getMessasge", (data) => {
            buffer = data.Json.args[0].ToString();
        });
    }

    void Update()
    {
        //Chat Manager
        //Update UserIsnfo
        //Delete UserInfo
        //Start Game
    }

    //Send Message
    void Send_Message()
    {
        
    }

    //Get Message
    void Chat_Manager()
    {
        
    }


}
