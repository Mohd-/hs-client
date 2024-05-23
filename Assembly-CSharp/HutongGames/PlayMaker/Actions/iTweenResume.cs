using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D30 RID: 3376
	[Tooltip("Resume an iTween action.")]
	[ActionCategory("iTween")]
	public class iTweenResume : FsmStateAction
	{
		// Token: 0x06006A80 RID: 27264 RVA: 0x001F49F1 File Offset: 0x001F2BF1
		public override void Reset()
		{
			this.iTweenType = iTweenFSMType.all;
			this.includeChildren = false;
			this.inScene = false;
		}

		// Token: 0x06006A81 RID: 27265 RVA: 0x001F4A08 File Offset: 0x001F2C08
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoiTween();
			base.Finish();
		}

		// Token: 0x06006A82 RID: 27266 RVA: 0x001F4A1C File Offset: 0x001F2C1C
		private void DoiTween()
		{
			if (this.iTweenType == iTweenFSMType.all)
			{
				iTween.Resume();
			}
			else if (this.inScene)
			{
				iTween.Resume(Enum.GetName(typeof(iTweenFSMType), this.iTweenType));
			}
			else
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				iTween.Resume(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), this.iTweenType), this.includeChildren);
			}
		}

		// Token: 0x040052B4 RID: 21172
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040052B5 RID: 21173
		public iTweenFSMType iTweenType;

		// Token: 0x040052B6 RID: 21174
		public bool includeChildren;

		// Token: 0x040052B7 RID: 21175
		public bool inScene;
	}
}
