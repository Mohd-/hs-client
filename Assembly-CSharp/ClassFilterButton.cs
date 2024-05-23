using System;
using UnityEngine;

// Token: 0x020006F4 RID: 1780
public class ClassFilterButton : PegUIElement
{
	// Token: 0x0600495C RID: 18780 RVA: 0x0015EC48 File Offset: 0x0015CE48
	protected override void Awake()
	{
		this.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.HandleRelease();
		});
		base.Awake();
	}

	// Token: 0x0600495D RID: 18781 RVA: 0x0015EC64 File Offset: 0x0015CE64
	public void HandleRelease()
	{
		switch (this.m_tabViewMode)
		{
		case CollectionManagerDisplay.ViewMode.CARDS:
		{
			TAG_CLASS? @class = this.m_class;
			if (@class != null)
			{
				CollectionManagerDisplay.Get().m_pageManager.JumpToCollectionClassPage(this.m_class.Value);
			}
			break;
		}
		case CollectionManagerDisplay.ViewMode.HERO_SKINS:
			CollectionManagerDisplay.Get().SetViewMode(CollectionManagerDisplay.ViewMode.HERO_SKINS, null);
			break;
		case CollectionManagerDisplay.ViewMode.CARD_BACKS:
			CollectionManagerDisplay.Get().SetViewMode(CollectionManagerDisplay.ViewMode.CARD_BACKS, null);
			break;
		}
		base.GetComponentInParent<SlidingTray>().HideTray();
	}

	// Token: 0x0600495E RID: 18782 RVA: 0x0015ECF0 File Offset: 0x0015CEF0
	public void SetClass(TAG_CLASS? classTag, Material material)
	{
		this.m_class = classTag;
		base.GetComponent<Renderer>().material = material;
		TAG_CLASS? @class = this.m_class;
		bool flag = @class == null;
		base.GetComponent<Renderer>().enabled = !flag;
		if (this.m_newCardCount != null)
		{
			this.m_newCardCount.SetActive(!flag);
		}
		if (this.m_disabled != null)
		{
			this.m_disabled.SetActive(flag);
		}
	}

	// Token: 0x0600495F RID: 18783 RVA: 0x0015ED70 File Offset: 0x0015CF70
	public void SetNewCardCount(int count)
	{
		if (this.m_newCardCount != null)
		{
			this.m_newCardCount.SetActive(count > 0);
		}
		if (count > 0 && this.m_newCardCountText != null)
		{
			this.m_newCardCountText.Text = GameStrings.Format("GLUE_COLLECTION_NEW_CARD_CALLOUT", new object[]
			{
				count
			});
		}
	}

	// Token: 0x0400307E RID: 12414
	public GameObject m_newCardCount;

	// Token: 0x0400307F RID: 12415
	public UberText m_newCardCountText;

	// Token: 0x04003080 RID: 12416
	public GameObject m_disabled;

	// Token: 0x04003081 RID: 12417
	public CollectionManagerDisplay.ViewMode m_tabViewMode;

	// Token: 0x04003082 RID: 12418
	private TAG_CLASS? m_class;
}
