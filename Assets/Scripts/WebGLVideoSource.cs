using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class WebGLVideoSource : MonoBehaviour
{
    [SerializeField] private string fileName;

    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        ConfigureSource();
    }

    private void OnEnable()
    {
        ConfigureSource();
    }

    private void ConfigureSource()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (string.IsNullOrWhiteSpace(fileName) && videoPlayer.clip != null)
        {
            fileName = videoPlayer.clip.name + ".mp4";
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = $"{Application.streamingAssetsPath}/{Uri.EscapeDataString(fileName)}";
        videoPlayer.Prepare();
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneHook()
    {
        SceneManager.sceneLoaded += (_, _) => ConfigureSceneVideos();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ConfigureInitialScene()
    {
        ConfigureSceneVideos();
    }

    private static void ConfigureSceneVideos()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        VideoPlayer[] players = FindObjectsByType<VideoPlayer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (VideoPlayer player in players)
        {
            string videoFileName = player.clip != null ? player.clip.name + ".mp4" : string.Empty;
            if (string.IsNullOrWhiteSpace(videoFileName))
            {
                continue;
            }

            bool shouldPlay = player.playOnAwake;
            player.source = VideoSource.Url;
            player.url = $"{Application.streamingAssetsPath}/{Uri.EscapeDataString(videoFileName)}";
            player.Prepare();

            if (shouldPlay)
            {
                player.Play();
            }
        }
#endif
    }
}
