using System;
using UnityEngine;

// Token: 0x0200070A RID: 1802
public class ClassFilterButtonContainer : MonoBehaviour
{
	// Token: 0x060049F5 RID: 18933 RVA: 0x00161D1F File Offset: 0x0015FF1F
	public void Awake()
	{
		this.m_neutralIndex = this.GetIndex(TAG_CLASS.INVALID);
	}

	// Token: 0x060049F6 RID: 18934 RVA: 0x00161D30 File Offset: 0x0015FF30
	public int GetNumVisibleClasses()
	{
		int num = 0;
		CollectionPageManager pageManager = CollectionManagerDisplay.Get().m_pageManager;
		for (int i = 0; i < this.m_classTags.Length; i++)
		{
			bool flag = pageManager.GetNumPagesForClass(this.m_classTags[i]) > 0;
			if (flag)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060049F7 RID: 18935 RVA: 0x00161D80 File Offset: 0x0015FF80
	private void SetCardBacksEnabled(bool enabled)
	{
		this.m_cardBacksButton.SetEnabled(enabled);
		this.m_cardBacksDisabled.SetActive(!enabled);
	}

	// Token: 0x060049F8 RID: 18936 RVA: 0x00161D9D File Offset: 0x0015FF9D
	private void SetHeroSkinsEnabled(bool enabled)
	{
		this.m_heroSkinsButton.SetEnabled(enabled);
		this.m_heroSkinsDisabled.SetActive(!enabled);
	}

	// Token: 0x060049F9 RID: 18937 RVA: 0x00161DBC File Offset: 0x0015FFBC
	public void SetDefaults()
	{
		this.SetCardBacksEnabled(true);
		this.SetHeroSkinsEnabled(true);
		for (int i = 0; i < this.m_classTags.Length; i++)
		{
			this.m_classButtons[i].SetClass(default(TAG_CLASS?), this.m_inactiveMaterial);
		}
		CollectionPageManager pageManager = CollectionManagerDisplay.Get().m_pageManager;
		int num = 0;
		for (int j = 0; j < this.m_classTags.Length; j++)
		{
			bool flag = pageManager.GetNumPagesForClass(this.m_classTags[j]) > 0;
			if (flag)
			{
				this.m_classButtons[num].SetClass(new TAG_CLASS?(this.m_classTags[j]), this.m_classMaterials[j]);
				int numNewCardsForClass = CollectionManagerDisplay.Get().m_pageManager.GetNumNewCardsForClass(this.m_classTags[j]);
				this.m_classButtons[num].SetNewCardCount(numNewCardsForClass);
				num++;
			}
		}
	}

	// Token: 0x060049FA RID: 18938 RVA: 0x00161E9C File Offset: 0x0016009C
	public void SetClass(TAG_CLASS classTag)
	{
		int count = CardBackManager.Get().GetCardBacksOwned().Count;
		int count2 = CollectionManager.Get().GetBestHeroesIOwn(classTag).Count;
		this.SetCardBacksEnabled(count > 1);
		this.SetHeroSkinsEnabled(count2 > 1);
		int index = this.GetIndex(classTag);
		for (int i = 0; i < this.m_classTags.Length; i++)
		{
			this.m_classButtons[i].SetClass(default(TAG_CLASS?), this.m_inactiveMaterial);
			this.m_classButtons[i].SetNewCardCount(0);
		}
		this.m_classButtons[0].SetClass(new TAG_CLASS?(classTag), this.m_classMaterials[index]);
		this.m_classButtons[0].SetNewCardCount(CollectionManagerDisplay.Get().m_pageManager.GetNumNewCardsForClass(classTag));
		this.m_classButtons[1].SetClass(new TAG_CLASS?(TAG_CLASS.INVALID), this.m_classMaterials[this.m_neutralIndex]);
		this.m_classButtons[1].SetNewCardCount(CollectionManagerDisplay.Get().m_pageManager.GetNumNewCardsForClass(TAG_CLASS.INVALID));
	}

	// Token: 0x060049FB RID: 18939 RVA: 0x00161FA0 File Offset: 0x001601A0
	private int GetIndex(TAG_CLASS classTag)
	{
		int result = 0;
		for (int i = 0; i < this.m_classTags.Length; i++)
		{
			if (this.m_classTags[i] == classTag)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	// Token: 0x04003111 RID: 12561
	public int m_rowSize = 5;

	// Token: 0x04003112 RID: 12562
	public TAG_CLASS[] m_classTags;

	// Token: 0x04003113 RID: 12563
	public ClassFilterButton[] m_classButtons;

	// Token: 0x04003114 RID: 12564
	public Material[] m_classMaterials;

	// Token: 0x04003115 RID: 12565
	public Material m_inactiveMaterial;

	// Token: 0x04003116 RID: 12566
	public Material m_templateMaterial;

	// Token: 0x04003117 RID: 12567
	public PegUIElement m_cardBacksButton;

	// Token: 0x04003118 RID: 12568
	public PegUIElement m_heroSkinsButton;

	// Token: 0x04003119 RID: 12569
	public GameObject m_cardBacksDisabled;

	// Token: 0x0400311A RID: 12570
	public GameObject m_heroSkinsDisabled;

	// Token: 0x0400311B RID: 12571
	private int m_neutralIndex;
}
