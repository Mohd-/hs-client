using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B97 RID: 2967
	[Tooltip("Draws a line from a Start point in a direction. Specify the start point as Game Objects or Vector3 world positions. If both are specified, position is used as a local offset from the Object's position.")]
	[ActionCategory(2)]
	public class DrawDebugRay : FsmStateAction
	{
		// Token: 0x060063C8 RID: 25544 RVA: 0x001DBD20 File Offset: 0x001D9F20
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
			this.direction = new FsmVector3
			{
				UseVariable = true
			};
			this.color = Color.white;
		}

		// Token: 0x060063C9 RID: 25545 RVA: 0x001DBD7C File Offset: 0x001D9F7C
		public override void OnUpdate()
		{
			Vector3 position = ActionHelpers.GetPosition(this.fromObject, this.fromPosition);
			Debug.DrawRay(position, this.direction.Value, this.color.Value);
		}

		// Token: 0x04004B38 RID: 19256
		[Tooltip("Draw ray from a GameObject.")]
		public FsmGameObject fromObject;

		// Token: 0x04004B39 RID: 19257
		[Tooltip("Draw ray from a world position, or local offset from GameObject if provided.")]
		public FsmVector3 fromPosition;

		// Token: 0x04004B3A RID: 19258
		[Tooltip("Direction vector of ray.")]
		public FsmVector3 direction;

		// Token: 0x04004B3B RID: 19259
		[Tooltip("The color of the ray.")]
		public FsmColor color;
	}
}
