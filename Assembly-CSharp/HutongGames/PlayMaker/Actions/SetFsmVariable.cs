using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB4 RID: 3252
	[Tooltip("Set the value of a variable in another FSM.")]
	[ActionCategory(12)]
	public class SetFsmVariable : FsmStateAction
	{
		// Token: 0x0600686C RID: 26732 RVA: 0x001EB430 File Offset: 0x001E9630
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = new FsmVar();
		}

		// Token: 0x0600686D RID: 26733 RVA: 0x001EB45F File Offset: 0x001E965F
		public override void OnEnter()
		{
			this.InitFsmVar();
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600686E RID: 26734 RVA: 0x001EB47E File Offset: 0x001E967E
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x0600686F RID: 26735 RVA: 0x001EB488 File Offset: 0x001E9688
		private void InitFsmVar()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.cachedGO)
			{
				this.sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
				this.sourceVariable = this.sourceFsm.FsmVariables.GetVariable(this.setValue.variableName);
				this.targetVariable = base.Fsm.Variables.GetVariable(this.setValue.variableName);
				this.setValue.Type = FsmUtility.GetVariableType(this.targetVariable);
				if (!string.IsNullOrEmpty(this.setValue.variableName) && this.sourceVariable == null)
				{
					this.LogWarning("Missing Variable: " + this.setValue.variableName);
				}
				this.cachedGO = ownerDefaultTarget;
			}
		}

		// Token: 0x06006870 RID: 26736 RVA: 0x001EB578 File Offset: 0x001E9778
		private void DoGetFsmVariable()
		{
			if (this.setValue.IsNone)
			{
				return;
			}
			this.InitFsmVar();
			this.setValue.GetValueFrom(this.sourceVariable);
			this.setValue.ApplyValueTo(this.targetVariable);
		}

		// Token: 0x04005045 RID: 20549
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005046 RID: 20550
		[UIHint(15)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04005047 RID: 20551
		public FsmString variableName;

		// Token: 0x04005048 RID: 20552
		[RequiredField]
		[HideTypeFilter]
		public FsmVar setValue;

		// Token: 0x04005049 RID: 20553
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400504A RID: 20554
		private GameObject cachedGO;

		// Token: 0x0400504B RID: 20555
		private PlayMakerFSM sourceFsm;

		// Token: 0x0400504C RID: 20556
		private INamedVariable sourceVariable;

		// Token: 0x0400504D RID: 20557
		private NamedVariable targetVariable;
	}
}
