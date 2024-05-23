using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE8 RID: 3048
	[ActionCategory(27)]
	[Tooltip("Tests if a Game Object is visible.")]
	public class GameObjectIsVisible : ComponentAction<Renderer>
	{
		// Token: 0x060064F3 RID: 25843 RVA: 0x001DFF50 File Offset: 0x001DE150
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060064F4 RID: 25844 RVA: 0x001DFF75 File Offset: 0x001DE175
		public override void OnEnter()
		{
			this.DoIsVisible();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060064F5 RID: 25845 RVA: 0x001DFF8E File Offset: 0x001DE18E
		public override void OnUpdate()
		{
			this.DoIsVisible();
		}

		// Token: 0x060064F6 RID: 25846 RVA: 0x001DFF98 File Offset: 0x001DE198
		private void DoIsVisible()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				bool isVisible = base.renderer.isVisible;
				this.storeResult.Value = isVisible;
				base.Fsm.Event((!isVisible) ? this.falseEvent : this.trueEvent);
			}
		}

		// Token: 0x04004C89 RID: 19593
		[RequiredField]
		[Tooltip("The GameObject to test.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004C8A RID: 19594
		[Tooltip("Event to send if the GameObject is visible.")]
		public FsmEvent trueEvent;

		// Token: 0x04004C8B RID: 19595
		[Tooltip("Event to send if the GameObject is NOT visible.")]
		public FsmEvent falseEvent;

		// Token: 0x04004C8C RID: 19596
		[Tooltip("Store the result in a bool variable.")]
		[UIHint(10)]
		public FsmBool storeResult;

		// Token: 0x04004C8D RID: 19597
		public bool everyFrame;
	}
}
