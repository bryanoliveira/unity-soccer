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
            ReadSavedTextsToInput();
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        // unpause
        else
        {
            PlayerPrefs.SetString("BlueGoalCelebrationText", blueGoalCelebrationInput.text);
            PlayerPrefs.SetString("BlueWinnerCelebrationText", blueWinnerCelebrationInput.text);
            PlayerPrefs.SetString("OrangeGoalCelebrationText", orangeGoalCelebrationInput.text);
            PlayerPrefs.SetString("OrangeWinnerCelebrationText", orangeWinnerCelebrationInput.text);
            PlayerPrefs.SetString("DrawText", drawInput.text);
            soccerSettings.blueGoalCelebrationText = blueGoalCelebrationInput.text;
            soccerSettings.blueWinnerCelebrationText = blueWinnerCelebrationInput.text;
            soccerSettings.orangeGoalCelebrationText = orangeGoalCelebrationInput.text;
            soccerSettings.orangeWinnerCelebrationText = orangeWinnerCelebrationInput.text;
            soccerSettings.drawText = drawInput.text;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void RestartGame()
    {
        PlayerPrefs.SetInt("BlueScore", 0);
        PlayerPrefs.SetInt("OrangeScore", 0);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ReadSavedTextsToInput()
    {
        blueGoalCelebrationInput.text = PlayerPrefs.GetString("BlueGoalCelebrationText", soccerSettings.blueGoalCelebrationText);
        blueWinnerCelebrationInput.text = PlayerPrefs.GetString("BlueWinnerCelebrationText", soccerSettings.blueWinnerCelebrationText);
        orangeGoalCelebrationInput.text = PlayerPrefs.GetString("OrangeGoalCelebrationText", soccerSettings.orangeGoalCelebrationText);
        orangeWinnerCelebrationInput.text = PlayerPrefs.GetString("OrangeWinnerCelebrationText", soccerSettings.orangeWinnerCelebrationText);
        drawInput.text = PlayerPrefs.GetString("DrawText", soccerSettings.drawText);
    }
}

