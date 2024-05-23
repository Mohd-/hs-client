using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200009E RID: 158
public class PlatformSettings
{
	// Token: 0x06000767 RID: 1895 RVA: 0x0001D5B2 File Offset: 0x0001B7B2
	static PlatformSettings()
	{
		PlatformSettings.RecomputeDeviceSettings();
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000768 RID: 1896 RVA: 0x0001D5D7 File Offset: 0x0001B7D7
	public static OSCategory OS
	{
		get
		{
			return PlatformSettings.s_os;
		}
	}

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000769 RID: 1897 RVA: 0x0001D5DE File Offset: 0x0001B7DE
	public static MemoryCategory Memory
	{
		get
		{
			return PlatformSettings.s_memory;
		}
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x0600076A RID: 1898 RVA: 0x0001D5E5 File Offset: 0x0001B7E5
	public static ScreenCategory Screen
	{
		get
		{
			return PlatformSettings.s_screen;
		}
	}

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x0600076B RID: 1899 RVA: 0x0001D5EC File Offset: 0x0001B7EC
	public static InputCategory Input
	{
		get
		{
			return PlatformSettings.s_input;
		}
	}

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x0600076C RID: 1900 RVA: 0x0001D5F3 File Offset: 0x0001B7F3
	public static ScreenDensityCategory ScreenDensity
	{
		get
		{
			return PlatformSettings.s_screenDensity;
		}
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x0600076D RID: 1901 RVA: 0x0001D5FA File Offset: 0x0001B7FA
	public static string DeviceName
	{
		get
		{
			if (string.IsNullOrEmpty(SystemInfo.deviceModel))
			{
				return "unknown";
			}
			return SystemInfo.deviceModel;
		}
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x0001D618 File Offset: 0x0001B818
	public static int GetBestScreenMatch(List<ScreenCategory> categories)
	{
		ScreenCategory screen = PlatformSettings.Screen;
		int result = 0;
		int num = int.MaxValue;
		for (int i = 0; i < categories.Count; i++)
		{
			int num2 = categories[i] - screen;
			if (num2 >= 0 && num2 < num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x0001D66C File Offset: 0x0001B86C
	private static void RecomputeDeviceSettings()
	{
		if (PlatformSettings.EmulateMobileDevice())
		{
			return;
		}
		PlatformSettings.s_os = OSCategory.PC;
		PlatformSettings.s_input = InputCategory.Mouse;
		PlatformSettings.s_screen = ScreenCategory.PC;
		PlatformSettings.s_screenDensity = ScreenDensityCategory.High;
		PlatformSettings.s_os = OSCategory.PC;
		int systemMemorySize = SystemInfo.systemMemorySize;
		if (systemMemorySize < 500)
		{
			Debug.LogWarning("Low Memory Warning: Device has only " + systemMemorySize + "MBs of system memory");
			PlatformSettings.s_memory = MemoryCategory.Low;
		}
		else if (systemMemorySize < 1000)
		{
			PlatformSettings.s_memory = MemoryCategory.Low;
		}
		else if (systemMemorySize < 1500)
		{
			PlatformSettings.s_memory = MemoryCategory.Medium;
		}
		else
		{
			PlatformSettings.s_memory = MemoryCategory.High;
		}
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x0001D70C File Offset: 0x0001B90C
	private static bool EmulateMobileDevice()
	{
		ConfigFile configFile = new ConfigFile();
		if (!configFile.FullLoad(Vars.GetClientConfigPath()))
		{
			Blizzard.Log.Warning("Failed to read DeviceEmulation from client.config");
			return false;
		}
		DevicePreset devicePreset = new DevicePreset();
		devicePreset.ReadFromConfig(configFile);
		if (devicePreset.name == "No Emulation")
		{
			return false;
		}
		if (!configFile.Get("Emulation.emulateOnDevice", false))
		{
			return false;
		}
		PlatformSettings.s_os = devicePreset.os;
		PlatformSettings.s_input = devicePreset.input;
		PlatformSettings.s_screen = devicePreset.screen;
		PlatformSettings.s_screenDensity = devicePreset.screenDensity;
		Log.DeviceEmulation.Print("Emulating an " + devicePreset.name, new object[0]);
		return true;
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x0001D7BF File Offset: 0x0001B9BF
	private static void SetIOSSettings()
	{
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x0001D7C1 File Offset: 0x0001B9C1
	private static void SetAndroidSettings()
	{
		PlatformSettings.s_os = OSCategory.Android;
		PlatformSettings.s_input = InputCategory.Touch;
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x0001D7CF File Offset: 0x0001B9CF
	public static void Refresh()
	{
		PlatformSettings.RecomputeDeviceSettings();
	}

	// Token: 0x040003E7 RID: 999
	public static bool s_isDeviceSupported = true;

	// Token: 0x040003E8 RID: 1000
	public static OSCategory s_os = OSCategory.PC;

	// Token: 0x040003E9 RID: 1001
	public static MemoryCategory s_memory = MemoryCategory.High;

	// Token: 0x040003EA RID: 1002
	public static ScreenCategory s_screen = ScreenCategory.PC;

	// Token: 0x040003EB RID: 1003
	public static InputCategory s_input;

	// Token: 0x040003EC RID: 1004
	public static ScreenDensityCategory s_screenDensity = ScreenDensityCategory.High;
}
