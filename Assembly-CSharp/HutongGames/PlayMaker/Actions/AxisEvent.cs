using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5B RID: 2907
	[Tooltip("Sends events based on the direction of Input Axis (Left/Right/Up/Down...).")]
	[ActionCategory(6)]
	public class AxisEvent : FsmStateAction
	{
		// Token: 0x060062D2 RID: 25298 RVA: 0x001D8A8C File Offset: 0x001D6C8C
		public override void Reset()
		{
			this.horizontalAxis = "Horizontal";
			this.verticalAxis = "Vertical";
			this.leftEvent = null;
			this.rightEvent = null;
			this.upEvent = null;
			this.downEvent = null;
			this.anyDirection = null;
			this.noDirection = null;
		}

		// Token: 0x060062D3 RID: 25299 RVA: 0x001D8AE4 File Offset: 0x001D6CE4
		public override void OnUpdate()
		{
			float num = (!(this.horizontalAxis.Value != string.Empty)) ? 0f : Input.GetAxis(this.horizontalAxis.Value);
			float num2 = (!(this.verticalAxis.Value != string.Empty)) ? 0f : Input.GetAxis(this.verticalAxis.Value);
			if ((num * num + num2 * num2).Equals(0f))
			{
				if (this.noDirection != null)
				{
					base.Fsm.Event(this.noDirection);
				}
				return;
			}
			float num3 = Mathf.Atan2(num2, num) * 57.29578f + 45f;
			if (num3 < 0f)
			{
				num3 += 360f;
			}
			int num4 = (int)(num3 / 90f);
			if (num4 == 0 && this.rightEvent != null)
			{
				base.Fsm.Event(this.rightEvent);
			}
			else if (num4 == 1 && this.upEvent != null)
			{
				base.Fsm.Event(this.upEvent);
			}
			else if (num4 == 2 && this.leftEvent != null)
			{
				base.Fsm.Event(this.leftEvent);
			}
			else if (num4 == 3 && this.downEvent != null)
			{
				base.Fsm.Event(this.downEvent);
			}
			else if (this.anyDirection != null)
			{
				base.Fsm.Event(this.anyDirection);
			}
		}

		// Token: 0x04004A40 RID: 19008
		[Tooltip("Horizontal axis as defined in the Input Manager")]
		public FsmString horizontalAxis;

		// Token: 0x04004A41 RID: 19009
		[Tooltip("Vertical axis as defined in the Input Manager")]
		public FsmString verticalAxis;

		// Token: 0x04004A42 RID: 19010
		[Tooltip("Event to send if input is to the left.")]
		public FsmEvent leftEvent;

		// Token: 0x04004A43 RID: 19011
		[Tooltip("Event to send if input is to the right.")]
		public FsmEvent rightEvent;

		// Token: 0x04004A44 RID: 19012
		[Tooltip("Event to send if input is to the up.")]
		public FsmEvent upEvent;

		// Token: 0x04004A45 RID: 19013
		[Tooltip("Event to send if input is to the down.")]
		public FsmEvent downEvent;

		// Token: 0x04004A46 RID: 19014
		[Tooltip("Event to send if input is in any direction.")]
		public FsmEvent anyDirection;

		// Token: 0x04004A47 RID: 19015
		[Tooltip("Event to send if no axis input (centered).")]
		public FsmEvent noDirection;
	}
}
