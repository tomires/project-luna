using UnityEngine.SceneManagement;

public class Initializer : MonoSingleton<Initializer>
{
    public bool supervisor = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if(supervisor)
            SceneManager.LoadScene("Supervisor", LoadSceneMode.Additive);
        else
            SceneManager.LoadScene("TestSubject", LoadSceneMode.Additive);
    }
}
