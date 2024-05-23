using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C42 RID: 3138
	[Tooltip("Checks if an Object has a Component. Optionally remove the Component on exiting the state.")]
	[ActionCategory(4)]
	public class HasComponent : FsmStateAction
	{
		// Token: 0x06006671 RID: 26225 RVA: 0x001E46AA File Offset: 0x001E28AA
		public override void Reset()
		{
			this.aComponent = null;
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.component = null;
			this.store = null;
			this.everyFrame = false;
		}

		// Token: 0x06006672 RID: 26226 RVA: 0x001E46E0 File Offset: 0x001E28E0
		public override void OnEnter()
		{
			this.DoHasComponent((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006673 RID: 26227 RVA: 0x001E4730 File Offset: 0x001E2930
		public override void OnUpdate()
		{
			this.DoHasComponent((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
		}

		// Token: 0x06006674 RID: 26228 RVA: 0x001E4770 File Offset: 0x001E2970
		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.aComponent != null)
			{
				Object.Destroy(this.aComponent);
			}
		}

		// Token: 0x06006675 RID: 26229 RVA: 0x001E47AC File Offset: 0x001E29AC
		private void DoHasComponent(GameObject go)
		{
			this.aComponent = go.GetComponent(this.component.Value);
			base.Fsm.Event((!(this.aComponent != null)) ? this.falseEvent : this.trueEvent);
		}

		// Token: 0x04004E26 RID: 20006
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E27 RID: 20007
		[RequiredField]
		[UIHint(11)]
		public FsmString component;

		// Token: 0x04004E28 RID: 20008
		public FsmBool removeOnExit;

		// Token: 0x04004E29 RID: 20009
		public FsmEvent trueEvent;

		// Token: 0x04004E2A RID: 20010
		public FsmEvent falseEvent;

		// Token: 0x04004E2B RID: 20011
		[UIHint(10)]
		public FsmBool store;

		// Token: 0x04004E2C RID: 20012
		public bool everyFrame;

		// Token: 0x04004E2D RID: 20013
		private Component aComponent;
	}
}
