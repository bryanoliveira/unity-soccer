using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using System.Text;
using System;
using System.Collections.Generic;

public class EnvConfigurationChannel : SideChannel
{
    internal enum ConfigurationType : int
    {
        BlueTeamName = 0,
        OrangeTeamName = 1,
        PlayerPosition = 2,
        PlayerVelocity = 3,
        PlayerRotation = 4,
        BallPosition = 5,
        BallVelocity = 6,
    }

    public SoccerSettings m_SoccerSettings;
    public SoccerEnvController m_SoccerEnvController;

    public EnvConfigurationChannel()
    {
        ChannelId = new Guid("3f07928c-2b0e-494a-810b-5f0bbb7aaeca");
    }


    protected override void OnMessageReceived(IncomingMessage msg)
    {
        var messageType = (ConfigurationType)msg.ReadInt32();
        int playerIndex;
        IList<float> vecValue;
        switch (messageType)
        {
            case ConfigurationType.BlueTeamName:
                m_SoccerSettings.SetBlueTeamName(msg.ReadString());
                break;
            case ConfigurationType.OrangeTeamName:
                m_SoccerSettings.SetOrangeTeamName(msg.ReadString());
                break;
            case ConfigurationType.PlayerPosition:
                playerIndex = msg.ReadInt32();
                vecValue = msg.ReadFloatList();
                m_SoccerEnvController.SetPlayerPosition(playerIndex, vecValue[0], vecValue[1]);
                break;
            case ConfigurationType.PlayerVelocity:
                playerIndex = msg.ReadInt32();
                vecValue = msg.ReadFloatList();
                m_SoccerEnvController.SetPlayerVelocity(playerIndex, vecValue[0], vecValue[1]);
                break;
            case ConfigurationType.PlayerRotation:
                playerIndex = msg.ReadInt32();
                m_SoccerEnvController.SetPlayerRotation(playerIndex, msg.ReadFloat32());
                break;
            case ConfigurationType.BallPosition:
                vecValue = msg.ReadFloatList();
                m_SoccerEnvController.SetBallPosition(vecValue[0], vecValue[1]);
                break;
            case ConfigurationType.BallVelocity:
                vecValue = msg.ReadFloatList();
                m_SoccerEnvController.SetBallVelocity(vecValue[0], vecValue[1]);
                break;
            default:
                Debug.LogWarning(
                    "Unknown engine configuration received from Python. Make sure" +
                    " your Unity and Python versions are compatible.");
                break;
        }
    }

    public void SendDebugStatementToPython(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error)
        {
            var stringToSend = type.ToString() + ": " + logString + "\n" + stackTrace;
            using (var msgOut = new OutgoingMessage())
            {
                msgOut.WriteString(stringToSend);
                QueueMessageToSend(msgOut);
            }
        }
    }
}