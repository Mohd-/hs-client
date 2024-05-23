using System;
using UnityEngine;

// Token: 0x02000F0C RID: 3852
public class DrawWireframe : MonoBehaviour
{
	// Token: 0x0600730F RID: 29455 RVA: 0x0021DDDF File Offset: 0x0021BFDF
	private void OnPreRender()
	{
		GL.wireframe = true;
	}

	// Token: 0x06007310 RID: 29456 RVA: 0x0021DDE7 File Offset: 0x0021BFE7
	private void OnPostRender()
	{
		GL.wireframe = false;
	}
}
