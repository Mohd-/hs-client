using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDC RID: 3292
	[Tooltip("Sets the Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	[ActionCategory(14)]
	public class SetPosition : FsmStateAction
	{
		// Token: 0x06006911 RID: 26897 RVA: 0x001ECDC8 File Offset: 0x001EAFC8
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.space = 1;
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		// Token: 0x06006912 RID: 26898 RVA: 0x001ECE34 File Offset: 0x001EB034
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate)
			{
				this.DoSetPosition();
				base.Finish();
			}
		}

		// Token: 0x06006913 RID: 26899 RVA: 0x001ECE63 File Offset: 0x001EB063
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSetPosition();
			}
		}

		// Token: 0x06006914 RID: 26900 RVA: 0x001ECE78 File Offset: 0x001EB078
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSetPosition();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006915 RID: 26901 RVA: 0x001ECEA8 File Offset: 0x001EB0A8
		private void DoSetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector;
			if (this.vector.IsNone)
			{
				vector = ((this.space != null) ? ownerDefaultTarget.transform.localPosition : ownerDefaultTarget.transform.position);
			}
			else
			{
				vector = this.vector.Value;
			}
			if (!this.x.IsNone)
			{
				vector.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				vector.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				vector.z = this.z.Value;
			}
			if (this.space == null)
			{
				ownerDefaultTarget.transform.position = vector;
			}
			else
			{
				ownerDefaultTarget.transform.localPosition = vector;
			}
		}

		// Token: 0x040050C4 RID: 20676
		[Tooltip("The GameObject to position.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050C5 RID: 20677
		[UIHint(10)]
		[Tooltip("Use a stored Vector3 position, and/or set individual axis below.")]
		public FsmVector3 vector;

		// Token: 0x040050C6 RID: 20678
		public FsmFloat x;

		// Token: 0x040050C7 RID: 20679
		public FsmFloat y;

		// Token: 0x040050C8 RID: 20680
		public FsmFloat z;

		// Token: 0x040050C9 RID: 20681
		[Tooltip("Use local or world space.")]
		public Space space;

		// Token: 0x040050CA RID: 20682
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040050CB RID: 20683
		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;
	}
}
