using System;

// Token: 0x02000EDB RID: 3803
public static class CustomEditTypeUtils
{
	// Token: 0x06007206 RID: 29190 RVA: 0x00218860 File Offset: 0x00216A60
	public static string GetExtension(EditType type)
	{
		switch (type)
		{
		case EditType.MATERIAL:
			return "mat";
		case EditType.TEXTURE:
		case EditType.CARD_TEXTURE:
			return "psd";
		default:
			return "prefab";
		}
	}
}
