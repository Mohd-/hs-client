using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B95 RID: 2965
	[ActionCategory(25)]
	[Tooltip("Makes the Game Object not be destroyed automatically when loading a new scene.")]
	public class DontDestroyOnLoad : FsmStateAction
	{
		// Token: 0x060063C2 RID: 25538 RVA: 0x001DBC21 File Offset: 0x001D9E21
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060063C3 RID: 25539 RVA: 0x001DBC2C File Offset: 0x001D9E2C
		public override void OnEnter()
		{
			Object.DontDestroyOnLoad(base.Owner.transform.root.gameObject);
			base.Finish();
		}

		// Token: 0x04004B32 RID: 19250
		[RequiredField]
		[Tooltip("GameObject to mark as DontDestroyOnLoad.")]
		public FsmOwnerDefault gameObject;
	}
}
