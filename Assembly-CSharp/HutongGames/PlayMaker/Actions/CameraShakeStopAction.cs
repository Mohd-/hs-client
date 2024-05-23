using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB0 RID: 3504
	[Tooltip("Stops a shaking camera, over time or immediately.")]
	[ActionCategory("Pegasus")]
	public class CameraShakeStopAction : CameraAction
	{
		// Token: 0x06006CD6 RID: 27862 RVA: 0x00200288 File Offset: 0x001FE488
		public override void Reset()
		{
			this.m_WhichCamera = CameraAction.WhichCamera.MAIN;
			this.m_SpecificCamera = new FsmGameObject
			{
				UseVariable = true
			};
			this.m_NamedCamera = new FsmString
			{
				UseVariable = false
			};
			this.m_Time = 0f;
			this.m_Delay = 0f;
			this.m_FinishedEvent = null;
		}

		// Token: 0x06006CD7 RID: 27863 RVA: 0x002002EB File Offset: 0x001FE4EB
		public override void OnEnter()
		{
			this.m_timerSec = 0f;
			this.m_shakeStopped = false;
		}

		// Token: 0x06006CD8 RID: 27864 RVA: 0x00200300 File Offset: 0x001FE500
		public override void OnUpdate()
		{
			Camera camera = base.GetCamera(this.m_WhichCamera, this.m_SpecificCamera, this.m_NamedCamera);
			if (!camera)
			{
				Error.AddDevFatal("CameraShakeStopAction.OnUpdate() - Failed to get a camera. Owner={0}", new object[]
				{
					base.Owner
				});
				base.Finish();
			}
			this.m_timerSec += Time.deltaTime;
			float num = (!this.m_Delay.IsNone) ? this.m_Delay.Value : 0f;
			if (this.m_timerSec < num)
			{
				return;
			}
			if (!this.m_shakeStopped)
			{
				this.StopShake(camera);
			}
			if (CameraShakeMgr.IsShaking(camera))
			{
				return;
			}
			base.Fsm.Event(this.m_FinishedEvent);
			base.Finish();
		}

		// Token: 0x06006CD9 RID: 27865 RVA: 0x002003CC File Offset: 0x001FE5CC
		private void StopShake(Camera camera)
		{
			float time = (!this.m_Time.IsNone) ? this.m_Time.Value : 0f;
			CameraShakeMgr.Stop(camera, time);
			this.m_shakeStopped = true;
		}

		// Token: 0x0400557A RID: 21882
		public CameraAction.WhichCamera m_WhichCamera;

		// Token: 0x0400557B RID: 21883
		[CheckForComponent(typeof(Camera))]
		public FsmGameObject m_SpecificCamera;

		// Token: 0x0400557C RID: 21884
		public FsmString m_NamedCamera;

		// Token: 0x0400557D RID: 21885
		public FsmFloat m_Time;

		// Token: 0x0400557E RID: 21886
		public FsmFloat m_Delay;

		// Token: 0x0400557F RID: 21887
		public FsmEvent m_FinishedEvent;

		// Token: 0x04005580 RID: 21888
		private float m_timerSec;

		// Token: 0x04005581 RID: 21889
		private bool m_shakeStopped;
	}
}
