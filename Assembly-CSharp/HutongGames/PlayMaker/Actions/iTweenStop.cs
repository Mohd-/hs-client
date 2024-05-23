using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D3E RID: 3390
	[Tooltip("Stop an iTween action.")]
	[ActionCategory("iTween")]
	public class iTweenStop : FsmStateAction
	{
		// Token: 0x06006AC7 RID: 27335 RVA: 0x001F7114 File Offset: 0x001F5314
		public override void Reset()
		{
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.iTweenType = iTweenFSMType.all;
			this.includeChildren = false;
			this.inScene = false;
		}

		// Token: 0x06006AC8 RID: 27336 RVA: 0x001F714A File Offset: 0x001F534A
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoiTween();
			base.Finish();
		}

		// Token: 0x06006AC9 RID: 27337 RVA: 0x001F7160 File Offset: 0x001F5360
		private void DoiTween()
		{
			if (this.id.IsNone)
			{
				if (this.iTweenType == iTweenFSMType.all)
				{
					iTween.Stop();
				}
				else if (this.inScene)
				{
					iTween.Stop(Enum.GetName(typeof(iTweenFSMType), this.iTweenType));
				}
				else
				{
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
					if (ownerDefaultTarget == null)
					{
						return;
					}
					iTween.Stop(ownerDefaultTarget, Enum.GetName(typeof(iTweenFSMType), this.iTweenType), this.includeChildren);
				}
			}
			else
			{
				iTween.StopByName(this.id.Value);
			}
		}

		// Token: 0x04005322 RID: 21282
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005323 RID: 21283
		public FsmString id;

		// Token: 0x04005324 RID: 21284
		public iTweenFSMType iTweenType;

		// Token: 0x04005325 RID: 21285
		public bool includeChildren;

		// Token: 0x04005326 RID: 21286
		public bool inScene;
	}
}
