using System;
using UnityEngine;

// Token: 0x02000D82 RID: 3458
public class ArcEndSphere : MonoBehaviour
{
	// Token: 0x06006C23 RID: 27683 RVA: 0x001FC63E File Offset: 0x001FA83E
	private void Start()
	{
	}

	// Token: 0x06006C24 RID: 27684 RVA: 0x001FC640 File Offset: 0x001FA840
	private void Update()
	{
		Vector2 mainTextureOffset = base.GetComponent<Renderer>().material.mainTextureOffset;
		mainTextureOffset.x += Time.deltaTime * 1f;
		base.GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset;
	}
}
