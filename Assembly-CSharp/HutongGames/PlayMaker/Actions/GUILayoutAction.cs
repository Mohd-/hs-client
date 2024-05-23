using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC1 RID: 3009
	[Tooltip("GUILayout base action - don't use!")]
	public abstract class GUILayoutAction : FsmStateAction
	{
		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x06006471 RID: 25713 RVA: 0x001DE108 File Offset: 0x001DC308
		public GUILayoutOption[] LayoutOptions
		{
			get
			{
				if (this.options == null)
				{
					this.options = new GUILayoutOption[this.layoutOptions.Length];
					for (int i = 0; i < this.layoutOptions.Length; i++)
					{
						this.options[i] = this.layoutOptions[i].GetGUILayoutOption();
					}
				}
				return this.options;
			}
		}

		// Token: 0x06006472 RID: 25714 RVA: 0x001DE167 File Offset: 0x001DC367
		public override void Reset()
		{
			this.layoutOptions = new LayoutOption[0];
		}

		// Token: 0x04004BF6 RID: 19446
		public LayoutOption[] layoutOptions;

		// Token: 0x04004BF7 RID: 19447
		private GUILayoutOption[] options;
	}
}
