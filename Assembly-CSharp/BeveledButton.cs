using System;
using UnityEngine;

// Token: 0x02000F68 RID: 3944
public class BeveledButton : PegUIElement
{
	// Token: 0x0600750D RID: 29965 RVA: 0x00228EAC File Offset: 0x002270AC
	protected override void Awake()
	{
		base.Awake();
		base.SetOriginalLocalPosition();
		this.m_highlight.SetActive(false);
	}

	// Token: 0x0600750E RID: 29966 RVA: 0x00228EC8 File Offset: 0x002270C8
	protected override void OnPress()
	{
		Vector3 originalLocalPosition = base.GetOriginalLocalPosition();
		Vector3 vector;
		vector..ctor(originalLocalPosition.x, originalLocalPosition.y - 0.3f * base.transform.localScale.y, originalLocalPosition.z);
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			vector,
			"isLocal",
			true,
			"time",
			0.15f
		}));
	}

	// Token: 0x0600750F RID: 29967 RVA: 0x00228F60 File Offset: 0x00227160
	protected override void OnRelease()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			base.GetOriginalLocalPosition(),
			"isLocal",
			true,
			"time",
			0.15f
		}));
	}

	// Token: 0x06007510 RID: 29968 RVA: 0x00228FC0 File Offset: 0x002271C0
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		Vector3 originalLocalPosition = base.GetOriginalLocalPosition();
		Vector3 vector;
		vector..ctor(originalLocalPosition.x, originalLocalPosition.y + 0.5f * base.transform.localScale.y, originalLocalPosition.z);
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			vector,
			"isLocal",
			true,
			"time",
			0.15f
		}));
		this.m_highlight.SetActive(true);
	}

	// Token: 0x06007511 RID: 29969 RVA: 0x00229064 File Offset: 0x00227264
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			base.GetOriginalLocalPosition(),
			"isLocal",
			true,
			"time",
			0.15f
		}));
		this.m_highlight.SetActive(false);
	}

	// Token: 0x06007512 RID: 29970 RVA: 0x002290D0 File Offset: 0x002272D0
	public void SetText(string text)
	{
		if (this.m_uberLabel != null)
		{
			this.m_uberLabel.Text = text;
			return;
		}
		this.m_label.text = text;
	}

	// Token: 0x06007513 RID: 29971 RVA: 0x00229107 File Offset: 0x00227307
	public void Show()
	{
		base.gameObject.SetActive(true);
		this.m_highlight.SetActive(false);
	}

	// Token: 0x06007514 RID: 29972 RVA: 0x00229121 File Offset: 0x00227321
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06007515 RID: 29973 RVA: 0x0022912F File Offset: 0x0022732F
	public UberText GetUberText()
	{
		return this.m_uberLabel;
	}

	// Token: 0x04005F9D RID: 24477
	public TextMesh m_label;

	// Token: 0x04005F9E RID: 24478
	public UberText m_uberLabel;

	// Token: 0x04005F9F RID: 24479
	public GameObject m_highlight;
}
