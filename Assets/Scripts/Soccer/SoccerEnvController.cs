using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
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


    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_PurpleAgentGroup;

    private int m_ResetTimer;
    private float m_GameTimer;

    public bool isVisualizer = false;
    [HideInInspector]
    public bool inGame;
    private int blueGoals;
    private int purpleGoals;

    public List<ParticleSystem> goalExplosions;
    private AudioSource audioSource;
    public List<AudioClip> cheerSounds;

    void Start()
    {
        blueGoals = 0;
        purpleGoals = 0;
        inGame = false;

        audioSource = GetComponent<AudioSource>();
        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        m_GameTimer = m_SoccerSettings.gameDuration;
        // Initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_PurpleAgentGroup = new SimpleMultiAgentGroup();
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
                m_PurpleAgentGroup.RegisterAgent(item.Agent);
            }
        }
        ResetScene();

        if (isVisualizer)
            StartCoroutine(StartDelayed());
        else
            inGame = true;
    }

    void FixedUpdate()
    {
        if (inGame)
        {
            if (isVisualizer)
            {
                if (m_GameTimer > 0)
                {
                    m_GameTimer -= Time.fixedDeltaTime;
                    CanvasController.UpdateTimer(m_GameTimer);
                }
                else
                {
                    CanvasController.TriggerScoredAnimation("GAME OVER!");
                    m_GameTimer = m_SoccerSettings.gameDuration;
                    m_BlueAgentGroup.GroupEpisodeInterrupted();
                    m_PurpleAgentGroup.GroupEpisodeInterrupted();
                    ResetScene();
                }
            }
            else
            {
                m_ResetTimer += 1;
                if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
                {
                    m_BlueAgentGroup.GroupEpisodeInterrupted();
                    m_PurpleAgentGroup.GroupEpisodeInterrupted();
                    ResetScene();
                }
            }
        }
    }


    public void ResetBall()
    {
        var randomPosX = Random.Range(-2.5f, 2.5f);
        var randomPosZ = Random.Range(-2.5f, 2.5f);

        ball.transform.position = m_BallStartingPos + new Vector3(randomPosX, 0f, randomPosZ);
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

    }

    public void GoalTouched(Team scoredTeam)
    {
        if (scoredTeam == Team.Blue)
        {
            m_BlueAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_PurpleAgentGroup.AddGroupReward(-1);
        }
        else
        {
            m_PurpleAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_BlueAgentGroup.AddGroupReward(-1);
        }
        m_PurpleAgentGroup.EndGroupEpisode();
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
                CanvasController.UpdateBlueScore(blueGoals);
                direction = new Vector3(-1, 0, 0);
                CanvasController.TriggerScoredAnimation(m_SoccerSettings.blueCelebrationText);
            }
            else
            {
                purpleGoals++;
                CanvasController.UpdatePurpleScore(purpleGoals);
                direction = new Vector3(1, 0, 0);
                CanvasController.TriggerScoredAnimation(m_SoccerSettings.purpleCelebrationText);
            }
            int explosion = Random.Range(0, goalExplosions.Count);
            goalExplosions[explosion].transform.position = ball.transform.position;
            goalExplosions[explosion].Play();
            ball.SetActive(false);

            // push agents away from the goal
            foreach (var item in AgentsList)
                item.Rb.AddForce(direction * 50f, ForceMode.VelocityChange);

            yield return new WaitForSeconds(2);
            ResetScene();

            ball.SetActive(true);
            Time.timeScale = 1;
            StartCoroutine(StartDelayed());
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
            var randomPosX = Random.Range(-5f, 5f);
            var newStartPos = item.Agent.initialPos + new Vector3(randomPosX, 0f, 0f);
            var rot = item.Agent.rotSign * Random.Range(80.0f, 100.0f);
            var newRot = Quaternion.Euler(0, rot, 0);
            item.Agent.transform.SetPositionAndRotation(newStartPos, newRot);

            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }

        //Reset Ball
        ResetBall();
    }
}
