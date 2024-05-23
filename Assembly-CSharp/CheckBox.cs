using System;
using UnityEngine;

// Token: 0x0200051F RID: 1311
[CustomEditClass]
public class CheckBox : PegUIElement
{
	// Token: 0x06003CF5 RID: 15605 RVA: 0x00126C61 File Offset: 0x00124E61
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.SetState(PegUIElement.InteractionState.Over);
	}

	// Token: 0x06003CF6 RID: 15606 RVA: 0x00126C7B File Offset: 0x00124E7B
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.SetState(PegUIElement.InteractionState.Up);
	}

	// Token: 0x06003CF7 RID: 15607 RVA: 0x00126C95 File Offset: 0x00124E95
	protected override void OnPress()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.SetState(PegUIElement.InteractionState.Down);
	}

	// Token: 0x06003CF8 RID: 15608 RVA: 0x00126CB0 File Offset: 0x00124EB0
	protected override void OnRelease()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.ToggleChecked();
		if (this.m_checked && !string.IsNullOrEmpty(this.m_checkOnSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_checkOnSound));
		}
		else if (!this.m_checked && !string.IsNullOrEmpty(this.m_checkOffSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_checkOffSound));
		}
		this.SetState(PegUIElement.InteractionState.Over);
	}

	// Token: 0x06003CF9 RID: 15609 RVA: 0x00126D44 File Offset: 0x00124F44
	public void SetButtonText(string s)
	{
		if (this.m_text != null)
		{
			this.m_text.text = s;
		}
		if (this.m_uberText != null)
		{
			this.m_uberText.Text = s;
		}
	}

	// Token: 0x06003CFA RID: 15610 RVA: 0x00126D8B File Offset: 0x00124F8B
	public void SetButtonID(int id)
	{
		this.m_buttonID = id;
	}

	// Token: 0x06003CFB RID: 15611 RVA: 0x00126D94 File Offset: 0x00124F94
	public int GetButtonID()
	{
		return this.m_buttonID;
	}

	// Token: 0x06003CFC RID: 15612 RVA: 0x00126D9C File Offset: 0x00124F9C
	public void SetState(PegUIElement.InteractionState state)
	{
		this.SetEnabled(true);
		switch (state)
		{
		}
	}

	// Token: 0x06003CFD RID: 15613 RVA: 0x00126DE8 File Offset: 0x00124FE8
	public virtual void SetChecked(bool isChecked)
	{
		this.m_checked = isChecked;
		if (this.m_check != null)
		{
			this.m_check.SetActive(this.m_checked);
		}
	}

	// Token: 0x06003CFE RID: 15614 RVA: 0x00126E13 File Offset: 0x00125013
	public bool IsChecked()
	{
		return this.m_checked;
	}

	// Token: 0x06003CFF RID: 15615 RVA: 0x00126E1B File Offset: 0x0012501B
	private bool ToggleChecked()
	{
		this.SetChecked(!this.m_checked);
		return this.m_checked;
	}

	// Token: 0x040026C9 RID: 9929
	public GameObject m_check;

	// Token: 0x040026CA RID: 9930
	public TextMesh m_text;

	// Token: 0x040026CB RID: 9931
	public UberText m_uberText;

	// Token: 0x040026CC RID: 9932
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_checkOnSound;

	// Token: 0x040026CD RID: 9933
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_checkOffSound;

	// Token: 0x040026CE RID: 9934
	private bool m_checked;

	// Token: 0x040026CF RID: 9935
	private int m_buttonID;
}
