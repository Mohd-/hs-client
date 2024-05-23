using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3F RID: 3135
	[ActionCategory("Mesh")]
	[Tooltip("Gets the number of vertices in a GameObject's mesh. Useful in conjunction with GetVertexPosition.")]
	public class GetVertexCount : FsmStateAction
	{
		// Token: 0x06006664 RID: 26212 RVA: 0x001E4497 File Offset: 0x001E2697
		public override void Reset()
		{
			this.gameObject = null;
			this.storeCount = null;
			this.everyFrame = false;
		}

		// Token: 0x06006665 RID: 26213 RVA: 0x001E44AE File Offset: 0x001E26AE
		public override void OnEnter()
		{
			this.DoGetVertexCount();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006666 RID: 26214 RVA: 0x001E44C7 File Offset: 0x001E26C7
		public override void OnUpdate()
		{
			this.DoGetVertexCount();
		}

		// Token: 0x06006667 RID: 26215 RVA: 0x001E44D0 File Offset: 0x001E26D0
		private void DoGetVertexCount()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				MeshFilter component = ownerDefaultTarget.GetComponent<MeshFilter>();
				if (component == null)
				{
					this.LogError("Missing MeshFilter!");
					return;
				}
				this.storeCount.Value = component.mesh.vertexCount;
			}
		}

		// Token: 0x04004E1E RID: 19998
		[Tooltip("The GameObject to check.")]
		[CheckForComponent(typeof(MeshFilter))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E1F RID: 19999
		[Tooltip("Store the vertex count in a variable.")]
		[RequiredField]
		[UIHint(10)]
		public FsmInt storeCount;

		// Token: 0x04004E20 RID: 20000
		public bool everyFrame;
	}
}
