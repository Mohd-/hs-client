using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E79 RID: 3705
public class NefarianSwapSpell : HeroSwapSpell
{
	// Token: 0x06007059 RID: 28761 RVA: 0x00210A94 File Offset: 0x0020EC94
	public override bool AddPowerTargets()
	{
		if (!base.AddPowerTargets())
		{
			return false;
		}
		int tag = this.m_oldHeroCard.GetEntity().GetTag(GAME_TAG.LINKED_ENTITY);
		if (tag != 0)
		{
			this.m_obsoleteHeroCard = GameState.Get().GetEntity(tag).GetCard();
		}
		return !(this.m_obsoleteHeroCard == null);
	}

	// Token: 0x0600705A RID: 28762 RVA: 0x00210AF4 File Offset: 0x0020ECF4
	public override void CustomizeFXProcess(Actor heroActor)
	{
		if (heroActor == this.m_newHeroCard.GetActor())
		{
			base.StartCoroutine(this.DestroyObsolete());
		}
	}

	// Token: 0x0600705B RID: 28763 RVA: 0x00210B1C File Offset: 0x0020ED1C
	private IEnumerator DestroyObsolete()
	{
		yield return new WaitForSeconds(this.m_obsoleteRemovalDelay);
		Object.Destroy(this.m_obsoleteHeroCard.GetActor().gameObject);
		yield break;
	}

	// Token: 0x04005977 RID: 22903
	public float m_obsoleteRemovalDelay;

	// Token: 0x04005978 RID: 22904
	private Card m_obsoleteHeroCard;
}
