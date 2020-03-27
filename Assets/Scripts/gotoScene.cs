using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gotoScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ReceiveJavaMessage(string message)
    {
        if (message.Equals("ARMainScren_3") || message.Equals("Scene2"))
        {
            ChangeScene(message);
        }
        else
        {
            Debug.Log("The scene name is incorrect");
        }
    }
}
