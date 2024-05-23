using System;
using UnityEngine;

// Token: 0x02000846 RID: 2118
public class CardTypeBanner : MonoBehaviour
{
	// Token: 0x06005144 RID: 20804 RVA: 0x00183BA9 File Offset: 0x00181DA9
	private void Awake()
	{
		CardTypeBanner.s_instance = this;
	}

	// Token: 0x06005145 RID: 20805 RVA: 0x00183BB1 File Offset: 0x00181DB1
	private void OnDestroy()
	{
		CardTypeBanner.s_instance = null;
	}

	// Token: 0x06005146 RID: 20806 RVA: 0x00183BB9 File Offset: 0x00181DB9
	private void Update()
	{
		if (this.m_actor != null)
		{
			this.UpdatePosition();
		}
	}

	// Token: 0x06005147 RID: 20807 RVA: 0x00183BD2 File Offset: 0x00181DD2
	public static CardTypeBanner Get()
	{
		return CardTypeBanner.s_instance;
	}

	// Token: 0x06005148 RID: 20808 RVA: 0x00183BD9 File Offset: 0x00181DD9
	public bool IsShown()
	{
		return this.m_actor;
	}

	// Token: 0x06005149 RID: 20809 RVA: 0x00183BE6 File Offset: 0x00181DE6
	public void Show(Actor a)
	{
		this.m_actor = a;
		this.ShowImpl();
	}

	// Token: 0x0600514A RID: 20810 RVA: 0x00183BF5 File Offset: 0x00181DF5
	public void Hide()
	{
		this.m_actor = null;
		this.HideImpl();
	}

	// Token: 0x0600514B RID: 20811 RVA: 0x00183C04 File Offset: 0x00181E04
	public void Hide(Actor actor)
	{
		if (this.m_actor == actor)
		{
			this.Hide();
		}
	}

	// Token: 0x0600514C RID: 20812 RVA: 0x00183C1D File Offset: 0x00181E1D
	public CardDef GetCardDef()
	{
		if (this.m_actor != null)
		{
			return this.m_actor.GetCardDef();
		}
		return null;
	}

	// Token: 0x0600514D RID: 20813 RVA: 0x00183C40 File Offset: 0x00181E40
	private void ShowImpl()
	{
		this.m_root.gameObject.SetActive(true);
		TAG_CARDTYPE cardType = this.m_actor.GetEntity().GetCardType();
		this.m_text.gameObject.SetActive(true);
		this.m_text.Text = GameStrings.GetCardTypeName(cardType);
		switch (cardType)
		{
		case TAG_CARDTYPE.MINION:
			this.m_text.TextColor = this.MINION_COLOR;
			this.m_minionBanner.SetActive(true);
			break;
		case TAG_CARDTYPE.SPELL:
			this.m_text.TextColor = this.SPELL_COLOR;
			this.m_spellBanner.SetActive(true);
			break;
		case TAG_CARDTYPE.WEAPON:
			this.m_text.TextColor = this.WEAPON_COLOR;
			this.m_weaponBanner.SetActive(true);
			break;
		}
		this.UpdatePosition();
	}

	// Token: 0x0600514E RID: 20814 RVA: 0x00183D1C File Offset: 0x00181F1C
	private void HideImpl()
	{
		this.m_root.gameObject.SetActive(false);
	}

	// Token: 0x0600514F RID: 20815 RVA: 0x00183D30 File Offset: 0x00181F30
	private void UpdatePosition()
	{
		GameObject cardTypeBannerAnchor = this.m_actor.GetCardTypeBannerAnchor();
		this.m_root.transform.position = cardTypeBannerAnchor.transform.position;
	}

	// Token: 0x04003801 RID: 14337
	public GameObject m_root;

	// Token: 0x04003802 RID: 14338
	public UberText m_text;

	// Token: 0x04003803 RID: 14339
	public GameObject m_spellBanner;

	// Token: 0x04003804 RID: 14340
	public GameObject m_minionBanner;

	// Token: 0x04003805 RID: 14341
	public GameObject m_weaponBanner;

	// Token: 0x04003806 RID: 14342
	private static CardTypeBanner s_instance;

	// Token: 0x04003807 RID: 14343
	private Actor m_actor;

	// Token: 0x04003808 RID: 14344
	private readonly Color MINION_COLOR = new Color(0.15294118f, 0.1254902f, 0.03529412f);

	// Token: 0x04003809 RID: 14345
	private readonly Color SPELL_COLOR = new Color(0.8745098f, 0.7882353f, 0.5254902f);

	// Token: 0x0400380A RID: 14346
	private readonly Color WEAPON_COLOR = new Color(0.8745098f, 0.7882353f, 0.5254902f);
}
