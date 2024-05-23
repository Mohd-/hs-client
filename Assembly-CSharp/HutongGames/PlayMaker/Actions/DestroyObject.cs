using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8E RID: 2958
	[ActionCategory(4)]
	[Tooltip("Destroys a Game Object.")]
	public class DestroyObject : FsmStateAction
	{
		// Token: 0x060063A9 RID: 25513 RVA: 0x001DB9D0 File Offset: 0x001D9BD0
		public override void Reset()
		{
			this.gameObject = null;
			this.delay = 0f;
		}

		// Token: 0x060063AA RID: 25514 RVA: 0x001DB9EC File Offset: 0x001D9BEC
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				if (this.delay.Value <= 0f)
				{
					Object.Destroy(value);
				}
				else
				{
					Object.Destroy(value, this.delay.Value);
				}
				if (this.detachChildren.Value)
				{
					value.transform.DetachChildren();
				}
			}
			base.Finish();
		}

		// Token: 0x060063AB RID: 25515 RVA: 0x001DBA63 File Offset: 0x001D9C63
		public override void OnUpdate()
		{
		}

		// Token: 0x04004B25 RID: 19237
		[RequiredField]
		[Tooltip("The GameObject to destroy.")]
		public FsmGameObject gameObject;

		// Token: 0x04004B26 RID: 19238
		[Tooltip("Optional delay before destroying the Game Object.")]
		[HasFloatSlider(0f, 5f)]
		public FsmFloat delay;

		// Token: 0x04004B27 RID: 19239
		[Tooltip("Detach children before destroying the Game Object.")]
		public FsmBool detachChildren;
	}
}
