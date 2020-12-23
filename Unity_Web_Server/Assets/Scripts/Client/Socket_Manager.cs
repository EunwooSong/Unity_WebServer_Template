using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;

public class Socket_Manager : MonoBehaviour
{
    //=-- Socet --=
    public static Socket_Manager socketMgr = null;
    public Client socket { get; private set; }

    private string socketURL = "http://15.164.163.141:51234/";
    private string socketStateMsg;

    void Awake()
    {
        if(socketMgr == null)
            socketMgr = this;

        socketStateMsg = "Socket is not Connect - " + System.DateTime.Now.ToString();

        socket = new Client(socketURL);
        socket.Opened += SocketOpened;
        socket.Connect();
    }

    //ConnectSocket / 설정한 URL로 소켓 접속 시작
    public void ConnectSocket()
    {
        socket.Connect();
    }

    //DisconnectSocket / 소켓 연결 종료
    public void DisconnectSocket()
    {
        socket.Close();
        socketStateMsg = "Disconnected Socket - " + System.DateTime.Now.ToString();
    }

    //GetSocketConnectedMessage / 소켓이 연결되었는지 가져옴(string), 소켓 상태 메세지 불러옴
    public string GetSocketConnectedMessage()
    {
        return socketStateMsg;
    }

    //SocketOpened / 소켓이 접속되면 호출되는 함수
    private void SocketOpened(object sender, System.EventArgs e)
    {
        Debug.Log("Socket Opened - " + System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        socketStateMsg = "Server Connected - " + System.DateTime.Now.ToString();
    }

    //OnDisable / 프로그램 종료시 소켓 연결 종료
    private void OnDisable()
    {
        DisconnectSocket();
        Debug.Log(socketStateMsg);
    }
}
