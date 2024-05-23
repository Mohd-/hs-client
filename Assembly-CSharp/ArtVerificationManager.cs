using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F71 RID: 3953
public class ArtVerificationManager : MonoBehaviour
{
	// Token: 0x06007532 RID: 30002 RVA: 0x002296BF File Offset: 0x002278BF
	private void Start()
	{
		base.StartCoroutine(this.StartVerification());
	}

	// Token: 0x06007533 RID: 30003 RVA: 0x002296D0 File Offset: 0x002278D0
	private IEnumerator StartVerification()
	{
		Debug.Log("Preparing to verify art.");
		yield return new WaitForSeconds(1f);
		Debug.Log("Starting art verification now. This may take a few minutes.");
		yield return new WaitForSeconds(0.5f);
		this.LoadCards();
		yield break;
	}

	// Token: 0x06007534 RID: 30004 RVA: 0x002296EC File Offset: 0x002278EC
	private void LoadCards()
	{
		GameDbf.Load();
		List<string> allCardIds = GameUtils.GetAllCardIds();
		this.m_cardsToLoad = allCardIds.Count;
		foreach (string cardId in allCardIds)
		{
			CardPortraitQuality quality = new CardPortraitQuality(3, TAG_PREMIUM.GOLDEN);
			DefLoader.Get().LoadCardDef(cardId, new DefLoader.LoadDefCallback<CardDef>(this.OnCardDefLoaded), null, quality);
		}
	}

	// Token: 0x06007535 RID: 30005 RVA: 0x00229774 File Offset: 0x00227974
	private void OnCardDefLoaded(string cardID, CardDef def, object userData)
	{
		this.m_cardsToLoad--;
		this.CleanUpCard(def);
		if (this.m_cardsToLoad > 0)
		{
			return;
		}
		this.FinishVerification();
	}

	// Token: 0x06007536 RID: 30006 RVA: 0x002297A9 File Offset: 0x002279A9
	private void CleanUpCard(CardDef def)
	{
		if (!def)
		{
			return;
		}
		Object.Destroy(def);
	}

	// Token: 0x06007537 RID: 30007 RVA: 0x002297BD File Offset: 0x002279BD
	private void FinishVerification()
	{
		Debug.Log("Finished");
		GeneralUtils.ExitApplication();
	}

	// Token: 0x04005FB2 RID: 24498
	private const float START_DELAY_SEC = 1f;

	// Token: 0x04005FB3 RID: 24499
	private int m_cardsToLoad;
}
