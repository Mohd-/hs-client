using System;
using UnityEngine;

// Token: 0x0200009F RID: 159
public class PlatformDependentValue<T>
{
	// Token: 0x06000774 RID: 1908 RVA: 0x0001D7D8 File Offset: 0x0001B9D8
	public PlatformDependentValue(PlatformCategory t)
	{
		this.type = t;
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000775 RID: 1909 RVA: 0x0001D88C File Offset: 0x0001BA8C
	// (set) Token: 0x06000776 RID: 1910 RVA: 0x0001D899 File Offset: 0x0001BA99
	public T PC
	{
		get
		{
			return this.PCSetting.Get();
		}
		set
		{
			this.PCSetting.Set(value);
		}
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000777 RID: 1911 RVA: 0x0001D8A7 File Offset: 0x0001BAA7
	// (set) Token: 0x06000778 RID: 1912 RVA: 0x0001D8B4 File Offset: 0x0001BAB4
	public T Mac
	{
		get
		{
			return this.MacSetting.Get();
		}
		set
		{
			this.MacSetting.Set(value);
		}
	}

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000779 RID: 1913 RVA: 0x0001D8C2 File Offset: 0x0001BAC2
	// (set) Token: 0x0600077A RID: 1914 RVA: 0x0001D8CF File Offset: 0x0001BACF
	public T iOS
	{
		get
		{
			return this.iOSSetting.Get();
		}
		set
		{
			this.iOSSetting.Set(value);
		}
	}

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x0600077B RID: 1915 RVA: 0x0001D8DD File Offset: 0x0001BADD
	// (set) Token: 0x0600077C RID: 1916 RVA: 0x0001D8EA File Offset: 0x0001BAEA
	public T Android
	{
		get
		{
			return this.AndroidSetting.Get();
		}
		set
		{
			this.AndroidSetting.Set(value);
		}
	}

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x0600077D RID: 1917 RVA: 0x0001D8F8 File Offset: 0x0001BAF8
	// (set) Token: 0x0600077E RID: 1918 RVA: 0x0001D905 File Offset: 0x0001BB05
	public T Tablet
	{
		get
		{
			return this.TabletSetting.Get();
		}
		set
		{
			this.TabletSetting.Set(value);
		}
	}

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x0600077F RID: 1919 RVA: 0x0001D913 File Offset: 0x0001BB13
	// (set) Token: 0x06000780 RID: 1920 RVA: 0x0001D920 File Offset: 0x0001BB20
	public T MiniTablet
	{
		get
		{
			return this.MiniTabletSetting.Get();
		}
		set
		{
			this.MiniTabletSetting.Set(value);
		}
	}

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000781 RID: 1921 RVA: 0x0001D92E File Offset: 0x0001BB2E
	// (set) Token: 0x06000782 RID: 1922 RVA: 0x0001D93B File Offset: 0x0001BB3B
	public T Phone
	{
		get
		{
			return this.PhoneSetting.Get();
		}
		set
		{
			this.PhoneSetting.Set(value);
		}
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000783 RID: 1923 RVA: 0x0001D949 File Offset: 0x0001BB49
	// (set) Token: 0x06000784 RID: 1924 RVA: 0x0001D956 File Offset: 0x0001BB56
	public T Mouse
	{
		get
		{
			return this.MouseSetting.Get();
		}
		set
		{
			this.MouseSetting.Set(value);
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000785 RID: 1925 RVA: 0x0001D964 File Offset: 0x0001BB64
	// (set) Token: 0x06000786 RID: 1926 RVA: 0x0001D971 File Offset: 0x0001BB71
	public T Touch
	{
		get
		{
			return this.TouchSetting.Get();
		}
		set
		{
			this.TouchSetting.Set(value);
		}
	}

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000787 RID: 1927 RVA: 0x0001D97F File Offset: 0x0001BB7F
	// (set) Token: 0x06000788 RID: 1928 RVA: 0x0001D98C File Offset: 0x0001BB8C
	public T LowMemory
	{
		get
		{
			return this.LowMemorySetting.Get();
		}
		set
		{
			this.LowMemorySetting.Set(value);
		}
	}

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000789 RID: 1929 RVA: 0x0001D99A File Offset: 0x0001BB9A
	// (set) Token: 0x0600078A RID: 1930 RVA: 0x0001D9A7 File Offset: 0x0001BBA7
	public T MediumMemory
	{
		get
		{
			return this.MediumMemorySetting.Get();
		}
		set
		{
			this.MediumMemorySetting.Set(value);
		}
	}

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x0600078B RID: 1931 RVA: 0x0001D9B5 File Offset: 0x0001BBB5
	// (set) Token: 0x0600078C RID: 1932 RVA: 0x0001D9C2 File Offset: 0x0001BBC2
	public T HighMemory
	{
		get
		{
			return this.HighMemorySetting.Get();
		}
		set
		{
			this.HighMemorySetting.Set(value);
		}
	}

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x0600078D RID: 1933 RVA: 0x0001D9D0 File Offset: 0x0001BBD0
	// (set) Token: 0x0600078E RID: 1934 RVA: 0x0001D9DD File Offset: 0x0001BBDD
	public T NormalScreenDensity
	{
		get
		{
			return this.NormalScreenDensitySetting.Get();
		}
		set
		{
			this.NormalScreenDensitySetting.Set(value);
		}
	}

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x0600078F RID: 1935 RVA: 0x0001D9EB File Offset: 0x0001BBEB
	// (set) Token: 0x06000790 RID: 1936 RVA: 0x0001D9F8 File Offset: 0x0001BBF8
	public T HighScreenDensity
	{
		get
		{
			return this.HighScreenDensitySetting.Get();
		}
		set
		{
			this.HighScreenDensitySetting.Set(value);
		}
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x0001DA06 File Offset: 0x0001BC06
	public void Reset()
	{
		this.resolved = false;
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000792 RID: 1938 RVA: 0x0001DA10 File Offset: 0x0001BC10
	private T Value
	{
		get
		{
			if (this.resolved)
			{
				return this.result;
			}
			switch (this.type)
			{
			case PlatformCategory.OS:
				this.result = this.GetOSSetting(PlatformSettings.OS);
				break;
			case PlatformCategory.Screen:
				this.result = this.GetScreenSetting(PlatformSettings.Screen);
				break;
			case PlatformCategory.Memory:
				this.result = this.GetMemorySetting(PlatformSettings.Memory);
				break;
			case PlatformCategory.Input:
				this.result = this.GetInputSetting(PlatformSettings.Input);
				break;
			}
			this.resolved = true;
			return this.result;
		}
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x0001DAC0 File Offset: 0x0001BCC0
	private T GetOSSetting(OSCategory os)
	{
		switch (os)
		{
		case OSCategory.PC:
			if (this.PCSetting.WasSet)
			{
				return this.PC;
			}
			break;
		case OSCategory.Mac:
			return (!this.MacSetting.WasSet) ? this.GetOSSetting(OSCategory.PC) : this.Mac;
		case OSCategory.iOS:
			return (!this.iOSSetting.WasSet) ? this.GetOSSetting(OSCategory.PC) : this.iOS;
		case OSCategory.Android:
			return (!this.AndroidSetting.WasSet) ? this.GetOSSetting(OSCategory.PC) : this.Android;
		}
		Debug.LogError("Could not find OS dependent value");
		return default(T);
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x0001DB84 File Offset: 0x0001BD84
	private T GetScreenSetting(ScreenCategory screen)
	{
		switch (screen)
		{
		case ScreenCategory.Phone:
			return (!this.PhoneSetting.WasSet) ? this.GetScreenSetting(ScreenCategory.Tablet) : this.Phone;
		case ScreenCategory.MiniTablet:
			return (!this.MiniTabletSetting.WasSet) ? this.GetScreenSetting(ScreenCategory.Tablet) : this.MiniTablet;
		case ScreenCategory.Tablet:
			return (!this.TabletSetting.WasSet) ? this.GetScreenSetting(ScreenCategory.PC) : this.Tablet;
		case ScreenCategory.PC:
			if (this.PCSetting.WasSet)
			{
				return this.PC;
			}
			break;
		}
		Debug.LogError("Could not find screen dependent value");
		return default(T);
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x0001DC48 File Offset: 0x0001BE48
	private T GetMemorySetting(MemoryCategory memory)
	{
		switch (memory)
		{
		case MemoryCategory.Low:
			if (this.LowMemorySetting.WasSet)
			{
				return this.LowMemory;
			}
			break;
		case MemoryCategory.Medium:
			return (!this.MediumMemorySetting.WasSet) ? this.GetMemorySetting(MemoryCategory.Low) : this.MediumMemory;
		case MemoryCategory.High:
			return (!this.HighMemorySetting.WasSet) ? this.GetMemorySetting(MemoryCategory.Medium) : this.HighMemory;
		}
		Debug.LogError("Could not find memory dependent value");
		return default(T);
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0001DCE4 File Offset: 0x0001BEE4
	private T GetInputSetting(InputCategory input)
	{
		if (input != InputCategory.Mouse)
		{
			if (input == InputCategory.Touch)
			{
				return (!this.TouchSetting.WasSet) ? this.GetInputSetting(InputCategory.Mouse) : this.Touch;
			}
		}
		else if (this.MouseSetting.WasSet)
		{
			return this.Mouse;
		}
		Debug.LogError("Could not find input dependent value");
		return default(T);
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0001DD58 File Offset: 0x0001BF58
	private T GetScreenDensitySetting(ScreenDensityCategory input)
	{
		if (input != ScreenDensityCategory.Normal)
		{
			if (input == ScreenDensityCategory.High)
			{
				return (!this.HighScreenDensitySetting.WasSet) ? this.GetScreenDensitySetting(ScreenDensityCategory.Normal) : this.HighScreenDensity;
			}
		}
		else if (this.NormalScreenDensitySetting.WasSet)
		{
			return this.NormalScreenDensity;
		}
		Debug.LogError("Could not find screen density dependent value");
		return default(T);
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x0001DDCB File Offset: 0x0001BFCB
	public static implicit operator T(PlatformDependentValue<T> val)
	{
		return val.Value;
	}

	// Token: 0x040003ED RID: 1005
	private bool resolved;

	// Token: 0x040003EE RID: 1006
	private T result;

	// Token: 0x040003EF RID: 1007
	private PlatformCategory type;

	// Token: 0x040003F0 RID: 1008
	private T defaultValue;

	// Token: 0x040003F1 RID: 1009
	private PlatformSetting<T> PCSetting = new PlatformSetting<T>();

	// Token: 0x040003F2 RID: 1010
	private PlatformSetting<T> MacSetting = new PlatformSetting<T>();

	// Token: 0x040003F3 RID: 1011
	private PlatformSetting<T> iOSSetting = new PlatformSetting<T>();

	// Token: 0x040003F4 RID: 1012
	private PlatformSetting<T> AndroidSetting = new PlatformSetting<T>();

	// Token: 0x040003F5 RID: 1013
	private PlatformSetting<T> TabletSetting = new PlatformSetting<T>();

	// Token: 0x040003F6 RID: 1014
	private PlatformSetting<T> MiniTabletSetting = new PlatformSetting<T>();

	// Token: 0x040003F7 RID: 1015
	private PlatformSetting<T> PhoneSetting = new PlatformSetting<T>();

	// Token: 0x040003F8 RID: 1016
	private PlatformSetting<T> MouseSetting = new PlatformSetting<T>();

	// Token: 0x040003F9 RID: 1017
	private PlatformSetting<T> TouchSetting = new PlatformSetting<T>();

	// Token: 0x040003FA RID: 1018
	private PlatformSetting<T> LowMemorySetting = new PlatformSetting<T>();

	// Token: 0x040003FB RID: 1019
	private PlatformSetting<T> MediumMemorySetting = new PlatformSetting<T>();

	// Token: 0x040003FC RID: 1020
	private PlatformSetting<T> HighMemorySetting = new PlatformSetting<T>();

	// Token: 0x040003FD RID: 1021
	private PlatformSetting<T> NormalScreenDensitySetting = new PlatformSetting<T>();

	// Token: 0x040003FE RID: 1022
	private PlatformSetting<T> HighScreenDensitySetting = new PlatformSetting<T>();
}
