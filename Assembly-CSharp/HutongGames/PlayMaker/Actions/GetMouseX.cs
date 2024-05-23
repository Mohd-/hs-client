using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1C RID: 3100
	[Tooltip("Gets the X Position of the mouse and stores it in a Float Variable.")]
	[ActionCategory(6)]
	public class GetMouseX : FsmStateAction
	{
		// Token: 0x060065CD RID: 26061 RVA: 0x001E2D04 File Offset: 0x001E0F04
		public override void Reset()
		{
			this.storeResult = null;
			this.normalize = true;
		}

		// Token: 0x060065CE RID: 26062 RVA: 0x001E2D14 File Offset: 0x001E0F14
		public override void OnEnter()
		{
			this.DoGetMouseX();
		}

		// Token: 0x060065CF RID: 26063 RVA: 0x001E2D1C File Offset: 0x001E0F1C
		public override void OnUpdate()
		{
			this.DoGetMouseX();
		}

		// Token: 0x060065D0 RID: 26064 RVA: 0x001E2D24 File Offset: 0x001E0F24
		private void DoGetMouseX()
		{
			if (this.storeResult != null)
			{
				float num = Input.mousePosition.x;
				if (this.normalize)
				{
					num /= (float)Screen.width;
				}
				this.storeResult.Value = num;
			}
		}

		// Token: 0x04004D98 RID: 19864
		[RequiredField]
		[UIHint(10)]
		public FsmFloat storeResult;

		// Token: 0x04004D99 RID: 19865
		public bool normalize;
	}
}
