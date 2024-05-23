using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7F RID: 2943
	[Tooltip("Creates a Game Object at a spawn point.\nUse a Game Object and/or Position/Rotation for the Spawn Point. If you specify a Game Object, Position is used as a local offset, and Rotation will override the object's rotation.")]
	[ActionCategory(4)]
	public class CreateEmptyObject : FsmStateAction
	{
		// Token: 0x06006378 RID: 25464 RVA: 0x001DB04C File Offset: 0x001D924C
		public override void Reset()
		{
			this.gameObject = null;
			this.spawnPoint = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.rotation = new FsmVector3
			{
				UseVariable = true
			};
			this.storeObject = null;
		}

		// Token: 0x06006379 RID: 25465 RVA: 0x001DB098 File Offset: 0x001D9298
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			Vector3 vector = Vector3.zero;
			Vector3 eulerAngles = Vector3.up;
			if (this.spawnPoint.Value != null)
			{
				vector = this.spawnPoint.Value.transform.position;
				if (!this.position.IsNone)
				{
					vector += this.position.Value;
				}
				if (!this.rotation.IsNone)
				{
					eulerAngles = this.rotation.Value;
				}
				else
				{
					eulerAngles = this.spawnPoint.Value.transform.eulerAngles;
				}
			}
			else
			{
				if (!this.position.IsNone)
				{
					vector = this.position.Value;
				}
				if (!this.rotation.IsNone)
				{
					eulerAngles = this.rotation.Value;
				}
			}
			GameObject gameObject = this.storeObject.Value;
			if (value != null)
			{
				gameObject = Object.Instantiate<GameObject>(value);
				this.storeObject.Value = gameObject;
			}
			else
			{
				gameObject = new GameObject("EmptyObjectFromNull");
				this.storeObject.Value = gameObject;
			}
			if (gameObject != null)
			{
				gameObject.transform.position = vector;
				gameObject.transform.eulerAngles = eulerAngles;
			}
			base.Finish();
		}

		// Token: 0x04004AF8 RID: 19192
		public FsmGameObject gameObject;

		// Token: 0x04004AF9 RID: 19193
		public FsmGameObject spawnPoint;

		// Token: 0x04004AFA RID: 19194
		public FsmVector3 position;

		// Token: 0x04004AFB RID: 19195
		public FsmVector3 rotation;

		// Token: 0x04004AFC RID: 19196
		[UIHint(10)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;
	}
}
