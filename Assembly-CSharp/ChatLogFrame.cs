using System;
using UnityEngine;

// Token: 0x0200059E RID: 1438
public class ChatLogFrame : MonoBehaviour
{
	// Token: 0x1700049C RID: 1180
	// (get) Token: 0x060040AD RID: 16557 RVA: 0x00137D7E File Offset: 0x00135F7E
	// (set) Token: 0x060040AE RID: 16558 RVA: 0x00137D88 File Offset: 0x00135F88
	public BnetPlayer Receiver
	{
		get
		{
			return this.m_receiver;
		}
		set
		{
			if (this.m_receiver == value)
			{
				return;
			}
			this.m_receiver = value;
			if (this.m_receiver != null)
			{
				this.m_playerIcon.SetPlayer(this.m_receiver);
				this.UpdateReceiver();
				this.m_chatLog.Receiver = this.m_receiver;
			}
		}
	}

	// Token: 0x060040AF RID: 16559 RVA: 0x00137DDD File Offset: 0x00135FDD
	private void Awake()
	{
		this.InitPlayerIcon();
	}

	// Token: 0x060040B0 RID: 16560 RVA: 0x00137DE5 File Offset: 0x00135FE5
	private void Start()
	{
		this.UpdateLayout();
	}

	// Token: 0x060040B1 RID: 16561 RVA: 0x00137DED File Offset: 0x00135FED
	public void UpdateLayout()
	{
		this.OnResize();
	}

	// Token: 0x060040B2 RID: 16562 RVA: 0x00137DF8 File Offset: 0x00135FF8
	private void InitPlayerIcon()
	{
		this.m_playerIcon = Object.Instantiate<PlayerIcon>(this.m_Prefabs.m_PlayerIcon);
		this.m_playerIcon.transform.parent = base.transform;
		TransformUtil.CopyWorld(this.m_playerIcon, this.m_Bones.m_PlayerIcon);
		SceneUtils.SetLayer(this.m_playerIcon, base.gameObject.layer);
	}

	// Token: 0x060040B3 RID: 16563 RVA: 0x00137E60 File Offset: 0x00136060
	private void OnResize()
	{
		float viewWindowMaxValue = this.m_chatLog.messageFrames.ViewWindowMaxValue;
		this.m_chatLog.messageFrames.transform.position = (this.m_Bones.m_MessagesTopLeft.position + this.m_Bones.m_MessagesBottomRight.position) / 2f;
		Vector3 vector = this.m_Bones.m_MessagesBottomRight.localPosition - this.m_Bones.m_MessagesTopLeft.localPosition;
		this.m_chatLog.messageFrames.ClipSize = new Vector2(vector.x, Math.Abs(vector.y));
		this.m_chatLog.messageFrames.ViewWindowMaxValue = viewWindowMaxValue;
		this.m_chatLog.messageFrames.ScrollValue = Mathf.Clamp01(this.m_chatLog.messageFrames.ScrollValue);
		this.m_chatLog.OnResize();
	}

	// Token: 0x060040B4 RID: 16564 RVA: 0x00137F51 File Offset: 0x00136151
	private void UpdateReceiver()
	{
		this.m_playerIcon.UpdateIcon();
		this.m_NameText.Text = FriendUtils.GetUniqueNameWithColor(this.m_receiver);
	}

	// Token: 0x0400293B RID: 10555
	public ChatLogFrameBones m_Bones;

	// Token: 0x0400293C RID: 10556
	public ChatLogFramePrefabs m_Prefabs;

	// Token: 0x0400293D RID: 10557
	public UberText m_NameText;

	// Token: 0x0400293E RID: 10558
	public ChatLog m_chatLog;

	// Token: 0x0400293F RID: 10559
	private PlayerIcon m_playerIcon;

	// Token: 0x04002940 RID: 10560
	private BnetPlayer m_receiver;
}
