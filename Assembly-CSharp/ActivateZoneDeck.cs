using System;
using UnityEngine;

// Token: 0x020009DE RID: 2526
public class ActivateZoneDeck : MonoBehaviour
{
	// Token: 0x06005A3C RID: 23100 RVA: 0x001AEB8C File Offset: 0x001ACD8C
	public void ToggleActive()
	{
		if (GameState.Get() == null || GameState.Get().GetFriendlySidePlayer() == null || GameState.Get().GetOpposingSidePlayer() == null)
		{
			Debug.LogError("ActivateZoneDeck - Game State not yet initialized.");
			return;
		}
		ZoneDeck deckZone;
		if (this.m_friendlyDeck)
		{
			deckZone = GameState.Get().GetFriendlySidePlayer().GetDeckZone();
		}
		else
		{
			deckZone = GameState.Get().GetOpposingSidePlayer().GetDeckZone();
		}
		if (deckZone == null)
		{
			Debug.LogError("ActivateZoneDeck - zoneDeck is null!");
			return;
		}
		deckZone.SetVisibility(this.onoff);
	}

	// Token: 0x040041CB RID: 16843
	public bool m_friendlyDeck;

	// Token: 0x040041CC RID: 16844
	private bool onoff = true;
}
