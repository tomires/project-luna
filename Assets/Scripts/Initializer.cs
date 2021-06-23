using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer : MonoSingleton<Initializer>
{
    Coroutine autoSwitch;

    private void Awake()
    {
        //SceneManager.LoadScene("TestSubject", LoadSceneMode.Additive);
        SceneManager.LoadScene("Supervisor", LoadSceneMode.Additive);
        //autoSwitch = StartCoroutine(SwitchToVr());
    }

    private void OnDestroy()
    {
        if(autoSwitch != null)
            StopCoroutine(autoSwitch);
    }

    private IEnumerator SwitchToVr()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("TestSubject", LoadSceneMode.Additive);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            SceneManager.LoadScene("Supervisor", LoadSceneMode.Additive);
        else if (Input.GetKeyDown(KeyCode.S))
            SceneManager.LoadScene("TestSubject", LoadSceneMode.Additive);
    }
}
