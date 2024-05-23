using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200051D RID: 1309
public class MobileChatLogFrame : MonoBehaviour
{
	// Token: 0x1400001D RID: 29
	// (add) Token: 0x06003CBD RID: 15549 RVA: 0x00125E2C File Offset: 0x0012402C
	// (remove) Token: 0x06003CBE RID: 15550 RVA: 0x00125E45 File Offset: 0x00124045
	public event Action InputCanceled;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x06003CBF RID: 15551 RVA: 0x00125E5E File Offset: 0x0012405E
	// (remove) Token: 0x06003CC0 RID: 15552 RVA: 0x00125E77 File Offset: 0x00124077
	public event Action CloseButtonReleased;

	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x06003CC1 RID: 15553 RVA: 0x00125E90 File Offset: 0x00124090
	public bool HasFocus
	{
		get
		{
			return this.inputTextField.Active;
		}
	}

	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x06003CC2 RID: 15554 RVA: 0x00125E9D File Offset: 0x0012409D
	// (set) Token: 0x06003CC3 RID: 15555 RVA: 0x00125EA8 File Offset: 0x001240A8
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
			this.Focus(this.receiver != null);
			if (this.receiver != null)
			{
				this.playerIcon.SetPlayer(this.receiver);
				this.UpdateReceiver();
				this.chatLog.Receiver = this.receiver;
			}
		}
	}

	// Token: 0x06003CC4 RID: 15556 RVA: 0x00125F10 File Offset: 0x00124110
	private void Awake()
	{
		this.playerIcon = this.playerIconRef.Spawn<PlayerIcon>();
		this.UpdateBackgroundCollider();
		this.inputTextField.maxCharacters = 512;
		this.inputTextField.Submitted += new Action<string>(this.OnInputComplete);
		this.inputTextField.Canceled += new Action(this.OnInputCanceled);
		this.closeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCloseButtonReleased));
		BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
	}

	// Token: 0x06003CC5 RID: 15557 RVA: 0x00125FA2 File Offset: 0x001241A2
	private void Start()
	{
		if (this.receiver == null)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003CC6 RID: 15558 RVA: 0x00125FBB File Offset: 0x001241BB
	private void OnDestroy()
	{
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
	}

	// Token: 0x06003CC7 RID: 15559 RVA: 0x00125FD4 File Offset: 0x001241D4
	private void Update()
	{
	}

	// Token: 0x06003CC8 RID: 15560 RVA: 0x00125FD8 File Offset: 0x001241D8
	public void Focus(bool focus)
	{
		if (focus && !this.inputTextField.Active)
		{
			this.inputTextField.Activate();
		}
		else if (!focus && this.inputTextField.Active)
		{
			this.inputTextField.Deactivate();
		}
	}

	// Token: 0x06003CC9 RID: 15561 RVA: 0x0012602C File Offset: 0x0012422C
	public void SetWorldRect(float x, float y, float width, float height)
	{
		bool activeSelf = base.gameObject.activeSelf;
		base.gameObject.SetActive(true);
		float viewWindowMaxValue = this.messageFrames.ViewWindowMaxValue;
		this.window.SetEntireSize(width, height);
		Bounds bounds = TransformUtil.ComputeSetPointBounds(this.window);
		Vector3 vector = TransformUtil.ComputeWorldPoint(bounds, new Vector3(0f, 1f, 0f));
		Vector3 vector2 = new Vector3(x, y, vector.z) - vector;
		base.transform.Translate(vector2);
		this.messageFrames.transform.position = (this.messageInfo.messagesTopLeft.position + this.messageInfo.messagesBottomRight.position) / 2f;
		Vector3 vector3 = this.messageInfo.messagesBottomRight.position - this.messageInfo.messagesTopLeft.position;
		this.messageFrames.ClipSize = new Vector2(vector3.x, Math.Abs(vector3.y));
		this.messageFrames.ViewWindowMaxValue = viewWindowMaxValue;
		this.messageFrames.ScrollValue = Mathf.Clamp01(this.messageFrames.ScrollValue);
		this.chatLog.OnResize();
		this.UpdateBackgroundCollider();
		this.UpdateFollowers();
		base.gameObject.SetActive(activeSelf);
	}

	// Token: 0x06003CCA RID: 15562 RVA: 0x00126188 File Offset: 0x00124388
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		if (changelist.FindChange(this.receiver) == null)
		{
			return;
		}
		this.UpdateReceiver();
	}

	// Token: 0x06003CCB RID: 15563 RVA: 0x001261AF File Offset: 0x001243AF
	private void OnCloseButtonReleased(UIEvent e)
	{
		if (this.CloseButtonReleased != null)
		{
			this.CloseButtonReleased.Invoke();
		}
	}

	// Token: 0x06003CCC RID: 15564 RVA: 0x001261C8 File Offset: 0x001243C8
	private bool IsFullScreenKeyboard()
	{
		return ChatMgr.Get().KeyboardRect.height == (float)Screen.height;
	}

	// Token: 0x06003CCD RID: 15565 RVA: 0x001261F0 File Offset: 0x001243F0
	private void OnInputComplete(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return;
		}
		if (!BnetWhisperMgr.Get().SendWhisper(this.receiver, input))
		{
			this.chatLog.OnWhisperFailed();
		}
		ChatMgr.Get().AddRecentWhisperPlayerToTop(this.receiver);
	}

	// Token: 0x06003CCE RID: 15566 RVA: 0x0012623A File Offset: 0x0012443A
	private void OnInputCanceled()
	{
		if (this.InputCanceled != null)
		{
			this.InputCanceled.Invoke();
		}
	}

	// Token: 0x06003CCF RID: 15567 RVA: 0x00126254 File Offset: 0x00124454
	private void UpdateReceiver()
	{
		this.playerIcon.UpdateIcon();
		string text = (!this.receiver.IsOnline()) ? "999999ff" : "5ecaf0ff";
		string bestName = this.receiver.GetBestName();
		this.nameText.Text = string.Format("<color=#{0}>{1}</color>", text, bestName);
		if (this.receiver != null && this.receiver.IsDisplayable() && this.receiver.IsOnline())
		{
			MedalInfoTranslator rankPresenceField = RankMgr.Get().GetRankPresenceField(this.receiver.GetBestGameAccount());
			if (rankPresenceField == null || rankPresenceField.GetCurrentMedal(rankPresenceField.IsBestCurrentRankWild()).rank == 25)
			{
				this.medalPatch.SetActive(false);
				this.playerIcon.Show();
			}
			else
			{
				this.playerIcon.Hide();
				this.medal.SetEnabled(false);
				this.medal.SetMedal(rankPresenceField, false);
				this.medal.SetFormat(this.medal.IsBestCurrentRankWild());
				this.medalPatch.SetActive(true);
			}
		}
		else if (!this.receiver.IsOnline())
		{
			this.medalPatch.SetActive(false);
			this.playerIcon.Show();
		}
	}

	// Token: 0x06003CD0 RID: 15568 RVA: 0x0012639C File Offset: 0x0012459C
	private void UpdateBackgroundCollider()
	{
		Renderer[] componentsInChildren = this.window.GetComponentsInChildren<Renderer>();
		Bounds bounds = Enumerable.Aggregate<Renderer, Bounds>(componentsInChildren, new Bounds(base.transform.position, Vector3.zero), delegate(Bounds aggregate, Renderer renderer)
		{
			aggregate.Encapsulate(renderer.bounds);
			return aggregate;
		});
		Vector3 vector = base.transform.InverseTransformPoint(bounds.min);
		Vector3 vector2 = base.transform.InverseTransformPoint(bounds.max);
		BoxCollider boxCollider = base.GetComponent<BoxCollider>();
		if (boxCollider == null)
		{
			boxCollider = base.gameObject.AddComponent<BoxCollider>();
		}
		boxCollider.center = (vector + vector2) / 2f + Vector3.forward;
		boxCollider.size = vector2 - vector;
	}

	// Token: 0x06003CD1 RID: 15569 RVA: 0x00126466 File Offset: 0x00124666
	private void UpdateFollowers()
	{
		this.followers.UpdateFollowPosition();
	}

	// Token: 0x040026A7 RID: 9895
	public Spawner playerIconRef;

	// Token: 0x040026A8 RID: 9896
	public TouchList messageFrames;

	// Token: 0x040026A9 RID: 9897
	public MobileChatLogFrame.InputInfo inputInfo;

	// Token: 0x040026AA RID: 9898
	public TextField inputTextField;

	// Token: 0x040026AB RID: 9899
	public MobileChatLogFrame.MessageInfo messageInfo;

	// Token: 0x040026AC RID: 9900
	public NineSliceElement window;

	// Token: 0x040026AD RID: 9901
	public UberText nameText;

	// Token: 0x040026AE RID: 9902
	public UIBButton closeButton;

	// Token: 0x040026AF RID: 9903
	public MobileChatNotification notifications;

	// Token: 0x040026B0 RID: 9904
	public GameObject medalPatch;

	// Token: 0x040026B1 RID: 9905
	public TournamentMedal medal;

	// Token: 0x040026B2 RID: 9906
	public ChatLog chatLog;

	// Token: 0x040026B3 RID: 9907
	public MobileChatLogFrame.Followers followers;

	// Token: 0x040026B4 RID: 9908
	private PlayerIcon playerIcon;

	// Token: 0x040026B5 RID: 9909
	private BnetPlayer receiver;

	// Token: 0x02000553 RID: 1363
	[Serializable]
	public class MessageInfo
	{
		// Token: 0x04002812 RID: 10258
		public Transform messagesTopLeft;

		// Token: 0x04002813 RID: 10259
		public Transform messagesBottomRight;
	}

	// Token: 0x02000554 RID: 1364
	[Serializable]
	public class InputInfo
	{
		// Token: 0x04002814 RID: 10260
		public Transform inputTopLeft;

		// Token: 0x04002815 RID: 10261
		public Transform inputBottomRight;
	}

	// Token: 0x02000555 RID: 1365
	[Serializable]
	public class Followers
	{
		// Token: 0x06003E8B RID: 16011 RVA: 0x0012E870 File Offset: 0x0012CA70
		public void UpdateFollowPosition()
		{
			this.playerInfoFollower.UpdateFollowPosition();
			this.closeButtonFollower.UpdateFollowPosition();
			this.bubbleFollower.UpdateFollowPosition();
		}

		// Token: 0x04002816 RID: 10262
		public UIBFollowObject playerInfoFollower;

		// Token: 0x04002817 RID: 10263
		public UIBFollowObject closeButtonFollower;

		// Token: 0x04002818 RID: 10264
		public UIBFollowObject bubbleFollower;
	}
}
