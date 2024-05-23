using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0A RID: 3082
	[Tooltip("Get the value of a variable in another FSM and store it in a variable of the same name in this FSM.")]
	[ActionCategory(12)]
	public class GetFsmVariable : FsmStateAction
	{
		// Token: 0x06006585 RID: 25989 RVA: 0x001E2028 File Offset: 0x001E0228
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = new FsmVar();
		}

		// Token: 0x06006586 RID: 25990 RVA: 0x001E2057 File Offset: 0x001E0257
		public override void OnEnter()
		{
			this.InitFsmVar();
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006587 RID: 25991 RVA: 0x001E2076 File Offset: 0x001E0276
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x06006588 RID: 25992 RVA: 0x001E2080 File Offset: 0x001E0280
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
				this.sourceVariable = this.sourceFsm.FsmVariables.GetVariable(this.storeValue.variableName);
				this.targetVariable = base.Fsm.Variables.GetVariable(this.storeValue.variableName);
				this.storeValue.Type = FsmUtility.GetVariableType(this.targetVariable);
				if (!string.IsNullOrEmpty(this.storeValue.variableName) && this.sourceVariable == null)
				{
					this.LogWarning("Missing Variable: " + this.storeValue.variableName);
				}
				this.cachedGO = ownerDefaultTarget;
			}
		}

		// Token: 0x06006589 RID: 25993 RVA: 0x001E2170 File Offset: 0x001E0370
		private void DoGetFsmVariable()
		{
			if (this.storeValue.IsNone)
			{
				return;
			}
			this.InitFsmVar();
			this.storeValue.GetValueFrom(this.sourceVariable);
			this.storeValue.ApplyValueTo(this.targetVariable);
		}

		// Token: 0x04004D4C RID: 19788
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D4D RID: 19789
		[Tooltip("Optional name of FSM on Game Object")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004D4E RID: 19790
		[UIHint(10)]
		[HideTypeFilter]
		[RequiredField]
		public FsmVar storeValue;

		// Token: 0x04004D4F RID: 19791
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004D50 RID: 19792
		private GameObject cachedGO;

		// Token: 0x04004D51 RID: 19793
		private PlayMakerFSM sourceFsm;

		// Token: 0x04004D52 RID: 19794
		private INamedVariable sourceVariable;

		// Token: 0x04004D53 RID: 19795
		private NamedVariable targetVariable;
	}
}
