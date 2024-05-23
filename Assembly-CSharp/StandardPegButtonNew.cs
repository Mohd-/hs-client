using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007EA RID: 2026
[ExecuteInEditMode]
public class StandardPegButtonNew : PegUIElement
{
	// Token: 0x06004EB9 RID: 20153 RVA: 0x00175E53 File Offset: 0x00174053
	public void SetText(string t)
	{
		this.m_buttonText.Text = t;
	}

	// Token: 0x06004EBA RID: 20154 RVA: 0x00175E64 File Offset: 0x00174064
	public void SetWidth(float globalWidth)
	{
		this.m_button.SetWidth(globalWidth * 0.88f);
		if (this.m_border != null)
		{
			this.m_border.SetWidth(globalWidth);
		}
		Quaternion rotation = base.transform.rotation;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		Vector3 size = this.m_button.GetSize();
		Vector3 vector = TransformUtil.ComputeWorldScale(base.transform);
		Vector3 size2;
		size2..ctor(size.x / vector.x, size.z / vector.z, size.y / vector.y);
		base.GetComponent<BoxCollider>().size = size2;
		base.transform.rotation = rotation;
	}

	// Token: 0x06004EBB RID: 20155 RVA: 0x00175F25 File Offset: 0x00174125
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004EBC RID: 20156 RVA: 0x00175F33 File Offset: 0x00174133
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06004EBD RID: 20157 RVA: 0x00175F41 File Offset: 0x00174141
	public void Disable()
	{
		this.m_button.transform.localRotation = Quaternion.Euler(new Vector3(180f, 180f, 0f));
		this.SetEnabled(false);
	}

	// Token: 0x06004EBE RID: 20158 RVA: 0x00175F73 File Offset: 0x00174173
	public void Enable()
	{
		this.m_button.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		this.SetEnabled(true);
	}

	// Token: 0x06004EBF RID: 20159 RVA: 0x00175FA8 File Offset: 0x001741A8
	public void Reset()
	{
		iTween.StopByName(this.m_button.gameObject, "rotation");
		this.m_button.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
	}

	// Token: 0x06004EC0 RID: 20160 RVA: 0x00175FF3 File Offset: 0x001741F3
	public void LockHighlight()
	{
		this.m_highlight.gameObject.SetActive(true);
		this.m_highlightLocked = true;
	}

	// Token: 0x06004EC1 RID: 20161 RVA: 0x0017600D File Offset: 0x0017420D
	public void UnlockHighlight()
	{
		this.m_highlight.gameObject.SetActive(false);
		this.m_highlightLocked = false;
	}

	// Token: 0x06004EC2 RID: 20162 RVA: 0x00176028 File Offset: 0x00174228
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (this.m_highlightLocked)
		{
			return;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			new Vector3(90f, 0f, 0f),
			"time",
			0.5f,
			"name",
			"rotation"
		});
		iTween.StopByName(this.m_button.gameObject, "rotation");
		this.m_button.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		iTween.PunchRotation(this.m_button.gameObject, args);
		this.m_highlight.gameObject.SetActive(true);
		if (SoundManager.Get() != null && SoundManager.Get().GetConfig() != null)
		{
			SoundManager.Get().LoadAndPlay("Small_Mouseover");
		}
	}

	// Token: 0x06004EC3 RID: 20163 RVA: 0x0017612C File Offset: 0x0017432C
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_button.transform.localPosition = this.m_upBone.transform.localPosition;
		if (this.m_highlightLocked)
		{
			return;
		}
		this.m_highlight.gameObject.SetActive(false);
	}

	// Token: 0x06004EC4 RID: 20164 RVA: 0x00176178 File Offset: 0x00174378
	protected override void OnPress()
	{
		this.m_button.transform.localPosition = this.m_downBone.transform.localPosition;
		if (SoundManager.Get() != null && SoundManager.Get().GetConfig() != null)
		{
			SoundManager.Get().LoadAndPlay("Back_Click");
		}
	}

	// Token: 0x06004EC5 RID: 20165 RVA: 0x001761DC File Offset: 0x001743DC
	protected override void OnRelease()
	{
		this.m_button.transform.localPosition = this.m_upBone.transform.localPosition;
	}

	// Token: 0x0400359D RID: 13725
	private const float HIGHLIGHT_SCALE = 1.2f;

	// Token: 0x0400359E RID: 13726
	private const float GRAY_FRAME_SCALE = 0.88f;

	// Token: 0x0400359F RID: 13727
	public UberText m_buttonText;

	// Token: 0x040035A0 RID: 13728
	public ThreeSliceElement m_button;

	// Token: 0x040035A1 RID: 13729
	public ThreeSliceElement m_border;

	// Token: 0x040035A2 RID: 13730
	public ThreeSliceElement m_highlight;

	// Token: 0x040035A3 RID: 13731
	public GameObject m_upBone;

	// Token: 0x040035A4 RID: 13732
	public GameObject m_downBone;

	// Token: 0x040035A5 RID: 13733
	public float m_buttonWidth;

	// Token: 0x040035A6 RID: 13734
	public bool m_ExecuteInEditMode;

	// Token: 0x040035A7 RID: 13735
	private bool m_highlightLocked;
}
