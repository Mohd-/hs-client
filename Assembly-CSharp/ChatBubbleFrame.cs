using System;
using bgs;
using UnityEngine;

// Token: 0x02000515 RID: 1301
public class ChatBubbleFrame : MonoBehaviour
{
	// Token: 0x06003BFE RID: 15358 RVA: 0x00121F2F File Offset: 0x0012012F
	private void Awake()
	{
		BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
	}

	// Token: 0x06003BFF RID: 15359 RVA: 0x00121F48 File Offset: 0x00120148
	private void OnDestroy()
	{
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
	}

	// Token: 0x06003C00 RID: 15360 RVA: 0x00121F61 File Offset: 0x00120161
	public BnetWhisper GetWhisper()
	{
		return this.m_whisper;
	}

	// Token: 0x06003C01 RID: 15361 RVA: 0x00121F69 File Offset: 0x00120169
	public void SetWhisper(BnetWhisper whisper)
	{
		if (this.m_whisper == whisper)
		{
			return;
		}
		this.m_whisper = whisper;
		this.UpdateWhisper();
	}

	// Token: 0x06003C02 RID: 15362 RVA: 0x00121F85 File Offset: 0x00120185
	public bool DoesMessageFit()
	{
		return !this.m_MessageText.IsEllipsized();
	}

	// Token: 0x06003C03 RID: 15363 RVA: 0x00121F98 File Offset: 0x00120198
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(WhisperUtil.GetTheirGameAccountId(this.m_whisper));
		BnetPlayerChange bnetPlayerChange = changelist.FindChange(player);
		if (bnetPlayerChange == null)
		{
			return;
		}
		BnetPlayer oldPlayer = bnetPlayerChange.GetOldPlayer();
		BnetPlayer newPlayer = bnetPlayerChange.GetNewPlayer();
		if (oldPlayer != null && oldPlayer.IsOnline() == newPlayer.IsOnline())
		{
			return;
		}
		this.UpdateWhisper();
	}

	// Token: 0x06003C04 RID: 15364 RVA: 0x00121FF8 File Offset: 0x001201F8
	private void UpdateWhisper()
	{
		if (this.m_whisper == null)
		{
			return;
		}
		BnetGameAccountId speakerId = this.m_whisper.GetSpeakerId();
		bool flag = speakerId == BnetPresenceMgr.Get().GetMyGameAccountId();
		if (flag)
		{
			this.m_MyDecoration.SetActive(true);
			this.m_TheirDecoration.SetActive(false);
			BnetPlayer receiver = WhisperUtil.GetReceiver(this.m_whisper);
			this.m_NameText.Text = GameStrings.Format("GLOBAL_CHAT_BUBBLE_RECEIVER_NAME", new object[]
			{
				receiver.GetBestName()
			});
		}
		else
		{
			this.m_MyDecoration.SetActive(false);
			this.m_TheirDecoration.SetActive(true);
			BnetPlayer speaker = WhisperUtil.GetSpeaker(this.m_whisper);
			if (speaker.IsOnline())
			{
				this.m_NameText.TextColor = GameColors.PLAYER_NAME_ONLINE;
			}
			else
			{
				this.m_NameText.TextColor = GameColors.PLAYER_NAME_OFFLINE;
			}
			this.m_NameText.Text = speaker.GetBestName();
		}
		this.m_MessageText.Text = ChatUtils.GetMessage(this.m_whisper);
	}

	// Token: 0x04002662 RID: 9826
	public GameObject m_VisualRoot;

	// Token: 0x04002663 RID: 9827
	public GameObject m_MyDecoration;

	// Token: 0x04002664 RID: 9828
	public GameObject m_TheirDecoration;

	// Token: 0x04002665 RID: 9829
	public UberText m_NameText;

	// Token: 0x04002666 RID: 9830
	public UberText m_MessageText;

	// Token: 0x04002667 RID: 9831
	public Vector3_MobileOverride m_ScaleOverride;

	// Token: 0x04002668 RID: 9832
	private BnetWhisper m_whisper;
}
