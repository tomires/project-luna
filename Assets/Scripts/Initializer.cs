using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer : MonoSingleton<Initializer>
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            SceneManager.LoadScene("Supervisor", LoadSceneMode.Additive);
        else if (Input.GetKeyDown(KeyCode.S))
            SceneManager.LoadScene("TestSubject", LoadSceneMode.Additive);
    }
}
