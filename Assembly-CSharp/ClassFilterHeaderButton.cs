using System;
using UnityEngine;

// Token: 0x02000704 RID: 1796
public class ClassFilterHeaderButton : PegUIElement
{
	// Token: 0x060049CB RID: 18891 RVA: 0x001613E5 File Offset: 0x0015F5E5
	protected override void Awake()
	{
		this.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.HandleRelease();
		});
		base.Awake();
	}

	// Token: 0x060049CC RID: 18892 RVA: 0x00161404 File Offset: 0x0015F604
	public void HandleRelease()
	{
		CollectionManagerDisplay.Get().HideDeckHelpPopup();
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		bool flag = taggedDeck != null;
		if (this.m_buttons == null)
		{
			this.m_buttons = this.m_classFilterTray.GetComponentsInChildren<ClassFilterButton>();
		}
		if (!flag)
		{
			this.m_container.SetDefaults();
		}
		else
		{
			TAG_CLASS @class = taggedDeck.GetClass();
			this.m_container.SetClass(@class);
		}
		this.m_classFilterTray.ToggleTraySlider(true, this.m_showTwoRowsBone, true);
		NotificationManager.Get().DestroyAllPopUps();
	}

	// Token: 0x060049CD RID: 18893 RVA: 0x00161494 File Offset: 0x0015F694
	public void SetMode(CollectionManagerDisplay.ViewMode mode, TAG_CLASS? classTag)
	{
		if (mode == CollectionManagerDisplay.ViewMode.CARD_BACKS)
		{
			this.m_headerText.Text = GameStrings.Get("GLUE_COLLECTION_MANAGER_CARD_BACKS_TITLE");
		}
		else if (mode == CollectionManagerDisplay.ViewMode.HERO_SKINS)
		{
			this.m_headerText.Text = GameStrings.Get("GLUE_COLLECTION_MANAGER_HERO_SKINS_TITLE");
		}
		else if (classTag != null)
		{
			this.m_headerText.Text = GameStrings.GetClassName(classTag.Value);
		}
		else
		{
			this.m_headerText.Text = string.Empty;
		}
	}

	// Token: 0x040030FC RID: 12540
	public SlidingTray m_classFilterTray;

	// Token: 0x040030FD RID: 12541
	public UberText m_headerText;

	// Token: 0x040030FE RID: 12542
	public Transform m_showTwoRowsBone;

	// Token: 0x040030FF RID: 12543
	public ClassFilterButtonContainer m_container;

	// Token: 0x04003100 RID: 12544
	private ClassFilterButton[] m_buttons;
}
