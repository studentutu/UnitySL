﻿using System;
using Assets.Scripts.Communication.SlMessagingSystem.Messages.MessageSystem;
using Assets.Scripts.Extensions.SystemExtensions;
using UnityEngine;

namespace Assets.Scripts.Communication.SlMessagingSystem.Messages.Chat
{
    public enum OnlineMode : byte
    {
		Online = 0,
		Offline = 1
    }

    public enum DialogType : byte
	{
		/// <summary>
		/// Default. ID is meaningless, nothing in the binary bucket. 
		/// </summary>
		NothingSpecial = 0,

		/// <summary>
		/// Pops a messagebox with a single OK button
		/// </summary>
		MessageBox = 1,

		// pops a countdown messagebox with a single OK button
		// IM_MESSAGEBOX_COUNTDOWN = 2,

		/// <summary>
		/// You've been invited to join a group.
		///
        /// ID is the group id.
        /// The binary bucket contains a null terminated string
        /// representation of the officer/member status and join cost for
        /// the invitee. (bug # 7672) The format is 1 byte for
        /// officer/member (O for officer, M for member), and as many bytes
        /// as necessary for cost.
		/// </summary>
		GroupInvitation = 3,

		/// <summary>
		/// Inventory offer.
        /// ID is the transaction id
        /// Binary bucket is a list of inventory uuid and type. 
		/// </summary>
		InventoryOffered = 4,
		InventoryAccepted = 5,
		InventoryDeclined = 6,

		/// <summary>
		/// Group vote
        /// Name is name of person who called vote.
        /// ID is vote ID used for internal tracking
        /// TODO: _DEPRECATED suffix as part of vote removal - DEV-24856
		/// </summary>
		GroupVote = 7,

		/// <summary>
		/// Group message
        /// This means that the message is meant for everyone in the
        /// agent's group. This will result in a database query to find all
        /// participants and start an im session.
		/// </summary>
		GroupMessage_DEPRECATED = 8,

		/// <summary>
		/// Task inventory offer.
        /// ID is the transaction id
        /// Binary bucket is a (mostly) complete packed inventory item
		/// </summary>
		TaskInventoryOffered = 9,
        TaskInventoryAccepted = 10,
        TaskInventoryDeclined = 11,

		/// <summary>
		/// Copied as pending, type LL_NOTHING_SPECIAL, for new users
        /// used by offline tools
		/// </summary>
		NewUserDefault = 12,

		// session based messaging - the way that people usually actually
		// communicate with each other.

		/// <summary>
		/// Invite users to a session.
		/// </summary>
		SessionInvite = 13,

		SessionP2PInvite = 14,

		/// <summary>
		/// Start a session with your gruop
		/// </summary>
		SessionGroupStart = 15,

		/// <summary>
		/// Start a session without a calling card (finder or objects)
		/// </summary>
		SessionConferenceStart = 16,

		/// <summary>
		/// Send a message to a session.
		/// </summary>
		SessionSend = 17,

		/// <summary>
		/// Leave a session
		/// </summary>
		SessionLeave = 18,


		/// <summary>
		/// An instant message from an object - for differentiation on the
        /// viewer, since you can't IM an object yet.
		/// </summary>
		FromTask = 19,

		/// <summary>
		/// Sent an IM to a do not disturb user, this is the auto response
		/// </summary>
		DoNotDisturbAutoResponse = 20,

		/// <summary>
		/// Shows the message in the console and chat history
		/// </summary>
		ConsoleAndChatHistory = 21,

		// IM Types used for luring your friends
		LureUser = 22,
		LureAccepted = 23,
		LureDeclined = 24,
		GodLikeLureUser = 25,
		TeleportRequest = 26,

		/// <summary>
		/// IM that notifies of a new group election.
        /// Name is name of person who called vote.
        /// ID is election ID used for internal tracking
		/// </summary>
		GroupElection_DEPRECATED = 27,

		/// <summary>
		/// IM to tell the user to go to an URL. Put a text message in the
        /// message field, and put the url with a trailing \0 in the binary
        /// bucket.
		/// </summary>
		GotoUrl = 28,

		/// <summary>
		/// A message generated by a script which we don't want to
        /// be sent through e-mail.  Similar to IM_FROM_TASK, but
        /// it is shown as an alert on the viewer.
		/// </summary>
		FromTaskAsAlert = 31,

		/// <summary>
		/// IM from group officer to all group members.
		/// </summary>
		GroupNotice = 32,
		GroupNoticeInventoryAccepted = 33,
		GroupNoticeInventoryDeclined = 34,

		GroupInvitationAccept = 35,
        GroupInvitationDecline = 36,

		GroupNoticeRequested = 37,

		FriendshipOffered = 38,
        FriendshipAccepted = 39,
        FriendshipDeclined_DEPRECATED = 40,

		TypingStart = 41,
		TypingStop = 42,
    };

	public class ImprovedInstantMessageMessage : Message
    {
        public Guid AgentId { get; set; }
        public Guid SessionId { get; set; }

        public bool IsFromGroup { get; set; }
        public Guid ToAgentId { get; set; }
        public UInt32 ParentEstateId { get; set; }
        public Guid RegionId { get; set; }
        public Vector3 Position { get; set; }
        public OnlineMode OnlineMode { get; set; }
        public DialogType DialogType { get; set; }
        public Guid Id { get; set; }
        public UInt32 Timestamp { get; set; }
        public string FromAgentName { get; set; }
        public string MessageText { get; set; }
        public byte[] BinaryBucket { get; set; }

        public ImprovedInstantMessageMessage()
        {
            MessageId = MessageId.ImprovedInstantMessage;
            Flags = 0;
        }

        public ImprovedInstantMessageMessage (Guid agentId,
                                              Guid sessionId,
                                              bool isFromGroup,
                                              Guid toAgentId,
                                              UInt32 parentEstateId,
                                              Guid regionId,
                                              Vector3 position,
                                              OnlineMode onlineMode,
                                              DialogType dialogType,
                                              Guid id,
                                              UInt32 timestamp,
                                              string fromAgentName,
                                              string messageText,
                                              byte[] binaryBucket)
        {
            MessageId = MessageId.ImprovedInstantMessage;
            Flags = PacketFlags.Reliable; //TODO: Could be zerocoded

            AgentId        = agentId;
            SessionId      = sessionId;
            IsFromGroup    = isFromGroup;
            ToAgentId      = toAgentId;
            ParentEstateId = parentEstateId;
            RegionId       = regionId;
            Position       = position;
            OnlineMode     = onlineMode;
            DialogType     = dialogType;
            Id             = id;
            Timestamp      = timestamp;
            FromAgentName  = fromAgentName;
            MessageText    = messageText;
            BinaryBucket   = binaryBucket;
        }


        #region Serialise
        public override int GetSerializedLength()
        {
            return base.GetSerializedLength()
                   + 16     // AgentId
                   + 16     // SessionId
                   +  1     // IsFromGroup
                   + 16     // ToAgentId
                   +  4     // ParentEstateId
                   + 16     // RegionId
                   + 12     // Position
                   +  1     // OnlineMode
                   +  1     // DialogType
                   + 16     // Id
                   +  4     // Timestamp
                   + BinarySerializer.GetSerializedLength(FromAgentName, 1) // FromAgentName
                   + BinarySerializer.GetSerializedLength(MessageText,   2)  // MessageText
                   + 2      // BinaryBucket length
                   + (BinaryBucket == null ? 0 : BinaryBucket.Length); // BinaryBucket
        }

        public override int Serialize (byte[] buffer, int offset, int length)
        {
            // TODO: LL code truncates at MTU

            int o = offset;
            o += base.Serialize (buffer, offset, length);

            o = BinarySerializer.Serialize    (AgentId,        buffer, o, length);
            o = BinarySerializer.Serialize    (SessionId,      buffer, o, length);
            
            buffer[o++] =               (byte)(IsFromGroup ? 1 : 0); // TODO: Add boolean to BinarySerializer to make sure these are consistent
            o = BinarySerializer.Serialize    (ToAgentId,      buffer, o, length);
            o = BinarySerializer.Serialize_Le (ParentEstateId, buffer, o, length);
            o = BinarySerializer.Serialize    (RegionId,       buffer, o, length);
            o = BinarySerializer.Serialize_Le (Position,       buffer, o, length);
            buffer[o++] = (byte)OnlineMode;
            buffer[o++] = (byte)DialogType;
            o = BinarySerializer.Serialize    (Id,             buffer, o, length);
            o = BinarySerializer.Serialize_Le (Timestamp,      buffer, o, length);
            o = BinarySerializer.Serialize    (FromAgentName,  buffer, o, length, 1);
            o = BinarySerializer.Serialize    (MessageText,    buffer, o, length, 2);

            UInt16 len = (UInt16)(BinaryBucket == null ? 0 : BinaryBucket.Length);
            o = BinarySerializer.Serialize_Le (len, buffer, o, length);
            if (len > 0)
            {
                Array.Copy (BinaryBucket, 0, buffer, o, len);
                o += len;
            }

            return o - offset;
        }
        #endregion Serialise

        #region DeSerialise
        protected override void DeSerialise(byte[] buf, ref int o, int length)
        {
            AgentId        = BinarySerializer.DeSerializeGuid      (buf, ref o, length);
            SessionId      = BinarySerializer.DeSerializeGuid      (buf, ref o, length);
            IsFromGroup    = buf[o++] != 0; // TODO: Put this in BinarySerializer to make sure that it is consistent
            ToAgentId      = BinarySerializer.DeSerializeGuid      (buf, ref o, length);
            ParentEstateId = BinarySerializer.DeSerializeUInt32_Le (buf, ref o, length);
            RegionId       = BinarySerializer.DeSerializeGuid      (buf, ref o, length);
            Position       = BinarySerializer.DeSerializeVector3   (buf, ref o, length);
            OnlineMode     = (OnlineMode)buf[o++];
            DialogType     = (DialogType)buf[o++];
            Id             = BinarySerializer.DeSerializeGuid      (buf, ref o, length);
            Timestamp      = BinarySerializer.DeSerializeUInt32_Le (buf, ref o, length);
            FromAgentName  = BinarySerializer.DeSerializeString    (buf, ref o, length, 1);
            MessageText    = BinarySerializer.DeSerializeString    (buf, ref o, length, 2);
            UInt16 len     = BinarySerializer.DeSerializeUInt16_Le (buf, ref o, length);
            BinaryBucket   = new byte[len];
            Array.Copy(buf, o, BinaryBucket, 0, len);
            o += len;
        }
        #endregion DeSerialise

        public override string ToString()
        {
            return   $"{base.ToString()}:\n"
                   + $"    AgentId={AgentId}\n"
                   + $"    SessionId={SessionId}\n"
                   + $"    IsFromGroup={IsFromGroup}\n"
                   + $"    ToAgentId={ToAgentId}\n"
                   + $"    ParentEstateId=0x{ParentEstateId:x8}\n"
                   + $"    RegionId={RegionId}\n"
                   + $"    Position={Position}\n"
                   + $"    OnlineMode={OnlineMode}\n"
                   + $"    DialogType={DialogType}\n"
                   + $"    Id={Id}\n"
                   + $"    Timestamp={Timestamp}\n" // TODO: Create an extension in either DateTime or DateTimeOffset to convert UNIX timestamps
                   + $"    FromAgentName={FromAgentName}\n"
                   + $"    MessageText={MessageText}\n"
                   + $"    BinaryBucket= ({BinaryBucket.Length})\n{BinaryBucket.ToHexDump()}"
                ;
        }
    }
}
