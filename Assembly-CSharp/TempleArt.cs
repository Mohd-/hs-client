using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006A7 RID: 1703
public class TempleArt : MonoBehaviour
{
	// Token: 0x06004779 RID: 18297 RVA: 0x00157162 File Offset: 0x00155362
	public void DoPortraitSwap(Actor actor, int turn)
	{
		base.StartCoroutine(this.DoPortraitSwapWithTiming(actor, turn));
	}

	// Token: 0x0600477A RID: 18298 RVA: 0x00157174 File Offset: 0x00155374
	private IEnumerator DoPortraitSwapWithTiming(Actor actor, int turn)
	{
		if (actor == null)
		{
			yield break;
		}
		if (this.m_portraitSwapSpell != null)
		{
			Spell spellInstance = Object.Instantiate<Spell>(this.m_portraitSwapSpell);
			spellInstance.transform.parent = actor.transform;
			spellInstance.AddStateFinishedCallback(delegate(Spell spell, SpellStateType prevStateType, object userData)
			{
				if (spell.GetActiveState() == SpellStateType.NONE)
				{
					Object.Destroy(spell.gameObject);
				}
			});
			spellInstance.SetSource(actor.gameObject);
			spellInstance.Activate();
			yield return new WaitForSeconds(this.m_portraitSwapDelay);
		}
		actor.SetPortraitTextureOverride(this.GetArtForTurn(turn));
		yield break;
	}

	// Token: 0x0600477B RID: 18299 RVA: 0x001571AB File Offset: 0x001553AB
	private Texture2D GetArtForTurn(int turn)
	{
		return this.m_portraits[turn];
	}

	// Token: 0x04002EAD RID: 11949
	public List<Texture2D> m_portraits;

	// Token: 0x04002EAE RID: 11950
	public Spell m_portraitSwapSpell;

	// Token: 0x04002EAF RID: 11951
	public float m_portraitSwapDelay = 0.5f;
}
