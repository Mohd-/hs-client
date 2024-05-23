using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C13 RID: 3091
	[Tooltip("Gets a Game Object's Layer and stores it in an Int Variable.")]
	[ActionCategory(4)]
	public class GetLayer : FsmStateAction
	{
		// Token: 0x060065AC RID: 26028 RVA: 0x001E2728 File Offset: 0x001E0928
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060065AD RID: 26029 RVA: 0x001E273F File Offset: 0x001E093F
		public override void OnEnter()
		{
			this.DoGetLayer();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065AE RID: 26030 RVA: 0x001E2758 File Offset: 0x001E0958
		public override void OnUpdate()
		{
			this.DoGetLayer();
		}

		// Token: 0x060065AF RID: 26031 RVA: 0x001E2760 File Offset: 0x001E0960
		private void DoGetLayer()
		{
			if (this.gameObject.Value == null)
			{
				return;
			}
			this.storeResult.Value = this.gameObject.Value.layer;
		}

		// Token: 0x04004D7A RID: 19834
		[RequiredField]
		public FsmGameObject gameObject;

		// Token: 0x04004D7B RID: 19835
		[RequiredField]
		[UIHint(10)]
		public FsmInt storeResult;

		// Token: 0x04004D7C RID: 19836
		public bool everyFrame;
	}
}
