using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE4 RID: 3300
	[ActionCategory(21)]
	[Tooltip("Sets the strength of the shadows cast by a Light.")]
	public class SetShadowStrength : ComponentAction<Light>
	{
		// Token: 0x06006939 RID: 26937 RVA: 0x001ED7BE File Offset: 0x001EB9BE
		public override void Reset()
		{
			this.gameObject = null;
			this.shadowStrength = 0.8f;
			this.everyFrame = false;
		}

		// Token: 0x0600693A RID: 26938 RVA: 0x001ED7DE File Offset: 0x001EB9DE
		public override void OnEnter()
		{
			this.DoSetShadowStrength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600693B RID: 26939 RVA: 0x001ED7F7 File Offset: 0x001EB9F7
		public override void OnUpdate()
		{
			this.DoSetShadowStrength();
		}

		// Token: 0x0600693C RID: 26940 RVA: 0x001ED800 File Offset: 0x001EBA00
		private void DoSetShadowStrength()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.shadowStrength = this.shadowStrength.Value;
			}
		}

		// Token: 0x040050EE RID: 20718
		[CheckForComponent(typeof(Light))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050EF RID: 20719
		public FsmFloat shadowStrength;

		// Token: 0x040050F0 RID: 20720
		public bool everyFrame;
	}
}
