using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB1 RID: 3505
	[ActionCategory("Pegasus")]
	[Tooltip("Shakes a camera over time.")]
	public class CameraShakerAction : CameraAction
	{
		// Token: 0x06006CDB RID: 27867 RVA: 0x00200418 File Offset: 0x001FE618
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
			this.m_Amount = new FsmVector3
			{
				UseVariable = false
			};
			this.m_IntensityCurve = null;
			this.m_Delay = 0f;
			this.m_HoldAtTime = new FsmFloat
			{
				UseVariable = true
			};
			this.m_FinishedEvent = null;
		}

		// Token: 0x06006CDC RID: 27868 RVA: 0x0020049A File Offset: 0x001FE69A
		public override void OnEnter()
		{
			this.m_timerSec = 0f;
			this.m_shakeFired = false;
		}

		// Token: 0x06006CDD RID: 27869 RVA: 0x002004B0 File Offset: 0x001FE6B0
		public override void OnUpdate()
		{
			Camera camera = base.GetCamera(this.m_WhichCamera, this.m_SpecificCamera, this.m_NamedCamera);
			if (!camera)
			{
				Error.AddDevFatal("CameraShakerAction.OnUpdate() - Failed to get a camera. Owner={0}", new object[]
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
			if (!this.m_shakeFired)
			{
				if (this.m_IntensityCurve == null || this.m_IntensityCurve.curve == null)
				{
					base.Fsm.Event(this.m_FinishedEvent);
					base.Finish();
					return;
				}
				this.Shake(camera);
			}
			if (CameraShakeMgr.IsShaking(camera))
			{
				return;
			}
			base.Fsm.Event(this.m_FinishedEvent);
			base.Finish();
		}

		// Token: 0x06006CDE RID: 27870 RVA: 0x002005B0 File Offset: 0x001FE7B0
		private void Shake(Camera camera)
		{
			Vector3 amount = (!this.m_Amount.IsNone) ? this.m_Amount.Value : Vector3.zero;
			AnimationCurve curve = this.m_IntensityCurve.curve;
			float? holdAtTime = default(float?);
			if (!this.m_HoldAtTime.IsNone)
			{
				holdAtTime = new float?(this.m_HoldAtTime.Value);
			}
			CameraShakeMgr.Shake(camera, amount, curve, holdAtTime);
			this.m_shakeFired = true;
		}

		// Token: 0x04005582 RID: 21890
		public CameraAction.WhichCamera m_WhichCamera;

		// Token: 0x04005583 RID: 21891
		[CheckForComponent(typeof(Camera))]
		public FsmGameObject m_SpecificCamera;

		// Token: 0x04005584 RID: 21892
		public FsmString m_NamedCamera;

		// Token: 0x04005585 RID: 21893
		public FsmVector3 m_Amount;

		// Token: 0x04005586 RID: 21894
		[RequiredField]
		public FsmAnimationCurve m_IntensityCurve;

		// Token: 0x04005587 RID: 21895
		public FsmFloat m_Delay;

		// Token: 0x04005588 RID: 21896
		[Tooltip("[Optional] Hold the shake forever once the shake passes this time.")]
		public FsmFloat m_HoldAtTime;

		// Token: 0x04005589 RID: 21897
		public FsmEvent m_FinishedEvent;

		// Token: 0x0400558A RID: 21898
		private float m_timerSec;

		// Token: 0x0400558B RID: 21899
		private bool m_shakeFired;
	}
}
