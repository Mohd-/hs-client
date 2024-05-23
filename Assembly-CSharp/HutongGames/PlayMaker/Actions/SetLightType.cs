using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD0 RID: 3280
	[ActionCategory(21)]
	[Tooltip("Set Spot, Directional, or Point Light type.")]
	public class SetLightType : ComponentAction<Light>
	{
		// Token: 0x060068E2 RID: 26850 RVA: 0x001EC365 File Offset: 0x001EA565
		public override void Reset()
		{
			this.gameObject = null;
			this.lightType = 2;
		}

		// Token: 0x060068E3 RID: 26851 RVA: 0x001EC375 File Offset: 0x001EA575
		public override void OnEnter()
		{
			this.DoSetLightType();
			base.Finish();
		}

		// Token: 0x060068E4 RID: 26852 RVA: 0x001EC384 File Offset: 0x001EA584
		private void DoSetLightType()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.type = this.lightType;
			}
		}

		// Token: 0x0400509A RID: 20634
		[CheckForComponent(typeof(Light))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400509B RID: 20635
		public LightType lightType;
	}
}
