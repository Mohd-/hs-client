using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200079E RID: 1950
public class ManaCrystal : MonoBehaviour
{
	// Token: 0x06004D00 RID: 19712 RVA: 0x0016E51A File Offset: 0x0016C71A
	private void Start()
	{
	}

	// Token: 0x06004D01 RID: 19713 RVA: 0x0016E51C File Offset: 0x0016C71C
	private void Update()
	{
		ManaCrystal.State state = this.state;
		if (state == this.m_visibleState)
		{
			return;
		}
		if (state == ManaCrystal.State.DESTROYED)
		{
			return;
		}
		string transitionAnimName = this.GetTransitionAnimName(this.m_visibleState, state);
		this.PlayGemAnimation(transitionAnimName, state);
	}

	// Token: 0x1700056A RID: 1386
	// (get) Token: 0x06004D02 RID: 19714 RVA: 0x0016E55B File Offset: 0x0016C75B
	// (set) Token: 0x06004D03 RID: 19715 RVA: 0x0016E564 File Offset: 0x0016C764
	public ManaCrystal.State state
	{
		get
		{
			return this.m_state;
		}
		set
		{
			if (this.m_state == ManaCrystal.State.DESTROYED)
			{
				return;
			}
			if (value == ManaCrystal.State.DESTROYED)
			{
				this.Destroy();
				return;
			}
			this.m_state = value;
		}
	}

	// Token: 0x06004D04 RID: 19716 RVA: 0x0016E593 File Offset: 0x0016C793
	public void MarkAsNotInGame()
	{
		this.m_isInGame = false;
	}

	// Token: 0x06004D05 RID: 19717 RVA: 0x0016E59C File Offset: 0x0016C79C
	public void MarkAsTemp()
	{
		this.m_isTemp = true;
		ManaCrystalMgr manaCrystalMgr = ManaCrystalMgr.Get();
		MeshRenderer componentInChildren = this.gem.GetComponentInChildren<MeshRenderer>();
		componentInChildren.material = manaCrystalMgr.tempManaCrystalMaterial;
		Transform transform = this.gem.transform.FindChild("Proposed_Quad");
		MeshRenderer component = transform.gameObject.GetComponent<MeshRenderer>();
		component.material = manaCrystalMgr.tempManaCrystalProposedQuadMaterial;
	}

	// Token: 0x06004D06 RID: 19718 RVA: 0x0016E5FC File Offset: 0x0016C7FC
	public void PlayCreateAnimation()
	{
		this.spawnEffects.SetActive(!this.m_isTemp);
		this.tempSpawnEffects.SetActive(this.m_isTemp);
		if (this.m_isTemp)
		{
			this.tempSpawnEffects.GetComponent<Animation>().Play(this.ANIM_TEMP_SPAWN_EFFECTS);
			this.PlayGemAnimation(this.ANIM_TEMP_MANA_GEM_BIRTH, ManaCrystal.State.READY);
		}
		else
		{
			this.spawnEffects.GetComponent<Animation>().Play(this.ANIM_SPAWN_EFFECTS);
			this.PlayGemAnimation(this.ANIM_MANA_GEM_BIRTH, ManaCrystal.State.READY);
		}
	}

	// Token: 0x06004D07 RID: 19719 RVA: 0x0016E686 File Offset: 0x0016C886
	public void Destroy()
	{
		this.m_state = ManaCrystal.State.DESTROYED;
		base.StartCoroutine(this.WaitThenDestroy());
	}

	// Token: 0x06004D08 RID: 19720 RVA: 0x0016E69C File Offset: 0x0016C89C
	public bool IsOverloaded()
	{
		return this.m_overloadPaidSpell != null;
	}

	// Token: 0x06004D09 RID: 19721 RVA: 0x0016E6AA File Offset: 0x0016C8AA
	public bool IsOwedForOverload()
	{
		return this.m_overloadOwedSpell != null;
	}

	// Token: 0x06004D0A RID: 19722 RVA: 0x0016E6B8 File Offset: 0x0016C8B8
	public void MarkAsOwedForOverload()
	{
		this.MarkAsOwedForOverload(false);
	}

	// Token: 0x06004D0B RID: 19723 RVA: 0x0016E6C4 File Offset: 0x0016C8C4
	public void ReclaimOverload()
	{
		if (!this.IsOwedForOverload())
		{
			return;
		}
		this.m_overloadOwedSpell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadBirthCompletePayOverload));
		this.m_overloadOwedSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadUnlockedAnimComplete));
		this.m_overloadOwedSpell.ActivateState(SpellStateType.DEATH);
		this.m_overloadOwedSpell = null;
	}

	// Token: 0x06004D0C RID: 19724 RVA: 0x0016E720 File Offset: 0x0016C920
	public void PayOverload()
	{
		if (!this.IsOwedForOverload())
		{
			this.state = ManaCrystal.State.USED;
			this.MarkAsOwedForOverload(true);
			return;
		}
		this.m_overloadPaidSpell = this.m_overloadOwedSpell;
		this.m_overloadOwedSpell = null;
		this.m_overloadPaidSpell.ActivateState(SpellStateType.ACTION);
	}

	// Token: 0x06004D0D RID: 19725 RVA: 0x0016E766 File Offset: 0x0016C966
	public void UnlockOverload()
	{
		if (!this.IsOverloaded())
		{
			return;
		}
		this.m_overloadPaidSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadUnlockedAnimComplete));
		this.m_overloadPaidSpell.ActivateState(SpellStateType.DEATH);
		this.m_overloadPaidSpell = null;
	}

	// Token: 0x06004D0E RID: 19726 RVA: 0x0016E7A0 File Offset: 0x0016C9A0
	private void PlayGemAnimation(string animName, ManaCrystal.State newVisibleState)
	{
		if (this.m_isInGame && !this.m_birthAnimationPlayed)
		{
			if (!animName.Equals(this.ANIM_MANA_GEM_BIRTH) && !animName.Equals(this.ANIM_TEMP_MANA_GEM_BIRTH))
			{
				return;
			}
			this.m_birthAnimationPlayed = true;
		}
		if (!this.gem.GetComponent<Animation>()[animName])
		{
			Debug.LogWarning(string.Format("Mana gem animation named '{0}' doesn't exist.", animName));
			return;
		}
		if (this.state == ManaCrystal.State.DESTROYED)
		{
			return;
		}
		if (this.m_playingAnimation)
		{
			return;
		}
		this.m_playingAnimation = true;
		this.gem.GetComponent<Animation>()[animName].normalizedTime = 1f;
		this.gem.GetComponent<Animation>()[animName].time = 0f;
		this.gem.GetComponent<Animation>()[animName].speed = 1f;
		this.gem.GetComponent<Animation>().Play(animName);
		if (!base.gameObject.activeInHierarchy)
		{
			this.m_playingAnimation = false;
			this.m_visibleState = newVisibleState;
		}
		else
		{
			base.StartCoroutine(this.WaitForAnimation(animName, newVisibleState));
		}
	}

	// Token: 0x06004D0F RID: 19727 RVA: 0x0016E8D4 File Offset: 0x0016CAD4
	private IEnumerator WaitForAnimation(string animName, ManaCrystal.State newVisibleState)
	{
		yield return new WaitForSeconds(this.gem.GetComponent<Animation>()[animName].length);
		this.m_visibleState = newVisibleState;
		this.m_playingAnimation = false;
		yield break;
	}

	// Token: 0x06004D10 RID: 19728 RVA: 0x0016E90C File Offset: 0x0016CB0C
	private string GetTransitionAnimName(ManaCrystal.State oldState, ManaCrystal.State newState)
	{
		string result = string.Empty;
		switch (oldState)
		{
		case ManaCrystal.State.READY:
			if (newState == ManaCrystal.State.PROPOSED)
			{
				result = ((!this.m_isTemp) ? this.ANIM_READY_TO_PROPOSED : this.ANIM_TEMP_READY_TO_PROPOSED);
			}
			else if (newState == ManaCrystal.State.USED)
			{
				result = this.ANIM_READY_TO_USED;
			}
			break;
		case ManaCrystal.State.USED:
			if (newState == ManaCrystal.State.READY)
			{
				result = this.ANIM_USED_TO_READY;
			}
			else if (newState == ManaCrystal.State.PROPOSED)
			{
				result = this.ANIM_USED_TO_PROPOSED;
			}
			break;
		case ManaCrystal.State.PROPOSED:
			if (newState == ManaCrystal.State.READY)
			{
				result = ((!this.m_isTemp) ? this.ANIM_PROPOSED_TO_READY : this.ANIM_TEMP_PROPOSED_TO_READY);
			}
			else if (newState == ManaCrystal.State.USED)
			{
				result = this.ANIM_PROPOSED_TO_USED;
			}
			break;
		case ManaCrystal.State.DESTROYED:
			Log.Rachelle.Print("Trying to get an anim name for a mana that's been destroyed!!!", new object[0]);
			break;
		}
		return result;
	}

	// Token: 0x06004D11 RID: 19729 RVA: 0x0016E9F4 File Offset: 0x0016CBF4
	private IEnumerator WaitThenDestroy()
	{
		while (this.m_playingAnimation)
		{
			yield return null;
		}
		Spell spell = (!this.m_isTemp) ? this.gemDestroy.GetComponent<Spell>() : this.tempGemDestroy.GetComponent<Spell>();
		spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnGemDestroyedAnimComplete));
		spell.Activate();
		yield break;
	}

	// Token: 0x06004D12 RID: 19730 RVA: 0x0016EA0F File Offset: 0x0016CC0F
	private void OnGemDestroyedAnimComplete(Spell spell, SpellStateType spellStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06004D13 RID: 19731 RVA: 0x0016EA28 File Offset: 0x0016CC28
	private void OnOverloadUnlockedAnimComplete(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		Object.Destroy(spell.transform.parent.gameObject);
	}

	// Token: 0x06004D14 RID: 19732 RVA: 0x0016EA58 File Offset: 0x0016CC58
	private void OnOverloadBirthCompletePayOverload(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.IDLE)
		{
			return;
		}
		spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadBirthCompletePayOverload));
		this.PayOverload();
	}

	// Token: 0x06004D15 RID: 19733 RVA: 0x0016EA8C File Offset: 0x0016CC8C
	public void MarkAsOwedForOverload(bool immediatelyLockForOverload)
	{
		if (this.IsOwedForOverload())
		{
			if (immediatelyLockForOverload)
			{
				this.PayOverload();
			}
			return;
		}
		GameObject gameObject = (GameObject)GameUtils.InstantiateGameObject(ManaCrystalMgr.Get().manaLockPrefab, base.gameObject, false);
		if (UniversalInputManager.UsePhoneUI)
		{
			gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
			gameObject.transform.localPosition = new Vector3(0f, 0.1f, 0f);
			float num = 1.1f;
			gameObject.transform.localScale = new Vector3(num, num, num);
		}
		else
		{
			float num2 = 1f / base.transform.localScale.x;
			gameObject.transform.localScale = new Vector3(num2, num2, num2);
		}
		this.m_overloadOwedSpell = gameObject.transform.FindChild("Lock_Mana").GetComponent<Spell>();
		this.m_overloadOwedSpell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadUnlockedAnimComplete));
		if (immediatelyLockForOverload)
		{
			this.m_overloadOwedSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnOverloadBirthCompletePayOverload));
		}
		this.m_overloadOwedSpell.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x040033EA RID: 13290
	public GameObject gem;

	// Token: 0x040033EB RID: 13291
	public GameObject spawnEffects;

	// Token: 0x040033EC RID: 13292
	public GameObject gemDestroy;

	// Token: 0x040033ED RID: 13293
	public GameObject tempSpawnEffects;

	// Token: 0x040033EE RID: 13294
	public GameObject tempGemDestroy;

	// Token: 0x040033EF RID: 13295
	private readonly string ANIM_SPAWN_EFFECTS = "mana_spawn_edit";

	// Token: 0x040033F0 RID: 13296
	private readonly string ANIM_TEMP_SPAWN_EFFECTS = "mana_spawn_edit_temp";

	// Token: 0x040033F1 RID: 13297
	private readonly string ANIM_MANA_GEM_BIRTH = "ManaGemBirth";

	// Token: 0x040033F2 RID: 13298
	private readonly string ANIM_TEMP_MANA_GEM_BIRTH = "ManaGemBirth_Temp";

	// Token: 0x040033F3 RID: 13299
	private readonly string ANIM_READY_TO_USED = "ManaGemUsed";

	// Token: 0x040033F4 RID: 13300
	private readonly string ANIM_USED_TO_READY = "ManaGem_Restore";

	// Token: 0x040033F5 RID: 13301
	private readonly string ANIM_READY_TO_PROPOSED = "ManaGemProposed";

	// Token: 0x040033F6 RID: 13302
	private readonly string ANIM_TEMP_READY_TO_PROPOSED = "ManaGemProposed_Temp";

	// Token: 0x040033F7 RID: 13303
	private readonly string ANIM_PROPOSED_TO_READY = "ManaGemProposed_Cancel";

	// Token: 0x040033F8 RID: 13304
	private readonly string ANIM_TEMP_PROPOSED_TO_READY = "ManaGemProposed_Cancel_Temp";

	// Token: 0x040033F9 RID: 13305
	private readonly string ANIM_USED_TO_PROPOSED = "ManaGemUsed_Proposed";

	// Token: 0x040033FA RID: 13306
	private readonly string ANIM_PROPOSED_TO_USED = "ManaGemProposed_Used";

	// Token: 0x040033FB RID: 13307
	private bool m_isInGame = true;

	// Token: 0x040033FC RID: 13308
	private bool m_birthAnimationPlayed;

	// Token: 0x040033FD RID: 13309
	private bool m_playingAnimation;

	// Token: 0x040033FE RID: 13310
	private bool m_isTemp;

	// Token: 0x040033FF RID: 13311
	private Spell m_overloadOwedSpell;

	// Token: 0x04003400 RID: 13312
	private Spell m_overloadPaidSpell;

	// Token: 0x04003401 RID: 13313
	private ManaCrystal.State m_state;

	// Token: 0x04003402 RID: 13314
	private ManaCrystal.State m_visibleState;

	// Token: 0x0200079F RID: 1951
	public enum State
	{
		// Token: 0x04003404 RID: 13316
		READY,
		// Token: 0x04003405 RID: 13317
		USED,
		// Token: 0x04003406 RID: 13318
		PROPOSED,
		// Token: 0x04003407 RID: 13319
		DESTROYED
	}
}
