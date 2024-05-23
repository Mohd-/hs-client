using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD9 RID: 3289
	[Tooltip("Sets a Game Object's Name.")]
	[ActionCategory(4)]
	public class SetName : FsmStateAction
	{
		// Token: 0x06006906 RID: 26886 RVA: 0x001ECC33 File Offset: 0x001EAE33
		public override void Reset()
		{
			this.gameObject = null;
			this.name = null;
		}

		// Token: 0x06006907 RID: 26887 RVA: 0x001ECC43 File Offset: 0x001EAE43
		public override void OnEnter()
		{
			this.DoSetLayer();
			base.Finish();
		}

		// Token: 0x06006908 RID: 26888 RVA: 0x001ECC54 File Offset: 0x001EAE54
		private void DoSetLayer()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			ownerDefaultTarget.name = this.name.Value;
		}

		// Token: 0x040050BB RID: 20667
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050BC RID: 20668
		[RequiredField]
		public FsmString name;
	}
}
