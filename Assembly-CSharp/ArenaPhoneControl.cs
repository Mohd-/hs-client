using System;
using UnityEngine;

// Token: 0x020007F0 RID: 2032
public class ArenaPhoneControl : MonoBehaviour
{
	// Token: 0x06004EFB RID: 20219 RVA: 0x00176D40 File Offset: 0x00174F40
	private void Awake()
	{
		this.m_CurrentMode = ArenaPhoneControl.ControlMode.ChooseHero;
		this.m_ButtonCollider.enabled = false;
		this.m_ChooseText.Text = GameStrings.Get("GLUE_CHOOSE_YOUR_HERO");
	}

	// Token: 0x06004EFC RID: 20220 RVA: 0x00176D75 File Offset: 0x00174F75
	[ContextMenu("ChooseHero")]
	public void SetModeChooseHero()
	{
		this.SetMode(ArenaPhoneControl.ControlMode.ChooseHero);
	}

	// Token: 0x06004EFD RID: 20221 RVA: 0x00176D7E File Offset: 0x00174F7E
	[ContextMenu("ChooseCard")]
	public void SetModeChooseCard()
	{
		this.SetMode(ArenaPhoneControl.ControlMode.ChooseCard);
	}

	// Token: 0x06004EFE RID: 20222 RVA: 0x00176D87 File Offset: 0x00174F87
	[ContextMenu("CardCountViewDeck")]
	public void SetModeCardCountViewDeck()
	{
		this.SetMode(ArenaPhoneControl.ControlMode.CardCountViewDeck);
	}

	// Token: 0x06004EFF RID: 20223 RVA: 0x00176D90 File Offset: 0x00174F90
	[ContextMenu("ViewDeck")]
	public void SetModeViewDeck()
	{
		this.SetMode(ArenaPhoneControl.ControlMode.ViewDeck);
	}

	// Token: 0x06004F00 RID: 20224 RVA: 0x00176D9C File Offset: 0x00174F9C
	public void SetMode(ArenaPhoneControl.ControlMode mode)
	{
		if (mode == this.m_CurrentMode)
		{
			return;
		}
		switch (mode)
		{
		case ArenaPhoneControl.ControlMode.ChooseHero:
			this.m_ViewDeckButton.SetActive(false);
			this.m_ButtonCollider.enabled = false;
			this.m_ChooseText.Text = GameStrings.Get("GLUE_CHOOSE_YOUR_HERO");
			if (this.m_CurrentMode == ArenaPhoneControl.ControlMode.CardCountViewDeck)
			{
				this.RotateTo(180f, 0f);
			}
			break;
		case ArenaPhoneControl.ControlMode.ChooseCard:
			this.m_ViewDeckButton.SetActive(false);
			this.m_ButtonCollider.enabled = false;
			this.m_ChooseText.Text = GameStrings.Get("GLUE_DRAFT_INSTRUCTIONS");
			if (this.m_CurrentMode == ArenaPhoneControl.ControlMode.CardCountViewDeck)
			{
				this.RotateTo(180f, 0f);
			}
			break;
		case ArenaPhoneControl.ControlMode.CardCountViewDeck:
			this.m_ButtonCollider.center = this.m_CountAndViewDeckCollCenter;
			this.m_ButtonCollider.size = this.m_CountAndViewDeckCollSize;
			this.m_ButtonCollider.enabled = true;
			this.RotateTo(0f, 180f);
			break;
		case ArenaPhoneControl.ControlMode.ViewDeck:
			this.m_ButtonCollider.center = this.m_ViewDeckCollCenter;
			this.m_ButtonCollider.size = this.m_ViewDeckCollSize;
			this.m_ViewDeckButton.SetActive(true);
			this.m_ButtonCollider.enabled = true;
			if (this.m_CurrentMode == ArenaPhoneControl.ControlMode.CardCountViewDeck)
			{
				this.RotateTo(180f, 0f);
			}
			break;
		case ArenaPhoneControl.ControlMode.Rewards:
			this.m_ViewDeckButton.SetActive(false);
			this.m_ButtonCollider.enabled = false;
			this.m_ChooseText.Text = string.Empty;
			if (this.m_CurrentMode == ArenaPhoneControl.ControlMode.CardCountViewDeck)
			{
				this.RotateTo(180f, 0f);
			}
			break;
		}
		this.m_CurrentMode = mode;
	}

	// Token: 0x06004F01 RID: 20225 RVA: 0x00176F64 File Offset: 0x00175164
	private void RotateTo(float rotFrom, float rotTo)
	{
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
		{
			"from",
			rotFrom,
			"to",
			rotTo,
			"time",
			1f,
			"easetype",
			iTween.EaseType.easeOutBounce,
			"onupdate",
			delegate(object val)
			{
				base.transform.localRotation = Quaternion.Euler((float)val, 0f, 0f);
			}
		}));
	}

	// Token: 0x040035CF RID: 13775
	public UberText m_ChooseText;

	// Token: 0x040035D0 RID: 13776
	public GameObject m_ViewDeckButton;

	// Token: 0x040035D1 RID: 13777
	public BoxCollider m_ButtonCollider;

	// Token: 0x040035D2 RID: 13778
	public Vector3 m_CountAndViewDeckCollCenter;

	// Token: 0x040035D3 RID: 13779
	public Vector3 m_CountAndViewDeckCollSize;

	// Token: 0x040035D4 RID: 13780
	public Vector3 m_ViewDeckCollCenter;

	// Token: 0x040035D5 RID: 13781
	public Vector3 m_ViewDeckCollSize;

	// Token: 0x040035D6 RID: 13782
	private ArenaPhoneControl.ControlMode m_CurrentMode;

	// Token: 0x020007F1 RID: 2033
	public enum ControlMode
	{
		// Token: 0x040035D8 RID: 13784
		ChooseHero,
		// Token: 0x040035D9 RID: 13785
		ChooseCard,
		// Token: 0x040035DA RID: 13786
		CardCountViewDeck,
		// Token: 0x040035DB RID: 13787
		ViewDeck,
		// Token: 0x040035DC RID: 13788
		Rewards
	}
}
