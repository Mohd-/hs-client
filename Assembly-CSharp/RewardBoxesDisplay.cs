using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007F6 RID: 2038
[CustomEditClass]
public class RewardBoxesDisplay : MonoBehaviour
{
	// Token: 0x06004F2A RID: 20266 RVA: 0x001783F0 File Offset: 0x001765F0
	private void Awake()
	{
		RewardBoxesDisplay.s_Instance = this;
		this.m_addRewardsToCacheValues = !Login.IsLoginSceneActive();
		this.m_InstancedObjects = new List<GameObject>();
		this.m_doneCallbacks = new List<Action>();
		CollectionManager.Get().RegisterAchievesCompletedListener(new CollectionManager.DelOnAchievesCompleted(this.OnCollectionAchievesCompleted));
		RenderUtils.SetAlpha(this.m_ClickCatcher, 0f);
	}

	// Token: 0x06004F2B RID: 20267 RVA: 0x0017844D File Offset: 0x0017664D
	private void Start()
	{
		if (this.m_RewardSets.m_RewardPackage != null)
		{
			this.m_RewardSets.m_RewardPackage.SetActive(false);
		}
	}

	// Token: 0x06004F2C RID: 20268 RVA: 0x00178476 File Offset: 0x00176676
	private void OnDisable()
	{
	}

	// Token: 0x06004F2D RID: 20269 RVA: 0x00178478 File Offset: 0x00176678
	private void OnDestroy()
	{
		this.CleanUp();
		this.m_destroyed = true;
	}

	// Token: 0x06004F2E RID: 20270 RVA: 0x00178487 File Offset: 0x00176687
	private void OnEnable()
	{
	}

	// Token: 0x06004F2F RID: 20271 RVA: 0x00178489 File Offset: 0x00176689
	public static RewardBoxesDisplay Get()
	{
		return RewardBoxesDisplay.s_Instance;
	}

	// Token: 0x06004F30 RID: 20272 RVA: 0x00178490 File Offset: 0x00176690
	public void SetRewards(List<RewardData> rewards)
	{
		this.m_Rewards = rewards;
	}

	// Token: 0x06004F31 RID: 20273 RVA: 0x00178499 File Offset: 0x00176699
	public void UseDarkeningClickCatcher(bool value)
	{
		this.m_useDarkeningClickCatcher = value;
	}

	// Token: 0x06004F32 RID: 20274 RVA: 0x001784A2 File Offset: 0x001766A2
	public void RegisterDoneCallback(Action action)
	{
		this.m_doneCallbacks.Add(action);
	}

	// Token: 0x06004F33 RID: 20275 RVA: 0x001784B0 File Offset: 0x001766B0
	public List<RewardBoxesDisplay.RewardPackageData> GetPackageData(int rewardCount)
	{
		for (int i = 0; i < this.m_RewardSets.m_RewardData.Count; i++)
		{
			if (this.m_RewardSets.m_RewardData[i].m_PackageData.Count == rewardCount)
			{
				return this.m_RewardSets.m_RewardData[i].m_PackageData;
			}
		}
		Debug.LogError("RewardBoxesDisplay: GetPackageData - no package data found with a reward count of " + rewardCount);
		return null;
	}

	// Token: 0x06004F34 RID: 20276 RVA: 0x0017852E File Offset: 0x0017672E
	public void SetLayer(GameLayer layer)
	{
		this.m_layer = layer;
		SceneUtils.SetLayer(base.gameObject, this.m_layer);
	}

	// Token: 0x06004F35 RID: 20277 RVA: 0x00178548 File Offset: 0x00176748
	public void ShowAlreadyOpenedRewards()
	{
		this.m_RewardPackages = this.GetPackageData(this.m_Rewards.Count);
		this.m_RewardObjects = new GameObject[this.m_Rewards.Count];
		this.FadeFullscreenEffectsIn();
		this.ShowOpenedRewards();
		this.AllDone();
	}

	// Token: 0x06004F36 RID: 20278 RVA: 0x00178594 File Offset: 0x00176794
	public void ShowOpenedRewards()
	{
		for (int i = 0; i < this.m_RewardPackages.Count; i++)
		{
			RewardBoxesDisplay.RewardPackageData rewardPackageData = this.m_RewardPackages[i];
			if (rewardPackageData.m_TargetBone == null)
			{
				Debug.LogWarning("RewardBoxesDisplay: AnimateRewards package target bone is null!");
				return;
			}
			if (i >= this.m_RewardObjects.Length)
			{
				Debug.LogWarning("RewardBoxesDisplay: AnimateRewards reward index exceeded!");
				return;
			}
			this.m_RewardObjects[i] = this.CreateRewardInstance(i, rewardPackageData.m_TargetBone.position, true);
		}
	}

	// Token: 0x06004F37 RID: 20279 RVA: 0x0017861C File Offset: 0x0017681C
	public void AnimateRewards()
	{
		int count = this.m_Rewards.Count;
		this.m_RewardPackages = this.GetPackageData(count);
		this.m_RewardObjects = new GameObject[count];
		for (int i = 0; i < this.m_RewardPackages.Count; i++)
		{
			RewardBoxesDisplay.RewardPackageData rewardPackageData = this.m_RewardPackages[i];
			if (rewardPackageData.m_TargetBone == null)
			{
				Debug.LogWarning("RewardBoxesDisplay: AnimateRewards package target bone is null!");
				return;
			}
			if (i >= this.m_RewardObjects.Length)
			{
				Debug.LogWarning("RewardBoxesDisplay: AnimateRewards reward index exceeded!");
				return;
			}
			this.m_RewardObjects[i] = this.CreateRewardInstance(i, rewardPackageData.m_TargetBone.position, false);
		}
		this.RewardPackageAnimation();
	}

	// Token: 0x06004F38 RID: 20280 RVA: 0x001786D0 File Offset: 0x001768D0
	public void OpenReward(int rewardIndex, Vector3 rewardPos)
	{
		if (rewardIndex >= this.m_RewardObjects.Length)
		{
			Debug.LogWarning("RewardBoxesDisplay: OpenReward reward index exceeded!");
			return;
		}
		GameObject gameObject = this.m_RewardObjects[rewardIndex];
		if (gameObject == null)
		{
			Debug.LogWarning("RewardBoxesDisplay: OpenReward object is null!");
			return;
		}
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
		if (this.CheckAllRewardsActive())
		{
			this.AllDone();
		}
	}

	// Token: 0x06004F39 RID: 20281 RVA: 0x0017873C File Offset: 0x0017693C
	private void RewardPackageAnimation()
	{
		if (this.m_RewardSets.m_RewardPackage == null)
		{
			Debug.LogWarning("RewardBoxesDisplay: missing Reward Package!");
			return;
		}
		this.FadeFullscreenEffectsIn();
		for (int i = 0; i < this.m_RewardPackages.Count; i++)
		{
			RewardBoxesDisplay.RewardPackageData rewardPackageData = this.m_RewardPackages[i];
			if (rewardPackageData.m_TargetBone == null || rewardPackageData.m_StartBone == null)
			{
				Debug.LogWarning("RewardBoxesDisplay: missing reward target bone!");
			}
			else
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.m_RewardSets.m_RewardPackage);
				TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, this.m_Root.transform);
				gameObject.transform.position = rewardPackageData.m_StartBone.position;
				gameObject.SetActive(true);
				this.m_InstancedObjects.Add(gameObject);
				Vector3 localScale = gameObject.transform.localScale;
				gameObject.transform.localScale = Vector3.zero;
				SceneUtils.EnableColliders(gameObject, false);
				iTween.ScaleTo(gameObject, iTween.Hash(new object[]
				{
					"scale",
					localScale,
					"time",
					this.m_RewardSets.m_AnimationTime,
					"delay",
					rewardPackageData.m_StartDelay,
					"easetype",
					iTween.EaseType.linear
				}));
				PlayMakerFSM component = gameObject.GetComponent<PlayMakerFSM>();
				if (component == null)
				{
					Debug.LogWarning("RewardBoxesDisplay: missing reward Playmaker FSM!");
				}
				else
				{
					if (!this.m_playBoxFlyoutSound)
					{
						component.FsmVariables.FindFsmBool("PlayFlyoutSound").Value = false;
					}
					RewardPackage component2 = gameObject.GetComponent<RewardPackage>();
					component2.m_RewardIndex = i;
					RewardBoxesDisplay.RewardBoxData rewardBoxData = new RewardBoxesDisplay.RewardBoxData();
					rewardBoxData.m_GameObject = gameObject;
					rewardBoxData.m_RewardPackage = component2;
					rewardBoxData.m_FSM = component;
					rewardBoxData.m_Index = i;
					iTween.MoveTo(gameObject, iTween.Hash(new object[]
					{
						"position",
						rewardPackageData.m_TargetBone.transform.position,
						"time",
						this.m_RewardSets.m_AnimationTime,
						"delay",
						rewardPackageData.m_StartDelay,
						"easetype",
						iTween.EaseType.linear,
						"onstarttarget",
						base.gameObject,
						"onstart",
						"RewardPackageOnStart",
						"onstartparams",
						rewardBoxData,
						"oncompletetarget",
						base.gameObject,
						"oncomplete",
						"RewardPackageOnComplete",
						"oncompleteparams",
						rewardBoxData
					}));
				}
			}
		}
	}

	// Token: 0x06004F3A RID: 20282 RVA: 0x001789FD File Offset: 0x00176BFD
	private void RewardPackageOnStart(RewardBoxesDisplay.RewardBoxData boxData)
	{
		boxData.m_FSM.SendEvent("Birth");
	}

	// Token: 0x06004F3B RID: 20283 RVA: 0x00178A0F File Offset: 0x00176C0F
	private void RewardPackageOnComplete(RewardBoxesDisplay.RewardBoxData boxData)
	{
		base.StartCoroutine(this.RewardPackageActivate(boxData));
	}

	// Token: 0x06004F3C RID: 20284 RVA: 0x00178A20 File Offset: 0x00176C20
	private IEnumerator RewardPackageActivate(RewardBoxesDisplay.RewardBoxData boxData)
	{
		yield return new WaitForSeconds(0.5f);
		SceneUtils.EnableColliders(boxData.m_GameObject, true);
		boxData.m_RewardPackage.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.RewardPackagePressed));
		yield break;
	}

	// Token: 0x06004F3D RID: 20285 RVA: 0x00178A49 File Offset: 0x00176C49
	private void RewardPackagePressed(UIEvent e)
	{
		Log.Kyle.Print("box clicked!", new object[0]);
	}

	// Token: 0x06004F3E RID: 20286 RVA: 0x00178A60 File Offset: 0x00176C60
	private GameObject CreateRewardInstance(int rewardIndex, Vector3 rewardPos, bool activeOnStart)
	{
		RewardData rewardData = this.m_Rewards[rewardIndex];
		GameObject gameObject = null;
		switch (rewardData.RewardType)
		{
		case Reward.Type.ARCANE_DUST:
		{
			gameObject = Object.Instantiate<GameObject>(this.m_RewardSets.m_RewardDust);
			TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, this.m_Root.transform);
			gameObject.transform.position = rewardPos;
			gameObject.SetActive(true);
			UberText componentInChildren = gameObject.GetComponentInChildren<UberText>();
			ArcaneDustRewardData arcaneDustRewardData = (ArcaneDustRewardData)rewardData;
			componentInChildren.Text = arcaneDustRewardData.Amount.ToString();
			gameObject.SetActive(activeOnStart);
			break;
		}
		case Reward.Type.BOOSTER_PACK:
		{
			BoosterPackRewardData boosterPackRewardData = rewardData as BoosterPackRewardData;
			int num = boosterPackRewardData.Id;
			if (num == 0)
			{
				num = 1;
				Debug.LogWarning("RewardBoxesDisplay - booster reward is not valid. ID = 0");
			}
			Log.Kyle.Print(string.Format("Booster DB ID: {0}", num), new object[0]);
			BoosterDbfRecord record = GameDbf.Booster.GetRecord(num);
			string arenaPrefab = record.ArenaPrefab;
			if (string.IsNullOrEmpty(arenaPrefab))
			{
				Debug.LogError(string.Format("RewardBoxesDisplay - no prefab found for booster {0}!", boosterPackRewardData.Id));
			}
			else
			{
				gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(arenaPrefab), true, false);
				TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, this.m_Root.transform);
				gameObject.transform.position = rewardPos;
				gameObject.SetActive(activeOnStart);
			}
			break;
		}
		case Reward.Type.CARD:
		{
			gameObject = Object.Instantiate<GameObject>(this.m_RewardSets.m_RewardCard);
			TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, this.m_Root.transform);
			gameObject.transform.position = rewardPos;
			gameObject.SetActive(true);
			CardRewardData cardData = (CardRewardData)rewardData;
			RewardCard componentInChildren2 = gameObject.GetComponentInChildren<RewardCard>();
			componentInChildren2.LoadCard(cardData, this.m_layer);
			gameObject.SetActive(activeOnStart);
			break;
		}
		case Reward.Type.CARD_BACK:
		{
			gameObject = Object.Instantiate<GameObject>(this.m_RewardSets.m_RewardCardBack);
			TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, this.m_Root.transform);
			gameObject.transform.position = rewardPos;
			gameObject.SetActive(true);
			CardBackRewardData cardbackData = (CardBackRewardData)rewardData;
			RewardCardBack componentInChildren3 = gameObject.GetComponentInChildren<RewardCardBack>();
			componentInChildren3.LoadCardBack(cardbackData, this.m_layer);
			gameObject.SetActive(activeOnStart);
			break;
		}
		case Reward.Type.GOLD:
		{
			gameObject = Object.Instantiate<GameObject>(this.m_RewardSets.m_RewardGold);
			TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, this.m_Root.transform);
			gameObject.transform.position = rewardPos;
			gameObject.SetActive(true);
			UberText componentInChildren4 = gameObject.GetComponentInChildren<UberText>();
			GoldRewardData goldRewardData = (GoldRewardData)rewardData;
			componentInChildren4.Text = goldRewardData.Amount.ToString();
			gameObject.SetActive(activeOnStart);
			break;
		}
		}
		if (gameObject == null)
		{
			Debug.LogWarning("RewardBoxesDisplay: Unable to create reward, object null!");
			return null;
		}
		if (rewardIndex >= this.m_RewardObjects.Length)
		{
			Debug.LogWarning("RewardBoxesDisplay: CreateRewardInstance reward index exceeded!");
			return null;
		}
		SceneUtils.SetLayer(gameObject, this.m_layer);
		this.m_RewardObjects[rewardIndex] = gameObject;
		this.m_InstancedObjects.Add(gameObject);
		return gameObject;
	}

	// Token: 0x06004F3F RID: 20287 RVA: 0x00178D6C File Offset: 0x00176F6C
	private void AllDone()
	{
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.m_RewardPackages.Count; i++)
		{
			RewardBoxesDisplay.RewardPackageData rewardPackageData = this.m_RewardPackages[i];
			vector += rewardPackageData.m_TargetBone.position;
		}
		this.m_DoneButton.transform.position = vector / (float)this.m_RewardPackages.Count;
		this.m_DoneButton.gameObject.SetActive(true);
		this.m_DoneButton.SetText(GameStrings.Get("GLOBAL_DONE"));
		Spell component = this.m_DoneButton.m_button.GetComponent<Spell>();
		component.AddFinishedCallback(new Spell.FinishedCallback(this.OnDoneButtonShown));
		component.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x06004F40 RID: 20288 RVA: 0x00178E2C File Offset: 0x0017702C
	private void OnDoneButtonShown(Spell spell, object userData)
	{
		this.m_doneButtonFinishedShown = true;
		this.m_DoneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x06004F41 RID: 20289 RVA: 0x00178E5F File Offset: 0x0017705F
	private void OnDoneButtonPressed(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x170005B3 RID: 1459
	// (get) Token: 0x06004F42 RID: 20290 RVA: 0x00178E67 File Offset: 0x00177067
	// (set) Token: 0x06004F43 RID: 20291 RVA: 0x00178E6F File Offset: 0x0017706F
	public bool IsClosing { get; private set; }

	// Token: 0x06004F44 RID: 20292 RVA: 0x00178E78 File Offset: 0x00177078
	public void Close()
	{
		this.IsClosing = true;
		if (this.m_doneButtonFinishedShown)
		{
			this.OnNavigateBack();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06004F45 RID: 20293 RVA: 0x00178EA4 File Offset: 0x001770A4
	private bool OnNavigateBack()
	{
		Debug.Log("navigating back!");
		if (!this.m_DoneButton.m_button.activeSelf)
		{
			return false;
		}
		foreach (GameObject gameObject in this.m_RewardObjects)
		{
			if (!(gameObject == null))
			{
				PlayMakerFSM component = gameObject.GetComponent<PlayMakerFSM>();
				if (component != null)
				{
					component.SendEvent("Death");
				}
				UberText[] componentsInChildren = gameObject.GetComponentsInChildren<UberText>();
				foreach (UberText uberText in componentsInChildren)
				{
					iTween.FadeTo(uberText.gameObject, iTween.Hash(new object[]
					{
						"alpha",
						0f,
						"time",
						0.8f,
						"includechildren",
						true,
						"easetype",
						iTween.EaseType.easeInOutCubic
					}));
				}
				RewardCard componentInChildren = gameObject.GetComponentInChildren<RewardCard>();
				if (componentInChildren != null)
				{
					componentInChildren.Death();
				}
			}
		}
		SceneUtils.EnableColliders(this.m_DoneButton.gameObject, false);
		this.m_DoneButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));
		Spell component2 = this.m_DoneButton.m_button.GetComponent<Spell>();
		component2.AddFinishedCallback(new Spell.FinishedCallback(this.OnDoneButtonHidden));
		component2.ActivateState(SpellStateType.DEATH);
		CollectionManager.Get().RemoveAchievesCompletedListener(new CollectionManager.DelOnAchievesCompleted(this.OnCollectionAchievesCompleted));
		if (this.m_addRewardsToCacheValues)
		{
			this.AddRewardsToCacheValues();
		}
		return true;
	}

	// Token: 0x06004F46 RID: 20294 RVA: 0x0017904C File Offset: 0x0017724C
	private void AddRewardsToCacheValues()
	{
		bool flag = false;
		foreach (RewardData rewardData in this.m_Rewards)
		{
			switch (rewardData.RewardType)
			{
			case Reward.Type.ARCANE_DUST:
			{
				ArcaneDustRewardData arcaneDustRewardData = (ArcaneDustRewardData)rewardData;
				NetCache.Get().OnArcaneDustBalanceChanged((long)arcaneDustRewardData.Amount);
				break;
			}
			case Reward.Type.CARD:
			{
				CardRewardData cardRewardData = (CardRewardData)rewardData;
				CollectionManager.Get().OnCardRewardOpened(cardRewardData.CardID, cardRewardData.Premium, cardRewardData.Count);
				break;
			}
			case Reward.Type.GOLD:
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		NetCache.Get().RefreshNetObject<NetCache.NetCacheGoldBalance>();
	}

	// Token: 0x06004F47 RID: 20295 RVA: 0x00179138 File Offset: 0x00177338
	private void OnDoneButtonHidden(Spell spell, object userData)
	{
		this.FadeFullscreenEffectsOut();
	}

	// Token: 0x06004F48 RID: 20296 RVA: 0x00179140 File Offset: 0x00177340
	private void FadeFullscreenEffectsIn()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			Debug.LogWarning("RewardBoxesDisplay: FullScreenFXMgr.Get() returned null!");
			return;
		}
		fullScreenFXMgr.SetBlurBrightness(0.85f);
		fullScreenFXMgr.SetBlurDesaturation(0f);
		fullScreenFXMgr.Vignette(0.4f, 0.5f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.Blur(1f, 0.5f, iTween.EaseType.easeOutCirc, null);
		if (this.m_useDarkeningClickCatcher)
		{
			iTween.FadeTo(this.m_ClickCatcher, 0.75f, 0.5f);
		}
	}

	// Token: 0x06004F49 RID: 20297 RVA: 0x001791C8 File Offset: 0x001773C8
	private void FadeFullscreenEffectsOut()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			Debug.LogWarning("RewardBoxesDisplay: FullScreenFXMgr.Get() returned null!");
			return;
		}
		fullScreenFXMgr.StopVignette(2f, iTween.EaseType.easeOutCirc, new FullScreenFXMgr.EffectListener(this.FadeFullscreenEffectsOutFinished));
		fullScreenFXMgr.StopBlur(2f, iTween.EaseType.easeOutCirc, null);
		if (this.m_useDarkeningClickCatcher)
		{
			iTween.FadeTo(this.m_ClickCatcher, 0f, 0.5f);
		}
	}

	// Token: 0x06004F4A RID: 20298 RVA: 0x0017923C File Offset: 0x0017743C
	private void FadeVignetteIn()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			Debug.LogWarning("RewardBoxesDisplay: FullScreenFXMgr.Get() returned null!");
			return;
		}
		fullScreenFXMgr.DisableBlur();
		fullScreenFXMgr.Vignette(1.4f, 1.5f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06004F4B RID: 20299 RVA: 0x00179280 File Offset: 0x00177480
	private void FadeFullscreenEffectsOutFinished()
	{
		foreach (Action action in this.m_doneCallbacks)
		{
			action.Invoke();
		}
		this.m_doneCallbacks.Clear();
		if (!this.m_destroyed)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06004F4C RID: 20300 RVA: 0x001792FC File Offset: 0x001774FC
	private bool CheckAllRewardsActive()
	{
		foreach (GameObject gameObject in this.m_RewardObjects)
		{
			if (!gameObject.activeSelf)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004F4D RID: 20301 RVA: 0x00179338 File Offset: 0x00177538
	private void CleanUp()
	{
		foreach (GameObject gameObject in this.m_InstancedObjects)
		{
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
		}
		this.FadeFullscreenEffectsOut();
		RewardBoxesDisplay.s_Instance = null;
	}

	// Token: 0x06004F4E RID: 20302 RVA: 0x001793AC File Offset: 0x001775AC
	public void DebugLogRewards()
	{
		Debug.Log("BOX REWARDS:");
		for (int i = 0; i < this.m_Rewards.Count; i++)
		{
			RewardData rewardData = this.m_Rewards[i];
			Debug.Log(string.Format("  reward {0}={1}", i, rewardData));
		}
	}

	// Token: 0x06004F4F RID: 20303 RVA: 0x00179402 File Offset: 0x00177602
	private void OnCollectionAchievesCompleted(List<Achievement> achievements)
	{
		this.m_completeAchievesToDisplay.AddRange(achievements);
		this.ShowCompleteAchieve(null);
	}

	// Token: 0x06004F50 RID: 20304 RVA: 0x00179418 File Offset: 0x00177618
	private void ShowCompleteAchieve(object userData)
	{
		if (this.m_completeAchievesToDisplay.Count == 0)
		{
			return;
		}
		Achievement quest = this.m_completeAchievesToDisplay[0];
		this.m_completeAchievesToDisplay.RemoveAt(0);
		QuestToast.ShowQuestToast(UserAttentionBlocker.NONE, new QuestToast.DelOnCloseQuestToast(this.ShowCompleteAchieve), true, quest, false);
	}

	// Token: 0x04003615 RID: 13845
	public const string DEFAULT_PREFAB = "RewardBoxes";

	// Token: 0x04003616 RID: 13846
	public bool m_playBoxFlyoutSound = true;

	// Token: 0x04003617 RID: 13847
	public GameObject m_Root;

	// Token: 0x04003618 RID: 13848
	public GameObject m_ClickCatcher;

	// Token: 0x04003619 RID: 13849
	[CustomEditField(Sections = "Reward Panel")]
	public NormalButton m_DoneButton;

	// Token: 0x0400361A RID: 13850
	public RewardBoxesDisplay.RewardSet m_RewardSets;

	// Token: 0x0400361B RID: 13851
	private List<Action> m_doneCallbacks;

	// Token: 0x0400361C RID: 13852
	private List<GameObject> m_InstancedObjects;

	// Token: 0x0400361D RID: 13853
	private GameObject[] m_RewardObjects;

	// Token: 0x0400361E RID: 13854
	private List<Achievement> m_completeAchievesToDisplay = new List<Achievement>();

	// Token: 0x0400361F RID: 13855
	private List<RewardBoxesDisplay.RewardPackageData> m_RewardPackages;

	// Token: 0x04003620 RID: 13856
	private GameLayer m_layer = GameLayer.IgnoreFullScreenEffects;

	// Token: 0x04003621 RID: 13857
	private bool m_useDarkeningClickCatcher;

	// Token: 0x04003622 RID: 13858
	private bool m_doneButtonFinishedShown;

	// Token: 0x04003623 RID: 13859
	private bool m_destroyed;

	// Token: 0x04003624 RID: 13860
	private bool m_addRewardsToCacheValues = true;

	// Token: 0x04003625 RID: 13861
	private List<RewardData> m_Rewards;

	// Token: 0x04003626 RID: 13862
	private static RewardBoxesDisplay s_Instance;

	// Token: 0x020007F7 RID: 2039
	[Serializable]
	public class RewardPackageData
	{
		// Token: 0x04003628 RID: 13864
		public Transform m_StartBone;

		// Token: 0x04003629 RID: 13865
		public Transform m_TargetBone;

		// Token: 0x0400362A RID: 13866
		public float m_StartDelay;
	}

	// Token: 0x020007F8 RID: 2040
	[Serializable]
	public class RewardSet
	{
		// Token: 0x0400362B RID: 13867
		public GameObject m_RewardPackage;

		// Token: 0x0400362C RID: 13868
		public float m_AnimationTime = 1f;

		// Token: 0x0400362D RID: 13869
		public GameObject m_RewardCard;

		// Token: 0x0400362E RID: 13870
		public GameObject m_RewardCardBack;

		// Token: 0x0400362F RID: 13871
		public GameObject m_RewardGold;

		// Token: 0x04003630 RID: 13872
		public GameObject m_RewardDust;

		// Token: 0x04003631 RID: 13873
		public List<RewardBoxesDisplay.BoxRewardData> m_RewardData;
	}

	// Token: 0x020007F9 RID: 2041
	[Serializable]
	public class BoxRewardData
	{
		// Token: 0x04003632 RID: 13874
		public List<RewardBoxesDisplay.RewardPackageData> m_PackageData;
	}

	// Token: 0x020007FA RID: 2042
	public class RewardBoxData
	{
		// Token: 0x04003633 RID: 13875
		public GameObject m_GameObject;

		// Token: 0x04003634 RID: 13876
		public RewardPackage m_RewardPackage;

		// Token: 0x04003635 RID: 13877
		public PlayMakerFSM m_FSM;

		// Token: 0x04003636 RID: 13878
		public int m_Index;
	}

	// Token: 0x020007FC RID: 2044
	public class RewardCardLoadData
	{
		// Token: 0x04003638 RID: 13880
		public EntityDef m_EntityDef;

		// Token: 0x04003639 RID: 13881
		public Transform m_ParentTransform;

		// Token: 0x0400363A RID: 13882
		public CardRewardData m_CardRewardData;
	}

	// Token: 0x020007FD RID: 2045
	private enum Events
	{
		// Token: 0x0400363C RID: 13884
		GVG_PROMOTION
	}
}
