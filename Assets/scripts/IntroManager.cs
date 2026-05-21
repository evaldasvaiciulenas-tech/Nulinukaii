using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenu");
    }
}