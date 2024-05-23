using System;
using System.Text;
using UnityEngine;

namespace MATSDK
{
	// Token: 0x02000B31 RID: 2865
	public class MATDelegate : MonoBehaviour
	{
		// Token: 0x060061FF RID: 25087 RVA: 0x001D246C File Offset: 0x001D066C
		public void trackerDidSucceed(string data)
		{
		}

		// Token: 0x06006200 RID: 25088 RVA: 0x001D246E File Offset: 0x001D066E
		public void trackerDidFail(string error)
		{
			MonoBehaviour.print("MATDelegate trackerDidFail: " + error);
		}

		// Token: 0x06006201 RID: 25089 RVA: 0x001D2480 File Offset: 0x001D0680
		public void trackerDidEnqueueRequest(string refId)
		{
			MonoBehaviour.print("MATDelegate trackerDidEnqueueRequest: " + refId);
		}

		// Token: 0x06006202 RID: 25090 RVA: 0x001D2492 File Offset: 0x001D0692
		public void trackerDidReceiveDeepLink(string url)
		{
			MonoBehaviour.print("MATDelegate trackerDidReceiveDeepLink: " + url);
		}

		// Token: 0x06006203 RID: 25091 RVA: 0x001D24A4 File Offset: 0x001D06A4
		public static string DecodeFrom64(string encodedString)
		{
			MonoBehaviour.print("MATDelegate.DecodeFrom64(string)");
			return Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
		}
	}
}
