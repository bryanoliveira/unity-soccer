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
    private InputField blueGoalCelebrationInput;
    [SerializeField]
    private InputField orangeGoalCelebrationInput;
    [SerializeField]
    private InputField blueWinnerCelebrationInput;
    [SerializeField]
    private InputField orangeWinnerCelebrationInput;
    [SerializeField]
    private InputField drawInput;

    // TOP UI
    [SerializeField]
    private Text timerText;
    [SerializeField]
    private Text blueScoreText;
    [SerializeField]
    private Text orangeScoreText;
    [SerializeField]
    private Text blueTeamNameText;
    [SerializeField]
    private Text orangeTeamNameText;
    [SerializeField]

    private GameObject pauseMenu;


    // Awake is called before Start
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
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

    public static void UpdateOrangeScore(int score)
    {
        instance.orangeScoreText.text = score.ToString();
    }

    public static void StaticPause()
    {
        instance.Pause();
    }

    //// INSTANCE METHODS

    public void Pause()
    {
        // update ui
        blueTeamNameText.text = soccerSettings.blueTeamName;
        orangeTeamNameText.text = soccerSettings.orangeTeamName;

        // pause
        if (Time.timeScale == 1)
        {
            blueGoalCelebrationInput.text = soccerSettings.blueGoalCelebrationText;
            blueWinnerCelebrationInput.text = soccerSettings.blueWinnerCelebrationText;
            orangeGoalCelebrationInput.text = soccerSettings.orangeGoalCelebrationText;
            orangeWinnerCelebrationInput.text = soccerSettings.orangeWinnerCelebrationText;
            drawInput.text = soccerSettings.drawText;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        // unpause
        else
        {
            soccerSettings.SetUITexts(
                blueGoalCelebrationInput.text,
                blueWinnerCelebrationInput.text,
                orangeGoalCelebrationInput.text,
                orangeWinnerCelebrationInput.text,
                drawInput.text
            );
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void RestartGame()
    {
        soccerSettings.ResetPrefs();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

