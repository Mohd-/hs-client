using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DEA RID: 3562
	[ActionCategory("Pegasus")]
	[Tooltip("INTERNAL USE ONLY. Do not put this on your FSMs.")]
	public abstract class SpellCardIdAudioAction : SpellAction
	{
		// Token: 0x06006DC4 RID: 28100 RVA: 0x00203F3C File Offset: 0x0020213C
		protected AudioSource GetAudioSource(FsmOwnerDefault ownerDefault)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(ownerDefault);
			if (ownerDefaultTarget == null)
			{
				return null;
			}
			return ownerDefaultTarget.GetComponent<AudioSource>();
		}

		// Token: 0x06006DC5 RID: 28101 RVA: 0x00203F6C File Offset: 0x0020216C
		protected AudioClip GetClipMatchingCardId(SpellAction.Which whichCard, string[] cardIds, AudioClip[] clips, AudioClip defaultClip)
		{
			Card card = base.GetCard(whichCard);
			if (card == null)
			{
				Debug.LogWarning(string.Format("SpellCardIdAudioAction.GetClipMatchingCardId() - could not find {0} card", whichCard));
				return null;
			}
			string cardId = card.GetEntity().GetCardId();
			int indexMatchingCardId = base.GetIndexMatchingCardId(cardId, cardIds);
			if (indexMatchingCardId < 0)
			{
				return defaultClip;
			}
			return clips[indexMatchingCardId];
		}
	}
}
