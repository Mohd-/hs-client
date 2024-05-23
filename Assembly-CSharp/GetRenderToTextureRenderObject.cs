using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000DBB RID: 3515
[Tooltip("Get the object being rendered to from RenderToTexture")]
[ActionCategory("Pegasus")]
public class GetRenderToTextureRenderObject : FsmStateAction
{
	// Token: 0x06006D06 RID: 27910 RVA: 0x0020129C File Offset: 0x001FF49C
	[Tooltip("Get the object being rendered to from RenderToTexture. This is used to get the procedurally generated render plane object.")]
	public override void Reset()
	{
		this.gameObject = null;
		this.renderObject = null;
	}

	// Token: 0x06006D07 RID: 27911 RVA: 0x002012AC File Offset: 0x001FF4AC
	public override void OnEnter()
	{
		this.DoGetObject();
		base.Finish();
	}

	// Token: 0x06006D08 RID: 27912 RVA: 0x002012BC File Offset: 0x001FF4BC
	private void DoGetObject()
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
		this.renderObject.Value = component.GetRenderToObject();
	}

	// Token: 0x040055B8 RID: 21944
	[RequiredField]
	[CheckForComponent(typeof(RenderToTexture))]
	public FsmOwnerDefault gameObject;

	// Token: 0x040055B9 RID: 21945
	[RequiredField]
	[UIHint(10)]
	public FsmGameObject renderObject;
}
