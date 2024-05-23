using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCF RID: 3279
	[Tooltip("Sets the Spot Angle of a Light.")]
	[ActionCategory(21)]
	public class SetLightSpotAngle : ComponentAction<Light>
	{
		// Token: 0x060068DD RID: 26845 RVA: 0x001EC2D9 File Offset: 0x001EA4D9
		public override void Reset()
		{
			this.gameObject = null;
			this.lightSpotAngle = 20f;
			this.everyFrame = false;
		}

		// Token: 0x060068DE RID: 26846 RVA: 0x001EC2F9 File Offset: 0x001EA4F9
		public override void OnEnter()
		{
			this.DoSetLightRange();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068DF RID: 26847 RVA: 0x001EC312 File Offset: 0x001EA512
		public override void OnUpdate()
		{
			this.DoSetLightRange();
		}

		// Token: 0x060068E0 RID: 26848 RVA: 0x001EC31C File Offset: 0x001EA51C
		private void DoSetLightRange()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.spotAngle = this.lightSpotAngle.Value;
			}
		}

		// Token: 0x04005097 RID: 20631
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005098 RID: 20632
		public FsmFloat lightSpotAngle;

		// Token: 0x04005099 RID: 20633
		public bool everyFrame;
	}
}
