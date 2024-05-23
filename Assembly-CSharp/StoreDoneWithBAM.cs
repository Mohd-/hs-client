using System;
using System.Collections.Generic;

// Token: 0x0200040A RID: 1034
public class StoreDoneWithBAM : UIBPopup
{
	// Token: 0x060034A7 RID: 13479 RVA: 0x0010677E File Offset: 0x0010497E
	private void Awake()
	{
		this.m_okayButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOkayPressed));
	}

	// Token: 0x060034A8 RID: 13480 RVA: 0x00106799 File Offset: 0x00104999
	public void RegisterOkayListener(StoreDoneWithBAM.ButtonPressedListener listener)
	{
		if (this.m_okayListeners.Contains(listener))
		{
			return;
		}
		this.m_okayListeners.Add(listener);
	}

	// Token: 0x060034A9 RID: 13481 RVA: 0x001067B9 File Offset: 0x001049B9
	public void RemoveOkayListener(StoreDoneWithBAM.ButtonPressedListener listener)
	{
		this.m_okayListeners.Remove(listener);
	}

	// Token: 0x060034AA RID: 13482 RVA: 0x001067C8 File Offset: 0x001049C8
	private void OnOkayPressed(UIEvent e)
	{
		this.Hide(true);
		StoreDoneWithBAM.ButtonPressedListener[] array = this.m_okayListeners.ToArray();
		foreach (StoreDoneWithBAM.ButtonPressedListener buttonPressedListener in array)
		{
			buttonPressedListener();
		}
	}

	// Token: 0x040020CE RID: 8398
	public UIBButton m_okayButton;

	// Token: 0x040020CF RID: 8399
	public UberText m_headlineText;

	// Token: 0x040020D0 RID: 8400
	public UberText m_messageText;

	// Token: 0x040020D1 RID: 8401
	private List<StoreDoneWithBAM.ButtonPressedListener> m_okayListeners = new List<StoreDoneWithBAM.ButtonPressedListener>();

	// Token: 0x02000429 RID: 1065
	// (Invoke) Token: 0x060035FF RID: 13823
	public delegate void ButtonPressedListener();
}
