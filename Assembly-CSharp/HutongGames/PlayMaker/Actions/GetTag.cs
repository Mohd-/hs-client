using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C34 RID: 3124
	[Tooltip("Gets a Game Object's Tag and stores it in a String Variable.")]
	[ActionCategory(4)]
	public class GetTag : FsmStateAction
	{
		// Token: 0x06006636 RID: 26166 RVA: 0x001E3C18 File Offset: 0x001E1E18
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006637 RID: 26167 RVA: 0x001E3C2F File Offset: 0x001E1E2F
		public override void OnEnter()
		{
			this.DoGetTag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006638 RID: 26168 RVA: 0x001E3C48 File Offset: 0x001E1E48
		public override void OnUpdate()
		{
			this.DoGetTag();
		}

		// Token: 0x06006639 RID: 26169 RVA: 0x001E3C50 File Offset: 0x001E1E50
		private void DoGetTag()
		{
			if (this.gameObject.Value == null)
			{
				return;
			}
			this.storeResult.Value = this.gameObject.Value.tag;
		}

		// Token: 0x04004DEB RID: 19947
		[RequiredField]
		public FsmGameObject gameObject;

		// Token: 0x04004DEC RID: 19948
		[RequiredField]
		[UIHint(10)]
		public FsmString storeResult;

		// Token: 0x04004DED RID: 19949
		public bool everyFrame;
	}
}
