using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x02000206 RID: 518
public class CheatMgr : MonoBehaviour
{
	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06001EF7 RID: 7927 RVA: 0x0009291F File Offset: 0x00090B1F
	public Map<string, string> cheatDesc
	{
		get
		{
			return this.m_cheatDesc;
		}
	}

	// Token: 0x1700032E RID: 814
	// (get) Token: 0x06001EF8 RID: 7928 RVA: 0x00092927 File Offset: 0x00090B27
	public Map<string, string> cheatArgs
	{
		get
		{
			return this.m_cheatArgs;
		}
	}

	// Token: 0x1700032F RID: 815
	// (get) Token: 0x06001EF9 RID: 7929 RVA: 0x0009292F File Offset: 0x00090B2F
	public Map<string, string> cheatExamples
	{
		get
		{
			return this.m_cheatExamples;
		}
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x00092937 File Offset: 0x00090B37
	public static CheatMgr Get()
	{
		return CheatMgr.s_instance;
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x0009293E File Offset: 0x00090B3E
	public void Awake()
	{
		CheatMgr.s_instance = this;
		this.m_cheatHistory = new List<string>();
		Cheats.Initialize();
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x00092956 File Offset: 0x00090B56
	public Map<string, List<CheatMgr.ProcessCheatCallback>>.KeyCollection GetCheatCommands()
	{
		return this.m_funcMap.Keys;
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x00092964 File Offset: 0x00090B64
	public bool HandleKeyboardInput()
	{
		if (ApplicationMgr.IsPublic())
		{
			return false;
		}
		if (!Input.GetKeyUp(96))
		{
			return false;
		}
		Rect rect;
		rect..ctor(0f, 0f, 1f, 0.05f);
		this.m_cheatInputBackground = rect;
		this.m_cheatInputBackground.x = this.m_cheatInputBackground.x * ((float)Screen.width * 0.95f);
		this.m_cheatInputBackground.y = this.m_cheatInputBackground.y * (float)Screen.height;
		this.m_cheatInputBackground.width = this.m_cheatInputBackground.width * (float)Screen.width;
		this.m_cheatInputBackground.height = this.m_cheatInputBackground.height * ((float)Screen.height * 1.03f);
		this.m_inputActive = true;
		this.m_cheatHistoryIndex = -1;
		this.ReadCheatHistoryOption();
		this.m_cheatTextBeforeScrollingThruHistory = null;
		UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams
		{
			m_owner = base.gameObject,
			m_preprocessCallback = new UniversalInputManager.TextInputPreprocessCallback(this.OnInputPreprocess),
			m_rect = rect,
			m_color = new Color?(Color.white),
			m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(this.OnInputComplete)
		};
		UniversalInputManager.Get().UseTextInput(parms, false);
		return true;
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x00092A90 File Offset: 0x00090C90
	private void ReadCheatHistoryOption()
	{
		string @string = Options.Get().GetString(Option.CHEAT_HISTORY);
		this.m_cheatHistory = new List<string>(@string.Split(new char[]
		{
			';'
		}));
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x00092AC6 File Offset: 0x00090CC6
	private void WriteCheatHistoryOption()
	{
		Options.Get().SetString(Option.CHEAT_HISTORY, string.Join(";", this.m_cheatHistory.ToArray()));
	}

	// Token: 0x06001F00 RID: 7936 RVA: 0x00092AEC File Offset: 0x00090CEC
	private bool OnInputPreprocess(Event e)
	{
		if (e.type != 4)
		{
			return false;
		}
		KeyCode keyCode = e.keyCode;
		if (keyCode == 96)
		{
			string inputText = UniversalInputManager.Get().GetInputText();
			if (string.IsNullOrEmpty(inputText))
			{
				UniversalInputManager.Get().CancelTextInput(base.gameObject, false);
				return true;
			}
		}
		if (this.m_cheatHistory.Count < 1)
		{
			return false;
		}
		if (keyCode == 273)
		{
			if (this.m_cheatHistoryIndex >= this.m_cheatHistory.Count - 1)
			{
				return true;
			}
			string inputText2 = UniversalInputManager.Get().GetInputText();
			if (this.m_cheatTextBeforeScrollingThruHistory == null)
			{
				this.m_cheatTextBeforeScrollingThruHistory = inputText2;
			}
			string inputText3 = this.m_cheatHistory[++this.m_cheatHistoryIndex];
			UniversalInputManager.Get().SetInputText(inputText3);
			ApplicationMgr.Get().ScheduleCallback(0f, false, delegate(object u)
			{
				TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
				if (textEditor != null)
				{
					textEditor.MoveTextEnd();
				}
			}, null);
			return true;
		}
		else
		{
			if (keyCode == 274)
			{
				string inputText4;
				if (this.m_cheatHistoryIndex <= 0)
				{
					this.m_cheatHistoryIndex = -1;
					if (this.m_cheatTextBeforeScrollingThruHistory == null)
					{
						return false;
					}
					inputText4 = this.m_cheatTextBeforeScrollingThruHistory;
					this.m_cheatTextBeforeScrollingThruHistory = null;
				}
				else
				{
					inputText4 = this.m_cheatHistory[--this.m_cheatHistoryIndex];
				}
				UniversalInputManager.Get().SetInputText(inputText4);
				return true;
			}
			return false;
		}
	}

	// Token: 0x06001F01 RID: 7937 RVA: 0x00092C60 File Offset: 0x00090E60
	public void RegisterCheatHandler(string func, CheatMgr.ProcessCheatCallback callback, string desc = null, string argDesc = null, string exampleArgs = null)
	{
		this.RegisterCheatHandler_(func, callback);
		if (desc != null)
		{
			this.m_cheatDesc[func] = desc;
		}
		if (argDesc != null)
		{
			this.m_cheatArgs[func] = argDesc;
		}
		if (exampleArgs != null)
		{
			this.m_cheatExamples[func] = exampleArgs;
		}
	}

	// Token: 0x06001F02 RID: 7938 RVA: 0x00092CB4 File Offset: 0x00090EB4
	public void RegisterCheatAlias(string func, params string[] aliases)
	{
		List<CheatMgr.ProcessCheatCallback> list;
		if (!this.m_funcMap.TryGetValue(func, out list))
		{
			Debug.LogError(string.Format("CheatMgr.RegisterCheatAlias() - cannot register aliases for func {0} because it does not exist", func));
			return;
		}
		foreach (string key in aliases)
		{
			this.m_cheatAlias[key] = func;
		}
	}

	// Token: 0x06001F03 RID: 7939 RVA: 0x00092D0C File Offset: 0x00090F0C
	public void UnregisterCheatHandler(string func, CheatMgr.ProcessCheatCallback callback)
	{
		this.UnregisterCheatHandler_(func, callback);
	}

	// Token: 0x06001F04 RID: 7940 RVA: 0x00092D18 File Offset: 0x00090F18
	public void OnGUI()
	{
		if (this.m_inputActive)
		{
			if (!UniversalInputManager.Get().IsTextInputActive())
			{
				this.m_inputActive = false;
				return;
			}
			GUI.depth = 1000;
			GUI.backgroundColor = Color.black;
			GUI.Box(this.m_cheatInputBackground, GUIContent.none);
			GUI.Box(this.m_cheatInputBackground, GUIContent.none);
			GUI.Box(this.m_cheatInputBackground, GUIContent.none);
		}
	}

	// Token: 0x06001F05 RID: 7941 RVA: 0x00092D8C File Offset: 0x00090F8C
	private void RegisterCheatHandler_(string func, CheatMgr.ProcessCheatCallback callback)
	{
		if (string.IsNullOrEmpty(func.Trim()))
		{
			Debug.LogError("CheatMgr.RegisterCheatHandler() - FAILED to register a null, empty, or all-whitespace function name");
			return;
		}
		List<CheatMgr.ProcessCheatCallback> list;
		if (this.m_funcMap.TryGetValue(func, out list))
		{
			if (!list.Contains(callback))
			{
				list.Add(callback);
			}
		}
		else
		{
			list = new List<CheatMgr.ProcessCheatCallback>();
			this.m_funcMap.Add(func, list);
			list.Add(callback);
		}
	}

	// Token: 0x06001F06 RID: 7942 RVA: 0x00092DFC File Offset: 0x00090FFC
	private void UnregisterCheatHandler_(string func, CheatMgr.ProcessCheatCallback callback)
	{
		List<CheatMgr.ProcessCheatCallback> list;
		if (!this.m_funcMap.TryGetValue(func, out list))
		{
			return;
		}
		list.Remove(callback);
	}

	// Token: 0x06001F07 RID: 7943 RVA: 0x00092E28 File Offset: 0x00091028
	private void OnInputComplete(string inputCommand)
	{
		this.m_inputActive = false;
		inputCommand = inputCommand.TrimStart(new char[0]);
		if (string.IsNullOrEmpty(inputCommand))
		{
			return;
		}
		string text = this.ProcessCheat(inputCommand);
		if (!string.IsNullOrEmpty(text))
		{
			UIStatus.Get().AddError(text);
		}
	}

	// Token: 0x06001F08 RID: 7944 RVA: 0x00092E74 File Offset: 0x00091074
	public string ProcessCheat(string inputCommand)
	{
		string text = this.ExtractFunc(inputCommand);
		if (text == null)
		{
			return "\"" + inputCommand.Split(new char[]
			{
				' '
			})[0] + "\" cheat command not found!";
		}
		int length = text.Length;
		string text2;
		string[] array;
		if (length == inputCommand.Length)
		{
			text2 = string.Empty;
			array = new string[]
			{
				string.Empty
			};
		}
		else
		{
			text2 = inputCommand.Remove(0, length + 1);
			MatchCollection matchCollection = Regex.Matches(text2, "\\S+");
			if (matchCollection.Count == 0)
			{
				array = new string[]
				{
					string.Empty
				};
			}
			else
			{
				array = new string[matchCollection.Count];
				for (int i = 0; i < matchCollection.Count; i++)
				{
					array[i] = matchCollection[i].Value;
				}
			}
		}
		string originalFunc = this.GetOriginalFunc(text);
		List<CheatMgr.ProcessCheatCallback> list = this.m_funcMap[originalFunc];
		bool flag = false;
		for (int j = 0; j < list.Count; j++)
		{
			CheatMgr.ProcessCheatCallback processCheatCallback = list[j];
			flag = (processCheatCallback(text, array, text2) || flag);
		}
		if (flag && (this.m_cheatHistory.Count < 1 || !this.m_cheatHistory[0].Equals(inputCommand)))
		{
			this.m_cheatHistory.Remove(inputCommand);
			this.m_cheatHistory.Insert(0, inputCommand);
		}
		if (this.m_cheatHistory.Count > 25)
		{
			this.m_cheatHistory.RemoveRange(24, this.m_cheatHistory.Count - 25);
		}
		this.m_cheatHistoryIndex = 0;
		this.m_cheatTextBeforeScrollingThruHistory = null;
		this.WriteCheatHistoryOption();
		if (!flag)
		{
			return "\"" + text + "\" cheat command executed, but failed!";
		}
		return null;
	}

	// Token: 0x06001F09 RID: 7945 RVA: 0x00093050 File Offset: 0x00091250
	private string ExtractFunc(string inputCommand)
	{
		inputCommand = inputCommand.TrimStart(new char[]
		{
			'/'
		});
		inputCommand = inputCommand.Trim();
		int num = 0;
		List<string> list = new List<string>();
		foreach (string text in this.m_funcMap.Keys)
		{
			list.Add(text);
			if (text.Length > list[num].Length)
			{
				num = list.Count - 1;
			}
		}
		foreach (string text2 in this.m_cheatAlias.Keys)
		{
			list.Add(text2);
			if (text2.Length > list[num].Length)
			{
				num = list.Count - 1;
			}
		}
		int i;
		for (i = 0; i < inputCommand.Length; i++)
		{
			char c = inputCommand.get_Chars(i);
			int j = 0;
			while (j < list.Count)
			{
				string text3 = list[j];
				if (i == text3.Length)
				{
					if (char.IsWhiteSpace(c))
					{
						return text3;
					}
					list.RemoveAt(j);
					if (j <= num)
					{
						num = this.ComputeLongestFuncIndex(list);
					}
				}
				else if (text3.get_Chars(i) != c)
				{
					list.RemoveAt(j);
					if (j <= num)
					{
						num = this.ComputeLongestFuncIndex(list);
					}
				}
				else
				{
					j++;
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
		}
		if (list.Count > 1)
		{
			foreach (string text4 in list)
			{
				if (inputCommand == text4)
				{
					return text4;
				}
			}
			return null;
		}
		string text5 = list[0];
		if (i < text5.Length)
		{
			return null;
		}
		return text5;
	}

	// Token: 0x06001F0A RID: 7946 RVA: 0x000932A8 File Offset: 0x000914A8
	private int ComputeLongestFuncIndex(List<string> funcs)
	{
		int num = 0;
		for (int i = 1; i < funcs.Count; i++)
		{
			string text = funcs[i];
			if (text.Length > funcs[num].Length)
			{
				num = i;
			}
		}
		return num;
	}

	// Token: 0x06001F0B RID: 7947 RVA: 0x000932F0 File Offset: 0x000914F0
	private string GetOriginalFunc(string func)
	{
		string result;
		if (!this.m_cheatAlias.TryGetValue(func, out result))
		{
			result = func;
		}
		return result;
	}

	// Token: 0x04001127 RID: 4391
	private const int MAX_HISTORY_LINES = 25;

	// Token: 0x04001128 RID: 4392
	private static CheatMgr s_instance;

	// Token: 0x04001129 RID: 4393
	private Map<string, List<CheatMgr.ProcessCheatCallback>> m_funcMap = new Map<string, List<CheatMgr.ProcessCheatCallback>>();

	// Token: 0x0400112A RID: 4394
	private Map<string, string> m_cheatAlias = new Map<string, string>();

	// Token: 0x0400112B RID: 4395
	private Map<string, string> m_cheatDesc = new Map<string, string>();

	// Token: 0x0400112C RID: 4396
	private Map<string, string> m_cheatArgs = new Map<string, string>();

	// Token: 0x0400112D RID: 4397
	private Map<string, string> m_cheatExamples = new Map<string, string>();

	// Token: 0x0400112E RID: 4398
	private Rect m_cheatInputBackground;

	// Token: 0x0400112F RID: 4399
	private bool m_inputActive;

	// Token: 0x04001130 RID: 4400
	private List<string> m_cheatHistory;

	// Token: 0x04001131 RID: 4401
	private int m_cheatHistoryIndex = -1;

	// Token: 0x04001132 RID: 4402
	private string m_cheatTextBeforeScrollingThruHistory;

	// Token: 0x02000234 RID: 564
	// (Invoke) Token: 0x0600214B RID: 8523
	public delegate bool ProcessCheatCallback(string func, string[] args, string rawArgs);
}
