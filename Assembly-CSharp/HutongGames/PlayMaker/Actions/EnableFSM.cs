using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9D RID: 2973
	[Tooltip("Enables/Disables an FSM component on a GameObject.")]
	[ActionCategory(12)]
	public class EnableFSM : FsmStateAction
	{
		// Token: 0x060063DF RID: 25567 RVA: 0x001DC37D File Offset: 0x001DA57D
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.enable = true;
			this.resetOnExit = true;
		}

		// Token: 0x060063E0 RID: 25568 RVA: 0x001DC3AE File Offset: 0x001DA5AE
		public override void OnEnter()
		{
			this.DoEnableFSM();
			base.Finish();
		}

		// Token: 0x060063E1 RID: 25569 RVA: 0x001DC3BC File Offset: 0x001DA5BC
		private void DoEnableFSM()
		{
			GameObject gameObject = (this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner;
			if (gameObject == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.fsmName.Value))
			{
				PlayMakerFSM[] components = gameObject.GetComponents<PlayMakerFSM>();
				foreach (PlayMakerFSM playMakerFSM in components)
				{
					if (playMakerFSM.FsmName == this.fsmName.Value)
					{
						this.fsmComponent = playMakerFSM;
						break;
					}
				}
			}
			else
			{
				this.fsmComponent = gameObject.GetComponent<PlayMakerFSM>();
			}
			if (this.fsmComponent == null)
			{
				this.LogError("Missing FsmComponent!");
				return;
			}
			this.fsmComponent.enabled = this.enable.Value;
		}

		// Token: 0x060063E2 RID: 25570 RVA: 0x001DC4A8 File Offset: 0x001DA6A8
		public override void OnExit()
		{
			if (this.fsmComponent == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.fsmComponent.enabled = !this.enable.Value;
			}
		}

		// Token: 0x04004B54 RID: 19284
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004B55 RID: 19285
		[Tooltip("Optional name of FSM on GameObject. Useful if you have more than one FSM on a GameObject.")]
		[UIHint(15)]
		public FsmString fsmName;

		// Token: 0x04004B56 RID: 19286
		[Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enable;

		// Token: 0x04004B57 RID: 19287
		[Tooltip("Reset the initial enabled state when exiting the state.")]
		public FsmBool resetOnExit;

		// Token: 0x04004B58 RID: 19288
		private PlayMakerFSM fsmComponent;
	}
}
