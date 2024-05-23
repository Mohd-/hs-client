using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C83 RID: 3203
	[ActionCategory(5)]
	[Tooltip("Scales the GUI around a pivot point. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class ScaleGUI : FsmStateAction
	{
		// Token: 0x06006793 RID: 26515 RVA: 0x001E8618 File Offset: 0x001E6818
		public override void Reset()
		{
			this.scaleX = 1f;
			this.scaleY = 1f;
			this.pivotX = 0.5f;
			this.pivotY = 0.5f;
			this.normalized = true;
			this.applyGlobally = false;
		}

		// Token: 0x06006794 RID: 26516 RVA: 0x001E8674 File Offset: 0x001E6874
		public override void OnGUI()
		{
			if (this.applied)
			{
				return;
			}
			Vector2 vector;
			vector..ctor(this.scaleX.Value, this.scaleY.Value);
			if (object.Equals(vector.x, 0))
			{
				vector.x = 0.0001f;
			}
			if (object.Equals(vector.y, 0))
			{
				vector.x = 0.0001f;
			}
			Vector2 vector2;
			vector2..ctor(this.pivotX.Value, this.pivotY.Value);
			if (this.normalized)
			{
				vector2.x *= (float)Screen.width;
				vector2.y *= (float)Screen.height;
			}
			GUIUtility.ScaleAroundPivot(vector, vector2);
			if (this.applyGlobally)
			{
				PlayMakerGUI.GUIMatrix = GUI.matrix;
				this.applied = true;
			}
		}

		// Token: 0x06006795 RID: 26517 RVA: 0x001E876D File Offset: 0x001E696D
		public override void OnUpdate()
		{
			this.applied = false;
		}

		// Token: 0x04004F5B RID: 20315
		[RequiredField]
		public FsmFloat scaleX;

		// Token: 0x04004F5C RID: 20316
		[RequiredField]
		public FsmFloat scaleY;

		// Token: 0x04004F5D RID: 20317
		[RequiredField]
		public FsmFloat pivotX;

		// Token: 0x04004F5E RID: 20318
		[RequiredField]
		public FsmFloat pivotY;

		// Token: 0x04004F5F RID: 20319
		[Tooltip("Pivot point uses normalized coordinates. E.g. 0.5 is the center of the screen.")]
		public bool normalized;

		// Token: 0x04004F60 RID: 20320
		public bool applyGlobally;

		// Token: 0x04004F61 RID: 20321
		private bool applied;
	}
}
