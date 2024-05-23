using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCE RID: 3278
	[Tooltip("Sets the Range of a Light.")]
	[ActionCategory(21)]
	public class SetLightRange : ComponentAction<Light>
	{
		// Token: 0x060068D8 RID: 26840 RVA: 0x001EC24D File Offset: 0x001EA44D
		public override void Reset()
		{
			this.gameObject = null;
			this.lightRange = 20f;
			this.everyFrame = false;
		}

		// Token: 0x060068D9 RID: 26841 RVA: 0x001EC26D File Offset: 0x001EA46D
		public override void OnEnter()
		{
			this.DoSetLightRange();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068DA RID: 26842 RVA: 0x001EC286 File Offset: 0x001EA486
		public override void OnUpdate()
		{
			this.DoSetLightRange();
		}

		// Token: 0x060068DB RID: 26843 RVA: 0x001EC290 File Offset: 0x001EA490
		private void DoSetLightRange()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.range = this.lightRange.Value;
			}
		}

		// Token: 0x04005094 RID: 20628
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005095 RID: 20629
		public FsmFloat lightRange;

		// Token: 0x04005096 RID: 20630
		public bool everyFrame;
	}
}
