using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE0 RID: 3296
	[ActionCategory(39)]
	[Tooltip("Sets the individual fields of a Rect Variable. To leave any field unchanged, set variable to 'None'.")]
	public class SetRectFields : FsmStateAction
	{
		// Token: 0x06006924 RID: 26916 RVA: 0x001ED254 File Offset: 0x001EB454
		public override void Reset()
		{
			this.rectVariable = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.width = new FsmFloat
			{
				UseVariable = true
			};
			this.height = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06006925 RID: 26917 RVA: 0x001ED2BF File Offset: 0x001EB4BF
		public override void OnEnter()
		{
			this.DoSetRectFields();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006926 RID: 26918 RVA: 0x001ED2D8 File Offset: 0x001EB4D8
		public override void OnUpdate()
		{
			this.DoSetRectFields();
		}

		// Token: 0x06006927 RID: 26919 RVA: 0x001ED2E0 File Offset: 0x001EB4E0
		private void DoSetRectFields()
		{
			if (this.rectVariable.IsNone)
			{
				return;
			}
			Rect value = this.rectVariable.Value;
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			if (!this.width.IsNone)
			{
				value.width = this.width.Value;
			}
			if (!this.height.IsNone)
			{
				value.height = this.height.Value;
			}
			this.rectVariable.Value = value;
		}

		// Token: 0x040050D5 RID: 20693
		[UIHint(10)]
		[RequiredField]
		public FsmRect rectVariable;

		// Token: 0x040050D6 RID: 20694
		public FsmFloat x;

		// Token: 0x040050D7 RID: 20695
		public FsmFloat y;

		// Token: 0x040050D8 RID: 20696
		public FsmFloat width;

		// Token: 0x040050D9 RID: 20697
		public FsmFloat height;

		// Token: 0x040050DA RID: 20698
		public bool everyFrame;
	}
}
