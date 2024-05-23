using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E74 RID: 3700
[RequireComponent(typeof(Animation))]
public class Misdirection : Spell
{
	// Token: 0x0600702D RID: 28717 RVA: 0x0020FCD4 File Offset: 0x0020DED4
	public override bool AddPowerTargets()
	{
		if (!base.CanAddPowerTargets())
		{
			return false;
		}
		Card sourceCard = base.GetSourceCard();
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		base.AddMultiplePowerTargets_FromAnyPower(sourceCard, taskList);
		return true;
	}

	// Token: 0x0600702E RID: 28718 RVA: 0x0020FD0C File Offset: 0x0020DF0C
	protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		if (power.Type != Network.PowerType.TAG_CHANGE)
		{
			return null;
		}
		Network.HistTagChange histTagChange = power as Network.HistTagChange;
		if (histTagChange.Tag != 37)
		{
			return null;
		}
		Entity entity = GameState.Get().GetEntity(histTagChange.Value);
		if (entity == null)
		{
			string text = string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", this, histTagChange.Value);
			Debug.LogWarning(text);
			return null;
		}
		return entity.GetCard();
	}

	// Token: 0x0600702F RID: 28719 RVA: 0x0020FD80 File Offset: 0x0020DF80
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.StartAnimation();
	}

	// Token: 0x06007030 RID: 28720 RVA: 0x0020FD88 File Offset: 0x0020DF88
	private void StartAnimation()
	{
		GameState gameState = GameState.Get();
		GameEntity gameEntity = gameState.GetGameEntity();
		Entity entity = gameState.GetEntity(gameEntity.GetTag(GAME_TAG.PROPOSED_ATTACKER));
		this.m_AttackingMinionCard = entity.GetCard();
		this.m_PlayerHeroCard = this.GetCurrentPlayerHeroCard();
		this.m_ReticleInstance = (GameObject)Object.Instantiate(this.m_Reticle, this.m_PlayerHeroCard.transform.position, Quaternion.identity);
		Material renderMaterial = this.m_ReticleInstance.GetComponent<RenderToTexture>().GetRenderMaterial();
		renderMaterial.SetFloat("_Alpha", 0f);
		renderMaterial.SetFloat("_blur", this.m_ReticleBlur);
		base.StartCoroutine(this.ReticleFadeIn());
		base.StartCoroutine(this.AnimateReticle());
		AudioSource component = base.GetComponent<AudioSource>();
		if (component != null)
		{
			SoundManager.Get().Play(component);
		}
	}

	// Token: 0x06007031 RID: 28721 RVA: 0x0020FE64 File Offset: 0x0020E064
	private IEnumerator ReticleFadeIn()
	{
		Action<object> reticleFadeInUpdate = delegate(object amount)
		{
			Material renderMaterial = this.m_ReticleInstance.GetComponent<RenderToTexture>().GetRenderMaterial();
			renderMaterial.SetFloat("_Alpha", (float)amount);
		};
		Hashtable reticleFadeInArgs = iTween.Hash(new object[]
		{
			"time",
			this.m_ReticleFadeInTime,
			"from",
			0f,
			"to",
			1f,
			"onupdate",
			reticleFadeInUpdate,
			"onupdatetarget",
			this.m_ReticleInstance.gameObject
		});
		iTween.ValueTo(this.m_ReticleInstance.gameObject, reticleFadeInArgs);
		Hashtable reticleAttackScaleArgs = iTween.Hash(new object[]
		{
			"time",
			this.m_ReticleFadeInTime,
			"scale",
			Vector3.one,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.ScaleTo(this.m_ReticleInstance.gameObject, reticleAttackScaleArgs);
		yield break;
	}

	// Token: 0x06007032 RID: 28722 RVA: 0x0020FE80 File Offset: 0x0020E080
	private void SetReticleAlphaValue(float val)
	{
		Material renderMaterial = this.m_ReticleInstance.GetComponent<RenderToTexture>().GetRenderMaterial();
		renderMaterial.SetFloat("_Alpha", val);
	}

	// Token: 0x06007033 RID: 28723 RVA: 0x0020FEAC File Offset: 0x0020E0AC
	private IEnumerator AnimateReticle()
	{
		yield return new WaitForSeconds(this.m_ReticleFadeInTime);
		Hashtable reticalArgs = iTween.Hash(new object[]
		{
			"path",
			this.BuildAnimationPath(),
			"time",
			this.m_ReticlePathTime,
			"easetype",
			iTween.EaseType.easeInOutQuad,
			"oncomplete",
			"ReticleAnimationComplete",
			"oncompletetarget",
			base.gameObject,
			"orienttopath",
			false
		});
		iTween.MoveTo(this.m_ReticleInstance, reticalArgs);
		yield break;
	}

	// Token: 0x06007034 RID: 28724 RVA: 0x0020FEC7 File Offset: 0x0020E0C7
	private void ReticleAnimationComplete()
	{
		base.StartCoroutine(this.ReticleAttackAnimation());
	}

	// Token: 0x06007035 RID: 28725 RVA: 0x0020FED8 File Offset: 0x0020E0D8
	private IEnumerator ReticleAttackAnimation()
	{
		Action<object> reticleAttackColorUpdate = delegate(object col)
		{
			if (this.m_ReticleInstance != null)
			{
				Material renderMaterial = this.m_ReticleInstance.GetComponent<RenderToTexture>().GetRenderMaterial();
				renderMaterial.SetColor("_Color", (Color)col);
			}
		};
		Hashtable reticleAttackColorArgs = iTween.Hash(new object[]
		{
			"time",
			this.m_ReticleAttackTime,
			"from",
			this.m_ReticleInstance.GetComponent<RenderToTexture>().GetRenderMaterial().color,
			"to",
			this.m_ReticleAttackColor,
			"onupdate",
			reticleAttackColorUpdate,
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(base.gameObject, reticleAttackColorArgs);
		Hashtable reticleAttackScaleArgs = iTween.Hash(new object[]
		{
			"time",
			this.m_ReticleAttackTime,
			"scale",
			this.m_ReticleAttackScale,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.ScaleTo(this.m_ReticleInstance, reticleAttackScaleArgs);
		Hashtable reticleAttackRotArgs = iTween.Hash(new object[]
		{
			"time",
			this.m_ReticleAttackTime,
			"rotation",
			this.m_ReticleAttackRotate,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.RotateTo(this.m_ReticleInstance, reticleAttackRotArgs);
		Action<object> reticleFocusUpdate = delegate(object amount)
		{
			if (this.m_ReticleInstance != null)
			{
				Material renderMaterial = this.m_ReticleInstance.GetComponent<RenderToTexture>().GetRenderMaterial();
				renderMaterial.SetFloat("_Blur", (float)amount);
			}
		};
		Hashtable reticleFocusArgs = iTween.Hash(new object[]
		{
			"time",
			this.m_ReticleBlurFocusTime,
			"from",
			this.m_ReticleBlur,
			"to",
			0f,
			"onupdate",
			reticleFocusUpdate,
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(base.gameObject, reticleFocusArgs);
		yield return new WaitForSeconds(this.m_ReticleBlurFocusTime + this.m_ReticleAttackHold);
		base.StartCoroutine(this.ReticleFadeOut());
		yield break;
	}

	// Token: 0x06007036 RID: 28726 RVA: 0x0020FEF4 File Offset: 0x0020E0F4
	private IEnumerator ReticleFadeOut()
	{
		this.OnSpellFinished();
		Action<object> reticleFadeOutUpdate = delegate(object amount)
		{
			if (this.m_ReticleInstance != null)
			{
				Material renderMaterial = this.m_ReticleInstance.GetComponent<RenderToTexture>().GetRenderMaterial();
				renderMaterial.SetFloat("_Alpha", (float)amount);
			}
		};
		Hashtable reticleFadeOutArgs = iTween.Hash(new object[]
		{
			"time",
			this.m_ReticleFadeOutTime,
			"from",
			1f,
			"to",
			0f,
			"onupdate",
			reticleFadeOutUpdate,
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(base.gameObject, reticleFadeOutArgs);
		yield return new WaitForSeconds(this.m_ReticleFadeOutTime);
		Object.Destroy(this.m_ReticleInstance);
		yield break;
	}

	// Token: 0x06007037 RID: 28727 RVA: 0x0020FF10 File Offset: 0x0020E110
	private Vector3[] BuildAnimationPath()
	{
		Card[] array = this.FindPossibleTargetCards();
		int num = Random.Range(3, 4);
		if (num >= array.Length + 2)
		{
			num = array.Length + 2;
		}
		if (array.Length <= 1)
		{
			return new Vector3[]
			{
				this.m_PlayerHeroCard.transform.position,
				base.GetTarget().transform.position
			};
		}
		List<Vector3> list = new List<Vector3>();
		list.Add(this.m_PlayerHeroCard.transform.position);
		GameObject gameObject = this.m_PlayerHeroCard.gameObject;
		for (int i = 1; i < num; i++)
		{
			GameObject gameObject2 = array[Random.Range(0, array.Length - 1)].gameObject;
			if (gameObject2 == gameObject)
			{
				gameObject2 = array[Random.Range(0, array.Length - 1)].gameObject;
				if (gameObject2 == gameObject)
				{
					if (gameObject2 == array[array.Length - 1])
					{
						gameObject2 = array[0].gameObject;
					}
					else
					{
						gameObject2 = array[array.Length - 1].gameObject;
					}
				}
			}
			if (i == num - 1 && gameObject2 == base.GetTarget() && gameObject2 == gameObject)
			{
				if (gameObject2 == array[array.Length - 1])
				{
					gameObject2 = array[0].gameObject;
				}
				else
				{
					gameObject2 = array[array.Length - 1].gameObject;
				}
			}
			list.Add(gameObject2.transform.position);
		}
		list.Add(base.GetTarget().transform.position);
		return list.ToArray();
	}

	// Token: 0x06007038 RID: 28728 RVA: 0x002100C4 File Offset: 0x0020E2C4
	private Card[] FindPossibleTargetCards()
	{
		List<Card> list = new List<Card>();
		ZoneMgr zoneMgr = ZoneMgr.Get();
		if (zoneMgr == null)
		{
			return list.ToArray();
		}
		List<ZonePlay> list2 = zoneMgr.FindZonesOfType<ZonePlay>();
		foreach (ZonePlay zonePlay in list2)
		{
			foreach (Card card in zonePlay.GetCards())
			{
				if (!(card == this.m_AttackingMinionCard))
				{
					list.Add(card);
				}
			}
		}
		list.Add(this.GetOpponentHeroCard());
		return list.ToArray();
	}

	// Token: 0x06007039 RID: 28729 RVA: 0x002101B0 File Offset: 0x0020E3B0
	private Card[] GetOpponentZoneMinions()
	{
		List<Card> list = new List<Card>();
		Player firstOpponentPlayer = GameState.Get().GetFirstOpponentPlayer(base.GetSourceCard().GetController());
		ZonePlay battlefieldZone = firstOpponentPlayer.GetBattlefieldZone();
		foreach (Card card in battlefieldZone.GetCards())
		{
			if (!(card == this.m_AttackingMinionCard))
			{
				list.Add(card);
			}
		}
		return list.ToArray();
	}

	// Token: 0x0600703A RID: 28730 RVA: 0x0021024C File Offset: 0x0020E44C
	private Card GetCurrentPlayerHeroCard()
	{
		Player controller = base.GetSourceCard().GetController();
		return controller.GetHeroCard();
	}

	// Token: 0x0600703B RID: 28731 RVA: 0x0021026C File Offset: 0x0020E46C
	private Card GetOpponentHeroCard()
	{
		Player firstOpponentPlayer = GameState.Get().GetFirstOpponentPlayer(base.GetSourceCard().GetController());
		return firstOpponentPlayer.GetHeroCard();
	}

	// Token: 0x04005950 RID: 22864
	public float m_ReticleFadeInTime = 0.8f;

	// Token: 0x04005951 RID: 22865
	public float m_ReticleFadeOutTime = 0.4f;

	// Token: 0x04005952 RID: 22866
	public float m_ReticlePathTime = 3f;

	// Token: 0x04005953 RID: 22867
	public float m_ReticleBlur = 0.005f;

	// Token: 0x04005954 RID: 22868
	public float m_ReticleBlurFocusTime = 0.8f;

	// Token: 0x04005955 RID: 22869
	public Color m_ReticleAttackColor = Color.red;

	// Token: 0x04005956 RID: 22870
	public float m_ReticleAttackScale = 1.1f;

	// Token: 0x04005957 RID: 22871
	public float m_ReticleAttackTime = 0.3f;

	// Token: 0x04005958 RID: 22872
	public Vector3 m_ReticleAttackRotate = new Vector3(0f, 90f, 0f);

	// Token: 0x04005959 RID: 22873
	public float m_ReticleAttackHold = 0.25f;

	// Token: 0x0400595A RID: 22874
	public GameObject m_Reticle;

	// Token: 0x0400595B RID: 22875
	private GameObject m_ReticleInstance;

	// Token: 0x0400595C RID: 22876
	private Card m_AttackingMinionCard;

	// Token: 0x0400595D RID: 22877
	private Card m_PlayerHeroCard;

	// Token: 0x0400595E RID: 22878
	private Color m_OrgAmbient;
}
