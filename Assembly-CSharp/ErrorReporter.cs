using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Xml;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class ErrorReporter : MonoBehaviour
{
	// Token: 0x06000DA4 RID: 3492 RVA: 0x00037650 File Offset: 0x00035850
	public void send(string message, string stackTrace)
	{
		string text = ErrorReporter.createHash(message + stackTrace);
		if (ErrorReporter.alreadySent(text))
		{
			return;
		}
		ErrorReporter.sentReports_.Add(text);
		try
		{
			string text2 = ErrorReporter.buildMarkup(message, stackTrace, text);
			byte[] bytes = Encoding.UTF8.GetBytes(text2);
			WWWForm wwwform = new WWWForm();
			wwwform.AddBinaryData("file", bytes, "ReportedIssue.xml", "application/octet-stream");
			WWW www = new WWW(this.submitURL, wwwform);
			this.sendCount_++;
			base.StartCoroutine(this.wait(www));
		}
		catch (SecurityException ex)
		{
			this.unregister();
			Blizzard.Log.Error("Unable to send error report (security): " + ex.Message);
		}
		catch (Exception ex2)
		{
			this.unregister();
			Blizzard.Log.Error("Unable to send error report (unknown): " + ex2.Message);
		}
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x00037748 File Offset: 0x00035948
	public static ErrorReporter Get()
	{
		return ErrorReporter.instance_;
	}

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x06000DA6 RID: 3494 RVA: 0x0003774F File Offset: 0x0003594F
	public bool busy
	{
		get
		{
			return this.sendCount_ > 0;
		}
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x0003775A File Offset: 0x0003595A
	private void Awake()
	{
		ErrorReporter.instance_ = this;
		this.register();
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x00037768 File Offset: 0x00035968
	private void OnEnable()
	{
		this.register();
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x00037770 File Offset: 0x00035970
	private void OnDisable()
	{
		this.unregister();
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x00037778 File Offset: 0x00035978
	private void OnApplicationQuit()
	{
		this.unregister();
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x00037780 File Offset: 0x00035980
	public void register()
	{
		Application.logMessageReceived += new Application.LogCallback(this.callback);
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x00037793 File Offset: 0x00035993
	public void unregister()
	{
		Application.logMessageReceived -= new Application.LogCallback(this.callback);
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x000377A8 File Offset: 0x000359A8
	private void callback(string message, string stackTrace, LogType logType)
	{
		switch (logType)
		{
		case 0:
			if (Vars.Key("Application.SendErrors").GetBool(false))
			{
				this.send(message, stackTrace);
			}
			break;
		case 1:
			if (Vars.Key("Application.SendAsserts").GetBool(false))
			{
				this.send(message, stackTrace);
			}
			break;
		case 4:
			if (Vars.Key("Application.SendExceptions").GetBool(true))
			{
				this.send(message, stackTrace);
			}
			this.reportUnhandledException(message, stackTrace);
			break;
		}
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x00037844 File Offset: 0x00035A44
	private static string buildMarkup(string title, string stackTrace, string hashBlock)
	{
		string text = ErrorReporter.createEscapedSGML(stackTrace);
		return string.Concat(new object[]
		{
			"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<ReportedIssue xmlns=\"http://schemas.datacontract.org/2004/07/Inspector.Models\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">\n\t<Summary>",
			title,
			"</Summary>\n\t<Assertion>",
			text,
			"</Assertion>\n\t<HashBlock>",
			hashBlock,
			"</HashBlock>\n\t<BuildNumber>",
			12574,
			"</BuildNumber>\n\t<Module>Hearthstone Client</Module>\n\t<EnteredBy>0</EnteredBy>\n\t<IssueType>Exception</IssueType>\n\t<ProjectId>",
			70,
			"</ProjectId>\n\t<Metadata><NameValuePairs>\n\t\t<NameValuePair><Name>Build</Name><Value>",
			12574,
			"</Value></NameValuePair>\n\t\t<NameValuePair><Name>OS.Platform</Name><Value>",
			Application.platform,
			"</Value></NameValuePair>\n\t\t<NameValuePair><Name>Unity.Version</Name><Value>",
			Application.unityVersion,
			"</Value></NameValuePair>\n\t\t<NameValuePair><Name>Unity.Genuine</Name><Value>",
			Application.genuine,
			"</Value></NameValuePair>\n\t\t<NameValuePair><Name>Locale</Name><Value>",
			Localization.GetLocaleName(),
			"</Value></NameValuePair>\n\t</NameValuePairs></Metadata>\n</ReportedIssue>\n"
		});
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x00037924 File Offset: 0x00035B24
	private static string createHash(string blob)
	{
		return Blizzard.Crypto.SHA1.Calc(blob);
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x0003792C File Offset: 0x00035B2C
	private static string createEscapedSGML(string blob)
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlElement xmlElement = xmlDocument.CreateElement("root");
		xmlElement.InnerText = blob;
		return xmlElement.InnerXml;
	}

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x06000DB1 RID: 3505 RVA: 0x00037958 File Offset: 0x00035B58
	private IPAddress ipAddress
	{
		get
		{
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ipaddress in hostEntry.AddressList)
				{
					if (ipaddress.AddressFamily == 2)
					{
						return ipaddress;
					}
				}
			}
			catch (SocketException)
			{
			}
			catch (ArgumentException)
			{
			}
			return this.unknownAddress_;
		}
	}

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x06000DB2 RID: 3506 RVA: 0x000379DC File Offset: 0x00035BDC
	private static string localTime
	{
		get
		{
			return DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("en-US"));
		}
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x00037A08 File Offset: 0x00035C08
	private IEnumerator wait(WWW www)
	{
		yield return www;
		this.sendCount_--;
		yield break;
	}

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x06000DB4 RID: 3508 RVA: 0x00037A31 File Offset: 0x00035C31
	private string submitURL
	{
		get
		{
			return "http://iir.blizzard.com:3724/submit/" + 70;
		}
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x00037A44 File Offset: 0x00035C44
	private void reportUnhandledException(string message, string stackTrace)
	{
		string text = ErrorReporter.createHash(message + stackTrace);
		if (this.m_previousExceptions.Contains(text))
		{
			return;
		}
		this.m_previousExceptions.Add(text);
		Error.AddDevFatal("Uncaught Exception!\n{0}\nAt:\n{1}", new object[]
		{
			message,
			stackTrace
		});
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x00037A94 File Offset: 0x00035C94
	private static bool alreadySent(string hash)
	{
		return ErrorReporter.sentReports_.Contains(hash);
	}

	// Token: 0x0400073F RID: 1855
	private const int hearthstoneProjectID_ = 70;

	// Token: 0x04000740 RID: 1856
	private List<string> m_previousExceptions = new List<string>();

	// Token: 0x04000741 RID: 1857
	private static ErrorReporter instance_;

	// Token: 0x04000742 RID: 1858
	private int sendCount_;

	// Token: 0x04000743 RID: 1859
	private IPAddress unknownAddress_ = new IPAddress(new byte[4]);

	// Token: 0x04000744 RID: 1860
	private static readonly HashSet<string> sentReports_ = new HashSet<string>();
}
