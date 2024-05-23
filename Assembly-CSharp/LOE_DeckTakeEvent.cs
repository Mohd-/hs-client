using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000A03 RID: 2563
[CustomEditClass]
public class LOE_DeckTakeEvent : MonoBehaviour
{
	// Token: 0x06005B11 RID: 23313 RVA: 0x001B2B2A File Offset: 0x001B0D2A
	private void Start()
	{
		CardBackManager.Get().SetCardBackTexture(this.m_friendlyDeckRenderer, 0, true);
	}

	// Token: 0x06005B12 RID: 23314 RVA: 0x001B2B40 File Offset: 0x001B0D40
	public IEnumerator PlayTakeDeckAnim()
	{
		this.m_animIsPlaying = true;
		this.m_takeDeckAnimator.enabled = true;
		this.m_takeDeckAnimator.Play(this.m_takeDeckAnimName);
		if (!string.IsNullOrEmpty(this.m_takeDeckSoundPrefab))
		{
			string takeDeckSoundName = FileUtils.GameAssetPathToName(this.m_takeDeckSoundPrefab);
			SoundManager.Get().LoadAndPlay(takeDeckSoundName);
		}
		yield return new WaitForEndOfFrame();
		float animDuration = this.m_takeDeckAnimator.GetCurrentAnimatorStateInfo(0).length;
		Log.JMac.Print("Take Deck anim duration: " + animDuration, new object[0]);
		yield return new WaitForSeconds(animDuration);
		this.m_animIsPlaying = false;
		yield break;
	}

	// Token: 0x06005B13 RID: 23315 RVA: 0x001B2B5C File Offset: 0x001B0D5C
	public IEnumerator PlayReplacementDeckAnim()
	{
		this.m_animIsPlaying = true;
		this.m_replacementDeckAnimator.enabled = true;
		this.m_replacementDeckAnimator.Play(this.m_replacementDeckAnimName);
		if (!string.IsNullOrEmpty(this.m_replacementDeckSoundPrefab))
		{
			string replacementDeckSoundName = FileUtils.GameAssetPathToName(this.m_replacementDeckSoundPrefab);
			SoundManager.Get().LoadAndPlay(replacementDeckSoundName);
		}
		yield return new WaitForEndOfFrame();
		float animDuration = this.m_replacementDeckAnimator.GetCurrentAnimatorStateInfo(0).length;
		Log.JMac.Print("Take Deck anim duration: " + animDuration, new object[0]);
		yield return new WaitForSeconds(animDuration);
		this.m_animIsPlaying = false;
		yield break;
	}

	// Token: 0x06005B14 RID: 23316 RVA: 0x001B2B77 File Offset: 0x001B0D77
	public bool AnimIsPlaying()
	{
		return this.m_animIsPlaying;
	}

	// Token: 0x04004298 RID: 17048
	public Renderer m_friendlyDeckRenderer;

	// Token: 0x04004299 RID: 17049
	public Animator m_takeDeckAnimator;

	// Token: 0x0400429A RID: 17050
	public string m_takeDeckAnimName = "LOE_TakeDeck";

	// Token: 0x0400429B RID: 17051
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_takeDeckSoundPrefab;

	// Token: 0x0400429C RID: 17052
	public Animator m_replacementDeckAnimator;

	// Token: 0x0400429D RID: 17053
	public string m_replacementDeckAnimName = "CardsToPlayerDeck";

	// Token: 0x0400429E RID: 17054
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_replacementDeckSoundPrefab;

	// Token: 0x0400429F RID: 17055
	private bool m_animIsPlaying;
}
