using System;
using UnityEngine;

// Token: 0x020001EF RID: 495
public class ShaderUtils
{
	// Token: 0x06001DC3 RID: 7619 RVA: 0x0008AB9B File Offset: 0x00088D9B
	public static Shader FindShader(string name)
	{
		return ShaderPreCompiler.GetShader(name);
	}
}
