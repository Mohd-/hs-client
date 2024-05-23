using System;
using UnityEngine;

// Token: 0x020007B0 RID: 1968
[CustomEditClass]
public class NormalButton : PegUIElement
{
	// Token: 0x06004D55 RID: 19797 RVA: 0x00170A60 File Offset: 0x0016EC60
	protected override void Awake()
	{
		this.SetOriginalButtonPosition();
	}

	// Token: 0x06004D56 RID: 19798 RVA: 0x00170A68 File Offset: 0x0016EC68
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (this.m_mouseOverBone != null)
		{
			this.m_button.transform.position = this.m_mouseOverBone.transform.position;
		}
		else
		{
			TransformUtil.SetLocalPosY(this.m_button.gameObject, this.m_originalButtonPosition.y + this.m_userOverYOffset);
		}
	}

	// Token: 0x06004D57 RID: 19799 RVA: 0x00170ACD File Offset: 0x0016ECCD
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_button.gameObject.transform.localPosition = this.m_originalButtonPosition;
	}

	// Token: 0x06004D58 RID: 19800 RVA: 0x00170AEA File Offset: 0x0016ECEA
	public void SetUserOverYOffset(float userOverYOffset)
	{
		this.m_userOverYOffset = userOverYOffset;
	}

	// Token: 0x06004D59 RID: 19801 RVA: 0x00170AF3 File Offset: 0x0016ECF3
	public void SetButtonID(int newID)
	{
		this.buttonID = newID;
	}

	// Token: 0x06004D5A RID: 19802 RVA: 0x00170AFC File Offset: 0x0016ECFC
	public int GetButtonID()
	{
		return this.buttonID;
	}

	// Token: 0x06004D5B RID: 19803 RVA: 0x00170B04 File Offset: 0x0016ED04
	public void SetText(string t)
	{
		if (this.m_buttonUberText == null)
		{
			this.m_buttonText.text = t;
		}
		else
		{
			this.m_buttonUberText.Text = t;
		}
	}

	// Token: 0x06004D5C RID: 19804 RVA: 0x00170B40 File Offset: 0x0016ED40
	public float GetTextWidth()
	{
		if (this.m_buttonUberText == null)
		{
			return this.m_buttonText.GetComponent<Renderer>().bounds.extents.x * 2f;
		}
		return this.m_buttonUberText.Width;
	}

	// Token: 0x06004D5D RID: 19805 RVA: 0x00170B90 File Offset: 0x0016ED90
	public float GetTextHeight()
	{
		if (this.m_buttonUberText == null)
		{
			return this.m_buttonText.GetComponent<Renderer>().bounds.extents.y * 2f;
		}
		return this.m_buttonUberText.Height;
	}

	// Token: 0x06004D5E RID: 19806 RVA: 0x00170BE0 File Offset: 0x0016EDE0
	public float GetRight()
	{
		return base.GetComponent<BoxCollider>().bounds.max.x;
	}

	// Token: 0x06004D5F RID: 19807 RVA: 0x00170C08 File Offset: 0x0016EE08
	public float GetLeft()
	{
		Bounds bounds = base.GetComponent<BoxCollider>().bounds;
		return bounds.center.x - bounds.extents.x;
	}

	// Token: 0x06004D60 RID: 19808 RVA: 0x00170C40 File Offset: 0x0016EE40
	public float GetTop()
	{
		Bounds bounds = base.GetComponent<BoxCollider>().bounds;
		return bounds.center.y + bounds.extents.y;
	}

	// Token: 0x06004D61 RID: 19809 RVA: 0x00170C78 File Offset: 0x0016EE78
	public float GetBottom()
	{
		Bounds bounds = base.GetComponent<BoxCollider>().bounds;
		return bounds.center.y - bounds.extents.y;
	}

	// Token: 0x06004D62 RID: 19810 RVA: 0x00170CB0 File Offset: 0x0016EEB0
	public void SetOriginalButtonPosition()
	{
		this.m_originalButtonPosition = this.m_button.transform.localPosition;
	}

	// Token: 0x06004D63 RID: 19811 RVA: 0x00170CC8 File Offset: 0x0016EEC8
	public GameObject GetButtonTextGO()
	{
		if (this.m_buttonUberText == null)
		{
			return this.m_buttonText.gameObject;
		}
		return this.m_buttonUberText.gameObject;
	}

	// Token: 0x06004D64 RID: 19812 RVA: 0x00170CFD File Offset: 0x0016EEFD
	public UberText GetButtonUberText()
	{
		return this.m_buttonUberText;
	}

	// Token: 0x06004D65 RID: 19813 RVA: 0x00170D08 File Offset: 0x0016EF08
	public string GetText()
	{
		if (this.m_buttonUberText == null)
		{
			return this.m_buttonText.text;
		}
		return this.m_buttonUberText.Text;
	}

	// Token: 0x04003492 RID: 13458
	[CustomEditField(Sections = "Button Properties")]
	public GameObject m_button;

	// Token: 0x04003493 RID: 13459
	[CustomEditField(Sections = "Button Properties")]
	public TextMesh m_buttonText;

	// Token: 0x04003494 RID: 13460
	[CustomEditField(Sections = "Button Properties")]
	public UberText m_buttonUberText;

	// Token: 0x04003495 RID: 13461
	[CustomEditField(Sections = "Mouse Over Settings")]
	public GameObject m_mouseOverBone;

	// Token: 0x04003496 RID: 13462
	[CustomEditField(Sections = "Mouse Over Settings")]
	public float m_userOverYOffset = -0.05f;

	// Token: 0x04003497 RID: 13463
	private Vector3 m_originalButtonPosition;

	// Token: 0x04003498 RID: 13464
	private int buttonID;
}
