using System;
using UnityEngine;

// Token: 0x020006F0 RID: 1776
public class CollectionCardCount : MonoBehaviour
{
	// Token: 0x06004939 RID: 18745 RVA: 0x0015DF85 File Offset: 0x0015C185
	private void Start()
	{
	}

	// Token: 0x0600493A RID: 18746 RVA: 0x0015DF87 File Offset: 0x0015C187
	private void Update()
	{
	}

	// Token: 0x0600493B RID: 18747 RVA: 0x0015DF89 File Offset: 0x0015C189
	private void Awake()
	{
	}

	// Token: 0x0600493C RID: 18748 RVA: 0x0015DF8B File Offset: 0x0015C18B
	public void SetCount(int cardCount)
	{
		this.m_count = cardCount;
		this.UpdateVisibility();
	}

	// Token: 0x0600493D RID: 18749 RVA: 0x0015DF9A File Offset: 0x0015C19A
	public int GetCount()
	{
		return this.m_count;
	}

	// Token: 0x0600493E RID: 18750 RVA: 0x0015DFA2 File Offset: 0x0015C1A2
	public void Show()
	{
		this.UpdateVisibility();
	}

	// Token: 0x0600493F RID: 18751 RVA: 0x0015DFAA File Offset: 0x0015C1AA
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06004940 RID: 18752 RVA: 0x0015DFB8 File Offset: 0x0015C1B8
	private void UpdateVisibility()
	{
		if (this.m_count <= 1)
		{
			this.Hide();
			return;
		}
		base.gameObject.SetActive(true);
		Color textColor = (!CollectionManager.Get().IsShowingWildTheming(null)) ? this.m_standardTextColor : this.m_wildTextColor;
		this.m_countText.TextColor = textColor;
		Material material = (!CollectionManager.Get().IsShowingWildTheming(null)) ? this.m_standardBorderMaterial : this.m_wildBorderMaterial;
		this.m_wideBorder.GetComponent<Renderer>().material = material;
		this.m_border.GetComponent<Renderer>().material = material;
		GameObject gameObject;
		GameObject gameObject2;
		if (this.m_count < 10)
		{
			gameObject = this.m_border;
			gameObject2 = this.m_wideBorder;
			this.m_countText.Text = GameStrings.Format("GLUE_COLLECTION_CARD_COUNT", new object[]
			{
				this.m_count
			});
		}
		else
		{
			gameObject = this.m_wideBorder;
			gameObject2 = this.m_border;
			this.m_countText.Text = GameStrings.Get("GLUE_COLLECTION_CARD_COUNT_LARGE");
		}
		gameObject.SetActive(true);
		gameObject2.SetActive(false);
	}

	// Token: 0x04003054 RID: 12372
	public UberText m_countText;

	// Token: 0x04003055 RID: 12373
	public GameObject m_border;

	// Token: 0x04003056 RID: 12374
	public GameObject m_wideBorder;

	// Token: 0x04003057 RID: 12375
	public Color m_standardTextColor;

	// Token: 0x04003058 RID: 12376
	public Color m_wildTextColor;

	// Token: 0x04003059 RID: 12377
	public Material m_standardBorderMaterial;

	// Token: 0x0400305A RID: 12378
	public Material m_wildBorderMaterial;

	// Token: 0x0400305B RID: 12379
	private int m_count = 1;
}
