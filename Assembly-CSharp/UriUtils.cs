using System;
using System.Net;

// Token: 0x02000EEB RID: 3819
public class UriUtils
{
	// Token: 0x06007252 RID: 29266 RVA: 0x00219854 File Offset: 0x00217A54
	public static bool GetHostAddressAsIp(string hostName, out string address)
	{
		address = string.Empty;
		IPAddress ipaddress;
		if (IPAddress.TryParse(hostName, ref ipaddress))
		{
			address = ipaddress.ToString();
			return true;
		}
		return false;
	}

	// Token: 0x06007253 RID: 29267 RVA: 0x00219880 File Offset: 0x00217A80
	public static bool GetHostAddressByDns(string hostName, out string address)
	{
		address = string.Empty;
		try
		{
			IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
			IPAddress[] addressList = hostEntry.AddressList;
			int num = 0;
			if (num < addressList.Length)
			{
				IPAddress ipaddress = addressList[num];
				address = ipaddress.ToString();
				return true;
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
		address = hostName;
		return false;
	}

	// Token: 0x06007254 RID: 29268 RVA: 0x002198F0 File Offset: 0x00217AF0
	public static bool GetHostAddress(string hostName, out string address)
	{
		if (UriUtils.GetHostAddressAsIp(hostName, out address))
		{
			return true;
		}
		try
		{
			if (UriUtils.GetHostAddressByDns(hostName, out address))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
		return false;
	}
}
