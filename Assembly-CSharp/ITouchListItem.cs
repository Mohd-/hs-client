using System;
using UnityEngine;

// Token: 0x02000573 RID: 1395
public interface ITouchListItem
{
	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x06003FD0 RID: 16336
	Bounds LocalBounds { get; }

	// Token: 0x17000489 RID: 1161
	// (get) Token: 0x06003FD1 RID: 16337
	bool IsHeader { get; }

	// Token: 0x1700048A RID: 1162
	// (get) Token: 0x06003FD2 RID: 16338
	// (set) Token: 0x06003FD3 RID: 16339
	bool Visible { get; set; }

	// Token: 0x1700048B RID: 1163
	// (get) Token: 0x06003FD4 RID: 16340
	GameObject gameObject { get; }

	// Token: 0x1700048C RID: 1164
	// (get) Token: 0x06003FD5 RID: 16341
	Transform transform { get; }

	// Token: 0x06003FD6 RID: 16342
	T GetComponent<T>() where T : MonoBehaviour;
}
