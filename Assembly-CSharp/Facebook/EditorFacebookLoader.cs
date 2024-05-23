using System;

namespace Facebook
{
	// Token: 0x02000B22 RID: 2850
	public class EditorFacebookLoader : FB.CompiledFacebookLoader
	{
		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x06006186 RID: 24966 RVA: 0x001D17D9 File Offset: 0x001CF9D9
		protected override IFacebook fb
		{
			get
			{
				return FBComponentFactory.GetComponent<EditorFacebook>(0);
			}
		}
	}
}
