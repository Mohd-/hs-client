using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C14 RID: 3092
	[ActionCategory(33)]
	[Tooltip("Gets Location Info from a mobile device. NOTE: Use StartLocationService before trying to get location info.")]
	public class GetLocationInfo : FsmStateAction
	{
		// Token: 0x060065B1 RID: 26033 RVA: 0x001E27A8 File Offset: 0x001E09A8
		public override void Reset()
		{
			this.longitude = null;
			this.latitude = null;
			this.altitude = null;
			this.horizontalAccuracy = null;
			this.verticalAccuracy = null;
			this.errorEvent = null;
		}

		// Token: 0x060065B2 RID: 26034 RVA: 0x001E27DF File Offset: 0x001E09DF
		public override void OnEnter()
		{
			this.DoGetLocationInfo();
			base.Finish();
		}

		// Token: 0x060065B3 RID: 26035 RVA: 0x001E27ED File Offset: 0x001E09ED
		private void DoGetLocationInfo()
		{
		}

		// Token: 0x04004D7D RID: 19837
		[UIHint(10)]
		public FsmVector3 vectorPosition;

		// Token: 0x04004D7E RID: 19838
		[UIHint(10)]
		public FsmFloat longitude;

		// Token: 0x04004D7F RID: 19839
		[UIHint(10)]
		public FsmFloat latitude;

		// Token: 0x04004D80 RID: 19840
		[UIHint(10)]
		public FsmFloat altitude;

		// Token: 0x04004D81 RID: 19841
		[UIHint(10)]
		public FsmFloat horizontalAccuracy;

		// Token: 0x04004D82 RID: 19842
		[UIHint(10)]
		public FsmFloat verticalAccuracy;

		// Token: 0x04004D83 RID: 19843
		[Tooltip("Event to send if the location cannot be queried.")]
		public FsmEvent errorEvent;
	}
}
