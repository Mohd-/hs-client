using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B36 RID: 2870
	[Tooltip("Activates/deactivates a Game Object. Use this to hide/show areas, or enable/disable many Behaviours at once.")]
	[ActionCategory(4)]
	public class ActivateGameObject : FsmStateAction
	{
		// Token: 0x06006210 RID: 25104 RVA: 0x001D2C60 File Offset: 0x001D0E60
		public override void Reset()
		{
			this.gameObject = null;
			this.activate = true;
			this.recursive = true;
			this.resetOnExit = false;
			this.everyFrame = false;
		}

		// Token: 0x06006211 RID: 25105 RVA: 0x001D2C8F File Offset: 0x001D0E8F
		public override void OnEnter()
		{
			this.DoActivateGameObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006212 RID: 25106 RVA: 0x001D2CA8 File Offset: 0x001D0EA8
		public override void OnUpdate()
		{
			this.DoActivateGameObject();
		}

		// Token: 0x06006213 RID: 25107 RVA: 0x001D2CB0 File Offset: 0x001D0EB0
		public override void OnExit()
		{
			if (this.activatedGameObject == null)
			{
				return;
			}
			if (this.resetOnExit)
			{
				if (this.recursive.Value)
				{
					this.SetActiveRecursively(this.activatedGameObject, !this.activate.Value);
				}
				else
				{
					this.activatedGameObject.SetActive(!this.activate.Value);
				}
			}
		}

		// Token: 0x06006214 RID: 25108 RVA: 0x001D2D24 File Offset: 0x001D0F24
		private void DoActivateGameObject()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.recursive.Value)
			{
				this.SetActiveRecursively(ownerDefaultTarget, this.activate.Value);
			}
			else
			{
				ownerDefaultTarget.SetActive(this.activate.Value);
			}
			this.activatedGameObject = ownerDefaultTarget;
		}

		// Token: 0x06006215 RID: 25109 RVA: 0x001D2D90 File Offset: 0x001D0F90
		public void SetActiveRecursively(GameObject go, bool state)
		{
			go.SetActive(state);
			foreach (object obj in go.transform)
			{
				Transform transform = (Transform)obj;
				this.SetActiveRecursively(transform.gameObject, state);
			}
		}

		// Token: 0x04004923 RID: 18723
		[RequiredField]
		[Tooltip("The GameObject to activate/deactivate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004924 RID: 18724
		[Tooltip("Check to activate, uncheck to deactivate Game Object.")]
		[RequiredField]
		public FsmBool activate;

		// Token: 0x04004925 RID: 18725
		[Tooltip("Recursively activate/deactivate all children.")]
		public FsmBool recursive;

		// Token: 0x04004926 RID: 18726
		[Tooltip("Reset the game objects when exiting this state. Useful if you want an object to be active only while this state is active.\nNote: Only applies to the last Game Object activated/deactivated (won't work if Game Object changes).")]
		public bool resetOnExit;

		// Token: 0x04004927 RID: 18727
		[Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
		public bool everyFrame;

		// Token: 0x04004928 RID: 18728
		private GameObject activatedGameObject;
	}
}
