using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006A9 RID: 1705
public class MineCartRushArt : MonoBehaviour
{
	// Token: 0x06004784 RID: 18308 RVA: 0x00157338 File Offset: 0x00155538
	public void DoPortraitSwap(Actor actor)
	{
		base.StartCoroutine(this.DoPortraitSwapWithTiming(actor));
	}

	// Token: 0x06004785 RID: 18309 RVA: 0x00157348 File Offset: 0x00155548
	private IEnumerator DoPortraitSwapWithTiming(Actor actor)
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
		actor.SetPortraitTextureOverride(this.GetNextPortrait());
		yield break;
	}

	// Token: 0x06004786 RID: 18310 RVA: 0x00157374 File Offset: 0x00155574
	private Texture2D GetNextPortrait()
	{
		if (this.m_portraits.Count == 0)
		{
			return null;
		}
		if (this.m_portraits.Count == 1)
		{
			return this.m_portraits[0];
		}
		Texture2D texture2D = this.m_portraits[0];
		int num = Random.Range(1, this.m_portraits.Count);
		this.m_portraits[0] = this.m_portraits[num];
		this.m_portraits[num] = texture2D;
		return this.m_portraits[0];
	}

	// Token: 0x04002EB9 RID: 11961
	public List<Texture2D> m_portraits = new List<Texture2D>();

	// Token: 0x04002EBA RID: 11962
	public Spell m_portraitSwapSpell;

	// Token: 0x04002EBB RID: 11963
	public float m_portraitSwapDelay = 0.5f;
}
