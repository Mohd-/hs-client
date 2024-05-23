using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C40 RID: 3136
	[ActionCategory("Mesh")]
	[Tooltip("Gets the position of a vertex in a GameObject's mesh. Hint: Use GetVertexCount to get the number of vertices in a mesh.")]
	public class GetVertexPosition : FsmStateAction
	{
		// Token: 0x06006669 RID: 26217 RVA: 0x001E4538 File Offset: 0x001E2738
		public override void Reset()
		{
			this.gameObject = null;
			this.space = 0;
			this.storePosition = null;
			this.everyFrame = false;
		}

		// Token: 0x0600666A RID: 26218 RVA: 0x001E4556 File Offset: 0x001E2756
		public override void OnEnter()
		{
			this.DoGetVertexPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600666B RID: 26219 RVA: 0x001E456F File Offset: 0x001E276F
		public override void OnUpdate()
		{
			this.DoGetVertexPosition();
		}

		// Token: 0x0600666C RID: 26220 RVA: 0x001E4578 File Offset: 0x001E2778
		private void DoGetVertexPosition()
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
				Space space = this.space;
				if (space != null)
				{
					if (space == 1)
					{
						this.storePosition.Value = component.mesh.vertices[this.vertexIndex.Value];
					}
				}
				else
				{
					Vector3 vector = component.mesh.vertices[this.vertexIndex.Value];
					this.storePosition.Value = ownerDefaultTarget.transform.TransformPoint(vector);
				}
			}
		}

		// Token: 0x04004E21 RID: 20001
		[CheckForComponent(typeof(MeshFilter))]
		[Tooltip("The GameObject to check.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E22 RID: 20002
		[Tooltip("The index of the vertex.")]
		[RequiredField]
		public FsmInt vertexIndex;

		// Token: 0x04004E23 RID: 20003
		[Tooltip("Coordinate system to use.")]
		public Space space;

		// Token: 0x04004E24 RID: 20004
		[Tooltip("Store the vertex position in a variable.")]
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 storePosition;

		// Token: 0x04004E25 RID: 20005
		[Tooltip("Repeat every frame. Useful if the mesh is animated.")]
		public bool everyFrame;
	}
}
