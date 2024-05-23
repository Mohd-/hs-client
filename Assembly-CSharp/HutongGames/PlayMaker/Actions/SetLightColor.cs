using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CCA RID: 3274
	[Tooltip("Sets the Color of a Light.")]
	[ActionCategory(21)]
	public class SetLightColor : ComponentAction<Light>
	{
		// Token: 0x060068C6 RID: 26822 RVA: 0x001EC068 File Offset: 0x001EA268
		public override void Reset()
		{
			this.gameObject = null;
			this.lightColor = Color.white;
			this.everyFrame = false;
		}

		// Token: 0x060068C7 RID: 26823 RVA: 0x001EC088 File Offset: 0x001EA288
		public override void OnEnter()
		{
			this.DoSetLightColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068C8 RID: 26824 RVA: 0x001EC0A1 File Offset: 0x001EA2A1
		public override void OnUpdate()
		{
			this.DoSetLightColor();
		}

		// Token: 0x060068C9 RID: 26825 RVA: 0x001EC0AC File Offset: 0x001EA2AC
		private void DoSetLightColor()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.color = this.lightColor.Value;
			}
		}

		// Token: 0x0400508A RID: 20618
		[CheckForComponent(typeof(Light))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400508B RID: 20619
		[RequiredField]
		public FsmColor lightColor;

		// Token: 0x0400508C RID: 20620
		public bool everyFrame;
	}
}
