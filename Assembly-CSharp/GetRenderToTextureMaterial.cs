using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000DBA RID: 3514
[ActionCategory("Pegasus")]
[Tooltip("Get the material instance from an object with RenderToTexture")]
public class GetRenderToTextureMaterial : FsmStateAction
{
	// Token: 0x06006D02 RID: 27906 RVA: 0x00201217 File Offset: 0x001FF417
	[Tooltip("Get the material instance from an object with RenderToTexture. This is used to get the material of the procedurally generated render plane.")]
	public override void Reset()
	{
		this.gameObject = null;
		this.material = null;
	}

	// Token: 0x06006D03 RID: 27907 RVA: 0x00201227 File Offset: 0x001FF427
	public override void OnEnter()
	{
		this.DoGetMaterial();
		base.Finish();
	}

	// Token: 0x06006D04 RID: 27908 RVA: 0x00201238 File Offset: 0x001FF438
	private void DoGetMaterial()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
		if (ownerDefaultTarget == null)
		{
			return;
		}
		RenderToTexture component = ownerDefaultTarget.GetComponent<RenderToTexture>();
		if (component == null)
		{
			this.LogError("Missing RenderToTexture component!");
			return;
		}
		this.material.Value = component.GetRenderMaterial();
	}

	// Token: 0x040055B6 RID: 21942
	[CheckForComponent(typeof(RenderToTexture))]
	[RequiredField]
	public FsmOwnerDefault gameObject;

	// Token: 0x040055B7 RID: 21943
	[RequiredField]
	[UIHint(10)]
	public FsmMaterial material;
}
