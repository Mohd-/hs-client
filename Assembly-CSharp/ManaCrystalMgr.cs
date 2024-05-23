using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000623 RID: 1571
[CustomEditClass]
public class ManaCrystalMgr : MonoBehaviour
{
	// Token: 0x06004490 RID: 17552 RVA: 0x00149AEC File Offset: 0x00147CEC
	private void Awake()
	{
		ManaCrystalMgr.s_instance = this;
		if (base.gameObject.GetComponent<AudioSource>() == null)
		{
			base.gameObject.AddComponent<AudioSource>();
		}
	}

	// Token: 0x06004491 RID: 17553 RVA: 0x00149B21 File Offset: 0x00147D21
	private void OnDestroy()
	{
		ManaCrystalMgr.s_instance = null;
	}

	// Token: 0x06004492 RID: 17554 RVA: 0x00149B2C File Offset: 0x00147D2C
	private void Start()
	{
		this.m_permanentCrystals = new List<ManaCrystal>();
		this.m_temporaryCrystals = new List<ManaCrystal>();
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_friendlyManaText = this.friendlyManaCounter.GetComponent<UberText>();
			ManaCounter component = this.friendlyManaCounter.GetComponent<ManaCounter>();
			this.m_friendlyManaGem = component.GetPhoneGem();
			if (this.friendlyManaGemTexture != null)
			{
				this.SetFriendlyManaGemTexture(this.friendlyManaGemTexture);
				this.friendlyManaGemTexture = null;
			}
		}
	}

	// Token: 0x06004493 RID: 17555 RVA: 0x00149BAB File Offset: 0x00147DAB
	public static ManaCrystalMgr Get()
	{
		return ManaCrystalMgr.s_instance;
	}

	// Token: 0x06004494 RID: 17556 RVA: 0x00149BB2 File Offset: 0x00147DB2
	public Vector3 GetManaCrystalSpawnPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06004495 RID: 17557 RVA: 0x00149BC0 File Offset: 0x00147DC0
	public void AddManaCrystals(int numCrystals, bool isTurnStart)
	{
		for (int i = 0; i < numCrystals; i++)
		{
			GameState.Get().GetGameEntity().NotifyOfManaCrystalSpawned();
			base.StartCoroutine(this.WaitThenAddManaCrystal(false, isTurnStart));
		}
	}

	// Token: 0x06004496 RID: 17558 RVA: 0x00149C00 File Offset: 0x00147E00
	public void AddTempManaCrystals(int numCrystals)
	{
		for (int i = 0; i < numCrystals; i++)
		{
			base.StartCoroutine(this.WaitThenAddManaCrystal(true, false));
		}
	}

	// Token: 0x06004497 RID: 17559 RVA: 0x00149C2E File Offset: 0x00147E2E
	public void DestroyManaCrystals(int numCrystals)
	{
		base.StartCoroutine(this.WaitThenDestroyManaCrystals(false, numCrystals));
	}

	// Token: 0x06004498 RID: 17560 RVA: 0x00149C3F File Offset: 0x00147E3F
	public void DestroyTempManaCrystals(int numCrystals)
	{
		base.StartCoroutine(this.WaitThenDestroyManaCrystals(true, numCrystals));
	}

	// Token: 0x06004499 RID: 17561 RVA: 0x00149C50 File Offset: 0x00147E50
	public void UpdateSpentMana(int shownChangeAmount)
	{
		if (shownChangeAmount > 0)
		{
			this.SpendManaCrystals(shownChangeAmount);
		}
		else if (GameState.Get().IsTurnStartManagerActive())
		{
			TurnStartManager.Get().NotifyOfManaCrystalFilled(-shownChangeAmount);
		}
		else
		{
			this.ReadyManaCrystals(-shownChangeAmount);
		}
	}

	// Token: 0x0600449A RID: 17562 RVA: 0x00149C98 File Offset: 0x00147E98
	public void SpendManaCrystals(int numCrystals)
	{
		SoundManager.Get().LoadAndPlay("mana_crystal_expend", base.gameObject);
		for (int i = 0; i < numCrystals; i++)
		{
			this.SpendManaCrystal();
		}
	}

	// Token: 0x0600449B RID: 17563 RVA: 0x00149CD4 File Offset: 0x00147ED4
	public void ReadyManaCrystals(int numCrystals)
	{
		for (int i = 0; i < numCrystals; i++)
		{
			this.ReadyManaCrystal();
		}
	}

	// Token: 0x0600449C RID: 17564 RVA: 0x00149CFC File Offset: 0x00147EFC
	public int GetSpendableManaCrystals()
	{
		int num = 0;
		for (int i = 0; i < this.m_temporaryCrystals.Count; i++)
		{
			ManaCrystal manaCrystal = this.m_temporaryCrystals[i];
			if (manaCrystal.state == ManaCrystal.State.READY)
			{
				num++;
			}
		}
		for (int j = 0; j < this.m_permanentCrystals.Count; j++)
		{
			ManaCrystal manaCrystal2 = this.m_permanentCrystals[j];
			if (manaCrystal2.state == ManaCrystal.State.READY)
			{
				if (!manaCrystal2.IsOverloaded())
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x0600449D RID: 17565 RVA: 0x00149D9C File Offset: 0x00147F9C
	public void CancelAllProposedMana(Entity entity)
	{
		if (entity == null)
		{
			return;
		}
		if (this.m_proposedManaSourceEntID != entity.GetEntityId())
		{
			return;
		}
		this.m_proposedManaSourceEntID = -1;
		this.m_eventSpells.m_proposeUsageSpell.ActivateState(SpellStateType.DEATH);
		for (int i = 0; i < this.m_temporaryCrystals.Count; i++)
		{
			if (this.m_temporaryCrystals[i].state == ManaCrystal.State.PROPOSED)
			{
				this.m_temporaryCrystals[i].state = ManaCrystal.State.READY;
			}
		}
		for (int j = this.m_permanentCrystals.Count - 1; j >= 0; j--)
		{
			if (this.m_permanentCrystals[j].state == ManaCrystal.State.PROPOSED)
			{
				this.m_permanentCrystals[j].state = ManaCrystal.State.READY;
			}
		}
	}

	// Token: 0x0600449E RID: 17566 RVA: 0x00149E70 File Offset: 0x00148070
	public void ProposeManaCrystalUsage(Entity entity)
	{
		if (entity == null)
		{
			return;
		}
		this.m_proposedManaSourceEntID = entity.GetEntityId();
		int cost = entity.GetCost();
		this.m_eventSpells.m_proposeUsageSpell.ActivateState(SpellStateType.BIRTH);
		int num = 0;
		for (int i = this.m_temporaryCrystals.Count - 1; i >= 0; i--)
		{
			if (this.m_temporaryCrystals[i].state == ManaCrystal.State.USED)
			{
				Log.Rachelle.Print("Found a SPENT temporary mana crystal... this shouldn't happen!", new object[0]);
			}
			else if (num < cost)
			{
				this.m_temporaryCrystals[i].state = ManaCrystal.State.PROPOSED;
				num++;
			}
			else
			{
				this.m_temporaryCrystals[i].state = ManaCrystal.State.READY;
			}
		}
		for (int j = 0; j < this.m_permanentCrystals.Count; j++)
		{
			if (this.m_permanentCrystals[j].state != ManaCrystal.State.USED)
			{
				if (!this.m_permanentCrystals[j].IsOverloaded())
				{
					if (num < cost)
					{
						this.m_permanentCrystals[j].state = ManaCrystal.State.PROPOSED;
						num++;
					}
					else
					{
						this.m_permanentCrystals[j].state = ManaCrystal.State.READY;
					}
				}
			}
		}
	}

	// Token: 0x0600449F RID: 17567 RVA: 0x00149FB4 File Offset: 0x001481B4
	public void HandleSameTurnOverloadChanged(int crystalsChanged)
	{
		if (crystalsChanged > 0)
		{
			this.MarkCrystalsOwedForOverload(crystalsChanged);
		}
		else if (crystalsChanged < 0)
		{
			this.ReclaimCrystalsOwedForOverload(-crystalsChanged);
		}
	}

	// Token: 0x060044A0 RID: 17568 RVA: 0x00149FE4 File Offset: 0x001481E4
	public void MarkCrystalsOwedForOverload(int numCrystals)
	{
		if (numCrystals > 0)
		{
			this.m_overloadLocksAreShowing = true;
		}
		int num = 0;
		int num2 = 0;
		while (numCrystals != num)
		{
			if (num2 == this.m_permanentCrystals.Count)
			{
				this.m_additionalOverloadedCrystalsOwedNextTurn += numCrystals - num;
				break;
			}
			ManaCrystal manaCrystal = this.m_permanentCrystals[num2];
			if (!manaCrystal.IsOwedForOverload())
			{
				manaCrystal.MarkAsOwedForOverload();
				num++;
			}
			num2++;
		}
	}

	// Token: 0x060044A1 RID: 17569 RVA: 0x0014A060 File Offset: 0x00148260
	public void ReclaimCrystalsOwedForOverload(int numCrystals)
	{
		int num = 0;
		int num2 = this.m_permanentCrystals.FindLastIndex((ManaCrystal crystal) => crystal.IsOwedForOverload());
		while (num < numCrystals && num2 >= 0)
		{
			ManaCrystal manaCrystal = this.m_permanentCrystals[num2];
			manaCrystal.ReclaimOverload();
			num2--;
			num++;
		}
		this.m_additionalOverloadedCrystalsOwedNextTurn -= numCrystals - num;
		this.m_overloadLocksAreShowing = (num2 >= 0 || this.m_additionalOverloadedCrystalsOwedNextTurn > 0);
	}

	// Token: 0x060044A2 RID: 17570 RVA: 0x0014A0F0 File Offset: 0x001482F0
	public void UnlockCrystals(int numCrystals)
	{
		int num = 0;
		int num2 = this.m_permanentCrystals.FindLastIndex((ManaCrystal crystal) => crystal.IsOverloaded());
		while (num < numCrystals && num2 >= 0)
		{
			ManaCrystal manaCrystal = this.m_permanentCrystals[num2];
			manaCrystal.UnlockOverload();
			num2--;
			num++;
		}
		this.m_additionalOverloadedCrystalsOwedThisTurn -= numCrystals - num;
		this.m_overloadLocksAreShowing = (num2 >= 0 || this.m_additionalOverloadedCrystalsOwedThisTurn > 0);
	}

	// Token: 0x060044A3 RID: 17571 RVA: 0x0014A180 File Offset: 0x00148380
	public void OnCurrentPlayerChanged()
	{
		this.m_additionalOverloadedCrystalsOwedThisTurn = this.m_additionalOverloadedCrystalsOwedNextTurn;
		this.m_additionalOverloadedCrystalsOwedNextTurn = 0;
		if (this.m_additionalOverloadedCrystalsOwedThisTurn > 0)
		{
			this.m_overloadLocksAreShowing = true;
		}
		else
		{
			this.m_overloadLocksAreShowing = false;
		}
		for (int i = 0; i < this.m_permanentCrystals.Count; i++)
		{
			ManaCrystal manaCrystal = this.m_permanentCrystals[i];
			if (manaCrystal.IsOverloaded())
			{
				manaCrystal.UnlockOverload();
			}
			if (manaCrystal.IsOwedForOverload())
			{
				this.m_overloadLocksAreShowing = true;
				manaCrystal.PayOverload();
			}
			else if (this.m_additionalOverloadedCrystalsOwedThisTurn > 0)
			{
				manaCrystal.PayOverload();
				this.m_additionalOverloadedCrystalsOwedThisTurn--;
			}
		}
	}

	// Token: 0x060044A4 RID: 17572 RVA: 0x0014A236 File Offset: 0x00148436
	public bool ShouldShowOverloadTooltip()
	{
		return this.m_overloadLocksAreShowing;
	}

	// Token: 0x060044A5 RID: 17573 RVA: 0x0014A240 File Offset: 0x00148440
	public void SetFriendlyManaGemTexture(Texture texture)
	{
		if (this.m_friendlyManaGem == null)
		{
			this.friendlyManaGemTexture = texture;
			return;
		}
		MeshRenderer componentInChildren = this.m_friendlyManaGem.GetComponentInChildren<MeshRenderer>();
		componentInChildren.material.mainTexture = texture;
	}

	// Token: 0x060044A6 RID: 17574 RVA: 0x0014A280 File Offset: 0x00148480
	public void SetFriendlyManaGemTint(Color tint)
	{
		if (this.m_friendlyManaGem == null)
		{
			return;
		}
		MeshRenderer componentInChildren = this.m_friendlyManaGem.GetComponentInChildren<MeshRenderer>();
		componentInChildren.material.SetColor("_TintColor", tint);
	}

	// Token: 0x060044A7 RID: 17575 RVA: 0x0014A2BC File Offset: 0x001484BC
	public void ShowPhoneManaTray()
	{
		this.m_friendlyManaGem.GetComponent<Animation>()[this.GEM_FLIP_ANIM_NAME].speed = 1f;
		this.m_friendlyManaGem.GetComponent<Animation>().Play(this.GEM_FLIP_ANIM_NAME);
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
		{
			"from",
			this.m_friendlyManaText.TextAlpha,
			"to",
			0f,
			"time",
			0.1f,
			"onupdate",
			delegate(object newVal)
			{
				this.m_friendlyManaText.TextAlpha = (float)newVal;
			}
		}));
		this.manaTrayPhone.ToggleTraySlider(true, null, true);
	}

	// Token: 0x060044A8 RID: 17576 RVA: 0x0014A380 File Offset: 0x00148580
	public void HidePhoneManaTray()
	{
		this.m_friendlyManaGem.GetComponent<Animation>()[this.GEM_FLIP_ANIM_NAME].speed = -1f;
		if (this.m_friendlyManaGem.GetComponent<Animation>()[this.GEM_FLIP_ANIM_NAME].time == 0f)
		{
			this.m_friendlyManaGem.GetComponent<Animation>()[this.GEM_FLIP_ANIM_NAME].time = this.m_friendlyManaGem.GetComponent<Animation>()[this.GEM_FLIP_ANIM_NAME].length;
		}
		this.m_friendlyManaGem.GetComponent<Animation>().Play(this.GEM_FLIP_ANIM_NAME);
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
		{
			"from",
			this.m_friendlyManaText.TextAlpha,
			"to",
			1f,
			"time",
			0.1f,
			"onupdate",
			delegate(object newVal)
			{
				this.m_friendlyManaText.TextAlpha = (float)newVal;
			}
		}));
		this.manaTrayPhone.ToggleTraySlider(false, null, true);
	}

	// Token: 0x060044A9 RID: 17577 RVA: 0x0014A4A0 File Offset: 0x001486A0
	private void UpdateLayout()
	{
		if (this.m_permanentCrystals.Count <= 0)
		{
			return;
		}
		Vector3 position = base.transform.position;
		if (UniversalInputManager.UsePhoneUI)
		{
			position = this.manaGemBone.transform.position;
		}
		for (int i = this.m_permanentCrystals.Count - 1; i >= 0; i--)
		{
			this.m_permanentCrystals[i].transform.position = position;
			if (UniversalInputManager.UsePhoneUI)
			{
				position.z += this.m_manaCrystalWidth;
			}
			else
			{
				position.x += this.m_manaCrystalWidth;
			}
		}
		for (int j = 0; j < this.m_temporaryCrystals.Count; j++)
		{
			this.m_temporaryCrystals[j].transform.position = position;
			if (UniversalInputManager.UsePhoneUI)
			{
				position.z += this.m_manaCrystalWidth;
			}
			else
			{
				position.x += this.m_manaCrystalWidth;
			}
		}
	}

	// Token: 0x060044AA RID: 17578 RVA: 0x0014A5CC File Offset: 0x001487CC
	private IEnumerator UpdatePermanentCrystalStates()
	{
		while (this.m_numQueuedToReady > 0 || this.m_numCrystalsLoading > 0 || this.m_numQueuedToSpend > 0)
		{
			yield return null;
		}
		int numUsedCrystals = GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.RESOURCES_USED);
		int i;
		for (i = 0; i < numUsedCrystals; i++)
		{
			if (i == this.m_permanentCrystals.Count)
			{
				break;
			}
			if (this.m_permanentCrystals[i].state != ManaCrystal.State.USED)
			{
				this.m_permanentCrystals[i].state = ManaCrystal.State.USED;
			}
		}
		for (int j = i; j < this.m_permanentCrystals.Count; j++)
		{
			if (this.m_permanentCrystals[j].state != ManaCrystal.State.READY)
			{
				this.m_permanentCrystals[j].state = ManaCrystal.State.READY;
			}
		}
		yield break;
	}

	// Token: 0x060044AB RID: 17579 RVA: 0x0014A5E8 File Offset: 0x001487E8
	private void LoadCrystalCallback(string actorName, GameObject actorObject, object callbackData)
	{
		this.m_numCrystalsLoading--;
		if (this.m_manaCrystalWidth <= 0f)
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				this.m_manaCrystalWidth = 0.33f;
			}
			else
			{
				this.m_manaCrystalWidth = actorObject.transform.FindChild("Gem_Mana").GetComponent<Renderer>().bounds.size.x;
			}
		}
		ManaCrystalMgr.LoadCrystalCallbackData loadCrystalCallbackData = callbackData as ManaCrystalMgr.LoadCrystalCallbackData;
		ManaCrystal component = actorObject.GetComponent<ManaCrystal>();
		if (loadCrystalCallbackData.IsTempCrystal)
		{
			component.MarkAsTemp();
			this.m_temporaryCrystals.Add(component);
		}
		else
		{
			this.m_permanentCrystals.Add(component);
			if (loadCrystalCallbackData.IsTurnStart)
			{
				if (this.m_additionalOverloadedCrystalsOwedThisTurn > 0)
				{
					component.PayOverload();
					this.m_additionalOverloadedCrystalsOwedThisTurn--;
				}
			}
			else if (this.m_additionalOverloadedCrystalsOwedNextTurn > 0)
			{
				component.state = ManaCrystal.State.USED;
				component.MarkAsOwedForOverload();
				this.m_additionalOverloadedCrystalsOwedNextTurn--;
			}
			base.StartCoroutine(this.UpdatePermanentCrystalStates());
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			component.transform.parent = this.manaGemBone.transform.parent;
			component.transform.localRotation = this.manaGemBone.transform.localRotation;
			component.transform.localScale = this.manaGemBone.transform.localScale;
		}
		else
		{
			component.transform.parent = base.transform;
		}
		component.transform.localPosition = Vector3.zero;
		component.PlayCreateAnimation();
		SoundManager.Get().LoadAndPlay("mana_crystal_add", base.gameObject);
		this.UpdateLayout();
	}

	// Token: 0x060044AC RID: 17580 RVA: 0x0014A7AC File Offset: 0x001489AC
	public float GetWidth()
	{
		if (this.m_permanentCrystals.Count == 0)
		{
			return 0f;
		}
		float x = this.m_permanentCrystals[0].transform.FindChild("Gem_Mana").GetComponent<Renderer>().bounds.size.x;
		return x * (float)this.m_permanentCrystals.Count * (float)this.m_temporaryCrystals.Count;
	}

	// Token: 0x060044AD RID: 17581 RVA: 0x0014A820 File Offset: 0x00148A20
	private IEnumerator WaitThenAddManaCrystal(bool isTemp, bool isTurnStart)
	{
		this.m_numCrystalsLoading++;
		this.m_numQueuedToSpawn++;
		yield return new WaitForSeconds((float)this.m_numQueuedToSpawn * 0.2f);
		ManaCrystalMgr.LoadCrystalCallbackData callbackData = new ManaCrystalMgr.LoadCrystalCallbackData(isTemp, isTurnStart);
		AssetLoader.Get().LoadActor("Resource", new AssetLoader.GameObjectCallback(this.LoadCrystalCallback), callbackData, false);
		this.m_numQueuedToSpawn--;
		yield break;
	}

	// Token: 0x060044AE RID: 17582 RVA: 0x0014A858 File Offset: 0x00148A58
	private IEnumerator WaitThenDestroyManaCrystals(bool isTemp, int numCrystals)
	{
		while (this.m_numCrystalsLoading > 0)
		{
			yield return null;
		}
		for (int i = 0; i < numCrystals; i++)
		{
			if (isTemp)
			{
				this.DestroyTempManaCrystal();
			}
			else
			{
				this.DestroyManaCrystal();
			}
		}
		yield break;
	}

	// Token: 0x060044AF RID: 17583 RVA: 0x0014A890 File Offset: 0x00148A90
	private IEnumerator WaitThenReadyManaCrystal()
	{
		this.m_numQueuedToReady++;
		yield return new WaitForSeconds((float)this.m_numQueuedToReady * 0.2f);
		if (this.m_permanentCrystals.Count > 0)
		{
			for (int i = this.m_permanentCrystals.Count - 1; i >= 0; i--)
			{
				if (this.m_permanentCrystals[i].state == ManaCrystal.State.USED)
				{
					SoundManager.Get().LoadAndPlay("mana_crystal_refresh", base.gameObject);
					this.m_permanentCrystals[i].state = ManaCrystal.State.READY;
					break;
				}
			}
		}
		this.m_numQueuedToReady--;
		yield break;
	}

	// Token: 0x060044B0 RID: 17584 RVA: 0x0014A8AC File Offset: 0x00148AAC
	private IEnumerator WaitThenSpendManaCrystal()
	{
		this.m_numQueuedToSpend++;
		yield return new WaitForSeconds((float)(this.m_numQueuedToSpend - 1) * 0.2f);
		for (int i = 0; i < this.m_permanentCrystals.Count; i++)
		{
			if (this.m_permanentCrystals[i].state != ManaCrystal.State.USED)
			{
				if (!this.m_permanentCrystals[i].IsOverloaded())
				{
					this.m_permanentCrystals[i].state = ManaCrystal.State.USED;
					break;
				}
			}
		}
		this.m_numQueuedToSpend--;
		if (this.m_numQueuedToSpend > 0)
		{
			yield break;
		}
		InputManager.Get().OnManaCrystalMgrManaSpent();
		yield break;
	}

	// Token: 0x060044B1 RID: 17585 RVA: 0x0014A8C8 File Offset: 0x00148AC8
	private void DestroyManaCrystal()
	{
		if (this.m_permanentCrystals.Count <= 0)
		{
			return;
		}
		int num = 0;
		ManaCrystal manaCrystal = this.m_permanentCrystals[num];
		this.m_permanentCrystals.RemoveAt(num);
		manaCrystal.GetComponent<ManaCrystal>().Destroy();
		this.UpdateLayout();
		base.StartCoroutine(this.UpdatePermanentCrystalStates());
	}

	// Token: 0x060044B2 RID: 17586 RVA: 0x0014A920 File Offset: 0x00148B20
	private void DestroyTempManaCrystal()
	{
		if (this.m_temporaryCrystals.Count <= 0)
		{
			return;
		}
		int num = this.m_temporaryCrystals.Count - 1;
		ManaCrystal manaCrystal = this.m_temporaryCrystals[num];
		this.m_temporaryCrystals.RemoveAt(num);
		manaCrystal.GetComponent<ManaCrystal>().Destroy();
		this.UpdateLayout();
	}

	// Token: 0x060044B3 RID: 17587 RVA: 0x0014A977 File Offset: 0x00148B77
	private void SpendManaCrystal()
	{
		base.StartCoroutine(this.WaitThenSpendManaCrystal());
	}

	// Token: 0x060044B4 RID: 17588 RVA: 0x0014A986 File Offset: 0x00148B86
	private void ReadyManaCrystal()
	{
		base.StartCoroutine(this.WaitThenReadyManaCrystal());
	}

	// Token: 0x04002B7C RID: 11132
	private const float ANIMATE_TIME = 0.6f;

	// Token: 0x04002B7D RID: 11133
	private const float SECS_BETW_MANA_SPAWNS = 0.2f;

	// Token: 0x04002B7E RID: 11134
	private const float SECS_BETW_MANA_READIES = 0.2f;

	// Token: 0x04002B7F RID: 11135
	private const float SECS_BETW_MANA_SPENDS = 0.2f;

	// Token: 0x04002B80 RID: 11136
	private const float GEM_FLIP_TEXT_FADE_TIME = 0.1f;

	// Token: 0x04002B81 RID: 11137
	public Material tempManaCrystalMaterial;

	// Token: 0x04002B82 RID: 11138
	public Material tempManaCrystalProposedQuadMaterial;

	// Token: 0x04002B83 RID: 11139
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public String_MobileOverride manaLockPrefab;

	// Token: 0x04002B84 RID: 11140
	public ManaCrystalEventSpells m_eventSpells;

	// Token: 0x04002B85 RID: 11141
	public SlidingTray manaTrayPhone;

	// Token: 0x04002B86 RID: 11142
	public Transform manaGemBone;

	// Token: 0x04002B87 RID: 11143
	public GameObject friendlyManaCounter;

	// Token: 0x04002B88 RID: 11144
	private readonly string GEM_FLIP_ANIM_NAME = "Resource_Large_phone_Flip";

	// Token: 0x04002B89 RID: 11145
	private static ManaCrystalMgr s_instance;

	// Token: 0x04002B8A RID: 11146
	private List<ManaCrystal> m_permanentCrystals;

	// Token: 0x04002B8B RID: 11147
	private List<ManaCrystal> m_temporaryCrystals;

	// Token: 0x04002B8C RID: 11148
	private int m_proposedManaSourceEntID = -1;

	// Token: 0x04002B8D RID: 11149
	private int m_numCrystalsLoading;

	// Token: 0x04002B8E RID: 11150
	private int m_numQueuedToSpawn;

	// Token: 0x04002B8F RID: 11151
	private int m_numQueuedToReady;

	// Token: 0x04002B90 RID: 11152
	private int m_numQueuedToSpend;

	// Token: 0x04002B91 RID: 11153
	private int m_additionalOverloadedCrystalsOwedNextTurn;

	// Token: 0x04002B92 RID: 11154
	private int m_additionalOverloadedCrystalsOwedThisTurn;

	// Token: 0x04002B93 RID: 11155
	private bool m_overloadLocksAreShowing;

	// Token: 0x04002B94 RID: 11156
	private float m_manaCrystalWidth;

	// Token: 0x04002B95 RID: 11157
	private GameObject m_friendlyManaGem;

	// Token: 0x04002B96 RID: 11158
	private UberText m_friendlyManaText;

	// Token: 0x04002B97 RID: 11159
	private Texture friendlyManaGemTexture;

	// Token: 0x02000949 RID: 2377
	private class LoadCrystalCallbackData
	{
		// Token: 0x06005748 RID: 22344 RVA: 0x001A2739 File Offset: 0x001A0939
		public LoadCrystalCallbackData(bool isTempCrystal, bool isTurnStart)
		{
			this.m_isTempCrystal = isTempCrystal;
			this.m_isTurnStart = isTurnStart;
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x06005749 RID: 22345 RVA: 0x001A274F File Offset: 0x001A094F
		public bool IsTempCrystal
		{
			get
			{
				return this.m_isTempCrystal;
			}
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x0600574A RID: 22346 RVA: 0x001A2757 File Offset: 0x001A0957
		public bool IsTurnStart
		{
			get
			{
				return this.m_isTurnStart;
			}
		}

		// Token: 0x04003E1C RID: 15900
		private bool m_isTempCrystal;

		// Token: 0x04003E1D RID: 15901
		private bool m_isTurnStart;
	}
}
