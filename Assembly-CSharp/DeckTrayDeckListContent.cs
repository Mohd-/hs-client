using System;
using System.Collections;
using System.Collections.Generic;
using PegasusShared;
using UnityEngine;

// Token: 0x02000384 RID: 900
[CustomEditClass]
public class DeckTrayDeckListContent : DeckTrayContent
{
	// Token: 0x170003AD RID: 941
	// (get) Token: 0x06002E0D RID: 11789 RVA: 0x000E6C9D File Offset: 0x000E4E9D
	// (set) Token: 0x06002E0C RID: 11788 RVA: 0x000E6C8D File Offset: 0x000E4E8D
	[CustomEditField(Sections = "Deck Button Settings")]
	public Vector3 DeckButtonOffset
	{
		get
		{
			return this.m_deckButtonOffset;
		}
		set
		{
			this.m_deckButtonOffset = value;
			this.UpdateNewDeckButton(null);
		}
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x000E6CA8 File Offset: 0x000E4EA8
	private void Awake()
	{
		CollectionManager collectionManager = CollectionManager.Get();
		collectionManager.RegisterFavoriteHeroChangedListener(new CollectionManager.FavoriteHeroChangedCallback(this.OnFavoriteHeroChanged));
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x000E6CD0 File Offset: 0x000E4ED0
	private void Update()
	{
		if (this.m_wasTouchModeEnabled == UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
		if (UniversalInputManager.Get().IsTouchMode() && this.m_deckInfoTooltip != null)
		{
			this.HideDeckInfo();
		}
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x000E6D2C File Offset: 0x000E4F2C
	private void OnDestroy()
	{
		CollectionManager collectionManager = CollectionManager.Get();
		collectionManager.RemoveFavoriteHeroChangedListener(new CollectionManager.FavoriteHeroChangedCallback(this.OnFavoriteHeroChanged));
		collectionManager.RemoveDeckDeletedListener(new CollectionManager.DelOnDeckDeleted(this.OnDeckDeleted));
		if (Box.Get() != null)
		{
			Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
		}
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x000E6D8C File Offset: 0x000E4F8C
	public bool IsDoneEntering()
	{
		return this.m_doneEntering;
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x000E6D94 File Offset: 0x000E4F94
	public IEnumerator ShowTrayDoors(bool show)
	{
		if (!show)
		{
			yield return new WaitForSeconds(0.3f);
		}
		foreach (TraySection traySection in this.m_traySections)
		{
			traySection.ShowDoor(show);
		}
		yield return null;
		yield break;
	}

	// Token: 0x06002E13 RID: 11795 RVA: 0x000E6DC0 File Offset: 0x000E4FC0
	public override bool AnimateContentEntranceStart()
	{
		this.Initialize();
		this.UpdateAllTrays(SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL);
		if (this.m_editingTraySection != null)
		{
			this.FinishRenamingEditingDeck(null);
			this.m_editingTraySection.MoveDeckBoxBackToOriginalPosition(0.25f, delegate(object o)
			{
				this.m_editingTraySection = null;
			});
		}
		this.m_newDeckButton.SetIsUsable(this.CanShowNewDeckButton());
		this.FireBusyWithDeckEvent(true);
		this.FireDeckCountChangedEvent();
		CollectionManager.Get().DoneEditing();
		return true;
	}

	// Token: 0x06002E14 RID: 11796 RVA: 0x000E6E44 File Offset: 0x000E5044
	public override bool AnimateContentEntranceEnd()
	{
		if (this.m_editingTraySection != null)
		{
			return false;
		}
		this.m_newDeckButton.SetEnabled(true);
		this.FireBusyWithDeckEvent(false);
		this.DeleteQueuedDecks(true);
		return true;
	}

	// Token: 0x06002E15 RID: 11797 RVA: 0x000E6E80 File Offset: 0x000E5080
	public override bool AnimateContentExitStart()
	{
		this.m_animatingExit = true;
		this.FireBusyWithDeckEvent(true);
		float? speed = default(float?);
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			speed = new float?(500f);
		}
		this.ShowNewDeckButton(false, speed, null);
		float animationWaitTime = 0.5f;
		ApplicationMgr.Get().ScheduleCallback(0.5f, false, delegate(object _0)
		{
			foreach (TraySection traySection in this.m_traySections)
			{
				if (this.m_editingTraySection != traySection)
				{
					traySection.HideDeckBox(false, null);
				}
			}
			if (this.m_newlyCreatedTraySection != null)
			{
				TraySection animateTraySection = this.m_newlyCreatedTraySection;
				this.UpdateNewDeckButtonPosition(animateTraySection);
				this.ShowNewDeckButton(true, delegate(object _1)
				{
					animateTraySection.ShowDeckBox(true, delegate(object _2)
					{
						animateTraySection.m_deckBox.gameObject.SetActive(false);
						this.m_newDeckButton.FlipHalfOverAndHide(0.1f, delegate(object _3)
						{
							animateTraySection.FlipDeckBoxHalfOverToShow(0.1f, delegate(object _4)
							{
								animateTraySection.MoveDeckBoxToEditPosition(this.m_deckEditTopPos.position, 0.25f, null);
							});
						});
					});
				});
				this.m_editingTraySection = this.m_newlyCreatedTraySection;
				this.m_newlyCreatedTraySection = null;
				animationWaitTime += 0.7f;
			}
			else if (this.m_editingTraySection != null)
			{
				this.m_editingTraySection.MoveDeckBoxToEditPosition(this.m_deckEditTopPos.position, 0.25f, null);
			}
			ApplicationMgr.Get().ScheduleCallback(animationWaitTime, false, delegate(object o)
			{
				this.m_animatingExit = false;
				this.FireBusyWithDeckEvent(false);
			}, null);
		}, null);
		return true;
	}

	// Token: 0x06002E16 RID: 11798 RVA: 0x000E6F01 File Offset: 0x000E5101
	public override bool AnimateContentExitEnd()
	{
		return !this.m_animatingExit;
	}

	// Token: 0x06002E17 RID: 11799 RVA: 0x000E6F0C File Offset: 0x000E510C
	public override bool PreAnimateContentExit()
	{
		if (this.m_scrollbar == null)
		{
			return true;
		}
		if (this.m_centeringDeckList != -1)
		{
			int num = this.GetTotalDeckBoxesInUse() - 1;
			float percentage;
			if (this.m_scrollbar.IsEnabledAndScrollable() && num > 0)
			{
				percentage = (float)this.m_centeringDeckList / (float)num;
			}
			else
			{
				percentage = 0f;
			}
			this.m_scrollbar.SetScroll(percentage, delegate(float f)
			{
				this.m_animatingExit = false;
			}, iTween.EaseType.linear, this.m_scrollbar.m_ScrollTweenTime, true, true);
			this.m_animatingExit = true;
			this.m_centeringDeckList = -1;
		}
		return !this.m_animatingExit;
	}

	// Token: 0x06002E18 RID: 11800 RVA: 0x000E6FAC File Offset: 0x000E51AC
	public override bool PostAnimateContentExit()
	{
		base.StartCoroutine(this.ShowTrayDoors(false));
		return true;
	}

	// Token: 0x06002E19 RID: 11801 RVA: 0x000E6FBD File Offset: 0x000E51BD
	public override bool PreAnimateContentEntrance()
	{
		this.m_doneEntering = false;
		base.StartCoroutine(this.ShowTrayDoors(true));
		return true;
	}

	// Token: 0x06002E1A RID: 11802 RVA: 0x000E6FD8 File Offset: 0x000E51D8
	public override void OnTaggedDeckChanged(CollectionManager.DeckTag tag, CollectionDeck newDeck, CollectionDeck oldDeck, bool isNewDeck)
	{
		if (tag != CollectionManager.DeckTag.Editing)
		{
			return;
		}
		if (newDeck != null && this.m_deckInfoTooltip != null)
		{
			this.m_deckInfoTooltip.SetDeck(newDeck);
			if (this.m_deckOptionsMenu != null)
			{
				this.m_deckOptionsMenu.SetDeck(newDeck);
			}
		}
		if (base.IsModeActive())
		{
			this.InitializeTraysFromDecks();
		}
		if (isNewDeck && newDeck != null)
		{
			this.m_newlyCreatedTraySection = this.GetExistingTrayFromDeck(newDeck);
			if (this.m_newlyCreatedTraySection != null)
			{
				this.m_centeringDeckList = this.m_newlyCreatedTraySection.m_deckBox.GetPositionIndex();
			}
		}
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x000E7080 File Offset: 0x000E5280
	public void CreateNewDeckFromUserSelection(TAG_CLASS heroClass, string heroCardID, string customDeckName = null)
	{
		bool flag = SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL;
		DeckType deckType = 1;
		string text = customDeckName;
		if (flag)
		{
			deckType = 6;
			text = GameStrings.Get("GLUE_COLLECTION_TAVERN_BRAWL_DECKNAME");
		}
		else if (string.IsNullOrEmpty(text))
		{
			text = CollectionManager.Get().AutoGenerateDeckName(heroClass);
		}
		CollectionManager.Get().SendCreateDeck(deckType, text, heroCardID);
		this.EndCreateNewDeck(true);
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x000E70E3 File Offset: 0x000E52E3
	public void CreateNewDeckCancelled()
	{
		this.EndCreateNewDeck(false);
	}

	// Token: 0x06002E1D RID: 11805 RVA: 0x000E70EC File Offset: 0x000E52EC
	public bool IsWaitingToDeleteDeck()
	{
		return this.m_waitingToDeleteDeck;
	}

	// Token: 0x06002E1E RID: 11806 RVA: 0x000E70F4 File Offset: 0x000E52F4
	public int NumDecksToDelete()
	{
		return this.m_decksToDelete.Count;
	}

	// Token: 0x06002E1F RID: 11807 RVA: 0x000E7101 File Offset: 0x000E5301
	public bool IsDeletingDecks()
	{
		return this.m_deletingDecks;
	}

	// Token: 0x06002E20 RID: 11808 RVA: 0x000E710C File Offset: 0x000E530C
	public void DeleteDeck(long deckID)
	{
		this.m_decksToDelete.Add(deckID);
		CollectionDeck deck = CollectionManager.Get().GetDeck(deckID);
		if (deck != null)
		{
			deck.MarkBeingDeleted();
		}
		this.DeleteQueuedDecks(false);
	}

	// Token: 0x06002E21 RID: 11809 RVA: 0x000E7144 File Offset: 0x000E5344
	public void DeleteEditingDeck()
	{
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		if (taggedDeck == null)
		{
			Debug.LogWarning("No deck currently being edited!");
			return;
		}
		this.m_waitingToDeleteDeck = true;
		this.DeleteDeck(taggedDeck.ID);
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x000E7181 File Offset: 0x000E5381
	public void CancelRenameEditingDeck()
	{
		this.FinishRenamingEditingDeck(null);
	}

	// Token: 0x06002E23 RID: 11811 RVA: 0x000E718A File Offset: 0x000E538A
	public Vector3 GetNewDeckButtonPosition()
	{
		return this.m_newDeckButton.transform.position;
	}

	// Token: 0x06002E24 RID: 11812 RVA: 0x000E719C File Offset: 0x000E539C
	public void UpdateEditingDeckBoxVisual(string heroCardId)
	{
		if (this.m_editingTraySection == null)
		{
			return;
		}
		this.m_editingTraySection.m_deckBox.SetHeroCardID(heroCardId);
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x000E71C4 File Offset: 0x000E53C4
	private void OnDrawGizmos()
	{
		if (this.m_editingTraySection == null)
		{
			return;
		}
		CollectionDeckBoxVisual deckBox = this.m_editingTraySection.m_deckBox;
		Bounds bounds = deckBox.GetDeckNameText().GetBounds();
		Gizmos.DrawWireSphere(bounds.min, 0.1f);
		Gizmos.DrawWireSphere(bounds.max, 0.1f);
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x000E7220 File Offset: 0x000E5420
	public void RenameCurrentlyEditingDeck()
	{
		if (this.m_editingTraySection == null)
		{
			Debug.LogWarning("Unable to rename deck. No deck currently being edited.", base.gameObject);
			return;
		}
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			return;
		}
		CollectionDeckBoxVisual deckBox = this.m_editingTraySection.m_deckBox;
		deckBox.HideDeckName();
		Camera camera = Box.Get().GetCamera();
		Bounds bounds = deckBox.GetDeckNameText().GetBounds();
		Rect rect = CameraUtils.CreateGUIViewportRect(camera, bounds.min, bounds.max);
		Font localizedFont = deckBox.GetDeckNameText().GetLocalizedFont();
		this.m_previousDeckName = deckBox.GetDeckNameText().Text;
		UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams
		{
			m_owner = base.gameObject,
			m_rect = rect,
			m_updatedCallback = delegate(string newName)
			{
				this.UpdateRenamingEditingDeck(newName);
			},
			m_completedCallback = delegate(string newName)
			{
				this.FinishRenamingEditingDeck(newName);
			},
			m_canceledCallback = delegate(bool a1, GameObject a2)
			{
				this.FinishRenamingEditingDeck(this.m_previousDeckName);
			},
			m_maxCharacters = 24,
			m_font = localizedFont,
			m_text = deckBox.GetDeckNameText().Text
		};
		UniversalInputManager.Get().UseTextInput(parms, false);
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x000E7347 File Offset: 0x000E5547
	public void RegisterDeckCountUpdated(DeckTrayDeckListContent.DeckCountChanged dlg)
	{
		this.m_deckCountChangedListeners.Add(dlg);
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x000E7355 File Offset: 0x000E5555
	public void UnregisterDeckCountUpdated(DeckTrayDeckListContent.DeckCountChanged dlg)
	{
		this.m_deckCountChangedListeners.Remove(dlg);
	}

	// Token: 0x06002E29 RID: 11817 RVA: 0x000E7364 File Offset: 0x000E5564
	public void RegisterBusyWithDeck(DeckTrayDeckListContent.BusyWithDeck dlg)
	{
		this.m_busyWithDeckListeners.Add(dlg);
	}

	// Token: 0x06002E2A RID: 11818 RVA: 0x000E7372 File Offset: 0x000E5572
	public void UnregisterBusyWithDeck(DeckTrayDeckListContent.BusyWithDeck dlg)
	{
		this.m_busyWithDeckListeners.Remove(dlg);
	}

	// Token: 0x06002E2B RID: 11819 RVA: 0x000E7384 File Offset: 0x000E5584
	public void UpdateDeckName(string deckName = null)
	{
		if (deckName == null)
		{
			CollectionDeck editingDeck = CollectionDeckTray.Get().GetCardsContent().GetEditingDeck();
			if (editingDeck == null)
			{
				return;
			}
			deckName = editingDeck.Name;
		}
		this.FinishRenamingEditingDeck(deckName);
	}

	// Token: 0x06002E2C RID: 11820 RVA: 0x000E73C4 File Offset: 0x000E55C4
	public void HideTraySectionsNotInBounds(Bounds bounds)
	{
		int num = 0;
		foreach (TraySection traySection in this.m_traySections)
		{
			if (traySection.HideIfNotInBounds(bounds))
			{
				num++;
			}
		}
		Log.JMac.Print("Hid {0} tray sections that were not visible.", new object[]
		{
			num
		});
		UIBScrollableItem component = this.m_newDeckButtonContainer.GetComponent<UIBScrollableItem>();
		if (component == null)
		{
			Debug.LogWarning("UIBScrollableItem not found on m_newDeckButtonContainer! This button may not be hidden properly while exiting Collection Manager!");
		}
		else
		{
			Bounds bounds2 = default(Bounds);
			Vector3 vector;
			Vector3 vector2;
			component.GetWorldBounds(out vector, out vector2);
			bounds2.SetMinMax(vector, vector2);
			if (!bounds.Intersects(bounds2))
			{
				Log.JMac.Print("Hiding the New Deck button because it's out of the visible scroll area.", new object[0]);
				this.m_newDeckButton.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x000E74C0 File Offset: 0x000E56C0
	private CollectionDeck UpdateRenamingEditingDeck(string newDeckName)
	{
		CollectionDeck editingDeck = CollectionDeckTray.Get().GetCardsContent().GetEditingDeck();
		if (editingDeck != null && !string.IsNullOrEmpty(newDeckName))
		{
			editingDeck.Name = newDeckName;
		}
		return editingDeck;
	}

	// Token: 0x06002E2E RID: 11822 RVA: 0x000E74F8 File Offset: 0x000E56F8
	private void FinishRenamingEditingDeck(string newDeckName = null)
	{
		if (this.m_editingTraySection == null)
		{
			return;
		}
		CollectionDeckBoxVisual deckBox = this.m_editingTraySection.m_deckBox;
		CollectionDeck collectionDeck = this.UpdateRenamingEditingDeck(newDeckName);
		if (collectionDeck != null && this.m_editingTraySection != null)
		{
			deckBox.SetDeckName(collectionDeck.Name);
		}
		if (UniversalInputManager.Get() != null && UniversalInputManager.Get().IsTextInputActive())
		{
			UniversalInputManager.Get().CancelTextInput(base.gameObject, false);
		}
		deckBox.ShowDeckName();
	}

	// Token: 0x06002E2F RID: 11823 RVA: 0x000E7584 File Offset: 0x000E5784
	private void Initialize()
	{
		if (this.m_initialized)
		{
			return;
		}
		this.m_newDeckButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.OnNewDeckButtonPress();
		});
		CollectionManager.Get().RegisterDeckDeletedListener(new CollectionManager.DelOnDeckDeleted(this.OnDeckDeleted));
		GameObject gameObject = AssetLoader.Get().LoadActor(FileUtils.GameAssetPathToName(this.m_deckInfoActorPrefab), false, false);
		if (gameObject == null)
		{
			Debug.LogError(string.Format("Unable to load actor {0}: null", this.m_deckInfoActorPrefab), base.gameObject);
			return;
		}
		this.m_deckInfoTooltip = gameObject.GetComponent<CollectionDeckInfo>();
		if (this.m_deckInfoTooltip == null)
		{
			Debug.LogError(string.Format("Actor {0} does not contain CollectionDeckInfo component.", this.m_deckInfoActorPrefab), base.gameObject);
			return;
		}
		GameUtils.SetParent(this.m_deckInfoTooltip, this.m_deckInfoTooltipBone, false);
		this.m_deckInfoTooltip.RegisterHideListener(new CollectionDeckInfo.HideListener(this.HideDeckInfoListener));
		gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(this.m_deckOptionsPrefab), true, false);
		this.m_deckOptionsMenu = gameObject.GetComponent<DeckOptionsMenu>();
		GameUtils.SetParent(this.m_deckOptionsMenu.gameObject, this.m_deckOptionsBone, false);
		this.m_deckOptionsMenu.SetDeckInfo(this.m_deckInfoTooltip);
		this.HideDeckInfo();
		this.CreateTraySections();
		this.m_initialized = true;
	}

	// Token: 0x06002E30 RID: 11824 RVA: 0x000E76D0 File Offset: 0x000E58D0
	private void HideDeckInfoListener()
	{
		if (this.m_editingTraySection != null)
		{
			SceneUtils.SetLayer(this.m_editingTraySection.m_deckBox.gameObject, GameLayer.Default);
			SceneUtils.SetLayer(this.m_deckOptionsMenu.gameObject, GameLayer.Default);
			this.m_editingTraySection.m_deckBox.HideRenameVisuals();
		}
		FullScreenFXMgr.Get().StopDesaturate(0.25f, iTween.EaseType.easeInOutQuad, null);
		if (UniversalInputManager.Get().IsTouchMode())
		{
			if (this.m_editingTraySection != null)
			{
				this.m_editingTraySection.m_deckBox.SetHighlightState(ActorStateType.NONE);
				this.m_editingTraySection.m_deckBox.ShowDeckName();
			}
			this.FinishRenamingEditingDeck(null);
		}
		this.m_deckOptionsMenu.Hide(true);
	}

	// Token: 0x06002E31 RID: 11825 RVA: 0x000E778C File Offset: 0x000E598C
	private void ShowDeckInfo()
	{
		if (!UniversalInputManager.Get().IsTouchMode())
		{
			this.m_editingTraySection.m_deckBox.ShowRenameVisuals();
		}
		SceneUtils.SetLayer(this.m_editingTraySection.m_deckBox.gameObject, GameLayer.IgnoreFullScreenEffects);
		SceneUtils.SetLayer(this.m_deckInfoTooltip.gameObject, GameLayer.IgnoreFullScreenEffects);
		SceneUtils.SetLayer(this.m_deckOptionsMenu.gameObject, GameLayer.IgnoreFullScreenEffects);
		FullScreenFXMgr.Get().Desaturate(0.9f, 0.25f, iTween.EaseType.easeInOutQuad, null);
		this.m_deckInfoTooltip.UpdateManaCurve();
		this.m_deckInfoTooltip.Show();
		this.m_deckOptionsMenu.Show();
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x000E782A File Offset: 0x000E5A2A
	private void HideDeckInfo()
	{
		this.m_deckInfoTooltip.Hide();
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x000E7838 File Offset: 0x000E5A38
	private void CreateTraySections()
	{
		Vector3 localScale = this.m_traySectionStartPos.localScale;
		Vector3 localEulerAngles = this.m_traySectionStartPos.localEulerAngles;
		GameObject gameObject = this.m_traySectionStartPos.gameObject;
		for (int i = 0; i < 20; i++)
		{
			TraySection traySection = (TraySection)GameUtils.Instantiate(this.m_traySectionPrefab, base.gameObject, false);
			traySection.transform.localScale = localScale;
			traySection.transform.localEulerAngles = localEulerAngles;
			traySection.EnableDoors(i < 18);
			if (i == 0)
			{
				traySection.transform.localPosition = this.m_traySectionStartPos.localPosition;
			}
			else
			{
				TransformUtil.SetPoint(traySection.gameObject, Anchor.FRONT, gameObject, Anchor.BACK);
			}
			Material material = null;
			foreach (Material material2 in traySection.m_door.GetComponent<Renderer>().materials)
			{
				if (material2.name.Equals("DeckTray", 5) || material2.name.Equals("DeckTray (Instance)", 5))
				{
					material = material2;
					break;
				}
			}
			Vector2 mainTextureOffset;
			mainTextureOffset..ctor(0f, -0.0825f * (float)i);
			traySection.GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset;
			if (material != null)
			{
				material.mainTextureOffset = mainTextureOffset;
			}
			gameObject = traySection.gameObject;
			CollectionDeckBoxVisual deckBox = traySection.m_deckBox;
			deckBox.SetPositionIndex(i);
			deckBox.AddEventListener(UIEventType.ROLLOVER, delegate(UIEvent e)
			{
				this.OnDeckBoxVisualOver(deckBox);
			});
			deckBox.AddEventListener(UIEventType.ROLLOUT, delegate(UIEvent e)
			{
				this.OnDeckBoxVisualOut(deckBox);
			});
			deckBox.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
			{
				this.OnDeckBoxVisualPress(deckBox);
			});
			deckBox.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.OnDeckBoxVisualRelease(traySection);
			});
			deckBox.SetOriginalButtonPosition();
			deckBox.HideBanner();
			this.m_traySections.Add(traySection);
		}
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.HideTraySectionsNotInBounds(CollectionDeckTray.Get().m_scrollbar.m_ScrollBounds.bounds);
			Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
		}
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x000E7AE0 File Offset: 0x000E5CE0
	private void OnBoxTransitionFinished(object userData)
	{
		Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
		foreach (TraySection traySection in this.m_traySections)
		{
			traySection.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002E35 RID: 11829 RVA: 0x000E7B58 File Offset: 0x000E5D58
	private TraySection GetExistingTrayFromDeck(CollectionDeck deck)
	{
		return this.GetExistingTrayFromDeck(deck.ID);
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x000E7B68 File Offset: 0x000E5D68
	private TraySection GetExistingTrayFromDeck(long deckID)
	{
		foreach (TraySection traySection in this.m_traySections)
		{
			if (traySection.m_deckBox.GetDeckID() == deckID)
			{
				return traySection;
			}
		}
		return null;
	}

	// Token: 0x06002E37 RID: 11831 RVA: 0x000E7BD8 File Offset: 0x000E5DD8
	public TraySection GetEditingTraySection()
	{
		return this.m_editingTraySection;
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x000E7BE0 File Offset: 0x000E5DE0
	private void InitializeTraysFromDecks()
	{
		this.UpdateDeckTrayVisuals();
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x000E7BEC File Offset: 0x000E5DEC
	private void UpdateAllTrays(bool immediate = false)
	{
		this.InitializeTraysFromDecks();
		List<TraySection> list = new List<TraySection>();
		foreach (TraySection traySection in this.m_traySections)
		{
			if (traySection.m_deckBox.GetDeckID() == -1L && !traySection.m_deckBox.IsLocked())
			{
				traySection.HideDeckBox(immediate, null);
			}
			else if (this.m_editingTraySection != traySection && !traySection.IsOpen())
			{
				list.Add(traySection);
			}
		}
		base.StartCoroutine(this.UpdateAllTraysAnimation(list, immediate));
	}

	// Token: 0x06002E3A RID: 11834 RVA: 0x000E7CAC File Offset: 0x000E5EAC
	private IEnumerator UpdateAllTraysAnimation(List<TraySection> showTraySections, bool immediate)
	{
		foreach (TraySection traySection in showTraySections)
		{
			traySection.ShowDeckBox(immediate, null);
			if (!immediate)
			{
				yield return new WaitForSeconds(0.015f);
			}
		}
		this.UpdateNewDeckButton(null);
		this.m_doneEntering = true;
		yield break;
	}

	// Token: 0x06002E3B RID: 11835 RVA: 0x000E7CE4 File Offset: 0x000E5EE4
	private void UpdateNewDeckButton(TraySection setNewDeckButtonPosition = null)
	{
		bool flag = this.UpdateNewDeckButtonPosition(setNewDeckButtonPosition);
		this.ShowNewDeckButton(flag && this.CanShowNewDeckButton(), null);
	}

	// Token: 0x06002E3C RID: 11836 RVA: 0x000E7D10 File Offset: 0x000E5F10
	private bool UpdateNewDeckButtonPosition(TraySection setNewDeckButtonPosition = null)
	{
		bool result = false;
		Vector3 localPosition;
		this.GetIdealNewDeckButtonLocalPosition(setNewDeckButtonPosition, out localPosition, out result);
		this.m_newDeckButtonContainer.transform.localPosition = localPosition;
		return result;
	}

	// Token: 0x06002E3D RID: 11837 RVA: 0x000E7D3C File Offset: 0x000E5F3C
	private void GetIdealNewDeckButtonLocalPosition(TraySection setNewDeckButtonPosition, out Vector3 outPosition, out bool outActive)
	{
		TraySection lastUnusedTraySection = this.GetLastUnusedTraySection();
		TraySection traySection = (!(setNewDeckButtonPosition == null)) ? setNewDeckButtonPosition : lastUnusedTraySection;
		outActive = (lastUnusedTraySection != null);
		outPosition = ((!(traySection != null)) ? this.m_traySectionStartPos.localPosition : traySection.transform.localPosition) + this.m_deckButtonOffset;
	}

	// Token: 0x06002E3E RID: 11838 RVA: 0x000E7DA8 File Offset: 0x000E5FA8
	public TraySection GetLastUnusedTraySection()
	{
		int num = 0;
		foreach (TraySection traySection in this.m_traySections)
		{
			if (num >= 18)
			{
				break;
			}
			if (traySection.m_deckBox.GetDeckID() == -1L)
			{
				return traySection;
			}
			num++;
		}
		return null;
	}

	// Token: 0x06002E3F RID: 11839 RVA: 0x000E7E2C File Offset: 0x000E602C
	public TraySection GetLastUsedTraySection()
	{
		int num = 0;
		TraySection result = null;
		foreach (TraySection traySection in this.m_traySections)
		{
			if (num >= 18)
			{
				break;
			}
			if (traySection.m_deckBox.GetDeckID() == -1L)
			{
				return result;
			}
			result = traySection;
			num++;
		}
		return result;
	}

	// Token: 0x06002E40 RID: 11840 RVA: 0x000E7EB4 File Offset: 0x000E60B4
	public TraySection GetTraySection(int index)
	{
		if (index >= 0 && index < this.m_traySections.Count)
		{
			return this.m_traySections[index];
		}
		return null;
	}

	// Token: 0x06002E41 RID: 11841 RVA: 0x000E7EE8 File Offset: 0x000E60E8
	public void ShowNewDeckButton(bool newDeckButtonActive, CollectionNewDeckButton.DelOnAnimationFinished callback = null)
	{
		this.ShowNewDeckButton(newDeckButtonActive, default(float?), callback);
	}

	// Token: 0x06002E42 RID: 11842 RVA: 0x000E7F08 File Offset: 0x000E6108
	public void ShowNewDeckButton(bool newDeckButtonActive, float? speed, CollectionNewDeckButton.DelOnAnimationFinished callback = null)
	{
		if (this.m_newDeckButton.gameObject.activeSelf != newDeckButtonActive)
		{
			if (newDeckButtonActive)
			{
				this.m_newDeckButton.gameObject.SetActive(true);
				this.m_newDeckButton.PlayPopUpAnimation(delegate(object o)
				{
					if (callback != null)
					{
						callback(this);
					}
				}, null, speed);
			}
			else
			{
				this.m_newDeckButton.PlayPopDownAnimation(delegate(object o)
				{
					this.m_newDeckButton.gameObject.SetActive(false);
					if (callback != null)
					{
						callback(this);
					}
				}, null, speed);
			}
		}
		else if (callback != null)
		{
			callback(this);
		}
	}

	// Token: 0x06002E43 RID: 11843 RVA: 0x000E7FAC File Offset: 0x000E61AC
	public bool CanShowNewDeckButton()
	{
		return AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES) && CollectionManager.Get().GetDecks(1).Count < 18;
	}

	// Token: 0x06002E44 RID: 11844 RVA: 0x000E7FE4 File Offset: 0x000E61E4
	public void SetEditingTraySection(int index)
	{
		this.m_editingTraySection = this.m_traySections[index];
		this.m_centeringDeckList = this.m_editingTraySection.m_deckBox.GetPositionIndex();
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x000E8019 File Offset: 0x000E6219
	private bool IsEditingCards()
	{
		return CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing) != null;
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x000E802C File Offset: 0x000E622C
	private void OnDeckBoxVisualOver(CollectionDeckBoxVisual deckBox)
	{
		if (deckBox.IsLocked())
		{
			return;
		}
		if (UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		if (this.IsEditingCards() && this.m_deckInfoTooltip != null)
		{
			this.ShowDeckInfo();
			return;
		}
		if (base.IsModeTryingOrActive())
		{
			deckBox.ShowDeleteButton(true);
		}
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x000E808C File Offset: 0x000E628C
	private void OnDeckBoxVisualOut(CollectionDeckBoxVisual deckBox)
	{
		if (deckBox.IsLocked())
		{
			return;
		}
		if (UniversalInputManager.Get().IsTouchMode())
		{
			if (this.m_deckInfoTooltip != null && this.m_deckInfoTooltip.IsShown())
			{
				deckBox.SetHighlightState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
			}
			return;
		}
		if (!UniversalInputManager.Get().InputIsOver(deckBox.m_deleteButton.gameObject))
		{
			deckBox.ShowDeleteButton(false);
		}
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x000E8101 File Offset: 0x000E6301
	private void OnDeckBoxVisualPress(CollectionDeckBoxVisual deckBox)
	{
		if (deckBox.IsLocked())
		{
			return;
		}
		deckBox.enabled = false;
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x000E8118 File Offset: 0x000E6318
	private void OnDeckBoxVisualRelease(TraySection traySection)
	{
		if (traySection.m_deckBox.IsLocked())
		{
			return;
		}
		CollectionDeckBoxVisual deckBox = traySection.m_deckBox;
		deckBox.enabled = true;
		if (this.m_scrollbar != null && this.m_scrollbar.IsTouchDragging())
		{
			return;
		}
		long deckID = deckBox.GetDeckID();
		CollectionDeck deck = CollectionManager.Get().GetDeck(deckID);
		if (deck.IsBeingDeleted())
		{
			Log.JMac.Print(string.Format("DeckTrayDeckListContent.OnDeckBoxVisualRelease(): cannot edit deck {0}; it is being deleted", deck), new object[0]);
			return;
		}
		if (deck.IsSavingChanges())
		{
			Log.Rachelle.Print(string.Format("DeckTrayDeckListContent.OnDeckBoxVisualRelease(): cannot edit deck {0}; waiting for changes to be saved", deck), new object[0]);
			return;
		}
		if (this.IsEditingCards())
		{
			if (!UniversalInputManager.Get().IsTouchMode())
			{
				this.RenameCurrentlyEditingDeck();
			}
			else if (this.m_deckInfoTooltip != null && !this.m_deckInfoTooltip.IsShown())
			{
				this.ShowDeckInfo();
			}
		}
		else if (base.IsModeActive())
		{
			this.m_editingTraySection = traySection;
			this.m_centeringDeckList = this.m_editingTraySection.m_deckBox.GetPositionIndex();
			CollectionManagerDisplay.Get().RequestContentsToShowDeck(deckID);
			this.m_newDeckButton.SetEnabled(false);
			CollectionManagerDisplay.Get().HideDeckHelpPopup();
			Options.Get().SetBool(Option.HAS_STARTED_A_DECK, true);
		}
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x000E8274 File Offset: 0x000E6474
	private void OnNewDeckButtonPress()
	{
		if (!base.IsModeActive())
		{
			return;
		}
		if (this.m_scrollbar != null && this.m_scrollbar.IsTouchDragging())
		{
			return;
		}
		SoundManager.Get().LoadAndPlay("Hub_Click");
		this.StartCreateNewDeck();
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x000E82C4 File Offset: 0x000E64C4
	private void StartCreateNewDeck()
	{
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.DECKEDITOR
		});
		this.ShowNewDeckButton(false, null);
		CollectionManagerDisplay.Get().EnterSelectNewDeckHeroMode();
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x000E82F4 File Offset: 0x000E64F4
	private void EndCreateNewDeck(bool newDeck)
	{
		CollectionManagerDisplay.Get().ExitSelectNewDeckHeroMode();
		this.ShowNewDeckButton(true, delegate(object o)
		{
			if (newDeck)
			{
				this.UpdateAllTrays(true);
			}
		});
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x000E8334 File Offset: 0x000E6534
	private void DeleteQueuedDecks(bool force = false)
	{
		if (this.m_decksToDelete.Count == 0 || (!base.IsModeActive() && !force))
		{
			return;
		}
		foreach (long id in this.m_decksToDelete)
		{
			CollectionManager.Get().SendDeleteDeck(id);
		}
		this.m_decksToDelete.Clear();
	}

	// Token: 0x06002E4E RID: 11854 RVA: 0x000E83C0 File Offset: 0x000E65C0
	private void OnDeckDeleted(long deckID)
	{
		this.m_waitingToDeleteDeck = false;
		base.StartCoroutine(this.DeleteDeckAnimation(deckID, null));
	}

	// Token: 0x06002E4F RID: 11855 RVA: 0x000E83D8 File Offset: 0x000E65D8
	private IEnumerator DeleteDeckAnimation(long deckID, Action callback = null)
	{
		while (this.m_deletingDecks)
		{
			yield return null;
		}
		int delIndex = 0;
		TraySection delTraySection = null;
		TraySection newDeckButtonTrayLocation = this.m_traySections[0];
		for (int i = 0; i < this.m_traySections.Count; i++)
		{
			TraySection traySection = this.m_traySections[i];
			long existingDeckID = traySection.m_deckBox.GetDeckID();
			if (existingDeckID == deckID)
			{
				delIndex = i;
				delTraySection = traySection;
			}
			else if (existingDeckID == -1L)
			{
				break;
			}
			newDeckButtonTrayLocation = traySection;
		}
		if (delTraySection == null)
		{
			Debug.LogWarning("Unable to delete deck with ID {0}. Not found in tray sections.", base.gameObject);
			yield break;
		}
		this.FireBusyWithDeckEvent(true);
		this.m_deletingDecks = true;
		this.FireDeckCountChangedEvent();
		this.m_traySections.RemoveAt(delIndex);
		Vector3 newDeckBtnPos;
		bool newDeckBtnActive;
		this.GetIdealNewDeckButtonLocalPosition(newDeckButtonTrayLocation, out newDeckBtnPos, out newDeckBtnActive);
		Vector3 prevTraySectionPosition = delTraySection.transform.localPosition;
		if (HeroPickerDisplay.Get() == null || !HeroPickerDisplay.Get().IsShown())
		{
			SoundManager.Get().LoadAndPlay("collection_manager_delete_deck", base.gameObject);
		}
		this.m_deleteDeckPoof.transform.position = delTraySection.m_deckBox.transform.position;
		this.m_deleteDeckPoof.Play(true);
		delTraySection.ClearDeckInfo();
		delTraySection.gameObject.SetActive(false);
		int itemsToMove = this.m_traySections.Count - delIndex + 1;
		Action<object> onAnimationsComplete = delegate(object _1)
		{
			itemsToMove--;
			if (itemsToMove > 0)
			{
				return;
			}
			this.ShowNewDeckButton(newDeckBtnActive, null);
			this.FireBusyWithDeckEvent(false);
			if (callback != null)
			{
				callback.Invoke();
			}
			this.m_deletingDecks = false;
		};
		for (int j = delIndex; j < this.m_traySections.Count; j++)
		{
			TraySection traySection2 = this.m_traySections[j];
			Vector3 traySectionPos = traySection2.transform.localPosition;
			iTween.MoveTo(traySection2.gameObject, iTween.Hash(new object[]
			{
				"position",
				prevTraySectionPosition,
				"isLocal",
				true,
				"time",
				0.5f,
				"easeType",
				iTween.EaseType.easeOutBounce,
				"oncomplete",
				onAnimationsComplete,
				"name",
				"position"
			}));
			prevTraySectionPosition = traySectionPos;
		}
		this.m_traySections.Add(delTraySection);
		this.m_newDeckButton.SetIsUsable(this.CanShowNewDeckButton());
		delTraySection.gameObject.SetActive(true);
		delTraySection.HideDeckBox(true, null);
		delTraySection.transform.localPosition = prevTraySectionPosition;
		if (this.m_newDeckButton.gameObject.activeSelf)
		{
			iTween.MoveTo(this.m_newDeckButtonContainer, iTween.Hash(new object[]
			{
				"position",
				newDeckBtnPos,
				"isLocal",
				true,
				"time",
				0.5f,
				"easeType",
				iTween.EaseType.easeOutBounce,
				"oncomplete",
				onAnimationsComplete,
				"name",
				"position"
			}));
		}
		else
		{
			this.m_newDeckButtonContainer.transform.localPosition = newDeckBtnPos;
			onAnimationsComplete.Invoke(null);
		}
		yield break;
	}

	// Token: 0x06002E50 RID: 11856 RVA: 0x000E8410 File Offset: 0x000E6610
	private void FireDeckCountChangedEvent()
	{
		DeckTrayDeckListContent.DeckCountChanged[] array = this.m_deckCountChangedListeners.ToArray();
		int count = CollectionManager.Get().GetDecks(1).Count;
		foreach (DeckTrayDeckListContent.DeckCountChanged deckCountChanged in array)
		{
			deckCountChanged(count);
		}
	}

	// Token: 0x06002E51 RID: 11857 RVA: 0x000E8460 File Offset: 0x000E6660
	private void FireBusyWithDeckEvent(bool busy)
	{
		DeckTrayDeckListContent.BusyWithDeck[] array = this.m_busyWithDeckListeners.ToArray();
		foreach (DeckTrayDeckListContent.BusyWithDeck busyWithDeck in array)
		{
			busyWithDeck(busy);
		}
	}

	// Token: 0x06002E52 RID: 11858 RVA: 0x000E849C File Offset: 0x000E669C
	private int GetTotalDeckBoxesInUse()
	{
		int num = 0;
		foreach (TraySection traySection in this.m_traySections)
		{
			if (traySection.m_deckBox.IsShown())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x000E8508 File Offset: 0x000E6708
	private void OnFavoriteHeroChanged(TAG_CLASS heroClass, NetCache.CardDefinition favoriteHero, object userData)
	{
		this.UpdateDeckTrayVisuals();
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x000E8514 File Offset: 0x000E6714
	private int UpdateDeckTrayVisuals()
	{
		List<CollectionDeck> decks;
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
		{
			decks = CollectionManager.Get().GetDecks(6);
		}
		else
		{
			decks = CollectionManager.Get().GetDecks(1);
		}
		int num = decks.Count;
		if (!AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES))
		{
			num = 9;
		}
		for (int i = 0; i < num; i++)
		{
			if (i >= this.m_traySections.Count)
			{
				break;
			}
			if (i < decks.Count)
			{
				CollectionDeck deck = decks[i];
				this.m_traySections[i].m_deckBox.AssignFromCollectionDeck(deck, false);
			}
			this.m_traySections[i].m_deckBox.SetIsLocked(i >= decks.Count);
		}
		return decks.Count;
	}

	// Token: 0x04001CAA RID: 7338
	private const float DECK_BUTTON_ROTATION_TIME = 0.1f;

	// Token: 0x04001CAB RID: 7339
	private const int MAX_NUM_DECKBOXES_AVAILABLE = 18;

	// Token: 0x04001CAC RID: 7340
	private const int NUM_DECKBOXES_TO_DISPLAY = 20;

	// Token: 0x04001CAD RID: 7341
	private const float TIME_BETWEEN_TRAY_DOOR_ANIMS = 0.015f;

	// Token: 0x04001CAE RID: 7342
	private const float DELETE_DECK_ANIM_TIME = 0.5f;

	// Token: 0x04001CAF RID: 7343
	[CustomEditField(Sections = "Deck Tray Settings")]
	public Transform m_deckEditTopPos;

	// Token: 0x04001CB0 RID: 7344
	[CustomEditField(Sections = "Deck Tray Settings")]
	public Transform m_traySectionStartPos;

	// Token: 0x04001CB1 RID: 7345
	[CustomEditField(Sections = "Deck Tray Settings")]
	public GameObject m_deckInfoTooltipBone;

	// Token: 0x04001CB2 RID: 7346
	[CustomEditField(Sections = "Deck Tray Settings")]
	public GameObject m_deckOptionsBone;

	// Token: 0x04001CB3 RID: 7347
	[SerializeField]
	private Vector3 m_deckButtonOffset;

	// Token: 0x04001CB4 RID: 7348
	[CustomEditField(Sections = "Deck Button Settings")]
	public GameObject m_newDeckButtonContainer;

	// Token: 0x04001CB5 RID: 7349
	[CustomEditField(Sections = "Deck Button Settings")]
	public CollectionNewDeckButton m_newDeckButton;

	// Token: 0x04001CB6 RID: 7350
	[CustomEditField(Sections = "Deck Button Settings")]
	public ParticleSystem m_deleteDeckPoof;

	// Token: 0x04001CB7 RID: 7351
	[CustomEditField(Sections = "Prefabs")]
	public TraySection m_traySectionPrefab;

	// Token: 0x04001CB8 RID: 7352
	[CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
	public string m_deckInfoActorPrefab;

	// Token: 0x04001CB9 RID: 7353
	[CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
	public string m_deckOptionsPrefab;

	// Token: 0x04001CBA RID: 7354
	[CustomEditField(Sections = "Scroll Settings")]
	public UIBScrollable m_scrollbar;

	// Token: 0x04001CBB RID: 7355
	private CollectionDeckInfo m_deckInfoTooltip;

	// Token: 0x04001CBC RID: 7356
	private bool m_initialized;

	// Token: 0x04001CBD RID: 7357
	private List<TraySection> m_traySections = new List<TraySection>();

	// Token: 0x04001CBE RID: 7358
	private TraySection m_editingTraySection;

	// Token: 0x04001CBF RID: 7359
	private TraySection m_newlyCreatedTraySection;

	// Token: 0x04001CC0 RID: 7360
	private bool m_animatingExit;

	// Token: 0x04001CC1 RID: 7361
	private bool m_deletingDecks;

	// Token: 0x04001CC2 RID: 7362
	private bool m_wasTouchModeEnabled;

	// Token: 0x04001CC3 RID: 7363
	private List<long> m_decksToDelete = new List<long>();

	// Token: 0x04001CC4 RID: 7364
	private string m_previousDeckName;

	// Token: 0x04001CC5 RID: 7365
	private List<DeckTrayDeckListContent.DeckCountChanged> m_deckCountChangedListeners = new List<DeckTrayDeckListContent.DeckCountChanged>();

	// Token: 0x04001CC6 RID: 7366
	private List<DeckTrayDeckListContent.BusyWithDeck> m_busyWithDeckListeners = new List<DeckTrayDeckListContent.BusyWithDeck>();

	// Token: 0x04001CC7 RID: 7367
	private int m_centeringDeckList = -1;

	// Token: 0x04001CC8 RID: 7368
	private bool m_waitingToDeleteDeck;

	// Token: 0x04001CC9 RID: 7369
	private DeckOptionsMenu m_deckOptionsMenu;

	// Token: 0x04001CCA RID: 7370
	private bool m_doneEntering;

	// Token: 0x02000733 RID: 1843
	// (Invoke) Token: 0x06004B0F RID: 19215
	public delegate void DeckCountChanged(int deckCount);

	// Token: 0x02000734 RID: 1844
	// (Invoke) Token: 0x06004B13 RID: 19219
	public delegate void BusyWithDeck(bool busy);
}
