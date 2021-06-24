using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer : MonoSingleton<Initializer>
{
    [SerializeField] private bool supervisor = false;

    private void Awake()
    {
        if(supervisor)
            SceneManager.LoadScene("Supervisor", LoadSceneMode.Additive);
        else
            SceneManager.LoadScene("TestSubject", LoadSceneMode.Additive);
    }
}
