using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000F66 RID: 3942
[CustomEditClass]
public class UIBInfoButton : PegUIElement
{
	// Token: 0x06007503 RID: 29955 RVA: 0x00228B80 File Offset: 0x00226D80
	protected override void Awake()
	{
		base.Awake();
		UIBHighlight uibhighlight = base.GetComponent<UIBHighlight>();
		if (uibhighlight == null)
		{
			uibhighlight = base.gameObject.AddComponent<UIBHighlight>();
		}
		this.m_UIBHighlight = uibhighlight;
		if (this.m_UIBHighlight != null)
		{
			this.m_UIBHighlight.m_MouseOverHighlight = this.m_Highlight;
			this.m_UIBHighlight.m_HideMouseOverOnPress = false;
		}
	}

	// Token: 0x06007504 RID: 29956 RVA: 0x00228BE7 File Offset: 0x00226DE7
	public void Select()
	{
		this.Depress();
	}

	// Token: 0x06007505 RID: 29957 RVA: 0x00228BEF File Offset: 0x00226DEF
	public void Deselect()
	{
		this.Raise();
	}

	// Token: 0x06007506 RID: 29958 RVA: 0x00228BF8 File Offset: 0x00226DF8
	private void Raise()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_UpBone.localPosition,
			"time",
			0.1f,
			"easeType",
			iTween.EaseType.linear,
			"isLocal",
			true
		});
		iTween.MoveTo(this.m_RootObject, args);
	}

	// Token: 0x06007507 RID: 29959 RVA: 0x00228C70 File Offset: 0x00226E70
	private void Depress()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_DownBone.localPosition,
			"time",
			0.1f,
			"easeType",
			iTween.EaseType.linear,
			"isLocal",
			true
		});
		iTween.MoveTo(this.m_RootObject, args);
	}

	// Token: 0x04005F90 RID: 24464
	private const float RAISE_TIME = 0.1f;

	// Token: 0x04005F91 RID: 24465
	private const float DEPRESS_TIME = 0.1f;

	// Token: 0x04005F92 RID: 24466
	[CustomEditField(Sections = "Button Objects")]
	[SerializeField]
	public GameObject m_RootObject;

	// Token: 0x04005F93 RID: 24467
	[SerializeField]
	[CustomEditField(Sections = "Button Objects")]
	public Transform m_UpBone;

	// Token: 0x04005F94 RID: 24468
	[CustomEditField(Sections = "Button Objects")]
	[SerializeField]
	public Transform m_DownBone;

	// Token: 0x04005F95 RID: 24469
	[SerializeField]
	[CustomEditField(Sections = "Highlight")]
	public GameObject m_Highlight;

	// Token: 0x04005F96 RID: 24470
	private UIBHighlight m_UIBHighlight;
}
