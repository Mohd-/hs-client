using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDE RID: 3038
	[Tooltip("GUILayout Toolbar. NOTE: Arrays must be the same length as NumButtons or empty.")]
	[ActionCategory(26)]
	public class GUILayoutToolbar : GUILayoutAction
	{
		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x060064C8 RID: 25800 RVA: 0x001DF5E0 File Offset: 0x001DD7E0
		public GUIContent[] Contents
		{
			get
			{
				if (this.contents == null)
				{
					this.contents = new GUIContent[this.numButtons.Value];
					for (int i = 0; i < this.numButtons.Value; i++)
					{
						this.contents[i] = new GUIContent();
					}
					for (int j = 0; j < this.imagesArray.Length; j++)
					{
						this.contents[j].image = this.imagesArray[j].Value;
					}
					for (int k = 0; k < this.textsArray.Length; k++)
					{
						this.contents[k].text = this.textsArray[k].Value;
					}
					for (int l = 0; l < this.tooltipsArray.Length; l++)
					{
						this.contents[l].tooltip = this.tooltipsArray[l].Value;
					}
				}
				return this.contents;
			}
		}

		// Token: 0x060064C9 RID: 25801 RVA: 0x001DF6D8 File Offset: 0x001DD8D8
		public override void Reset()
		{
			base.Reset();
			this.numButtons = 0;
			this.selectedButton = null;
			this.buttonEventsArray = new FsmEvent[0];
			this.imagesArray = new FsmTexture[0];
			this.tooltipsArray = new FsmString[0];
			this.style = "Button";
		}

		// Token: 0x060064CA RID: 25802 RVA: 0x001DF734 File Offset: 0x001DD934
		public override void OnEnter()
		{
			string text = this.ErrorCheck();
			if (!string.IsNullOrEmpty(text))
			{
				this.LogError(text);
				base.Finish();
			}
		}

		// Token: 0x060064CB RID: 25803 RVA: 0x001DF760 File Offset: 0x001DD960
		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.selectedButton.Value = GUILayout.Toolbar(this.selectedButton.Value, this.Contents, this.style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				if (this.selectedButton.Value < this.buttonEventsArray.Length)
				{
					base.Fsm.Event(this.buttonEventsArray[this.selectedButton.Value]);
					GUIUtility.ExitGUI();
				}
			}
			else
			{
				GUI.changed = changed;
			}
		}

		// Token: 0x060064CC RID: 25804 RVA: 0x001DF800 File Offset: 0x001DDA00
		public override string ErrorCheck()
		{
			string text = string.Empty;
			if (this.imagesArray.Length > 0 && this.imagesArray.Length != this.numButtons.Value)
			{
				text += "Images array doesn't match NumButtons.\n";
			}
			if (this.textsArray.Length > 0 && this.textsArray.Length != this.numButtons.Value)
			{
				text += "Texts array doesn't match NumButtons.\n";
			}
			if (this.tooltipsArray.Length > 0 && this.tooltipsArray.Length != this.numButtons.Value)
			{
				text += "Tooltips array doesn't match NumButtons.\n";
			}
			return text;
		}

		// Token: 0x04004C58 RID: 19544
		public FsmInt numButtons;

		// Token: 0x04004C59 RID: 19545
		[UIHint(10)]
		public FsmInt selectedButton;

		// Token: 0x04004C5A RID: 19546
		public FsmEvent[] buttonEventsArray;

		// Token: 0x04004C5B RID: 19547
		public FsmTexture[] imagesArray;

		// Token: 0x04004C5C RID: 19548
		public FsmString[] textsArray;

		// Token: 0x04004C5D RID: 19549
		public FsmString[] tooltipsArray;

		// Token: 0x04004C5E RID: 19550
		public FsmString style;

		// Token: 0x04004C5F RID: 19551
		private GUIContent[] contents;
	}
}
