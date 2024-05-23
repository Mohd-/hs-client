using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCC RID: 3276
	[ActionCategory(21)]
	[Tooltip("Sets the Flare effect used by a Light.")]
	public class SetLightFlare : ComponentAction<Light>
	{
		// Token: 0x060068CF RID: 26831 RVA: 0x001EC15D File Offset: 0x001EA35D
		public override void Reset()
		{
			this.gameObject = null;
			this.lightFlare = null;
		}

		// Token: 0x060068D0 RID: 26832 RVA: 0x001EC16D File Offset: 0x001EA36D
		public override void OnEnter()
		{
			this.DoSetLightRange();
			base.Finish();
		}

		// Token: 0x060068D1 RID: 26833 RVA: 0x001EC17C File Offset: 0x001EA37C
		private void DoSetLightRange()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.flare = this.lightFlare;
			}
		}

		// Token: 0x0400508F RID: 20623
		[CheckForComponent(typeof(Light))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005090 RID: 20624
		public Flare lightFlare;
	}
}
