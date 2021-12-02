using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using UnityEngine;

public class SoccerEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public AgentSoccer Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }


    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    /// <summary>
    /// The area bounds.
    /// </summary>

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>

    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRb;
    Vector3 m_BallStartingPos;

    //List of Agents On Platform
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();

    private SoccerSettings m_SoccerSettings;
    EnvConfigurationChannel m_EnvConfigurationChannel;


    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_OrangeAgentGroup;

    private int m_ResetTimer;
    private float m_GameTimer;

    public bool isVisualizer = false;
    [HideInInspector]
    public bool inGame;
    private int blueGoals;
    private int orangeGoals;

    public List<ParticleSystem> goalExplosions;
    private AudioSource audioSource;
    public List<AudioClip> cheerSounds;

    void Awake()
    {
        m_SoccerSettings = FindObjectOfType<SoccerSettings>();

        // configure side channels
        m_EnvConfigurationChannel = new EnvConfigurationChannel();
        Application.logMessageReceived += m_EnvConfigurationChannel.SendDebugStatementToPython;
        m_EnvConfigurationChannel.m_SoccerSettings = m_SoccerSettings;
        m_EnvConfigurationChannel.m_SoccerEnvController = this;
        SideChannelManager.RegisterSideChannel(m_EnvConfigurationChannel);
        Debug.Log("EnvConfigurationChannel is set up.");
    }

    void Start()
    {
        inGame = false;

        // retrieve objects
        audioSource = GetComponent<AudioSource>();
        m_GameTimer = m_SoccerSettings.gameDuration;

        // initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_OrangeAgentGroup = new SimpleMultiAgentGroup();
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallStartingPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            if (item.Agent.team == Team.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            }
            else
            {
                m_OrangeAgentGroup.RegisterAgent(item.Agent);
            }
        }

        // load preferences
        blueGoals = m_SoccerSettings.GetBlueScore();
        orangeGoals = m_SoccerSettings.GetOrangeScore();

        // initialize simulation
        ResetScene();
        if (isVisualizer)
        {
            CanvasController.UpdateBlueScore(blueGoals);
            CanvasController.UpdateOrangeScore(orangeGoals);
            StartCoroutine(StartDelayed());
        }
        else
            inGame = true;
    }

    public void OnDestroy()
    {
        // De-register the Debug.Log callback
        Application.logMessageReceived -= m_EnvConfigurationChannel.SendDebugStatementToPython;
        if (Academy.IsInitialized)
            SideChannelManager.UnregisterSideChannel(m_EnvConfigurationChannel);
    }

    void FixedUpdate()
    {
        if (inGame)
        {
            if (isVisualizer)
            {
                m_GameTimer -= Time.fixedDeltaTime;
                if (m_GameTimer > 0)
                    CanvasController.UpdateTimer(m_GameTimer);
                else
                    StartCoroutine(GameOver());
            }
            else
            {
                m_ResetTimer += 1;
                if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
                {
                    m_BlueAgentGroup.GroupEpisodeInterrupted();
                    m_OrangeAgentGroup.GroupEpisodeInterrupted();
                    ResetScene();
                }
            }
        }
    }

    public void ResetBall()
    {
        ball.transform.position = m_BallStartingPos;
        if (!isVisualizer)
            ball.transform.position += new Vector3(
                Random.Range(-2.5f, 2.5f),
                0f,
                Random.Range(-2.5f, 2.5f)
            );
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

    }

    public void ExplodeBall()
    {
        int explosion = Random.Range(0, goalExplosions.Count);
        goalExplosions[explosion].transform.position = ball.transform.position;
        goalExplosions[explosion].Play();
        ball.SetActive(false);
    }

    public void GoalTouched(Team scoredTeam)
    {
        if (scoredTeam == Team.Blue)
        {
            m_BlueAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_OrangeAgentGroup.AddGroupReward(-1);
        }
        else
        {
            m_OrangeAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_BlueAgentGroup.AddGroupReward(-1);
        }
        m_OrangeAgentGroup.EndGroupEpisode();
        m_BlueAgentGroup.EndGroupEpisode();

        if (isVisualizer)
            StartCoroutine(CelebrateGoal(scoredTeam == Team.Blue));
        else
            ResetScene();
    }


    public IEnumerator CelebrateGoal(bool isBlueTeam)
    {
        if (inGame)
        {
            inGame = false;
            Time.timeScale = .6f;

            audioSource.PlayOneShot(cheerSounds[Random.Range(0, cheerSounds.Count)]);

            Vector3 direction;
            if (isBlueTeam)
            {
                blueGoals++;
                m_SoccerSettings.SetBlueScore(blueGoals);
                CanvasController.UpdateBlueScore(blueGoals);
                direction = new Vector3(-1, 0, 0);
                CanvasController.TriggerScoredAnimation(
                    m_SoccerSettings.blueTeamName +
                    " " +
                    m_SoccerSettings.blueGoalCelebrationText
                );
            }
            else
            {
                orangeGoals++;
                m_SoccerSettings.SetOrangeScore(orangeGoals);
                CanvasController.UpdateOrangeScore(orangeGoals);
                direction = new Vector3(1, 0, 0);
                CanvasController.TriggerScoredAnimation(
                    m_SoccerSettings.orangeTeamName +
                    " " +
                    m_SoccerSettings.orangeGoalCelebrationText
                );
            }

            ExplodeBall();

            // push agents away from the goal
            foreach (var item in AgentsList)
                item.Rb.AddForce(direction * 50f, ForceMode.VelocityChange);

            yield return new WaitForSeconds(2);
            ResetScene();

            if (Mathf.Abs(orangeGoals - blueGoals) >= 10)
            {
                inGame = true;
                StartCoroutine(GameOver());
            }
            else
            {
                ball.SetActive(true);
                Time.timeScale = 1f;
                StartCoroutine(StartDelayed());
            }
        }
    }

    public IEnumerator GameOver()
    {
        if (inGame)
        {
            inGame = false;
            Time.timeScale = .5f;
            // reset timers
            m_GameTimer = 0;
            CanvasController.UpdateTimer(m_GameTimer);

            // interrupt mlagents episode
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_OrangeAgentGroup.GroupEpisodeInterrupted();

            // cheer and indicate game over
            audioSource.PlayOneShot(cheerSounds[Random.Range(0, cheerSounds.Count)]);
            if (orangeGoals > blueGoals)
                CanvasController.TriggerScoredAnimation(
                    m_SoccerSettings.orangeTeamName +
                    " " +
                    m_SoccerSettings.orangeWinnerCelebrationText
                );
            else if (blueGoals > orangeGoals)
                CanvasController.TriggerScoredAnimation(
                    m_SoccerSettings.blueTeamName +
                    " " +
                    m_SoccerSettings.blueWinnerCelebrationText
                );
            else
                CanvasController.TriggerScoredAnimation(m_SoccerSettings.drawText);

            ExplodeBall();

            // reset game state
            yield return new WaitForSeconds(2.5f);
            Time.timeScale = 1;
            CanvasController.StaticPause();
        }
    }

    public IEnumerator StartDelayed()
    {
        if (!inGame)
        {
            yield return new WaitForSeconds(1f);
            inGame = true;
        }
    }

    public void ResetScene()
    {
        m_ResetTimer = 0;

        //Reset Agents
        foreach (var item in AgentsList)
        {
            var newStartPos = item.Agent.initialPos;
            var rot = item.Agent.rotSign * 90.0f;
            if (!isVisualizer)
            {
                newStartPos += new Vector3(Random.Range(-5f, 5f), 0f, 0f);
                rot *= Random.Range(0.85f, 1.15f);
            }
            item.Agent.transform.SetPositionAndRotation(newStartPos, Quaternion.Euler(0, rot, 0));

            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }

        //Reset Ball
        ResetBall();
    }

    public void SetBallPosition(float x, float z)
    {
        ball.transform.position = new Vector3(x, m_BallStartingPos.y, z);
    }
    public void SetBallVelocity(float x, float z)
    {
        ballRb.velocity = new Vector3(x, 0, z);
    }

    public void SetPlayerPosition(int agentId, float x, float z)
    {
        var agent = AgentsList[agentId].Agent;
        agent.transform.position = (
            new Vector3(x, agent.initialPos.y, z)
        );
    }
    public void SetPlayerVelocity(int agentId, float x, float z)
    {
        AgentsList[agentId].Rb.velocity = (
            new Vector3(x, 0, z)
        );
    }
    public void SetPlayerRotation(int agentId, float y)
    {
        AgentsList[agentId].Agent.transform.rotation = (
            Quaternion.Euler(0, y, 0)
        );
    }

}
