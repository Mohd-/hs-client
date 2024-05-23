using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E6E RID: 3694
public class MimironsHead : SuperSpell
{
	// Token: 0x06006FF3 RID: 28659 RVA: 0x0020DFA8 File Offset: 0x0020C1A8
	public override bool AddPowerTargets()
	{
		if (!base.CanAddPowerTargets())
		{
			return false;
		}
		Entity sourceEntity = this.m_taskList.GetSourceEntity();
		Card card = sourceEntity.GetCard();
		if (this.m_taskList.IsOrigin())
		{
			List<PowerTaskList> list = new List<PowerTaskList>();
			for (PowerTaskList powerTaskList = this.m_taskList; powerTaskList != null; powerTaskList = powerTaskList.GetNext())
			{
				list.Add(powerTaskList);
			}
			foreach (PowerTaskList powerTaskList2 in list)
			{
				foreach (PowerTask powerTask in powerTaskList2.GetTaskList())
				{
					Network.PowerHistory power = powerTask.GetPower();
					if (power.Type == Network.PowerType.TAG_CHANGE)
					{
						Network.HistTagChange histTagChange = power as Network.HistTagChange;
						if (histTagChange.Tag == 360 && histTagChange.Value == 1)
						{
							Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
							if (entity == null)
							{
								string text = string.Format("{0}.AddPowerTargets() - WARNING trying to target entity with id {1} but there is no entity with that id", this, histTagChange.Entity);
								Debug.LogWarning(text);
								continue;
							}
							Card card2 = entity.GetCard();
							if (card2 != card)
							{
								this.m_mechMinions.Add(card2);
							}
							else
							{
								this.m_mimiron = card2;
							}
							this.m_waitForTaskList = powerTaskList2;
						}
					}
					if (power.Type == Network.PowerType.FULL_ENTITY)
					{
						Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
						Network.Entity entity2 = histFullEntity.Entity;
						Entity entity3 = GameState.Get().GetEntity(entity2.ID);
						if (entity3 == null)
						{
							string text2 = string.Format("{0}.AddPowerTargets() - WARNING trying to target entity with id {1} but there is no entity with that id", this, entity2.ID);
							Debug.LogWarning(text2);
						}
						else
						{
							Card card3 = entity3.GetCard();
							Log.Becca.Print("Found V-07-TR-0N: {0}", new object[]
							{
								entity3.GetName()
							});
							this.m_volt = card3;
							this.m_waitForTaskList = powerTaskList2;
						}
					}
				}
			}
			if (this.m_volt != null && this.m_mimiron != null && this.m_mechMinions.Count > 0)
			{
				this.m_mimiron.IgnoreDeath(true);
				foreach (Card card4 in this.m_mechMinions)
				{
					card4.IgnoreDeath(true);
				}
				ZonePlay battlefieldZone = card.GetController().GetBattlefieldZone();
				foreach (Card card5 in battlefieldZone.GetCards())
				{
					card5.SetDoNotSort(true);
				}
			}
			else
			{
				this.m_volt = null;
				this.m_mimiron = null;
				this.m_mechMinions.Clear();
			}
		}
		if (this.m_volt == null || this.m_mimiron == null || this.m_mechMinions.Count == 0 || this.m_taskList != this.m_waitForTaskList)
		{
			return false;
		}
		ZonePlay battlefieldZone2 = card.GetController().GetBattlefieldZone();
		foreach (Card card6 in battlefieldZone2.GetCards())
		{
			card6.SetDoNotSort(true);
		}
		return true;
	}

	// Token: 0x06006FF4 RID: 28660 RVA: 0x0020E3C4 File Offset: 0x0020C5C4
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		if (this.m_voltSpawnOverrideSpell)
		{
			this.m_volt.OverrideCustomSpawnSpell(Object.Instantiate<Spell>(this.m_voltSpawnOverrideSpell));
		}
		Log.Becca.Print("V-07-TR-0N: {0}", new object[]
		{
			this.m_volt.GetEntity().GetName()
		});
		base.StartCoroutine(this.TransformEffect());
	}

	// Token: 0x06006FF5 RID: 28661 RVA: 0x0020E444 File Offset: 0x0020C644
	private IEnumerator TransformEffect()
	{
		foreach (string sound in this.m_startSounds)
		{
			SoundManager.Get().LoadAndPlay(sound);
		}
		this.m_volt.SetDoNotSort(true);
		this.m_taskList.DoAllTasks();
		while (!this.m_taskList.IsComplete())
		{
			yield return null;
		}
		this.m_volt.GetActor().Hide();
		GameObject volt = this.m_volt.GetActor().gameObject;
		this.m_voltParent = volt.transform.parent;
		volt.transform.parent = this.m_highPosBone.transform;
		volt.transform.localPosition = new Vector3(0f, -0.1f, 0f);
		this.m_root.transform.parent = null;
		this.m_root.transform.localPosition = Vector3.zero;
		GameObject mimiron = this.m_mimiron.gameObject;
		iTween.MoveTo(mimiron, iTween.Hash(new object[]
		{
			"position",
			this.m_highPosBone.transform.localPosition,
			"easetype",
			iTween.EaseType.easeOutQuart,
			"time",
			this.m_mimironHighTime,
			"delay",
			0.5f
		}));
		yield return new WaitForSeconds(0.5f + this.m_mimironHighTime / 5f);
		this.TransformMinions();
		yield break;
	}

	// Token: 0x06006FF6 RID: 28662 RVA: 0x0020E460 File Offset: 0x0020C660
	private void TransformMinions()
	{
		float num = 1f;
		Vector3 vector;
		vector..ctor(0f, 0f, 2.3f);
		List<int> list = new List<int>();
		for (int i = 0; i < this.m_mechMinions.Count; i++)
		{
			list.Add(i);
		}
		List<int> list2 = new List<int>();
		for (int j = 0; j < this.m_mechMinions.Count; j++)
		{
			int num2 = Random.Range(0, list.Count);
			list2.Add(list[num2]);
			list.RemoveAt(num2);
		}
		for (int k = 0; k < this.m_mechMinions.Count; k++)
		{
			Vector3 vector2 = Quaternion.Euler(0f, (float)(360 / this.m_mechMinions.Count * list2[k] + 15), 0f) * vector;
			this.m_minionPosBone.transform.localPosition = this.m_highPosBone.transform.localPosition + vector2;
			GameObject gameObject = this.m_mechMinions[k].GetActor().gameObject;
			float num3 = num / (float)this.m_mechMinions.Count * (float)k;
			base.StartCoroutine(this.MinionPlayFX(gameObject, this.m_minionElectricity, num3 / 2f));
			List<Vector3> list3 = new List<Vector3>();
			Vector3 vector3;
			vector3..ctor(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
			list3.Add(gameObject.transform.position + (this.m_minionPosBone.transform.localPosition - gameObject.transform.position) / 4f + vector3);
			list3.Add(this.m_minionPosBone.transform.localPosition);
			if (k < this.m_mechMinions.Count - 1)
			{
				iTween.MoveTo(gameObject, iTween.Hash(new object[]
				{
					"path",
					list3.ToArray(),
					"easetype",
					iTween.EaseType.easeInOutSine,
					"delay",
					num3,
					"time",
					this.m_minionHighTime / (float)this.m_mechMinions.Count
				}));
			}
			else
			{
				iTween.MoveTo(gameObject, iTween.Hash(new object[]
				{
					"path",
					list3.ToArray(),
					"easetype",
					iTween.EaseType.easeInOutSine,
					"delay",
					num3,
					"time",
					this.m_minionHighTime / (float)this.m_mechMinions.Count,
					"oncomplete",
					delegate(object newVal)
					{
						this.FadeInBackground();
					}
				}));
			}
		}
	}

	// Token: 0x06006FF7 RID: 28663 RVA: 0x0020E760 File Offset: 0x0020C960
	private IEnumerator MinionPlayFX(GameObject minion, GameObject FX, float delay)
	{
		GameObject minionFX = Object.Instantiate<GameObject>(FX);
		minionFX.transform.parent = minion.transform;
		minionFX.transform.localPosition = new Vector3(0f, 0.5f, 0f);
		if (!this.m_cleanup.ContainsKey(minion))
		{
			this.m_cleanup.Add(minion, new List<GameObject>());
		}
		this.m_cleanup[minion].Add(minionFX);
		yield return new WaitForSeconds(delay);
		minionFX.GetComponent<ParticleSystem>().Play();
		yield break;
	}

	// Token: 0x06006FF8 RID: 28664 RVA: 0x0020E7A8 File Offset: 0x0020C9A8
	private IEnumerator MimironNegativeFX()
	{
		while (this.m_isNegFlash)
		{
			yield return new WaitForSeconds(this.m_flashDelay);
			this.m_mimironNegative.SetActive(!this.m_mimironNegative.activeSelf);
			if (this.m_flashDelay > 0.05f)
			{
				this.m_flashDelay -= 0.01f;
			}
		}
		this.m_mimironNegative.SetActive(false);
		yield break;
	}

	// Token: 0x06006FF9 RID: 28665 RVA: 0x0020E7C4 File Offset: 0x0020C9C4
	private void MinionCleanup(GameObject minion)
	{
		if (this.m_cleanup.ContainsKey(minion))
		{
			foreach (GameObject gameObject in this.m_cleanup[minion])
			{
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
			}
		}
	}

	// Token: 0x06006FFA RID: 28666 RVA: 0x0020E840 File Offset: 0x0020CA40
	private void FadeInBackground()
	{
		this.m_background.SetActive(true);
		Material material = this.m_background.GetComponent<Renderer>().material;
		material.SetColor("_Color", this.m_clear);
		HighlightState componentInChildren = this.m_volt.GetActor().gameObject.GetComponentInChildren<HighlightState>();
		if (componentInChildren != null)
		{
			componentInChildren.Hide();
		}
		iTween.ColorTo(this.m_background, iTween.Hash(new object[]
		{
			"r",
			1f,
			"g",
			1f,
			"b",
			1f,
			"a",
			1f,
			"time",
			0.5f,
			"oncomplete",
			delegate(object newVal)
			{
				this.MimironPowerUp();
			}
		}));
	}

	// Token: 0x06006FFB RID: 28667 RVA: 0x0020E93D File Offset: 0x0020CB3D
	private void SetGlow(Material glowMat, float newVal, string colorVal = "_TintColor")
	{
		glowMat.SetColor(colorVal, Color.Lerp(this.m_clear, Color.white, newVal));
	}

	// Token: 0x06006FFC RID: 28668 RVA: 0x0020E958 File Offset: 0x0020CB58
	private void MimironPowerUp()
	{
		this.m_mimironElectricity.GetComponent<ParticleSystem>().Play();
		for (int i = 0; i < this.m_mechMinions.Count; i++)
		{
			GameObject gameObject = this.m_mechMinions[i].GetActor().gameObject;
			GameObject minionGlow = Object.Instantiate<GameObject>(this.m_minionGlow);
			if (!this.m_cleanup.ContainsKey(gameObject))
			{
				this.m_cleanup.Add(gameObject, new List<GameObject>());
			}
			this.m_cleanup[gameObject].Add(minionGlow);
			minionGlow.transform.parent = gameObject.transform;
			minionGlow.transform.localPosition = new Vector3(0f, 0.5f, 0f);
			float num = this.m_absorbTime / (float)this.m_mechMinions.Count * (float)i;
			minionGlow.GetComponent<Renderer>().material.SetColor("_TintColor", this.m_clear);
			SceneUtils.EnableRenderers(minionGlow, true);
			if (i < this.m_mechMinions.Count - 1)
			{
				iTween.ValueTo(minionGlow, iTween.Hash(new object[]
				{
					"from",
					0f,
					"to",
					1f,
					"time",
					this.m_glowTime,
					"delay",
					0.1f + num + this.m_sparkDelay,
					"onstart",
					delegate(object newVal)
					{
						SoundManager.Get().LoadAndPlay(this.m_perMinionSound);
					},
					"onupdate",
					delegate(object newVal)
					{
						this.SetGlow(minionGlow.GetComponent<Renderer>().material, (float)newVal, "_TintColor");
					}
				}));
				iTween.ValueTo(minionGlow, iTween.Hash(new object[]
				{
					"from",
					1f,
					"to",
					0f,
					"time",
					this.m_glowTime,
					"delay",
					0.1f + num + this.m_sparkDelay + this.m_glowTime,
					"onupdate",
					delegate(object newVal)
					{
						this.SetGlow(minionGlow.GetComponent<Renderer>().material, (float)newVal, "_TintColor");
					}
				}));
			}
			else
			{
				iTween.ValueTo(minionGlow, iTween.Hash(new object[]
				{
					"from",
					0f,
					"to",
					1f,
					"time",
					this.m_glowTime,
					"delay",
					0.1f + num + this.m_sparkDelay,
					"onstart",
					delegate(object newVal)
					{
						SoundManager.Get().LoadAndPlay(this.m_perMinionSound);
					},
					"onupdate",
					delegate(object newVal)
					{
						this.SetGlow(minionGlow.GetComponent<Renderer>().material, (float)newVal, "_TintColor");
					},
					"oncomplete",
					delegate(object newVal)
					{
						this.AbsorbMinions();
					}
				}));
				iTween.ValueTo(minionGlow, iTween.Hash(new object[]
				{
					"from",
					1f,
					"to",
					0f,
					"time",
					this.m_glowTime,
					"delay",
					0.1f + num + this.m_sparkDelay + this.m_glowTime,
					"onupdate",
					delegate(object newVal)
					{
						this.SetGlow(minionGlow.GetComponent<Renderer>().material, (float)newVal, "_TintColor");
					}
				}));
			}
		}
	}

	// Token: 0x06006FFD RID: 28669 RVA: 0x0020ED24 File Offset: 0x0020CF24
	private void AbsorbMinions()
	{
		Vector3 vector;
		vector..ctor(0f, -1f, 0f);
		for (int i = 0; i < this.m_mechMinions.Count; i++)
		{
			float num = this.m_absorbTime / (float)this.m_mechMinions.Count * (float)i;
			GameObject minion = this.m_mechMinions[i].GetActor().gameObject;
			if (i < this.m_mechMinions.Count - 1)
			{
				iTween.MoveTo(minion, iTween.Hash(new object[]
				{
					"position",
					this.m_highPosBone.transform.localPosition + vector,
					"easetype",
					iTween.EaseType.easeInOutSine,
					"delay",
					this.m_glowTime + num + this.m_sparkDelay,
					"time",
					0.5f,
					"oncomplete",
					delegate(object newVal)
					{
						this.MinionCleanup(minion);
					}
				}));
			}
			else
			{
				iTween.MoveTo(minion, iTween.Hash(new object[]
				{
					"position",
					this.m_highPosBone.transform.localPosition + vector,
					"easetype",
					iTween.EaseType.easeInOutSine,
					"delay",
					this.m_glowTime + num + this.m_sparkDelay,
					"time",
					0.5f,
					"oncomplete",
					delegate(object newVal)
					{
						this.MinionCleanup(minion);
						this.FlareMimiron();
					}
				}));
			}
		}
		this.m_isNegFlash = true;
		base.StartCoroutine(this.MimironNegativeFX());
	}

	// Token: 0x06006FFE RID: 28670 RVA: 0x0020EF08 File Offset: 0x0020D108
	private void FlareMimiron()
	{
		this.m_mimironGlow.GetComponent<Renderer>().material.SetColor("_TintColor", this.m_clear);
		this.m_mimironFlare.GetComponent<Renderer>().material.SetColor("_TintColor", this.m_clear);
		this.m_mimironGlow.SetActive(true);
		this.m_mimironFlare.SetActive(true);
		iTween.ValueTo(this.m_mimironGlow, iTween.Hash(new object[]
		{
			"from",
			0f,
			"to",
			0.7f,
			"time",
			0.3,
			"onupdate",
			delegate(object newVal)
			{
				this.SetGlow(this.m_mimironGlow.GetComponent<Renderer>().material, (float)newVal, "_TintColor");
			}
		}));
		iTween.ValueTo(this.m_mimironFlare, iTween.Hash(new object[]
		{
			"from",
			0f,
			"to",
			2.5f,
			"time",
			0.3f,
			"onupdate",
			delegate(object newVal)
			{
				this.SetGlow(this.m_mimironFlare.GetComponent<Renderer>().material, (float)newVal, "_Intensity");
			},
			"oncomplete",
			delegate(object newVal)
			{
				this.UnflareMimiron();
			}
		}));
	}

	// Token: 0x06006FFF RID: 28671 RVA: 0x0020F064 File Offset: 0x0020D264
	private void UnflareMimiron()
	{
		this.m_volt.SetDoNotSort(false);
		ZonePlay battlefieldZone = this.m_volt.GetController().GetBattlefieldZone();
		foreach (Card card in battlefieldZone.GetCards())
		{
			card.SetDoNotSort(false);
		}
		battlefieldZone.UpdateLayout();
		this.DestroyMinions();
		this.m_volt.GetActor().Show();
		this.m_mimironGlow.GetComponent<Renderer>().material.SetColor("_TintColor", this.m_clear);
		this.m_mimironFlare.GetComponent<Renderer>().material.SetColor("_TintColor", this.m_clear);
		this.m_mimironGlow.SetActive(true);
		this.m_mimironFlare.SetActive(true);
		iTween.ValueTo(this.m_mimironGlow, iTween.Hash(new object[]
		{
			"from",
			0.7f,
			"to",
			0f,
			"time",
			0.3,
			"onupdate",
			delegate(object newVal)
			{
				this.SetGlow(this.m_mimironGlow.GetComponent<Renderer>().material, (float)newVal, "_TintColor");
			}
		}));
		iTween.ValueTo(this.m_mimironFlare, iTween.Hash(new object[]
		{
			"from",
			2.5f,
			"to",
			0f,
			"time",
			0.3f,
			"onupdate",
			delegate(object newVal)
			{
				this.SetGlow(this.m_mimironFlare.GetComponent<Renderer>().material, (float)newVal, "_Intensity");
			},
			"oncomplete",
			delegate(object newVal)
			{
				this.FadeOutBackground();
			}
		}));
		this.m_isNegFlash = false;
		this.OnSpellFinished();
	}

	// Token: 0x06007000 RID: 28672 RVA: 0x0020F254 File Offset: 0x0020D454
	private void FadeOutBackground()
	{
		this.m_mimironGlow.SetActive(false);
		this.m_mimironFlare.SetActive(false);
		iTween.ColorTo(this.m_background, iTween.Hash(new object[]
		{
			"r",
			1f,
			"g",
			1f,
			"b",
			1f,
			"a",
			0f,
			"time",
			0.5f,
			"oncomplete",
			delegate(object newVal)
			{
				this.RaiseVolt();
			}
		}));
	}

	// Token: 0x06007001 RID: 28673 RVA: 0x0020F314 File Offset: 0x0020D514
	private void DestroyMinions()
	{
		foreach (Card card in this.m_mechMinions)
		{
			card.IgnoreDeath(false);
			card.SetDoNotSort(false);
			card.GetActor().Destroy();
		}
		this.m_mimiron.IgnoreDeath(false);
		this.m_mimiron.SetDoNotSort(false);
		this.m_mimiron.GetActor().Destroy();
	}

	// Token: 0x06007002 RID: 28674 RVA: 0x0020F3A8 File Offset: 0x0020D5A8
	private void RaiseVolt()
	{
		this.m_mimironElectricity.GetComponent<ParticleSystem>().Stop();
		Material material = this.m_background.GetComponent<Renderer>().material;
		material.SetColor("_Color", this.m_clear);
		this.m_background.SetActive(false);
		GameObject gameObject = this.m_volt.GetActor().gameObject;
		gameObject.transform.parent = this.m_voltParent;
		iTween.MoveTo(gameObject, iTween.Hash(new object[]
		{
			"position",
			gameObject.transform.localPosition + new Vector3(0f, 3f, 0f),
			"time",
			0.2f,
			"islocal",
			true,
			"oncomplete",
			delegate(object newVal)
			{
				this.DropV07tron();
			}
		}));
	}

	// Token: 0x06007003 RID: 28675 RVA: 0x0020F498 File Offset: 0x0020D698
	private void DropV07tron()
	{
		GameObject gameObject = this.m_volt.GetActor().gameObject;
		iTween.MoveTo(gameObject, iTween.Hash(new object[]
		{
			"position",
			Vector3.zero,
			"time",
			0.3f,
			"islocal",
			true
		}));
		this.Finish();
	}

	// Token: 0x06007004 RID: 28676 RVA: 0x0020F508 File Offset: 0x0020D708
	private void Finish()
	{
		this.m_volt = null;
		this.m_mimiron = null;
		this.m_mechMinions.Clear();
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
	}

	// Token: 0x0400591C RID: 22812
	public GameObject m_root;

	// Token: 0x0400591D RID: 22813
	public GameObject m_highPosBone;

	// Token: 0x0400591E RID: 22814
	public GameObject m_minionPosBone;

	// Token: 0x0400591F RID: 22815
	public GameObject m_background;

	// Token: 0x04005920 RID: 22816
	public GameObject m_minionElectricity;

	// Token: 0x04005921 RID: 22817
	public GameObject m_minionGlow;

	// Token: 0x04005922 RID: 22818
	public GameObject m_mimironNegative;

	// Token: 0x04005923 RID: 22819
	public GameObject m_mimironFlare;

	// Token: 0x04005924 RID: 22820
	public GameObject m_mimironGlow;

	// Token: 0x04005925 RID: 22821
	public GameObject m_mimironElectricity;

	// Token: 0x04005926 RID: 22822
	public Spell m_voltSpawnOverrideSpell;

	// Token: 0x04005927 RID: 22823
	public string m_perMinionSound;

	// Token: 0x04005928 RID: 22824
	public string[] m_startSounds;

	// Token: 0x04005929 RID: 22825
	private Card m_volt;

	// Token: 0x0400592A RID: 22826
	private Card m_mimiron;

	// Token: 0x0400592B RID: 22827
	private List<Card> m_mechMinions = new List<Card>();

	// Token: 0x0400592C RID: 22828
	private Transform m_voltParent;

	// Token: 0x0400592D RID: 22829
	private Color m_clear = new Color(1f, 1f, 1f, 0f);

	// Token: 0x0400592E RID: 22830
	private Map<GameObject, List<GameObject>> m_cleanup = new Map<GameObject, List<GameObject>>();

	// Token: 0x0400592F RID: 22831
	private bool m_isNegFlash;

	// Token: 0x04005930 RID: 22832
	private float m_flashDelay = 0.15f;

	// Token: 0x04005931 RID: 22833
	private float m_mimironHighTime = 1.5f;

	// Token: 0x04005932 RID: 22834
	private float m_minionHighTime = 2f;

	// Token: 0x04005933 RID: 22835
	private float m_sparkDelay = 0.3f;

	// Token: 0x04005934 RID: 22836
	private float m_absorbTime = 1f;

	// Token: 0x04005935 RID: 22837
	private float m_glowTime = 0.5f;

	// Token: 0x04005936 RID: 22838
	private PowerTaskList m_waitForTaskList;
}
