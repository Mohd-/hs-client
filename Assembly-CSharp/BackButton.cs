using System;
using UnityEngine;

// Token: 0x02000467 RID: 1127
public class BackButton : PegUIElement
{
	// Token: 0x06003742 RID: 14146 RVA: 0x0010EB34 File Offset: 0x0010CD34
	protected override void Awake()
	{
		base.Awake();
		base.SetOriginalLocalPosition();
		this.m_highlight.SetActive(false);
		if (this.m_backText)
		{
			this.m_backText.Text = GameStrings.Get("GLOBAL_BACK");
		}
	}

	// Token: 0x06003743 RID: 14147 RVA: 0x0010EB80 File Offset: 0x0010CD80
	protected override void OnPress()
	{
		Vector3 originalLocalPosition = base.GetOriginalLocalPosition();
		Vector3 vector;
		vector..ctor(originalLocalPosition.x, originalLocalPosition.y - 0.3f, originalLocalPosition.z);
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

	// Token: 0x06003744 RID: 14148 RVA: 0x0010EC04 File Offset: 0x0010CE04
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

	// Token: 0x06003745 RID: 14149 RVA: 0x0010EC64 File Offset: 0x0010CE64
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		Vector3 originalLocalPosition = base.GetOriginalLocalPosition();
		Vector3 vector;
		vector..ctor(originalLocalPosition.x, originalLocalPosition.y + 0.5f, originalLocalPosition.z);
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

	// Token: 0x06003746 RID: 14150 RVA: 0x0010ECF4 File Offset: 0x0010CEF4
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

	// Token: 0x04002296 RID: 8854
	public GameObject m_highlight;

	// Token: 0x04002297 RID: 8855
	public UberText m_backText;

	// Token: 0x04002298 RID: 8856
	public static KeyCode backKey;
}
