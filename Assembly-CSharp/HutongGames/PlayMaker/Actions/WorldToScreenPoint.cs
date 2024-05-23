using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1F RID: 3359
	[ActionCategory(22)]
	[Tooltip("Transforms position from world space into screen space. NOTE: Uses the MainCamera!")]
	public class WorldToScreenPoint : FsmStateAction
	{
		// Token: 0x06006A34 RID: 27188 RVA: 0x001F1610 File Offset: 0x001EF810
		public override void Reset()
		{
			this.worldPosition = null;
			this.worldX = new FsmFloat
			{
				UseVariable = true
			};
			this.worldY = new FsmFloat
			{
				UseVariable = true
			};
			this.worldZ = new FsmFloat
			{
				UseVariable = true
			};
			this.storeScreenPoint = null;
			this.storeScreenX = null;
			this.storeScreenY = null;
			this.everyFrame = false;
		}

		// Token: 0x06006A35 RID: 27189 RVA: 0x001F167C File Offset: 0x001EF87C
		public override void OnEnter()
		{
			this.DoWorldToScreenPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A36 RID: 27190 RVA: 0x001F1695 File Offset: 0x001EF895
		public override void OnUpdate()
		{
			this.DoWorldToScreenPoint();
		}

		// Token: 0x06006A37 RID: 27191 RVA: 0x001F16A0 File Offset: 0x001EF8A0
		private void DoWorldToScreenPoint()
		{
			if (Camera.main == null)
			{
				this.LogError("No MainCamera defined!");
				base.Finish();
				return;
			}
			Vector3 vector = Vector3.zero;
			if (!this.worldPosition.IsNone)
			{
				vector = this.worldPosition.Value;
			}
			if (!this.worldX.IsNone)
			{
				vector.x = this.worldX.Value;
			}
			if (!this.worldY.IsNone)
			{
				vector.y = this.worldY.Value;
			}
			if (!this.worldZ.IsNone)
			{
				vector.z = this.worldZ.Value;
			}
			vector = Camera.main.WorldToScreenPoint(vector);
			if (this.normalize.Value)
			{
				vector.x /= (float)Screen.width;
				vector.y /= (float)Screen.height;
			}
			this.storeScreenPoint.Value = vector;
			this.storeScreenX.Value = vector.x;
			this.storeScreenY.Value = vector.y;
		}

		// Token: 0x0400520D RID: 21005
		[Tooltip("World position to transform into screen coordinates.")]
		[UIHint(10)]
		public FsmVector3 worldPosition;

		// Token: 0x0400520E RID: 21006
		[Tooltip("World X position.")]
		public FsmFloat worldX;

		// Token: 0x0400520F RID: 21007
		[Tooltip("World Y position.")]
		public FsmFloat worldY;

		// Token: 0x04005210 RID: 21008
		[Tooltip("World Z position.")]
		public FsmFloat worldZ;

		// Token: 0x04005211 RID: 21009
		[Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero.")]
		[UIHint(10)]
		public FsmVector3 storeScreenPoint;

		// Token: 0x04005212 RID: 21010
		[Tooltip("Store the screen X position in a Float Variable.")]
		[UIHint(10)]
		public FsmFloat storeScreenX;

		// Token: 0x04005213 RID: 21011
		[UIHint(10)]
		[Tooltip("Store the screen Y position in a Float Variable.")]
		public FsmFloat storeScreenY;

		// Token: 0x04005214 RID: 21012
		[Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
		public FsmBool normalize;

		// Token: 0x04005215 RID: 21013
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
