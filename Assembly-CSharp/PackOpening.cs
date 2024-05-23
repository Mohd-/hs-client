using System;
using System.Collections;
using System.Collections.Generic;
using bgs;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class PackOpening : MonoBehaviour
{
	// Token: 0x06001FA8 RID: 8104 RVA: 0x0009AB80 File Offset: 0x00098D80
	private void Awake()
	{
		PackOpening.s_instance = this;
		if (UniversalInputManager.UsePhoneUI)
		{
			AssetLoader.Get().LoadGameObject("PackOpeningCardFX_Phone", new AssetLoader.GameObjectCallback(this.OnPackOpeningFXLoaded), null, false);
		}
		else
		{
			AssetLoader.Get().LoadGameObject("PackOpeningCardFX", new AssetLoader.GameObjectCallback(this.OnPackOpeningFXLoaded), null, false);
		}
		this.InitializeNet();
		this.InitializeUI();
		Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
		this.m_StoreButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnStoreButtonReleased));
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x06001FA9 RID: 8105 RVA: 0x0009AC2F File Offset: 0x00098E2F
	private void Update()
	{
		this.UpdateDraggedPack();
	}

	// Token: 0x06001FAA RID: 8106 RVA: 0x0009AC38 File Offset: 0x00098E38
	private void OnDestroy()
	{
		if (this.m_draggedPack != null && PegCursor.Get() != null)
		{
			PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
		}
		this.ShutdownNet();
		PackOpening.s_instance = null;
	}

	// Token: 0x06001FAB RID: 8107 RVA: 0x0009AC7D File Offset: 0x00098E7D
	public static PackOpening Get()
	{
		return PackOpening.s_instance;
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x0009AC84 File Offset: 0x00098E84
	public GameObject GetPackOpeningCardEffects()
	{
		return this.m_PackOpeningCardFX;
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x0009AC8C File Offset: 0x00098E8C
	public bool HandleKeyboardInput()
	{
		if (Input.GetKeyUp(32))
		{
			if (this.CanOpenPackAutomatically())
			{
				this.m_autoOpenPending = true;
				this.m_director.FinishPackOpen();
				base.StartCoroutine(this.OpenNextPackWhenReady());
			}
			return true;
		}
		return false;
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x0009ACD3 File Offset: 0x00098ED3
	private void NotifySceneLoadedWhenReady()
	{
		if (!this.m_waitingForInitialNetData)
		{
			return;
		}
		this.m_waitingForInitialNetData = false;
		SceneMgr.Get().NotifySceneLoaded();
	}

	// Token: 0x06001FAF RID: 8111 RVA: 0x0009ACF2 File Offset: 0x00098EF2
	private void OnBoxTransitionFinished(object userData)
	{
		Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
		this.Show();
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x0009AD14 File Offset: 0x00098F14
	private void Show()
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_shown = true;
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.PACKOPENING
		});
		if (!Options.Get().GetBool(Option.HAS_SEEN_PACK_OPENING, false))
		{
			NetCache.NetCacheBoosters netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
			bool flag = netObject != null && netObject.GetTotalNumBoosters() > 0;
			if (flag)
			{
				Options.Get().SetBool(Option.HAS_SEEN_PACK_OPENING, true);
			}
		}
		MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_PackOpening);
		this.CreateDirector();
		BnetBar.Get().m_currencyFrame.RefreshContents();
		if (NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>().BoosterStacks.Count < 2)
		{
			this.ShowHintOnUnopenedPack();
		}
		this.UpdateUIEvents();
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x0009ADDC File Offset: 0x00098FDC
	private void Hide()
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		this.DestroyHint();
		this.m_StoreButton.Unload();
		this.m_InputBlocker.SetActive(false);
		this.UnregisterUIEvents();
		this.ShutdownNet();
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
	}

	// Token: 0x06001FB2 RID: 8114 RVA: 0x0009AE30 File Offset: 0x00099030
	private bool OnNavigateBack()
	{
		if (!this.m_enableBackButton || this.m_InputBlocker.activeSelf)
		{
			return false;
		}
		this.Hide();
		return true;
	}

	// Token: 0x06001FB3 RID: 8115 RVA: 0x0009AE64 File Offset: 0x00099064
	private void InitializeNet()
	{
		this.m_waitingForInitialNetData = true;
		NetCache.Get().RegisterScreenPackOpening(new NetCache.NetCacheCallback(this.OnNetDataReceived), new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
		Network.Get().RegisterNetHandler(226, new Network.NetHandler(this.OnBoosterOpened), null);
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x0009AEBC File Offset: 0x000990BC
	private void ShutdownNet()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetDataReceived));
		Network.Get().RemoveNetHandler(226, new Network.NetHandler(this.OnBoosterOpened));
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x0009AF00 File Offset: 0x00099100
	private void OnNetDataReceived()
	{
		this.NotifySceneLoadedWhenReady();
		this.UpdatePacks();
		this.UpdateUIEvents();
	}

	// Token: 0x06001FB6 RID: 8118 RVA: 0x0009AF14 File Offset: 0x00099114
	private void UpdatePacks()
	{
		NetCache.NetCacheBoosters netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
		if (netObject == null)
		{
			Debug.LogError(string.Format("PackOpening.UpdatePacks() - boosters are null", new object[0]));
			return;
		}
		foreach (NetCache.BoosterStack boosterStack in netObject.BoosterStacks)
		{
			int id = boosterStack.Id;
			if (this.m_unopenedPacks.ContainsKey(id) && this.m_unopenedPacks[id] != null)
			{
				if (netObject.GetBoosterStack(id) == null)
				{
					Object.Destroy(this.m_unopenedPacks[id]);
					this.m_unopenedPacks[id] = null;
				}
				else
				{
					this.UpdatePack(this.m_unopenedPacks[id], netObject.GetBoosterStack(id));
				}
			}
			else if (netObject.GetBoosterStack(id) != null)
			{
				if (!this.m_unopenedPacksLoading.ContainsKey(id) || !this.m_unopenedPacksLoading[id])
				{
					this.m_unopenedPacksLoading[id] = true;
					BoosterDbfRecord record = GameDbf.Booster.GetRecord(id);
					string text = FileUtils.GameAssetPathToName(record.PackOpeningPrefab);
					if (string.IsNullOrEmpty(text))
					{
						Debug.LogError(string.Format("PackOpening.UpdatePacks() - no prefab found for booster {0}!", id));
					}
					else
					{
						AssetLoader.Get().LoadActor(text, new AssetLoader.GameObjectCallback(this.OnUnopenedPackLoaded), boosterStack, false);
					}
				}
			}
		}
		this.LayoutPacks(false);
	}

	// Token: 0x06001FB7 RID: 8119 RVA: 0x0009B0C0 File Offset: 0x000992C0
	private void OnBoosterOpened()
	{
		List<NetCache.BoosterCard> cards = Network.OpenedBooster();
		this.m_director.OnBoosterOpened(cards);
	}

	// Token: 0x06001FB8 RID: 8120 RVA: 0x0009B0E0 File Offset: 0x000992E0
	private void CreateDirector()
	{
		GameObject gameObject = this.m_DirectorPrefab.gameObject;
		GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
		this.m_director = gameObject2.GetComponent<PackOpeningDirector>();
		gameObject2.transform.parent = base.transform;
		TransformUtil.CopyWorld(this.m_director, this.m_Bones.m_Director);
	}

	// Token: 0x06001FB9 RID: 8121 RVA: 0x0009B134 File Offset: 0x00099334
	private void PickUpBooster()
	{
		UnopenedPack creatorPack = this.m_draggedPack.GetCreatorPack();
		creatorPack.RemoveBooster();
		this.m_draggedPack.SetBoosterStack(new NetCache.BoosterStack
		{
			Id = creatorPack.GetBoosterStack().Id,
			Count = 1
		});
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x0009B180 File Offset: 0x00099380
	private void OpenBooster(UnopenedPack pack)
	{
		int num = 1;
		if (!GameUtils.IsFakePackOpeningEnabled())
		{
			num = pack.GetBoosterStack().Id;
			Network.OpenBooster(num);
		}
		this.m_InputBlocker.SetActive(true);
		this.m_director.AddFinishedListener(new PackOpeningDirector.FinishedCallback(this.OnDirectorFinished));
		this.m_director.Play(num);
		this.m_lastOpenedBoosterId = num;
		BnetBar.Get().m_currencyFrame.HideTemporarily();
		if (GameUtils.IsFakePackOpeningEnabled())
		{
			base.StartCoroutine(this.OnFakeBoosterOpened());
		}
		this.m_UnopenedPackScroller.Pause(true);
	}

	// Token: 0x06001FBB RID: 8123 RVA: 0x0009B214 File Offset: 0x00099414
	private IEnumerator OnFakeBoosterOpened()
	{
		float fakeNetDelay = Random.Range(0f, 1f);
		yield return new WaitForSeconds(fakeNetDelay);
		List<NetCache.BoosterCard> cards = new List<NetCache.BoosterCard>();
		NetCache.BoosterCard boosterCard = new NetCache.BoosterCard();
		boosterCard.Def.Name = "CS1_042";
		boosterCard.Def.Premium = TAG_PREMIUM.NORMAL;
		cards.Add(boosterCard);
		boosterCard = new NetCache.BoosterCard();
		boosterCard.Def.Name = "CS1_129";
		boosterCard.Def.Premium = TAG_PREMIUM.NORMAL;
		cards.Add(boosterCard);
		boosterCard = new NetCache.BoosterCard();
		boosterCard.Def.Name = "EX1_050";
		boosterCard.Def.Premium = TAG_PREMIUM.NORMAL;
		cards.Add(boosterCard);
		boosterCard = new NetCache.BoosterCard();
		boosterCard.Def.Name = "EX1_105";
		boosterCard.Def.Premium = TAG_PREMIUM.NORMAL;
		cards.Add(boosterCard);
		boosterCard = new NetCache.BoosterCard();
		boosterCard.Def.Name = "EX1_350";
		boosterCard.Def.Premium = TAG_PREMIUM.NORMAL;
		cards.Add(boosterCard);
		this.m_director.OnBoosterOpened(cards);
		yield break;
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x0009B230 File Offset: 0x00099430
	private void PutBackBooster()
	{
		UnopenedPack creatorPack = this.m_draggedPack.GetCreatorPack();
		this.m_draggedPack.RemoveBooster();
		creatorPack.AddBooster();
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x0009B25A File Offset: 0x0009945A
	private void UpdatePack(UnopenedPack pack, NetCache.BoosterStack boosterStack)
	{
		pack.SetBoosterStack(boosterStack);
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x0009B264 File Offset: 0x00099464
	private void OnUnopenedPackLoaded(string name, GameObject go, object userData)
	{
		NetCache.BoosterStack boosterStack = (NetCache.BoosterStack)userData;
		int id = boosterStack.Id;
		this.m_unopenedPacksLoading[id] = false;
		if (go == null)
		{
			Debug.LogError(string.Format("PackOpening.OnUnopenedPackLoaded() - FAILED to load {0}", name));
			return;
		}
		UnopenedPack component = go.GetComponent<UnopenedPack>();
		go.SetActive(false);
		if (component == null)
		{
			Debug.LogError(string.Format("PackOpening.OnUnopenedPackLoaded() - asset {0} did not have a {1} script on it", name, typeof(UnopenedPack)));
			return;
		}
		this.m_unopenedPacks.Add(id, component);
		component.gameObject.SetActive(true);
		GameUtils.SetParent(component, this.m_UnopenedPackContainer, false);
		component.transform.localScale = Vector3.one;
		component.AddEventListener(UIEventType.HOLD, new UIEvent.Handler(this.OnUnopenedPackHold));
		component.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnUnopenedPackRollover));
		component.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnUnopenedPackRollout));
		component.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.OnUnopenedPackReleaseAll));
		this.UpdatePack(component, boosterStack);
		AchieveManager.Get().NotifyOfPacksReadyToOpen(component, new AchieveManager.ActiveAchievesUpdatedCallback(this.NotifyOfPacksReadyToOpen_OnActiveAchievesUpdated));
		if (NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>().BoosterStacks.Count < 2)
		{
			this.ShowHintOnUnopenedPack();
		}
		this.LayoutPacks(false);
		this.UpdateUIEvents();
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x0009B3B0 File Offset: 0x000995B0
	private void NotifyOfPacksReadyToOpen_OnActiveAchievesUpdated(object userData)
	{
		HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
		hashSet.Add(RewardVisualTiming.IMMEDIATE);
		HashSet<RewardVisualTiming> rewardVisualTimings = hashSet;
		FixedRewardsMgr.Get().ShowFixedRewards(UserAttentionBlocker.NONE, rewardVisualTimings, null, null, AdventureScene.REWARD_PUNCH_SCALE, AdventureScene.REWARD_SCALE);
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x0009B3F0 File Offset: 0x000995F0
	private void LayoutPacks(bool animate = false)
	{
		IEnumerable<int> sortedPackIds = GameUtils.GetSortedPackIds(false);
		this.m_UnopenedPackContainer.ClearObjects();
		foreach (int key in sortedPackIds)
		{
			UnopenedPack unopenedPack;
			this.m_unopenedPacks.TryGetValue(key, out unopenedPack);
			if (!(unopenedPack == null) && unopenedPack.GetBoosterStack().Count != 0)
			{
				unopenedPack.gameObject.SetActive(true);
				this.m_UnopenedPackContainer.AddObject(unopenedPack, true);
			}
		}
		if (this.m_OnePackCentered && this.m_UnopenedPackContainer.m_Objects.Count <= 1)
		{
			this.m_UnopenedPackContainer.AddSpace(0);
		}
		this.m_UnopenedPackContainer.AddObject(this.m_StoreButton, this.m_StoreButtonOffset, true);
		if (animate)
		{
			this.m_UnopenedPackContainer.AnimateUpdatePositions(0.25f, iTween.EaseType.easeInOutQuad);
		}
		else
		{
			this.m_UnopenedPackContainer.UpdatePositions();
		}
	}

	// Token: 0x06001FC1 RID: 8129 RVA: 0x0009B504 File Offset: 0x00099704
	private void CreateDraggedPack(UnopenedPack creatorPack)
	{
		this.m_draggedPack = creatorPack.AcquireDraggedPack();
		Vector3 position = this.m_draggedPack.transform.position;
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(GameLayer.DragPlane.LayerBit(), out raycastHit))
		{
			position = raycastHit.point;
		}
		float num = Vector3.Dot(Camera.main.transform.forward, Vector3.up);
		float num2 = -num / Mathf.Abs(num);
		Bounds bounds = this.m_draggedPack.GetComponent<Collider>().bounds;
		position.y += num2 * bounds.extents.y * this.m_draggedPack.transform.lossyScale.y;
		this.m_draggedPack.transform.position = position;
	}

	// Token: 0x06001FC2 RID: 8130 RVA: 0x0009B5D4 File Offset: 0x000997D4
	private void DestroyDraggedPack()
	{
		this.m_draggedPack.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDraggedPackRelease));
		UnopenedPack creatorPack = this.m_draggedPack.GetCreatorPack();
		creatorPack.ReleaseDraggedPack();
		this.m_draggedPack = null;
	}

	// Token: 0x06001FC3 RID: 8131 RVA: 0x0009B614 File Offset: 0x00099814
	private void UpdateDraggedPack()
	{
		if (this.m_draggedPack == null)
		{
			return;
		}
		Vector3 position = this.m_draggedPack.transform.position;
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(GameLayer.DragPlane.LayerBit(), out raycastHit))
		{
			position.x = raycastHit.point.x;
			position.z = raycastHit.point.z;
			this.m_draggedPack.transform.position = position;
		}
		if (UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			this.DropPack();
		}
	}

	// Token: 0x06001FC4 RID: 8132 RVA: 0x0009B6B4 File Offset: 0x000998B4
	private IEnumerator HideAfterNoMorePacks()
	{
		while (!(this.m_director == null) && !(this.m_director.gameObject == null))
		{
			yield return new WaitForSeconds(0.2f);
		}
		this.Hide();
		yield break;
	}

	// Token: 0x06001FC5 RID: 8133 RVA: 0x0009B6D0 File Offset: 0x000998D0
	private void OnDirectorFinished(object userData)
	{
		this.m_UnopenedPackScroller.Pause(false);
		int num = 0;
		foreach (KeyValuePair<int, UnopenedPack> keyValuePair in this.m_unopenedPacks)
		{
			if (!(keyValuePair.Value == null))
			{
				int count = keyValuePair.Value.GetBoosterStack().Count;
				num += count;
				keyValuePair.Value.gameObject.SetActive(count > 0);
			}
		}
		if (num == 0)
		{
			base.StartCoroutine(this.HideAfterNoMorePacks());
		}
		else
		{
			this.m_InputBlocker.SetActive(false);
			this.m_openedPacksUntilCardCacheReset++;
			if (this.m_openedPacksUntilCardCacheReset == 10)
			{
				DefLoader.Get().ClearCardDefs();
				this.m_openedPacksUntilCardCacheReset = 0;
			}
			this.CreateDirector();
			this.LayoutPacks(true);
		}
		BnetBar.Get().m_currencyFrame.RefreshContents();
	}

	// Token: 0x06001FC6 RID: 8134 RVA: 0x0009B7E0 File Offset: 0x000999E0
	private void ShowHintOnUnopenedPack()
	{
		List<UnopenedPack> list = new List<UnopenedPack>();
		foreach (KeyValuePair<int, UnopenedPack> keyValuePair in this.m_unopenedPacks)
		{
			if (!(keyValuePair.Value == null))
			{
				if (keyValuePair.Value.GetBoosterStack().Count > 0)
				{
					list.Add(keyValuePair.Value);
				}
			}
		}
		if (list.Count < 1 || list[0] == null)
		{
			return;
		}
		list[0].PlayAlert();
		if (Options.Get().GetBool(Option.HAS_OPENED_BOOSTER, false) || !UserAttentionManager.CanShowAttentionGrabber("PackOpening.ShowHintOnUnopenedPack"))
		{
			return;
		}
		if (this.m_hintArrow == null)
		{
			this.m_hintArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, false);
		}
		if (this.m_hintArrow != null)
		{
			this.FixArrowScale(list[0].transform);
			Bounds bounds = list[0].GetComponent<Collider>().bounds;
			Bounds bounds2 = this.m_hintArrow.bounceObject.GetComponent<Renderer>().bounds;
			Vector3 center = bounds.center;
			center.z += bounds.extents.z + bounds2.extents.z;
			this.m_hintArrow.transform.position = center;
		}
	}

	// Token: 0x06001FC7 RID: 8135 RVA: 0x0009B97C File Offset: 0x00099B7C
	private void ShowHintOnSlot()
	{
		if (Options.Get().GetBool(Option.HAS_OPENED_BOOSTER, false) || !UserAttentionManager.CanShowAttentionGrabber("PackOpening.ShowHintOnSlot"))
		{
			return;
		}
		if (this.m_hintArrow == null)
		{
			this.m_hintArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, false);
		}
		if (this.m_hintArrow != null)
		{
			this.FixArrowScale(this.m_draggedPack.transform);
			Bounds bounds = this.m_hintArrow.bounceObject.GetComponent<Renderer>().bounds;
			Vector3 position = this.m_Bones.m_Hint.position;
			position.z += bounds.extents.z;
			this.m_hintArrow.transform.position = position;
		}
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x0009BA48 File Offset: 0x00099C48
	private void FixArrowScale(Transform parent)
	{
		Transform parent2 = this.m_hintArrow.transform.parent;
		this.m_hintArrow.transform.parent = parent;
		this.m_hintArrow.transform.localScale = Vector3.one;
		this.m_hintArrow.transform.parent = parent2;
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x0009BAA0 File Offset: 0x00099CA0
	private void HideHint()
	{
		if (this.m_hintArrow == null)
		{
			return;
		}
		Options.Get().SetBool(Option.HAS_OPENED_BOOSTER, true);
		Object.Destroy(this.m_hintArrow.gameObject);
		this.m_hintArrow = null;
	}

	// Token: 0x06001FCA RID: 8138 RVA: 0x0009BAE3 File Offset: 0x00099CE3
	private void DestroyHint()
	{
		if (this.m_hintArrow == null)
		{
			return;
		}
		Object.Destroy(this.m_hintArrow.gameObject);
		this.m_hintArrow = null;
	}

	// Token: 0x06001FCB RID: 8139 RVA: 0x0009BB10 File Offset: 0x00099D10
	private void InitializeUI()
	{
		this.m_HeaderText.Text = GameStrings.Get("GLUE_PACK_OPENING_HEADER");
		this.m_BackButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonPressed));
		this.m_DragPlane.SetActive(false);
		this.m_InputBlocker.SetActive(false);
	}

	// Token: 0x06001FCC RID: 8140 RVA: 0x0009BB64 File Offset: 0x00099D64
	private void UpdateUIEvents()
	{
		if (!this.m_shown)
		{
			this.UnregisterUIEvents();
			return;
		}
		if (this.m_draggedPack != null)
		{
			this.UnregisterUIEvents();
			return;
		}
		this.m_enableBackButton = true;
		this.m_BackButton.SetEnabled(true);
		this.m_StoreButton.SetEnabled(true);
		foreach (KeyValuePair<int, UnopenedPack> keyValuePair in this.m_unopenedPacks)
		{
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.SetEnabled(true);
			}
		}
	}

	// Token: 0x06001FCD RID: 8141 RVA: 0x0009BC20 File Offset: 0x00099E20
	private void UnregisterUIEvents()
	{
		this.m_enableBackButton = false;
		this.m_BackButton.SetEnabled(false);
		this.m_StoreButton.SetEnabled(false);
		foreach (KeyValuePair<int, UnopenedPack> keyValuePair in this.m_unopenedPacks)
		{
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.SetEnabled(false);
			}
		}
	}

	// Token: 0x06001FCE RID: 8142 RVA: 0x0009BCB4 File Offset: 0x00099EB4
	private void OnBackButtonPressed(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x06001FCF RID: 8143 RVA: 0x0009BCBC File Offset: 0x00099EBC
	private void HoldPack(UnopenedPack selectedPack)
	{
		if (!selectedPack.CanOpenPack())
		{
			return;
		}
		this.HideUnopenedPackTooltip();
		PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
		this.m_DragPlane.SetActive(true);
		this.CreateDraggedPack(selectedPack);
		if (this.m_draggedPack != null)
		{
			KeywordHelpPanel componentInChildren = this.m_draggedPack.GetComponentInChildren<KeywordHelpPanel>();
			if (componentInChildren != null)
			{
				Object.Destroy(componentInChildren.gameObject);
			}
		}
		this.PickUpBooster();
		selectedPack.StopAlert();
		this.ShowHintOnSlot();
		this.m_Socket.OnPackHeld();
		this.m_SocketAccent.OnPackHeld();
		this.UpdateUIEvents();
	}

	// Token: 0x06001FD0 RID: 8144 RVA: 0x0009BD5C File Offset: 0x00099F5C
	private void DropPack()
	{
		PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
		this.m_Socket.OnPackReleased();
		this.m_SocketAccent.OnPackReleased();
		bool flag = UniversalInputManager.Get().InputIsOver(this.m_Socket.gameObject);
		if (flag)
		{
			if (BattleNet.GetAccountCountry() == "KOR")
			{
				PackOpening.m_hasAcknowledgedKoreanWarning = true;
			}
			this.OpenBooster(this.m_draggedPack);
			this.HideHint();
		}
		else
		{
			this.PutBackBooster();
			this.DestroyHint();
		}
		this.DestroyDraggedPack();
		this.UpdateUIEvents();
		this.m_DragPlane.SetActive(false);
	}

	// Token: 0x06001FD1 RID: 8145 RVA: 0x0009BDFC File Offset: 0x00099FFC
	private void AutomaticallyOpenPack()
	{
		if (!this.CanOpenPackAutomatically())
		{
			return;
		}
		this.HideUnopenedPackTooltip();
		UnopenedPack unopenedPack = null;
		if (!this.m_unopenedPacks.TryGetValue(this.m_lastOpenedBoosterId, out unopenedPack) || unopenedPack.GetBoosterStack().Count == 0)
		{
			foreach (KeyValuePair<int, UnopenedPack> keyValuePair in this.m_unopenedPacks)
			{
				if (!(keyValuePair.Value == null))
				{
					if (keyValuePair.Value.GetBoosterStack().Count > 0)
					{
						unopenedPack = keyValuePair.Value;
						break;
					}
				}
			}
		}
		if (unopenedPack == null)
		{
			return;
		}
		if (!unopenedPack.CanOpenPack())
		{
			return;
		}
		this.m_draggedPack = unopenedPack.AcquireDraggedPack();
		this.PickUpBooster();
		unopenedPack.StopAlert();
		this.OpenBooster(this.m_draggedPack);
		this.DestroyDraggedPack();
		this.UpdateUIEvents();
		this.m_DragPlane.SetActive(false);
	}

	// Token: 0x06001FD2 RID: 8146 RVA: 0x0009BF1C File Offset: 0x0009A11C
	private void OnUnopenedPackHold(UIEvent e)
	{
		this.HoldPack(e.GetElement() as UnopenedPack);
	}

	// Token: 0x06001FD3 RID: 8147 RVA: 0x0009BF30 File Offset: 0x0009A130
	private void OnUnopenedPackRollover(UIEvent e)
	{
		if (PackOpening.m_hasAcknowledgedKoreanWarning || BattleNet.GetAccountCountry() != "KOR")
		{
			return;
		}
		UnopenedPack unopenedPack = e.GetElement() as UnopenedPack;
		TooltipZone component = unopenedPack.GetComponent<TooltipZone>();
		if (component == null)
		{
			return;
		}
		component.ShowTooltip(" ", GameStrings.Get("GLUE_PACK_OPENING_TOOLTIP"), 5f, true);
	}

	// Token: 0x06001FD4 RID: 8148 RVA: 0x0009BF98 File Offset: 0x0009A198
	private void OnUnopenedPackRollout(UIEvent e)
	{
		this.HideUnopenedPackTooltip();
	}

	// Token: 0x06001FD5 RID: 8149 RVA: 0x0009BFA0 File Offset: 0x0009A1A0
	private void OnUnopenedPackReleaseAll(UIEvent e)
	{
		if (this.m_draggedPack == null)
		{
			if (!UniversalInputManager.Get().IsTouchMode())
			{
				UIReleaseAllEvent uireleaseAllEvent = (UIReleaseAllEvent)e;
				if (uireleaseAllEvent.GetMouseIsOver())
				{
					this.HoldPack(e.GetElement() as UnopenedPack);
				}
			}
		}
		else
		{
			this.DropPack();
		}
	}

	// Token: 0x06001FD6 RID: 8150 RVA: 0x0009BFFB File Offset: 0x0009A1FB
	private void OnDraggedPackRelease(UIEvent e)
	{
		this.DropPack();
	}

	// Token: 0x06001FD7 RID: 8151 RVA: 0x0009C004 File Offset: 0x0009A204
	private void HideUnopenedPackTooltip()
	{
		foreach (KeyValuePair<int, UnopenedPack> keyValuePair in this.m_unopenedPacks)
		{
			if (!(keyValuePair.Value == null))
			{
				keyValuePair.Value.GetComponent<TooltipZone>().HideTooltip();
			}
		}
	}

	// Token: 0x06001FD8 RID: 8152 RVA: 0x0009C080 File Offset: 0x0009A280
	private bool CanOpenPackAutomatically()
	{
		return !this.m_autoOpenPending && this.m_shown && GameUtils.HaveBoosters() && (!this.m_director.IsPlaying() || this.m_director.IsDoneButtonShown()) && !this.m_DragPlane.activeSelf && !StoreManager.Get().IsShownOrWaitingToShow();
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x0009C0FC File Offset: 0x0009A2FC
	private IEnumerator OpenNextPackWhenReady()
	{
		float waitTime = 0f;
		if (this.m_director.IsPlaying())
		{
			waitTime = 1f;
		}
		while (this.m_director.IsPlaying())
		{
			yield return null;
		}
		yield return new WaitForSeconds(waitTime);
		this.m_autoOpenPending = false;
		this.AutomaticallyOpenPack();
		yield break;
	}

	// Token: 0x06001FDA RID: 8154 RVA: 0x0009C117 File Offset: 0x0009A317
	private void OnPackOpeningFXLoaded(string name, GameObject gameObject, object callbackData)
	{
		this.m_PackOpeningCardFX = gameObject;
	}

	// Token: 0x06001FDB RID: 8155 RVA: 0x0009C120 File Offset: 0x0009A320
	private void OnStoreButtonReleased(UIEvent e)
	{
		if (this.m_StoreButton.IsVisualClosed())
		{
			return;
		}
		StoreManager.Get().StartGeneralTransaction();
	}

	// Token: 0x0400116F RID: 4463
	private const int MAX_OPENED_PACKS_BEFORE_CARD_CACHE_RESET = 10;

	// Token: 0x04001170 RID: 4464
	public PackOpeningBones m_Bones;

	// Token: 0x04001171 RID: 4465
	public PackOpeningDirector m_DirectorPrefab;

	// Token: 0x04001172 RID: 4466
	public PackOpeningSocket m_Socket;

	// Token: 0x04001173 RID: 4467
	public PackOpeningSocket m_SocketAccent;

	// Token: 0x04001174 RID: 4468
	public UberText m_HeaderText;

	// Token: 0x04001175 RID: 4469
	public UIBButton m_BackButton;

	// Token: 0x04001176 RID: 4470
	public StoreButton m_StoreButton;

	// Token: 0x04001177 RID: 4471
	public GameObject m_DragPlane;

	// Token: 0x04001178 RID: 4472
	public GameObject m_InputBlocker;

	// Token: 0x04001179 RID: 4473
	public UIBObjectSpacing m_UnopenedPackContainer;

	// Token: 0x0400117A RID: 4474
	public UIBScrollable m_UnopenedPackScroller;

	// Token: 0x0400117B RID: 4475
	public Vector3 m_StoreButtonOffset;

	// Token: 0x0400117C RID: 4476
	public float m_UnopenedPackPadding;

	// Token: 0x0400117D RID: 4477
	public bool m_OnePackCentered = true;

	// Token: 0x0400117E RID: 4478
	private static PackOpening s_instance;

	// Token: 0x0400117F RID: 4479
	private bool m_waitingForInitialNetData = true;

	// Token: 0x04001180 RID: 4480
	private bool m_shown;

	// Token: 0x04001181 RID: 4481
	private Map<int, UnopenedPack> m_unopenedPacks = new Map<int, UnopenedPack>();

	// Token: 0x04001182 RID: 4482
	private Map<int, bool> m_unopenedPacksLoading = new Map<int, bool>();

	// Token: 0x04001183 RID: 4483
	private bool m_loadingUnopenedPack;

	// Token: 0x04001184 RID: 4484
	private PackOpeningDirector m_director;

	// Token: 0x04001185 RID: 4485
	private UnopenedPack m_draggedPack;

	// Token: 0x04001186 RID: 4486
	private Notification m_hintArrow;

	// Token: 0x04001187 RID: 4487
	private GameObject m_PackOpeningCardFX;

	// Token: 0x04001188 RID: 4488
	private int m_openedPacksUntilCardCacheReset;

	// Token: 0x04001189 RID: 4489
	private bool m_autoOpenPending;

	// Token: 0x0400118A RID: 4490
	private int m_lastOpenedBoosterId;

	// Token: 0x0400118B RID: 4491
	private bool m_enableBackButton;

	// Token: 0x0400118C RID: 4492
	private static bool m_hasAcknowledgedKoreanWarning;
}
