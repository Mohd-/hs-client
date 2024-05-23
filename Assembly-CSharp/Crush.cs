using System;
using UnityEngine;

// Token: 0x02000E51 RID: 3665
public class Crush : Spell
{
	// Token: 0x06006F56 RID: 28502 RVA: 0x0020AE74 File Offset: 0x00209074
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		Card sourceCard = base.GetSourceCard();
		Entity entity = sourceCard.GetEntity();
		Actor actor = SceneUtils.FindComponentInParents<Actor>(this);
		GameObject gameObject = this.m_minionPieces.m_main;
		bool flag = entity.HasTag(GAME_TAG.PREMIUM);
		if (flag)
		{
			gameObject = this.m_minionPieces.m_premium;
			SceneUtils.EnableRenderers(this.m_minionPieces.m_main, false);
		}
		GameObject portraitMesh = actor.GetPortraitMesh();
		gameObject.GetComponent<Renderer>().material = portraitMesh.GetComponent<Renderer>().sharedMaterial;
		gameObject.SetActive(true);
		SceneUtils.EnableRenderers(gameObject, true);
		if (entity.HasTaunt())
		{
			if (flag)
			{
				this.m_minionPieces.m_taunt.GetComponent<Renderer>().material = this.m_premiumTauntMaterial;
			}
			this.m_minionPieces.m_taunt.SetActive(true);
			SceneUtils.EnableRenderers(this.m_minionPieces.m_taunt, true);
		}
		if (entity.IsElite())
		{
			if (flag)
			{
				this.m_minionPieces.m_legendary.GetComponent<Renderer>().material = this.m_premiumEliteMaterial;
			}
			this.m_minionPieces.m_legendary.SetActive(true);
			SceneUtils.EnableRenderers(this.m_minionPieces.m_legendary, true);
		}
		this.m_attack.SetGameStringText(entity.GetATK().ToString());
		this.m_health.SetGameStringText(entity.GetHealth().ToString());
	}

	// Token: 0x04005882 RID: 22658
	public MinionPieces m_minionPieces;

	// Token: 0x04005883 RID: 22659
	public Material m_premiumTauntMaterial;

	// Token: 0x04005884 RID: 22660
	public Material m_premiumEliteMaterial;

	// Token: 0x04005885 RID: 22661
	public UberText m_attack;

	// Token: 0x04005886 RID: 22662
	public UberText m_health;
}
