using System;

namespace Facebook
{
	// Token: 0x02000B2B RID: 2859
	public class IOSFacebookLoader : FB.CompiledFacebookLoader
	{
		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x060061B9 RID: 25017 RVA: 0x001D1F47 File Offset: 0x001D0147
		protected override IFacebook fb
		{
			get
			{
				return FBComponentFactory.GetComponent<IOSFacebook>(0);
			}
		}
	}
}
