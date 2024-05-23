using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C36 RID: 3126
	[ActionCategory(31)]
	[Tooltip("Gets various useful Time measurements.")]
	public class GetTimeInfo : FsmStateAction
	{
		// Token: 0x0600663E RID: 26174 RVA: 0x001E3D01 File Offset: 0x001E1F01
		public override void Reset()
		{
			this.getInfo = GetTimeInfo.TimeInfo.TimeSinceLevelLoad;
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600663F RID: 26175 RVA: 0x001E3D18 File Offset: 0x001E1F18
		public override void OnEnter()
		{
			this.DoGetTimeInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006640 RID: 26176 RVA: 0x001E3D31 File Offset: 0x001E1F31
		public override void OnUpdate()
		{
			this.DoGetTimeInfo();
		}

		// Token: 0x06006641 RID: 26177 RVA: 0x001E3D3C File Offset: 0x001E1F3C
		private void DoGetTimeInfo()
		{
			switch (this.getInfo)
			{
			case GetTimeInfo.TimeInfo.DeltaTime:
				this.storeValue.Value = Time.deltaTime;
				break;
			case GetTimeInfo.TimeInfo.TimeScale:
				this.storeValue.Value = Time.timeScale;
				break;
			case GetTimeInfo.TimeInfo.SmoothDeltaTime:
				this.storeValue.Value = Time.smoothDeltaTime;
				break;
			case GetTimeInfo.TimeInfo.TimeInCurrentState:
				this.storeValue.Value = base.State.StateTime;
				break;
			case GetTimeInfo.TimeInfo.TimeSinceStartup:
				this.storeValue.Value = Time.time;
				break;
			case GetTimeInfo.TimeInfo.TimeSinceLevelLoad:
				this.storeValue.Value = Time.timeSinceLevelLoad;
				break;
			case GetTimeInfo.TimeInfo.RealTimeSinceStartup:
				this.storeValue.Value = FsmTime.RealtimeSinceStartup;
				break;
			case GetTimeInfo.TimeInfo.RealTimeInCurrentState:
				this.storeValue.Value = FsmTime.RealtimeSinceStartup - base.State.RealStartTime;
				break;
			default:
				this.storeValue.Value = 0f;
				break;
			}
		}

		// Token: 0x04004DF0 RID: 19952
		public GetTimeInfo.TimeInfo getInfo;

		// Token: 0x04004DF1 RID: 19953
		[UIHint(10)]
		[RequiredField]
		public FsmFloat storeValue;

		// Token: 0x04004DF2 RID: 19954
		public bool everyFrame;

		// Token: 0x02000C37 RID: 3127
		public enum TimeInfo
		{
			// Token: 0x04004DF4 RID: 19956
			DeltaTime,
			// Token: 0x04004DF5 RID: 19957
			TimeScale,
			// Token: 0x04004DF6 RID: 19958
			SmoothDeltaTime,
			// Token: 0x04004DF7 RID: 19959
			TimeInCurrentState,
			// Token: 0x04004DF8 RID: 19960
			TimeSinceStartup,
			// Token: 0x04004DF9 RID: 19961
			TimeSinceLevelLoad,
			// Token: 0x04004DFA RID: 19962
			RealTimeSinceStartup,
			// Token: 0x04004DFB RID: 19963
			RealTimeInCurrentState
		}
	}
}
