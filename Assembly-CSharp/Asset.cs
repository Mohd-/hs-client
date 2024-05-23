using System;

// Token: 0x020002CA RID: 714
public class Asset
{
	// Token: 0x060025FE RID: 9726 RVA: 0x000B98B0 File Offset: 0x000B7AB0
	public Asset(string name, AssetFamily family, bool persistent = false)
	{
		this.m_name = name;
		this.m_family = family;
		this.m_persistent = persistent;
		this.m_path = string.Format(AssetPathInfo.FamilyInfo[family].format, name);
	}

	// Token: 0x060025FF RID: 9727 RVA: 0x000B98E9 File Offset: 0x000B7AE9
	public string GetName()
	{
		return this.m_name;
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x000B98F1 File Offset: 0x000B7AF1
	public AssetFamily GetFamily()
	{
		return this.m_family;
	}

	// Token: 0x06002601 RID: 9729 RVA: 0x000B98F9 File Offset: 0x000B7AF9
	public bool IsPersistent()
	{
		return this.m_persistent;
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x000B9901 File Offset: 0x000B7B01
	public string GetDirectory()
	{
		return AssetPathInfo.FamilyInfo[this.m_family].sourceDir;
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x000B9918 File Offset: 0x000B7B18
	public string[] GetExtensions()
	{
		return AssetPathInfo.FamilyInfo[this.m_family].exts;
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x000B992F File Offset: 0x000B7B2F
	public string GetPath()
	{
		return this.m_path;
	}

	// Token: 0x06002605 RID: 9733 RVA: 0x000B9937 File Offset: 0x000B7B37
	public string GetPath(Locale locale)
	{
		return FileUtils.MakeLocalizedPathFromSourcePath(locale, this.m_path);
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x000B9945 File Offset: 0x000B7B45
	public Locale GetLocale()
	{
		return this.m_locale;
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x000B994D File Offset: 0x000B7B4D
	public void SetLocale(Locale locale)
	{
		this.m_locale = locale;
	}

	// Token: 0x04001676 RID: 5750
	private string m_name;

	// Token: 0x04001677 RID: 5751
	private AssetFamily m_family;

	// Token: 0x04001678 RID: 5752
	private bool m_persistent;

	// Token: 0x04001679 RID: 5753
	private string m_path;

	// Token: 0x0400167A RID: 5754
	private Locale m_locale;
}
