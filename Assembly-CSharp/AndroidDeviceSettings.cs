using System;
using System.Text;
using UnityEngine;

// Token: 0x02000699 RID: 1689
public class AndroidDeviceSettings
{
	// Token: 0x0600473A RID: 18234 RVA: 0x00155A7F File Offset: 0x00153C7F
	private AndroidDeviceSettings()
	{
	}

	// Token: 0x0600473B RID: 18235 RVA: 0x00155A9C File Offset: 0x00153C9C
	public static bool IsCurrentTextureFormatSupported()
	{
		Map<string, TextureFormat> map = new Map<string, TextureFormat>
		{
			{
				string.Empty,
				5
			},
			{
				"etc1",
				34
			},
			{
				"etc2",
				47
			},
			{
				"astc",
				58
			},
			{
				"atc",
				36
			},
			{
				"dxt",
				12
			},
			{
				"pvrtc",
				33
			}
		};
		bool flag = SystemInfo.SupportsTextureFormat(map[string.Empty]);
		Debug.Log("Checking whether texture format of build () is supported? " + flag);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("All supported texture formats: ");
		foreach (object obj in Enum.GetValues(typeof(TextureFormat)))
		{
			TextureFormat textureFormat = (int)obj;
			try
			{
				if (SystemInfo.SupportsTextureFormat(textureFormat))
				{
					stringBuilder.Append(textureFormat + ", ");
				}
			}
			catch (ArgumentException)
			{
			}
		}
		Log.Graphics.Print(stringBuilder.ToString(), new object[0]);
		return flag;
	}

	// Token: 0x0600473C RID: 18236 RVA: 0x00155BFC File Offset: 0x00153DFC
	public bool IsMusicPlaying()
	{
		return false;
	}

	// Token: 0x0600473D RID: 18237 RVA: 0x00155BFF File Offset: 0x00153DFF
	public static AndroidDeviceSettings Get()
	{
		if (AndroidDeviceSettings.s_instance == null)
		{
			AndroidDeviceSettings.s_instance = new AndroidDeviceSettings();
		}
		return AndroidDeviceSettings.s_instance;
	}

	// Token: 0x04002E2E RID: 11822
	public const int SCREENLAYOUT_SIZE_XLARGE = 4;

	// Token: 0x04002E2F RID: 11823
	private static AndroidDeviceSettings s_instance;

	// Token: 0x04002E30 RID: 11824
	public float heightPixels;

	// Token: 0x04002E31 RID: 11825
	public float widthPixels;

	// Token: 0x04002E32 RID: 11826
	public float xdpi;

	// Token: 0x04002E33 RID: 11827
	public float ydpi;

	// Token: 0x04002E34 RID: 11828
	public float widthInches;

	// Token: 0x04002E35 RID: 11829
	public float heightInches;

	// Token: 0x04002E36 RID: 11830
	public float diagonalInches;

	// Token: 0x04002E37 RID: 11831
	public float aspectRatio;

	// Token: 0x04002E38 RID: 11832
	public int densityDpi = 300;

	// Token: 0x04002E39 RID: 11833
	public bool isExtraLarge = true;

	// Token: 0x04002E3A RID: 11834
	public bool isOnTabletWhitelist;

	// Token: 0x04002E3B RID: 11835
	public bool isOnPhoneWhitelist;

	// Token: 0x04002E3C RID: 11836
	public string applicationStorageFolder;

	// Token: 0x04002E3D RID: 11837
	public int screenLayout;
}
