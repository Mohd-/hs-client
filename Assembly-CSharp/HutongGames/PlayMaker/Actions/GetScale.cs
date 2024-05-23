using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2B RID: 3115
	[Tooltip("Gets the Scale of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	[ActionCategory(14)]
	public class GetScale : FsmStateAction
	{
		// Token: 0x0600660E RID: 26126 RVA: 0x001E371F File Offset: 0x001E191F
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.xScale = null;
			this.yScale = null;
			this.zScale = null;
			this.space = 0;
			this.everyFrame = false;
		}

		// Token: 0x0600660F RID: 26127 RVA: 0x001E3752 File Offset: 0x001E1952
		public override void OnEnter()
		{
			this.DoGetScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006610 RID: 26128 RVA: 0x001E376B File Offset: 0x001E196B
		public override void OnUpdate()
		{
			this.DoGetScale();
		}

		// Token: 0x06006611 RID: 26129 RVA: 0x001E3774 File Offset: 0x001E1974
		private void DoGetScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 value = (this.space != null) ? ownerDefaultTarget.transform.localScale : ownerDefaultTarget.transform.lossyScale;
			this.vector.Value = value;
			this.xScale.Value = value.x;
			this.yScale.Value = value.y;
			this.zScale.Value = value.z;
		}

		// Token: 0x04004DCC RID: 19916
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004DCD RID: 19917
		[UIHint(10)]
		public FsmVector3 vector;

		// Token: 0x04004DCE RID: 19918
		[UIHint(10)]
		public FsmFloat xScale;

		// Token: 0x04004DCF RID: 19919
		[UIHint(10)]
		public FsmFloat yScale;

		// Token: 0x04004DD0 RID: 19920
		[UIHint(10)]
		public FsmFloat zScale;

		// Token: 0x04004DD1 RID: 19921
		public Space space;

		// Token: 0x04004DD2 RID: 19922
		public bool everyFrame;
	}
}
