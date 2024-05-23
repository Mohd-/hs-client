using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B96 RID: 2966
	[Tooltip("Draws a line from a Start point to an End point. Specify the points as Game Objects or Vector3 world positions. If both are specified, position is used as a local offset from the Object's position.")]
	[ActionCategory(2)]
	public class DrawDebugLine : FsmStateAction
	{
		// Token: 0x060063C5 RID: 25541 RVA: 0x001DBC64 File Offset: 0x001D9E64
		public override void Reset()
		{
			this.fromObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.fromPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.toObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.toPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.color = Color.white;
		}

		// Token: 0x060063C6 RID: 25542 RVA: 0x001DBCD4 File Offset: 0x001D9ED4
		public override void OnUpdate()
		{
			Vector3 position = ActionHelpers.GetPosition(this.fromObject, this.fromPosition);
			Vector3 position2 = ActionHelpers.GetPosition(this.toObject, this.toPosition);
			Debug.DrawLine(position, position2, this.color.Value);
		}

		// Token: 0x04004B33 RID: 19251
		[Tooltip("Draw line from a GameObject.")]
		public FsmGameObject fromObject;

		// Token: 0x04004B34 RID: 19252
		[Tooltip("Draw line from a world position, or local offset from GameObject if provided.")]
		public FsmVector3 fromPosition;

		// Token: 0x04004B35 RID: 19253
		[Tooltip("Draw line to a GameObject.")]
		public FsmGameObject toObject;

		// Token: 0x04004B36 RID: 19254
		[Tooltip("Draw line to a world position, or local offset from GameObject if provided.")]
		public FsmVector3 toPosition;

		// Token: 0x04004B37 RID: 19255
		[Tooltip("The color of the line.")]
		public FsmColor color;
	}
}
