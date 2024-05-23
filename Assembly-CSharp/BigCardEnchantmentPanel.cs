using System;
using UnityEngine;

// Token: 0x020008AD RID: 2221
public class BigCardEnchantmentPanel : MonoBehaviour
{
	// Token: 0x0600542D RID: 21549 RVA: 0x00192C8C File Offset: 0x00190E8C
	private void Awake()
	{
		this.m_initialScale = base.transform.localScale;
		this.m_initialBackgroundHeight = this.m_Background.GetComponentInChildren<MeshRenderer>().bounds.size.z;
		this.m_initialBackgroundScale = this.m_Background.transform.localScale;
	}

	// Token: 0x0600542E RID: 21550 RVA: 0x00192CE8 File Offset: 0x00190EE8
	public void SetEnchantment(Entity enchantment)
	{
		this.m_enchantment = enchantment;
		string cardId = this.m_enchantment.GetCardId();
		DefLoader.Get().LoadCardDef(cardId, new DefLoader.LoadDefCallback<CardDef>(this.OnCardDefLoaded), null, new CardPortraitQuality(1, this.m_enchantment.GetPremiumType()));
	}

	// Token: 0x0600542F RID: 21551 RVA: 0x00192D31 File Offset: 0x00190F31
	public void Show()
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_shown = true;
		base.gameObject.SetActive(true);
		this.UpdateLayout();
	}

	// Token: 0x06005430 RID: 21552 RVA: 0x00192D58 File Offset: 0x00190F58
	public void Hide()
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06005431 RID: 21553 RVA: 0x00192D7C File Offset: 0x00190F7C
	public void ResetScale()
	{
		base.transform.localScale = this.m_initialScale;
		this.m_Background.transform.localScale = this.m_initialBackgroundScale;
	}

	// Token: 0x06005432 RID: 21554 RVA: 0x00192DB0 File Offset: 0x00190FB0
	public bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x06005433 RID: 21555 RVA: 0x00192DB8 File Offset: 0x00190FB8
	public float GetHeight()
	{
		return this.m_Background.GetComponentInChildren<MeshRenderer>().bounds.size.z;
	}

	// Token: 0x06005434 RID: 21556 RVA: 0x00192DE8 File Offset: 0x00190FE8
	private void OnCardDefLoaded(string cardId, CardDef cardDef, object callbackData)
	{
		if (cardDef != null)
		{
			if (cardDef.GetEnchantmentPortrait() != null)
			{
				this.m_Actor.GetMeshRenderer().material = cardDef.GetEnchantmentPortrait();
			}
			else
			{
				this.m_Actor.SetPortraitTextureOverride(cardDef.GetPortraitTexture());
			}
		}
		this.m_HeaderText.Text = this.m_enchantment.GetName();
		this.m_BodyText.Text = this.m_enchantment.GetCardTextInHand();
	}

	// Token: 0x06005435 RID: 21557 RVA: 0x00192E6C File Offset: 0x0019106C
	private void UpdateLayout()
	{
		this.m_HeaderText.UpdateNow();
		this.m_BodyText.UpdateNow();
		Bounds bounds = this.m_Actor.GetMeshRenderer().bounds;
		Bounds textWorldSpaceBounds = this.m_HeaderText.GetTextWorldSpaceBounds();
		Bounds textWorldSpaceBounds2 = this.m_BodyText.GetTextWorldSpaceBounds();
		float z = bounds.min.z;
		float z2 = bounds.max.z;
		float z3 = textWorldSpaceBounds.min.z;
		float z4 = textWorldSpaceBounds.max.z;
		float z5 = textWorldSpaceBounds2.min.z;
		float z6 = textWorldSpaceBounds2.max.z;
		float num = Mathf.Min(Mathf.Min(z, z3), z5);
		float num2 = Mathf.Max(Mathf.Max(z2, z4), z6);
		float num3 = num2 - num + 0.1f;
		base.transform.localScale = this.m_initialScale;
		base.transform.localEulerAngles = Vector3.zero;
		TransformUtil.SetLocalScaleZ(this.m_Background, this.m_initialBackgroundScale.z * (num3 / this.m_initialBackgroundHeight));
	}

	// Token: 0x04003A5D RID: 14941
	public Actor m_Actor;

	// Token: 0x04003A5E RID: 14942
	public UberText m_HeaderText;

	// Token: 0x04003A5F RID: 14943
	public UberText m_BodyText;

	// Token: 0x04003A60 RID: 14944
	public GameObject m_Background;

	// Token: 0x04003A61 RID: 14945
	private Entity m_enchantment;

	// Token: 0x04003A62 RID: 14946
	private Vector3 m_initialScale;

	// Token: 0x04003A63 RID: 14947
	private float m_initialBackgroundHeight;

	// Token: 0x04003A64 RID: 14948
	private Vector3 m_initialBackgroundScale;

	// Token: 0x04003A65 RID: 14949
	private bool m_shown;
}
