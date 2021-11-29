using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using System.Text;
using System;

public class EnvConfigurationChannel : SideChannel
{
    internal enum ConfigurationType : int
    {
        BlueTeamName = 0,
        OrangeTeamName = 1
    }

    public SoccerSettings m_SoccerSettings;

    public EnvConfigurationChannel()
    {
        ChannelId = new Guid("3f07928c-2b0e-494a-810b-5f0bbb7aaeca");
    }


    protected override void OnMessageReceived(IncomingMessage msg)
    {
        var messageType = (ConfigurationType)msg.ReadInt32();
        switch (messageType)
        {
            case ConfigurationType.BlueTeamName:
                m_SoccerSettings.blueTeamName = msg.ReadString();
                break;
            case ConfigurationType.OrangeTeamName:
                m_SoccerSettings.orangeTeamName = msg.ReadString();
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