using UnityEngine;
using UnityEngine.SceneManagement;

public class WebGLAudioUnlock : MonoBehaviour
{
    private bool sceneMusicPlaying;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        GameObject gameObject = new GameObject(nameof(WebGLAudioUnlock));
        DontDestroyOnLoad(gameObject);
        gameObject.AddComponent<WebGLAudioUnlock>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        TryStartSceneMusic();
    }

    private void Update()
    {
        if (sceneMusicPlaying)
        {
            return;
        }

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            TryStartSceneMusic();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneMusicPlaying = false;
        TryStartSceneMusic();
    }

    private void TryStartSceneMusic()
    {
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (AudioSource source in sources)
        {
            if (!IsSceneMusic(source))
            {
                continue;
            }

            if (!source.isPlaying)
            {
                source.loop = true;
                source.Play();
            }

            sceneMusicPlaying |= source.isPlaying;
        }
    }

    private static bool IsSceneMusic(AudioSource source)
    {
        return source != null
            && source.clip != null
            && (source.gameObject.name == "Music" || (source.playOnAwake && source.loop));
    }
}
