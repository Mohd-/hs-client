using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBB RID: 3259
	[Tooltip("Sets the sorting depth of subsequent GUI elements.")]
	[ActionCategory(5)]
	public class SetGUIDepth : FsmStateAction
	{
		// Token: 0x06006888 RID: 26760 RVA: 0x001EB979 File Offset: 0x001E9B79
		public override void Reset()
		{
			this.depth = 0;
		}

		// Token: 0x06006889 RID: 26761 RVA: 0x001EB987 File Offset: 0x001E9B87
		public override void Awake()
		{
			base.Fsm.HandleOnGUI = true;
		}

		// Token: 0x0600688A RID: 26762 RVA: 0x001EB995 File Offset: 0x001E9B95
		public override void OnGUI()
		{
			GUI.depth = this.depth.Value;
		}

		// Token: 0x04005064 RID: 20580
		[RequiredField]
		public FsmInt depth;
	}
}
