using System;
using UnityEngine;

// Token: 0x02000A93 RID: 2707
public abstract class CarouselItem
{
	// Token: 0x06005E3F RID: 24127
	public abstract void Show(Carousel parent);

	// Token: 0x06005E40 RID: 24128
	public abstract void Hide();

	// Token: 0x06005E41 RID: 24129
	public abstract void Clear();

	// Token: 0x06005E42 RID: 24130
	public abstract GameObject GetGameObject();

	// Token: 0x06005E43 RID: 24131
	public abstract bool IsLoaded();
}
