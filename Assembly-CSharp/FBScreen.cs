using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B19 RID: 2841
public class FBScreen
{
	// Token: 0x17000918 RID: 2328
	// (get) Token: 0x0600612F RID: 24879 RVA: 0x001D0A62 File Offset: 0x001CEC62
	// (set) Token: 0x06006130 RID: 24880 RVA: 0x001D0A69 File Offset: 0x001CEC69
	public static bool FullScreen
	{
		get
		{
			return Screen.fullScreen;
		}
		set
		{
			Screen.fullScreen = value;
		}
	}

	// Token: 0x17000919 RID: 2329
	// (get) Token: 0x06006131 RID: 24881 RVA: 0x001D0A71 File Offset: 0x001CEC71
	public static bool Resizable
	{
		get
		{
			return FBScreen.resizable;
		}
	}

	// Token: 0x1700091A RID: 2330
	// (get) Token: 0x06006132 RID: 24882 RVA: 0x001D0A78 File Offset: 0x001CEC78
	public static int Width
	{
		get
		{
			return Screen.width;
		}
	}

	// Token: 0x1700091B RID: 2331
	// (get) Token: 0x06006133 RID: 24883 RVA: 0x001D0A7F File Offset: 0x001CEC7F
	public static int Height
	{
		get
		{
			return Screen.height;
		}
	}

	// Token: 0x06006134 RID: 24884 RVA: 0x001D0A86 File Offset: 0x001CEC86
	public static void SetResolution(int width, int height, bool fullscreen, int preferredRefreshRate = 0, params FBScreen.Layout[] layoutParams)
	{
		Screen.SetResolution(width, height, fullscreen, preferredRefreshRate);
	}

	// Token: 0x06006135 RID: 24885 RVA: 0x001D0A94 File Offset: 0x001CEC94
	public static void SetAspectRatio(int width, int height, params FBScreen.Layout[] layoutParams)
	{
		int num = Screen.height / height * width;
		Screen.SetResolution(num, Screen.height, Screen.fullScreen);
	}

	// Token: 0x06006136 RID: 24886 RVA: 0x001D0ABB File Offset: 0x001CECBB
	public static void SetUnityPlayerEmbedCSS(string key, string value)
	{
	}

	// Token: 0x06006137 RID: 24887 RVA: 0x001D0AC0 File Offset: 0x001CECC0
	public static FBScreen.Layout.OptionLeft Left(float amount)
	{
		return new FBScreen.Layout.OptionLeft
		{
			Amount = amount
		};
	}

	// Token: 0x06006138 RID: 24888 RVA: 0x001D0ADC File Offset: 0x001CECDC
	public static FBScreen.Layout.OptionTop Top(float amount)
	{
		return new FBScreen.Layout.OptionTop
		{
			Amount = amount
		};
	}

	// Token: 0x06006139 RID: 24889 RVA: 0x001D0AF7 File Offset: 0x001CECF7
	public static FBScreen.Layout.OptionCenterHorizontal CenterHorizontal()
	{
		return new FBScreen.Layout.OptionCenterHorizontal();
	}

	// Token: 0x0600613A RID: 24890 RVA: 0x001D0AFE File Offset: 0x001CECFE
	public static FBScreen.Layout.OptionCenterVertical CenterVertical()
	{
		return new FBScreen.Layout.OptionCenterVertical();
	}

	// Token: 0x0600613B RID: 24891 RVA: 0x001D0B05 File Offset: 0x001CED05
	private static void SetLayout(IEnumerable<FBScreen.Layout> parameters)
	{
	}

	// Token: 0x04004893 RID: 18579
	private static bool resizable;

	// Token: 0x02000B1A RID: 2842
	public class Layout
	{
		// Token: 0x02000B23 RID: 2851
		public class OptionLeft : FBScreen.Layout
		{
			// Token: 0x040048C4 RID: 18628
			public float Amount;
		}

		// Token: 0x02000B24 RID: 2852
		public class OptionTop : FBScreen.Layout
		{
			// Token: 0x040048C5 RID: 18629
			public float Amount;
		}

		// Token: 0x02000B25 RID: 2853
		public class OptionCenterHorizontal : FBScreen.Layout
		{
		}

		// Token: 0x02000B26 RID: 2854
		public class OptionCenterVertical : FBScreen.Layout
		{
		}
	}
}
