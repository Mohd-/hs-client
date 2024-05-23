using System;
using UnityEngine;

// Token: 0x02000B1C RID: 2844
public class FBSettings : ScriptableObject
{
	// Token: 0x1700091D RID: 2333
	// (get) Token: 0x06006140 RID: 24896 RVA: 0x001D0BB0 File Offset: 0x001CEDB0
	private static FBSettings Instance
	{
		get
		{
			if (FBSettings.instance == null)
			{
				FBSettings.instance = (Resources.Load("FacebookSettings") as FBSettings);
				if (FBSettings.instance == null)
				{
					FBSettings.instance = ScriptableObject.CreateInstance<FBSettings>();
				}
			}
			return FBSettings.instance;
		}
	}

	// Token: 0x06006141 RID: 24897 RVA: 0x001D0C00 File Offset: 0x001CEE00
	public void SetAppIndex(int index)
	{
		if (this.selectedAppIndex != index)
		{
			this.selectedAppIndex = index;
			FBSettings.DirtyEditor();
		}
	}

	// Token: 0x1700091E RID: 2334
	// (get) Token: 0x06006142 RID: 24898 RVA: 0x001D0C1A File Offset: 0x001CEE1A
	public int SelectedAppIndex
	{
		get
		{
			return this.selectedAppIndex;
		}
	}

	// Token: 0x06006143 RID: 24899 RVA: 0x001D0C22 File Offset: 0x001CEE22
	public void SetAppId(int index, string value)
	{
		if (this.appIds[index] != value)
		{
			this.appIds[index] = value;
			FBSettings.DirtyEditor();
		}
	}

	// Token: 0x1700091F RID: 2335
	// (get) Token: 0x06006144 RID: 24900 RVA: 0x001D0C45 File Offset: 0x001CEE45
	// (set) Token: 0x06006145 RID: 24901 RVA: 0x001D0C4D File Offset: 0x001CEE4D
	public string[] AppIds
	{
		get
		{
			return this.appIds;
		}
		set
		{
			if (this.appIds != value)
			{
				this.appIds = value;
				FBSettings.DirtyEditor();
			}
		}
	}

	// Token: 0x06006146 RID: 24902 RVA: 0x001D0C67 File Offset: 0x001CEE67
	public void SetAppLabel(int index, string value)
	{
		if (this.appLabels[index] != value)
		{
			this.AppLabels[index] = value;
			FBSettings.DirtyEditor();
		}
	}

	// Token: 0x17000920 RID: 2336
	// (get) Token: 0x06006147 RID: 24903 RVA: 0x001D0C8A File Offset: 0x001CEE8A
	// (set) Token: 0x06006148 RID: 24904 RVA: 0x001D0C92 File Offset: 0x001CEE92
	public string[] AppLabels
	{
		get
		{
			return this.appLabels;
		}
		set
		{
			if (this.appLabels != value)
			{
				this.appLabels = value;
				FBSettings.DirtyEditor();
			}
		}
	}

	// Token: 0x17000921 RID: 2337
	// (get) Token: 0x06006149 RID: 24905 RVA: 0x001D0CAC File Offset: 0x001CEEAC
	public static string[] AllAppIds
	{
		get
		{
			return FBSettings.Instance.AppIds;
		}
	}

	// Token: 0x17000922 RID: 2338
	// (get) Token: 0x0600614A RID: 24906 RVA: 0x001D0CB8 File Offset: 0x001CEEB8
	public static string AppId
	{
		get
		{
			return FBSettings.Instance.AppIds[FBSettings.Instance.SelectedAppIndex];
		}
	}

	// Token: 0x17000923 RID: 2339
	// (get) Token: 0x0600614B RID: 24907 RVA: 0x001D0CDC File Offset: 0x001CEEDC
	public static bool IsValidAppId
	{
		get
		{
			return FBSettings.AppId != null && FBSettings.AppId.Length > 0 && !FBSettings.AppId.Equals("0");
		}
	}

	// Token: 0x17000924 RID: 2340
	// (get) Token: 0x0600614C RID: 24908 RVA: 0x001D0D18 File Offset: 0x001CEF18
	// (set) Token: 0x0600614D RID: 24909 RVA: 0x001D0D24 File Offset: 0x001CEF24
	public static bool Cookie
	{
		get
		{
			return FBSettings.Instance.cookie;
		}
		set
		{
			if (FBSettings.Instance.cookie != value)
			{
				FBSettings.Instance.cookie = value;
				FBSettings.DirtyEditor();
			}
		}
	}

	// Token: 0x17000925 RID: 2341
	// (get) Token: 0x0600614E RID: 24910 RVA: 0x001D0D51 File Offset: 0x001CEF51
	// (set) Token: 0x0600614F RID: 24911 RVA: 0x001D0D60 File Offset: 0x001CEF60
	public static bool Logging
	{
		get
		{
			return FBSettings.Instance.logging;
		}
		set
		{
			if (FBSettings.Instance.logging != value)
			{
				FBSettings.Instance.logging = value;
				FBSettings.DirtyEditor();
			}
		}
	}

	// Token: 0x17000926 RID: 2342
	// (get) Token: 0x06006150 RID: 24912 RVA: 0x001D0D8D File Offset: 0x001CEF8D
	// (set) Token: 0x06006151 RID: 24913 RVA: 0x001D0D9C File Offset: 0x001CEF9C
	public static bool Status
	{
		get
		{
			return FBSettings.Instance.status;
		}
		set
		{
			if (FBSettings.Instance.status != value)
			{
				FBSettings.Instance.status = value;
				FBSettings.DirtyEditor();
			}
		}
	}

	// Token: 0x17000927 RID: 2343
	// (get) Token: 0x06006152 RID: 24914 RVA: 0x001D0DC9 File Offset: 0x001CEFC9
	// (set) Token: 0x06006153 RID: 24915 RVA: 0x001D0DD8 File Offset: 0x001CEFD8
	public static bool Xfbml
	{
		get
		{
			return FBSettings.Instance.xfbml;
		}
		set
		{
			if (FBSettings.Instance.xfbml != value)
			{
				FBSettings.Instance.xfbml = value;
				FBSettings.DirtyEditor();
			}
		}
	}

	// Token: 0x17000928 RID: 2344
	// (get) Token: 0x06006154 RID: 24916 RVA: 0x001D0E05 File Offset: 0x001CF005
	// (set) Token: 0x06006155 RID: 24917 RVA: 0x001D0E11 File Offset: 0x001CF011
	public static string IosURLSuffix
	{
		get
		{
			return FBSettings.Instance.iosURLSuffix;
		}
		set
		{
			if (FBSettings.Instance.iosURLSuffix != value)
			{
				FBSettings.Instance.iosURLSuffix = value;
				FBSettings.DirtyEditor();
			}
		}
	}

	// Token: 0x17000929 RID: 2345
	// (get) Token: 0x06006156 RID: 24918 RVA: 0x001D0E38 File Offset: 0x001CF038
	public static string ChannelUrl
	{
		get
		{
			return "/channel.html";
		}
	}

	// Token: 0x1700092A RID: 2346
	// (get) Token: 0x06006157 RID: 24919 RVA: 0x001D0E3F File Offset: 0x001CF03F
	// (set) Token: 0x06006158 RID: 24920 RVA: 0x001D0E4C File Offset: 0x001CF04C
	public static bool FrictionlessRequests
	{
		get
		{
			return FBSettings.Instance.frictionlessRequests;
		}
		set
		{
			if (FBSettings.Instance.frictionlessRequests != value)
			{
				FBSettings.Instance.frictionlessRequests = value;
				FBSettings.DirtyEditor();
			}
		}
	}

	// Token: 0x06006159 RID: 24921 RVA: 0x001D0E79 File Offset: 0x001CF079
	private static void DirtyEditor()
	{
	}

	// Token: 0x04004894 RID: 18580
	private const string facebookSettingsAssetName = "FacebookSettings";

	// Token: 0x04004895 RID: 18581
	private const string facebookSettingsPath = "MobileAdTracking/Facebook/Resources";

	// Token: 0x04004896 RID: 18582
	private const string facebookSettingsAssetExtension = ".asset";

	// Token: 0x04004897 RID: 18583
	private static FBSettings instance;

	// Token: 0x04004898 RID: 18584
	[SerializeField]
	private int selectedAppIndex;

	// Token: 0x04004899 RID: 18585
	[SerializeField]
	private string[] appIds = new string[]
	{
		"0"
	};

	// Token: 0x0400489A RID: 18586
	[SerializeField]
	private string[] appLabels = new string[]
	{
		"App Name"
	};

	// Token: 0x0400489B RID: 18587
	[SerializeField]
	private bool cookie = true;

	// Token: 0x0400489C RID: 18588
	[SerializeField]
	private bool logging = true;

	// Token: 0x0400489D RID: 18589
	[SerializeField]
	private bool status = true;

	// Token: 0x0400489E RID: 18590
	[SerializeField]
	private bool xfbml;

	// Token: 0x0400489F RID: 18591
	[SerializeField]
	private bool frictionlessRequests = true;

	// Token: 0x040048A0 RID: 18592
	[SerializeField]
	private string iosURLSuffix = string.Empty;
}
