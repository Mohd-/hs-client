using System;
using UnityEngine;

// Token: 0x020007CF RID: 1999
public class ShowAllCardsTab : MonoBehaviour
{
	// Token: 0x06004E1A RID: 19994 RVA: 0x001739A8 File Offset: 0x00171BA8
	private void Awake()
	{
		this.m_showAllCardsCheckBox.SetButtonText(GameStrings.Get("GLUE_COLLECTION_SHOW_ALL_CARDS"));
		this.m_includePremiumsCheckBox.SetButtonText(GameStrings.Get("GLUE_COLLECTION_INCLUDE_PREMIUMS"));
	}

	// Token: 0x06004E1B RID: 19995 RVA: 0x001739E0 File Offset: 0x00171BE0
	private void Start()
	{
		this.m_includePremiumsCheckBox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleIncludePremiums));
		this.m_showAllCardsCheckBox.SetChecked(false);
		this.m_includePremiumsCheckBox.SetChecked(false);
		this.m_includePremiumsCheckBox.gameObject.SetActive(false);
	}

	// Token: 0x06004E1C RID: 19996 RVA: 0x00173A2F File Offset: 0x00171C2F
	public bool IsShowAllChecked()
	{
		return this.m_showAllCardsCheckBox.IsChecked();
	}

	// Token: 0x06004E1D RID: 19997 RVA: 0x00173A3C File Offset: 0x00171C3C
	private void ToggleIncludePremiums(UIEvent e)
	{
		bool flag = this.m_includePremiumsCheckBox.IsChecked();
		CollectionManagerDisplay.Get().ShowPremiumCardsNotOwned(flag);
		if (flag)
		{
			SoundManager.Get().LoadAndPlay("checkbox_toggle_on", base.gameObject);
		}
		else
		{
			SoundManager.Get().LoadAndPlay("checkbox_toggle_off", base.gameObject);
		}
	}

	// Token: 0x04003520 RID: 13600
	public CheckBox m_showAllCardsCheckBox;

	// Token: 0x04003521 RID: 13601
	public CheckBox m_includePremiumsCheckBox;
}
