using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1E RID: 3102
	[Tooltip("Gets the name of a Game Object and stores it in a String Variable.")]
	[ActionCategory(4)]
	public class GetName : FsmStateAction
	{
		// Token: 0x060065D7 RID: 26071 RVA: 0x001E2DE4 File Offset: 0x001E0FE4
		public override void Reset()
		{
			this.gameObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.storeName = null;
			this.everyFrame = false;
		}

		// Token: 0x060065D8 RID: 26072 RVA: 0x001E2E13 File Offset: 0x001E1013
		public override void OnEnter()
		{
			this.DoGetGameObjectName();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065D9 RID: 26073 RVA: 0x001E2E2C File Offset: 0x001E102C
		public override void OnUpdate()
		{
			this.DoGetGameObjectName();
		}

		// Token: 0x060065DA RID: 26074 RVA: 0x001E2E34 File Offset: 0x001E1034
		private void DoGetGameObjectName()
		{
			GameObject value = this.gameObject.Value;
			this.storeName.Value = ((!(value != null)) ? string.Empty : value.name);
		}

		// Token: 0x04004D9C RID: 19868
		[RequiredField]
		public FsmGameObject gameObject;

		// Token: 0x04004D9D RID: 19869
		[UIHint(10)]
		[RequiredField]
		public FsmString storeName;

		// Token: 0x04004D9E RID: 19870
		public bool everyFrame;
	}
}
