using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomCard : MonoBehaviour
{
    [Header(" - Room Card Child GameObject")]
    public Text roomName;
    public Text personnel;
    public Text connetedUsers;
    public GameObject passwordLock;
    public GameObject progress;

    public bool isLock;
    [SerializeField] private string _id;

    // Start is called before the first frame update
    void Awake()
    {
        //각 컴포넌트 자동 추가
        roomName = this.transform.GetChild(0).GetComponent<Text>();
        progress = this.transform.GetChild(1).gameObject;
        passwordLock = this.transform.GetChild(2).gameObject;
        personnel = this.transform.GetChild(3).GetChild(0).GetComponent<Text>();
        connetedUsers = this.transform.GetChild(3).GetChild(2).GetComponent<Text>();
    }

    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(SetWantToJoin);
    }

    //방 최초 생성 시
    public void SetRoomInfo(string _roomName, string _personnel, string _connetedUsers, bool _passwordLock, bool _progress, string _id)
    {
        roomName.text = _roomName;
        personnel.text = _personnel;
        connetedUsers.text = _connetedUsers;
        passwordLock.SetActive(_passwordLock);

        isLock = _passwordLock;
        progress.SetActive((_progress.Equals("true")) ? true : false);

        this._id = _id;
    }

    //방 정보 수정 시
    public void SetRoomInfo(string _connetedUsers, bool _progress)
    {
        connetedUsers.text = _connetedUsers;
        progress.SetActive(_progress);
    }

    public string GetRoomID()
    {
        return _id;
    }

    public void SetWantToJoin()
    {
        RoomManager.RoomMgr.wantToJoin = this;
        Debug.Log("Current WantToJoin Room ID : " + _id);
    }
}
