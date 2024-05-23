using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000564 RID: 1380
public class FriendListItemHeader : PegUIElement, ITouchListItem
{
	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x06003F38 RID: 16184 RVA: 0x001338EB File Offset: 0x00131AEB
	// (set) Token: 0x06003F39 RID: 16185 RVA: 0x001338F3 File Offset: 0x00131AF3
	public GameObject Background { get; set; }

	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x06003F3A RID: 16186 RVA: 0x001338FC File Offset: 0x00131AFC
	// (set) Token: 0x06003F3B RID: 16187 RVA: 0x00133904 File Offset: 0x00131B04
	public Bounds LocalBounds { get; private set; }

	// Token: 0x06003F3C RID: 16188 RVA: 0x0013390D File Offset: 0x00131B0D
	public void SetText(string text)
	{
		this.m_Text.Text = text;
	}

	// Token: 0x17000470 RID: 1136
	// (get) Token: 0x06003F3D RID: 16189 RVA: 0x0013391B File Offset: 0x00131B1B
	public bool IsHeader
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000471 RID: 1137
	// (get) Token: 0x06003F3E RID: 16190 RVA: 0x0013391E File Offset: 0x00131B1E
	// (set) Token: 0x06003F3F RID: 16191 RVA: 0x00133926 File Offset: 0x00131B26
	public bool Visible
	{
		get
		{
			return this.IsShowingContents;
		}
		set
		{
		}
	}

	// Token: 0x17000472 RID: 1138
	// (get) Token: 0x06003F40 RID: 16192 RVA: 0x00133928 File Offset: 0x00131B28
	public bool IsShowingContents
	{
		get
		{
			return this.m_ShowContents;
		}
	}

	// Token: 0x17000473 RID: 1139
	// (get) Token: 0x06003F41 RID: 16193 RVA: 0x00133930 File Offset: 0x00131B30
	// (set) Token: 0x06003F42 RID: 16194 RVA: 0x00133938 File Offset: 0x00131B38
	public MobileFriendListItem.TypeFlags SubType { get; set; }

	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x06003F43 RID: 16195 RVA: 0x00133941 File Offset: 0x00131B41
	// (set) Token: 0x06003F44 RID: 16196 RVA: 0x00133949 File Offset: 0x00131B49
	public Option Option { get; set; }

	// Token: 0x06003F45 RID: 16197 RVA: 0x00133954 File Offset: 0x00131B54
	public void SetInitialShowContents(bool show)
	{
		this.m_ShowContents = show;
		this.m_Arrow.transform.rotation = this.GetCurrentBoneTransform().rotation;
	}

	// Token: 0x06003F46 RID: 16198 RVA: 0x00133984 File Offset: 0x00131B84
	public void AddToggleListener(FriendListItemHeader.ToggleContentsFunc func, object userdata)
	{
		FriendListItemHeader.ToggleContentsListener toggleContentsListener = new FriendListItemHeader.ToggleContentsListener();
		toggleContentsListener.SetCallback(func);
		toggleContentsListener.SetUserData(userdata);
		this.m_ToggleEventListeners.Add(toggleContentsListener);
	}

	// Token: 0x06003F47 RID: 16199 RVA: 0x001339B1 File Offset: 0x00131BB1
	public void ClearToggleListeners()
	{
		this.m_ToggleEventListeners.Clear();
	}

	// Token: 0x06003F48 RID: 16200 RVA: 0x001339C0 File Offset: 0x00131BC0
	protected override void Awake()
	{
		base.Awake();
		this.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnHeaderButtonReleased));
		if (this.m_multiSlice == null)
		{
			this.m_multiSlice = base.GetComponentInChildren<MultiSliceElement>();
			if (this.m_multiSlice)
			{
				this.m_multiSlice.UpdateSlices();
			}
		}
	}

	// Token: 0x06003F49 RID: 16201 RVA: 0x00133A20 File Offset: 0x00131C20
	private void OnHeaderButtonReleased(UIEvent e)
	{
		this.m_ShowContents = !this.m_ShowContents;
		FriendListItemHeader.ToggleContentsListener[] array = this.m_ToggleEventListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire(this.m_ShowContents);
		}
		this.UpdateFoldoutArrow();
	}

	// Token: 0x06003F4A RID: 16202 RVA: 0x00133A70 File Offset: 0x00131C70
	private void UpdateFoldoutArrow()
	{
		if (this.m_Arrow == null || this.m_FoldinBone == null || this.m_FoldoutBone == null)
		{
			return;
		}
		iTween.RotateTo(this.m_Arrow, this.GetCurrentBoneTransform().rotation.eulerAngles, this.m_AnimRotateTime);
	}

	// Token: 0x06003F4B RID: 16203 RVA: 0x00133AD5 File Offset: 0x00131CD5
	private Transform GetCurrentBoneTransform()
	{
		return (!this.m_ShowContents) ? this.m_FoldinBone : this.m_FoldoutBone;
	}

	// Token: 0x06003F4C RID: 16204 RVA: 0x00133AF3 File Offset: 0x00131CF3
	virtual T GetComponent<T>()
	{
		return base.GetComponent<T>();
	}

	// Token: 0x06003F4D RID: 16205 RVA: 0x00133AFB File Offset: 0x00131CFB
	virtual GameObject get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06003F4E RID: 16206 RVA: 0x00133B03 File Offset: 0x00131D03
	virtual Transform get_transform()
	{
		return base.transform;
	}

	// Token: 0x0400288E RID: 10382
	public UberText m_Text;

	// Token: 0x0400288F RID: 10383
	public GameObject m_Arrow;

	// Token: 0x04002890 RID: 10384
	public Transform m_FoldinBone;

	// Token: 0x04002891 RID: 10385
	public Transform m_FoldoutBone;

	// Token: 0x04002892 RID: 10386
	public float m_AnimRotateTime = 0.25f;

	// Token: 0x04002893 RID: 10387
	private List<FriendListItemHeader.ToggleContentsListener> m_ToggleEventListeners = new List<FriendListItemHeader.ToggleContentsListener>();

	// Token: 0x04002894 RID: 10388
	private bool m_ShowContents = true;

	// Token: 0x04002895 RID: 10389
	private MultiSliceElement m_multiSlice;

	// Token: 0x02000585 RID: 1413
	// (Invoke) Token: 0x0600402E RID: 16430
	public delegate void ToggleContentsFunc(bool show, object userdata);

	// Token: 0x0200063E RID: 1598
	private class ToggleContentsListener : EventListener<FriendListItemHeader.ToggleContentsFunc>
	{
		// Token: 0x06004547 RID: 17735 RVA: 0x0014CF11 File Offset: 0x0014B111
		public void Fire(bool show)
		{
			this.m_callback(show, this.m_userData);
		}
	}
}
