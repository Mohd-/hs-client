using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1D RID: 3101
	[Tooltip("Gets the Y Position of the mouse and stores it in a Float Variable.")]
	[ActionCategory(6)]
	public class GetMouseY : FsmStateAction
	{
		// Token: 0x060065D2 RID: 26066 RVA: 0x001E2D72 File Offset: 0x001E0F72
		public override void Reset()
		{
			this.storeResult = null;
			this.normalize = true;
		}

		// Token: 0x060065D3 RID: 26067 RVA: 0x001E2D82 File Offset: 0x001E0F82
		public override void OnEnter()
		{
			this.DoGetMouseY();
		}

		// Token: 0x060065D4 RID: 26068 RVA: 0x001E2D8A File Offset: 0x001E0F8A
		public override void OnUpdate()
		{
			this.DoGetMouseY();
		}

		// Token: 0x060065D5 RID: 26069 RVA: 0x001E2D94 File Offset: 0x001E0F94
		private void DoGetMouseY()
		{
			if (this.storeResult != null)
			{
				float num = Input.mousePosition.y;
				if (this.normalize)
				{
					num /= (float)Screen.height;
				}
				this.storeResult.Value = num;
			}
		}

		// Token: 0x04004D9A RID: 19866
		[RequiredField]
		[UIHint(10)]
		public FsmFloat storeResult;

		// Token: 0x04004D9B RID: 19867
		public bool normalize;
	}
}
