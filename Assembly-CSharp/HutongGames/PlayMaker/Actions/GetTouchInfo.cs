using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C39 RID: 3129
	[Tooltip("Gets info on a touch event.")]
	[ActionCategory(33)]
	public class GetTouchInfo : FsmStateAction
	{
		// Token: 0x06006648 RID: 26184 RVA: 0x001E3EA4 File Offset: 0x001E20A4
		public override void Reset()
		{
			this.fingerId = new FsmInt
			{
				UseVariable = true
			};
			this.normalize = true;
			this.storePosition = null;
			this.storeDeltaPosition = null;
			this.storeDeltaTime = null;
			this.storeTapCount = null;
			this.everyFrame = true;
		}

		// Token: 0x06006649 RID: 26185 RVA: 0x001E3EF4 File Offset: 0x001E20F4
		public override void OnEnter()
		{
			this.screenWidth = (float)Screen.width;
			this.screenHeight = (float)Screen.height;
			this.DoGetTouchInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600664A RID: 26186 RVA: 0x001E3F25 File Offset: 0x001E2125
		public override void OnUpdate()
		{
			this.DoGetTouchInfo();
		}

		// Token: 0x0600664B RID: 26187 RVA: 0x001E3F30 File Offset: 0x001E2130
		private void DoGetTouchInfo()
		{
			if (Input.touchCount > 0)
			{
				foreach (Touch touch in Input.touches)
				{
					if (this.fingerId.IsNone || touch.fingerId == this.fingerId.Value)
					{
						float num = this.normalize.Value ? (touch.position.x / this.screenWidth) : touch.position.x;
						float num2 = this.normalize.Value ? (touch.position.y / this.screenHeight) : touch.position.y;
						if (!this.storePosition.IsNone)
						{
							this.storePosition.Value = new Vector3(num, num2, 0f);
						}
						this.storeX.Value = num;
						this.storeY.Value = num2;
						float num3 = this.normalize.Value ? (touch.deltaPosition.x / this.screenWidth) : touch.deltaPosition.x;
						float num4 = this.normalize.Value ? (touch.deltaPosition.y / this.screenHeight) : touch.deltaPosition.y;
						if (!this.storeDeltaPosition.IsNone)
						{
							this.storeDeltaPosition.Value = new Vector3(num3, num4, 0f);
						}
						this.storeDeltaX.Value = num3;
						this.storeDeltaY.Value = num4;
						this.storeDeltaTime.Value = touch.deltaTime;
						this.storeTapCount.Value = touch.tapCount;
					}
				}
			}
		}

		// Token: 0x04004DFE RID: 19966
		[Tooltip("Filter by a Finger ID. You can store a Finger ID in other Touch actions, e.g., Touch Event.")]
		public FsmInt fingerId;

		// Token: 0x04004DFF RID: 19967
		[Tooltip("If true, all screen coordinates are returned normalized (0-1), otherwise in pixels.")]
		public FsmBool normalize;

		// Token: 0x04004E00 RID: 19968
		[UIHint(10)]
		public FsmVector3 storePosition;

		// Token: 0x04004E01 RID: 19969
		[UIHint(10)]
		public FsmFloat storeX;

		// Token: 0x04004E02 RID: 19970
		[UIHint(10)]
		public FsmFloat storeY;

		// Token: 0x04004E03 RID: 19971
		[UIHint(10)]
		public FsmVector3 storeDeltaPosition;

		// Token: 0x04004E04 RID: 19972
		[UIHint(10)]
		public FsmFloat storeDeltaX;

		// Token: 0x04004E05 RID: 19973
		[UIHint(10)]
		public FsmFloat storeDeltaY;

		// Token: 0x04004E06 RID: 19974
		[UIHint(10)]
		public FsmFloat storeDeltaTime;

		// Token: 0x04004E07 RID: 19975
		[UIHint(10)]
		public FsmInt storeTapCount;

		// Token: 0x04004E08 RID: 19976
		public bool everyFrame = true;

		// Token: 0x04004E09 RID: 19977
		private float screenWidth;

		// Token: 0x04004E0A RID: 19978
		private float screenHeight;
	}
}
