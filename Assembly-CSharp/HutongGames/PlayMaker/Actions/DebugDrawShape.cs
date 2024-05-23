using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B84 RID: 2948
	[ActionCategory(2)]
	[Tooltip("Draw gizmos shape")]
	public class DebugDrawShape : FsmStateAction
	{
		// Token: 0x0600638D RID: 25485 RVA: 0x001DB4E4 File Offset: 0x001D96E4
		public override void Reset()
		{
			this.gameObject = null;
			this.shape = DebugDrawShape.ShapeType.Sphere;
			this.color = Color.grey;
			this.radius = 1f;
			this.size = new Vector3(1f, 1f, 1f);
		}

		// Token: 0x0600638E RID: 25486 RVA: 0x001DB540 File Offset: 0x001D9740
		public override void OnDrawGizmos()
		{
			Transform transform = base.Fsm.GetOwnerDefaultTarget(this.gameObject).transform;
			if (transform == null)
			{
				return;
			}
			Gizmos.color = this.color.Value;
			switch (this.shape)
			{
			case DebugDrawShape.ShapeType.Sphere:
				Gizmos.DrawSphere(transform.position, this.radius.Value);
				break;
			case DebugDrawShape.ShapeType.Cube:
				Gizmos.DrawCube(transform.position, this.size.Value);
				break;
			case DebugDrawShape.ShapeType.WireSphere:
				Gizmos.DrawWireSphere(transform.position, this.radius.Value);
				break;
			case DebugDrawShape.ShapeType.WireCube:
				Gizmos.DrawWireCube(transform.position, this.size.Value);
				break;
			}
		}

		// Token: 0x04004B0A RID: 19210
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004B0B RID: 19211
		public DebugDrawShape.ShapeType shape;

		// Token: 0x04004B0C RID: 19212
		public FsmColor color;

		// Token: 0x04004B0D RID: 19213
		[Tooltip("Use this for sphere gizmos")]
		public FsmFloat radius;

		// Token: 0x04004B0E RID: 19214
		[Tooltip("Use this for cube gizmos")]
		public FsmVector3 size;

		// Token: 0x02000B85 RID: 2949
		public enum ShapeType
		{
			// Token: 0x04004B10 RID: 19216
			Sphere,
			// Token: 0x04004B11 RID: 19217
			Cube,
			// Token: 0x04004B12 RID: 19218
			WireSphere,
			// Token: 0x04004B13 RID: 19219
			WireCube
		}
	}
}
