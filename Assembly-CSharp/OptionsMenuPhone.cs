using System;
using UnityEngine;

// Token: 0x02000A85 RID: 2693
public class OptionsMenuPhone : MonoBehaviour
{
	// Token: 0x06005D9F RID: 23967 RVA: 0x001C114F File Offset: 0x001BF34F
	private void Start()
	{
		this.m_doneButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.m_optionsMenu.Hide(true);
		});
	}

	// Token: 0x04004563 RID: 17763
	public OptionsMenu m_optionsMenu;

	// Token: 0x04004564 RID: 17764
	public UIBButton m_doneButton;

	// Token: 0x04004565 RID: 17765
	public GameObject m_mainContentsPanel;
}
