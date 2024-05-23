using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B80 RID: 2944
	[ActionCategory(4)]
	[Tooltip("Creates a Game Object, usually from a Prefab.")]
	public class CreateObject : FsmStateAction
	{
		// Token: 0x0600637B RID: 25467 RVA: 0x001DB1F8 File Offset: 0x001D93F8
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
			this.networkInstantiate = false;
			this.networkGroup = 0;
		}

		// Token: 0x0600637C RID: 25468 RVA: 0x001DB25C File Offset: 0x001D945C
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.up;
				if (this.spawnPoint.Value != null)
				{
					vector = this.spawnPoint.Value.transform.position;
					if (!this.position.IsNone)
					{
						vector += this.position.Value;
					}
					vector2 = (this.rotation.IsNone ? this.spawnPoint.Value.transform.eulerAngles : this.rotation.Value);
				}
				else
				{
					if (!this.position.IsNone)
					{
						vector = this.position.Value;
					}
					if (!this.rotation.IsNone)
					{
						vector2 = this.rotation.Value;
					}
				}
				GameObject value2 = (GameObject)Object.Instantiate(value, vector, Quaternion.Euler(vector2));
				this.storeObject.Value = value2;
			}
			base.Finish();
		}

		// Token: 0x04004AFD RID: 19197
		[Tooltip("GameObject to create. Usually a Prefab.")]
		[RequiredField]
		public FsmGameObject gameObject;

		// Token: 0x04004AFE RID: 19198
		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		// Token: 0x04004AFF RID: 19199
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04004B00 RID: 19200
		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		// Token: 0x04004B01 RID: 19201
		[Tooltip("Optionally store the created object.")]
		[UIHint(10)]
		public FsmGameObject storeObject;

		// Token: 0x04004B02 RID: 19202
		[Tooltip("Use Network.Instantiate to create a Game Object on all clients in a networked game.")]
		public FsmBool networkInstantiate;

		// Token: 0x04004B03 RID: 19203
		[Tooltip("Usually 0. The group number allows you to group together network messages which allows you to filter them if so desired.")]
		public FsmInt networkGroup;
	}
}
