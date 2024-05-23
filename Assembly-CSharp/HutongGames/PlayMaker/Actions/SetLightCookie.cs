using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCB RID: 3275
	[ActionCategory(21)]
	[Tooltip("Sets the Texture projected by a Light.")]
	public class SetLightCookie : ComponentAction<Light>
	{
		// Token: 0x060068CB RID: 26827 RVA: 0x001EC0F5 File Offset: 0x001EA2F5
		public override void Reset()
		{
			this.gameObject = null;
			this.lightCookie = null;
		}

		// Token: 0x060068CC RID: 26828 RVA: 0x001EC105 File Offset: 0x001EA305
		public override void OnEnter()
		{
			this.DoSetLightCookie();
			base.Finish();
		}

		// Token: 0x060068CD RID: 26829 RVA: 0x001EC114 File Offset: 0x001EA314
		private void DoSetLightCookie()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.cookie = this.lightCookie.Value;
			}
		}

		// Token: 0x0400508D RID: 20621
		[CheckForComponent(typeof(Light))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400508E RID: 20622
		public FsmTexture lightCookie;
	}
}
