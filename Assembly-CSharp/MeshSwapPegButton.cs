using System;
using UnityEngine;

// Token: 0x02000F55 RID: 3925
public class MeshSwapPegButton : PegUIElement
{
	// Token: 0x060074B8 RID: 29880 RVA: 0x00227128 File Offset: 0x00225328
	protected override void Awake()
	{
		this.originalPosition = this.upState.transform.localPosition;
		base.Awake();
		this.SetState(PegUIElement.InteractionState.Up);
		Bounds boundsOfChildren = TransformUtil.GetBoundsOfChildren(base.gameObject);
		if (base.GetComponent<MeshRenderer>() != null)
		{
			base.GetComponent<MeshRenderer>().bounds.SetMinMax(boundsOfChildren.min, boundsOfChildren.max);
		}
	}

	// Token: 0x060074B9 RID: 29881 RVA: 0x00227196 File Offset: 0x00225396
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		this.SetState(PegUIElement.InteractionState.Over);
	}

	// Token: 0x060074BA RID: 29882 RVA: 0x002271B0 File Offset: 0x002253B0
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		this.SetState(PegUIElement.InteractionState.Up);
	}

	// Token: 0x060074BB RID: 29883 RVA: 0x002271CA File Offset: 0x002253CA
	protected override void OnPress()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		this.SetState(PegUIElement.InteractionState.Down);
	}

	// Token: 0x060074BC RID: 29884 RVA: 0x002271E4 File Offset: 0x002253E4
	protected override void OnRelease()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		this.SetState(PegUIElement.InteractionState.Over);
	}

	// Token: 0x060074BD RID: 29885 RVA: 0x002271FE File Offset: 0x002253FE
	public void SetButtonText(string s)
	{
		this.buttonText.text = s;
	}

	// Token: 0x060074BE RID: 29886 RVA: 0x0022720C File Offset: 0x0022540C
	public void SetButtonID(int id)
	{
		this.m_buttonID = id;
	}

	// Token: 0x060074BF RID: 29887 RVA: 0x00227215 File Offset: 0x00225415
	public int GetButtonID()
	{
		return this.m_buttonID;
	}

	// Token: 0x060074C0 RID: 29888 RVA: 0x00227220 File Offset: 0x00225420
	public void SetState(PegUIElement.InteractionState state)
	{
		if (this.overState != null)
		{
			this.overState.SetActive(false);
		}
		if (this.disabledState != null)
		{
			this.disabledState.SetActive(false);
		}
		if (this.upState != null)
		{
			this.upState.SetActive(false);
		}
		if (this.downState != null)
		{
			this.downState.SetActive(false);
		}
		this.SetEnabled(true);
		switch (state)
		{
		case PegUIElement.InteractionState.Over:
			this.overState.SetActive(true);
			break;
		case PegUIElement.InteractionState.Down:
			this.downState.transform.localPosition = this.originalPosition + this.downOffset;
			this.downState.SetActive(true);
			break;
		case PegUIElement.InteractionState.Up:
			this.upState.SetActive(true);
			this.downState.transform.localPosition = this.originalPosition;
			break;
		case PegUIElement.InteractionState.Disabled:
			this.disabledState.SetActive(true);
			this.SetEnabled(false);
			break;
		}
	}

	// Token: 0x060074C1 RID: 29889 RVA: 0x0022734E File Offset: 0x0022554E
	public void Show()
	{
		base.gameObject.SetActive(true);
		this.SetState(PegUIElement.InteractionState.Up);
	}

	// Token: 0x060074C2 RID: 29890 RVA: 0x00227363 File Offset: 0x00225563
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04005F3A RID: 24378
	public GameObject upState;

	// Token: 0x04005F3B RID: 24379
	public GameObject overState;

	// Token: 0x04005F3C RID: 24380
	public GameObject downState;

	// Token: 0x04005F3D RID: 24381
	public GameObject disabledState;

	// Token: 0x04005F3E RID: 24382
	public Vector3 downOffset;

	// Token: 0x04005F3F RID: 24383
	private Vector3 originalPosition;

	// Token: 0x04005F40 RID: 24384
	private Vector3 originalScale;

	// Token: 0x04005F41 RID: 24385
	private int m_buttonID;

	// Token: 0x04005F42 RID: 24386
	public TextMesh buttonText;
}
