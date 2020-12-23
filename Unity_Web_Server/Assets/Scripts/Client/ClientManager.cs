using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour
{
    public static ClientManager instance = null;
    
    //서버 주소
    [Header(" - Server")]
    public string serverUrl = "http://15.164.163.141";

    [Header(" - SignUp Text Field")]
    public InputField set_ID_Field;             //아이디 입력칸
    public InputField set_PW_Field;             //비밀번호 입력칸
    public InputField set_NickName_Field;       //닉네임 입력칸
    public InputField set_Email_Field;          //이메일 입력칸

    [Header(" - SignIn / Out")]                 // - 로그인, 로그아웃
    public InputField input_ID_Field;           //아이디 입력칸
    public InputField input_PW_Field;           //비밀번호 입력칸
    public GameObject Btn_SignOut;              //로그아웃 버튼
    public GameObject btn_SignIn_Submit;        //로그인 버튼
    public Text logIn_ID_Data;                  //로그인 정보

    [Header(" - Show Message")]
    public Text message;

    [Header(" - Input Server URL")]
    public InputField input_URL_Field;

    // Start is called before the first frame update
    void Start()
    {
        //싱글톤 구현
        instance = this;

        message = GameObject.Find("Message").GetComponent<Text>();
    }

    public void SignUp()
    {
        StartCoroutine(SendUserInfo_SignUp());
    }

    public void LogIn()
    {
        StartCoroutine(SendUserInfo_SignIn());
    }

    //유저의 회원가입 정보를 POST 형식으로 서버에게 보내는 함수
    IEnumerator SendUserInfo_SignUp()
    {
        //프레임 종료시 서버 요청
        yield return new WaitForEndOfFrame();

        WWWForm form = new WWWForm();
        form.AddField("id", set_ID_Field.text);             set_ID_Field.text = "";
        form.AddField("password", set_PW_Field.text);       set_PW_Field.text = "";
        form.AddField("nickname", set_NickName_Field.text); set_NickName_Field.text = "";
        form.AddField("email", set_Email_Field.text);       set_Email_Field.text = "";

        //서버 업로드 - Post 형식을 사용
        using (var w = UnityWebRequest.Post(serverUrl + "/api/account/signup", form))
        {
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
                Debug.LogError(w.error);

            else {
                JSONObject json = new JSONObject(w.downloadHandler.text);                   //받은 텍스트를 json으로 변경
                message.text = json.GetField("mes").str;                                    //사용하고자 하면 ? JSONObject.GetField("KEY_NAME").str!!
            }
        }
    }

    IEnumerator SendUserInfo_SignIn()
    {
        //프레임 종료시 서버 요청
        yield return new WaitForEndOfFrame();

        WWWForm form = new WWWForm();
        form.AddField("id", input_ID_Field.text);
        form.AddField("password", input_PW_Field.text);

        using (var w = UnityWebRequest.Post(serverUrl + "/api/account/signin", form))
        {
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
                Debug.LogError(w.error);

            else
            {
                JSONObject json = new JSONObject(w.downloadHandler.text);

                if(json.GetField("result").b)
                {
                    UserData.Set_userID(input_ID_Field.text);
                    UserData.Set_userNickname(json.GetField("nickname").str);
                    //나중에는 유저 데이터를 받아와서 저장함
                    UserData.Set_UserData("{\"string\" : \"123\"}");
                    //UserData.Set_serverURL(input_URL_Field.text);
                    
                    UnityEngine.SceneManagement.SceneManager.LoadScene("RoomTest");
                }
                else
                {
                    message.text = "SignIn Failed, " + json.GetField("mes").str;
                    Debug.Log(message.text);
                } 
            }
        }
    }
}
