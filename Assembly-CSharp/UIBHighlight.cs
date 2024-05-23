using System;
using UnityEngine;

// Token: 0x020001AF RID: 431
[RequireComponent(typeof(PegUIElement), typeof(BoxCollider))]
[CustomEditClass]
public class UIBHighlight : MonoBehaviour
{
	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06001C3D RID: 7229 RVA: 0x00084FA1 File Offset: 0x000831A1
	// (set) Token: 0x06001C3E RID: 7230 RVA: 0x00084FA9 File Offset: 0x000831A9
	[CustomEditField(Sections = "Behavior Settings")]
	public bool AlwaysOver
	{
		get
		{
			return this.m_AlwaysOver;
		}
		set
		{
			this.m_AlwaysOver = value;
			this.ResetState();
		}
	}

	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06001C3F RID: 7231 RVA: 0x00084FB8 File Offset: 0x000831B8
	// (set) Token: 0x06001C40 RID: 7232 RVA: 0x00084FC0 File Offset: 0x000831C0
	[CustomEditField(Sections = "Behavior Settings")]
	public bool EnableResponse
	{
		get
		{
			return this.m_EnableResponse;
		}
		set
		{
			this.m_EnableResponse = value;
			this.ResetState();
		}
	}

	// Token: 0x06001C41 RID: 7233 RVA: 0x00084FD0 File Offset: 0x000831D0
	private void Awake()
	{
		PegUIElement component = base.gameObject.GetComponent<PegUIElement>();
		if (component != null)
		{
			component.AddEventListener(UIEventType.ROLLOVER, delegate(UIEvent e)
			{
				this.OnRollOver(false);
			});
			component.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
			{
				this.OnPress(true);
			});
			component.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.OnRelease();
			});
			component.AddEventListener(UIEventType.ROLLOUT, delegate(UIEvent e)
			{
				this.OnRollOut(false);
			});
			this.ResetState();
		}
	}

	// Token: 0x06001C42 RID: 7234 RVA: 0x0008504B File Offset: 0x0008324B
	public void HighlightOnce()
	{
		this.OnRollOver(true);
	}

	// Token: 0x06001C43 RID: 7235 RVA: 0x00085054 File Offset: 0x00083254
	public void Select()
	{
		if (this.m_SelectOnRelease)
		{
			this.OnRelease(true);
		}
		else
		{
			this.OnPress(true);
		}
	}

	// Token: 0x06001C44 RID: 7236 RVA: 0x00085074 File Offset: 0x00083274
	public void SelectNoSound()
	{
		if (this.m_SelectOnRelease)
		{
			this.OnRelease(false);
		}
		else
		{
			this.OnPress(false);
		}
	}

	// Token: 0x06001C45 RID: 7237 RVA: 0x00085094 File Offset: 0x00083294
	public void Reset()
	{
		this.ResetState();
		this.ShowHighlightObject(this.m_SelectedHighlight, false);
		this.ShowHighlightObject(this.m_MouseOverSelectedHighlight, false);
		this.ShowHighlightObject(this.m_MouseOverHighlight, false);
	}

	// Token: 0x06001C46 RID: 7238 RVA: 0x000850CE File Offset: 0x000832CE
	private void ResetState()
	{
		if (this.m_AlwaysOver)
		{
			this.OnRollOver(true);
		}
		else
		{
			this.OnRollOut(true);
		}
	}

	// Token: 0x06001C47 RID: 7239 RVA: 0x000850F0 File Offset: 0x000832F0
	private void OnRollOver(bool force = false)
	{
		if (!this.m_EnableResponse && !force)
		{
			return;
		}
		if (!this.m_AlwaysOver)
		{
			this.PlaySound(this.m_MouseOverSound);
		}
		if (this.m_AllowSelection && (this.m_SelectedHighlight == null || this.m_SelectedHighlight.activeSelf))
		{
			this.ShowHighlightObject(this.m_MouseOverSelectedHighlight, true);
			this.ShowHighlightObject(this.m_SelectedHighlight, false);
			this.ShowHighlightObject(this.m_MouseOverHighlight, false);
			this.ShowHighlightObject(this.m_MouseUpHighlight, false);
			this.ShowHighlightObject(this.m_MouseDownHighlight, false);
		}
		else
		{
			this.ShowHighlightObject(this.m_MouseDownHighlight, false);
			this.ShowHighlightObject(this.m_MouseOverHighlight, true);
			this.ShowHighlightObject(this.m_MouseUpHighlight, false);
		}
	}

	// Token: 0x06001C48 RID: 7240 RVA: 0x000851C0 File Offset: 0x000833C0
	private void OnRollOut(bool force = false)
	{
		if (!this.m_EnableResponse && !force)
		{
			return;
		}
		this.PlaySound(this.m_MouseOutSound);
		if (this.m_AllowSelection && (this.m_MouseOverSelectedHighlight == null || this.m_MouseOverSelectedHighlight.activeSelf))
		{
			this.ShowHighlightObject(this.m_SelectedHighlight, true);
			this.ShowHighlightObject(this.m_MouseOverSelectedHighlight, false);
			this.ShowHighlightObject(this.m_MouseOverHighlight, false);
			this.ShowHighlightObject(this.m_MouseUpHighlight, false);
			this.ShowHighlightObject(this.m_MouseDownHighlight, false);
		}
		else
		{
			this.ShowHighlightObject(this.m_MouseDownHighlight, false);
			this.ShowHighlightObject(this.m_MouseOverHighlight, this.m_AlwaysOver);
			this.ShowHighlightObject(this.m_MouseUpHighlight, !this.m_AlwaysOver);
		}
	}

	// Token: 0x06001C49 RID: 7241 RVA: 0x00085291 File Offset: 0x00083491
	private void OnPress()
	{
		this.OnPress(true);
	}

	// Token: 0x06001C4A RID: 7242 RVA: 0x0008529C File Offset: 0x0008349C
	private void OnPress(bool playSound)
	{
		if (!this.m_EnableResponse)
		{
			return;
		}
		if (playSound)
		{
			this.PlaySound(this.m_MouseDownSound);
		}
		if (this.m_AllowSelection && !this.m_SelectOnRelease)
		{
			this.ShowHighlightObject(this.m_SelectedHighlight, true);
			this.ShowHighlightObject(this.m_MouseOverSelectedHighlight, false);
			this.ShowHighlightObject(this.m_MouseOverHighlight, false);
			this.ShowHighlightObject(this.m_MouseUpHighlight, false);
			this.ShowHighlightObject(this.m_MouseDownHighlight, false);
		}
		else
		{
			this.ShowHighlightObject(this.m_MouseDownHighlight, true);
			this.ShowHighlightObject(this.m_MouseOverHighlight, this.m_AlwaysOver || !this.m_HideMouseOverOnPress);
			this.ShowHighlightObject(this.m_MouseUpHighlight, !this.m_AlwaysOver);
		}
	}

	// Token: 0x06001C4B RID: 7243 RVA: 0x00085368 File Offset: 0x00083568
	private void OnRelease()
	{
		this.OnRelease(true);
	}

	// Token: 0x06001C4C RID: 7244 RVA: 0x00085374 File Offset: 0x00083574
	private void OnRelease(bool playSound)
	{
		if (!this.m_EnableResponse)
		{
			return;
		}
		if (playSound)
		{
			this.PlaySound(this.m_MouseUpSound);
		}
		if (this.m_AllowSelection && this.m_SelectOnRelease)
		{
			this.ShowHighlightObject(this.m_SelectedHighlight, true);
			this.ShowHighlightObject(this.m_MouseOverSelectedHighlight, false);
			this.ShowHighlightObject(this.m_MouseOverHighlight, false);
			this.ShowHighlightObject(this.m_MouseUpHighlight, false);
			this.ShowHighlightObject(this.m_MouseDownHighlight, false);
		}
		else
		{
			this.ShowHighlightObject(this.m_MouseDownHighlight, false);
			this.ShowHighlightObject(this.m_MouseOverHighlight, true);
			this.ShowHighlightObject(this.m_MouseUpHighlight, false);
		}
	}

	// Token: 0x06001C4D RID: 7245 RVA: 0x00085422 File Offset: 0x00083622
	private void ShowHighlightObject(GameObject obj, bool show)
	{
		if (obj != null && obj.activeSelf != show)
		{
			obj.SetActive(show);
		}
	}

	// Token: 0x06001C4E RID: 7246 RVA: 0x00085444 File Offset: 0x00083644
	private void PlaySound(string soundFilePath)
	{
		if (!string.IsNullOrEmpty(soundFilePath))
		{
			string soundName = FileUtils.GameAssetPathToName(soundFilePath);
			SoundManager.Get().LoadAndPlay(soundName);
		}
	}

	// Token: 0x04000EC9 RID: 3785
	[CustomEditField(Sections = "Highlight Objects")]
	public GameObject m_MouseOverHighlight;

	// Token: 0x04000ECA RID: 3786
	[CustomEditField(Sections = "Highlight Objects")]
	public GameObject m_MouseDownHighlight;

	// Token: 0x04000ECB RID: 3787
	[CustomEditField(Sections = "Highlight Objects")]
	public GameObject m_MouseUpHighlight;

	// Token: 0x04000ECC RID: 3788
	[CustomEditField(Sections = "Highlight Sounds", T = EditType.SOUND_PREFAB)]
	public string m_MouseOverSound = "Assets/Game/Sounds/Interface/Small_Mouseover";

	// Token: 0x04000ECD RID: 3789
	[CustomEditField(Sections = "Highlight Sounds", T = EditType.SOUND_PREFAB)]
	public string m_MouseOutSound;

	// Token: 0x04000ECE RID: 3790
	[CustomEditField(Sections = "Highlight Sounds", T = EditType.SOUND_PREFAB)]
	public string m_MouseDownSound = "Assets/Game/Sounds/Interface/Small_Click";

	// Token: 0x04000ECF RID: 3791
	[CustomEditField(Sections = "Highlight Sounds", T = EditType.SOUND_PREFAB)]
	public string m_MouseUpSound;

	// Token: 0x04000ED0 RID: 3792
	[CustomEditField(Sections = "Behavior Settings")]
	public bool m_SelectOnRelease;

	// Token: 0x04000ED1 RID: 3793
	[CustomEditField(Sections = "Behavior Settings")]
	public bool m_HideMouseOverOnPress;

	// Token: 0x04000ED2 RID: 3794
	[SerializeField]
	private bool m_AlwaysOver;

	// Token: 0x04000ED3 RID: 3795
	[SerializeField]
	private bool m_EnableResponse = true;

	// Token: 0x04000ED4 RID: 3796
	[CustomEditField(Sections = "Allow Selection", Label = "Enable")]
	public bool m_AllowSelection;

	// Token: 0x04000ED5 RID: 3797
	[CustomEditField(Parent = "m_AllowSelection")]
	public GameObject m_SelectedHighlight;

	// Token: 0x04000ED6 RID: 3798
	[CustomEditField(Parent = "m_AllowSelection")]
	public GameObject m_MouseOverSelectedHighlight;

	// Token: 0x04000ED7 RID: 3799
	private PegUIElement m_PegUIElement;
}
