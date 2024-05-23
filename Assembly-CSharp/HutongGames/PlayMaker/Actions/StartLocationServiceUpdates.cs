using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF4 RID: 3316
	[Tooltip("Starts location service updates. Last location coordinates can be retrieved with GetLocationInfo.")]
	[ActionCategory(33)]
	public class StartLocationServiceUpdates : FsmStateAction
	{
		// Token: 0x06006985 RID: 27013 RVA: 0x001EEDA4 File Offset: 0x001ECFA4
		public override void Reset()
		{
			this.maxWait = 20f;
			this.desiredAccuracy = 10f;
			this.updateDistance = 10f;
			this.successEvent = null;
			this.failedEvent = null;
		}

		// Token: 0x06006986 RID: 27014 RVA: 0x001EEDEF File Offset: 0x001ECFEF
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x06006987 RID: 27015 RVA: 0x001EEDF7 File Offset: 0x001ECFF7
		public override void OnUpdate()
		{
		}

		// Token: 0x04005143 RID: 20803
		[Tooltip("Maximum time to wait in seconds before failing.")]
		public FsmFloat maxWait;

		// Token: 0x04005144 RID: 20804
		public FsmFloat desiredAccuracy;

		// Token: 0x04005145 RID: 20805
		public FsmFloat updateDistance;

		// Token: 0x04005146 RID: 20806
		[Tooltip("Event to send when the location services have started.")]
		public FsmEvent successEvent;

		// Token: 0x04005147 RID: 20807
		[Tooltip("Event to send if the location services fail to start.")]
		public FsmEvent failedEvent;
	}
}
