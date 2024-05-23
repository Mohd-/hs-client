using System;
using UnityEngine;

// Token: 0x02000455 RID: 1109
public class RadioButton : PegUIElement
{
	// Token: 0x060036E7 RID: 14055 RVA: 0x0010DFC8 File Offset: 0x0010C1C8
	protected override void Awake()
	{
		base.Awake();
		this.m_hoverGlow.SetActive(false);
		this.m_selectedGlow.SetActive(false);
		SoundManager.Get().Load("tiny_button_press_2");
		SoundManager.Get().Load("tiny_button_mouseover_2");
	}

	// Token: 0x060036E8 RID: 14056 RVA: 0x0010E013 File Offset: 0x0010C213
	public void SetButtonID(int id)
	{
		this.m_id = id;
	}

	// Token: 0x060036E9 RID: 14057 RVA: 0x0010E01C File Offset: 0x0010C21C
	public int GetButtonID()
	{
		return this.m_id;
	}

	// Token: 0x060036EA RID: 14058 RVA: 0x0010E024 File Offset: 0x0010C224
	public void SetUserData(object userData)
	{
		this.m_userData = userData;
	}

	// Token: 0x060036EB RID: 14059 RVA: 0x0010E02D File Offset: 0x0010C22D
	public object GetUserData()
	{
		return this.m_userData;
	}

	// Token: 0x060036EC RID: 14060 RVA: 0x0010E035 File Offset: 0x0010C235
	public void SetSelected(bool selected)
	{
		this.m_selectedGlow.SetActive(selected);
	}

	// Token: 0x060036ED RID: 14061 RVA: 0x0010E043 File Offset: 0x0010C243
	public bool IsSelected()
	{
		return this.m_selectedGlow.activeSelf;
	}

	// Token: 0x060036EE RID: 14062 RVA: 0x0010E050 File Offset: 0x0010C250
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		SoundManager.Get().LoadAndPlay("tiny_button_mouseover_2");
		this.m_hoverGlow.SetActive(true);
	}

	// Token: 0x060036EF RID: 14063 RVA: 0x0010E06D File Offset: 0x0010C26D
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_hoverGlow.SetActive(false);
	}

	// Token: 0x060036F0 RID: 14064 RVA: 0x0010E07B File Offset: 0x0010C27B
	protected override void OnRelease()
	{
		base.OnRelease();
		SoundManager.Get().LoadAndPlay("tiny_button_press_2");
	}

	// Token: 0x060036F1 RID: 14065 RVA: 0x0010E092 File Offset: 0x0010C292
	protected override void OnDoubleClick()
	{
	}

	// Token: 0x04002231 RID: 8753
	public GameObject m_hoverGlow;

	// Token: 0x04002232 RID: 8754
	public GameObject m_selectedGlow;

	// Token: 0x04002233 RID: 8755
	private int m_id;

	// Token: 0x04002234 RID: 8756
	private object m_userData;
}
