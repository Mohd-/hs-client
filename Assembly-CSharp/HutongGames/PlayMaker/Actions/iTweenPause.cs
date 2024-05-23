using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D2B RID: 3371
	[ActionCategory("iTween")]
	[Tooltip("Pause an iTween action.")]
	public class iTweenPause : FsmStateAction
	{
		// Token: 0x06006A6D RID: 27245 RVA: 0x001F4170 File Offset: 0x001F2370
		public override void Reset()
		{
			this.iTweenType = iTweenFSMType.all;
			this.includeChildren = false;
			this.inScene = false;
		}

		// Token: 0x06006A6E RID: 27246 RVA: 0x001F4187 File Offset: 0x001F2387
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoiTween();
			base.Finish();
		}

		// Token: 0x06006A6F RID: 27247 RVA: 0x001F419C File Offset: 0x001F239C
		private void DoiTween()
		{
			if (this.iTweenType == iTweenFSMType.all)
			{
				iTween.Pause();
			}
			else if (this.inScene)
			{
				iTween.Pause(Enum.GetName(typeof(iTweenFSMType), this.iTweenType));
			}
			else
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				iTween.Pause(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), this.iTweenType), this.includeChildren);
			}
		}

		// Token: 0x04005292 RID: 21138
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005293 RID: 21139
		public iTweenFSMType iTweenType;

		// Token: 0x04005294 RID: 21140
		public bool includeChildren;

		// Token: 0x04005295 RID: 21141
		public bool inScene;
	}
}
