using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0B RID: 3083
	[Tooltip("Get the values of multiple variables in another FSM and store in variables of the same name in this FSM.")]
	[ActionCategory(12)]
	public class GetFsmVariables : FsmStateAction
	{
		// Token: 0x0600658B RID: 25995 RVA: 0x001E21BE File Offset: 0x001E03BE
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.getVariables = null;
		}

		// Token: 0x0600658C RID: 25996 RVA: 0x001E21E0 File Offset: 0x001E03E0
		private void InitFsmVars()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.cachedGO)
			{
				this.sourceVariables = new INamedVariable[this.getVariables.Length];
				this.targetVariables = new NamedVariable[this.getVariables.Length];
				for (int i = 0; i < this.getVariables.Length; i++)
				{
					string variableName = this.getVariables[i].variableName;
					this.sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
					this.sourceVariables[i] = this.sourceFsm.FsmVariables.GetVariable(variableName);
					this.targetVariables[i] = base.Fsm.Variables.GetVariable(variableName);
					this.getVariables[i].Type = FsmUtility.GetVariableType(this.targetVariables[i]);
					if (!string.IsNullOrEmpty(variableName) && this.sourceVariables[i] == null)
					{
						this.LogWarning("Missing Variable: " + variableName);
					}
					this.cachedGO = ownerDefaultTarget;
				}
			}
		}

		// Token: 0x0600658D RID: 25997 RVA: 0x001E22FD File Offset: 0x001E04FD
		public override void OnEnter()
		{
			this.InitFsmVars();
			this.DoGetFsmVariables();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600658E RID: 25998 RVA: 0x001E231C File Offset: 0x001E051C
		public override void OnUpdate()
		{
			this.DoGetFsmVariables();
		}

		// Token: 0x0600658F RID: 25999 RVA: 0x001E2324 File Offset: 0x001E0524
		private void DoGetFsmVariables()
		{
			this.InitFsmVars();
			for (int i = 0; i < this.getVariables.Length; i++)
			{
				this.getVariables[i].GetValueFrom(this.sourceVariables[i]);
				this.getVariables[i].ApplyValueTo(this.targetVariables[i]);
			}
		}

		// Token: 0x04004D54 RID: 19796
		[Tooltip("The GameObject that owns the FSM")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D55 RID: 19797
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004D56 RID: 19798
		[HideTypeFilter]
		[UIHint(10)]
		[RequiredField]
		public FsmVar[] getVariables;

		// Token: 0x04004D57 RID: 19799
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004D58 RID: 19800
		private GameObject cachedGO;

		// Token: 0x04004D59 RID: 19801
		private PlayMakerFSM sourceFsm;

		// Token: 0x04004D5A RID: 19802
		private INamedVariable[] sourceVariables;

		// Token: 0x04004D5B RID: 19803
		private NamedVariable[] targetVariables;
	}
}
