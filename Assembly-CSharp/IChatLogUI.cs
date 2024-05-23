using System;
using UnityEngine;

// Token: 0x02000516 RID: 1302
public interface IChatLogUI
{
	// Token: 0x17000441 RID: 1089
	// (get) Token: 0x06003C05 RID: 15365
	bool IsShowing { get; }

	// Token: 0x17000442 RID: 1090
	// (get) Token: 0x06003C06 RID: 15366
	GameObject GameObject { get; }

	// Token: 0x17000443 RID: 1091
	// (get) Token: 0x06003C07 RID: 15367
	BnetPlayer Receiver { get; }

	// Token: 0x06003C08 RID: 15368
	void ShowForPlayer(BnetPlayer player);

	// Token: 0x06003C09 RID: 15369
	void Hide();

	// Token: 0x06003C0A RID: 15370
	void GoBack();
}
