using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA2 RID: 2978
	[Tooltip("Finds the closest object to the specified Game Object.\nOptionally filter by Tag and Visibility.")]
	[ActionCategory(4)]
	public class FindClosest : FsmStateAction
	{
		// Token: 0x060063F6 RID: 25590 RVA: 0x001DC758 File Offset: 0x001DA958
		public override void Reset()
		{
			this.gameObject = null;
			this.withTag = "Untagged";
			this.ignoreOwner = true;
			this.mustBeVisible = false;
			this.storeObject = null;
			this.storeDistance = null;
			this.everyFrame = false;
		}

		// Token: 0x060063F7 RID: 25591 RVA: 0x001DC7A9 File Offset: 0x001DA9A9
		public override void OnEnter()
		{
			this.DoFindClosest();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060063F8 RID: 25592 RVA: 0x001DC7C2 File Offset: 0x001DA9C2
		public override void OnUpdate()
		{
			this.DoFindClosest();
		}

		// Token: 0x060063F9 RID: 25593 RVA: 0x001DC7CC File Offset: 0x001DA9CC
		private void DoFindClosest()
		{
			GameObject gameObject = (this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner;
			GameObject[] array;
			if (string.IsNullOrEmpty(this.withTag.Value) || this.withTag.Value == "Untagged")
			{
				array = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
			}
			else
			{
				array = GameObject.FindGameObjectsWithTag(this.withTag.Value);
			}
			GameObject value = null;
			float num = float.PositiveInfinity;
			foreach (GameObject gameObject2 in array)
			{
				if (!this.ignoreOwner.Value || !(gameObject2 == base.Owner))
				{
					if (!this.mustBeVisible.Value || ActionHelpers.IsVisible(gameObject2))
					{
						float sqrMagnitude = (gameObject.transform.position - gameObject2.transform.position).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							num = sqrMagnitude;
							value = gameObject2;
						}
					}
				}
			}
			this.storeObject.Value = value;
			if (!this.storeDistance.IsNone)
			{
				this.storeDistance.Value = Mathf.Sqrt(num);
			}
		}

		// Token: 0x04004B68 RID: 19304
		[RequiredField]
		[Tooltip("The GameObject to measure from.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004B69 RID: 19305
		[RequiredField]
		[Tooltip("Only consider objects with this Tag. NOTE: It's generally a lot quicker to find objects with a Tag!")]
		[UIHint(7)]
		public FsmString withTag;

		// Token: 0x04004B6A RID: 19306
		[Tooltip("If checked, ignores the object that owns this FSM.")]
		public FsmBool ignoreOwner;

		// Token: 0x04004B6B RID: 19307
		[Tooltip("Only consider objects visible to the camera.")]
		public FsmBool mustBeVisible;

		// Token: 0x04004B6C RID: 19308
		[UIHint(10)]
		[Tooltip("Store the closest object.")]
		public FsmGameObject storeObject;

		// Token: 0x04004B6D RID: 19309
		[UIHint(10)]
		[Tooltip("Store the distance to the closest object.")]
		public FsmFloat storeDistance;

		// Token: 0x04004B6E RID: 19310
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
