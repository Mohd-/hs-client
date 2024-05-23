using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D10 RID: 3344
	[ActionCategory(19)]
	[Tooltip("Interpolates between 2 Vector3 values over a specified Time.")]
	public class Vector3Interpolate : FsmStateAction
	{
		// Token: 0x060069FA RID: 27130 RVA: 0x001F08B4 File Offset: 0x001EEAB4
		public override void Reset()
		{
			this.mode = 0;
			this.fromVector = new FsmVector3
			{
				UseVariable = true
			};
			this.toVector = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.storeResult = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x060069FB RID: 27131 RVA: 0x001F0918 File Offset: 0x001EEB18
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			if (this.storeResult == null)
			{
				base.Finish();
			}
			else
			{
				this.storeResult.Value = this.fromVector.Value;
			}
		}

		// Token: 0x060069FC RID: 27132 RVA: 0x001F0968 File Offset: 0x001EEB68
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.currentTime += Time.deltaTime;
			}
			float num = this.currentTime / this.time.Value;
			InterpolationType interpolationType = this.mode;
			if (interpolationType != null)
			{
				if (interpolationType == 1)
				{
					num = Mathf.SmoothStep(0f, 1f, num);
				}
			}
			this.storeResult.Value = Vector3.Lerp(this.fromVector.Value, this.toVector.Value, num);
			if (num > 1f)
			{
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				base.Finish();
			}
		}

		// Token: 0x040051C8 RID: 20936
		public InterpolationType mode;

		// Token: 0x040051C9 RID: 20937
		[RequiredField]
		public FsmVector3 fromVector;

		// Token: 0x040051CA RID: 20938
		[RequiredField]
		public FsmVector3 toVector;

		// Token: 0x040051CB RID: 20939
		[RequiredField]
		public FsmFloat time;

		// Token: 0x040051CC RID: 20940
		[UIHint(10)]
		[RequiredField]
		public FsmVector3 storeResult;

		// Token: 0x040051CD RID: 20941
		public FsmEvent finishEvent;

		// Token: 0x040051CE RID: 20942
		[Tooltip("Ignore TimeScale")]
		public bool realTime;

		// Token: 0x040051CF RID: 20943
		private float startTime;

		// Token: 0x040051D0 RID: 20944
		private float currentTime;
	}
}
