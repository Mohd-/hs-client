using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3A RID: 3130
	[Tooltip("Gets a Game Object's Transform and stores it in an Object Variable.")]
	[ActionCategory(4)]
	public class GetTransform : FsmStateAction
	{
		// Token: 0x0600664D RID: 26189 RVA: 0x001E4140 File Offset: 0x001E2340
		public override void Reset()
		{
			this.gameObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.storeTransform = null;
			this.everyFrame = false;
		}

		// Token: 0x0600664E RID: 26190 RVA: 0x001E416F File Offset: 0x001E236F
		public override void OnEnter()
		{
			this.DoGetGameObjectName();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600664F RID: 26191 RVA: 0x001E4188 File Offset: 0x001E2388
		public override void OnUpdate()
		{
			this.DoGetGameObjectName();
		}

		// Token: 0x06006650 RID: 26192 RVA: 0x001E4190 File Offset: 0x001E2390
		private void DoGetGameObjectName()
		{
			GameObject value = this.gameObject.Value;
			this.storeTransform.Value = ((!(value != null)) ? null : value.transform);
		}

		// Token: 0x04004E0B RID: 19979
		[RequiredField]
		public FsmGameObject gameObject;

		// Token: 0x04004E0C RID: 19980
		[RequiredField]
		[ObjectType(typeof(Transform))]
		[UIHint(10)]
		public FsmObject storeTransform;

		// Token: 0x04004E0D RID: 19981
		public bool everyFrame;
	}
}
