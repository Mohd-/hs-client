using System;

// Token: 0x02000548 RID: 1352
[Serializable]
public class DevicePreset : ICloneable
{
	// Token: 0x06003E4E RID: 15950 RVA: 0x0012DC50 File Offset: 0x0012BE50
	// Note: this type is marked as 'beforefieldinit'.
	static DevicePreset()
	{
		DevicePresetList devicePresetList = new DevicePresetList();
		devicePresetList.Add(new DevicePreset
		{
			name = "No Emulation"
		});
		devicePresetList.Add(new DevicePreset
		{
			name = "PC",
			os = OSCategory.PC,
			screen = ScreenCategory.PC
		});
		devicePresetList.Add(new DevicePreset
		{
			name = "iPhone",
			os = OSCategory.iOS,
			screen = ScreenCategory.Phone,
			input = InputCategory.Touch
		});
		devicePresetList.Add(new DevicePreset
		{
			name = "iPad",
			os = OSCategory.iOS,
			screen = ScreenCategory.Tablet,
			input = InputCategory.Touch
		});
		devicePresetList.Add(new DevicePreset
		{
			name = "Android Phone",
			os = OSCategory.Android,
			screen = ScreenCategory.Phone,
			input = InputCategory.Touch
		});
		devicePresetList.Add(new DevicePreset
		{
			name = "Android Tablet",
			os = OSCategory.Android,
			screen = ScreenCategory.Tablet,
			input = InputCategory.Touch
		});
		DevicePreset.s_devicePresets = devicePresetList;
	}

	// Token: 0x06003E4F RID: 15951 RVA: 0x0012DD5B File Offset: 0x0012BF5B
	public object Clone()
	{
		return base.MemberwiseClone();
	}

	// Token: 0x06003E50 RID: 15952 RVA: 0x0012DD64 File Offset: 0x0012BF64
	public void ReadFromConfig(ConfigFile config)
	{
		this.name = config.Get("Emulation.DeviceName", this.name.ToString());
		DevicePreset devicePreset = DevicePreset.s_devicePresets.Find((DevicePreset x) => x.name.Equals(this.name));
		this.os = devicePreset.os;
		this.input = devicePreset.input;
		this.screen = devicePreset.screen;
		this.screenDensity = devicePreset.screenDensity;
	}

	// Token: 0x06003E51 RID: 15953 RVA: 0x0012DDD4 File Offset: 0x0012BFD4
	public void WriteToConfig(ConfigFile config)
	{
		Log.ConfigFile.Print("Writing Emulated Device: " + this.name + " to " + config.GetPath(), new object[0]);
		config.Set("Emulation.DeviceName", this.name.ToString());
		config.Delete("Emulation.OSCategory", true);
		config.Delete("Emulation.InputCategory", true);
		config.Delete("Emulation.ScreenCategory", true);
		config.Delete("Emulation.ScreenDensityCategory", true);
		config.Save(null);
	}

	// Token: 0x040027DF RID: 10207
	public static readonly DevicePresetList s_devicePresets;

	// Token: 0x040027E0 RID: 10208
	public string name = "No Emulation";

	// Token: 0x040027E1 RID: 10209
	public OSCategory os = OSCategory.PC;

	// Token: 0x040027E2 RID: 10210
	public InputCategory input;

	// Token: 0x040027E3 RID: 10211
	public ScreenCategory screen = ScreenCategory.PC;

	// Token: 0x040027E4 RID: 10212
	public ScreenDensityCategory screenDensity = ScreenDensityCategory.High;
}
