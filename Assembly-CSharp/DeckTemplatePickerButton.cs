using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000788 RID: 1928
[CustomEditClass]
public class DeckTemplatePickerButton : PegUIElement
{
	// Token: 0x06004C92 RID: 19602 RVA: 0x0016C6AB File Offset: 0x0016A8AB
	public void SetIsStarterDeck(bool starter)
	{
		this.m_isStarterDeck = starter;
	}

	// Token: 0x06004C93 RID: 19603 RVA: 0x0016C6B4 File Offset: 0x0016A8B4
	public bool IsStarterDeck()
	{
		return this.m_isStarterDeck;
	}

	// Token: 0x06004C94 RID: 19604 RVA: 0x0016C6BC File Offset: 0x0016A8BC
	public void SetSelected(bool selected)
	{
		if (this.m_selectGlow != null)
		{
			this.m_selectGlow.SetActive(selected);
		}
	}

	// Token: 0x06004C95 RID: 19605 RVA: 0x0016C6DB File Offset: 0x0016A8DB
	public void SetTitleText(string titleText)
	{
		if (this.m_title != null)
		{
			this.m_title.Text = titleText;
		}
	}

	// Token: 0x06004C96 RID: 19606 RVA: 0x0016C6FC File Offset: 0x0016A8FC
	public void SetCardCountText(int count)
	{
		this.m_ownedCardCount = count;
		int deckSize = CollectionManager.Get().GetDeckSize();
		foreach (UberText uberText in this.m_cardCountTexts)
		{
			uberText.Text = string.Format("{0}/{1}", count, deckSize);
		}
		bool flag = count < DeckTemplatePickerButton.s_MinimumRecommendedSize && !this.m_isStarterDeck;
		if (this.m_incompleteTextRibbon != null)
		{
			this.m_incompleteTextRibbon.SetActive(flag);
		}
		if (this.m_completeTextRibbon != null)
		{
			this.m_completeTextRibbon.SetActive(!flag);
		}
	}

	// Token: 0x06004C97 RID: 19607 RVA: 0x0016C7D4 File Offset: 0x0016A9D4
	public int GetOwnedCardCount()
	{
		return this.m_ownedCardCount;
	}

	// Token: 0x06004C98 RID: 19608 RVA: 0x0016C7DC File Offset: 0x0016A9DC
	public void SetDeckTexture(string texturePath)
	{
		if (this.m_deckTexture == null)
		{
			return;
		}
		Texture texture = string.IsNullOrEmpty(texturePath) ? null : AssetLoader.Get().LoadTexture(FileUtils.GameAssetPathToName(texturePath), false);
		if (texture != null)
		{
			this.m_deckTexture.material.mainTexture = texture;
		}
	}

	// Token: 0x0400335E RID: 13150
	public MeshRenderer m_deckTexture;

	// Token: 0x0400335F RID: 13151
	public MeshRenderer m_packRibbon;

	// Token: 0x04003360 RID: 13152
	public GameObject m_selectGlow;

	// Token: 0x04003361 RID: 13153
	public UberText m_title;

	// Token: 0x04003362 RID: 13154
	public List<UberText> m_cardCountTexts = new List<UberText>();

	// Token: 0x04003363 RID: 13155
	public GameObject m_incompleteTextRibbon;

	// Token: 0x04003364 RID: 13156
	public GameObject m_completeTextRibbon;

	// Token: 0x04003365 RID: 13157
	public static readonly int s_MinimumRecommendedSize = 25;

	// Token: 0x04003366 RID: 13158
	private bool m_isStarterDeck;

	// Token: 0x04003367 RID: 13159
	private int m_ownedCardCount;
}
