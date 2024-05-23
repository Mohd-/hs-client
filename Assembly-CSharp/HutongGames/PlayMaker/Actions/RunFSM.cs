using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C81 RID: 3201
	[Tooltip("Creates an FSM from a saved FSM Template.")]
	[ActionCategory(12)]
	public class RunFSM : FsmStateAction
	{
		// Token: 0x0600677C RID: 26492 RVA: 0x001E829E File Offset: 0x001E649E
		public override void Reset()
		{
			this.fsmTemplateControl = new FsmTemplateControl();
			this.storeID = null;
			this.runFsm = null;
		}

		// Token: 0x0600677D RID: 26493 RVA: 0x001E82B9 File Offset: 0x001E64B9
		public override void Awake()
		{
			if (this.fsmTemplateControl.fsmTemplate != null && Application.isPlaying)
			{
				this.runFsm = base.Fsm.CreateSubFsm(this.fsmTemplateControl);
			}
		}

		// Token: 0x0600677E RID: 26494 RVA: 0x001E82F4 File Offset: 0x001E64F4
		public override bool Event(FsmEvent fsmEvent)
		{
			if (this.runFsm != null && (fsmEvent.IsGlobal || fsmEvent.IsSystemEvent))
			{
				this.runFsm.Event(fsmEvent);
			}
			return false;
		}

		// Token: 0x0600677F RID: 26495 RVA: 0x001E8330 File Offset: 0x001E6530
		public override void OnEnter()
		{
			if (this.runFsm == null)
			{
				base.Finish();
				return;
			}
			this.fsmTemplateControl.UpdateValues();
			this.fsmTemplateControl.ApplyOverrides(this.runFsm);
			this.runFsm.OnEnable();
			if (!this.runFsm.Started)
			{
				this.runFsm.Start();
			}
			this.storeID.Value = this.fsmTemplateControl.ID;
			this.CheckIfFinished();
		}

		// Token: 0x06006780 RID: 26496 RVA: 0x001E83AD File Offset: 0x001E65AD
		public override void OnUpdate()
		{
			if (this.runFsm != null)
			{
				this.runFsm.Update();
				this.CheckIfFinished();
			}
			else
			{
				base.Finish();
			}
		}

		// Token: 0x06006781 RID: 26497 RVA: 0x001E83D6 File Offset: 0x001E65D6
		public override void OnFixedUpdate()
		{
			if (this.runFsm != null)
			{
				this.runFsm.FixedUpdate();
				this.CheckIfFinished();
			}
			else
			{
				base.Finish();
			}
		}

		// Token: 0x06006782 RID: 26498 RVA: 0x001E83FF File Offset: 0x001E65FF
		public override void OnLateUpdate()
		{
			if (this.runFsm != null)
			{
				this.runFsm.LateUpdate();
				this.CheckIfFinished();
			}
			else
			{
				base.Finish();
			}
		}

		// Token: 0x06006783 RID: 26499 RVA: 0x001E8428 File Offset: 0x001E6628
		public override void DoTriggerEnter(Collider other)
		{
			if (this.runFsm.HandleTriggerEnter)
			{
				this.runFsm.OnTriggerEnter(other);
			}
		}

		// Token: 0x06006784 RID: 26500 RVA: 0x001E8446 File Offset: 0x001E6646
		public override void DoTriggerStay(Collider other)
		{
			if (this.runFsm.HandleTriggerStay)
			{
				this.runFsm.OnTriggerStay(other);
			}
		}

		// Token: 0x06006785 RID: 26501 RVA: 0x001E8464 File Offset: 0x001E6664
		public override void DoTriggerExit(Collider other)
		{
			if (this.runFsm.HandleTriggerExit)
			{
				this.runFsm.OnTriggerExit(other);
			}
		}

		// Token: 0x06006786 RID: 26502 RVA: 0x001E8482 File Offset: 0x001E6682
		public override void DoCollisionEnter(Collision collisionInfo)
		{
			if (this.runFsm.HandleCollisionEnter)
			{
				this.runFsm.OnCollisionEnter(collisionInfo);
			}
		}

		// Token: 0x06006787 RID: 26503 RVA: 0x001E84A0 File Offset: 0x001E66A0
		public override void DoCollisionStay(Collision collisionInfo)
		{
			if (this.runFsm.HandleCollisionStay)
			{
				this.runFsm.OnCollisionStay(collisionInfo);
			}
		}

		// Token: 0x06006788 RID: 26504 RVA: 0x001E84BE File Offset: 0x001E66BE
		public override void DoCollisionExit(Collision collisionInfo)
		{
			if (this.runFsm.HandleCollisionExit)
			{
				this.runFsm.OnCollisionExit(collisionInfo);
			}
		}

		// Token: 0x06006789 RID: 26505 RVA: 0x001E84DC File Offset: 0x001E66DC
		public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
		{
			this.runFsm.OnControllerColliderHit(collisionInfo);
		}

		// Token: 0x0600678A RID: 26506 RVA: 0x001E84EC File Offset: 0x001E66EC
		public override void OnGUI()
		{
			if (this.runFsm != null && this.runFsm.HandleOnGUI)
			{
				this.runFsm.OnGUI();
			}
		}

		// Token: 0x0600678B RID: 26507 RVA: 0x001E851F File Offset: 0x001E671F
		public override void OnExit()
		{
			if (this.runFsm != null)
			{
				this.runFsm.Stop();
			}
		}

		// Token: 0x0600678C RID: 26508 RVA: 0x001E8537 File Offset: 0x001E6737
		private void CheckIfFinished()
		{
			if (this.runFsm == null || this.runFsm.Finished)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
			}
		}

		// Token: 0x04004F53 RID: 20307
		public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl();

		// Token: 0x04004F54 RID: 20308
		[UIHint(10)]
		public FsmInt storeID;

		// Token: 0x04004F55 RID: 20309
		[Tooltip("Event to send when the FSM has finished (usually because it ran a Finish FSM action).")]
		public FsmEvent finishEvent;

		// Token: 0x04004F56 RID: 20310
		private Fsm runFsm;
	}
}
