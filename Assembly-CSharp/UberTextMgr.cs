using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000F50 RID: 3920
public class UberTextMgr : MonoBehaviour
{
	// Token: 0x060074A0 RID: 29856 RVA: 0x00226AF2 File Offset: 0x00224CF2
	private void Awake()
	{
		UberTextMgr.s_Instance = this;
	}

	// Token: 0x060074A1 RID: 29857 RVA: 0x00226AFC File Offset: 0x00224CFC
	private void Start()
	{
		this.m_AtlasCharacters = this.BuildCharacterSet();
		Log.Kyle.Print("Updating Atlas to include: {0}", new object[]
		{
			this.m_AtlasCharacters
		});
	}

	// Token: 0x060074A2 RID: 29858 RVA: 0x00226B33 File Offset: 0x00224D33
	private void Update()
	{
		if (!this.m_Active)
		{
			return;
		}
		this.ForceEnglishCharactersInAtlas();
	}

	// Token: 0x060074A3 RID: 29859 RVA: 0x00226B47 File Offset: 0x00224D47
	public static UberTextMgr Get()
	{
		return UberTextMgr.s_Instance;
	}

	// Token: 0x060074A4 RID: 29860 RVA: 0x00226B50 File Offset: 0x00224D50
	public void StartAtlasUpdate()
	{
		Log.Kyle.Print("UberTextMgr.StartAtlasUpdate()", new object[0]);
		this.m_BelweFont = this.GetLocalizedFont(this.m_BelweFont);
		this.m_BelweOutlineFont = this.GetLocalizedFont(this.m_BelweOutlineFont);
		this.m_FranklinGothicFont = this.GetLocalizedFont(this.m_FranklinGothicFont);
		this.m_Active = true;
		Font.textureRebuilt += new Action<Font>(this.LogFontAtlasUpdate);
	}

	// Token: 0x060074A5 RID: 29861 RVA: 0x00226BC0 File Offset: 0x00224DC0
	private void ForceEnglishCharactersInAtlas()
	{
		if (this.m_FranklinGothicFont == null)
		{
			Debug.LogError("UberTextMgr: m_FranklinGothicFont == null");
			return;
		}
		this.m_FranklinGothicFont.RequestCharactersInTexture(this.m_AtlasCharacters, 40, 0);
		this.m_FranklinGothicFont.RequestCharactersInTexture(this.m_AtlasCharacters, 40, 2);
		if (this.m_BelweOutlineFont == null)
		{
			Debug.LogError("UberTextMgr: m_BelweOutlineFont == null");
			return;
		}
		this.m_BelweOutlineFont.RequestCharactersInTexture(this.m_AtlasCharacters, 40, 0);
		this.m_BelweOutlineFont.RequestCharactersInTexture(this.m_AtlasNumbers, 65, 0);
	}

	// Token: 0x060074A6 RID: 29862 RVA: 0x00226C58 File Offset: 0x00224E58
	private string BuildCharacterSet()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 33; i < 127; i++)
		{
			stringBuilder.Append((char)i);
		}
		for (int j = 192; j < 256; j++)
		{
			stringBuilder.Append((char)j);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060074A7 RID: 29863 RVA: 0x00226CB4 File Offset: 0x00224EB4
	private Font GetLocalizedFont(Font font)
	{
		FontTable fontTable = FontTable.Get();
		if (fontTable == null)
		{
			Debug.LogError("UberTextMgr: Error loading FontTable");
			return null;
		}
		FontDef fontDef = fontTable.GetFontDef(font);
		if (fontDef == null)
		{
			Debug.LogError("UberTextMgr: Error loading fontDef for: " + font.name);
			return null;
		}
		return fontDef.m_Font;
	}

	// Token: 0x060074A8 RID: 29864 RVA: 0x00226D10 File Offset: 0x00224F10
	private void LogFontAtlasUpdate(Font font)
	{
		if (font == this.m_BelweFont)
		{
			this.LogBelweAtlasUpdate();
		}
		else if (font == this.m_BelweOutlineFont)
		{
			this.LogBelweOutlineAtlasUpdate();
		}
		else if (font == this.m_FranklinGothicFont)
		{
			this.LogFranklinGothicAtlasUpdate();
		}
		else if (font == this.m_BlizzardGlobal)
		{
			this.LogBlizzardGlobalAtlasUpdate();
		}
	}

	// Token: 0x060074A9 RID: 29865 RVA: 0x00226D88 File Offset: 0x00224F88
	private void LogBelweAtlasUpdate()
	{
		int width = this.m_BelweFont.material.mainTexture.width;
		int height = this.m_BelweFont.material.mainTexture.height;
		Log.Kyle.Print("Belwe Atlas Updated: {0}, {1}", new object[]
		{
			width,
			height
		});
	}

	// Token: 0x060074AA RID: 29866 RVA: 0x00226DE8 File Offset: 0x00224FE8
	private void LogBelweOutlineAtlasUpdate()
	{
		int width = this.m_BelweOutlineFont.material.mainTexture.width;
		int height = this.m_BelweOutlineFont.material.mainTexture.height;
		Log.Kyle.Print("BelweOutline Atlas Updated: {0}, {1}", new object[]
		{
			width,
			height
		});
	}

	// Token: 0x060074AB RID: 29867 RVA: 0x00226E48 File Offset: 0x00225048
	private void LogFranklinGothicAtlasUpdate()
	{
		int width = this.m_FranklinGothicFont.material.mainTexture.width;
		int height = this.m_FranklinGothicFont.material.mainTexture.height;
		Log.Kyle.Print("Franklin Gothic Atlas Updated: {0}, {1}", new object[]
		{
			width,
			height
		});
	}

	// Token: 0x060074AC RID: 29868 RVA: 0x00226EA8 File Offset: 0x002250A8
	private void LogBlizzardGlobalAtlasUpdate()
	{
		int width = this.m_BlizzardGlobal.material.mainTexture.width;
		int height = this.m_BlizzardGlobal.material.mainTexture.height;
		Log.Kyle.Print("Blizzard Global Atlas Updated: {0}, {1}", new object[]
		{
			width,
			height
		});
	}

	// Token: 0x04005F0C RID: 24332
	public Font m_BelweFont;

	// Token: 0x04005F0D RID: 24333
	public Font m_BelweOutlineFont;

	// Token: 0x04005F0E RID: 24334
	public Font m_FranklinGothicFont;

	// Token: 0x04005F0F RID: 24335
	public Font m_BlizzardGlobal;

	// Token: 0x04005F10 RID: 24336
	private bool m_Active;

	// Token: 0x04005F11 RID: 24337
	private List<Font> m_Fonts;

	// Token: 0x04005F12 RID: 24338
	private Locale m_Locale;

	// Token: 0x04005F13 RID: 24339
	private string m_AtlasCharacters;

	// Token: 0x04005F14 RID: 24340
	private string m_AtlasNumbers = "0123456789.";

	// Token: 0x04005F15 RID: 24341
	private static UberTextMgr s_Instance;
}
