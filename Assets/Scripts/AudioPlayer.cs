using UnityEngine;

public class AudioPlayer : MonoSingleton<AudioPlayer>
{
    [Header("Sounds")]
    [SerializeField] private AudioClip confirmation;
    [SerializeField] private AudioClip up;
    [SerializeField] private AudioClip down;

    private AudioSource _audioSource;

    public void PlaySound(Constants.Sounds sound)
    {
        if (_audioSource.isPlaying) return;
        var clip = sound switch
        {
            Constants.Sounds.Up => up,
            Constants.Sounds.Down => down,
            _ => confirmation
        };
        _audioSource.PlayOneShot(clip);
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _audioSource = gameObject.AddComponent<AudioSource>();
    }
}
