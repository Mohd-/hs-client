using System;

namespace Facebook
{
	// Token: 0x02000B0F RID: 2831
	public class AndroidFacebookLoader : FB.CompiledFacebookLoader
	{
		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x060060EC RID: 24812 RVA: 0x001D022C File Offset: 0x001CE42C
		protected override IFacebook fb
		{
			get
			{
				return FBComponentFactory.GetComponent<AndroidFacebook>(0);
			}
		}
	}
}
