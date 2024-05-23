using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CEE RID: 3310
	[Tooltip("Sets the visibility of a GameObject. Note: this action sets the GameObject Renderer's enabled state.")]
	[ActionCategory(20)]
	public class SetVisibility : ComponentAction<Renderer>
	{
		// Token: 0x06006968 RID: 26984 RVA: 0x001EE181 File Offset: 0x001EC381
		public override void Reset()
		{
			this.gameObject = null;
			this.toggle = false;
			this.visible = false;
			this.resetOnExit = true;
			this.initialVisibility = false;
		}

		// Token: 0x06006969 RID: 26985 RVA: 0x001EE1B0 File Offset: 0x001EC3B0
		public override void OnEnter()
		{
			this.DoSetVisibility(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x0600696A RID: 26986 RVA: 0x001EE1D0 File Offset: 0x001EC3D0
		private void DoSetVisibility(GameObject go)
		{
			if (!base.UpdateCache(go))
			{
				return;
			}
			this.initialVisibility = base.renderer.enabled;
			if (!this.toggle.Value)
			{
				base.renderer.enabled = this.visible.Value;
				return;
			}
			base.renderer.enabled = !base.renderer.enabled;
		}

		// Token: 0x0600696B RID: 26987 RVA: 0x001EE23B File Offset: 0x001EC43B
		public override void OnExit()
		{
			if (this.resetOnExit)
			{
				this.ResetVisibility();
			}
		}

		// Token: 0x0600696C RID: 26988 RVA: 0x001EE24E File Offset: 0x001EC44E
		private void ResetVisibility()
		{
			if (base.renderer != null)
			{
				base.renderer.enabled = this.initialVisibility;
			}
		}

		// Token: 0x04005118 RID: 20760
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005119 RID: 20761
		[Tooltip("Should the object visibility be toggled?\nHas priority over the 'visible' setting")]
		public FsmBool toggle;

		// Token: 0x0400511A RID: 20762
		[Tooltip("Should the object be set to visible or invisible?")]
		public FsmBool visible;

		// Token: 0x0400511B RID: 20763
		[Tooltip("Resets to the initial visibility when it leaves the state")]
		public bool resetOnExit;

		// Token: 0x0400511C RID: 20764
		private bool initialVisibility;
	}
}
