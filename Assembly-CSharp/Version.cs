using System;

// Token: 0x02000EC9 RID: 3785
public abstract class Version
{
	// Token: 0x0600719F RID: 29087 RVA: 0x002168B2 File Offset: 0x00214AB2
	public static void Reset()
	{
		Version.report_ = string.Empty;
	}

	// Token: 0x170009D5 RID: 2517
	// (get) Token: 0x060071A0 RID: 29088 RVA: 0x002168BE File Offset: 0x00214ABE
	public static string FullReport
	{
		get
		{
			if (string.IsNullOrEmpty(Version.report_))
			{
				Version.createReport();
			}
			return Version.report_;
		}
	}

	// Token: 0x060071A1 RID: 29089 RVA: 0x002168DC File Offset: 0x00214ADC
	private static void createReport()
	{
		Version.report_ = string.Format("Version {0} (client {1}{2}{3})", new object[]
		{
			12574,
			203139,
			Version.serverChangelist,
			Version.bobNetAddress
		});
	}

	// Token: 0x170009D6 RID: 2518
	// (get) Token: 0x060071A2 RID: 29090 RVA: 0x00216928 File Offset: 0x00214B28
	// (set) Token: 0x060071A3 RID: 29091 RVA: 0x0021693B File Offset: 0x00214B3B
	public static string serverChangelist
	{
		get
		{
			return Version.serverChangelist_ ?? string.Empty;
		}
		set
		{
			Version.serverChangelist_ = string.Format(", server {0}", value);
			Version.Reset();
		}
	}

	// Token: 0x170009D7 RID: 2519
	// (get) Token: 0x060071A4 RID: 29092 RVA: 0x00216952 File Offset: 0x00214B52
	// (set) Token: 0x060071A5 RID: 29093 RVA: 0x00216965 File Offset: 0x00214B65
	public static string bobNetAddress
	{
		get
		{
			return Version.bobNetAddress_ ?? string.Empty;
		}
		set
		{
			Version.bobNetAddress_ = string.Format(", Battle.net {0}", value);
			Version.Reset();
		}
	}

	// Token: 0x04005BD5 RID: 23509
	public const int version = 12574;

	// Token: 0x04005BD6 RID: 23510
	public const int clientChangelist = 203139;

	// Token: 0x04005BD7 RID: 23511
	public const string androidTextureCompression = "";

	// Token: 0x04005BD8 RID: 23512
	public const string cosmeticVersion = "5.0";

	// Token: 0x04005BD9 RID: 23513
	private static int clientChangelist_;

	// Token: 0x04005BDA RID: 23514
	private static string serverChangelist_;

	// Token: 0x04005BDB RID: 23515
	private static string bobNetAddress_;

	// Token: 0x04005BDC RID: 23516
	private static string report_;
}
