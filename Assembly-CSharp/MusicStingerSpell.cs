using System;

// Token: 0x02000E16 RID: 3606
public class MusicStingerSpell : CardSoundSpell
{
	// Token: 0x06006E7F RID: 28287 RVA: 0x00206ACC File Offset: 0x00204CCC
	private bool CanPlay()
	{
		if (GameState.Get() == null)
		{
			return true;
		}
		Card sourceCard = base.GetSourceCard();
		Player controller = sourceCard.GetController();
		return controller == null || controller.IsLocalUser();
	}

	// Token: 0x0400570C RID: 22284
	public MusicStingerData m_MusicStingerData = new MusicStingerData();
}
