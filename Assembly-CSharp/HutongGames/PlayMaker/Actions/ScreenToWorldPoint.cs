using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C86 RID: 3206
	[ActionCategory(22)]
	[Tooltip("Transforms position from screen space into world space. NOTE: Uses the MainCamera!")]
	public class ScreenToWorldPoint : FsmStateAction
	{
		// Token: 0x060067A1 RID: 26529 RVA: 0x001E8A90 File Offset: 0x001E6C90
		public override void Reset()
		{
			this.screenVector = null;
			this.screenX = new FsmFloat
			{
				UseVariable = true
			};
			this.screenY = new FsmFloat
			{
				UseVariable = true
			};
			this.screenZ = 1f;
			this.normalized = false;
			this.storeWorldVector = null;
			this.storeWorldX = null;
			this.storeWorldY = null;
			this.storeWorldZ = null;
			this.everyFrame = false;
		}

		// Token: 0x060067A2 RID: 26530 RVA: 0x001E8B0B File Offset: 0x001E6D0B
		public override void OnEnter()
		{
			this.DoScreenToWorldPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067A3 RID: 26531 RVA: 0x001E8B24 File Offset: 0x001E6D24
		public override void OnUpdate()
		{
			this.DoScreenToWorldPoint();
		}

		// Token: 0x060067A4 RID: 26532 RVA: 0x001E8B2C File Offset: 0x001E6D2C
		private void DoScreenToWorldPoint()
		{
			if (Camera.main == null)
			{
				this.LogError("No MainCamera defined!");
				base.Finish();
				return;
			}
			Vector3 vector = Vector3.zero;
			if (!this.screenVector.IsNone)
			{
				vector = this.screenVector.Value;
			}
			if (!this.screenX.IsNone)
			{
				vector.x = this.screenX.Value;
			}
			if (!this.screenY.IsNone)
			{
				vector.y = this.screenY.Value;
			}
			if (!this.screenZ.IsNone)
			{
				vector.z = this.screenZ.Value;
			}
			if (this.normalized.Value)
			{
				vector.x *= (float)Screen.width;
				vector.y *= (float)Screen.height;
			}
			vector = Camera.main.ScreenToWorldPoint(vector);
			this.storeWorldVector.Value = vector;
			this.storeWorldX.Value = vector.x;
			this.storeWorldY.Value = vector.y;
			this.storeWorldZ.Value = vector.z;
		}

		// Token: 0x04004F72 RID: 20338
		[Tooltip("Screen position as a vector.")]
		[UIHint(10)]
		public FsmVector3 screenVector;

		// Token: 0x04004F73 RID: 20339
		[Tooltip("Screen X position in pixels or normalized. See Normalized.")]
		public FsmFloat screenX;

		// Token: 0x04004F74 RID: 20340
		[Tooltip("Screen X position in pixels or normalized. See Normalized.")]
		public FsmFloat screenY;

		// Token: 0x04004F75 RID: 20341
		[Tooltip("Distance into the screen in world units.")]
		public FsmFloat screenZ;

		// Token: 0x04004F76 RID: 20342
		[Tooltip("If true, X/Y coordinates are considered normalized (0-1), otherwise they are expected to be in pixels")]
		public FsmBool normalized;

		// Token: 0x04004F77 RID: 20343
		[Tooltip("Store the world position in a vector3 variable.")]
		[UIHint(10)]
		public FsmVector3 storeWorldVector;

		// Token: 0x04004F78 RID: 20344
		[Tooltip("Store the world X position in a float variable.")]
		[UIHint(10)]
		public FsmFloat storeWorldX;

		// Token: 0x04004F79 RID: 20345
		[UIHint(10)]
		[Tooltip("Store the world Y position in a float variable.")]
		public FsmFloat storeWorldY;

		// Token: 0x04004F7A RID: 20346
		[UIHint(10)]
		[Tooltip("Store the world Z position in a float variable.")]
		public FsmFloat storeWorldZ;

		// Token: 0x04004F7B RID: 20347
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
