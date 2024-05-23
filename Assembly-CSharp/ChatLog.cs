using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using bgs;
using UnityEngine;

// Token: 0x0200055A RID: 1370
public class ChatLog : MonoBehaviour
{
	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x06003F02 RID: 16130 RVA: 0x00132A40 File Offset: 0x00130C40
	// (set) Token: 0x06003F03 RID: 16131 RVA: 0x00132A48 File Offset: 0x00130C48
	public BnetPlayer Receiver
	{
		get
		{
			return this.receiver;
		}
		set
		{
			if (this.receiver == value)
			{
				return;
			}
			this.receiver = value;
			if (this.receiver != null)
			{
				this.UpdateMessages();
				if (!this.receiver.IsOnline())
				{
					this.AddOfflineMessage();
				}
				this.messageFrames.ScrollValue = 1f;
			}
		}
	}

	// Token: 0x06003F04 RID: 16132 RVA: 0x00132AA0 File Offset: 0x00130CA0
	private void Awake()
	{
		this.CreateMessagesCamera();
		if (this.notifications != null)
		{
			this.notifications.Notified += this.OnNotified;
		}
		BnetWhisperMgr.Get().AddWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
		BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
	}

	// Token: 0x06003F05 RID: 16133 RVA: 0x00132B0C File Offset: 0x00130D0C
	private void OnDestroy()
	{
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		BnetWhisperMgr.Get().RemoveWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
		if (this.notifications != null)
		{
			this.notifications.Notified -= this.OnNotified;
		}
	}

	// Token: 0x06003F06 RID: 16134 RVA: 0x00132B6F File Offset: 0x00130D6F
	public void OnResize()
	{
		this.UpdateMessagesCamera();
	}

	// Token: 0x06003F07 RID: 16135 RVA: 0x00132B77 File Offset: 0x00130D77
	public void OnWhisperFailed()
	{
		this.AddOfflineMessage();
	}

	// Token: 0x06003F08 RID: 16136 RVA: 0x00132B80 File Offset: 0x00130D80
	private void OnWhisper(BnetWhisper whisper, object userData)
	{
		if (this.receiver == null || !WhisperUtil.IsSpeakerOrReceiver(this.receiver, whisper))
		{
			return;
		}
		this.AddWhisperMessage(whisper);
		this.messageFrames.ScrollValue = 1f;
	}

	// Token: 0x06003F09 RID: 16137 RVA: 0x00132BC4 File Offset: 0x00130DC4
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		BnetPlayerChange bnetPlayerChange = changelist.FindChange(this.receiver);
		if (bnetPlayerChange == null)
		{
			return;
		}
		BnetPlayer oldPlayer = bnetPlayerChange.GetOldPlayer();
		BnetPlayer newPlayer = bnetPlayerChange.GetNewPlayer();
		if (oldPlayer == null || oldPlayer.IsOnline() != newPlayer.IsOnline())
		{
			if (newPlayer.IsOnline())
			{
				this.AddOnlineMessage();
			}
			else
			{
				this.AddOfflineMessage();
			}
		}
	}

	// Token: 0x06003F0A RID: 16138 RVA: 0x00132C26 File Offset: 0x00130E26
	private void OnNotified(string text)
	{
		this.AddSystemMessage(text, this.messageInfo.notificationColor);
	}

	// Token: 0x06003F0B RID: 16139 RVA: 0x00132C3C File Offset: 0x00130E3C
	private void UpdateMessages()
	{
		List<MobileChatLogMessageFrame> list = Enumerable.ToList<MobileChatLogMessageFrame>(Enumerable.Select<ITouchListItem, MobileChatLogMessageFrame>(this.messageFrames, (ITouchListItem i) => i.GetComponent<MobileChatLogMessageFrame>()));
		this.messageFrames.Clear();
		foreach (MobileChatLogMessageFrame mobileChatLogMessageFrame in list)
		{
			Object.Destroy(mobileChatLogMessageFrame.gameObject);
		}
		List<BnetWhisper> whispersWithPlayer = BnetWhisperMgr.Get().GetWhispersWithPlayer(this.receiver);
		if (whispersWithPlayer != null && whispersWithPlayer.Count > 0)
		{
			int num = Mathf.Max(whispersWithPlayer.Count - 500, 0);
			for (int j = num; j < whispersWithPlayer.Count; j++)
			{
				BnetWhisper whisper = whispersWithPlayer[j];
				this.AddWhisperMessage(whisper);
			}
		}
		this.OnMessagesAdded();
	}

	// Token: 0x06003F0C RID: 16140 RVA: 0x00132D3C File Offset: 0x00130F3C
	private void AddWhisperMessage(BnetWhisper whisper)
	{
		string message = ChatUtils.GetMessage(whisper);
		MobileChatLogMessageFrame prefab = (!WhisperUtil.IsSpeaker(this.receiver, whisper)) ? this.prefabs.myMessage : this.prefabs.theirMessage;
		MobileChatLogMessageFrame item = this.CreateMessage(prefab, message);
		this.messageFrames.Add(item);
	}

	// Token: 0x06003F0D RID: 16141 RVA: 0x00132D94 File Offset: 0x00130F94
	private void AddMyMessage(string message)
	{
		string message2 = ChatUtils.GetMessage(message);
		MobileChatLogMessageFrame item = this.CreateMessage(this.prefabs.myMessage, message2);
		this.messageFrames.Add(item);
		this.OnMessagesAdded();
	}

	// Token: 0x06003F0E RID: 16142 RVA: 0x00132DD0 File Offset: 0x00130FD0
	private void AddSystemMessage(string message, Color color)
	{
		MobileChatLogMessageFrame item = this.CreateMessage(this.prefabs.systemMessage, message, color);
		this.messageFrames.Add(item);
		this.OnMessagesAdded();
	}

	// Token: 0x06003F0F RID: 16143 RVA: 0x00132E04 File Offset: 0x00131004
	private void AddOnlineMessage()
	{
		string message = GameStrings.Format("GLOBAL_CHAT_RECEIVER_ONLINE", new object[]
		{
			this.receiver.GetBestName()
		});
		this.AddSystemMessage(message, this.messageInfo.infoColor);
	}

	// Token: 0x06003F10 RID: 16144 RVA: 0x00132E44 File Offset: 0x00131044
	private void AddOfflineMessage()
	{
		string message = GameStrings.Format("GLOBAL_CHAT_RECEIVER_OFFLINE", new object[]
		{
			this.receiver.GetBestName()
		});
		this.AddSystemMessage(message, this.messageInfo.errorColor);
	}

	// Token: 0x06003F11 RID: 16145 RVA: 0x00132E84 File Offset: 0x00131084
	private void OnMessagesAdded()
	{
		if (this.messageFrames.Count > 500)
		{
			ITouchListItem touchListItem = this.messageFrames[0];
			this.messageFrames.RemoveAt(0);
			Object.Destroy(touchListItem.gameObject);
		}
		this.messageFrames.ScrollValue = 1f;
	}

	// Token: 0x06003F12 RID: 16146 RVA: 0x00132EDC File Offset: 0x001310DC
	private MobileChatLogMessageFrame CreateMessage(MobileChatLogMessageFrame prefab, string message)
	{
		MobileChatLogMessageFrame mobileChatLogMessageFrame = Object.Instantiate<MobileChatLogMessageFrame>(prefab);
		mobileChatLogMessageFrame.Width = this.messageFrames.ClipSize.x - this.messageFrames.padding.x - 10f;
		mobileChatLogMessageFrame.Message = message;
		SceneUtils.SetLayer(mobileChatLogMessageFrame, GameLayer.BattleNetChat);
		return mobileChatLogMessageFrame;
	}

	// Token: 0x06003F13 RID: 16147 RVA: 0x00132F30 File Offset: 0x00131130
	private MobileChatLogMessageFrame CreateMessage(MobileChatLogMessageFrame prefab, string message, Color color)
	{
		MobileChatLogMessageFrame mobileChatLogMessageFrame = this.CreateMessage(prefab, message);
		mobileChatLogMessageFrame.Color = color;
		return mobileChatLogMessageFrame;
	}

	// Token: 0x06003F14 RID: 16148 RVA: 0x00132F50 File Offset: 0x00131150
	private void CreateMessagesCamera()
	{
		this.messagesCamera = new GameObject("MessagesCamera")
		{
			transform = 
			{
				parent = this.messageFrames.transform,
				localPosition = new Vector3(0f, 0f, -100f)
			}
		}.AddComponent<Camera>();
		this.messagesCamera.orthographic = true;
		this.messagesCamera.depth = (float)(BnetBar.CameraDepth + 1);
		this.messagesCamera.clearFlags = 3;
		this.messagesCamera.cullingMask = GameLayer.BattleNetChat.LayerBit();
		this.UpdateMessagesCamera();
	}

	// Token: 0x06003F15 RID: 16149 RVA: 0x00132FEC File Offset: 0x001311EC
	private Bounds GetBoundsFromGameObject(GameObject go)
	{
		Renderer component = go.GetComponent<Renderer>();
		if (component != null)
		{
			return component.bounds;
		}
		Collider component2 = go.GetComponent<Collider>();
		if (component2 != null)
		{
			return component2.bounds;
		}
		return default(Bounds);
	}

	// Token: 0x06003F16 RID: 16150 RVA: 0x00133038 File Offset: 0x00131238
	private void UpdateMessagesCamera()
	{
		Camera bnetCamera = BaseUI.Get().GetBnetCamera();
		Bounds boundsFromGameObject = this.GetBoundsFromGameObject(this.cameraTarget);
		Vector3 vector = bnetCamera.WorldToScreenPoint(boundsFromGameObject.min);
		Vector3 vector2 = bnetCamera.WorldToScreenPoint(boundsFromGameObject.max);
		this.messagesCamera.pixelRect = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
		this.messagesCamera.orthographicSize = this.messagesCamera.rect.height * bnetCamera.orthographicSize;
	}

	// Token: 0x06003F17 RID: 16151 RVA: 0x001330DC File Offset: 0x001312DC
	[Conditional("CHATLOG_DEBUG")]
	private void AssignMessageFrameNames()
	{
		for (int i = 0; i < this.messageFrames.Count; i++)
		{
			MobileChatLogMessageFrame component = this.messageFrames[i].GetComponent<MobileChatLogMessageFrame>();
			component.name = string.Format("MessageFrame {0} ({1})", i, component.Message);
		}
	}

	// Token: 0x04002853 RID: 10323
	private const int maxMessageFrames = 500;

	// Token: 0x04002854 RID: 10324
	private const GameLayer messageLayer = GameLayer.BattleNetChat;

	// Token: 0x04002855 RID: 10325
	public TouchList messageFrames;

	// Token: 0x04002856 RID: 10326
	public GameObject cameraTarget;

	// Token: 0x04002857 RID: 10327
	public ChatLog.Prefabs prefabs;

	// Token: 0x04002858 RID: 10328
	public ChatLog.MessageInfo messageInfo;

	// Token: 0x04002859 RID: 10329
	public MobileChatNotification notifications;

	// Token: 0x0400285A RID: 10330
	private BnetPlayer receiver;

	// Token: 0x0400285B RID: 10331
	private Camera messagesCamera;

	// Token: 0x0200058C RID: 1420
	[Serializable]
	public class Prefabs
	{
		// Token: 0x04002918 RID: 10520
		public MobileChatLogMessageFrame myMessage;

		// Token: 0x04002919 RID: 10521
		public MobileChatLogMessageFrame theirMessage;

		// Token: 0x0400291A RID: 10522
		public MobileChatLogMessageFrame systemMessage;
	}

	// Token: 0x0200058E RID: 1422
	[Serializable]
	public class MessageInfo
	{
		// Token: 0x0400291E RID: 10526
		public Color infoColor = Color.yellow;

		// Token: 0x0400291F RID: 10527
		public Color errorColor = Color.red;

		// Token: 0x04002920 RID: 10528
		public Color notificationColor = Color.cyan;
	}
}
