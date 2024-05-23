using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007CD RID: 1997
public class MassDisenchantTab : PegUIElement
{
	// Token: 0x06004E0C RID: 19980 RVA: 0x00173698 File Offset: 0x00171898
	protected override void Awake()
	{
		base.Awake();
		this.m_highlight.SetActive(false);
		this.m_originalLocalPos = base.transform.localPosition;
		this.m_originalScale = base.transform.localScale;
	}

	// Token: 0x06004E0D RID: 19981 RVA: 0x001736DC File Offset: 0x001718DC
	private void Start()
	{
		this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnRollover));
		this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnRollout));
	}

	// Token: 0x06004E0E RID: 19982 RVA: 0x00173711 File Offset: 0x00171911
	public void Show()
	{
		this.m_isVisible = true;
		this.m_root.SetActive(true);
		this.SetEnabled(true);
	}

	// Token: 0x06004E0F RID: 19983 RVA: 0x0017372D File Offset: 0x0017192D
	public void Hide()
	{
		this.m_isVisible = false;
		this.m_root.SetActive(false);
		this.SetEnabled(false);
	}

	// Token: 0x06004E10 RID: 19984 RVA: 0x00173749 File Offset: 0x00171949
	public bool IsVisible()
	{
		return this.m_isVisible;
	}

	// Token: 0x06004E11 RID: 19985 RVA: 0x00173751 File Offset: 0x00171951
	public void SetAmount(int amount)
	{
		this.m_amount.Text = amount.ToString();
	}

	// Token: 0x06004E12 RID: 19986 RVA: 0x00173768 File Offset: 0x00171968
	public void Select()
	{
		if (this.m_isSelected)
		{
			return;
		}
		this.m_isSelected = true;
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			MassDisenchantTab.SELECTED_SCALE,
			"time",
			CollectionPageManager.SELECT_TAB_ANIM_TIME,
			"name",
			"scale"
		});
		iTween.StopByName(base.gameObject, "scale");
		iTween.ScaleTo(base.gameObject, args);
		Vector3 originalLocalPos = this.m_originalLocalPos;
		originalLocalPos.y += MassDisenchantTab.SELECTED_LOCAL_Y_OFFSET;
		Hashtable args2 = iTween.Hash(new object[]
		{
			"position",
			originalLocalPos,
			"isLocal",
			true,
			"time",
			CollectionPageManager.SELECT_TAB_ANIM_TIME,
			"name",
			"position"
		});
		iTween.StopByName(base.gameObject, "position");
		iTween.MoveTo(base.gameObject, args2);
	}

	// Token: 0x06004E13 RID: 19987 RVA: 0x00173874 File Offset: 0x00171A74
	public void Deselect()
	{
		if (!this.m_isSelected)
		{
			return;
		}
		this.m_isSelected = false;
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			this.m_originalScale,
			"time",
			CollectionPageManager.SELECT_TAB_ANIM_TIME,
			"name",
			"scale"
		});
		iTween.StopByName(base.gameObject, "scale");
		iTween.ScaleTo(base.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"position",
			this.m_originalLocalPos,
			"isLocal",
			true,
			"time",
			CollectionPageManager.SELECT_TAB_ANIM_TIME,
			"name",
			"position"
		});
		iTween.StopByName(base.gameObject, "position");
		iTween.MoveTo(base.gameObject, args2);
	}

	// Token: 0x06004E14 RID: 19988 RVA: 0x0017396B File Offset: 0x00171B6B
	private void OnRollover(UIEvent e)
	{
		this.m_highlight.SetActive(true);
	}

	// Token: 0x06004E15 RID: 19989 RVA: 0x00173979 File Offset: 0x00171B79
	private void OnRollout(UIEvent e)
	{
		this.m_highlight.SetActive(false);
	}

	// Token: 0x04003517 RID: 13591
	public GameObject m_root;

	// Token: 0x04003518 RID: 13592
	public GameObject m_highlight;

	// Token: 0x04003519 RID: 13593
	public UberText m_amount;

	// Token: 0x0400351A RID: 13594
	private static readonly Vector3 SELECTED_SCALE = new Vector3(0.6f, 0.6f, 0.6f);

	// Token: 0x0400351B RID: 13595
	private static readonly float SELECTED_LOCAL_Y_OFFSET = 0.3822131f;

	// Token: 0x0400351C RID: 13596
	private bool m_isSelected;

	// Token: 0x0400351D RID: 13597
	private Vector3 m_originalLocalPos;

	// Token: 0x0400351E RID: 13598
	private Vector3 m_originalScale;

	// Token: 0x0400351F RID: 13599
	private bool m_isVisible;
}
