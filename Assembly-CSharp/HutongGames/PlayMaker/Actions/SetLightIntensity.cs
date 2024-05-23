using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCD RID: 3277
	[Tooltip("Sets the Intensity of a Light.")]
	[ActionCategory(21)]
	public class SetLightIntensity : ComponentAction<Light>
	{
		// Token: 0x060068D3 RID: 26835 RVA: 0x001EC1C0 File Offset: 0x001EA3C0
		public override void Reset()
		{
			this.gameObject = null;
			this.lightIntensity = 1f;
			this.everyFrame = false;
		}

		// Token: 0x060068D4 RID: 26836 RVA: 0x001EC1E0 File Offset: 0x001EA3E0
		public override void OnEnter()
		{
			this.DoSetLightIntensity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068D5 RID: 26837 RVA: 0x001EC1F9 File Offset: 0x001EA3F9
		public override void OnUpdate()
		{
			this.DoSetLightIntensity();
		}

		// Token: 0x060068D6 RID: 26838 RVA: 0x001EC204 File Offset: 0x001EA404
		private void DoSetLightIntensity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.intensity = this.lightIntensity.Value;
			}
		}

		// Token: 0x04005091 RID: 20625
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005092 RID: 20626
		public FsmFloat lightIntensity;

		// Token: 0x04005093 RID: 20627
		public bool everyFrame;
	}
}
