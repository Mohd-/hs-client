using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C80 RID: 3200
	[ActionCategory(5)]
	[Tooltip("Rotates the GUI around a pivot point. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class RotateGUI : FsmStateAction
	{
		// Token: 0x06006778 RID: 26488 RVA: 0x001E81A0 File Offset: 0x001E63A0
		public override void Reset()
		{
			this.angle = 0f;
			this.pivotX = 0.5f;
			this.pivotY = 0.5f;
			this.normalized = true;
			this.applyGlobally = false;
		}

		// Token: 0x06006779 RID: 26489 RVA: 0x001E81EC File Offset: 0x001E63EC
		public override void OnGUI()
		{
			if (this.applied)
			{
				return;
			}
			Vector2 vector;
			vector..ctor(this.pivotX.Value, this.pivotY.Value);
			if (this.normalized)
			{
				vector.x *= (float)Screen.width;
				vector.y *= (float)Screen.height;
			}
			GUIUtility.RotateAroundPivot(this.angle.Value, vector);
			if (this.applyGlobally)
			{
				PlayMakerGUI.GUIMatrix = GUI.matrix;
				this.applied = true;
			}
		}

		// Token: 0x0600677A RID: 26490 RVA: 0x001E8282 File Offset: 0x001E6482
		public override void OnUpdate()
		{
			this.applied = false;
		}

		// Token: 0x04004F4D RID: 20301
		[RequiredField]
		public FsmFloat angle;

		// Token: 0x04004F4E RID: 20302
		[RequiredField]
		public FsmFloat pivotX;

		// Token: 0x04004F4F RID: 20303
		[RequiredField]
		public FsmFloat pivotY;

		// Token: 0x04004F50 RID: 20304
		public bool normalized;

		// Token: 0x04004F51 RID: 20305
		public bool applyGlobally;

		// Token: 0x04004F52 RID: 20306
		private bool applied;
	}
}
