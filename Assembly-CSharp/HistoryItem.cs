using System;
using UnityEngine;

// Token: 0x02000342 RID: 834
public class HistoryItem : MonoBehaviour
{
	// Token: 0x06002BA8 RID: 11176 RVA: 0x000D953A File Offset: 0x000D773A
	public Entity GetEntity()
	{
		return this.m_entity;
	}

	// Token: 0x06002BA9 RID: 11177 RVA: 0x000D9542 File Offset: 0x000D7742
	public Texture GetPortraitTexture()
	{
		return this.m_portraitTexture;
	}

	// Token: 0x06002BAA RID: 11178 RVA: 0x000D954A File Offset: 0x000D774A
	public Material GetPortraitGoldenMaterial()
	{
		return this.m_portraitGoldenMaterial;
	}

	// Token: 0x06002BAB RID: 11179 RVA: 0x000D9552 File Offset: 0x000D7752
	public bool IsMainCardActorInitialized()
	{
		return this.m_mainCardActorInitialized;
	}

	// Token: 0x06002BAC RID: 11180 RVA: 0x000D955A File Offset: 0x000D775A
	public void InitializeMainCardActor()
	{
		if (this.m_mainCardActorInitialized)
		{
			return;
		}
		this.m_mainCardActor.TurnOffCollider();
		this.m_mainCardActor.SetActorState(ActorStateType.CARD_HISTORY);
		this.m_mainCardActorInitialized = true;
	}

	// Token: 0x06002BAD RID: 11181 RVA: 0x000D9588 File Offset: 0x000D7788
	public void DisplaySpells()
	{
		if (this.m_fatigue)
		{
			return;
		}
		if (!this.m_entity.IsCharacter() && !this.m_entity.IsWeapon())
		{
			return;
		}
		int remainingHealth = this.m_entity.GetRemainingHealth();
		if (this.m_dead || this.m_splatAmount >= remainingHealth)
		{
			this.DisplaySkullOnActor(this.m_mainCardActor);
		}
		else if (this.m_splatAmount != 0)
		{
			this.DisplaySplatOnActor(this.m_mainCardActor, this.m_splatAmount);
		}
	}

	// Token: 0x06002BAE RID: 11182 RVA: 0x000D9614 File Offset: 0x000D7814
	private void DisplaySplatOnActor(Actor actor, int damage)
	{
		Spell spell = actor.GetSpell(SpellType.DAMAGE);
		if (spell == null)
		{
			return;
		}
		DamageSplatSpell damageSplatSpell = (DamageSplatSpell)spell;
		damageSplatSpell.SetDamage(damage);
		damageSplatSpell.ActivateState(SpellStateType.IDLE);
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x000D964C File Offset: 0x000D784C
	private void DisplaySkullOnActor(Actor actor)
	{
		Spell spell = actor.GetSpell(SpellType.SKULL);
		if (spell == null)
		{
			return;
		}
		spell.ActivateState(SpellStateType.IDLE);
	}

	// Token: 0x04001A6D RID: 6765
	public Actor m_mainCardActor;

	// Token: 0x04001A6E RID: 6766
	protected bool m_dead;

	// Token: 0x04001A6F RID: 6767
	protected int m_splatAmount;

	// Token: 0x04001A70 RID: 6768
	protected Entity m_entity;

	// Token: 0x04001A71 RID: 6769
	protected Texture m_portraitTexture;

	// Token: 0x04001A72 RID: 6770
	protected Material m_portraitGoldenMaterial;

	// Token: 0x04001A73 RID: 6771
	protected bool m_mainCardActorInitialized;

	// Token: 0x04001A74 RID: 6772
	protected bool m_fatigue;
}
