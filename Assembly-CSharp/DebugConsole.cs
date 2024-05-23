using System;
using System.Collections.Generic;
using System.ComponentModel;

// Token: 0x020000BE RID: 190
public class DebugConsole
{
	// Token: 0x06000A70 RID: 2672 RVA: 0x0002E1D8 File Offset: 0x0002C3D8
	private static List<DebugConsole.CommandParamDecl> CreateParamDeclList(params DebugConsole.CommandParamDecl[] paramDecls)
	{
		List<DebugConsole.CommandParamDecl> list = new List<DebugConsole.CommandParamDecl>();
		foreach (DebugConsole.CommandParamDecl commandParamDecl in paramDecls)
		{
			list.Add(commandParamDecl);
		}
		return list;
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x0002E20D File Offset: 0x0002C40D
	private void InitConsoleCallbackMaps()
	{
		this.InitClientConsoleCallbackMap();
		this.InitServerConsoleCallbackMap();
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0002E21C File Offset: 0x0002C41C
	private void InitServerConsoleCallbackMap()
	{
		if (DebugConsole.s_serverConsoleCallbackMap != null)
		{
			return;
		}
		DebugConsole.s_serverConsoleCallbackMap = new Map<string, DebugConsole.ConsoleCallbackInfo>();
		DebugConsole.s_serverConsoleCallbackMap.Add("spawncard", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.STR, "cardGUID"),
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID"),
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.STR, "zoneName"),
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "premium")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("drawcard", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("shuffle", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("cyclehand", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("nuke", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("damage", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID"),
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "damage")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("addmana", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("readymana", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("maxmana", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("nocosts", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[0])));
		DebugConsole.s_serverConsoleCallbackMap.Add("healhero", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "playerID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("healentity", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("ready", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("exhaust", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("freeze", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID")
		})));
		DebugConsole.s_serverConsoleCallbackMap.Add("move", new DebugConsole.ConsoleCallbackInfo(true, null, DebugConsole.CreateParamDeclList(new DebugConsole.CommandParamDecl[]
		{
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "entityID"),
			new DebugConsole.CommandParamDecl(DebugConsole.CommandParamDecl.ParamType.I32, "zoneID")
		})));
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0002E566 File Offset: 0x0002C766
	private void InitClientConsoleCallbackMap()
	{
		if (DebugConsole.s_clientConsoleCallbackMap != null)
		{
			return;
		}
		DebugConsole.s_clientConsoleCallbackMap = new Map<string, DebugConsole.ConsoleCallbackInfo>();
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x0002E57D File Offset: 0x0002C77D
	private void SendDebugConsoleResponse(DebugConsole.DebugConsoleResponseType type, string message)
	{
		ConnectAPI.SendDebugConsoleResponse((int)type, message);
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x0002E588 File Offset: 0x0002C788
	private void SendConsoleCmdToServer(string commandName, List<string> commandParams)
	{
		if (!DebugConsole.s_serverConsoleCallbackMap.ContainsKey(commandName))
		{
			return;
		}
		string text = commandName;
		foreach (string text2 in commandParams)
		{
			text = text + " " + text2;
		}
		if (Network.SendConsoleCmdToServer(text))
		{
			return;
		}
		this.SendDebugConsoleResponse(DebugConsole.DebugConsoleResponseType.CONSOLE_OUTPUT, string.Format("Cannot send command '{0}'; not currently connected to a game server.", commandName));
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x0002E614 File Offset: 0x0002C814
	private void OnCommandReceived()
	{
		string debugConsoleCommand = ConnectAPI.GetDebugConsoleCommand();
		string[] array = debugConsoleCommand.Split(new char[]
		{
			' '
		});
		if (array.Length == 0)
		{
			Log.Rachelle.Print("Received empty command from debug console!", new object[0]);
			return;
		}
		string text = array[0];
		List<string> list = new List<string>();
		for (int i = 1; i < array.Length; i++)
		{
			list.Add(array[i]);
		}
		if (DebugConsole.s_serverConsoleCallbackMap.ContainsKey(text))
		{
			this.SendConsoleCmdToServer(text, list);
			return;
		}
		if (!DebugConsole.s_clientConsoleCallbackMap.ContainsKey(text))
		{
			this.SendDebugConsoleResponse(DebugConsole.DebugConsoleResponseType.CONSOLE_OUTPUT, string.Format("Unknown command '{0}'.", text));
			return;
		}
		DebugConsole.ConsoleCallbackInfo consoleCallbackInfo = DebugConsole.s_clientConsoleCallbackMap[text];
		if (consoleCallbackInfo.GetNumParams() != list.Count)
		{
			this.SendDebugConsoleResponse(DebugConsole.DebugConsoleResponseType.CONSOLE_OUTPUT, string.Format("Invalid params for command '{0}'.", text));
			return;
		}
		Log.Rachelle.Print(string.Format("Processing command '{0}' from debug console.", text), new object[0]);
		consoleCallbackInfo.Callback(list);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0002E71C File Offset: 0x0002C91C
	private void OnCommandResponseReceived()
	{
		Network.DebugConsoleResponse debugConsoleResponse = ConnectAPI.GetDebugConsoleResponse();
		if (debugConsoleResponse != null)
		{
			this.SendDebugConsoleResponse((DebugConsole.DebugConsoleResponseType)debugConsoleResponse.Type, debugConsoleResponse.Response);
		}
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0002E747 File Offset: 0x0002C947
	public static DebugConsole Get()
	{
		if (DebugConsole.s_instance == null)
		{
			DebugConsole.s_instance = new DebugConsole();
		}
		return DebugConsole.s_instance;
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0002E764 File Offset: 0x0002C964
	public void Init()
	{
		if (this.m_initialized)
		{
			return;
		}
		this.InitConsoleCallbackMaps();
		Network network = Network.Get();
		network.RegisterNetHandler(123, new Network.NetHandler(this.OnCommandReceived), null);
		network.RegisterNetHandler(124, new Network.NetHandler(this.OnCommandResponseReceived), null);
		this.m_initialized = true;
	}

	// Token: 0x040004F6 RID: 1270
	private static DebugConsole s_instance;

	// Token: 0x040004F7 RID: 1271
	private bool m_initialized;

	// Token: 0x040004F8 RID: 1272
	private static Map<string, DebugConsole.ConsoleCallbackInfo> s_serverConsoleCallbackMap;

	// Token: 0x040004F9 RID: 1273
	private static Map<string, DebugConsole.ConsoleCallbackInfo> s_clientConsoleCallbackMap;

	// Token: 0x02000A77 RID: 2679
	private class CommandParamDecl
	{
		// Token: 0x06005D6F RID: 23919 RVA: 0x001C07C8 File Offset: 0x001BE9C8
		public CommandParamDecl(DebugConsole.CommandParamDecl.ParamType type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		// Token: 0x04004535 RID: 17717
		public string Name;

		// Token: 0x04004536 RID: 17718
		public DebugConsole.CommandParamDecl.ParamType Type;

		// Token: 0x02000A78 RID: 2680
		public enum ParamType
		{
			// Token: 0x04004538 RID: 17720
			[Description("string")]
			STR,
			// Token: 0x04004539 RID: 17721
			[Description("int32")]
			I32,
			// Token: 0x0400453A RID: 17722
			[Description("float32")]
			F32,
			// Token: 0x0400453B RID: 17723
			[Description("bool")]
			BOOL
		}
	}

	// Token: 0x02000A79 RID: 2681
	private class ConsoleCallbackInfo
	{
		// Token: 0x06005D70 RID: 23920 RVA: 0x001C07E0 File Offset: 0x001BE9E0
		public ConsoleCallbackInfo(bool displayInCmdList, DebugConsole.ConsoleCallback callback, DebugConsole.CommandParamDecl[] commandParams)
		{
			this.DisplayInCommandList = displayInCmdList;
			this.ParamList = new List<DebugConsole.CommandParamDecl>(commandParams);
			this.Callback = callback;
		}

		// Token: 0x06005D71 RID: 23921 RVA: 0x001C080D File Offset: 0x001BEA0D
		public ConsoleCallbackInfo(bool displayInCmdList, DebugConsole.ConsoleCallback callback, List<DebugConsole.CommandParamDecl> commandParams) : this(displayInCmdList, callback, commandParams.ToArray())
		{
		}

		// Token: 0x06005D72 RID: 23922 RVA: 0x001C081D File Offset: 0x001BEA1D
		public int GetNumParams()
		{
			return this.ParamList.Count;
		}

		// Token: 0x0400453C RID: 17724
		public bool DisplayInCommandList;

		// Token: 0x0400453D RID: 17725
		public List<DebugConsole.CommandParamDecl> ParamList;

		// Token: 0x0400453E RID: 17726
		public DebugConsole.ConsoleCallback Callback;
	}

	// Token: 0x02000A7A RID: 2682
	// (Invoke) Token: 0x06005D74 RID: 23924
	private delegate void ConsoleCallback(List<string> commandParams);

	// Token: 0x02000A7B RID: 2683
	private enum DebugConsoleResponseType
	{
		// Token: 0x04004540 RID: 17728
		CONSOLE_OUTPUT,
		// Token: 0x04004541 RID: 17729
		LOG_MESSAGE
	}
}
