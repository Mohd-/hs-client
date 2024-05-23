using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000892 RID: 2194
public class FatalErrorDialog : MonoBehaviour
{
	// Token: 0x17000634 RID: 1588
	// (get) Token: 0x0600539E RID: 21406 RVA: 0x0018F13E File Offset: 0x0018D33E
	private float DialogTop
	{
		get
		{
			return ((float)Screen.height - 347f) / 2f;
		}
	}

	// Token: 0x17000635 RID: 1589
	// (get) Token: 0x0600539F RID: 21407 RVA: 0x0018F152 File Offset: 0x0018D352
	private float DialogLeft
	{
		get
		{
			return ((float)Screen.width - 600f) / 2f;
		}
	}

	// Token: 0x17000636 RID: 1590
	// (get) Token: 0x060053A0 RID: 21408 RVA: 0x0018F168 File Offset: 0x0018D368
	private Rect DialogRect
	{
		get
		{
			return new Rect(this.DialogLeft, this.DialogTop, 600f, 347f);
		}
	}

	// Token: 0x17000637 RID: 1591
	// (get) Token: 0x060053A1 RID: 21409 RVA: 0x0018F190 File Offset: 0x0018D390
	private float ButtonTop
	{
		get
		{
			return this.DialogTop + 347f - 20f - 31f;
		}
	}

	// Token: 0x17000638 RID: 1592
	// (get) Token: 0x060053A2 RID: 21410 RVA: 0x0018F1AA File Offset: 0x0018D3AA
	private float ButtonLeft
	{
		get
		{
			return ((float)Screen.width - 100f) / 2f;
		}
	}

	// Token: 0x17000639 RID: 1593
	// (get) Token: 0x060053A3 RID: 21411 RVA: 0x0018F1C0 File Offset: 0x0018D3C0
	private Rect ButtonRect
	{
		get
		{
			return new Rect(this.ButtonLeft, this.ButtonTop, 100f, 31f);
		}
	}

	// Token: 0x060053A4 RID: 21412 RVA: 0x0018F1E8 File Offset: 0x0018D3E8
	private void Awake()
	{
		this.BuildText();
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
	}

	// Token: 0x060053A5 RID: 21413 RVA: 0x0018F208 File Offset: 0x0018D408
	private void OnGUI()
	{
		this.InitGUIStyles();
		GUI.Box(this.DialogRect, string.Empty, this.m_dialogStyle);
		GUI.Box(this.DialogRect, this.m_text, this.m_dialogStyle);
		if (GUI.Button(this.ButtonRect, GameStrings.Get("GLOBAL_EXIT")))
		{
			Log.Mike.Print("FatalErrorDialog.OnGUI() - calling FatalErrorMgr.Get().NotifyExitPressed()", new object[0]);
			FatalErrorMgr.Get().NotifyExitPressed();
			Log.Mike.Print("FatalErrorDialog.OnGUI() - called FatalErrorMgr.Get().NotifyExitPressed()", new object[0]);
		}
	}

	// Token: 0x060053A6 RID: 21414 RVA: 0x0018F298 File Offset: 0x0018D498
	private void InitGUIStyles()
	{
		if (this.m_dialogStyle != null)
		{
			return;
		}
		Log.Mike.Print("FatalErrorDialog.InitGUIStyles()", new object[0]);
		this.m_dialogStyle = new GUIStyle("box")
		{
			clipping = 0,
			stretchHeight = true,
			stretchWidth = true,
			wordWrap = true,
			fontSize = 16
		};
	}

	// Token: 0x060053A7 RID: 21415 RVA: 0x0018F301 File Offset: 0x0018D501
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		this.BuildText();
	}

	// Token: 0x060053A8 RID: 21416 RVA: 0x0018F320 File Offset: 0x0018D520
	private void BuildText()
	{
		List<FatalErrorMessage> messages = FatalErrorMgr.Get().GetMessages();
		if (messages.Count == 0)
		{
			this.m_text = string.Empty;
			return;
		}
		List<string> list = new List<string>();
		for (int i = 0; i < messages.Count; i++)
		{
			FatalErrorMessage fatalErrorMessage = messages[i];
			string text = fatalErrorMessage.m_text;
			if (!list.Contains(text))
			{
				list.Add(text);
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int j = 0; j < list.Count; j++)
		{
			string text2 = list[j];
			stringBuilder.Append(text2);
			stringBuilder.Append("\n");
		}
		stringBuilder.Remove(stringBuilder.Length - 1, 1);
		this.m_text = stringBuilder.ToString();
	}

	// Token: 0x040039D7 RID: 14807
	private const float DialogWidth = 600f;

	// Token: 0x040039D8 RID: 14808
	private const float DialogHeight = 347f;

	// Token: 0x040039D9 RID: 14809
	private const float DialogPadding = 20f;

	// Token: 0x040039DA RID: 14810
	private const float ButtonWidth = 100f;

	// Token: 0x040039DB RID: 14811
	private const float ButtonHeight = 31f;

	// Token: 0x040039DC RID: 14812
	private GUIStyle m_dialogStyle;

	// Token: 0x040039DD RID: 14813
	private string m_text;
}
