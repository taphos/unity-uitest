using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FirstScreen : MonoBehaviour 
{
    [Inject] NetworkClient networkClient;

    [SerializeField] GameObject secondScreenPrefab;
    [SerializeField] Text responseText;

    void Start()
    {
        this.Inject();
    }

    public void OpenSecondScreen()
    {
        var s = Object.Instantiate(secondScreenPrefab);
        s.name = secondScreenPrefab.name;
        s.transform.SetParent(transform.parent, false);
    }

    public void SendNetworkRequest()
    {
        responseText.text = networkClient.SendServerRequest("i_need_data");
    }
}
