using UnityEngine;

public class SoccerSettings : MonoBehaviour
{
    public Material orangeMaterial;
    public Material blueMaterial;
    public bool randomizePlayersTeamForTraining = true;
    public float agentRunSpeed;
    public float gameDuration = 300; // 5 minutes
    public string blueTeamName = "BLUE";
    public string orangeTeamName = "ORANGE";
    public string drawText = "IT'S A DRAW!";
    public string blueGoalCelebrationText = "SCORED!";
    public string blueWinnerCelebrationText = "WINS!";
    public string orangeGoalCelebrationText = "SCORED!";
    public string orangeWinnerCelebrationText = "WINS!";

    void Awake()
    {
        blueTeamName = PlayerPrefs.GetString("BlueTeamName", "BLUE");
        orangeTeamName = PlayerPrefs.GetString("OrangeTeamName", "ORANGE");

        blueGoalCelebrationText = PlayerPrefs.GetString("BlueGoalCelebrationText", "SCORED!");
        blueWinnerCelebrationText = PlayerPrefs.GetString("BlueWinnerCelebrationText", "WINS!");
        orangeGoalCelebrationText = PlayerPrefs.GetString("OrangeGoalCelebrationText", "SCORED!");
        orangeWinnerCelebrationText = PlayerPrefs.GetString("OrangeWinnerCelebrationText", "WINS!");
        drawText = PlayerPrefs.GetString("DrawText", "IT'S A DRAW!");
    }

    public int GetBlueScore()
    {
        return PlayerPrefs.GetInt("Score_" + blueTeamName, 0);
    }
    public int GetOrangeScore()
    {
        return PlayerPrefs.GetInt("Score_" + orangeTeamName, 0);
    }

    public void SetBlueScore(int score)
    {
        PlayerPrefs.SetInt("Score_" + blueTeamName, score);
        PlayerPrefs.Save();
    }
    public void SetOrangeScore(int score)
    {
        PlayerPrefs.SetInt("Score_" + orangeTeamName, score);
        PlayerPrefs.Save();
    }

    public void SetBlueTeamName(string name)
    {
        blueTeamName = name;
        PlayerPrefs.SetString("BlueTeamName", name);
        PlayerPrefs.Save();
    }

    public void SetOrangeTeamName(string name)
    {
        orangeTeamName = name;
        PlayerPrefs.SetString("OrangeTeamName", name);
        PlayerPrefs.Save();
    }

    public void SetUITexts(
        string _blueGoalCelebrationText,
        string _blueWinnerCelebrationText,
        string _orangeGoalCelebrationText,
        string _orangeWinnerCelebrationText,
        string _drawText
    )
    {
        PlayerPrefs.SetString("BlueGoalCelebrationText", _blueGoalCelebrationText);
        PlayerPrefs.SetString("BlueWinnerCelebrationText", _blueWinnerCelebrationText);
        PlayerPrefs.SetString("OrangeGoalCelebrationText", _orangeGoalCelebrationText);
        PlayerPrefs.SetString("OrangeWinnerCelebrationText", _orangeWinnerCelebrationText);
        PlayerPrefs.SetString("DrawText", _drawText);
        PlayerPrefs.Save();
        blueGoalCelebrationText = _blueGoalCelebrationText;
        blueWinnerCelebrationText = _blueWinnerCelebrationText;
        orangeGoalCelebrationText = _orangeGoalCelebrationText;
        orangeWinnerCelebrationText = _orangeWinnerCelebrationText;
        drawText = _drawText;
    }

    public void ResetPrefs()
    {
        PlayerPrefs.SetInt("Score_" + orangeTeamName, 0);
        PlayerPrefs.SetInt("Score_" + blueTeamName, 0);
    }
}
