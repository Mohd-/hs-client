using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class Options
{
	// Token: 0x06000AC0 RID: 2752 RVA: 0x0002EF6F File Offset: 0x0002D16F
	public static Options Get()
	{
		if (Options.s_instance == null)
		{
			Options.s_instance = new Options();
			Options.s_instance.Initialize();
		}
		return Options.s_instance;
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x0002EF94 File Offset: 0x0002D194
	public bool IsClientOption(Option option)
	{
		return this.m_clientOptionMap.ContainsKey(option);
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x0002EFA2 File Offset: 0x0002D1A2
	public bool IsServerOption(Option option)
	{
		return this.m_serverOptionMap.ContainsKey(option);
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x0002EFB0 File Offset: 0x0002D1B0
	public Map<Option, string> GetClientOptions()
	{
		return this.m_clientOptionMap;
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x0002EFB8 File Offset: 0x0002D1B8
	public Map<Option, ServerOption> GetServerOptions()
	{
		return this.m_serverOptionMap;
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x0002EFC0 File Offset: 0x0002D1C0
	public Type GetOptionType(Option option)
	{
		Type result;
		if (OptionDataTables.s_typeMap.TryGetValue(option, out result))
		{
			return result;
		}
		if (this.m_serverOptionFlagMap.ContainsKey(option))
		{
			return typeof(bool);
		}
		Debug.LogError(string.Format("Options.GetOptionType() - {0} does not have an option type!", option));
		return null;
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x0002F013 File Offset: 0x0002D213
	public bool RegisterChangedListener(Option option, Options.ChangedCallback callback)
	{
		return this.RegisterChangedListener(option, callback, null);
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x0002F020 File Offset: 0x0002D220
	public bool RegisterChangedListener(Option option, Options.ChangedCallback callback, object userData)
	{
		Options.ChangedListener changedListener = new Options.ChangedListener();
		changedListener.SetCallback(callback);
		changedListener.SetUserData(userData);
		List<Options.ChangedListener> list;
		if (!this.m_changedListeners.TryGetValue(option, out list))
		{
			list = new List<Options.ChangedListener>();
			this.m_changedListeners.Add(option, list);
		}
		else if (list.Contains(changedListener))
		{
			return false;
		}
		list.Add(changedListener);
		return true;
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x0002F082 File Offset: 0x0002D282
	public bool UnregisterChangedListener(Option option, Options.ChangedCallback callback)
	{
		return this.UnregisterChangedListener(option, callback, null);
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x0002F090 File Offset: 0x0002D290
	public bool UnregisterChangedListener(Option option, Options.ChangedCallback callback, object userData)
	{
		Options.ChangedListener changedListener = new Options.ChangedListener();
		changedListener.SetCallback(callback);
		changedListener.SetUserData(userData);
		List<Options.ChangedListener> list;
		if (!this.m_changedListeners.TryGetValue(option, out list))
		{
			return false;
		}
		if (!list.Remove(changedListener))
		{
			return false;
		}
		if (list.Count == 0)
		{
			this.m_changedListeners.Remove(option);
		}
		return true;
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x0002F0ED File Offset: 0x0002D2ED
	public bool RegisterGlobalChangedListener(Options.ChangedCallback callback)
	{
		return this.RegisterGlobalChangedListener(callback, null);
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x0002F0F8 File Offset: 0x0002D2F8
	public bool RegisterGlobalChangedListener(Options.ChangedCallback callback, object userData)
	{
		Options.ChangedListener changedListener = new Options.ChangedListener();
		changedListener.SetCallback(callback);
		changedListener.SetUserData(userData);
		if (this.m_globalChangedListeners.Contains(changedListener))
		{
			return false;
		}
		this.m_globalChangedListeners.Add(changedListener);
		return true;
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x0002F139 File Offset: 0x0002D339
	public bool UnregisterGlobalChangedListener(Options.ChangedCallback callback)
	{
		return this.UnregisterGlobalChangedListener(callback, null);
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x0002F144 File Offset: 0x0002D344
	public bool UnregisterGlobalChangedListener(Options.ChangedCallback callback, object userData)
	{
		Options.ChangedListener changedListener = new Options.ChangedListener();
		changedListener.SetCallback(callback);
		changedListener.SetUserData(userData);
		return this.m_globalChangedListeners.Remove(changedListener);
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x0002F174 File Offset: 0x0002D374
	public bool HasOption(Option option)
	{
		string key;
		if (this.m_clientOptionMap.TryGetValue(option, out key))
		{
			return LocalOptions.Get().Has(key);
		}
		ServerOption type;
		if (this.m_serverOptionMap.TryGetValue(option, out type))
		{
			return NetCache.Get().ClientOptionExists(type);
		}
		ServerOptionFlag serverOptionFlag;
		return this.m_serverOptionFlagMap.TryGetValue(option, out serverOptionFlag) && this.HasServerOptionFlag(serverOptionFlag);
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x0002F1DC File Offset: 0x0002D3DC
	public void DeleteOption(Option option)
	{
		string optionName;
		if (this.m_clientOptionMap.TryGetValue(option, out optionName))
		{
			this.DeleteClientOption(option, optionName);
			return;
		}
		ServerOption serverOption;
		if (this.m_serverOptionMap.TryGetValue(option, out serverOption))
		{
			this.DeleteServerOption(option, serverOption);
			return;
		}
		ServerOptionFlag serverOptionFlag;
		if (this.m_serverOptionFlagMap.TryGetValue(option, out serverOptionFlag))
		{
			this.DeleteServerOptionFlag(option, serverOptionFlag);
			return;
		}
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x0002F240 File Offset: 0x0002D440
	public object GetOption(Option option)
	{
		object result;
		if (this.GetOptionImpl(option, out result))
		{
			return result;
		}
		object result2;
		if (OptionDataTables.s_defaultsMap.TryGetValue(option, out result2))
		{
			return result2;
		}
		return null;
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x0002F274 File Offset: 0x0002D474
	public object GetOption(Option option, object defaultVal)
	{
		object result;
		if (this.GetOptionImpl(option, out result))
		{
			return result;
		}
		return defaultVal;
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x0002F294 File Offset: 0x0002D494
	public bool GetBool(Option option)
	{
		bool result;
		if (this.GetBoolImpl(option, out result))
		{
			return result;
		}
		object obj;
		return OptionDataTables.s_defaultsMap.TryGetValue(option, out obj) && (bool)obj;
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0002F2CC File Offset: 0x0002D4CC
	public bool GetBool(Option option, bool defaultVal)
	{
		bool result;
		if (this.GetBoolImpl(option, out result))
		{
			return result;
		}
		return defaultVal;
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x0002F2EC File Offset: 0x0002D4EC
	public int GetInt(Option option)
	{
		int result;
		if (this.GetIntImpl(option, out result))
		{
			return result;
		}
		object obj;
		if (OptionDataTables.s_defaultsMap.TryGetValue(option, out obj))
		{
			return (int)obj;
		}
		return 0;
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0002F324 File Offset: 0x0002D524
	public int GetInt(Option option, int defaultVal)
	{
		int result;
		if (this.GetIntImpl(option, out result))
		{
			return result;
		}
		return defaultVal;
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0002F344 File Offset: 0x0002D544
	public long GetLong(Option option)
	{
		long result;
		if (this.GetLongImpl(option, out result))
		{
			return result;
		}
		object obj;
		if (OptionDataTables.s_defaultsMap.TryGetValue(option, out obj))
		{
			return (long)obj;
		}
		return 0L;
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0002F37C File Offset: 0x0002D57C
	public long GetLong(Option option, long defaultVal)
	{
		long result;
		if (this.GetLongImpl(option, out result))
		{
			return result;
		}
		return defaultVal;
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x0002F39C File Offset: 0x0002D59C
	public float GetFloat(Option option)
	{
		float result;
		if (this.GetFloatImpl(option, out result))
		{
			return result;
		}
		object obj;
		if (OptionDataTables.s_defaultsMap.TryGetValue(option, out obj))
		{
			return (float)obj;
		}
		return 0f;
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0002F3D8 File Offset: 0x0002D5D8
	public float GetFloat(Option option, float defaultVal)
	{
		float result;
		if (this.GetFloatImpl(option, out result))
		{
			return result;
		}
		return defaultVal;
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x0002F3F8 File Offset: 0x0002D5F8
	public ulong GetULong(Option option)
	{
		ulong result;
		if (this.GetULongImpl(option, out result))
		{
			return result;
		}
		object obj;
		if (OptionDataTables.s_defaultsMap.TryGetValue(option, out obj))
		{
			return (ulong)obj;
		}
		return 0UL;
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0002F430 File Offset: 0x0002D630
	public ulong GetULong(Option option, ulong defaultVal)
	{
		ulong result;
		if (this.GetULongImpl(option, out result))
		{
			return result;
		}
		return defaultVal;
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0002F450 File Offset: 0x0002D650
	public string GetString(Option option)
	{
		string result;
		if (this.GetStringImpl(option, out result))
		{
			return result;
		}
		object obj;
		if (OptionDataTables.s_defaultsMap.TryGetValue(option, out obj))
		{
			return (string)obj;
		}
		return string.Empty;
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0002F48C File Offset: 0x0002D68C
	public string GetString(Option option, string defaultVal)
	{
		string result;
		if (this.GetStringImpl(option, out result))
		{
			return result;
		}
		return defaultVal;
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x0002F4AC File Offset: 0x0002D6AC
	public T GetEnum<T>(Option option)
	{
		T result;
		if (this.GetEnumImpl<T>(option, out result))
		{
			return result;
		}
		object genericVal;
		if (OptionDataTables.s_defaultsMap.TryGetValue(option, out genericVal) && this.TranslateEnumVal<T>(option, genericVal, out result))
		{
			return result;
		}
		return default(T);
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x0002F4F8 File Offset: 0x0002D6F8
	public T GetEnum<T>(Option option, T defaultVal)
	{
		T result;
		if (this.GetEnumImpl<T>(option, out result))
		{
			return result;
		}
		return defaultVal;
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x0002F518 File Offset: 0x0002D718
	public void SetOption(Option option, object val)
	{
		Type optionType = this.GetOptionType(option);
		if (optionType == typeof(bool))
		{
			this.SetBool(option, (bool)val);
		}
		else if (optionType == typeof(int))
		{
			this.SetInt(option, (int)val);
		}
		else if (optionType == typeof(long))
		{
			this.SetLong(option, (long)val);
		}
		else if (optionType == typeof(float))
		{
			this.SetFloat(option, (float)val);
		}
		else if (optionType == typeof(string))
		{
			this.SetString(option, (string)val);
		}
		else if (optionType == typeof(ulong))
		{
			this.SetULong(option, (ulong)val);
		}
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x0002F5F4 File Offset: 0x0002D7F4
	public void SetBool(Option option, bool val)
	{
		string key;
		if (this.m_clientOptionMap.TryGetValue(option, out key))
		{
			bool flag = LocalOptions.Get().Has(key);
			bool @bool = LocalOptions.Get().GetBool(key);
			if (!flag || @bool != val)
			{
				LocalOptions.Get().Set(key, val);
				this.FireChangedEvent(option, @bool, flag);
			}
			return;
		}
		ServerOptionFlag flag2;
		if (this.m_serverOptionFlagMap.TryGetValue(option, out flag2))
		{
			ServerOption type;
			ulong num;
			ulong num2;
			this.GetServerOptionFlagInfo(flag2, out type, out num, out num2);
			ulong ulongOption = NetCache.Get().GetULongOption(type);
			bool flag3 = (ulongOption & num) != 0UL;
			bool flag4 = (ulongOption & num2) != 0UL;
			if (!flag4 || flag3 != val)
			{
				ulong num3 = (!val) ? (ulongOption & ~num) : (ulongOption | num);
				num3 |= num2;
				NetCache.Get().SetULongOption(type, num3);
				this.FireChangedEvent(option, flag3, flag4);
			}
			return;
		}
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x0002F6F0 File Offset: 0x0002D8F0
	public void SetInt(Option option, int val)
	{
		string key;
		if (this.m_clientOptionMap.TryGetValue(option, out key))
		{
			bool flag = LocalOptions.Get().Has(key);
			int @int = LocalOptions.Get().GetInt(key);
			if (!flag || @int != val)
			{
				LocalOptions.Get().Set(key, val);
				this.FireChangedEvent(option, @int, flag);
			}
			return;
		}
		ServerOption type;
		if (this.m_serverOptionMap.TryGetValue(option, out type))
		{
			int num;
			bool intOption = NetCache.Get().GetIntOption(type, out num);
			if (!intOption || num != val)
			{
				NetCache.Get().SetIntOption(type, val);
				this.FireChangedEvent(option, num, intOption);
			}
			return;
		}
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x0002F7A4 File Offset: 0x0002D9A4
	public void SetLong(Option option, long val)
	{
		string key;
		if (this.m_clientOptionMap.TryGetValue(option, out key))
		{
			bool flag = LocalOptions.Get().Has(key);
			long @long = LocalOptions.Get().GetLong(key);
			if (!flag || @long != val)
			{
				LocalOptions.Get().Set(key, val);
				this.FireChangedEvent(option, @long, flag);
			}
			return;
		}
		ServerOption type;
		if (this.m_serverOptionMap.TryGetValue(option, out type))
		{
			long num;
			bool longOption = NetCache.Get().GetLongOption(type, out num);
			if (!longOption || num != val)
			{
				NetCache.Get().SetLongOption(type, val);
				this.FireChangedEvent(option, num, longOption);
			}
			return;
		}
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x0002F858 File Offset: 0x0002DA58
	public void SetFloat(Option option, float val)
	{
		string key;
		if (this.m_clientOptionMap.TryGetValue(option, out key))
		{
			bool flag = LocalOptions.Get().Has(key);
			float @float = LocalOptions.Get().GetFloat(key);
			if (!flag || @float != val)
			{
				LocalOptions.Get().Set(key, val);
				this.FireChangedEvent(option, @float, flag);
			}
			return;
		}
		ServerOption type;
		if (this.m_serverOptionMap.TryGetValue(option, out type))
		{
			float num;
			bool floatOption = NetCache.Get().GetFloatOption(type, out num);
			if (!floatOption || num != val)
			{
				NetCache.Get().SetFloatOption(type, val);
				this.FireChangedEvent(option, num, floatOption);
			}
			return;
		}
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0002F90C File Offset: 0x0002DB0C
	public void SetULong(Option option, ulong val)
	{
		string key;
		if (this.m_clientOptionMap.TryGetValue(option, out key))
		{
			bool flag = LocalOptions.Get().Has(key);
			ulong @ulong = LocalOptions.Get().GetULong(key);
			if (!flag || @ulong != val)
			{
				LocalOptions.Get().Set(key, val);
				this.FireChangedEvent(option, @ulong, flag);
			}
			return;
		}
		ServerOption type;
		if (this.m_serverOptionMap.TryGetValue(option, out type))
		{
			ulong num;
			bool ulongOption = NetCache.Get().GetULongOption(type, out num);
			if (!ulongOption || num != val)
			{
				NetCache.Get().SetULongOption(type, val);
				this.FireChangedEvent(option, num, ulongOption);
			}
			return;
		}
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0002F9C0 File Offset: 0x0002DBC0
	public void SetString(Option option, string val)
	{
		string key;
		if (this.m_clientOptionMap.TryGetValue(option, out key))
		{
			bool flag = LocalOptions.Get().Has(key);
			string @string = LocalOptions.Get().GetString(key);
			if (!flag || @string != val)
			{
				LocalOptions.Get().Set(key, val);
				this.FireChangedEvent(option, @string, flag);
			}
			return;
		}
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0002FA20 File Offset: 0x0002DC20
	public void SetEnum<T>(Option option, T val)
	{
		if (!Enum.IsDefined(typeof(T), val))
		{
			Error.AddDevFatal("Options.SetEnum() - {0} is not convertible to enum type {1} for option {2}", new object[]
			{
				val,
				typeof(T),
				option
			});
			return;
		}
		Type optionType = this.GetOptionType(option);
		if (optionType == typeof(int))
		{
			this.SetInt(option, Convert.ToInt32(val));
		}
		else if (optionType == typeof(long))
		{
			this.SetLong(option, Convert.ToInt64(val));
		}
		else
		{
			Error.AddDevFatal("Options.SetEnum() - option {0} has unsupported underlying type {1}", new object[]
			{
				option,
				optionType
			});
		}
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0002FAEC File Offset: 0x0002DCEC
	private void Initialize()
	{
		Array values = Enum.GetValues(typeof(Option));
		Map<string, Option> map = new Map<string, Option>();
		foreach (object obj in values)
		{
			Option option = (Option)((int)obj);
			if (option != Option.INVALID)
			{
				string key = option.ToString();
				map.Add(key, option);
			}
		}
		this.BuildClientOptionMap(map);
		this.BuildServerOptionMap(map);
		this.BuildServerOptionFlagMap(map);
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0002FB94 File Offset: 0x0002DD94
	private void BuildClientOptionMap(Map<string, Option> options)
	{
		this.m_clientOptionMap = new Map<Option, string>();
		foreach (object obj in Enum.GetValues(typeof(ClientOption)))
		{
			ClientOption clientOption = (ClientOption)((int)obj);
			if (clientOption != ClientOption.INVALID)
			{
				string key = clientOption.ToString();
				Option option;
				Type type;
				if (!options.TryGetValue(key, out option))
				{
					Debug.LogError(string.Format("Options.BuildClientOptionMap() - ClientOption {0} is not mirrored in the Option enum", clientOption));
				}
				else if (!OptionDataTables.s_typeMap.TryGetValue(option, out type))
				{
					Debug.LogError(string.Format("Options.BuildClientOptionMap() - ClientOption {0} has no type. Please add its type to the type map.", clientOption));
				}
				else
				{
					string @string = EnumUtils.GetString<Option>(option);
					this.m_clientOptionMap.Add(option, @string);
				}
			}
		}
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0002FC88 File Offset: 0x0002DE88
	private void BuildServerOptionMap(Map<string, Option> options)
	{
		this.m_serverOptionMap = new Map<Option, ServerOption>();
		foreach (object obj in Enum.GetValues(typeof(ServerOption)))
		{
			ServerOption serverOption = (ServerOption)((int)obj);
			if (serverOption != ServerOption.INVALID)
			{
				if (serverOption != ServerOption.LIMIT)
				{
					string text = serverOption.ToString();
					if (!text.StartsWith("FLAGS"))
					{
						if (!text.StartsWith("DEPRECATED"))
						{
							Option key;
							Type type;
							if (!options.TryGetValue(text, out key))
							{
								Debug.LogError(string.Format("Options.BuildServerOptionMap() - ServerOption {0} is not mirrored in the Option enum", serverOption));
							}
							else if (!OptionDataTables.s_typeMap.TryGetValue(key, out type))
							{
								Debug.LogError(string.Format("Options.BuildServerOptionMap() - ServerOption {0} has no type. Please add its type to the type map.", serverOption));
							}
							else if (type == typeof(bool))
							{
								Debug.LogError(string.Format("Options.BuildServerOptionMap() - ServerOption {0} is a bool. You should convert it to a ServerOptionFlag.", serverOption));
							}
							else
							{
								this.m_serverOptionMap.Add(key, serverOption);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0002FDD8 File Offset: 0x0002DFD8
	private void BuildServerOptionFlagMap(Map<string, Option> options)
	{
		this.m_serverOptionFlagMap = new Map<Option, ServerOptionFlag>();
		foreach (object obj in Enum.GetValues(typeof(ServerOptionFlag)))
		{
			ServerOptionFlag serverOptionFlag = (ServerOptionFlag)((int)obj);
			if (serverOptionFlag != ServerOptionFlag.LIMIT)
			{
				string text = serverOptionFlag.ToString();
				if (!text.StartsWith("DEPRECATED"))
				{
					Option key;
					if (!options.TryGetValue(text, out key))
					{
						Debug.LogError(string.Format("Options.BuildServerOptionFlagMap() - ServerOptionFlag {0} is not mirrored in the Option enum", serverOptionFlag));
					}
					else
					{
						this.m_serverOptionFlagMap.Add(key, serverOptionFlag);
					}
				}
			}
		}
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0002FEB4 File Offset: 0x0002E0B4
	private void GetServerOptionFlagInfo(ServerOptionFlag flag, out ServerOption container, out ulong flagBit, out ulong existenceBit)
	{
		int num = (int)(ServerOptionFlag.HAS_SEEN_TOURNAMENT * flag);
		int num2 = Mathf.FloorToInt((float)num * 0.015625f);
		int num3 = num % 64;
		int num4 = 1 + num3;
		container = Options.s_serverFlagContainers[num2];
		flagBit = 1UL << num3;
		existenceBit = 1UL << num4;
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0002FEF8 File Offset: 0x0002E0F8
	private bool HasServerOptionFlag(ServerOptionFlag serverOptionFlag)
	{
		ServerOption type;
		ulong num;
		ulong num2;
		this.GetServerOptionFlagInfo(serverOptionFlag, out type, out num, out num2);
		ulong ulongOption = NetCache.Get().GetULongOption(type);
		return (ulongOption & num2) != 0UL;
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0002FF2C File Offset: 0x0002E12C
	private void DeleteClientOption(Option option, string optionName)
	{
		if (!LocalOptions.Get().Has(optionName))
		{
			return;
		}
		object clientOption = this.GetClientOption(option, optionName);
		LocalOptions.Get().Delete(optionName);
		this.RemoveListeners(option, clientOption);
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0002FF68 File Offset: 0x0002E168
	private void DeleteServerOption(Option option, ServerOption serverOption)
	{
		if (!NetCache.Get().ClientOptionExists(serverOption))
		{
			return;
		}
		object serverOption2 = this.GetServerOption(option, serverOption);
		NetCache.Get().DeleteClientOption(serverOption);
		this.RemoveListeners(option, serverOption2);
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0002FFA4 File Offset: 0x0002E1A4
	private void DeleteServerOptionFlag(Option option, ServerOptionFlag serverOptionFlag)
	{
		ServerOption type;
		ulong num;
		ulong num2;
		this.GetServerOptionFlagInfo(serverOptionFlag, out type, out num, out num2);
		ulong num3 = NetCache.Get().GetULongOption(type);
		if ((num3 & num2) == 0UL)
		{
			return;
		}
		bool flag = (num3 & num) != 0UL;
		num3 &= ~num2;
		NetCache.Get().SetULongOption(type, num3);
		this.RemoveListeners(option, flag);
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0003000C File Offset: 0x0002E20C
	private object GetClientOption(Option option, string optionName)
	{
		Type optionType = this.GetOptionType(option);
		if (optionType == typeof(bool))
		{
			return LocalOptions.Get().GetBool(optionName);
		}
		if (optionType == typeof(int))
		{
			return LocalOptions.Get().GetInt(optionName);
		}
		if (optionType == typeof(long))
		{
			return LocalOptions.Get().GetLong(optionName);
		}
		if (optionType == typeof(ulong))
		{
			return LocalOptions.Get().GetULong(optionName);
		}
		if (optionType == typeof(float))
		{
			return LocalOptions.Get().GetFloat(optionName);
		}
		if (optionType == typeof(string))
		{
			return LocalOptions.Get().GetString(optionName);
		}
		Error.AddDevFatal("Options.GetClientOption() - option {0} has unsupported underlying type {1}", new object[]
		{
			option,
			optionType
		});
		return null;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00030100 File Offset: 0x0002E300
	private object GetServerOption(Option option, ServerOption serverOption)
	{
		Type optionType = this.GetOptionType(option);
		if (optionType == typeof(int))
		{
			return NetCache.Get().GetIntOption(serverOption);
		}
		if (optionType == typeof(long))
		{
			return NetCache.Get().GetLongOption(serverOption);
		}
		if (optionType == typeof(float))
		{
			return NetCache.Get().GetFloatOption(serverOption);
		}
		if (optionType == typeof(ulong))
		{
			return NetCache.Get().GetULongOption(serverOption);
		}
		Error.AddDevFatal("Options.GetServerOption() - option {0} has unsupported underlying type {1}", new object[]
		{
			option,
			optionType
		});
		return null;
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x000301B8 File Offset: 0x0002E3B8
	private bool GetOptionImpl(Option option, out object val)
	{
		val = null;
		string text;
		ServerOption serverOption;
		ServerOptionFlag flag;
		if (this.m_clientOptionMap.TryGetValue(option, out text))
		{
			if (LocalOptions.Get().Has(text))
			{
				val = this.GetClientOption(option, text);
			}
		}
		else if (this.m_serverOptionMap.TryGetValue(option, out serverOption))
		{
			if (NetCache.Get().ClientOptionExists(serverOption))
			{
				val = this.GetServerOption(option, serverOption);
			}
		}
		else if (this.m_serverOptionFlagMap.TryGetValue(option, out flag))
		{
			ulong num;
			ulong num2;
			this.GetServerOptionFlagInfo(flag, out serverOption, out num, out num2);
			ulong ulongOption = NetCache.Get().GetULongOption(serverOption);
			bool flag2 = (ulongOption & num2) != 0UL;
			if (flag2)
			{
				val = ((ulongOption & num) != 0UL);
			}
		}
		return val != null;
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00030288 File Offset: 0x0002E488
	private bool GetBoolImpl(Option option, out bool val)
	{
		val = false;
		object obj;
		if (this.GetOptionImpl(option, out obj))
		{
			val = (bool)obj;
			return true;
		}
		return false;
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x000302B4 File Offset: 0x0002E4B4
	private bool GetIntImpl(Option option, out int val)
	{
		val = 0;
		object obj;
		if (this.GetOptionImpl(option, out obj))
		{
			val = (int)obj;
			return true;
		}
		return false;
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x000302E0 File Offset: 0x0002E4E0
	private bool GetLongImpl(Option option, out long val)
	{
		val = 0L;
		object obj;
		if (this.GetOptionImpl(option, out obj))
		{
			val = (long)obj;
			return true;
		}
		return false;
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x0003030C File Offset: 0x0002E50C
	private bool GetFloatImpl(Option option, out float val)
	{
		val = 0f;
		object obj;
		if (this.GetOptionImpl(option, out obj))
		{
			val = (float)obj;
			return true;
		}
		return false;
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x0003033C File Offset: 0x0002E53C
	private bool GetULongImpl(Option option, out ulong val)
	{
		val = 0UL;
		object obj;
		if (this.GetOptionImpl(option, out obj))
		{
			val = (ulong)obj;
			return true;
		}
		return false;
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x00030368 File Offset: 0x0002E568
	private bool GetStringImpl(Option option, out string val)
	{
		val = string.Empty;
		object obj;
		if (this.GetOptionImpl(option, out obj))
		{
			val = (string)obj;
			return true;
		}
		return false;
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x00030398 File Offset: 0x0002E598
	private bool GetEnumImpl<T>(Option option, out T val)
	{
		val = default(T);
		object genericVal;
		return this.GetOptionImpl(option, out genericVal) && this.TranslateEnumVal<T>(option, genericVal, out val);
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x000303D0 File Offset: 0x0002E5D0
	private bool TranslateEnumVal<T>(Option option, object genericVal, out T val)
	{
		val = default(T);
		Type typeFromHandle = typeof(T);
		bool result;
		try
		{
			val = (T)((object)genericVal);
			result = true;
		}
		catch (Exception)
		{
			Debug.LogError(string.Format("Options.TranslateEnumVal() - option {0} has value {1}, which cannot be converted to type {2}", option, genericVal, typeFromHandle));
			result = false;
		}
		return result;
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00030444 File Offset: 0x0002E644
	private void RemoveListeners(Option option, object prevVal)
	{
		this.FireChangedEvent(option, prevVal, true);
		this.m_changedListeners.Remove(option);
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0003045C File Offset: 0x0002E65C
	private void FireChangedEvent(Option option, object prevVal, bool existed)
	{
		List<Options.ChangedListener> list;
		if (this.m_changedListeners.TryGetValue(option, out list))
		{
			Options.ChangedListener[] array = list.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Fire(option, prevVal, existed);
			}
		}
		Options.ChangedListener[] array2 = this.m_globalChangedListeners.ToArray();
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j].Fire(option, prevVal, existed);
		}
	}

	// Token: 0x0400052D RID: 1325
	private const string DEPRECATED_NAME_PREFIX = "DEPRECATED";

	// Token: 0x0400052E RID: 1326
	private const string FLAG_NAME_PREFIX = "FLAGS";

	// Token: 0x0400052F RID: 1327
	private const int FLAG_BIT_COUNT = 64;

	// Token: 0x04000530 RID: 1328
	private const float RCP_FLAG_BIT_COUNT = 0.015625f;

	// Token: 0x04000531 RID: 1329
	private static readonly ServerOption[] s_serverFlagContainers = new ServerOption[]
	{
		ServerOption.FLAGS1,
		ServerOption.FLAGS2,
		ServerOption.FLAGS3,
		ServerOption.FLAGS4,
		ServerOption.FLAGS5,
		ServerOption.FLAGS6,
		ServerOption.FLAGS7,
		ServerOption.FLAGS8,
		ServerOption.FLAGS9,
		ServerOption.FLAGS10
	};

	// Token: 0x04000532 RID: 1330
	private static Options s_instance;

	// Token: 0x04000533 RID: 1331
	private Map<Option, string> m_clientOptionMap;

	// Token: 0x04000534 RID: 1332
	private Map<Option, ServerOption> m_serverOptionMap;

	// Token: 0x04000535 RID: 1333
	private Map<Option, ServerOptionFlag> m_serverOptionFlagMap;

	// Token: 0x04000536 RID: 1334
	private Map<Option, List<Options.ChangedListener>> m_changedListeners = new Map<Option, List<Options.ChangedListener>>();

	// Token: 0x04000537 RID: 1335
	private List<Options.ChangedListener> m_globalChangedListeners = new List<Options.ChangedListener>();

	// Token: 0x02000365 RID: 869
	// (Invoke) Token: 0x06002C66 RID: 11366
	public delegate void ChangedCallback(Option option, object prevValue, bool existed, object userData);

	// Token: 0x0200046B RID: 1131
	private class ChangedListener : EventListener<Options.ChangedCallback>
	{
		// Token: 0x06003793 RID: 14227 RVA: 0x00110849 File Offset: 0x0010EA49
		public void Fire(Option option, object prevValue, bool didExist)
		{
			this.m_callback(option, prevValue, didExist, this.m_userData);
		}
	}
}
