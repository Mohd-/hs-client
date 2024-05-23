using System;
using UnityEngine;

// Token: 0x020008CE RID: 2254
public class EmoteOption : MonoBehaviour
{
	// Token: 0x06005505 RID: 21765 RVA: 0x00197578 File Offset: 0x00195778
	private void Awake()
	{
		this.UpdateEmoteType();
		this.m_Text.gameObject.SetActive(false);
		this.m_Backplate.enabled = false;
		this.m_startingScale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x06005506 RID: 21766 RVA: 0x001975CC File Offset: 0x001957CC
	private void Update()
	{
		if (EmoteHandler.Get().EmoteSpamBlocked())
		{
			if (!this.m_textIsGrey)
			{
				this.m_textIsGrey = true;
				this.m_Text.TextColor = new Color(0.5372549f, 0.5372549f, 0.5372549f);
			}
		}
		else if (this.m_textIsGrey)
		{
			this.m_textIsGrey = false;
			this.m_Text.TextColor = new Color(0f, 0f, 0f);
		}
	}

	// Token: 0x06005507 RID: 21767 RVA: 0x00197650 File Offset: 0x00195850
	public void DoClick()
	{
		EmoteHandler.Get().ResetTimeSinceLastEmote();
		GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(this.m_currentEmoteType);
		Network.Get().SendEmote(this.m_EmoteType);
		EmoteHandler.Get().HideEmotes();
	}

	// Token: 0x06005508 RID: 21768 RVA: 0x0019769C File Offset: 0x0019589C
	public void Enable()
	{
		this.m_Backplate.enabled = true;
		this.m_Text.gameObject.SetActive(true);
		base.GetComponent<Collider>().enabled = true;
		iTween.Stop(base.gameObject);
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			this.m_startingScale,
			"time",
			0.5f,
			"ignoretimescale",
			true,
			"easetype",
			iTween.EaseType.easeOutElastic
		}));
	}

	// Token: 0x06005509 RID: 21769 RVA: 0x00197744 File Offset: 0x00195944
	public void Disable()
	{
		base.GetComponent<Collider>().enabled = false;
		iTween.Stop(base.gameObject);
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			Vector3.zero,
			"time",
			0.1f,
			"ignoretimescale",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"FinishDisable"
		}));
	}

	// Token: 0x0600550A RID: 21770 RVA: 0x001977F0 File Offset: 0x001959F0
	public void HandleMouseOut()
	{
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			this.m_startingScale,
			"time",
			0.2f,
			"ignoretimescale",
			true
		}));
	}

	// Token: 0x0600550B RID: 21771 RVA: 0x00197850 File Offset: 0x00195A50
	public void HandleMouseOver()
	{
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			this.m_startingScale * 1.1f,
			"time",
			0.2f,
			"ignoretimescale",
			true
		}));
	}

	// Token: 0x0600550C RID: 21772 RVA: 0x001978BC File Offset: 0x00195ABC
	public void UpdateEmoteType()
	{
		if (this.ShouldUseFallbackEmote())
		{
			this.m_currentEmoteType = this.m_FallbackEmoteType;
			this.m_currentStringTag = this.m_FallbackStringTag;
		}
		else
		{
			this.m_currentEmoteType = this.m_EmoteType;
			this.m_currentStringTag = this.m_StringTag;
		}
		this.m_Text.Text = GameStrings.Get(this.m_currentStringTag);
	}

	// Token: 0x0600550D RID: 21773 RVA: 0x00197920 File Offset: 0x00195B20
	private bool ShouldUseFallbackEmote()
	{
		Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
		if (friendlySidePlayer == null)
		{
			return false;
		}
		Card heroCard = friendlySidePlayer.GetHeroCard();
		if (heroCard == null)
		{
			return false;
		}
		EmoteEntry emoteEntry = heroCard.GetEmoteEntry(this.m_EmoteType);
		return emoteEntry == null && heroCard.GetEmoteEntry(this.m_FallbackEmoteType) != null;
	}

	// Token: 0x0600550E RID: 21774 RVA: 0x00197980 File Offset: 0x00195B80
	private void FinishDisable()
	{
		if (base.GetComponent<Collider>().enabled)
		{
			return;
		}
		this.m_Backplate.enabled = false;
		this.m_Text.gameObject.SetActive(false);
	}

	// Token: 0x04003B2B RID: 15147
	public EmoteType m_EmoteType;

	// Token: 0x04003B2C RID: 15148
	public string m_StringTag;

	// Token: 0x04003B2D RID: 15149
	public EmoteType m_FallbackEmoteType;

	// Token: 0x04003B2E RID: 15150
	public string m_FallbackStringTag;

	// Token: 0x04003B2F RID: 15151
	public MeshRenderer m_Backplate;

	// Token: 0x04003B30 RID: 15152
	public UberText m_Text;

	// Token: 0x04003B31 RID: 15153
	private EmoteType m_currentEmoteType;

	// Token: 0x04003B32 RID: 15154
	private string m_currentStringTag;

	// Token: 0x04003B33 RID: 15155
	private Vector3 m_startingScale;

	// Token: 0x04003B34 RID: 15156
	private bool m_textIsGrey;
}
