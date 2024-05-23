using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000215 RID: 533
[CustomEditClass]
[RequireComponent(typeof(BoxCollider))]
public class UIBButton : PegUIElement
{
	// Token: 0x060020A7 RID: 8359 RVA: 0x0009F98A File Offset: 0x0009DB8A
	protected override void OnPress()
	{
		if (!this.m_DepressOnOver)
		{
			this.Depress();
		}
	}

	// Token: 0x060020A8 RID: 8360 RVA: 0x0009F99D File Offset: 0x0009DB9D
	protected override void OnRelease()
	{
		if (!this.m_DepressOnOver)
		{
			this.Raise();
		}
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x0009F9B0 File Offset: 0x0009DBB0
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		if (this.m_Depressed)
		{
			this.Raise();
		}
	}

	// Token: 0x060020AA RID: 8362 RVA: 0x0009F9C3 File Offset: 0x0009DBC3
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (this.m_DepressOnOver)
		{
			this.Depress();
		}
		this.Wiggle();
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x0009F9DC File Offset: 0x0009DBDC
	public void Select()
	{
		this.Depress();
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x0009F9E4 File Offset: 0x0009DBE4
	public void Deselect()
	{
		this.Raise();
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x0009F9EC File Offset: 0x0009DBEC
	public void Flip(bool faceUp)
	{
		if (this.m_RootObject == null)
		{
			return;
		}
		this.InitOriginalRotation();
		Vector3 targetLocalEulers = (!faceUp) ? (this.m_RootObjectOriginalRotation.Value + this.m_DisabledRotation) : this.m_RootObjectOriginalRotation.Value;
		iTween.StopByName(this.m_RootObject, "flip");
		if (this.m_AnimateFlip)
		{
			Vector3 vector = (!faceUp) ? this.m_DisabledRotation : (-this.m_DisabledRotation);
			Hashtable args = iTween.Hash(new object[]
			{
				"amount",
				vector,
				"time",
				this.m_AnimateFlipTime,
				"easeType",
				iTween.EaseType.linear,
				"isLocal",
				true,
				"name",
				"flip",
				"oncomplete",
				delegate(object o)
				{
					this.m_RootObject.transform.localEulerAngles = targetLocalEulers;
				}
			});
			iTween.RotateAdd(this.m_RootObject, args);
		}
		else
		{
			this.m_RootObject.transform.localEulerAngles = targetLocalEulers;
		}
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x0009FB30 File Offset: 0x0009DD30
	public void SetText(string text)
	{
		if (this.m_ButtonText != null)
		{
			this.m_ButtonText.Text = text;
		}
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x0009FB50 File Offset: 0x0009DD50
	public string GetText()
	{
		return (!this.m_ButtonText.GameStringLookup) ? this.m_ButtonText.Text : GameStrings.Get(this.m_ButtonText.Text);
	}

	// Token: 0x060020B0 RID: 8368 RVA: 0x0009FB90 File Offset: 0x0009DD90
	private void Raise()
	{
		if (this.m_RootObject == null || !this.m_Depressed)
		{
			return;
		}
		this.m_Depressed = false;
		iTween.StopByName(this.m_RootObject, "depress");
		if (this.m_RaiseTime > 0f)
		{
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				this.m_RootObjectOriginalPosition,
				"time",
				this.m_RaiseTime,
				"easeType",
				iTween.EaseType.linear,
				"isLocal",
				true,
				"name",
				"depress"
			});
			iTween.MoveTo(this.m_RootObject, args);
		}
		else
		{
			this.m_RootObject.transform.localPosition = this.m_RootObjectOriginalPosition.Value;
		}
	}

	// Token: 0x060020B1 RID: 8369 RVA: 0x0009FC7C File Offset: 0x0009DE7C
	private void Depress()
	{
		if (this.m_RootObject == null || this.m_Depressed || UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		this.InitOriginalPosition();
		this.m_Depressed = true;
		iTween.StopByName(this.m_RootObject, "depress");
		Vector3 vector = this.m_RootObjectOriginalPosition.Value + this.m_ClickDownOffset;
		if (this.m_DepressTime > 0f)
		{
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				vector,
				"time",
				this.m_DepressTime,
				"easeType",
				iTween.EaseType.linear,
				"isLocal",
				true,
				"name",
				"depress"
			});
			iTween.MoveTo(this.m_RootObject, args);
		}
		else
		{
			this.m_RootObject.transform.localPosition = vector;
		}
	}

	// Token: 0x060020B2 RID: 8370 RVA: 0x0009FD84 File Offset: 0x0009DF84
	private void Wiggle()
	{
		if (this.m_RootObject == null || this.m_WiggleAmount.sqrMagnitude == 0f || this.m_WiggleTime <= 0f || UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		this.InitOriginalRotation();
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			this.m_WiggleAmount,
			"time",
			this.m_WiggleTime,
			"name",
			"wiggle"
		});
		iTween.StopByName(this.m_RootObject, "wiggle");
		this.m_RootObject.transform.localEulerAngles = this.m_RootObjectOriginalRotation.Value;
		iTween.PunchRotation(this.m_RootObject, args);
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x0009FE5C File Offset: 0x0009E05C
	private void InitOriginalRotation()
	{
		if (this.m_RootObject == null)
		{
			return;
		}
		if (this.m_RootObjectOriginalRotation == null)
		{
			this.m_RootObjectOriginalRotation = new Vector3?(this.m_RootObject.transform.localEulerAngles);
		}
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x0009FEA8 File Offset: 0x0009E0A8
	private void InitOriginalPosition()
	{
		if (this.m_RootObject == null)
		{
			return;
		}
		if (this.m_RootObjectOriginalPosition == null)
		{
			this.m_RootObjectOriginalPosition = new Vector3?(this.m_RootObject.transform.localPosition);
		}
	}

	// Token: 0x040011DA RID: 4570
	[CustomEditField(Sections = "Button Objects")]
	public GameObject m_RootObject;

	// Token: 0x040011DB RID: 4571
	[CustomEditField(Sections = "Text Object")]
	public UberText m_ButtonText;

	// Token: 0x040011DC RID: 4572
	[CustomEditField(Sections = "Click Depress Behavior")]
	public Vector3 m_ClickDownOffset = new Vector3(0f, -0.05f, 0f);

	// Token: 0x040011DD RID: 4573
	[CustomEditField(Sections = "Click Depress Behavior")]
	public float m_RaiseTime = 0.1f;

	// Token: 0x040011DE RID: 4574
	[CustomEditField(Sections = "Click Depress Behavior")]
	public float m_DepressTime = 0.1f;

	// Token: 0x040011DF RID: 4575
	[CustomEditField(Sections = "Roll Over Depress Behavior")]
	public bool m_DepressOnOver;

	// Token: 0x040011E0 RID: 4576
	[CustomEditField(Sections = "Wiggle Behavior")]
	public Vector3 m_WiggleAmount = new Vector3(90f, 0f, 0f);

	// Token: 0x040011E1 RID: 4577
	[CustomEditField(Sections = "Wiggle Behavior")]
	public float m_WiggleTime = 0.5f;

	// Token: 0x040011E2 RID: 4578
	[CustomEditField(Sections = "Flip Enable Behavior")]
	public Vector3 m_DisabledRotation = Vector3.zero;

	// Token: 0x040011E3 RID: 4579
	[CustomEditField(Sections = "Flip Enable Behavior")]
	public bool m_AnimateFlip;

	// Token: 0x040011E4 RID: 4580
	[CustomEditField(Sections = "Flip Enable Behavior")]
	public float m_AnimateFlipTime = 0.25f;

	// Token: 0x040011E5 RID: 4581
	private Vector3? m_RootObjectOriginalPosition;

	// Token: 0x040011E6 RID: 4582
	private Vector3? m_RootObjectOriginalRotation;

	// Token: 0x040011E7 RID: 4583
	private bool m_Depressed;
}
