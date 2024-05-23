using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC9 RID: 3273
	[Tooltip("Sets a Game Object's Layer.")]
	[ActionCategory(4)]
	public class SetLayer : FsmStateAction
	{
		// Token: 0x060068C2 RID: 26818 RVA: 0x001EC00A File Offset: 0x001EA20A
		public override void Reset()
		{
			this.gameObject = null;
			this.layer = 0;
		}

		// Token: 0x060068C3 RID: 26819 RVA: 0x001EC01A File Offset: 0x001EA21A
		public override void OnEnter()
		{
			this.DoSetLayer();
			base.Finish();
		}

		// Token: 0x060068C4 RID: 26820 RVA: 0x001EC028 File Offset: 0x001EA228
		private void DoSetLayer()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			ownerDefaultTarget.layer = this.layer;
		}

		// Token: 0x04005088 RID: 20616
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005089 RID: 20617
		[UIHint(8)]
		public int layer;
	}
}
