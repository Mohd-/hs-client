using System;
using System.Collections.Generic;
using System.Linq;
using bgs;
using UnityEngine;

// Token: 0x02000517 RID: 1303
public class FriendListFrame : MonoBehaviour
{
	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06003C0D RID: 15373 RVA: 0x0012216B File Offset: 0x0012036B
	// (remove) Token: 0x06003C0E RID: 15374 RVA: 0x00122184 File Offset: 0x00120384
	public event Action AddFriendFrameOpened;

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x06003C0F RID: 15375 RVA: 0x0012219D File Offset: 0x0012039D
	// (remove) Token: 0x06003C10 RID: 15376 RVA: 0x001221B6 File Offset: 0x001203B6
	public event Action AddFriendFrameClosed;

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x06003C11 RID: 15377 RVA: 0x001221CF File Offset: 0x001203CF
	// (remove) Token: 0x06003C12 RID: 15378 RVA: 0x001221E8 File Offset: 0x001203E8
	public event Action RecruitAFriendFrameOpened;

	// Token: 0x1400001A RID: 26
	// (add) Token: 0x06003C13 RID: 15379 RVA: 0x00122201 File Offset: 0x00120401
	// (remove) Token: 0x06003C14 RID: 15380 RVA: 0x0012221A File Offset: 0x0012041A
	public event Action RecruitAFriendFrameClosed;

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x06003C15 RID: 15381 RVA: 0x00122233 File Offset: 0x00120433
	// (remove) Token: 0x06003C16 RID: 15382 RVA: 0x0012224C File Offset: 0x0012044C
	public event Action RemoveFriendPopupOpened;

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x06003C17 RID: 15383 RVA: 0x00122265 File Offset: 0x00120465
	// (remove) Token: 0x06003C18 RID: 15384 RVA: 0x0012227E File Offset: 0x0012047E
	public event Action RemoveFriendPopupClosed;

	// Token: 0x17000444 RID: 1092
	// (get) Token: 0x06003C19 RID: 15385 RVA: 0x00122298 File Offset: 0x00120498
	// (set) Token: 0x06003C1A RID: 15386 RVA: 0x00122300 File Offset: 0x00120500
	public BnetPlayer SelectedPlayer
	{
		get
		{
			if (this.items.SelectedItem == null)
			{
				return null;
			}
			FriendListBaseFriendFrame component = this.items.SelectedItem.GetComponent<FriendListBaseFriendFrame>();
			if (component != null)
			{
				return component.GetFriend();
			}
			FriendListNearbyPlayerFrame component2 = this.items.SelectedItem.GetComponent<FriendListNearbyPlayerFrame>();
			if (component2 != null)
			{
				return component2.GetNearbyPlayer();
			}
			return null;
		}
		set
		{
			this.items.SelectedIndex = this.items.FindIndex(delegate(ITouchListItem item)
			{
				if (item == null)
				{
					return false;
				}
				FriendListBaseFriendFrame component = item.GetComponent<FriendListBaseFriendFrame>();
				if (component != null)
				{
					return component.GetFriend() == value;
				}
				FriendListNearbyPlayerFrame component2 = item.GetComponent<FriendListNearbyPlayerFrame>();
				return component2 != null && component2.GetNearbyPlayer() == value;
			});
			this.UpdateItems();
		}
	}

	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x06003C1B RID: 15387 RVA: 0x00122342 File Offset: 0x00120542
	public bool ShowingAddFriendFrame
	{
		get
		{
			return this.m_addFriendFrame != null;
		}
	}

	// Token: 0x17000446 RID: 1094
	// (get) Token: 0x06003C1C RID: 15388 RVA: 0x00122350 File Offset: 0x00120550
	public bool IsInEditMode
	{
		get
		{
			return this.m_editFriendsMode;
		}
	}

	// Token: 0x06003C1D RID: 15389 RVA: 0x00122358 File Offset: 0x00120558
	private void Awake()
	{
		this.myPortrait = this.me.portraitRef.Spawn<PlayerPortrait>();
		this.recentOpponent.button.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRecentOpponentButtonReleased));
		this.InitButtons();
		this.RegisterFriendEvents();
		this.CreateItemsCamera();
		this.UpdateBackgroundCollider();
		bool active = !UniversalInputManager.Get().IsTouchMode() || PlatformSettings.OS == OSCategory.PC;
		if (this.scrollbar != null)
		{
			this.scrollbar.gameObject.SetActive(active);
		}
		this.me.m_Medal.m_legendIndex.RenderToTexture = false;
		this.me.m_Medal.m_legendIndex.TextColor = new Color(0.97f, 0.98f, 0.7f, 1f);
		if (BnetFriendMgr.Get().HasOnlineFriends() || BnetNearbyPlayerMgr.Get().HasNearbyStrangers())
		{
			CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded(null);
		}
	}

	// Token: 0x06003C1E RID: 15390 RVA: 0x00122462 File Offset: 0x00120662
	private void Start()
	{
		this.UpdateMyself();
		this.UpdateRecentOpponent();
		this.InitItems();
	}

	// Token: 0x06003C1F RID: 15391 RVA: 0x00122478 File Offset: 0x00120678
	private void OnDestroy()
	{
		this.UnregisterFriendEvents();
		this.CloseAddFriendFrame();
		if (this.m_longListBehavior != null && this.m_longListBehavior.FreeList != null)
		{
			foreach (MobileFriendListItem mobileFriendListItem in this.m_longListBehavior.FreeList)
			{
				if (mobileFriendListItem != null)
				{
					Object.Destroy(mobileFriendListItem.gameObject);
				}
			}
		}
		foreach (FriendListItemHeader friendListItemHeader in this.m_headers.Values)
		{
			if (friendListItemHeader != null)
			{
				Object.Destroy(friendListItemHeader.gameObject);
			}
		}
	}

	// Token: 0x06003C20 RID: 15392 RVA: 0x0012256C File Offset: 0x0012076C
	private void Update()
	{
		this.HandleKeyboardInput();
		if (this.m_nearbyPlayersNeedUpdate && Time.realtimeSinceStartup >= this.m_lastNearbyPlayersUpdate + 10f)
		{
			this.HandleNearbyPlayersChanged();
		}
		this.UpdateButtonGlows();
	}

	// Token: 0x06003C21 RID: 15393 RVA: 0x001225AD File Offset: 0x001207AD
	private void UpdateButtonGlows()
	{
		this.removeFriendButton.ShowActiveGlow(this.IsInEditMode);
	}

	// Token: 0x06003C22 RID: 15394 RVA: 0x001225C0 File Offset: 0x001207C0
	private void OnEnable()
	{
		if (this.m_nearbyPlayersNeedUpdate)
		{
			this.HandleNearbyPlayersChanged();
		}
		if (this.m_playersChangeList.GetChanges().Count > 0)
		{
			this.DoPlayersChanged(this.m_playersChangeList);
			this.m_playersChangeList.GetChanges().Clear();
		}
		if (this.items.IsInitialized)
		{
			this.ResumeItemsLayout();
		}
		this.UpdateMyself();
		this.items.ResetState();
		this.m_editFriendsMode = false;
		this.m_friendToRemove = null;
	}

	// Token: 0x06003C23 RID: 15395 RVA: 0x00122648 File Offset: 0x00120848
	public void SetWorldRect(float x, float y, float width, float height)
	{
		bool activeSelf = base.gameObject.activeSelf;
		base.gameObject.SetActive(true);
		this.window.SetEntireSize(width, height);
		Bounds bounds = TransformUtil.ComputeSetPointBounds(this.window);
		Vector3 vector = TransformUtil.ComputeWorldPoint(bounds, new Vector3(0f, 1f, 0f));
		Vector3 vector2 = new Vector3(x, y, vector.z) - vector;
		base.transform.Translate(vector2);
		this.UpdateItemsList();
		this.UpdateItemsCamera();
		this.UpdateBackgroundCollider();
		this.UpdateDropShadow();
		base.gameObject.SetActive(activeSelf);
	}

	// Token: 0x06003C24 RID: 15396 RVA: 0x001226E7 File Offset: 0x001208E7
	public void SetWorldPosition(float x, float y)
	{
		this.SetWorldPosition(new Vector3(x, y));
	}

	// Token: 0x06003C25 RID: 15397 RVA: 0x001226F8 File Offset: 0x001208F8
	public void SetWorldPosition(Vector3 pos)
	{
		bool activeSelf = base.gameObject.activeSelf;
		base.gameObject.SetActive(true);
		base.transform.position = pos;
		this.UpdateItemsList();
		this.UpdateItemsCamera();
		this.UpdateBackgroundCollider();
		base.gameObject.SetActive(activeSelf);
	}

	// Token: 0x06003C26 RID: 15398 RVA: 0x00122748 File Offset: 0x00120948
	public void SetWorldHeight(float height)
	{
		bool activeSelf = base.gameObject.activeSelf;
		base.gameObject.SetActive(true);
		this.window.SetEntireHeight(height);
		this.UpdateItemsList();
		this.UpdateItemsCamera();
		this.UpdateBackgroundCollider();
		this.UpdateDropShadow();
		base.gameObject.SetActive(activeSelf);
	}

	// Token: 0x06003C27 RID: 15399 RVA: 0x001227A0 File Offset: 0x001209A0
	public void ShowAddFriendFrame(BnetPlayer player = null)
	{
		this.m_addFriendFrame = Object.Instantiate<AddFriendFrame>(this.prefabs.addFriendFrame);
		this.m_addFriendFrame.Closed += new Action(this.CloseAddFriendFrame);
		this.RecruitAFriend_OnClosed();
		if (player != null)
		{
			this.m_addFriendFrame.SetPlayer(player);
		}
	}

	// Token: 0x06003C28 RID: 15400 RVA: 0x001227F4 File Offset: 0x001209F4
	public void CloseAddFriendFrame()
	{
		if (this.m_addFriendFrame == null)
		{
			return;
		}
		this.m_addFriendFrame.Close();
		if (this.AddFriendFrameClosed != null)
		{
			this.AddFriendFrameClosed.Invoke();
		}
		this.m_addFriendFrame = null;
	}

	// Token: 0x06003C29 RID: 15401 RVA: 0x0012283B File Offset: 0x00120A3B
	public void ShowRecruitAFriendFrame()
	{
		this.m_recruitAFriendFrame = Object.Instantiate<RecruitAFriendFrame>(this.prefabs.recruitAFriendFrame);
		this.m_recruitAFriendFrame.Closed += new Action(this.RecruitAFriend_OnClosed);
		this.CloseAddFriendFrame();
	}

	// Token: 0x06003C2A RID: 15402 RVA: 0x00122870 File Offset: 0x00120A70
	private void RecruitAFriend_OnClosed()
	{
		if (this.m_recruitAFriendFrame == null)
		{
			return;
		}
		this.m_recruitAFriendFrame.Close();
		if (this.RecruitAFriendFrameClosed != null)
		{
			this.RecruitAFriendFrameClosed.Invoke();
		}
		this.m_recruitAFriendFrame = null;
	}

	// Token: 0x06003C2B RID: 15403 RVA: 0x001228B8 File Offset: 0x00120AB8
	public void ShowRemoveFriendPopup(BnetPlayer friend)
	{
		this.m_friendToRemove = friend;
		if (this.m_friendToRemove == null)
		{
			return;
		}
		string uniqueName = FriendUtils.GetUniqueName(this.m_friendToRemove);
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_text = GameStrings.Format("GLOBAL_FRIENDLIST_REMOVE_FRIEND_ALERT_MESSAGE", new object[]
		{
			uniqueName
		});
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnRemoveFriendPopupResponse);
		popupInfo.m_layerToUse = new GameLayer?(GameLayer.HighPriorityUI);
		DialogManager.Get().ShowPopup(popupInfo, new DialogManager.DialogProcessCallback(this.OnRemoveFriendDialogShown), this.m_friendToRemove);
		if (this.RemoveFriendPopupOpened != null)
		{
			this.RemoveFriendPopupOpened.Invoke();
		}
	}

	// Token: 0x06003C2C RID: 15404 RVA: 0x00122964 File Offset: 0x00120B64
	private void CreateItemsCamera()
	{
		this.m_itemsCamera = new GameObject("ItemsCamera")
		{
			transform = 
			{
				parent = this.items.transform,
				localPosition = new Vector3(0f, 0f, -100f)
			}
		}.AddComponent<Camera>();
		this.m_itemsCamera.orthographic = true;
		this.m_itemsCamera.depth = (float)(BnetBar.CameraDepth + 1);
		this.m_itemsCamera.clearFlags = 3;
		this.m_itemsCamera.cullingMask = GameLayer.BattleNetFriendList.LayerBit();
		this.UpdateItemsCamera();
	}

	// Token: 0x06003C2D RID: 15405 RVA: 0x00122A00 File Offset: 0x00120C00
	private void UpdateItemsList()
	{
		Transform bottomRightBone = this.GetBottomRightBone();
		this.items.transform.position = (this.listInfo.topLeft.position + bottomRightBone.position) / 2f;
		Vector3 vector = bottomRightBone.position - this.listInfo.topLeft.position;
		this.items.ClipSize = new Vector2(vector.x, Math.Abs(vector.y));
		if (this.innerShadow != null)
		{
			this.innerShadow.transform.position = this.items.transform.position;
			Vector3 vector2 = this.GetBottomRightBone().position - this.listInfo.topLeft.position;
			TransformUtil.SetLocalScaleToWorldDimension(this.innerShadow, new WorldDimensionIndex[]
			{
				new WorldDimensionIndex(Mathf.Abs(vector2.x), 0),
				new WorldDimensionIndex(Mathf.Abs(vector2.y), 2)
			});
		}
	}

	// Token: 0x06003C2E RID: 15406 RVA: 0x00122B28 File Offset: 0x00120D28
	private void UpdateItemsCamera()
	{
		Camera bnetCamera = BaseUI.Get().GetBnetCamera();
		Transform bottomRightBone = this.GetBottomRightBone();
		Vector3 vector = bnetCamera.WorldToScreenPoint(this.listInfo.topLeft.position);
		Vector3 vector2 = bnetCamera.WorldToScreenPoint(bottomRightBone.position);
		GeneralUtils.Swap<float>(ref vector.y, ref vector2.y);
		this.m_itemsCamera.pixelRect = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
		this.m_itemsCamera.orthographicSize = this.m_itemsCamera.rect.height * bnetCamera.orthographicSize;
	}

	// Token: 0x06003C2F RID: 15407 RVA: 0x00122BE0 File Offset: 0x00120DE0
	private void UpdateBackgroundCollider()
	{
		Renderer[] componentsInChildren = this.window.GetComponentsInChildren<Renderer>();
		Bounds bounds = Enumerable.Aggregate<Renderer, Bounds>(componentsInChildren, new Bounds(base.transform.position, Vector3.zero), delegate(Bounds aggregate, Renderer renderer)
		{
			aggregate.Encapsulate(renderer.bounds);
			return aggregate;
		});
		Vector3 vector = base.transform.InverseTransformPoint(bounds.min);
		Vector3 vector2 = base.transform.InverseTransformPoint(bounds.max);
		BoxCollider boxCollider = base.GetComponent<BoxCollider>();
		if (boxCollider == null)
		{
			boxCollider = base.gameObject.AddComponent<BoxCollider>();
		}
		boxCollider.center = (vector + vector2) / 2f + Vector3.forward;
		boxCollider.size = vector2 - vector;
	}

	// Token: 0x06003C30 RID: 15408 RVA: 0x00122CAA File Offset: 0x00120EAA
	private void UpdateDropShadow()
	{
		if (this.outerShadow == null)
		{
			return;
		}
		this.outerShadow.SetActive(!UniversalInputManager.Get().IsTouchMode());
	}

	// Token: 0x06003C31 RID: 15409 RVA: 0x00122CD8 File Offset: 0x00120ED8
	private void UpdateMyself()
	{
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		if (myPlayer != null && myPlayer.IsDisplayable())
		{
			BnetBattleTag battleTag = myPlayer.GetBattleTag();
			this.myPortrait.SetProgramId(BnetProgramId.HEARTHSTONE);
			this.me.nameText.Text = battleTag.GetName();
			this.me.numberText.Text = string.Format("#{0}", battleTag.GetNumber().ToString());
			this.me.statusText.Text = GameStrings.Get("GLOBAL_FRIENDLIST_MYSTATUS");
			TransformUtil.SetPoint(this.me.numberText, Anchor.LEFT, this.me.nameText, Anchor.RIGHT, 6f * Vector3.right);
			NetCache.NetCacheMedalInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMedalInfo>();
			MedalInfoTranslator medalInfoTranslator = new MedalInfoTranslator(netObject);
			if (medalInfoTranslator == null || medalInfoTranslator.GetCurrentMedal(medalInfoTranslator.IsBestCurrentRankWild()).rank == 25)
			{
				this.me.m_MedalPatch.SetActive(false);
				this.myPortrait.gameObject.SetActive(true);
				if (this.portraitBackground != null)
				{
					Material[] materials = this.portraitBackground.GetComponent<Renderer>().materials;
					materials[0] = this.unrankedBackground;
					this.portraitBackground.GetComponent<Renderer>().materials = materials;
				}
			}
			else
			{
				this.myPortrait.gameObject.SetActive(false);
				this.me.m_Medal.SetEnabled(false);
				this.me.m_Medal.SetMedal(medalInfoTranslator, false);
				this.me.m_Medal.SetFormat(medalInfoTranslator.IsBestCurrentRankWild());
				this.me.m_MedalPatch.SetActive(true);
				if (this.portraitBackground != null)
				{
					Material[] materials2 = this.portraitBackground.GetComponent<Renderer>().materials;
					materials2[0] = this.rankedBackground;
					this.portraitBackground.GetComponent<Renderer>().materials = materials2;
				}
			}
		}
		else
		{
			this.me.nameText.Text = string.Empty;
			this.me.numberText.Text = string.Empty;
			this.me.statusText.Text = string.Empty;
		}
	}

	// Token: 0x06003C32 RID: 15410 RVA: 0x00122F14 File Offset: 0x00121114
	private void UpdateRecentOpponent()
	{
		BnetPlayer bnetPlayer = FriendMgr.Get().GetRecentOpponent();
		if (bnetPlayer == null)
		{
			this.recentOpponent.button.gameObject.SetActive(false);
		}
		else
		{
			this.recentOpponent.button.gameObject.SetActive(true);
			this.recentOpponent.nameText.Text = FriendUtils.GetUniqueNameWithColor(bnetPlayer);
		}
	}

	// Token: 0x06003C33 RID: 15411 RVA: 0x00122F7C File Offset: 0x0012117C
	private void InitItems()
	{
		BnetFriendMgr bnetFriendMgr = BnetFriendMgr.Get();
		BnetNearbyPlayerMgr bnetNearbyPlayerMgr = BnetNearbyPlayerMgr.Get();
		this.items.SelectionEnabled = true;
		this.items.SelectedIndexChanging += ((int index) => index != -1);
		this.SuspendItemsLayout();
		this.UpdateRequests(bnetFriendMgr.GetReceivedInvites(), null);
		this.UpdateAllFriends(bnetFriendMgr.GetFriends(), null);
		this.UpdateAllNearbyPlayers(bnetNearbyPlayerMgr.GetNearbyStrangers(), null);
		this.UpdateAllNearbyFriends(bnetNearbyPlayerMgr.GetNearbyFriends(), null);
		this.UpdateAllRecruits();
		this.UpdateAllHeaders();
		this.ResumeItemsLayout();
		this.UpdateAllHeaderBackgrounds();
		this.UpdateSelectedItem();
	}

	// Token: 0x06003C34 RID: 15412 RVA: 0x00123024 File Offset: 0x00121224
	private void UpdateItems()
	{
		foreach (FriendListRequestFrame friendListRequestFrame in this.GetItems<FriendListRequestFrame>())
		{
			friendListRequestFrame.UpdateInvite();
		}
		this.UpdateFriendItems();
	}

	// Token: 0x06003C35 RID: 15413 RVA: 0x00123080 File Offset: 0x00121280
	private void UpdateFriendItems()
	{
		foreach (FriendListCurrentGameFrame friendListCurrentGameFrame in this.GetItems<FriendListCurrentGameFrame>())
		{
			friendListCurrentGameFrame.UpdateFriend();
		}
		foreach (FriendListFriendFrame friendListFriendFrame in this.GetItems<FriendListFriendFrame>())
		{
			friendListFriendFrame.UpdateFriend();
		}
	}

	// Token: 0x06003C36 RID: 15414 RVA: 0x0012311C File Offset: 0x0012131C
	private void UpdateNearbyPlayerItems()
	{
		foreach (FriendListNearbyPlayerFrame friendListNearbyPlayerFrame in this.GetItems<FriendListNearbyPlayerFrame>())
		{
			friendListNearbyPlayerFrame.UpdateNearbyPlayer();
		}
	}

	// Token: 0x06003C37 RID: 15415 RVA: 0x00123174 File Offset: 0x00121374
	private void UpdateRecruitItems()
	{
		foreach (FriendListRecruitFrame friendListRecruitFrame in this.GetItems<FriendListRecruitFrame>())
		{
			friendListRecruitFrame.UpdateRecruit();
		}
	}

	// Token: 0x06003C38 RID: 15416 RVA: 0x001231CC File Offset: 0x001213CC
	private void UpdateRequests(List<BnetInvitation> addedList, List<BnetInvitation> removedList)
	{
		if (removedList == null && addedList == null)
		{
			return;
		}
		if (removedList != null)
		{
			foreach (BnetInvitation itemToRemove in removedList)
			{
				this.RemoveItem(false, MobileFriendListItem.TypeFlags.Request, itemToRemove);
			}
		}
		foreach (FriendListRequestFrame friendListRequestFrame in this.GetItems<FriendListRequestFrame>())
		{
			friendListRequestFrame.UpdateInvite();
		}
		if (addedList != null)
		{
			foreach (BnetInvitation itemData in addedList)
			{
				FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.Request, itemData);
				this.m_allItems.Add(friendListItem);
			}
		}
	}

	// Token: 0x06003C39 RID: 15417 RVA: 0x001232E4 File Offset: 0x001214E4
	private void UpdateAllFriends(List<BnetPlayer> addedList, List<BnetPlayer> removedList)
	{
		if (removedList == null && addedList == null)
		{
			return;
		}
		if (removedList != null)
		{
			foreach (BnetPlayer itemToRemove in removedList)
			{
				if (!this.RemoveItem(false, MobileFriendListItem.TypeFlags.Friend, itemToRemove))
				{
					if (this.RemoveItem(false, MobileFriendListItem.TypeFlags.CurrentGame, itemToRemove))
					{
					}
				}
			}
		}
		this.UpdateFriendItems();
		if (addedList != null)
		{
			foreach (BnetPlayer bnetPlayer in addedList)
			{
				FriendListFrame.FriendListItem friendListItem;
				if (bnetPlayer.GetPersistentGameId() == 0L)
				{
					friendListItem = new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.Friend, bnetPlayer);
				}
				else
				{
					friendListItem = new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.CurrentGame, bnetPlayer);
				}
				this.m_allItems.Add(friendListItem);
			}
		}
		this.SortAndRefreshTouchList();
	}

	// Token: 0x06003C3A RID: 15418 RVA: 0x001233F4 File Offset: 0x001215F4
	private void UpdateAllNearbyPlayers(List<BnetPlayer> addedList, List<BnetPlayer> removedList)
	{
		if (removedList != null)
		{
			foreach (BnetPlayer itemToRemove in removedList)
			{
				this.RemoveItem(false, MobileFriendListItem.TypeFlags.NearbyPlayer, itemToRemove);
			}
		}
		this.UpdateNearbyPlayerItems();
		if (addedList != null)
		{
			foreach (BnetPlayer itemData in addedList)
			{
				FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.NearbyPlayer, itemData);
				this.m_allItems.Add(friendListItem);
			}
		}
		this.SortAndRefreshTouchList();
	}

	// Token: 0x06003C3B RID: 15419 RVA: 0x001234BC File Offset: 0x001216BC
	private void UpdateAllNearbyFriends(List<BnetPlayer> addedList, List<BnetPlayer> removedList)
	{
		if (removedList != null)
		{
			foreach (BnetPlayer itemToRemove in removedList)
			{
				this.RemoveItem(false, MobileFriendListItem.TypeFlags.NearbyPlayer, itemToRemove);
			}
		}
		this.UpdateNearbyPlayerItems();
		if (addedList != null)
		{
			foreach (BnetPlayer itemData in addedList)
			{
				FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.NearbyPlayer, itemData);
				this.m_allItems.Add(friendListItem);
			}
		}
		this.SortAndRefreshTouchList();
	}

	// Token: 0x06003C3C RID: 15420 RVA: 0x00123584 File Offset: 0x00121784
	private void UpdateAllRecruits()
	{
		List<Network.RecruitInfo> recruitList = RecruitListMgr.Get().GetRecruitList();
		List<Network.RecruitInfo> list = new List<Network.RecruitInfo>();
		List<Network.RecruitInfo> list2 = new List<Network.RecruitInfo>();
		List<Network.RecruitInfo> list3 = new List<Network.RecruitInfo>();
		foreach (FriendListRecruitFrame friendListRecruitFrame in this.GetItems<FriendListRecruitFrame>())
		{
			list3.Add(friendListRecruitFrame.GetRecruitInfo());
		}
		foreach (Network.RecruitInfo recruitInfo in list3)
		{
			if (!recruitList.Contains(recruitInfo))
			{
				list2.Add(recruitInfo);
			}
		}
		Network.RecruitInfo info;
		foreach (Network.RecruitInfo info2 in recruitList)
		{
			info = info2;
			if (!(this.FindFirstItem<FriendListFriendFrame>((FriendListFriendFrame frame) => frame.GetFriend().GetAccountId() == info.RecruitID) != null))
			{
				if (!list3.Contains(info))
				{
					list.Add(info);
				}
			}
		}
		foreach (Network.RecruitInfo itemToRemove in list2)
		{
			this.RemoveItem(false, MobileFriendListItem.TypeFlags.Recruit, itemToRemove);
		}
		foreach (Network.RecruitInfo itemData in list)
		{
			FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.Recruit, itemData);
			this.m_allItems.Add(friendListItem);
		}
		this.UpdateRecruitItems();
		this.SortAndRefreshTouchList();
	}

	// Token: 0x06003C3D RID: 15421 RVA: 0x00123798 File Offset: 0x00121998
	private FriendListBaseFriendFrame FindBaseFriendFrame(BnetPlayer friend)
	{
		return this.FindFirstItem<FriendListBaseFriendFrame>((FriendListBaseFriendFrame frame) => frame.GetFriend() == friend);
	}

	// Token: 0x06003C3E RID: 15422 RVA: 0x001237C4 File Offset: 0x001219C4
	private FriendListCurrentGameFrame FindCurrentGameFrame(BnetPlayer friend)
	{
		return this.FindFirstItem<FriendListCurrentGameFrame>((FriendListCurrentGameFrame frame) => frame.GetFriend() == friend);
	}

	// Token: 0x06003C3F RID: 15423 RVA: 0x001237F0 File Offset: 0x001219F0
	private FriendListFriendFrame FindFriendFrame(BnetPlayer friend)
	{
		return this.FindFirstItem<FriendListFriendFrame>((FriendListFriendFrame frame) => frame.GetFriend() == friend);
	}

	// Token: 0x06003C40 RID: 15424 RVA: 0x0012381C File Offset: 0x00121A1C
	private FriendListFriendFrame FindFriendFrame(BnetAccountId id)
	{
		return this.FindFirstItem<FriendListFriendFrame>((FriendListFriendFrame frame) => frame.GetFriend().GetAccountId() == id);
	}

	// Token: 0x06003C41 RID: 15425 RVA: 0x00123848 File Offset: 0x00121A48
	private MobileFriendListItem CreateFriendFrame(BnetPlayer friend)
	{
		FriendListFriendFrame friendListFriendFrame = Object.Instantiate<FriendListFriendFrame>(this.prefabs.friendItem);
		UberText[] objs = UberText.EnableAllTextInObject(friendListFriendFrame.gameObject, false);
		friendListFriendFrame.SetFriend(friend);
		FriendListUIElement component = friendListFriendFrame.GetComponent<FriendListUIElement>();
		component.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBaseFriendFrameReleased));
		MobileFriendListItem result = this.FinishCreateVisualItem<FriendListFriendFrame>(friendListFriendFrame, MobileFriendListItem.TypeFlags.Friend, this.FindHeader(MobileFriendListItem.TypeFlags.Friend), friendListFriendFrame.gameObject);
		UberText.EnableAllTextObjects(objs, true);
		return result;
	}

	// Token: 0x06003C42 RID: 15426 RVA: 0x001238B8 File Offset: 0x00121AB8
	private MobileFriendListItem CreateNearbyPlayerFrame(BnetPlayer friend)
	{
		FriendListNearbyPlayerFrame friendListNearbyPlayerFrame = Object.Instantiate<FriendListNearbyPlayerFrame>(this.prefabs.nearbyPlayerItem);
		UberText[] objs = UberText.EnableAllTextInObject(friendListNearbyPlayerFrame.gameObject, false);
		friendListNearbyPlayerFrame.SetNearbyPlayer(friend);
		FriendListUIElement component = friendListNearbyPlayerFrame.GetComponent<FriendListUIElement>();
		component.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnNearbyPlayerFrameReleased));
		MobileFriendListItem result = this.FinishCreateVisualItem<FriendListNearbyPlayerFrame>(friendListNearbyPlayerFrame, MobileFriendListItem.TypeFlags.NearbyPlayer, this.FindHeader(MobileFriendListItem.TypeFlags.NearbyPlayer), friendListNearbyPlayerFrame.gameObject);
		UberText.EnableAllTextObjects(objs, true);
		return result;
	}

	// Token: 0x06003C43 RID: 15427 RVA: 0x00123928 File Offset: 0x00121B28
	private MobileFriendListItem CreateRecruitFrame(Network.RecruitInfo info)
	{
		FriendListRecruitFrame friendListRecruitFrame = Object.Instantiate<FriendListRecruitFrame>(this.prefabs.recruitItem);
		UberText[] objs = UberText.EnableAllTextInObject(friendListRecruitFrame.gameObject, false);
		friendListRecruitFrame.SetRecruitInfo(info);
		MobileFriendListItem result = this.FinishCreateVisualItem<FriendListRecruitFrame>(friendListRecruitFrame, MobileFriendListItem.TypeFlags.Recruit, this.FindHeader(MobileFriendListItem.TypeFlags.Recruit), friendListRecruitFrame.gameObject);
		UberText.EnableAllTextObjects(objs, true);
		return result;
	}

	// Token: 0x06003C44 RID: 15428 RVA: 0x00123978 File Offset: 0x00121B78
	private MobileFriendListItem CreateCurrentGameFrame(BnetPlayer friend)
	{
		FriendListCurrentGameFrame friendListCurrentGameFrame = Object.Instantiate<FriendListCurrentGameFrame>(this.prefabs.currentGameItem);
		UberText[] objs = UberText.EnableAllTextInObject(friendListCurrentGameFrame.gameObject, false);
		friendListCurrentGameFrame.SetFriend(friend);
		FriendListUIElement component = friendListCurrentGameFrame.GetComponent<FriendListUIElement>();
		component.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBaseFriendFrameReleased));
		MobileFriendListItem result = this.FinishCreateVisualItem<FriendListCurrentGameFrame>(friendListCurrentGameFrame, MobileFriendListItem.TypeFlags.CurrentGame, this.FindHeader(MobileFriendListItem.TypeFlags.CurrentGame), friendListCurrentGameFrame.gameObject);
		UberText.EnableAllTextObjects(objs, true);
		return result;
	}

	// Token: 0x06003C45 RID: 15429 RVA: 0x001239E8 File Offset: 0x00121BE8
	private MobileFriendListItem CreateRequestFrame(BnetInvitation invite)
	{
		FriendListRequestFrame friendListRequestFrame = Object.Instantiate<FriendListRequestFrame>(this.prefabs.requestItem);
		UberText[] objs = UberText.EnableAllTextInObject(friendListRequestFrame.gameObject, false);
		friendListRequestFrame.SetInvite(invite);
		MobileFriendListItem result = this.FinishCreateVisualItem<FriendListRequestFrame>(friendListRequestFrame, MobileFriendListItem.TypeFlags.Request, this.FindHeader(MobileFriendListItem.TypeFlags.Request), friendListRequestFrame.gameObject);
		UberText.EnableAllTextObjects(objs, true);
		return result;
	}

	// Token: 0x06003C46 RID: 15430 RVA: 0x00123A40 File Offset: 0x00121C40
	private void UpdateAllHeaders()
	{
		this.UpdateRequestsHeader(null);
		this.UpdateCurrentGamesHeader();
		this.UpdateNearbyPlayersHeader(null);
		this.UpdateFriendsHeader(null);
		this.UpdateRecruitsHeader();
	}

	// Token: 0x06003C47 RID: 15431 RVA: 0x00123A70 File Offset: 0x00121C70
	private void UpdateAllHeaderBackgrounds()
	{
		this.UpdateHeaderBackground(this.FindHeader(MobileFriendListItem.TypeFlags.Request));
		this.UpdateHeaderBackground(this.FindHeader(MobileFriendListItem.TypeFlags.CurrentGame));
	}

	// Token: 0x06003C48 RID: 15432 RVA: 0x00123A9C File Offset: 0x00121C9C
	private void UpdateRequestsHeader(FriendListItemHeader header = null)
	{
		int num = Enumerable.Count<FriendListFrame.FriendListItem>(this.m_allItems, (FriendListFrame.FriendListItem i) => i.ItemMainType == MobileFriendListItem.TypeFlags.Request);
		if (num > 0)
		{
			string text = GameStrings.Format("GLOBAL_FRIENDLIST_REQUESTS_HEADER", new object[]
			{
				num
			});
			if (header == null)
			{
				header = this.FindOrAddHeader(MobileFriendListItem.TypeFlags.Request);
				if (!Enumerable.Any<FriendListFrame.FriendListItem>(this.m_allItems, (FriendListFrame.FriendListItem item) => item.IsHeader && item.SubType == MobileFriendListItem.TypeFlags.Request))
				{
					FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(true, MobileFriendListItem.TypeFlags.Request, null);
					this.m_allItems.Add(friendListItem);
				}
			}
			header.SetText(text);
		}
		else if (header == null)
		{
			this.RemoveItem(true, MobileFriendListItem.TypeFlags.Request, null);
		}
	}

	// Token: 0x06003C49 RID: 15433 RVA: 0x00123B7C File Offset: 0x00121D7C
	private void UpdateCurrentGamesHeader()
	{
		int num = Enumerable.Count<FriendListFrame.FriendListItem>(this.m_allItems, (FriendListFrame.FriendListItem i) => i.ItemMainType == MobileFriendListItem.TypeFlags.CurrentGame);
		if (num > 0)
		{
			string text = GameStrings.Format("GLOBAL_FRIENDLIST_CURRENT_GAMES_HEADER", new object[]
			{
				num
			});
			FriendListItemHeader friendListItemHeader = this.FindOrAddHeader(MobileFriendListItem.TypeFlags.CurrentGame);
			if (!Enumerable.Any<FriendListFrame.FriendListItem>(this.m_allItems, (FriendListFrame.FriendListItem item) => item.IsHeader && item.SubType == MobileFriendListItem.TypeFlags.CurrentGame))
			{
				FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(true, MobileFriendListItem.TypeFlags.CurrentGame, null);
				this.m_allItems.Add(friendListItem);
			}
			friendListItemHeader.SetText(text);
		}
		else
		{
			this.RemoveItem(true, MobileFriendListItem.TypeFlags.CurrentGame, null);
		}
	}

	// Token: 0x06003C4A RID: 15434 RVA: 0x00123C38 File Offset: 0x00121E38
	private void UpdateNearbyPlayersHeader(FriendListItemHeader header = null)
	{
		int num = Enumerable.Count<FriendListFrame.FriendListItem>(this.m_allItems, (FriendListFrame.FriendListItem i) => i.ItemMainType == MobileFriendListItem.TypeFlags.NearbyPlayer);
		if (num > 0)
		{
			string text = GameStrings.Format("GLOBAL_FRIENDLIST_NEARBY_PLAYERS_HEADER", new object[]
			{
				num
			});
			if (header == null)
			{
				header = this.FindOrAddHeader(MobileFriendListItem.TypeFlags.NearbyPlayer);
				if (!Enumerable.Any<FriendListFrame.FriendListItem>(this.m_allItems, (FriendListFrame.FriendListItem item) => item.IsHeader && item.SubType == MobileFriendListItem.TypeFlags.NearbyPlayer))
				{
					FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(true, MobileFriendListItem.TypeFlags.NearbyPlayer, null);
					this.m_allItems.Add(friendListItem);
				}
			}
			header.SetText(text);
		}
		else if (header == null)
		{
			this.RemoveItem(true, MobileFriendListItem.TypeFlags.NearbyPlayer, null);
		}
	}

	// Token: 0x06003C4B RID: 15435 RVA: 0x00123D0C File Offset: 0x00121F0C
	private void UpdateRecruitsHeader()
	{
		int num = Enumerable.Count<FriendListFrame.FriendListItem>(this.m_allItems, (FriendListFrame.FriendListItem i) => i.ItemMainType == MobileFriendListItem.TypeFlags.Recruit);
		if (num > 0)
		{
			string text = GameStrings.Format("GLOBAL_FRIENDLIST_RECRUITS_HEADER", new object[]
			{
				num
			});
			FriendListItemHeader friendListItemHeader = this.FindOrAddHeader(MobileFriendListItem.TypeFlags.Recruit);
			if (!Enumerable.Any<FriendListFrame.FriendListItem>(this.m_allItems, (FriendListFrame.FriendListItem item) => item.IsHeader && item.SubType == MobileFriendListItem.TypeFlags.Recruit))
			{
				FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(true, MobileFriendListItem.TypeFlags.Recruit, null);
				this.m_allItems.Add(friendListItem);
			}
			friendListItemHeader.SetText(text);
		}
		else
		{
			this.RemoveItem(true, MobileFriendListItem.TypeFlags.Recruit, null);
		}
	}

	// Token: 0x06003C4C RID: 15436 RVA: 0x00123DC4 File Offset: 0x00121FC4
	private void UpdateFriendsHeader(FriendListItemHeader header = null)
	{
		IEnumerable<FriendListFrame.FriendListItem> enumerable = Enumerable.Where<FriendListFrame.FriendListItem>(this.m_allItems, (FriendListFrame.FriendListItem i) => i.ItemMainType == MobileFriendListItem.TypeFlags.Friend);
		int num = Enumerable.Count<FriendListFrame.FriendListItem>(enumerable, (FriendListFrame.FriendListItem i) => i.GetFriend().IsOnline());
		int num2 = Enumerable.Count<FriendListFrame.FriendListItem>(enumerable);
		string text;
		if (num == num2)
		{
			text = GameStrings.Format("GLOBAL_FRIENDLIST_FRIENDS_HEADER_ALL_ONLINE", new object[]
			{
				num
			});
		}
		else
		{
			text = GameStrings.Format("GLOBAL_FRIENDLIST_FRIENDS_HEADER", new object[]
			{
				num,
				num2
			});
		}
		if (header == null)
		{
			header = this.FindOrAddHeader(MobileFriendListItem.TypeFlags.Friend);
			if (!Enumerable.Any<FriendListFrame.FriendListItem>(this.m_allItems, (FriendListFrame.FriendListItem item) => item.IsHeader && item.SubType == MobileFriendListItem.TypeFlags.Friend))
			{
				FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(true, MobileFriendListItem.TypeFlags.Friend, null);
				this.m_allItems.Add(friendListItem);
			}
		}
		header.SetText(text);
	}

	// Token: 0x06003C4D RID: 15437 RVA: 0x00123ED4 File Offset: 0x001220D4
	private void UpdateHeaderBackground(FriendListItemHeader itemHeader)
	{
		if (itemHeader == null)
		{
			return;
		}
		MobileFriendListItem component = itemHeader.GetComponent<MobileFriendListItem>();
		if (component == null)
		{
			return;
		}
		if ((component.Type & MobileFriendListItem.TypeFlags.Request) == (MobileFriendListItem.TypeFlags)0 && (component.Type & MobileFriendListItem.TypeFlags.CurrentGame) == (MobileFriendListItem.TypeFlags)0)
		{
			return;
		}
		TiledBackground tiledBackground;
		if (itemHeader.Background == null)
		{
			GameObject gameObject = new GameObject("ItemsBackground");
			gameObject.transform.parent = component.transform;
			TransformUtil.Identity(gameObject);
			gameObject.layer = 24;
			FriendListFrame.HeaderBackgroundInfo headerBackgroundInfo = ((component.Type & MobileFriendListItem.TypeFlags.Request) == (MobileFriendListItem.TypeFlags)0) ? this.listInfo.currentGameBackgroundInfo : this.listInfo.requestBackgroundInfo;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = headerBackgroundInfo.mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = headerBackgroundInfo.material;
			tiledBackground = gameObject.AddComponent<TiledBackground>();
			itemHeader.Background = gameObject;
		}
		else
		{
			tiledBackground = itemHeader.Background.GetComponent<TiledBackground>();
		}
		tiledBackground.transform.parent = null;
		MobileFriendListItem.TypeFlags type = component.Type ^ MobileFriendListItem.TypeFlags.Header;
		Bounds bounds = Enumerable.Aggregate<ITouchListItem, Bounds>(this.items, new Bounds(component.transform.position, Vector3.zero), delegate(Bounds aggregate, ITouchListItem listItem)
		{
			MobileFriendListItem mobileFriendListItem = listItem as MobileFriendListItem;
			if ((mobileFriendListItem.Type & type) != (MobileFriendListItem.TypeFlags)0)
			{
				aggregate.Encapsulate(mobileFriendListItem.ComputeWorldBounds());
			}
			return aggregate;
		});
		tiledBackground.transform.parent = component.transform;
		bounds.center = component.transform.InverseTransformPoint(bounds.center);
		tiledBackground.SetBounds(bounds);
		TransformUtil.SetPosZ(tiledBackground.transform, 2f);
		tiledBackground.gameObject.SetActive(itemHeader.IsShowingContents);
	}

	// Token: 0x06003C4E RID: 15438 RVA: 0x0012407C File Offset: 0x0012227C
	private FriendListItemHeader FindHeader(MobileFriendListItem.TypeFlags type)
	{
		type |= MobileFriendListItem.TypeFlags.Header;
		FriendListItemHeader result;
		this.m_headers.TryGetValue(type, ref result);
		return result;
	}

	// Token: 0x06003C4F RID: 15439 RVA: 0x001240A0 File Offset: 0x001222A0
	private FriendListItemHeader FindOrAddHeader(MobileFriendListItem.TypeFlags type)
	{
		type |= MobileFriendListItem.TypeFlags.Header;
		FriendListItemHeader friendListItemHeader = this.FindHeader(type);
		if (friendListItemHeader == null)
		{
			FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(true, type, null);
			friendListItemHeader = Object.Instantiate<FriendListItemHeader>(this.prefabs.headerItem);
			this.m_headers[type] = friendListItemHeader;
			Option option = Option.FRIENDS_LIST_FRIEND_SECTION_HIDE;
			MobileFriendListItem.TypeFlags subType = friendListItem.SubType;
			if (subType != MobileFriendListItem.TypeFlags.Recruit)
			{
				if (subType != MobileFriendListItem.TypeFlags.Friend)
				{
					if (subType != MobileFriendListItem.TypeFlags.CurrentGame)
					{
						if (subType != MobileFriendListItem.TypeFlags.NearbyPlayer)
						{
							if (subType == MobileFriendListItem.TypeFlags.Request)
							{
								option = Option.FRIENDS_LIST_REQUEST_SECTION_HIDE;
							}
						}
						else
						{
							option = Option.FRIENDS_LIST_NEARBYPLAYER_SECTION_HIDE;
						}
					}
					else
					{
						option = Option.FRIENDS_LIST_CURRENTGAME_SECTION_HIDE;
					}
				}
				else
				{
					option = Option.FRIENDS_LIST_FRIEND_SECTION_HIDE;
				}
			}
			else
			{
				option = Option.FRIENDS_LIST_RECRUIT_SECTION_HIDE;
			}
			friendListItemHeader.SubType = friendListItem.SubType;
			friendListItemHeader.Option = option;
			bool showHeaderSection = this.GetShowHeaderSection(option);
			friendListItemHeader.SetInitialShowContents(showHeaderSection);
			friendListItemHeader.ClearToggleListeners();
			friendListItemHeader.AddToggleListener(new FriendListItemHeader.ToggleContentsFunc(this.OnHeaderSectionToggle), friendListItemHeader);
			UberText[] objs = UberText.EnableAllTextInObject(friendListItemHeader.gameObject, false);
			this.FinishCreateVisualItem<FriendListItemHeader>(friendListItemHeader, type, null, null);
			UberText.EnableAllTextObjects(objs, true);
		}
		return friendListItemHeader;
	}

	// Token: 0x06003C50 RID: 15440 RVA: 0x001241B4 File Offset: 0x001223B4
	private void OnHeaderSectionToggle(bool show, object userdata)
	{
		FriendListItemHeader header = (FriendListItemHeader)userdata;
		this.SetShowHeaderSection(header.Option, show);
		int startingLongListIndex = this.m_allItems.FindIndex((FriendListFrame.FriendListItem item) => item.IsHeader && item.SubType == header.SubType);
		this.items.RefreshList(startingLongListIndex, true);
		this.UpdateHeaderBackground(header);
	}

	// Token: 0x06003C51 RID: 15441 RVA: 0x00124218 File Offset: 0x00122418
	private T FindFirstItem<T>(Predicate<T> predicate) where T : MonoBehaviour
	{
		ITouchListItem touchListItem = Enumerable.FirstOrDefault<ITouchListItem>(this.items, delegate(ITouchListItem listItem)
		{
			T component = listItem.GetComponent<T>();
			return component != null && predicate.Invoke(component);
		});
		return (touchListItem == null) ? ((T)((object)null)) : touchListItem.GetComponent<T>();
	}

	// Token: 0x06003C52 RID: 15442 RVA: 0x00124264 File Offset: 0x00122464
	private IEnumerable<T> GetItems<T>() where T : MonoBehaviour
	{
		return Enumerable.Select(Enumerable.Where(Enumerable.Select(this.items, (ITouchListItem i) => new
		{
			i = i,
			c = i.GetComponent<T>()
		}), <>__TranspIdent0 => <>__TranspIdent0.c != null), <>__TranspIdent0 => <>__TranspIdent0.c);
	}

	// Token: 0x06003C53 RID: 15443 RVA: 0x001242AC File Offset: 0x001224AC
	private MobileFriendListItem FinishCreateVisualItem<T>(T obj, MobileFriendListItem.TypeFlags type, ITouchListItem parent, GameObject showObj) where T : MonoBehaviour
	{
		MobileFriendListItem mobileFriendListItem = obj.gameObject.GetComponent<MobileFriendListItem>();
		if (mobileFriendListItem == null)
		{
			mobileFriendListItem = obj.gameObject.AddComponent<MobileFriendListItem>();
			BoxCollider component = mobileFriendListItem.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.size = new Vector3(component.size.x, component.size.y + this.items.elementSpacing, component.size.z);
			}
		}
		mobileFriendListItem.Type = type;
		mobileFriendListItem.SetShowObject(showObj);
		mobileFriendListItem.SetParent(parent);
		if (mobileFriendListItem.Selectable)
		{
			BnetPlayer selectedFriend = FriendMgr.Get().GetSelectedFriend();
			if (selectedFriend != null)
			{
				BnetPlayer bnetPlayer = null;
				if (obj is FriendListFriendFrame)
				{
					bnetPlayer = ((FriendListFriendFrame)((object)obj)).GetFriend();
				}
				else if (obj is FriendListNearbyPlayerFrame)
				{
					bnetPlayer = ((FriendListNearbyPlayerFrame)((object)obj)).GetNearbyPlayer();
				}
				if (bnetPlayer != null && selectedFriend == bnetPlayer)
				{
					mobileFriendListItem.Selected();
				}
			}
		}
		return mobileFriendListItem;
	}

	// Token: 0x06003C54 RID: 15444 RVA: 0x001243D4 File Offset: 0x001225D4
	private bool RemoveItem(bool isHeader, MobileFriendListItem.TypeFlags type, object itemToRemove)
	{
		int num = this.m_allItems.FindIndex(delegate(FriendListFrame.FriendListItem item)
		{
			if (item.IsHeader != isHeader || item.SubType != type)
			{
				return false;
			}
			if (itemToRemove == null)
			{
				return true;
			}
			MobileFriendListItem.TypeFlags type2 = type;
			if (type2 == MobileFriendListItem.TypeFlags.Recruit)
			{
				return item.GetRecruit() == (Network.RecruitInfo)itemToRemove;
			}
			if (type2 == MobileFriendListItem.TypeFlags.Friend)
			{
				return item.GetFriend() == (BnetPlayer)itemToRemove;
			}
			if (type2 == MobileFriendListItem.TypeFlags.CurrentGame)
			{
				return item.GetCurrentGame() == (BnetPlayer)itemToRemove;
			}
			if (type2 != MobileFriendListItem.TypeFlags.NearbyPlayer)
			{
				return type2 == MobileFriendListItem.TypeFlags.Request && item.GetInvite() == (BnetInvitation)itemToRemove;
			}
			return item.GetNearbyPlayer() == (BnetPlayer)itemToRemove;
		});
		if (num < 0)
		{
			return false;
		}
		this.m_allItems.RemoveAt(num);
		return true;
	}

	// Token: 0x06003C55 RID: 15445 RVA: 0x0012442A File Offset: 0x0012262A
	private void SuspendItemsLayout()
	{
		this.items.SuspendLayout();
	}

	// Token: 0x06003C56 RID: 15446 RVA: 0x00124437 File Offset: 0x00122637
	private void ResumeItemsLayout()
	{
		this.items.ResumeLayout(false);
		this.SortAndRefreshTouchList();
	}

	// Token: 0x06003C57 RID: 15447 RVA: 0x0012444B File Offset: 0x0012264B
	private void ToggleEditFriendsMode()
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		this.m_editFriendsMode = !this.m_editFriendsMode;
		this.UpdateFriendItems();
	}

	// Token: 0x06003C58 RID: 15448 RVA: 0x00124474 File Offset: 0x00122674
	private void SortAndRefreshTouchList()
	{
		if (this.items.IsLayoutSuspended)
		{
			return;
		}
		this.m_allItems.Sort(new Comparison<FriendListFrame.FriendListItem>(this.ItemsSortCompare));
		if (this.m_longListBehavior == null)
		{
			this.m_longListBehavior = new FriendListFrame.VirtualizedFriendsListBehavior(this);
			this.items.LongListBehavior = this.m_longListBehavior;
		}
		else
		{
			this.items.RefreshList(0, true);
		}
	}

	// Token: 0x06003C59 RID: 15449 RVA: 0x001244E4 File Offset: 0x001226E4
	private int ItemsSortCompare(FriendListFrame.FriendListItem item1, FriendListFrame.FriendListItem item2)
	{
		int num = item2.ItemFlags.CompareTo(item1.ItemFlags);
		if (num != 0)
		{
			return num;
		}
		MobileFriendListItem.TypeFlags itemFlags = item1.ItemFlags;
		if (itemFlags == MobileFriendListItem.TypeFlags.Friend)
		{
			return FriendUtils.FriendSortCompare(item1.GetFriend(), item2.GetFriend());
		}
		if (itemFlags == MobileFriendListItem.TypeFlags.NearbyPlayer)
		{
			return FriendUtils.FriendSortCompare(item1.GetNearbyPlayer(), item2.GetNearbyPlayer());
		}
		if (itemFlags == MobileFriendListItem.TypeFlags.Recruit)
		{
			return item1.GetRecruit().ID.CompareTo(item2.GetRecruit().ID);
		}
		if (itemFlags == MobileFriendListItem.TypeFlags.CurrentGame)
		{
			return FriendUtils.FriendSortCompare(item1.GetCurrentGame(), item2.GetCurrentGame());
		}
		if (itemFlags != MobileFriendListItem.TypeFlags.Request)
		{
			return 0;
		}
		BnetInvitation invite = item1.GetInvite();
		BnetInvitation invite2 = item2.GetInvite();
		int num2 = string.Compare(invite.GetInviterName(), invite2.GetInviterName(), true);
		if (num2 != 0)
		{
			return num2;
		}
		long lo = (long)invite.GetInviterId().GetLo();
		long lo2 = (long)invite2.GetInviterId().GetLo();
		return (int)(lo - lo2);
	}

	// Token: 0x06003C5A RID: 15450 RVA: 0x001245F8 File Offset: 0x001227F8
	private void RegisterFriendEvents()
	{
		BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
		BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		FriendChallengeMgr.Get().AddChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
		BnetNearbyPlayerMgr.Get().AddChangeListener(new BnetNearbyPlayerMgr.ChangeCallback(this.OnNearbyPlayersChanged));
		RecruitListMgr.Get().AddRecruitsChangedListener(new RecruitListMgr.RecruitsChangedCallback(this.OnRecruitsChanged));
		RecruitListMgr.Get().AddRecruitAcceptedListener(new RecruitListMgr.RecruitAcceptedCallback(this.OnRecruitAccepted));
		FriendMgr.Get().AddRecentOpponentListener(new FriendMgr.RecentOpponentCallback(this.OnRecentOpponent));
		SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
		SpectatorManager.Get().OnInviteReceived += this.SpectatorManager_OnInviteReceivedOrSent;
		SpectatorManager.Get().OnInviteSent += this.SpectatorManager_OnInviteReceivedOrSent;
	}

	// Token: 0x06003C5B RID: 15451 RVA: 0x001246E8 File Offset: 0x001228E8
	private void UnregisterFriendEvents()
	{
		BnetFriendMgr.Get().RemoveChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
		BnetNearbyPlayerMgr.Get().RemoveChangeListener(new BnetNearbyPlayerMgr.ChangeCallback(this.OnNearbyPlayersChanged));
		RecruitListMgr.Get().RemoveRecruitsChangedListener(new RecruitListMgr.RecruitsChangedCallback(this.OnRecruitsChanged));
		FriendMgr.Get().RemoveRecentOpponentListener(new FriendMgr.RecentOpponentCallback(this.OnRecentOpponent));
		if (SceneMgr.Get() != null)
		{
			SceneMgr.Get().UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
		}
		SpectatorManager.Get().OnInviteReceived -= this.SpectatorManager_OnInviteReceivedOrSent;
		SpectatorManager.Get().OnInviteSent -= this.SpectatorManager_OnInviteReceivedOrSent;
	}

	// Token: 0x06003C5C RID: 15452 RVA: 0x001247D4 File Offset: 0x001229D4
	private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
	{
		this.SuspendItemsLayout();
		this.UpdateRequests(changelist.GetAddedReceivedInvites(), changelist.GetRemovedReceivedInvites());
		this.UpdateAllFriends(changelist.GetAddedFriends(), changelist.GetRemovedFriends());
		this.UpdateAllHeaders();
		this.ResumeItemsLayout();
		this.UpdateAllHeaderBackgrounds();
		this.UpdateSelectedItem();
	}

	// Token: 0x06003C5D RID: 15453 RVA: 0x00124824 File Offset: 0x00122A24
	private void OnNearbyPlayersChanged(BnetNearbyPlayerChangelist changelist, object userData)
	{
		this.m_nearbyPlayersNeedUpdate = true;
		if (changelist.GetAddedStrangers() != null)
		{
			foreach (BnetPlayer p in changelist.GetAddedStrangers())
			{
				this.m_nearbyPlayerUpdates.Add(new FriendListFrame.NearbyPlayerUpdate(FriendListFrame.NearbyPlayerUpdate.ChangeType.Added, p));
			}
		}
		if (changelist.GetRemovedStrangers() != null)
		{
			foreach (BnetPlayer p2 in changelist.GetRemovedStrangers())
			{
				this.m_nearbyPlayerUpdates.Add(new FriendListFrame.NearbyPlayerUpdate(FriendListFrame.NearbyPlayerUpdate.ChangeType.Removed, p2));
			}
		}
		if (changelist.GetAddedFriends() != null)
		{
			foreach (BnetPlayer p3 in changelist.GetAddedFriends())
			{
				this.m_nearbyPlayerUpdates.Add(new FriendListFrame.NearbyPlayerUpdate(FriendListFrame.NearbyPlayerUpdate.ChangeType.Added, p3));
			}
		}
		if (changelist.GetRemovedFriends() != null)
		{
			foreach (BnetPlayer p4 in changelist.GetRemovedFriends())
			{
				this.m_nearbyPlayerUpdates.Add(new FriendListFrame.NearbyPlayerUpdate(FriendListFrame.NearbyPlayerUpdate.ChangeType.Removed, p4));
			}
		}
		if (base.gameObject.activeInHierarchy && Time.realtimeSinceStartup >= this.m_lastNearbyPlayersUpdate + 10f)
		{
			this.HandleNearbyPlayersChanged();
		}
	}

	// Token: 0x06003C5E RID: 15454 RVA: 0x001249EC File Offset: 0x00122BEC
	private void OnRecruitsChanged()
	{
		this.UpdateAllRecruits();
		this.UpdateRecruitsHeader();
	}

	// Token: 0x06003C5F RID: 15455 RVA: 0x001249FC File Offset: 0x00122BFC
	private void OnRecruitAccepted(Network.RecruitInfo recruit)
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		string text = recruit.Nickname;
		FriendListFriendFrame friendListFriendFrame = this.FindFriendFrame(recruit.RecruitID);
		if (friendListFriendFrame != null)
		{
			text = friendListFriendFrame.GetFriend().GetBestName();
		}
		popupInfo.m_headerText = GameStrings.Format("GLOBAL_FRIENDLIST_RECRUIT_ACCEPTED_ALERT_HEADER", new object[0]);
		popupInfo.m_text = GameStrings.Format("GLOBAL_FRIENDLIST_RECRUIT_ACCEPTED_ALERT_MESSAGE", new object[]
		{
			text
		});
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06003C60 RID: 15456 RVA: 0x00124A80 File Offset: 0x00122C80
	private void HandleNearbyPlayersChanged()
	{
		if (!this.m_nearbyPlayersNeedUpdate)
		{
			return;
		}
		this.UpdateNearbyPlayerItems();
		if (this.m_nearbyPlayerUpdates.Count > 0)
		{
			this.SuspendItemsLayout();
			foreach (FriendListFrame.NearbyPlayerUpdate nearbyPlayerUpdate in this.m_nearbyPlayerUpdates)
			{
				if (nearbyPlayerUpdate.Change == FriendListFrame.NearbyPlayerUpdate.ChangeType.Added)
				{
					FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.NearbyPlayer, nearbyPlayerUpdate.Player);
					this.m_allItems.Add(friendListItem);
				}
				else
				{
					this.RemoveItem(false, MobileFriendListItem.TypeFlags.NearbyPlayer, nearbyPlayerUpdate.Player);
				}
			}
			this.m_nearbyPlayerUpdates.Clear();
			this.UpdateAllHeaders();
			this.ResumeItemsLayout();
			this.UpdateAllHeaderBackgrounds();
			this.UpdateSelectedItem();
		}
		this.m_nearbyPlayersNeedUpdate = false;
		this.m_lastNearbyPlayersUpdate = Time.realtimeSinceStartup;
	}

	// Token: 0x06003C61 RID: 15457 RVA: 0x00124B70 File Offset: 0x00122D70
	private void DoPlayersChanged(BnetPlayerChangelist changelist)
	{
		this.SuspendItemsLayout();
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		bool flag = false;
		bool flag2 = false;
		foreach (BnetPlayerChange bnetPlayerChange in changelist.GetChanges())
		{
			BnetPlayer oldPlayer = bnetPlayerChange.GetOldPlayer();
			BnetPlayer newPlayer = bnetPlayerChange.GetNewPlayer();
			if (newPlayer == FriendMgr.Get().GetRecentOpponent())
			{
				this.UpdateRecentOpponent();
			}
			if (newPlayer == myPlayer)
			{
				this.UpdateMyself();
				BnetGameAccount hearthstoneGameAccount = newPlayer.GetHearthstoneGameAccount();
				if (oldPlayer == null || oldPlayer.GetHearthstoneGameAccount() == null)
				{
					flag = hearthstoneGameAccount.CanBeInvitedToGame();
				}
				else
				{
					BnetGameAccount hearthstoneGameAccount2 = oldPlayer.GetHearthstoneGameAccount();
					flag = (hearthstoneGameAccount2.CanBeInvitedToGame() != hearthstoneGameAccount.CanBeInvitedToGame());
				}
			}
			else
			{
				if (oldPlayer == null || oldPlayer.GetBestName() != newPlayer.GetBestName())
				{
					flag2 = true;
				}
				long persistentGameId = newPlayer.GetPersistentGameId();
				FriendListFriendFrame friendListFriendFrame = this.FindFriendFrame(newPlayer);
				if (friendListFriendFrame != null)
				{
					if (persistentGameId != 0L)
					{
						this.RemoveItem(false, MobileFriendListItem.TypeFlags.Friend, newPlayer);
						FriendListFrame.FriendListItem friendListItem = new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.CurrentGame, newPlayer);
						this.m_allItems.Add(friendListItem);
						this.UpdateCurrentGamesHeader();
						this.UpdateFriendsHeader(null);
					}
					else
					{
						friendListFriendFrame.UpdateFriend();
					}
				}
				else
				{
					FriendListCurrentGameFrame friendListCurrentGameFrame = this.FindCurrentGameFrame(newPlayer);
					if (friendListCurrentGameFrame != null)
					{
						if (persistentGameId == 0L)
						{
							this.RemoveItem(false, MobileFriendListItem.TypeFlags.CurrentGame, null);
							FriendListFrame.FriendListItem friendListItem2 = new FriendListFrame.FriendListItem(false, MobileFriendListItem.TypeFlags.Friend, newPlayer);
							this.m_allItems.Add(friendListItem2);
							this.UpdateCurrentGamesHeader();
							this.UpdateFriendsHeader(null);
						}
						else
						{
							friendListCurrentGameFrame.UpdateFriend();
						}
					}
				}
			}
		}
		if (flag)
		{
			this.UpdateItems();
		}
		else if (flag2)
		{
			this.UpdateFriendItems();
		}
		this.UpdateAllHeaders();
		this.UpdateAllHeaderBackgrounds();
		this.ResumeItemsLayout();
	}

	// Token: 0x06003C62 RID: 15458 RVA: 0x00124D88 File Offset: 0x00122F88
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		if (base.gameObject.activeInHierarchy)
		{
			this.DoPlayersChanged(changelist);
		}
		else
		{
			List<BnetPlayerChange> changes = changelist.GetChanges();
			this.m_playersChangeList.GetChanges().AddRange(changes);
		}
	}

	// Token: 0x06003C63 RID: 15459 RVA: 0x00124DC9 File Offset: 0x00122FC9
	private void OnRecentOpponent(BnetPlayer recentOpponent, object userData)
	{
		this.UpdateRecentOpponent();
	}

	// Token: 0x06003C64 RID: 15460 RVA: 0x00124DD4 File Offset: 0x00122FD4
	private void OnScenePreUnload(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		if (mode != SceneMgr.Mode.FRIENDLY && mode != SceneMgr.Mode.FATAL_ERROR)
		{
			return;
		}
		if (ChatMgr.Get() != null)
		{
			ChatMgr.Get().HideFriendsList();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06003C65 RID: 15461 RVA: 0x00124E2C File Offset: 0x0012302C
	private void SpectatorManager_OnInviteReceivedOrSent(OnlineEventType evt, BnetPlayer inviter)
	{
		FriendListFriendFrame friendListFriendFrame = this.FindFriendFrame(inviter);
		if (friendListFriendFrame != null)
		{
			friendListFriendFrame.UpdateFriend();
		}
	}

	// Token: 0x06003C66 RID: 15462 RVA: 0x00124E54 File Offset: 0x00123054
	private void OnFriendChallengeChanged(FriendChallengeEvent challengeEvent, BnetPlayer player, object userData)
	{
		if (player == BnetPresenceMgr.Get().GetMyPlayer())
		{
			this.UpdateFriendItems();
		}
		else
		{
			FriendListBaseFriendFrame friendListBaseFriendFrame = this.FindBaseFriendFrame(player);
			if (friendListBaseFriendFrame != null)
			{
				friendListBaseFriendFrame.UpdateFriend();
			}
		}
	}

	// Token: 0x06003C67 RID: 15463 RVA: 0x00124E96 File Offset: 0x00123096
	private bool HandleKeyboardInput()
	{
		return FatalErrorMgr.Get().HasError() && false;
	}

	// Token: 0x06003C68 RID: 15464 RVA: 0x00124EAC File Offset: 0x001230AC
	private void OnAddFriendButtonReleased(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("Small_Click");
		if (this.m_addFriendFrame != null)
		{
			this.CloseAddFriendFrame();
			return;
		}
		if (this.AddFriendFrameOpened != null)
		{
			this.AddFriendFrameOpened.Invoke();
		}
		BnetPlayer selectedFriend = FriendMgr.Get().GetSelectedFriend();
		this.ShowAddFriendFrame(selectedFriend);
	}

	// Token: 0x06003C69 RID: 15465 RVA: 0x00124F08 File Offset: 0x00123108
	private void OnEditFriendsButtonReleased(UIEvent e)
	{
		this.ToggleEditFriendsMode();
	}

	// Token: 0x06003C6A RID: 15466 RVA: 0x00124F10 File Offset: 0x00123110
	private void OnRemoveFriendButtonReleased(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("Small_Click");
		if (this.items.SelectedItem == null)
		{
			return;
		}
		BnetPlayer selectedFriend = FriendMgr.Get().GetSelectedFriend();
		this.ShowRemoveFriendPopup(selectedFriend);
	}

	// Token: 0x06003C6B RID: 15467 RVA: 0x00124F50 File Offset: 0x00123150
	private void OnRecruitAFriendButtonReleased(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("Small_Click");
		if (this.m_recruitAFriendFrame != null)
		{
			this.RecruitAFriend_OnClosed();
			return;
		}
		if (this.RecruitAFriendFrameOpened != null)
		{
			this.RecruitAFriendFrameOpened.Invoke();
		}
		this.ShowRecruitAFriendFrame();
	}

	// Token: 0x06003C6C RID: 15468 RVA: 0x00124FA0 File Offset: 0x001231A0
	private bool OnRemoveFriendDialogShown(DialogBase dialog, object userData)
	{
		BnetPlayer player = (BnetPlayer)userData;
		if (!BnetFriendMgr.Get().IsFriend(player))
		{
			return false;
		}
		this.m_removeFriendPopup = (AlertPopup)dialog;
		return true;
	}

	// Token: 0x06003C6D RID: 15469 RVA: 0x00124FD4 File Offset: 0x001231D4
	private void OnRemoveFriendPopupResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CONFIRM && this.m_friendToRemove != null)
		{
			BnetFriendMgr.Get().RemoveFriend(this.m_friendToRemove);
		}
		this.m_friendToRemove = null;
		this.m_removeFriendPopup = null;
		if (this.RemoveFriendPopupClosed != null)
		{
			this.RemoveFriendPopupClosed.Invoke();
		}
	}

	// Token: 0x06003C6E RID: 15470 RVA: 0x00125028 File Offset: 0x00123228
	private void OnRecentOpponentButtonReleased(UIEvent e)
	{
		if (!string.IsNullOrEmpty(this.recentOpponent.nameText.Text))
		{
			BnetPlayer player = FriendMgr.Get().GetRecentOpponent();
			this.ShowAddFriendFrame(player);
		}
	}

	// Token: 0x06003C6F RID: 15471 RVA: 0x00125064 File Offset: 0x00123264
	private void OnBaseFriendFrameReleased(UIEvent e)
	{
		if (this.IsInEditMode)
		{
			return;
		}
		FriendListUIElement friendListUIElement = (FriendListUIElement)e.GetElement();
		FriendListBaseFriendFrame component = friendListUIElement.GetComponent<FriendListBaseFriendFrame>();
		BnetPlayer friend = component.GetFriend();
		FriendMgr.Get().SetSelectedFriend(friend);
		if (FriendListFrame.ALLOW_ITEM_SELECTION)
		{
			this.SelectedPlayer = friend;
		}
		ChatMgr.Get().OnFriendListFriendSelected(friend);
	}

	// Token: 0x06003C70 RID: 15472 RVA: 0x001250C4 File Offset: 0x001232C4
	private void OnNearbyPlayerFrameReleased(UIEvent e)
	{
		FriendListUIElement friendListUIElement = (FriendListUIElement)e.GetElement();
		FriendListNearbyPlayerFrame component = friendListUIElement.GetComponent<FriendListNearbyPlayerFrame>();
		BnetPlayer nearbyPlayer = component.GetNearbyPlayer();
		FriendMgr.Get().SetSelectedFriend(nearbyPlayer);
		if (FriendListFrame.ALLOW_ITEM_SELECTION)
		{
			this.SelectedPlayer = nearbyPlayer;
		}
	}

	// Token: 0x06003C71 RID: 15473 RVA: 0x0012510C File Offset: 0x0012330C
	private void UpdateSelectedItem()
	{
		BnetPlayer selectedFriend = FriendMgr.Get().GetSelectedFriend();
		FriendListBaseFriendFrame friendListBaseFriendFrame = this.FindBaseFriendFrame(selectedFriend);
		if (friendListBaseFriendFrame == null)
		{
			if (this.items.SelectedIndex == -1)
			{
				return;
			}
			this.items.SelectedIndex = -1;
			if (this.m_removeFriendPopup != null)
			{
				this.m_removeFriendPopup.Hide();
				this.m_removeFriendPopup = null;
				if (this.RemoveFriendPopupClosed != null)
				{
					this.RemoveFriendPopupClosed.Invoke();
				}
			}
		}
		else
		{
			this.items.SelectedIndex = this.items.IndexOf(friendListBaseFriendFrame.GetComponent<MobileFriendListItem>());
		}
	}

	// Token: 0x06003C72 RID: 15474 RVA: 0x001251B0 File Offset: 0x001233B0
	private void InitButtons()
	{
		this.addFriendButton.SetText(GameStrings.Get("GLOBAL_FRIENDLIST_ADD_FRIEND_BUTTON"));
		this.addFriendButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnAddFriendButtonReleased));
		this.removeFriendButton.SetText(GameStrings.Get("GLOBAL_FRIENDLIST_REMOVE_FRIEND_BUTTON"));
		if (UniversalInputManager.UsePhoneUI)
		{
			this.removeFriendButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnEditFriendsButtonReleased));
		}
		else
		{
			this.removeFriendButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRemoveFriendButtonReleased));
		}
	}

	// Token: 0x06003C73 RID: 15475 RVA: 0x00125246 File Offset: 0x00123446
	private bool GetShowHeaderSection(Option setoption)
	{
		return !(bool)Options.Get().GetOption(setoption, false);
	}

	// Token: 0x06003C74 RID: 15476 RVA: 0x00125264 File Offset: 0x00123464
	private void SetShowHeaderSection(Option sectionoption, bool show)
	{
		bool showHeaderSection = this.GetShowHeaderSection(sectionoption);
		if (showHeaderSection != show)
		{
			Options.Get().SetOption(sectionoption, !show);
		}
	}

	// Token: 0x06003C75 RID: 15477 RVA: 0x00125294 File Offset: 0x00123494
	private Transform GetBottomRightBone()
	{
		return (!(this.scrollbar != null) || !this.scrollbar.gameObject.activeSelf) ? this.listInfo.bottomRight : this.listInfo.bottomRightWithScrollbar;
	}

	// Token: 0x04002669 RID: 9833
	private const float NEARBY_PLAYERS_UPDATE_TIME = 10f;

	// Token: 0x0400266A RID: 9834
	public FriendListFrame.Me me;

	// Token: 0x0400266B RID: 9835
	public FriendListFrame.RecentOpponent recentOpponent;

	// Token: 0x0400266C RID: 9836
	public FriendListFrame.Prefabs prefabs;

	// Token: 0x0400266D RID: 9837
	public FriendListFrame.ListInfo listInfo;

	// Token: 0x0400266E RID: 9838
	public TouchList items;

	// Token: 0x0400266F RID: 9839
	public FriendListButton addFriendButton;

	// Token: 0x04002670 RID: 9840
	public FriendListButton removeFriendButton;

	// Token: 0x04002671 RID: 9841
	public TouchListScrollbar scrollbar;

	// Token: 0x04002672 RID: 9842
	public NineSliceElement window;

	// Token: 0x04002673 RID: 9843
	public GameObject portraitBackground;

	// Token: 0x04002674 RID: 9844
	public Material unrankedBackground;

	// Token: 0x04002675 RID: 9845
	public Material rankedBackground;

	// Token: 0x04002676 RID: 9846
	public GameObject innerShadow;

	// Token: 0x04002677 RID: 9847
	public GameObject outerShadow;

	// Token: 0x04002678 RID: 9848
	public FriendListButton m_RecruitAFriendButton;

	// Token: 0x04002679 RID: 9849
	private PlayerPortrait myPortrait;

	// Token: 0x0400267A RID: 9850
	private AddFriendFrame m_addFriendFrame;

	// Token: 0x0400267B RID: 9851
	private RecruitAFriendFrame m_recruitAFriendFrame;

	// Token: 0x0400267C RID: 9852
	private AlertPopup m_removeFriendPopup;

	// Token: 0x0400267D RID: 9853
	private Camera m_itemsCamera;

	// Token: 0x0400267E RID: 9854
	private bool m_editFriendsMode;

	// Token: 0x0400267F RID: 9855
	private BnetPlayer m_friendToRemove;

	// Token: 0x04002680 RID: 9856
	private List<FriendListFrame.NearbyPlayerUpdate> m_nearbyPlayerUpdates = new List<FriendListFrame.NearbyPlayerUpdate>();

	// Token: 0x04002681 RID: 9857
	private BnetPlayerChangelist m_playersChangeList = new BnetPlayerChangelist();

	// Token: 0x04002682 RID: 9858
	private float m_lastNearbyPlayersUpdate;

	// Token: 0x04002683 RID: 9859
	private bool m_nearbyPlayersNeedUpdate;

	// Token: 0x04002684 RID: 9860
	private static readonly PlatformDependentValue<bool> ALLOW_ITEM_SELECTION = new PlatformDependentValue<bool>(PlatformCategory.Screen)
	{
		PC = true,
		Tablet = true,
		Phone = false
	};

	// Token: 0x04002685 RID: 9861
	private List<FriendListFrame.FriendListItem> m_allItems = new List<FriendListFrame.FriendListItem>();

	// Token: 0x04002686 RID: 9862
	private FriendListFrame.VirtualizedFriendsListBehavior m_longListBehavior;

	// Token: 0x04002687 RID: 9863
	private Dictionary<MobileFriendListItem.TypeFlags, FriendListItemHeader> m_headers = new Dictionary<MobileFriendListItem.TypeFlags, FriendListItemHeader>();

	// Token: 0x0200055F RID: 1375
	private class NearbyPlayerUpdate
	{
		// Token: 0x06003F33 RID: 16179 RVA: 0x00133898 File Offset: 0x00131A98
		public NearbyPlayerUpdate(FriendListFrame.NearbyPlayerUpdate.ChangeType c, BnetPlayer p)
		{
			this.Change = c;
			this.Player = p;
		}

		// Token: 0x04002879 RID: 10361
		public FriendListFrame.NearbyPlayerUpdate.ChangeType Change;

		// Token: 0x0400287A RID: 10362
		public BnetPlayer Player;

		// Token: 0x02000560 RID: 1376
		public enum ChangeType
		{
			// Token: 0x0400287C RID: 10364
			Added,
			// Token: 0x0400287D RID: 10365
			Removed
		}
	}

	// Token: 0x02000561 RID: 1377
	[Serializable]
	public class Me
	{
		// Token: 0x0400287E RID: 10366
		public Spawner portraitRef;

		// Token: 0x0400287F RID: 10367
		public UberText nameText;

		// Token: 0x04002880 RID: 10368
		public UberText numberText;

		// Token: 0x04002881 RID: 10369
		public UberText statusText;

		// Token: 0x04002882 RID: 10370
		public GameObject m_MedalPatch;

		// Token: 0x04002883 RID: 10371
		public TournamentMedal m_Medal;
	}

	// Token: 0x02000562 RID: 1378
	[Serializable]
	public class RecentOpponent
	{
		// Token: 0x04002884 RID: 10372
		public PegUIElement button;

		// Token: 0x04002885 RID: 10373
		public UberText nameText;
	}

	// Token: 0x02000563 RID: 1379
	[Serializable]
	public class Prefabs
	{
		// Token: 0x04002886 RID: 10374
		public FriendListItemHeader headerItem;

		// Token: 0x04002887 RID: 10375
		public FriendListRequestFrame requestItem;

		// Token: 0x04002888 RID: 10376
		public FriendListCurrentGameFrame currentGameItem;

		// Token: 0x04002889 RID: 10377
		public FriendListFriendFrame friendItem;

		// Token: 0x0400288A RID: 10378
		public FriendListNearbyPlayerFrame nearbyPlayerItem;

		// Token: 0x0400288B RID: 10379
		public FriendListRecruitFrame recruitItem;

		// Token: 0x0400288C RID: 10380
		public AddFriendFrame addFriendFrame;

		// Token: 0x0400288D RID: 10381
		public RecruitAFriendFrame recruitAFriendFrame;
	}

	// Token: 0x0200056B RID: 1387
	[Serializable]
	public class ListInfo
	{
		// Token: 0x040028C0 RID: 10432
		public Transform topLeft;

		// Token: 0x040028C1 RID: 10433
		public Transform bottomRight;

		// Token: 0x040028C2 RID: 10434
		public Transform bottomRightWithScrollbar;

		// Token: 0x040028C3 RID: 10435
		public FriendListFrame.HeaderBackgroundInfo requestBackgroundInfo;

		// Token: 0x040028C4 RID: 10436
		public FriendListFrame.HeaderBackgroundInfo currentGameBackgroundInfo;
	}

	// Token: 0x0200056C RID: 1388
	[Serializable]
	public class HeaderBackgroundInfo
	{
		// Token: 0x040028C5 RID: 10437
		public Mesh mesh;

		// Token: 0x040028C6 RID: 10438
		public Material material;
	}

	// Token: 0x0200056D RID: 1389
	public struct FriendListItem
	{
		// Token: 0x06003F94 RID: 16276 RVA: 0x00134B44 File Offset: 0x00132D44
		public FriendListItem(bool isHeader, MobileFriendListItem.TypeFlags itemType, object itemData)
		{
			if (!isHeader && itemData == null)
			{
				Log.Henry.Print("FriendListItem: itemData is null! itemType=" + itemType, new object[0]);
			}
			this.m_item = itemData;
			this.ItemFlags = itemType;
			if (isHeader)
			{
				this.ItemFlags |= MobileFriendListItem.TypeFlags.Header;
			}
			else
			{
				this.ItemFlags &= ~MobileFriendListItem.TypeFlags.Header;
			}
		}

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06003F95 RID: 16277 RVA: 0x00134BB3 File Offset: 0x00132DB3
		// (set) Token: 0x06003F96 RID: 16278 RVA: 0x00134BBB File Offset: 0x00132DBB
		public MobileFriendListItem.TypeFlags ItemFlags { get; private set; }

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06003F97 RID: 16279 RVA: 0x00134BC4 File Offset: 0x00132DC4
		public bool IsHeader
		{
			get
			{
				return (this.ItemFlags & MobileFriendListItem.TypeFlags.Header) != (MobileFriendListItem.TypeFlags)0;
			}
		}

		// Token: 0x06003F98 RID: 16280 RVA: 0x00134BD4 File Offset: 0x00132DD4
		public BnetPlayer GetFriend()
		{
			if ((this.ItemFlags & MobileFriendListItem.TypeFlags.Friend) == (MobileFriendListItem.TypeFlags)0)
			{
				return null;
			}
			return (BnetPlayer)this.m_item;
		}

		// Token: 0x06003F99 RID: 16281 RVA: 0x00134BF1 File Offset: 0x00132DF1
		public BnetPlayer GetCurrentGame()
		{
			if ((this.ItemFlags & MobileFriendListItem.TypeFlags.CurrentGame) == (MobileFriendListItem.TypeFlags)0)
			{
				return null;
			}
			return (BnetPlayer)this.m_item;
		}

		// Token: 0x06003F9A RID: 16282 RVA: 0x00134C0E File Offset: 0x00132E0E
		public BnetPlayer GetNearbyPlayer()
		{
			if ((this.ItemFlags & MobileFriendListItem.TypeFlags.NearbyPlayer) == (MobileFriendListItem.TypeFlags)0)
			{
				return null;
			}
			return (BnetPlayer)this.m_item;
		}

		// Token: 0x06003F9B RID: 16283 RVA: 0x00134C2B File Offset: 0x00132E2B
		public BnetInvitation GetInvite()
		{
			if ((this.ItemFlags & MobileFriendListItem.TypeFlags.Request) == (MobileFriendListItem.TypeFlags)0)
			{
				return null;
			}
			return (BnetInvitation)this.m_item;
		}

		// Token: 0x06003F9C RID: 16284 RVA: 0x00134C4B File Offset: 0x00132E4B
		public Network.RecruitInfo GetRecruit()
		{
			if ((this.ItemFlags & MobileFriendListItem.TypeFlags.Recruit) == (MobileFriendListItem.TypeFlags)0)
			{
				return null;
			}
			return (Network.RecruitInfo)this.m_item;
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06003F9D RID: 16285 RVA: 0x00134C67 File Offset: 0x00132E67
		public MobileFriendListItem.TypeFlags ItemMainType
		{
			get
			{
				if (this.IsHeader)
				{
					return MobileFriendListItem.TypeFlags.Header;
				}
				return this.SubType;
			}
		}

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06003F9E RID: 16286 RVA: 0x00134C7C File Offset: 0x00132E7C
		public MobileFriendListItem.TypeFlags SubType
		{
			get
			{
				return this.ItemFlags & ~MobileFriendListItem.TypeFlags.Header;
			}
		}

		// Token: 0x06003F9F RID: 16287 RVA: 0x00134C94 File Offset: 0x00132E94
		public override string ToString()
		{
			if (this.IsHeader)
			{
				return string.Format("[{0}]Header", this.SubType);
			}
			return string.Format("[{0}]{1}", this.ItemMainType, this.m_item);
		}

		// Token: 0x06003FA0 RID: 16288 RVA: 0x00134CE0 File Offset: 0x00132EE0
		public Type GetFrameType()
		{
			MobileFriendListItem.TypeFlags itemMainType = this.ItemMainType;
			MobileFriendListItem.TypeFlags typeFlags = itemMainType;
			if (typeFlags == MobileFriendListItem.TypeFlags.Header)
			{
				return typeof(FriendListItemHeader);
			}
			if (typeFlags == MobileFriendListItem.TypeFlags.Recruit)
			{
				return typeof(FriendListRecruitFrame);
			}
			if (typeFlags == MobileFriendListItem.TypeFlags.Friend)
			{
				return typeof(FriendListFriendFrame);
			}
			if (typeFlags == MobileFriendListItem.TypeFlags.CurrentGame)
			{
				return typeof(FriendListCurrentGameFrame);
			}
			if (typeFlags == MobileFriendListItem.TypeFlags.NearbyPlayer)
			{
				return typeof(FriendListNearbyPlayerFrame);
			}
			if (typeFlags != MobileFriendListItem.TypeFlags.Request)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Unknown ItemType: ",
					this.ItemFlags,
					" (",
					(int)this.ItemFlags,
					")"
				}));
			}
			return typeof(FriendListRequestFrame);
		}

		// Token: 0x040028C7 RID: 10439
		private object m_item;
	}

	// Token: 0x02000570 RID: 1392
	private class VirtualizedFriendsListBehavior : TouchList.ILongListBehavior
	{
		// Token: 0x06003FB3 RID: 16307 RVA: 0x00134F5C File Offset: 0x0013315C
		public VirtualizedFriendsListBehavior(FriendListFrame friendList)
		{
			this.m_friendList = friendList;
		}

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06003FB4 RID: 16308 RVA: 0x00134F7D File Offset: 0x0013317D
		public List<MobileFriendListItem> FreeList
		{
			get
			{
				return this.m_freelist;
			}
		}

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06003FB5 RID: 16309 RVA: 0x00134F85 File Offset: 0x00133185
		public int AllItemsCount
		{
			get
			{
				return this.m_friendList.m_allItems.Count;
			}
		}

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06003FB6 RID: 16310 RVA: 0x00134F98 File Offset: 0x00133198
		public int MaxVisibleItems
		{
			get
			{
				if (this.m_cachedMaxVisibleItems >= 0)
				{
					return this.m_cachedMaxVisibleItems;
				}
				this.m_cachedMaxVisibleItems = 0;
				Vector2 clipSize = this.m_friendList.items.ClipSize;
				Bounds prefabBounds = FriendListFrame.VirtualizedFriendsListBehavior.GetPrefabBounds(this.m_friendList.prefabs.requestItem.gameObject);
				Bounds prefabBounds2 = FriendListFrame.VirtualizedFriendsListBehavior.GetPrefabBounds(this.m_friendList.prefabs.friendItem.gameObject);
				Bounds prefabBounds3 = FriendListFrame.VirtualizedFriendsListBehavior.GetPrefabBounds(this.m_friendList.prefabs.nearbyPlayerItem.gameObject);
				float num = prefabBounds.max.y - prefabBounds.min.y;
				float num2 = prefabBounds2.max.y - prefabBounds2.min.y;
				float num3 = prefabBounds3.max.y - prefabBounds3.min.y;
				float num4 = Mathf.Min(new float[]
				{
					num,
					num2,
					num3
				});
				if (num4 > 0f)
				{
					int num5 = Mathf.CeilToInt(clipSize.y / num4);
					this.m_cachedMaxVisibleItems = num5 + 3;
				}
				return this.m_cachedMaxVisibleItems;
			}
		}

		// Token: 0x06003FB7 RID: 16311 RVA: 0x001350D4 File Offset: 0x001332D4
		private static Bounds GetPrefabBounds(GameObject prefabGameObject)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(prefabGameObject);
			gameObject.SetActive(true);
			Bounds result = TransformUtil.ComputeSetPointBounds(gameObject);
			Object.DestroyImmediate(gameObject);
			return result;
		}

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06003FB8 RID: 16312 RVA: 0x001350FD File Offset: 0x001332FD
		public int MinBuffer
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06003FB9 RID: 16313 RVA: 0x00135100 File Offset: 0x00133300
		public int MaxAcquiredItems
		{
			get
			{
				return this.MaxVisibleItems + 2 * this.MinBuffer;
			}
		}

		// Token: 0x06003FBA RID: 16314 RVA: 0x00135120 File Offset: 0x00133320
		public bool IsItemShowable(int allItemsIndex)
		{
			if (allItemsIndex < 0 || allItemsIndex >= this.AllItemsCount)
			{
				return false;
			}
			FriendListFrame.FriendListItem friendListItem = this.m_friendList.m_allItems[allItemsIndex];
			if (friendListItem.IsHeader)
			{
				return true;
			}
			FriendListItemHeader friendListItemHeader = this.m_friendList.FindHeader(friendListItem.SubType);
			return friendListItemHeader != null && friendListItemHeader.IsShowingContents;
		}

		// Token: 0x06003FBB RID: 16315 RVA: 0x0013518C File Offset: 0x0013338C
		public Vector3 GetItemSize(int allItemsIndex)
		{
			if (allItemsIndex < 0 || allItemsIndex >= this.AllItemsCount)
			{
				return Vector3.zero;
			}
			FriendListFrame.FriendListItem friendListItem = this.m_friendList.m_allItems[allItemsIndex];
			if (this.m_boundsByType == null)
			{
				this.InitializeBoundsByTypeArray();
			}
			int boundsByTypeIndex = this.GetBoundsByTypeIndex(friendListItem.ItemMainType);
			return this.m_boundsByType[boundsByTypeIndex].size;
		}

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06003FBC RID: 16316 RVA: 0x001351F4 File Offset: 0x001333F4
		private bool HasCollapsedHeaders
		{
			get
			{
				return Enumerable.Any<KeyValuePair<MobileFriendListItem.TypeFlags, FriendListItemHeader>>(this.m_friendList.m_headers, (KeyValuePair<MobileFriendListItem.TypeFlags, FriendListItemHeader> kv) => !kv.Value.IsShowingContents);
			}
		}

		// Token: 0x06003FBD RID: 16317 RVA: 0x00135224 File Offset: 0x00133424
		public void ReleaseAllItems()
		{
			if (this.m_acquiredItems.Count == 0)
			{
				return;
			}
			if (this.m_freelist == null)
			{
				this.m_freelist = new List<MobileFriendListItem>();
			}
			foreach (MobileFriendListItem mobileFriendListItem in this.m_acquiredItems)
			{
				if (mobileFriendListItem.IsHeader)
				{
					mobileFriendListItem.gameObject.SetActive(false);
				}
				else if (this.m_freelist.Count >= 20)
				{
					Object.Destroy(mobileFriendListItem.gameObject);
				}
				else
				{
					this.m_freelist.Add(mobileFriendListItem);
					mobileFriendListItem.gameObject.SetActive(false);
				}
				mobileFriendListItem.Unselected();
			}
			this.m_acquiredItems.Clear();
		}

		// Token: 0x06003FBE RID: 16318 RVA: 0x00135308 File Offset: 0x00133508
		public void ReleaseItem(ITouchListItem item)
		{
			MobileFriendListItem mobileFriendListItem = item as MobileFriendListItem;
			if (mobileFriendListItem == null)
			{
				throw new ArgumentException("given item is not MobileFriendListItem: " + item);
			}
			if (this.m_freelist == null)
			{
				this.m_freelist = new List<MobileFriendListItem>();
			}
			if (mobileFriendListItem.IsHeader)
			{
				mobileFriendListItem.gameObject.SetActive(false);
			}
			else if (this.m_freelist.Count >= 20)
			{
				Object.Destroy(item.gameObject);
			}
			else
			{
				this.m_freelist.Add(mobileFriendListItem);
				mobileFriendListItem.gameObject.SetActive(false);
			}
			if (!this.m_acquiredItems.Remove(mobileFriendListItem))
			{
				Log.Henry.Print("VirtualizedFriendsListBehavior.ReleaseItem item not found in m_acquiredItems: {0}", new object[]
				{
					mobileFriendListItem
				});
			}
			mobileFriendListItem.Unselected();
		}

		// Token: 0x06003FBF RID: 16319 RVA: 0x001353D8 File Offset: 0x001335D8
		public ITouchListItem AcquireItem(int index)
		{
			if (this.m_acquiredItems.Count >= this.MaxAcquiredItems)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Bug in ILongListBehavior? there are too many acquired items! index=",
					index,
					" max=",
					this.MaxAcquiredItems,
					" maxVisible=",
					this.MaxVisibleItems,
					" minBuffer=",
					this.MinBuffer,
					" acquiredItems.Count=",
					this.m_acquiredItems.Count,
					" hasCollapsedHeaders=",
					this.HasCollapsedHeaders
				}));
			}
			if (index < 0 || index >= this.m_friendList.m_allItems.Count)
			{
				throw new IndexOutOfRangeException(string.Format("Invalid index, {0} has {1} elements.", DebugUtils.GetHierarchyPathAndType(this.m_friendList, '.'), this.m_friendList.m_allItems.Count));
			}
			FriendListFrame.FriendListItem item = this.m_friendList.m_allItems[index];
			MobileFriendListItem.TypeFlags itemMainType = item.ItemMainType;
			Type frameType = item.GetFrameType();
			MobileFriendListItem.TypeFlags typeFlags;
			if (this.m_freelist != null && !item.IsHeader)
			{
				int num = this.m_freelist.FindLastIndex((MobileFriendListItem m) => (!item.IsHeader) ? (m.GetComponent(frameType) != null) : m.IsHeader);
				if (num >= 0 && this.m_freelist[num] == null)
				{
					for (int i = 0; i < this.m_freelist.Count; i++)
					{
						if (this.m_freelist[i] == null)
						{
							this.m_freelist.RemoveAt(i);
							i--;
						}
					}
					num = this.m_freelist.FindLastIndex((MobileFriendListItem m) => (!item.IsHeader) ? (m.GetComponent(frameType) != null) : m.IsHeader);
				}
				if (num >= 0)
				{
					MobileFriendListItem mobileFriendListItem = this.m_freelist[num];
					this.m_freelist.RemoveAt(num);
					typeFlags = itemMainType;
					if (typeFlags != MobileFriendListItem.TypeFlags.Friend)
					{
						if (typeFlags != MobileFriendListItem.TypeFlags.NearbyPlayer)
						{
							if (typeFlags != MobileFriendListItem.TypeFlags.Request)
							{
								throw new NotImplementedException(string.Concat(new object[]
								{
									"VirtualizedFriendsListBehavior.AcquireItem[reuse] frameType=",
									frameType.FullName,
									" itemType=",
									itemMainType
								}));
							}
							FriendListRequestFrame component = mobileFriendListItem.GetComponent<FriendListRequestFrame>();
							component.SetInvite(item.GetInvite());
							this.m_friendList.FinishCreateVisualItem<FriendListRequestFrame>(component, itemMainType, this.m_friendList.FindHeader(itemMainType), component.gameObject);
							bool activeSelf = component.gameObject.activeSelf;
							component.gameObject.SetActive(true);
							component.UpdateInvite();
							if (!activeSelf)
							{
								component.gameObject.SetActive(activeSelf);
							}
						}
						else
						{
							FriendListNearbyPlayerFrame component2 = mobileFriendListItem.GetComponent<FriendListNearbyPlayerFrame>();
							component2.SetNearbyPlayer(item.GetNearbyPlayer());
							this.m_friendList.FinishCreateVisualItem<FriendListNearbyPlayerFrame>(component2, itemMainType, this.m_friendList.FindHeader(itemMainType), component2.gameObject);
							bool activeSelf2 = component2.gameObject.activeSelf;
							component2.gameObject.SetActive(true);
							component2.UpdateNearbyPlayer();
							if (!activeSelf2)
							{
								component2.gameObject.SetActive(activeSelf2);
							}
						}
					}
					else
					{
						FriendListFriendFrame component3 = mobileFriendListItem.GetComponent<FriendListFriendFrame>();
						component3.SetFriend(item.GetFriend());
						this.m_friendList.FinishCreateVisualItem<FriendListFriendFrame>(component3, itemMainType, this.m_friendList.FindHeader(itemMainType), component3.gameObject);
						bool activeSelf3 = component3.gameObject.activeSelf;
						component3.gameObject.SetActive(true);
						component3.UpdateFriend();
						if (!activeSelf3)
						{
							component3.gameObject.SetActive(activeSelf3);
						}
					}
					mobileFriendListItem.gameObject.SetActive(true);
					this.m_acquiredItems.Add(mobileFriendListItem);
					return mobileFriendListItem;
				}
			}
			typeFlags = itemMainType;
			MobileFriendListItem mobileFriendListItem2;
			if (typeFlags != MobileFriendListItem.TypeFlags.Header)
			{
				if (typeFlags != MobileFriendListItem.TypeFlags.Friend)
				{
					if (typeFlags != MobileFriendListItem.TypeFlags.NearbyPlayer)
					{
						if (typeFlags != MobileFriendListItem.TypeFlags.Request)
						{
							throw new NotImplementedException("VirtualizedFriendsListBehavior.AcquireItem[new] type=" + frameType.FullName);
						}
						mobileFriendListItem2 = this.m_friendList.CreateRequestFrame(item.GetInvite());
					}
					else
					{
						mobileFriendListItem2 = this.m_friendList.CreateNearbyPlayerFrame(item.GetNearbyPlayer());
					}
				}
				else
				{
					mobileFriendListItem2 = this.m_friendList.CreateFriendFrame(item.GetFriend());
				}
			}
			else
			{
				mobileFriendListItem2 = this.m_friendList.FindHeader(item.SubType).GetComponent<MobileFriendListItem>();
			}
			this.m_acquiredItems.Add(mobileFriendListItem2);
			return mobileFriendListItem2;
		}

		// Token: 0x06003FC0 RID: 16320 RVA: 0x001358B4 File Offset: 0x00133AB4
		private void InitializeBoundsByTypeArray()
		{
			Array values = Enum.GetValues(typeof(MobileFriendListItem.TypeFlags));
			this.m_boundsByType = new Bounds[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				MobileFriendListItem.TypeFlags itemType = (MobileFriendListItem.TypeFlags)((int)values.GetValue(i));
				Component prefab = this.GetPrefab(itemType);
				int boundsByTypeIndex = this.GetBoundsByTypeIndex(itemType);
				this.m_boundsByType[boundsByTypeIndex] = ((!(prefab == null)) ? FriendListFrame.VirtualizedFriendsListBehavior.GetPrefabBounds(prefab.gameObject) : default(Bounds));
			}
		}

		// Token: 0x06003FC1 RID: 16321 RVA: 0x00135950 File Offset: 0x00133B50
		private int GetBoundsByTypeIndex(MobileFriendListItem.TypeFlags itemType)
		{
			if (itemType == MobileFriendListItem.TypeFlags.Header)
			{
				return 0;
			}
			if (itemType == MobileFriendListItem.TypeFlags.Recruit)
			{
				return 5;
			}
			if (itemType == MobileFriendListItem.TypeFlags.Friend)
			{
				return 4;
			}
			if (itemType == MobileFriendListItem.TypeFlags.CurrentGame)
			{
				return 3;
			}
			if (itemType == MobileFriendListItem.TypeFlags.NearbyPlayer)
			{
				return 2;
			}
			if (itemType != MobileFriendListItem.TypeFlags.Request)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Unknown ItemType: ",
					itemType,
					" (",
					(int)itemType,
					")"
				}));
			}
			return 1;
		}

		// Token: 0x06003FC2 RID: 16322 RVA: 0x001359DC File Offset: 0x00133BDC
		private Component GetPrefab(MobileFriendListItem.TypeFlags itemType)
		{
			if (itemType == MobileFriendListItem.TypeFlags.Header)
			{
				return this.m_friendList.prefabs.headerItem;
			}
			if (itemType == MobileFriendListItem.TypeFlags.Recruit)
			{
				return this.m_friendList.prefabs.recruitItem;
			}
			if (itemType == MobileFriendListItem.TypeFlags.Friend)
			{
				return this.m_friendList.prefabs.friendItem;
			}
			if (itemType == MobileFriendListItem.TypeFlags.CurrentGame)
			{
				return this.m_friendList.prefabs.currentGameItem;
			}
			if (itemType == MobileFriendListItem.TypeFlags.NearbyPlayer)
			{
				return this.m_friendList.prefabs.nearbyPlayerItem;
			}
			if (itemType != MobileFriendListItem.TypeFlags.Request)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Unknown ItemType: ",
					itemType,
					" (",
					(int)itemType,
					")"
				}));
			}
			return this.m_friendList.prefabs.requestItem;
		}

		// Token: 0x040028D4 RID: 10452
		private const int MAX_FREELIST_ITEMS = 20;

		// Token: 0x040028D5 RID: 10453
		private FriendListFrame m_friendList;

		// Token: 0x040028D6 RID: 10454
		private int m_cachedMaxVisibleItems = -1;

		// Token: 0x040028D7 RID: 10455
		private List<MobileFriendListItem> m_freelist;

		// Token: 0x040028D8 RID: 10456
		private HashSet<MobileFriendListItem> m_acquiredItems = new HashSet<MobileFriendListItem>();

		// Token: 0x040028D9 RID: 10457
		private Bounds[] m_boundsByType;
	}
}
