using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA7 RID: 2983
	[ActionCategory(7)]
	[Tooltip("Adds a value to a Float Variable.")]
	public class FloatAdd : FsmStateAction
	{
		// Token: 0x0600640A RID: 25610 RVA: 0x001DCC17 File Offset: 0x001DAE17
		public override void Reset()
		{
			this.floatVariable = null;
			this.add = null;
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x0600640B RID: 25611 RVA: 0x001DCC35 File Offset: 0x001DAE35
		public override void OnEnter()
		{
			this.DoFloatAdd();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600640C RID: 25612 RVA: 0x001DCC4E File Offset: 0x001DAE4E
		public override void OnUpdate()
		{
			this.DoFloatAdd();
		}

		// Token: 0x0600640D RID: 25613 RVA: 0x001DCC58 File Offset: 0x001DAE58
		private void DoFloatAdd()
		{
			if (!this.perSecond)
			{
				this.floatVariable.Value += this.add.Value;
			}
			else
			{
				this.floatVariable.Value += this.add.Value * Time.deltaTime;
			}
		}

		// Token: 0x04004B7B RID: 19323
		[RequiredField]
		[Tooltip("The Float variable to add to.")]
		[UIHint(10)]
		public FsmFloat floatVariable;

		// Token: 0x04004B7C RID: 19324
		[Tooltip("Amount to add.")]
		[RequiredField]
		public FsmFloat add;

		// Token: 0x04004B7D RID: 19325
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04004B7E RID: 19326
		[Tooltip("Used with Every Frame. Adds the value over one second to make the operation frame rate independent.")]
		public bool perSecond;
	}
}
