﻿using System;
using System.Collections.Generic;

public class OfflineNotificationMessage : Message
{
    public List<Guid> Agents { get; set; } = new List<Guid>();

    public OfflineNotificationMessage()
    {
        Id = MessageId.OfflineNotification;
        Flags = 0;
        Frequency = MessageFrequency.Low;
    }

    #region DeSerialise
    protected override void DeSerialise(byte[] buf, ref int o, int length)
    {
        byte nAgents = buf[o++];
        for (byte i = 0; i < nAgents; i++)
        {
            Agents.Add(BinarySerializer.DeSerializeGuid(buf, ref o, length));
            Logger.LogDebug(ToString());
        }
    }
    #endregion DeSerialise

    public override string ToString()
    {
        string s = $"{base.ToString()}:";
        foreach (Guid agent in Agents)
        {
            s += $"\n    AgentId={agent}";
        }
        return s;
    }
}