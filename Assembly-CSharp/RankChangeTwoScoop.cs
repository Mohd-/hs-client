using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000823 RID: 2083
public class RankChangeTwoScoop : MonoBehaviour
{
	// Token: 0x0600502C RID: 20524 RVA: 0x0017C2A8 File Offset: 0x0017A4A8
	private void Awake()
	{
		PlatformDependentValue<float> val = new PlatformDependentValue<float>(PlatformCategory.Screen)
		{
			PC = 1.2f,
			Phone = 0.8f
		};
		float num = 1.25f * val;
		this.START_SCALE = new Vector3(0.01f, 0.01f, 0.01f);
		this.END_SCALE = new Vector3(num, num, num);
		this.AFTER_PUNCH_SCALE = new Vector3(val, val, val);
		for (int i = 0; i < 5; i++)
		{
			this.m_stars.Add((RankChangeStar)GameUtils.Instantiate(this.m_starPrefab, base.gameObject, false));
			this.m_chestStars.Add((RankChangeStar)GameUtils.Instantiate(this.m_starPrefab, base.gameObject, false));
		}
		for (int j = 0; j < this.m_chestStars.Count; j++)
		{
			this.m_chestStars[j].GetComponent<UberFloaty>().enabled = true;
			this.m_chestStars[j].gameObject.SetActive(false);
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_name.Width *= 1.2f;
		}
		this.m_winStreakParent.SetActive(false);
		this.m_medalContainer.SetActive(false);
		this.m_debugClickCatcher.gameObject.SetActive(false);
	}

	// Token: 0x0600502D RID: 20525 RVA: 0x0017C41C File Offset: 0x0017A61C
	private void OnDestroy()
	{
		foreach (RankChangeStar rankChangeStar in this.m_stars)
		{
			if (rankChangeStar.gameObject != null)
			{
				Object.Destroy(rankChangeStar.gameObject);
			}
		}
		if (EndGameScreen.Get() != null)
		{
			EndGameScreen.Get().m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.Hide));
		}
	}

	// Token: 0x0600502E RID: 20526 RVA: 0x0017C4B8 File Offset: 0x0017A6B8
	public void CheatRankUp(string[] args)
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i].ToLower() == "winstreak")
			{
				flag = true;
			}
			if (args[i].ToLower() == "chest")
			{
				flag2 = true;
			}
		}
		this.m_debugClickCatcher.gameObject.SetActive(true);
		this.m_debugClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.Hide));
		this.m_medalInfoTranslator = new MedalInfoTranslator();
		TranslatedMedalInfo translatedMedalInfo = new TranslatedMedalInfo();
		TranslatedMedalInfo translatedMedalInfo2 = new TranslatedMedalInfo();
		int num = 14;
		int num2 = 15;
		int earnedStars = 3;
		int earnedStars2 = 1;
		if (args.Length >= 2)
		{
			int.TryParse(args[0], ref num);
			int.TryParse(args[1], ref num2);
		}
		if (args.Length >= 4)
		{
			int.TryParse(args[2], ref earnedStars);
			int.TryParse(args[3], ref earnedStars2);
		}
		translatedMedalInfo.earnedStars = earnedStars;
		translatedMedalInfo.totalStars = 3;
		if (num <= 15)
		{
			translatedMedalInfo.totalStars = 4;
		}
		if (num <= 10)
		{
			translatedMedalInfo.totalStars = 5;
		}
		translatedMedalInfo.canLoseStars = (num <= 20);
		translatedMedalInfo.canLoseLevel = (num < 20);
		translatedMedalInfo.name = string.Format("Rank {0}", num);
		translatedMedalInfo.nextMedalName = string.Format("Rank {0}", num2);
		translatedMedalInfo.rank = num;
		translatedMedalInfo.textureName = string.Format("Medal_Ranked_{0}", num);
		translatedMedalInfo2.earnedStars = earnedStars2;
		translatedMedalInfo2.totalStars = 3;
		if (num2 <= 15)
		{
			translatedMedalInfo2.totalStars = 4;
		}
		if (num2 <= 10)
		{
			translatedMedalInfo2.totalStars = 5;
		}
		if (num2 == 0)
		{
			translatedMedalInfo2.legendIndex = 1337;
		}
		translatedMedalInfo2.canLoseStars = (num2 <= 20);
		translatedMedalInfo2.canLoseLevel = (num2 < 20);
		translatedMedalInfo2.name = string.Format("Rank {0}", num2);
		translatedMedalInfo2.nextMedalName = string.Format("Rank {0}", num2);
		translatedMedalInfo2.rank = num2;
		translatedMedalInfo2.textureName = string.Format("Medal_Ranked_{0}", num2);
		if (flag)
		{
			translatedMedalInfo2.winStreak = 3;
		}
		if (flag2)
		{
			translatedMedalInfo.bestRank = translatedMedalInfo.rank;
			translatedMedalInfo2.bestRank = translatedMedalInfo2.rank;
		}
		this.m_medalInfoTranslator.TestSetMedalInfo(translatedMedalInfo2, translatedMedalInfo);
	}

	// Token: 0x0600502F RID: 20527 RVA: 0x0017C724 File Offset: 0x0017A924
	public void Initialize(MedalInfoTranslator medalInfoTranslator, RankChangeTwoScoop.RankChangeClosed callback)
	{
		if (medalInfoTranslator != null)
		{
			this.m_medalInfoTranslator = medalInfoTranslator;
		}
		bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
		this.m_currMedalInfo = this.m_medalInfoTranslator.GetCurrentMedal(@bool);
		this.m_prevMedalInfo = this.m_medalInfoTranslator.GetPreviousMedal(@bool);
		this.m_validPrevMedal = this.m_medalInfoTranslator.IsPreviousMedalValid();
		this.m_closedCallback = callback;
		this.InitMedalsAndStars();
	}

	// Token: 0x06005030 RID: 20528 RVA: 0x0017C790 File Offset: 0x0017A990
	private void Show()
	{
		if (this.m_numTexturesLoading > 0)
		{
			return;
		}
		this.m_medalContainer.SetActive(true);
		AnimationUtil.ShowWithPunch(base.gameObject, this.START_SCALE, this.END_SCALE, this.AFTER_PUNCH_SCALE, "AnimateRankChange", true, null, null, null);
		SoundManager.Get().LoadAndPlay("rank_window_expand");
	}

	// Token: 0x06005031 RID: 20529 RVA: 0x0017C7EC File Offset: 0x0017A9EC
	public void AnimateRankChange()
	{
		float num = 0f;
		bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
		switch (this.m_medalInfoTranslator.GetChangeType(@bool))
		{
		case RankChangeType.RANK_UP:
			num = this.IncreaseStars(this.m_prevMedalInfo.earnedStars, this.m_prevMedalInfo.totalStars);
			if (this.m_medalInfoTranslator.ShowRewardChest(@bool))
			{
				base.StartCoroutine(this.PlayChestLevelUp(num + 0.6f));
			}
			else
			{
				base.StartCoroutine(this.PlayLevelUp(num + 0.6f));
			}
			break;
		case RankChangeType.RANK_DOWN:
			num = this.DecreaseStars(0, this.m_prevMedalInfo.earnedStars);
			base.StartCoroutine(this.PlayLevelDown(num + 0.2f));
			break;
		case RankChangeType.RANK_SAME:
			if (!this.m_currMedalInfo.IsLegendRank())
			{
				if (this.m_currMedalInfo.earnedStars > this.m_prevMedalInfo.earnedStars)
				{
					num = this.IncreaseStars(this.m_prevMedalInfo.earnedStars, this.m_currMedalInfo.earnedStars);
				}
				else
				{
					num = this.DecreaseStars(this.m_currMedalInfo.earnedStars, this.m_prevMedalInfo.earnedStars);
				}
			}
			else
			{
				if (this.m_currMedalInfo.legendIndex == 0)
				{
					this.m_legendIndex.gameObject.SetActive(false);
				}
				else
				{
					this.m_legendIndex.gameObject.SetActive(true);
				}
				this.UpdateLegendText(this.m_currMedalInfo.legendIndex);
			}
			break;
		}
		base.StartCoroutine(this.EnableHitboxOnAnimFinished(num));
	}

	// Token: 0x06005032 RID: 20530 RVA: 0x0017C990 File Offset: 0x0017AB90
	private void UpdateLegendText(int legendIndex)
	{
		if (legendIndex == -1)
		{
			this.m_legendIndex.Text = string.Empty;
		}
		else if (legendIndex == 0)
		{
			this.m_legendIndex.Text = string.Empty;
		}
		else
		{
			this.m_legendIndex.Text = legendIndex.ToString();
		}
	}

	// Token: 0x06005033 RID: 20531 RVA: 0x0017C9E8 File Offset: 0x0017ABE8
	private IEnumerator EnableHitboxOnAnimFinished(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (EndGameScreen.Get() != null)
		{
			EndGameScreen.Get().m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.Hide));
		}
		yield break;
	}

	// Token: 0x06005034 RID: 20532 RVA: 0x0017CA14 File Offset: 0x0017AC14
	private float IncreaseStars(int blinkToThisIndex, int burstToThisIndex)
	{
		if (!this.m_currMedalInfo.IsLegendRank())
		{
			int num2;
			if (this.m_prevMedalInfo.rank != this.m_currMedalInfo.rank)
			{
				int num = this.m_prevMedalInfo.totalStars - this.m_prevMedalInfo.earnedStars;
				int earnedStars = this.m_currMedalInfo.earnedStars;
				num2 = num + earnedStars;
			}
			else
			{
				num2 = this.m_currMedalInfo.earnedStars - this.m_prevMedalInfo.earnedStars;
			}
			if (this.m_currMedalInfo.winStreak >= 3 && num2 > 1)
			{
				this.m_winStreakParent.SetActive(true);
			}
		}
		float num3 = 0f;
		for (int i = 0; i < this.m_stars.Count; i++)
		{
			num3 = (float)(i + 1) * 0.2f;
			RankChangeStar rankChangeStar = this.m_stars[i];
			if (i < blinkToThisIndex)
			{
				rankChangeStar.Blink(num3);
			}
			else if (i < burstToThisIndex)
			{
				rankChangeStar.Burst(num3);
			}
		}
		return num3;
	}

	// Token: 0x06005035 RID: 20533 RVA: 0x0017CB20 File Offset: 0x0017AD20
	private float DecreaseStars(int lastWipeIndex, int firstWipeIndex)
	{
		bool flag = EndGameScreen.Get() is DefeatScreen;
		bool active = false;
		if (flag && this.m_validPrevMedal)
		{
			if (!this.m_currMedalInfo.canLoseStars)
			{
				active = true;
			}
			else if (this.m_currMedalInfo.IsHighestRankThatCannotBeLost() && this.m_currMedalInfo.earnedStars == 0 && this.m_prevMedalInfo.rank == this.m_currMedalInfo.rank && this.m_prevMedalInfo.earnedStars == 0)
			{
				active = true;
			}
		}
		this.m_scrubRankDesc.SetActive(active);
		float num = 0f;
		for (int i = this.m_stars.Count - 1; i >= 0; i--)
		{
			num = (float)(this.m_stars.Count - i) * 0.2f;
			RankChangeStar rankChangeStar = this.m_stars[i];
			if (rankChangeStar.gameObject.activeInHierarchy && i < firstWipeIndex && i >= lastWipeIndex)
			{
				rankChangeStar.Wipe(num);
			}
		}
		return num;
	}

	// Token: 0x06005036 RID: 20534 RVA: 0x0017CC30 File Offset: 0x0017AE30
	private void Hide(UIEvent e)
	{
		if (EndGameScreen.Get() != null)
		{
			EndGameScreen.Get().m_hitbox.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.Hide));
		}
		if (base.gameObject != null)
		{
			AnimationUtil.ScaleFade(base.gameObject, new Vector3(0.1f, 0.1f, 0.1f), "DestroyRankChange");
		}
		if (SoundManager.Get() != null)
		{
			SoundManager.Get().LoadAndPlay("rank_window_shrink");
		}
	}

	// Token: 0x06005037 RID: 20535 RVA: 0x0017CCBE File Offset: 0x0017AEBE
	private void DestroyRankChange()
	{
		if (this.m_closedCallback != null)
		{
			this.m_closedCallback();
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06005038 RID: 20536 RVA: 0x0017CCE4 File Offset: 0x0017AEE4
	private void InitStars(int numEarned, int numTotal, bool fadeIn = false)
	{
		int num = 0;
		List<Transform> list;
		if (numTotal % 2 == 0)
		{
			list = this.m_evenStarBones;
			if (numTotal == 2)
			{
				num = 1;
			}
		}
		else
		{
			list = this.m_oddStarBones;
			if (numTotal == 3)
			{
				num = 1;
			}
			else if (numTotal == 1)
			{
				num = 2;
			}
		}
		for (int i = 0; i < numTotal; i++)
		{
			RankChangeStar rankChangeStar = this.m_stars[i];
			rankChangeStar.gameObject.SetActive(true);
			rankChangeStar.transform.localScale = new Vector3(0.22f, 0.1f, 0.22f);
			rankChangeStar.transform.position = list[i + num].position;
			rankChangeStar.Reset();
			if (i >= numEarned)
			{
				rankChangeStar.BlackOut();
			}
			else
			{
				rankChangeStar.UnBlackOut();
			}
			if (fadeIn)
			{
				rankChangeStar.FadeIn();
			}
		}
		for (int j = numTotal; j < this.m_stars.Count; j++)
		{
			RankChangeStar rankChangeStar2 = this.m_stars[j];
			rankChangeStar2.gameObject.SetActive(false);
		}
	}

	// Token: 0x06005039 RID: 20537 RVA: 0x0017CDF8 File Offset: 0x0017AFF8
	private void OnTopTextureLoaded(string assetName, Object asset, object callbackData)
	{
		if (asset == null)
		{
			Debug.LogWarning(string.Format("RankChangeTwoScoop.OnTopTextureLoaded(): asset for {0} is null!", assetName));
			return;
		}
		Texture texture = asset as Texture;
		if (texture == null)
		{
			Debug.LogWarning(string.Format("RankChangeTwoScoop.OnTopTextureLoaded(): medalTexture for {0} is null (asset is not a texture)!", assetName));
			return;
		}
		this.m_rankMedalTop.GetComponent<Renderer>().material.mainTexture = texture;
		this.m_numTexturesLoading--;
		this.Show();
	}

	// Token: 0x0600503A RID: 20538 RVA: 0x0017CE70 File Offset: 0x0017B070
	private void OnBottomTextureLoaded(string assetName, Object asset, object callbackData)
	{
		if (asset == null)
		{
			Debug.LogWarning(string.Format("RankChangeTwoScoop.OnBottomTextureLoaded(): asset for {0} is null!", assetName));
			return;
		}
		Texture texture = asset as Texture;
		if (texture == null)
		{
			Debug.LogWarning(string.Format("RankChangeTwoScoop.OnBottomTextureLoaded(): medalTexture for {0} is null (asset is not a texture)!", assetName));
			return;
		}
		this.m_rankMedalBottom.GetComponent<Renderer>().material.mainTexture = texture;
		this.m_numTexturesLoading--;
		this.Show();
	}

	// Token: 0x0600503B RID: 20539 RVA: 0x0017CEE8 File Offset: 0x0017B0E8
	private IEnumerator PlayLevelUp(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (this.m_prevMedalInfo.rank == 1 && this.m_currMedalInfo.rank == 0)
		{
			this.m_mainFSM.SendEvent("Burst2");
			yield return base.StartCoroutine(this.LegendaryChanges());
			yield break;
		}
		this.m_mainFSM.SendEvent("Burst");
		yield return new WaitForSeconds(0.45f);
		if (this.m_currMedalInfo.earnedStars > 0)
		{
			this.m_prevMedalInfo.rank = this.m_currMedalInfo.rank;
			this.m_prevMedalInfo.earnedStars = 0;
			this.m_prevMedalInfo.totalStars = this.m_currMedalInfo.totalStars;
			this.LevelUpChanges();
			this.InitMedalsAndStars();
		}
		yield break;
	}

	// Token: 0x0600503C RID: 20540 RVA: 0x0017CF14 File Offset: 0x0017B114
	private IEnumerator PlayChestLevelUp(float delay)
	{
		base.gameObject.GetComponent<Animator>().enabled = true;
		this.m_winStreakParent.transform.localPosition = this.m_winStreakParent.transform.localPosition + new Vector3(0f, 0f, 0.558f);
		Animator animator = base.GetComponent<Animator>();
		yield return new WaitForSeconds(delay + 0.25f);
		if (this.m_prevMedalInfo.CanGetRankedRewardChest())
		{
			if (SoundManager.Get() != null)
			{
				SoundManager.Get().LoadAndPlay("tutorial_intro_hub_circle_360");
			}
			this.m_chestStars.RemoveRange(this.m_prevMedalInfo.totalStars - 1, this.m_chestStars.Count - this.m_prevMedalInfo.totalStars);
			for (int i = 0; i < this.m_chestStars.Count; i++)
			{
				TransformUtil.CopyLocal(this.m_chestStars[i], this.m_stars[i]);
				this.m_chestStars[i].gameObject.SetActive(true);
				this.m_stars[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < this.m_chestStars.Count; j++)
			{
				Vector3 away = this.m_stars[j].gameObject.transform.position - this.m_medalContainer.transform.position;
				away.Normalize();
				away += 10f * Vector3.forward;
				away.Normalize();
				Vector3 starPosition = this.m_stars[j].gameObject.transform.position + 0.4f * away;
				starPosition.y = this.m_rewardChest.m_rankNumber.transform.position.y;
				Hashtable moveArgs = iTween.Hash(new object[]
				{
					"islocal",
					false,
					"position",
					starPosition,
					"time",
					2f,
					"easetype",
					iTween.EaseType.easeOutElastic
				});
				this.m_chestStars[j].m_topGlowRenderer.enabled = true;
				iTween.ColorTo(this.m_chestStars[j].m_bottomGlowRenderer.gameObject, new Color(0.1f, 0.1f, 0.1f), 0.5f);
				Hashtable glowArgs = iTween.Hash(new object[]
				{
					"islocal",
					false,
					"position",
					starPosition,
					"color",
					new Color(0.36f, 0.25f, 0.25f),
					"time",
					5f,
					"easetype",
					iTween.EaseType.easeOutBack
				});
				iTween.ColorTo(this.m_chestStars[j].m_topGlowRenderer.gameObject, glowArgs);
				iTween.MoveTo(this.m_chestStars[j].gameObject, moveArgs);
				iTween.ScaleBy(this.m_chestStars[j].gameObject, 0.85f * Vector3.one, 0.5f);
				yield return new WaitForSeconds(0.1f);
			}
		}
		yield return new WaitForSeconds(0.4f);
		this.m_mainFSM.SendEvent("Wipe");
		while (this.m_mainFSM.ActiveStateName != "MedalWipeDone")
		{
			yield return new WaitForEndOfFrame();
		}
		this.m_chestRank = (this.m_prevMedalInfo.CanGetRankedRewardChest() ? this.m_prevMedalInfo.rank : this.m_currMedalInfo.rank);
		foreach (ChestVisual chestVisual in this.m_rewardChest.m_chests)
		{
			Log.EndOfGame.Print("setting chest visual inactive " + chestVisual.m_glowMesh, new object[0]);
			chestVisual.m_glowMesh.gameObject.SetActive(false);
		}
		this.m_rewardChest.GetChestVisualFromRank(this.m_chestRank).m_glowMesh.gameObject.SetActive(true);
		if (this.m_currMedalInfo.earnedStars > 0)
		{
			this.m_prevMedalInfo.rank = this.m_currMedalInfo.rank;
			this.m_prevMedalInfo.earnedStars = 0;
			this.m_prevMedalInfo.totalStars = this.m_currMedalInfo.totalStars;
		}
		this.LevelUpChanges();
		this.InitStars(this.m_prevMedalInfo.earnedStars, this.m_prevMedalInfo.totalStars, true);
		this.AnimateRankChange();
		yield return new WaitForSeconds(2f);
		if (!this.m_prevMedalInfo.CanGetRankedRewardChest())
		{
			animator.Play("MedalRanked_ChestChange_spawn");
		}
		else
		{
			animator.Play("MedalRanked_to_RewardChest");
		}
		yield break;
	}

	// Token: 0x0600503D RID: 20541 RVA: 0x0017CF40 File Offset: 0x0017B140
	private void DespawnOldStars()
	{
		this.m_winStreakParent.SetActive(false);
		for (int i = 0; i < this.m_stars.Count; i++)
		{
			if (this.m_stars[i].gameObject.activeSelf)
			{
				this.m_stars[i].GetComponent<PlayMakerFSM>().SendEvent("DeSpawn");
			}
		}
	}

	// Token: 0x0600503E RID: 20542 RVA: 0x0017CFAB File Offset: 0x0017B1AB
	private void StartChestGlow()
	{
		if (SoundManager.Get() != null)
		{
			SoundManager.Get().LoadAndPlay("tutorial_intro_box_opens");
		}
	}

	// Token: 0x0600503F RID: 20543 RVA: 0x0017CFCC File Offset: 0x0017B1CC
	private IEnumerator UpdateToCurrentChestCoroutine()
	{
		Animator animator = base.GetComponent<Animator>();
		this.m_rewardChest.SetRank(this.m_chestRank);
		if (!this.m_prevMedalInfo.CanGetRankedRewardChest())
		{
			yield break;
		}
		yield return new WaitForSeconds(0.8f);
		string levelUpAnimation = this.m_rewardChest.GetChestVisualFromRank(this.m_currMedalInfo.bestRank).levelUpAnimation;
		animator.Play(levelUpAnimation);
		Log.EndOfGame.Print("playing level up animation " + levelUpAnimation, new object[0]);
		for (int i = 0; i < this.m_chestStars.Count; i++)
		{
			RankChangeStar star = this.m_chestStars[this.m_chestStars.Count - 1 - i];
			object[] array = new object[12];
			array[0] = "islocal";
			array[1] = false;
			array[2] = "position";
			array[3] = this.m_rewardChest.m_rankNumber.transform.position;
			array[4] = "time";
			array[5] = 0.5f;
			array[6] = "easetype";
			array[7] = iTween.EaseType.easeInBack;
			array[8] = "oncompleteparams";
			array[9] = star;
			array[10] = "oncomplete";
			array[11] = delegate(object s)
			{
				((RankChangeStar)s).gameObject.SetActive(false);
			};
			Hashtable moveArgs = iTween.Hash(array);
			base.StartCoroutine(this.WaitAndPlayRankedStarImpact(0.5f));
			iTween.MoveTo(star.gameObject, moveArgs);
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x06005040 RID: 20544 RVA: 0x0017CFE8 File Offset: 0x0017B1E8
	private IEnumerator WaitAndPlayRankedStarImpact(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (SoundManager.Get() != null)
		{
			SoundManager.Get().LoadAndPlay("rank_star_gain");
		}
		yield break;
	}

	// Token: 0x06005041 RID: 20545 RVA: 0x0017D00A File Offset: 0x0017B20A
	private void UpdateToCurrentChest()
	{
		base.StartCoroutine(this.UpdateToCurrentChestCoroutine());
	}

	// Token: 0x06005042 RID: 20546 RVA: 0x0017D019 File Offset: 0x0017B219
	private void UpdateChestAfterLevelUp()
	{
		if (!this.m_rewardChest.DoesChestVisualChange(this.m_chestRank, this.m_currMedalInfo.bestRank))
		{
			this.UpdateToFinalChest();
		}
	}

	// Token: 0x06005043 RID: 20547 RVA: 0x0017D044 File Offset: 0x0017B244
	private void UpdateToFinalChest()
	{
		if (SoundManager.Get() != null && this.m_prevMedalInfo.CanGetRankedRewardChest())
		{
			SoundManager.Get().LoadAndPlay("level_up");
		}
		Log.EndOfGame.Print("Updating to final chest..", new object[0]);
		this.m_rewardChest.SetRank(this.m_currMedalInfo.bestRank);
		this.m_rewardChestInstructions.gameObject.SetActive(true);
		this.m_rewardChestInstructions.TextAlpha = 0f;
		iTween.FadeTo(this.m_rewardChestInstructions.gameObject, 1f, 1f);
	}

	// Token: 0x06005044 RID: 20548 RVA: 0x0017D0E8 File Offset: 0x0017B2E8
	private void PlayChestChange()
	{
		if (this.m_prevMedalInfo.CanGetRankedRewardChest() && this.m_rewardChest.DoesChestVisualChange(this.m_chestRank, this.m_currMedalInfo.bestRank))
		{
			this.m_rewardChest.GetChestVisualFromRank(this.m_currMedalInfo.bestRank).m_glowMesh.gameObject.SetActive(true);
			Animator component = base.GetComponent<Animator>();
			string chestChangeAnimation = this.m_rewardChest.GetChestVisualFromRank(this.m_currMedalInfo.bestRank).chestChangeAnimation;
			Log.EndOfGame.Print("playing chest change animation " + chestChangeAnimation, new object[0]);
			component.Play(chestChangeAnimation);
		}
	}

	// Token: 0x06005045 RID: 20549 RVA: 0x0017D194 File Offset: 0x0017B394
	private IEnumerator PlayLevelDown(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.m_mainFSM.SendEvent("Wipe");
		if (this.m_currMedalInfo.earnedStars <= this.m_currMedalInfo.totalStars)
		{
			this.m_prevMedalInfo.rank = this.m_currMedalInfo.rank;
			this.m_prevMedalInfo.earnedStars = this.m_currMedalInfo.totalStars;
			this.m_prevMedalInfo.totalStars = this.m_currMedalInfo.totalStars;
			yield return new WaitForSeconds(1.5f);
			this.m_name.Text = this.m_currMedalInfo.name;
			this.m_rankNumberTop.Text = this.m_currMedalInfo.rank.ToString();
			this.InitStars(this.m_currMedalInfo.earnedStars, this.m_currMedalInfo.totalStars, false);
		}
		yield break;
	}

	// Token: 0x06005046 RID: 20550 RVA: 0x0017D1C0 File Offset: 0x0017B3C0
	private void LevelUpChanges()
	{
		this.m_rankMedalTop.GetComponent<Renderer>().material.mainTexture = this.m_rankMedalBottom.GetComponent<Renderer>().material.mainTexture;
		this.m_rankNumberTop.Text = this.m_currMedalInfo.rank.ToString();
		this.m_name.Text = this.m_currMedalInfo.name;
		if (Gameplay.Get() != null)
		{
			Gameplay.Get().UpdateFriendlySideMedalChange(this.m_medalInfoTranslator);
		}
	}

	// Token: 0x06005047 RID: 20551 RVA: 0x0017D248 File Offset: 0x0017B448
	private IEnumerator LegendaryChanges()
	{
		this.m_legendIndex.gameObject.SetActive(true);
		this.m_legendIndex.Text = string.Empty;
		yield return new WaitForSeconds(0.3f);
		this.m_rankMedalTop.GetComponent<Renderer>().material = this.m_legendaryMaterial;
		this.UpdateLegendText(this.m_currMedalInfo.legendIndex);
		this.m_name.Text = this.m_currMedalInfo.name;
		this.m_bannerBottom.SetActive(false);
		foreach (RankChangeStar star in this.m_stars)
		{
			star.gameObject.SetActive(false);
		}
		if (Gameplay.Get() != null)
		{
			Gameplay.Get().UpdateFriendlySideMedalChange(this.m_medalInfoTranslator);
		}
		yield break;
	}

	// Token: 0x06005048 RID: 20552 RVA: 0x0017D264 File Offset: 0x0017B464
	private void InitMedalsAndStars()
	{
		bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
		this.m_wildFlair.SetActive(@bool);
		switch (this.m_medalInfoTranslator.GetChangeType(@bool))
		{
		case RankChangeType.RANK_UP:
			this.m_rankNumberTop.Text = this.m_prevMedalInfo.rank.ToString();
			this.m_name.Text = this.m_prevMedalInfo.name;
			this.m_numTexturesLoading = 2;
			AssetLoader.Get().LoadTexture(this.m_prevMedalInfo.textureName, new AssetLoader.ObjectCallback(this.OnTopTextureLoaded), null, false);
			AssetLoader.Get().LoadTexture(this.m_currMedalInfo.textureName, new AssetLoader.ObjectCallback(this.OnBottomTextureLoaded), null, false);
			this.InitStars(this.m_prevMedalInfo.earnedStars, this.m_prevMedalInfo.totalStars, false);
			break;
		case RankChangeType.RANK_DOWN:
			this.m_rankNumberBottom.Text = this.m_currMedalInfo.rank.ToString();
			this.m_rankNumberTop.Text = this.m_prevMedalInfo.rank.ToString();
			this.m_name.Text = this.m_prevMedalInfo.name;
			this.m_numTexturesLoading = 2;
			AssetLoader.Get().LoadTexture(this.m_prevMedalInfo.textureName, new AssetLoader.ObjectCallback(this.OnTopTextureLoaded), null, false);
			AssetLoader.Get().LoadTexture(this.m_currMedalInfo.textureName, new AssetLoader.ObjectCallback(this.OnBottomTextureLoaded), null, false);
			this.InitStars(this.m_prevMedalInfo.earnedStars, this.m_prevMedalInfo.totalStars, false);
			break;
		case RankChangeType.RANK_SAME:
			if (this.m_currMedalInfo.rank == 0)
			{
				this.m_bannerTop.SetActive(false);
				this.m_bannerBottom.SetActive(false);
				this.m_rankMedalBottom.SetActive(false);
			}
			else
			{
				this.m_rankNumberTop.Text = this.m_currMedalInfo.rank.ToString();
				this.m_bannerBottom.SetActive(false);
				this.m_rankNumberBottom.gameObject.SetActive(false);
			}
			this.m_name.Text = this.m_currMedalInfo.name;
			this.m_numTexturesLoading = 1;
			AssetLoader.Get().LoadTexture(this.m_currMedalInfo.textureName, new AssetLoader.ObjectCallback(this.OnTopTextureLoaded), null, false);
			this.InitStars(this.m_prevMedalInfo.earnedStars, this.m_prevMedalInfo.totalStars, false);
			break;
		}
	}

	// Token: 0x040036DC RID: 14044
	private const float STAR_ACTION_DELAY = 0.2f;

	// Token: 0x040036DD RID: 14045
	public GameObject m_medalContainer;

	// Token: 0x040036DE RID: 14046
	public GameObject m_bannerTop;

	// Token: 0x040036DF RID: 14047
	public UberText m_rankNumberTop;

	// Token: 0x040036E0 RID: 14048
	public GameObject m_bannerBottom;

	// Token: 0x040036E1 RID: 14049
	public UberText m_rankNumberBottom;

	// Token: 0x040036E2 RID: 14050
	public UberText m_legendIndex;

	// Token: 0x040036E3 RID: 14051
	public GameObject m_rankMedalTop;

	// Token: 0x040036E4 RID: 14052
	public GameObject m_rankMedalBottom;

	// Token: 0x040036E5 RID: 14053
	public GameObject m_wildFlair;

	// Token: 0x040036E6 RID: 14054
	public UberText m_name;

	// Token: 0x040036E7 RID: 14055
	public Material m_legendaryMaterial;

	// Token: 0x040036E8 RID: 14056
	public RankChangeStar m_starPrefab;

	// Token: 0x040036E9 RID: 14057
	public PlayMakerFSM m_mainFSM;

	// Token: 0x040036EA RID: 14058
	public List<Transform> m_evenStarBones;

	// Token: 0x040036EB RID: 14059
	public List<Transform> m_oddStarBones;

	// Token: 0x040036EC RID: 14060
	public GameObject m_winStreakParent;

	// Token: 0x040036ED RID: 14061
	public GameObject m_scrubRankDesc;

	// Token: 0x040036EE RID: 14062
	public PegUIElement m_debugClickCatcher;

	// Token: 0x040036EF RID: 14063
	public RankedRewardChest m_rewardChest;

	// Token: 0x040036F0 RID: 14064
	public MeshRenderer m_rewardChestGlow;

	// Token: 0x040036F1 RID: 14065
	public UberText m_rewardChestInstructions;

	// Token: 0x040036F2 RID: 14066
	private List<RankChangeStar> m_stars = new List<RankChangeStar>();

	// Token: 0x040036F3 RID: 14067
	private List<RankChangeStar> m_chestStars = new List<RankChangeStar>();

	// Token: 0x040036F4 RID: 14068
	private MedalInfoTranslator m_medalInfoTranslator;

	// Token: 0x040036F5 RID: 14069
	private TranslatedMedalInfo m_currMedalInfo;

	// Token: 0x040036F6 RID: 14070
	private TranslatedMedalInfo m_prevMedalInfo;

	// Token: 0x040036F7 RID: 14071
	private bool m_validPrevMedal;

	// Token: 0x040036F8 RID: 14072
	private int m_numTexturesLoading;

	// Token: 0x040036F9 RID: 14073
	private int m_chestRank;

	// Token: 0x040036FA RID: 14074
	private RankChangeTwoScoop.RankChangeClosed m_closedCallback;

	// Token: 0x040036FB RID: 14075
	protected Vector3 START_SCALE;

	// Token: 0x040036FC RID: 14076
	protected Vector3 END_SCALE;

	// Token: 0x040036FD RID: 14077
	protected Vector3 AFTER_PUNCH_SCALE;

	// Token: 0x02000824 RID: 2084
	// (Invoke) Token: 0x0600504A RID: 20554
	public delegate void RankChangeClosed();
}
