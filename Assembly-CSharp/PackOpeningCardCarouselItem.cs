using System;
using UnityEngine;

// Token: 0x02000A92 RID: 2706
public class PackOpeningCardCarouselItem : CarouselItem
{
	// Token: 0x06005E38 RID: 24120 RVA: 0x001C3ED9 File Offset: 0x001C20D9
	public PackOpeningCardCarouselItem(PackOpeningCard card)
	{
		this.m_card = card;
	}

	// Token: 0x06005E39 RID: 24121 RVA: 0x001C3EE8 File Offset: 0x001C20E8
	public override void Show(Carousel card)
	{
	}

	// Token: 0x06005E3A RID: 24122 RVA: 0x001C3EEA File Offset: 0x001C20EA
	public override void Hide()
	{
	}

	// Token: 0x06005E3B RID: 24123 RVA: 0x001C3EEC File Offset: 0x001C20EC
	public override void Clear()
	{
		this.m_card = null;
	}

	// Token: 0x06005E3C RID: 24124 RVA: 0x001C3EF5 File Offset: 0x001C20F5
	public override GameObject GetGameObject()
	{
		return this.m_card.gameObject;
	}

	// Token: 0x06005E3D RID: 24125 RVA: 0x001C3F02 File Offset: 0x001C2102
	public override bool IsLoaded()
	{
		return true;
	}

	// Token: 0x040045DC RID: 17884
	private PackOpeningCard m_card;
}
