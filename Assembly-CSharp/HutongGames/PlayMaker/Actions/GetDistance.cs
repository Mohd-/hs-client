using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFC RID: 3068
	[ActionCategory(4)]
	[Tooltip("Measures the Distance betweens 2 Game Objects and stores the result in a Float Variable.")]
	public class GetDistance : FsmStateAction
	{
		// Token: 0x06006541 RID: 25921 RVA: 0x001E1119 File Offset: 0x001DF319
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x06006542 RID: 25922 RVA: 0x001E1137 File Offset: 0x001DF337
		public override void OnEnter()
		{
			this.DoGetDistance();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006543 RID: 25923 RVA: 0x001E1150 File Offset: 0x001DF350
		public override void OnUpdate()
		{
			this.DoGetDistance();
		}

		// Token: 0x06006544 RID: 25924 RVA: 0x001E1158 File Offset: 0x001DF358
		private void DoGetDistance()
		{
			GameObject gameObject = (this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner;
			if (gameObject == null || this.target.Value == null || this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = Vector3.Distance(gameObject.transform.position, this.target.Value.transform.position);
		}

		// Token: 0x04004CE6 RID: 19686
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004CE7 RID: 19687
		[RequiredField]
		public FsmGameObject target;

		// Token: 0x04004CE8 RID: 19688
		[RequiredField]
		[UIHint(10)]
		public FsmFloat storeResult;

		// Token: 0x04004CE9 RID: 19689
		public bool everyFrame;
	}
}
