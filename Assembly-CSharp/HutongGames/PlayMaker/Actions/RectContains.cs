using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C79 RID: 3193
	[Tooltip("Tests if a point is inside a rectangle.")]
	[ActionCategory(39)]
	public class RectContains : FsmStateAction
	{
		// Token: 0x0600675C RID: 26460 RVA: 0x001E7C7C File Offset: 0x001E5E7C
		public override void Reset()
		{
			this.rectangle = new FsmRect
			{
				UseVariable = true
			};
			this.point = new FsmVector3
			{
				UseVariable = true
			};
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.storeResult = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x0600675D RID: 26461 RVA: 0x001E7CF5 File Offset: 0x001E5EF5
		public override void OnEnter()
		{
			this.DoRectContains();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600675E RID: 26462 RVA: 0x001E7D0E File Offset: 0x001E5F0E
		public override void OnUpdate()
		{
			this.DoRectContains();
		}

		// Token: 0x0600675F RID: 26463 RVA: 0x001E7D18 File Offset: 0x001E5F18
		private void DoRectContains()
		{
			if (this.rectangle.IsNone)
			{
				return;
			}
			Vector3 value = this.point.Value;
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			bool flag = this.rectangle.Value.Contains(value);
			this.storeResult.Value = flag;
			base.Fsm.Event((!flag) ? this.falseEvent : this.trueEvent);
		}

		// Token: 0x04004F36 RID: 20278
		[Tooltip("Rectangle to test.")]
		[RequiredField]
		public FsmRect rectangle;

		// Token: 0x04004F37 RID: 20279
		[Tooltip("Point to test.")]
		public FsmVector3 point;

		// Token: 0x04004F38 RID: 20280
		[Tooltip("Specify/override X value.")]
		public FsmFloat x;

		// Token: 0x04004F39 RID: 20281
		[Tooltip("Specify/override Y value.")]
		public FsmFloat y;

		// Token: 0x04004F3A RID: 20282
		[Tooltip("Event to send if the Point is inside the Rectangle.")]
		public FsmEvent trueEvent;

		// Token: 0x04004F3B RID: 20283
		[Tooltip("Event to send if the Point is outside the Rectangle.")]
		public FsmEvent falseEvent;

		// Token: 0x04004F3C RID: 20284
		[UIHint(10)]
		[Tooltip("Store the result in a variable.")]
		public FsmBool storeResult;

		// Token: 0x04004F3D RID: 20285
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
