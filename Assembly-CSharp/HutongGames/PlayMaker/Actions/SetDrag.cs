using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA2 RID: 3234
	[ActionCategory(9)]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=4734.0")]
	[Tooltip("Sets the Drag of a Game Object's Rigid Body.")]
	public class SetDrag : ComponentAction<Rigidbody>
	{
		// Token: 0x06006817 RID: 26647 RVA: 0x001EA254 File Offset: 0x001E8454
		public override void Reset()
		{
			this.gameObject = null;
			this.drag = 1f;
		}

		// Token: 0x06006818 RID: 26648 RVA: 0x001EA26D File Offset: 0x001E846D
		public override void OnEnter()
		{
			this.DoSetDrag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006819 RID: 26649 RVA: 0x001EA286 File Offset: 0x001E8486
		public override void OnUpdate()
		{
			this.DoSetDrag();
		}

		// Token: 0x0600681A RID: 26650 RVA: 0x001EA290 File Offset: 0x001E8490
		private void DoSetDrag()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.drag = this.drag.Value;
			}
		}

		// Token: 0x04004FDE RID: 20446
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FDF RID: 20447
		[HasFloatSlider(0f, 10f)]
		[RequiredField]
		public FsmFloat drag;

		// Token: 0x04004FE0 RID: 20448
		[Tooltip("Repeat every frame. Typically this would be set to True.")]
		public bool everyFrame;
	}
}
