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
}
