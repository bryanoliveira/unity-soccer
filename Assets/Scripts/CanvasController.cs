using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{

    static CanvasController instance;

    [SerializeField]
    private SoccerSettings soccerSettings;

    // ANNOUNCEMENTS
    [SerializeField]
    private Animator scoredAnimator;
    [SerializeField]
    private Text scoredText;
    [SerializeField]
    private InputField blueScoredInput;
    [SerializeField]
    private InputField purpleScoredInput;

    // TOP UI
    [SerializeField]
    private Text timerText;
    [SerializeField]
    private Text blueScoreText;
    [SerializeField]
    private Text purpleScoreText;
    [SerializeField]

    private GameObject pauseMenu;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        blueScoredInput.text = PlayerPrefs.GetString("BlueScoredText");
        purpleScoredInput.text = PlayerPrefs.GetString("PurpleScoredText");
        soccerSettings.blueCelebrationText = blueScoredInput.text;
        soccerSettings.purpleCelebrationText = purpleScoredInput.text;

        Pause();
    }

    //// STATIC METHODS

    public static void TriggerScoredAnimation(string text = null)
    {
        if (text != null)
            instance.scoredText.text = text;
        instance.scoredAnimator.SetTrigger("Scored");
    }

    public static void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        instance.timerText.text = minutes.ToString() + ":" + seconds.ToString("00");
    }

    public static void UpdateBlueScore(int score)
    {
        instance.blueScoreText.text = score.ToString();
    }

    public static void UpdatePurpleScore(int score)
    {
        instance.purpleScoreText.text = score.ToString();
    }

    public static void StaticPause()
    {
        instance.Pause();
    }

    //// INSTANCE METHODS

    public void Pause()
    {
        // pause
        if (Time.timeScale == 1)
        {
            blueScoredInput.text = PlayerPrefs.GetString("BlueScoredText");
            purpleScoredInput.text = PlayerPrefs.GetString("PurpleScoredText");
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        // unpause
        else
        {
            PlayerPrefs.SetString("BlueScoredText", blueScoredInput.text);
            PlayerPrefs.SetString("PurpleScoredText", purpleScoredInput.text);
            soccerSettings.blueCelebrationText = blueScoredInput.text;
            soccerSettings.purpleCelebrationText = purpleScoredInput.text;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

