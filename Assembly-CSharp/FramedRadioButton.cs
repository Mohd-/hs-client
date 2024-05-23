using System;
using UnityEngine;

// Token: 0x02000B01 RID: 2817
public class FramedRadioButton : MonoBehaviour
{
	// Token: 0x06006094 RID: 24724 RVA: 0x001CEB0F File Offset: 0x001CCD0F
	public int GetButtonID()
	{
		return this.m_radioButton.GetButtonID();
	}

	// Token: 0x06006095 RID: 24725 RVA: 0x001CEB1C File Offset: 0x001CCD1C
	public float GetLeftEdgeOffset()
	{
		return this.m_leftEdgeOffset;
	}

	// Token: 0x06006096 RID: 24726 RVA: 0x001CEB24 File Offset: 0x001CCD24
	public virtual void Init(FramedRadioButton.FrameType frameType, string text, int buttonID, object userData)
	{
		this.m_radioButton.SetButtonID(buttonID);
		this.m_radioButton.SetUserData(userData);
		this.m_text.Text = text;
		this.m_text.UpdateNow();
		this.m_frameFill.SetActive(true);
		bool flag = false;
		bool active = false;
		switch (frameType)
		{
		case FramedRadioButton.FrameType.SINGLE:
			flag = true;
			active = true;
			break;
		case FramedRadioButton.FrameType.MULTI_LEFT_END:
			flag = true;
			active = false;
			break;
		case FramedRadioButton.FrameType.MULTI_RIGHT_END:
			flag = false;
			active = true;
			break;
		case FramedRadioButton.FrameType.MULTI_MIDDLE:
			flag = false;
			active = false;
			break;
		}
		this.m_frameEndLeft.SetActive(flag);
		this.m_frameLeft.SetActive(!flag);
		this.m_frameEndRight.SetActive(active);
		Transform transform = (!flag) ? this.m_frameLeft.transform : this.m_frameEndLeft.transform;
		this.m_leftEdgeOffset = transform.position.x - base.transform.position.x;
	}

	// Token: 0x06006097 RID: 24727 RVA: 0x001CEC25 File Offset: 0x001CCE25
	public void Show()
	{
		this.m_root.SetActive(true);
	}

	// Token: 0x06006098 RID: 24728 RVA: 0x001CEC33 File Offset: 0x001CCE33
	public void Hide()
	{
		this.m_root.SetActive(false);
	}

	// Token: 0x06006099 RID: 24729 RVA: 0x001CEC44 File Offset: 0x001CCE44
	public Bounds GetBounds()
	{
		Bounds bounds = this.m_frameFill.GetComponent<Renderer>().bounds;
		this.IncludeBoundsOfGameObject(this.m_frameEndLeft, ref bounds);
		this.IncludeBoundsOfGameObject(this.m_frameEndRight, ref bounds);
		this.IncludeBoundsOfGameObject(this.m_frameLeft, ref bounds);
		return bounds;
	}

	// Token: 0x0600609A RID: 24730 RVA: 0x001CEC90 File Offset: 0x001CCE90
	private void IncludeBoundsOfGameObject(GameObject go, ref Bounds bounds)
	{
		if (!go.activeSelf)
		{
			return;
		}
		Bounds bounds2 = go.GetComponent<Renderer>().bounds;
		Vector3 vector = Vector3.Max(bounds2.max, bounds.max);
		Vector3 vector2 = Vector3.Min(bounds2.min, bounds.min);
		bounds.SetMinMax(vector2, vector);
	}

	// Token: 0x0400482B RID: 18475
	public GameObject m_root;

	// Token: 0x0400482C RID: 18476
	public GameObject m_frameEndLeft;

	// Token: 0x0400482D RID: 18477
	public GameObject m_frameEndRight;

	// Token: 0x0400482E RID: 18478
	public GameObject m_frameLeft;

	// Token: 0x0400482F RID: 18479
	public GameObject m_frameFill;

	// Token: 0x04004830 RID: 18480
	public RadioButton m_radioButton;

	// Token: 0x04004831 RID: 18481
	public UberText m_text;

	// Token: 0x04004832 RID: 18482
	private float m_leftEdgeOffset;

	// Token: 0x02000B03 RID: 2819
	public enum FrameType
	{
		// Token: 0x04004836 RID: 18486
		SINGLE,
		// Token: 0x04004837 RID: 18487
		MULTI_LEFT_END,
		// Token: 0x04004838 RID: 18488
		MULTI_RIGHT_END,
		// Token: 0x04004839 RID: 18489
		MULTI_MIDDLE
	}
}
