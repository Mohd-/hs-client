using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000411 RID: 1041
[CustomEditClass]
public class GeneralStorePacksContent : GeneralStoreContent
{
	// Token: 0x06003514 RID: 13588 RVA: 0x0010746C File Offset: 0x0010566C
	public override void PostStoreFlipIn(bool animatedFlipIn)
	{
		this.UpdatePacksTypeMusic();
		this.AnimateLogo(animatedFlipIn);
		this.AnimatePacksFlying(this.m_visiblePackCount, !animatedFlipIn);
		this.UpdateChinaKoreaInfoButton();
		this.m_savedLocalPosition = base.gameObject.transform.localPosition;
	}

	// Token: 0x06003515 RID: 13589 RVA: 0x001074B2 File Offset: 0x001056B2
	public override void PreStoreFlipOut()
	{
		this.ResetAnimations();
		this.GetCurrentDisplay().ClearPacks();
		this.UpdateChinaKoreaInfoButton();
	}

	// Token: 0x06003516 RID: 13590 RVA: 0x001074CC File Offset: 0x001056CC
	public override void StoreShown(bool isCurrent)
	{
		if (!isCurrent)
		{
			return;
		}
		this.AnimateLogo(false);
		this.AnimatePacksFlying(this.m_visiblePackCount, true);
		this.UpdatePackBuyButtons();
		this.UpdatePacksTypeMusic();
		this.UpdateChinaKoreaInfoButton();
	}

	// Token: 0x06003517 RID: 13591 RVA: 0x00107508 File Offset: 0x00105708
	public override void StoreHidden(bool isCurrent)
	{
		if (!isCurrent)
		{
			return;
		}
		this.ResetAnimations();
		if (this.m_quantityPrompt != null)
		{
			this.m_quantityPrompt.Hide();
		}
		this.GetCurrentDisplay().ClearPacks();
	}

	// Token: 0x06003518 RID: 13592 RVA: 0x00107549 File Offset: 0x00105749
	public override bool IsPurchaseDisabled()
	{
		return this.m_selectedBoosterId == 0;
	}

	// Token: 0x06003519 RID: 13593 RVA: 0x00107554 File Offset: 0x00105754
	public override string GetMoneyDisplayOwnedText()
	{
		return GameStrings.Get("GLUE_STORE_PACK_BUTTON_COST_OWNED_TEXT");
	}

	// Token: 0x0600351A RID: 13594 RVA: 0x00107560 File Offset: 0x00105760
	public void SetBoosterId(int id, bool forceImmediate = false)
	{
		if (this.m_selectedBoosterId == id)
		{
			return;
		}
		bool flag = this.m_selectedBoosterId == 0;
		this.GetCurrentDisplay().ClearPacks();
		this.m_visiblePackCount = 0;
		this.m_selectedBoosterId = id;
		if (flag)
		{
			this.UpdateSelectedBundle(false);
		}
		this.ResetAnimations();
		this.AnimateAndUpdateDisplay(id, forceImmediate);
		this.AnimateLogo(!forceImmediate);
		this.AnimatePacksFlying(this.m_visiblePackCount, forceImmediate);
		this.UpdatePackBuyButtons();
		this.UpdatePacksDescription();
		this.UpdatePacksTypeMusic();
		this.UpdateChinaKoreaInfoButton();
		if (base.GetCurrentGoldBundle() != null)
		{
			base.SetCurrentGoldBundle(this.GetCurrentGTAPPTransactionData());
		}
		else if (base.GetCurrentMoneyBundle() != null)
		{
			this.HandleMoneyPackBuyButtonClick(this.m_lastBundleIndex);
		}
	}

	// Token: 0x0600351B RID: 13595 RVA: 0x0010761A File Offset: 0x0010581A
	public int GetBoosterId()
	{
		return this.m_selectedBoosterId;
	}

	// Token: 0x0600351C RID: 13596 RVA: 0x00107622 File Offset: 0x00105822
	public Map<int, StorePackDef> GetStorePackDefs()
	{
		return this.m_storePackDefs;
	}

	// Token: 0x0600351D RID: 13597 RVA: 0x0010762C File Offset: 0x0010582C
	public StorePackDef GetStorePackDef(int packDbId)
	{
		StorePackDef result = null;
		this.m_storePackDefs.TryGetValue(packDbId, out result);
		return result;
	}

	// Token: 0x0600351E RID: 13598 RVA: 0x0010764C File Offset: 0x0010584C
	public void ShakeStore(int numPacks, float maxXRotation, float delay = 0f)
	{
		if (numPacks == 0)
		{
			return;
		}
		int num = 1;
		List<Network.Bundle> packBundles = this.GetPackBundles(false);
		foreach (Network.Bundle bundle in packBundles)
		{
			Network.BundleItem bundleItem = bundle.Items.Find((Network.BundleItem obj) => obj.Product == 1);
			if (bundleItem != null)
			{
				num = Mathf.Max(bundleItem.Quantity, num);
			}
		}
		int num2 = num - 1;
		if (num2 == 0)
		{
			return;
		}
		float xRotationAmount = (float)numPacks / (float)num2 * maxXRotation;
		this.m_parentStore.ShakeStore(xRotationAmount, this.m_packFlyShakeTime, delay);
	}

	// Token: 0x0600351F RID: 13599 RVA: 0x0010771C File Offset: 0x0010591C
	protected override void OnBundleChanged(NoGTAPPTransactionData goldBundle, Network.Bundle moneyBundle)
	{
		if (goldBundle != null)
		{
			this.m_visiblePackCount = goldBundle.Quantity;
		}
		else if (moneyBundle != null)
		{
			Network.BundleItem bundleItem = moneyBundle.Items.Find((Network.BundleItem obj) => obj.Product == 1);
			this.m_visiblePackCount = ((bundleItem != null) ? bundleItem.Quantity : 0);
		}
		this.AnimatePacksFlying(this.m_visiblePackCount, false);
	}

	// Token: 0x06003520 RID: 13600 RVA: 0x00107794 File Offset: 0x00105994
	protected override void OnRefresh()
	{
		this.UpdatePackBuyButtons();
		this.UpdatePacksDescription();
		if (base.HasBundleSet())
		{
			return;
		}
		if (this.m_selectedBoosterId == 0)
		{
			return;
		}
		this.UpdateSelectedBundle(true);
	}

	// Token: 0x06003521 RID: 13601 RVA: 0x001077CC File Offset: 0x001059CC
	private void Awake()
	{
		this.m_packDisplay1 = this.m_packDisplay;
		this.m_packDisplay2 = Object.Instantiate<GeneralStorePacksContentDisplay>(this.m_packDisplay);
		this.m_packDisplay2.transform.parent = this.m_packDisplay1.transform.parent;
		this.m_packDisplay2.transform.localPosition = this.m_packDisplay1.transform.localPosition;
		this.m_packDisplay2.transform.localScale = this.m_packDisplay1.transform.localScale;
		this.m_packDisplay2.transform.localRotation = this.m_packDisplay1.transform.localRotation;
		this.m_packDisplay2.gameObject.SetActive(false);
		this.m_logoMesh1 = this.m_logoMesh;
		this.m_logoMesh2 = Object.Instantiate<MeshRenderer>(this.m_logoMesh);
		this.m_logoMesh2.transform.parent = this.m_logoMesh1.transform.parent;
		this.m_logoMesh2.transform.localPosition = this.m_logoMesh1.transform.localPosition;
		this.m_logoMesh2.transform.localScale = this.m_logoMesh1.transform.localScale;
		this.m_logoMesh2.transform.localRotation = this.m_logoMesh1.transform.localRotation;
		this.m_logoMesh2.gameObject.SetActive(false);
		this.m_logoGlowMesh1 = this.m_logoGlowMesh;
		this.m_logoGlowMesh2 = Object.Instantiate<MeshRenderer>(this.m_logoGlowMesh);
		this.m_logoGlowMesh2.transform.parent = this.m_logoGlowMesh1.transform.parent;
		this.m_logoGlowMesh2.transform.localPosition = this.m_logoGlowMesh1.transform.localPosition;
		this.m_logoGlowMesh2.transform.localScale = this.m_logoGlowMesh1.transform.localScale;
		this.m_logoGlowMesh2.transform.localRotation = this.m_logoGlowMesh1.transform.localRotation;
		this.m_logoGlowMesh2.gameObject.SetActive(false);
		this.m_packDisplay1.SetParent(this);
		this.m_packDisplay2.SetParent(this);
		this.m_productType = 1;
		this.m_packBuyContainer.SetActive(false);
		if (this.m_availableDateText != null)
		{
			this.m_availableDateTextOrigScale = this.m_availableDateText.transform.localScale;
		}
		if (this.m_ChinaInfoButton != null)
		{
			this.m_ChinaInfoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChinaKoreaInfoPressed));
		}
		List<BoosterDbfRecord> packRecordsWithStorePrefab = GameUtils.GetPackRecordsWithStorePrefab();
		foreach (BoosterDbfRecord boosterDbfRecord in packRecordsWithStorePrefab)
		{
			int id = boosterDbfRecord.ID;
			string storePrefab = boosterDbfRecord.StorePrefab;
			GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(storePrefab), false, false);
			if (gameObject == null)
			{
				Debug.LogError(string.Format("Unable to load store pack def: {0}", storePrefab));
			}
			else
			{
				StorePackDef component = gameObject.GetComponent<StorePackDef>();
				if (component == null)
				{
					Debug.LogError(string.Format("StorePackDef component not found: {0}", storePrefab));
				}
				else
				{
					this.m_storePackDefs.Add(id, component);
				}
			}
		}
		this.UpdateChinaKoreaInfoButton();
	}

	// Token: 0x06003522 RID: 13602 RVA: 0x00107B30 File Offset: 0x00105D30
	private GameObject GetCurrentDisplayContainer()
	{
		return this.GetCurrentDisplay().gameObject;
	}

	// Token: 0x06003523 RID: 13603 RVA: 0x00107B40 File Offset: 0x00105D40
	private GameObject GetNextDisplayContainer()
	{
		return ((this.m_currentDisplay + 1) % 2 != 0) ? this.m_packDisplay2.gameObject : this.m_packDisplay1.gameObject;
	}

	// Token: 0x06003524 RID: 13604 RVA: 0x00107B77 File Offset: 0x00105D77
	private GeneralStorePacksContentDisplay GetCurrentDisplay()
	{
		return (this.m_currentDisplay != 0) ? this.m_packDisplay2 : this.m_packDisplay1;
	}

	// Token: 0x06003525 RID: 13605 RVA: 0x00107B95 File Offset: 0x00105D95
	private MeshRenderer GetCurrentLogo()
	{
		return (this.m_currentDisplay != 0) ? this.m_logoMesh2 : this.m_logoMesh1;
	}

	// Token: 0x06003526 RID: 13606 RVA: 0x00107BB3 File Offset: 0x00105DB3
	private MeshRenderer GetCurrentGlowLogo()
	{
		return (this.m_currentDisplay != 0) ? this.m_logoGlowMesh2 : this.m_logoGlowMesh1;
	}

	// Token: 0x06003527 RID: 13607 RVA: 0x00107BD4 File Offset: 0x00105DD4
	private void UpdateSelectedBundle(bool forceUpdate = false)
	{
		NoGTAPPTransactionData noGTAPPTransactionData = new NoGTAPPTransactionData
		{
			Product = this.m_productType,
			ProductData = this.m_selectedBoosterId,
			Quantity = 1
		};
		long num;
		if (StoreManager.Get().GetGoldCostNoGTAPP(noGTAPPTransactionData, out num))
		{
			base.SetCurrentGoldBundle(noGTAPPTransactionData);
		}
		else
		{
			Network.Bundle lowestCostUnownedBundle = StoreManager.Get().GetLowestCostUnownedBundle(this.m_productType, false, this.m_selectedBoosterId, 0);
			if (lowestCostUnownedBundle != null)
			{
				base.SetCurrentMoneyBundle(lowestCostUnownedBundle, forceUpdate);
			}
		}
	}

	// Token: 0x06003528 RID: 13608 RVA: 0x00107C50 File Offset: 0x00105E50
	private void UpdatePacksDescription()
	{
		if (this.m_selectedBoosterId == 0)
		{
			this.m_parentStore.HideAccentTexture();
			this.m_parentStore.SetChooseDescription(GameStrings.Get("GLUE_STORE_CHOOSE_PACK"));
			return;
		}
		BoosterDbfRecord record = GameDbf.Booster.GetRecord(this.m_selectedBoosterId);
		string text = record.Name;
		string title = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_PACK");
		string desc = GameStrings.Format("GLUE_STORE_PRODUCT_DETAILS_PACK", new object[]
		{
			text
		});
		Network.Bundle currentMoneyBundle = base.GetCurrentMoneyBundle();
		bool flag = false;
		if (currentMoneyBundle != null)
		{
			flag = StoreManager.Get().IsProductPrePurchase(currentMoneyBundle);
			if (flag && record.ID == 10)
			{
				desc = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_TGT_PACK_PRESALE");
				title = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_TGT_PACK_PRESALE");
			}
			if (flag && record.ID == 11)
			{
				desc = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_OG_PACK_PRESALE");
				title = GameStrings.Get("GLUE_STORE_PRODUCT_DETAILS_HEADLINE_OG_PACK_PRESALE");
			}
		}
		string warning = string.Empty;
		if (StoreManager.Get().IsKoreanCustomer())
		{
			if (flag)
			{
				warning = GameStrings.Get("GLUE_STORE_KOREAN_PRODUCT_DETAILS_PACKS_PREORDER");
			}
			else
			{
				warning = GameStrings.Get("GLUE_STORE_KOREAN_PRODUCT_DETAILS_EXPERT_PACK");
			}
		}
		this.m_parentStore.SetDescription(title, desc, warning);
		StorePackDef storePackDef = this.GetStorePackDef(this.m_selectedBoosterId);
		if (storePackDef != null)
		{
			Texture accentTexture = null;
			if (!string.IsNullOrEmpty(storePackDef.m_accentTextureName))
			{
				accentTexture = AssetLoader.Get().LoadTexture(FileUtils.GameAssetPathToName(storePackDef.m_accentTextureName), false);
			}
			this.m_parentStore.SetAccentTexture(accentTexture);
		}
	}

	// Token: 0x06003529 RID: 13609 RVA: 0x00107DD8 File Offset: 0x00105FD8
	private NoGTAPPTransactionData GetCurrentGTAPPTransactionData()
	{
		return new NoGTAPPTransactionData
		{
			Product = this.m_productType,
			ProductData = this.m_selectedBoosterId,
			Quantity = this.m_currentGoldPackQuantity
		};
	}

	// Token: 0x0600352A RID: 13610 RVA: 0x00107E10 File Offset: 0x00106010
	private void UpdatePackBuyButtons()
	{
		if (this.m_selectedBoosterId == 0)
		{
			return;
		}
		Network.Bundle preOrderBundle;
		bool flag = StoreManager.Get().IsBoosterPreorderActive(this.m_selectedBoosterId, out preOrderBundle);
		if (flag)
		{
			this.ShowPreorderBuyButtons(preOrderBundle);
		}
		else
		{
			this.ShowStandardBuyButtons();
		}
	}

	// Token: 0x0600352B RID: 13611 RVA: 0x00107E54 File Offset: 0x00106054
	private void ShowStandardBuyButtons()
	{
		this.m_packBuyPreorderContainer.SetActive(false);
		this.m_packBuyContainer.SetActive(true);
		Action action = null;
		int num = 0;
		GeneralStorePackBuyButton goldButton = this.GetPackBuyButton(num);
		if (goldButton == null)
		{
			goldButton = this.CreatePackBuyButton(num);
			goldButton.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
			{
				if (!this.IsContentActive())
				{
					return;
				}
				this.HandleGoldPackBuyButtonClick();
				this.SelectPackBuyButton(goldButton);
			});
			if (!UniversalInputManager.UsePhoneUI)
			{
				goldButton.AddEventListener(UIEventType.DOUBLECLICK, delegate(UIEvent e)
				{
					this.HandleGoldPackBuyButtonDoubleClick(goldButton);
				});
			}
		}
		if (this.m_selectedBoosterId != 0)
		{
			goldButton.UpdateFromGTAPP(this.GetCurrentGTAPPTransactionData());
		}
		action = delegate()
		{
			this.HandleGoldPackBuyButtonClick();
			this.SelectPackBuyButton(goldButton);
		};
		goldButton.Unselect();
		List<Network.Bundle> list = this.GetPackBundles(true);
		if (list.Count > this.m_maxPackBuyButtons - 1)
		{
			list = list.GetRange(0, this.m_maxPackBuyButtons - 1);
		}
		for (int i = 0; i < list.Count; i++)
		{
			num++;
			int bundleIndexCopy = i;
			Network.Bundle bundle = list[i];
			Network.BundleItem bundleItem = bundle.Items.Find((Network.BundleItem obj) => obj.Product == 1);
			if (bundleItem == null)
			{
				Debug.LogWarning(string.Format("GeneralStorePacksContent.UpdatePackBuyButtons() bundle {0} has no packs bundle item!", bundle.ProductID));
			}
			else
			{
				GeneralStorePackBuyButton moneyButton = this.GetPackBuyButton(num);
				if (moneyButton == null)
				{
					moneyButton = this.CreatePackBuyButton(num);
					moneyButton.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
					{
						if (!this.IsContentActive())
						{
							return;
						}
						this.HandleMoneyPackBuyButtonClick(bundleIndexCopy);
						this.SelectPackBuyButton(moneyButton);
					});
				}
				string productQuantityText = StoreManager.Get().GetProductQuantityText(bundleItem.Product, bundleItem.ProductData, bundleItem.Quantity);
				moneyButton.SetMoneyValue(bundle, productQuantityText);
				moneyButton.gameObject.SetActive(true);
				if (moneyButton.IsSelected() || base.GetCurrentMoneyBundle() == bundle)
				{
					action = delegate()
					{
						this.HandleMoneyPackBuyButtonClick(bundleIndexCopy);
						this.SelectPackBuyButton(moneyButton);
					};
				}
				moneyButton.Unselect();
			}
		}
		bool flag = StoreManager.Get().CanBuyBoosterWithGold(this.m_selectedBoosterId);
		goldButton.gameObject.SetActive(flag);
		if (!flag && this.m_packBuyButtons[0] != null)
		{
			this.m_packBuyButtons[0].gameObject.SetActive(false);
		}
		for (int j = num + 1; j < this.m_packBuyButtons.Count; j++)
		{
			GeneralStorePackBuyButton generalStorePackBuyButton = this.m_packBuyButtons[j];
			if (generalStorePackBuyButton != null)
			{
				generalStorePackBuyButton.gameObject.SetActive(false);
			}
		}
		int num2 = 1;
		foreach (GeneralStorePacksContent.ToggleableButtonFrame toggleableButtonFrame in this.m_toggleableButtonFrames)
		{
			bool active = num2 <= num;
			toggleableButtonFrame.m_IBar.SetActive(active);
			toggleableButtonFrame.m_Middle.SetActive(active);
			num2++;
		}
		if (this.m_packBuyFrameContainer != null)
		{
			this.m_packBuyFrameContainer.UpdateSlices();
		}
		this.m_packBuyButtonContainer.UpdateSlices();
		if (action != null)
		{
			action.Invoke();
		}
	}

	// Token: 0x0600352C RID: 13612 RVA: 0x00108204 File Offset: 0x00106404
	private void ShowPreorderBuyButtons(Network.Bundle preOrderBundle)
	{
		this.m_packBuyContainer.SetActive(false);
		this.m_packBuyPreorderContainer.SetActive(true);
		if (!this.m_packBuyPreorderButton.HasEventListener(UIEventType.PRESS))
		{
			this.m_packBuyPreorderButton.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
			{
				if (!base.IsContentActive())
				{
					return;
				}
				this.HandleMoneyPackBuyButtonClick(0);
				this.m_packBuyPreorderButton.Select();
			});
		}
		string quantityText = string.Empty;
		if (preOrderBundle != null)
		{
			Network.BundleItem bundleItem = preOrderBundle.Items.Find((Network.BundleItem obj) => obj.Product == 1);
			quantityText = GameStrings.Format("GLUE_STORE_PACKS_BUTTON_PREORDER_TEXT", new object[]
			{
				bundleItem.Quantity
			});
		}
		this.m_packBuyPreorderButton.SetMoneyValue(preOrderBundle, quantityText);
		this.HandleMoneyPackBuyButtonClick(0);
		this.m_packBuyPreorderButton.Select();
	}

	// Token: 0x0600352D RID: 13613 RVA: 0x001082C8 File Offset: 0x001064C8
	private void UpdatePacksTypeMusic()
	{
		if (this.m_parentStore.GetMode() == GeneralStoreMode.NONE)
		{
			return;
		}
		StorePackDef storePackDef = this.GetStorePackDef(this.m_selectedBoosterId);
		if (storePackDef == null || storePackDef.m_playlist == MusicPlaylistType.Invalid || !MusicManager.Get().StartPlaylist(storePackDef.m_playlist))
		{
			this.m_parentStore.ResumePreviousMusicPlaylist();
		}
	}

	// Token: 0x0600352E RID: 13614 RVA: 0x0010832C File Offset: 0x0010652C
	private void HandleGoldPackBuyButtonClick()
	{
		base.SetCurrentGoldBundle(new NoGTAPPTransactionData
		{
			Product = this.m_productType,
			ProductData = this.m_selectedBoosterId,
			Quantity = this.m_currentGoldPackQuantity
		});
		this.UpdatePacksDescription();
	}

	// Token: 0x0600352F RID: 13615 RVA: 0x00108370 File Offset: 0x00106570
	private void HandleGoldPackBuyButtonDoubleClick(GeneralStorePackBuyButton button)
	{
		this.m_parentStore.ActivateCover(true);
		this.m_quantityPrompt.Show(GeneralStorePacksContent.MAX_QUANTITY_BOUGHT_WITH_GOLD, delegate(int quantity)
		{
			this.m_parentStore.ActivateCover(false);
			this.m_currentGoldPackQuantity = quantity;
			NoGTAPPTransactionData currentGTAPPTransactionData = this.GetCurrentGTAPPTransactionData();
			button.UpdateFromGTAPP(currentGTAPPTransactionData);
			this.SetCurrentGoldBundle(currentGTAPPTransactionData);
		}, delegate()
		{
			this.m_parentStore.ActivateCover(false);
		});
	}

	// Token: 0x06003530 RID: 13616 RVA: 0x001083C8 File Offset: 0x001065C8
	private void HandleMoneyPackBuyButtonClick(int bundleIndex)
	{
		Network.Bundle bundle = null;
		List<Network.Bundle> packBundles = this.GetPackBundles(true);
		if (packBundles != null && packBundles.Count > 0)
		{
			if (bundleIndex >= packBundles.Count)
			{
				bundleIndex = 0;
			}
			bundle = packBundles[bundleIndex];
		}
		base.SetCurrentMoneyBundle(bundle, true);
		this.m_lastBundleIndex = bundleIndex;
		this.UpdatePacksDescription();
	}

	// Token: 0x06003531 RID: 13617 RVA: 0x00108420 File Offset: 0x00106620
	private void SelectPackBuyButton(GeneralStorePackBuyButton packBuyBtn)
	{
		foreach (GeneralStorePackBuyButton generalStorePackBuyButton in this.m_packBuyButtons)
		{
			generalStorePackBuyButton.Unselect();
		}
		packBuyBtn.Select();
	}

	// Token: 0x06003532 RID: 13618 RVA: 0x00108480 File Offset: 0x00106680
	private GeneralStorePackBuyButton GetPackBuyButton(int index)
	{
		if (index < this.m_packBuyButtons.Count)
		{
			return this.m_packBuyButtons[index];
		}
		return null;
	}

	// Token: 0x06003533 RID: 13619 RVA: 0x001084A4 File Offset: 0x001066A4
	private GeneralStorePackBuyButton CreatePackBuyButton(int buttonIndex)
	{
		if (buttonIndex >= this.m_packBuyButtons.Count)
		{
			int num = buttonIndex - this.m_packBuyButtons.Count + 1;
			for (int i = 0; i < num; i++)
			{
				GeneralStorePackBuyButton generalStorePackBuyButton = (GeneralStorePackBuyButton)GameUtils.Instantiate(this.m_packBuyButtonPrefab, this.m_packBuyButtonContainer.gameObject, true);
				SceneUtils.SetLayer(generalStorePackBuyButton.gameObject, this.m_packBuyButtonContainer.gameObject.layer);
				generalStorePackBuyButton.transform.localRotation = Quaternion.identity;
				generalStorePackBuyButton.transform.localScale = Vector3.one;
				this.m_packBuyButtonContainer.AddSlice(generalStorePackBuyButton.gameObject);
				this.m_packBuyButtons.Add(generalStorePackBuyButton);
			}
			this.m_packBuyButtonContainer.UpdateSlices();
		}
		return this.m_packBuyButtons[buttonIndex];
	}

	// Token: 0x06003534 RID: 13620 RVA: 0x00108570 File Offset: 0x00106770
	private List<Network.Bundle> GetPackBundles(bool sortByPackQuantity)
	{
		List<Network.Bundle> allBundlesForProduct = StoreManager.Get().GetAllBundlesForProduct(1, GeneralStorePacksContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, this.m_selectedBoosterId, 0);
		if (sortByPackQuantity)
		{
			allBundlesForProduct.Sort(delegate(Network.Bundle left, Network.Bundle right)
			{
				int num;
				if (left == null)
				{
					num = 0;
				}
				else
				{
					num = Enumerable.Max<Network.BundleItem>(Enumerable.Where<Network.BundleItem>(left.Items, (Network.BundleItem i) => i.Product == 1), (Network.BundleItem i) => i.Quantity);
				}
				int num2 = num;
				int num3;
				if (right == null)
				{
					num3 = 0;
				}
				else
				{
					num3 = Enumerable.Max<Network.BundleItem>(Enumerable.Where<Network.BundleItem>(right.Items, (Network.BundleItem i) => i.Product == 1), (Network.BundleItem i) => i.Quantity);
				}
				int num4 = num3;
				return num2 - num4;
			});
		}
		return allBundlesForProduct;
	}

	// Token: 0x06003535 RID: 13621 RVA: 0x001085C0 File Offset: 0x001067C0
	private void AnimateLogo(bool animateLogo)
	{
		if (!this.m_hasLogo || !base.gameObject.activeInHierarchy || this.m_selectedBoosterId == 0)
		{
			return;
		}
		MeshRenderer currentLogo = this.GetCurrentLogo();
		currentLogo.gameObject.SetActive(true);
		GeneralStorePacksContent.LogoAnimation logoAnimation = this.m_logoAnimation;
		if (logoAnimation != GeneralStorePacksContent.LogoAnimation.Slam)
		{
			if (logoAnimation == GeneralStorePacksContent.LogoAnimation.Fade)
			{
				if (animateLogo)
				{
					this.m_logoAnimCoroutine = base.StartCoroutine(this.AnimateFadeLogo(currentLogo));
				}
				else if (!this.m_animatingLogo)
				{
					currentLogo.gameObject.SetActive(false);
				}
			}
		}
		else if (animateLogo)
		{
			this.m_logoAnimCoroutine = base.StartCoroutine(this.AnimateSlamLogo(currentLogo));
		}
		else if (!this.m_animatingLogo)
		{
			currentLogo.transform.localPosition = this.m_logoAnimationEndBone.transform.localPosition;
			currentLogo.gameObject.SetActive(true);
		}
	}

	// Token: 0x06003536 RID: 13622 RVA: 0x001086B0 File Offset: 0x001068B0
	private void AnimatePacksFlying(int numVisiblePacks, bool forceImmediate = false)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		GeneralStorePacksContentDisplay currentDisplay = this.GetCurrentDisplay();
		if (this.m_packAnimCoroutine != null)
		{
			base.StopCoroutine(this.m_packAnimCoroutine);
		}
		if (this.m_preorderCardBackAnimCoroutine != null)
		{
			base.StopCoroutine(this.m_preorderCardBackAnimCoroutine);
		}
		this.m_packAnimCoroutine = base.StartCoroutine(this.AnimatePacks(currentDisplay, numVisiblePacks, forceImmediate));
		this.m_preorderCardBackAnimCoroutine = base.StartCoroutine(this.AnimatePreorderUI(currentDisplay));
	}

	// Token: 0x06003537 RID: 13623 RVA: 0x0010872C File Offset: 0x0010692C
	private IEnumerator AnimateFadeLogo(MeshRenderer logo)
	{
		if (logo == null || !logo.gameObject.activeInHierarchy)
		{
			yield break;
		}
		while (this.m_animatingLogo)
		{
			yield return null;
		}
		this.m_animatingLogo = true;
		PlayMakerFSM logoFSM = logo.GetComponent<PlayMakerFSM>();
		logo.transform.localPosition = this.m_logoAnimationStartBone.transform.localPosition;
		iTween.MoveFrom(logo.gameObject, iTween.Hash(new object[]
		{
			"position",
			logo.transform.localPosition - this.m_logoAppearOffset,
			"easetype",
			iTween.EaseType.easeInQuint,
			"time",
			this.m_logoIntroTime,
			"islocal",
			true
		}));
		AnimationUtil.FadeTexture(logo, 0f, 1f, this.m_logoIntroTime, 0f, null);
		if (logoFSM != null)
		{
			logoFSM.SendEvent("FadeIn");
		}
		if (this.m_logoHoldTime > 0f)
		{
			yield return new WaitForSeconds(this.m_logoHoldTime);
		}
		AnimationUtil.FadeTexture(logo, 1f, 0f, this.m_logoOutroTime, 0f, null);
		if (logoFSM != null)
		{
			logoFSM.SendEvent("FadeOut");
		}
		yield return new WaitForSeconds(this.m_logoOutroTime);
		this.m_animatingLogo = false;
		yield break;
	}

	// Token: 0x06003538 RID: 13624 RVA: 0x00108758 File Offset: 0x00106958
	private IEnumerator AnimateSlamLogo(MeshRenderer logo)
	{
		if (logo == null || !logo.gameObject.activeInHierarchy)
		{
			yield break;
		}
		while (this.m_animatingLogo)
		{
			yield return null;
		}
		this.m_animatingLogo = true;
		PlayMakerFSM logoFSM = logo.GetComponent<PlayMakerFSM>();
		logo.transform.localPosition = this.m_logoAnimationStartBone.transform.localPosition;
		iTween.MoveFrom(logo.gameObject, iTween.Hash(new object[]
		{
			"position",
			logo.transform.localPosition - this.m_logoAppearOffset,
			"easetype",
			iTween.EaseType.easeInQuint,
			"time",
			this.m_logoIntroTime,
			"islocal",
			true
		}));
		AnimationUtil.FadeTexture(logo, 0f, 1f, this.m_logoIntroTime, 0f, null);
		if (logoFSM != null)
		{
			logoFSM.SendEvent("FadeIn");
		}
		yield return new WaitForSeconds(this.m_logoIntroTime);
		if (this.m_logoHoldTime > 0f)
		{
			yield return new WaitForSeconds(this.m_logoHoldTime);
		}
		iTween.MoveTo(logo.gameObject, iTween.Hash(new object[]
		{
			"position",
			this.m_logoAnimationEndBone.transform.localPosition,
			"easetype",
			iTween.EaseType.easeInQuint,
			"time",
			this.m_logoOutroTime,
			"islocal",
			true
		}));
		yield return new WaitForSeconds(this.m_logoOutroTime);
		if (logoFSM != null)
		{
			logoFSM.SendEvent("PostSlamIn");
		}
		base.gameObject.transform.localPosition = this.m_savedLocalPosition;
		iTween.Stop(base.gameObject);
		iTween.PunchScale(base.gameObject, this.m_punchAmount, this.m_logoDisplayPunchTime);
		yield return new WaitForSeconds(this.m_logoDisplayPunchTime * 0.5f);
		this.m_animatingLogo = false;
		yield break;
	}

	// Token: 0x06003539 RID: 13625 RVA: 0x00108784 File Offset: 0x00106984
	private IEnumerator AnimatePacks(GeneralStorePacksContentDisplay display, int numVisiblePacks, bool forceImmediate)
	{
		while (this.m_animatingLogo)
		{
			yield return null;
		}
		this.m_animatingPacks = true;
		int packsFlown = display.ShowPacks(numVisiblePacks, this.m_packFlyInAnimTime, this.m_packFlyOutAnimTime, this.m_packFlyInDelay, this.m_packFlyOutDelay, forceImmediate);
		if (!forceImmediate && packsFlown != 0)
		{
			int packCount = Mathf.Abs(packsFlown);
			float shakeX = (packCount <= 0) ? this.m_maxPackFlyOutXShake : this.m_maxPackFlyInXShake;
			float delay = (packCount <= 0) ? this.m_packFlyOutDelay : this.m_packFlyInDelay;
			this.ShakeStore(packCount, shakeX, (float)packCount * delay * this.m_shakeObjectDelayMultiplier);
			yield return new WaitForSeconds(delay);
		}
		this.m_animatingPacks = false;
		yield break;
	}

	// Token: 0x0600353A RID: 13626 RVA: 0x001087CC File Offset: 0x001069CC
	private IEnumerator AnimatePreorderUI(GeneralStorePacksContentDisplay display)
	{
		if (this.m_preorderCardBackReward != null)
		{
			this.m_preorderCardBackReward.HideCardBackReward();
		}
		if (this.m_availableDateText != null)
		{
			this.m_availableDateText.gameObject.SetActive(false);
			this.m_availableDateText.transform.localScale = this.m_availableDateTextOrigScale;
		}
		if (!base.IsContentActive() || this.m_selectedBoosterId == 0)
		{
			yield break;
		}
		Network.Bundle preOrderBundle;
		if (!StoreManager.Get().IsBoosterPreorderActive(this.m_selectedBoosterId, out preOrderBundle))
		{
			yield break;
		}
		while (this.m_animatingLogo || this.m_animatingPacks)
		{
			yield return null;
		}
		Network.BundleItem cardBackItem = preOrderBundle.Items.Find((Network.BundleItem o) => o.Product == 5);
		if (cardBackItem == null)
		{
			yield break;
		}
		this.m_preorderCardBackReward.SetCardBack(cardBackItem.ProductData);
		this.m_preorderCardBackReward.ShowCardBackReward();
		if (this.m_availableDateText != null)
		{
			StorePackDef packDef = this.GetStorePackDef(this.m_selectedBoosterId);
			if (packDef != null)
			{
				this.m_availableDateText.Text = packDef.m_preorderAvailableDateString;
			}
			this.m_availableDateText.gameObject.SetActive(true);
			this.m_availableDateText.transform.localScale = this.m_availableDateTextOrigScale * 0.01f;
			iTween.ScaleTo(this.m_availableDateText.gameObject, iTween.Hash(new object[]
			{
				"scale",
				this.m_availableDateTextOrigScale,
				"time",
				0.25f,
				"easetype",
				iTween.EaseType.easeOutQuad
			}));
		}
		yield break;
	}

	// Token: 0x0600353B RID: 13627 RVA: 0x001087E8 File Offset: 0x001069E8
	private void ResetAnimations()
	{
		if (this.m_preorderCardBackReward != null)
		{
			this.m_preorderCardBackReward.HideCardBackReward();
		}
		if (this.m_availableDateText != null)
		{
			this.m_availableDateText.gameObject.SetActive(false);
		}
		if (this.m_logoAnimCoroutine != null)
		{
			iTween.Stop(this.m_logoMesh1.gameObject);
			iTween.Stop(this.m_logoMesh2.gameObject);
			base.StopCoroutine(this.m_logoAnimCoroutine);
		}
		this.m_logoMesh1.gameObject.SetActive(false);
		this.m_logoMesh2.gameObject.SetActive(false);
		if (this.m_packAnimCoroutine != null)
		{
			base.StopCoroutine(this.m_packAnimCoroutine);
		}
		if (this.m_preorderCardBackAnimCoroutine != null)
		{
			base.StopCoroutine(this.m_preorderCardBackAnimCoroutine);
		}
		this.m_animatingLogo = false;
		this.m_animatingPacks = false;
	}

	// Token: 0x0600353C RID: 13628 RVA: 0x001088C8 File Offset: 0x00106AC8
	private void AnimateAndUpdateDisplay(int id, bool forceImmediate = false)
	{
		if (this.m_preorderCardBackReward != null)
		{
			this.m_preorderCardBackReward.HideCardBackReward();
		}
		GameObject currDisplay = null;
		if (this.m_currentDisplay == -1)
		{
			this.m_currentDisplay = 1;
			currDisplay = this.m_packEmptyDisplay;
		}
		else
		{
			currDisplay = this.GetCurrentDisplayContainer();
		}
		GameObject nextDisplayContainer = this.GetNextDisplayContainer();
		MeshRenderer currentLogo = this.GetCurrentLogo();
		currentLogo.gameObject.SetActive(false);
		GeneralStorePacksContentDisplay currentDisplay = this.GetCurrentDisplay();
		currentDisplay.ClearPacks();
		this.m_currentDisplay = (this.m_currentDisplay + 1) % 2;
		nextDisplayContainer.SetActive(true);
		if (!forceImmediate)
		{
			currDisplay.transform.localRotation = Quaternion.identity;
			nextDisplayContainer.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
			iTween.StopByName(currDisplay, "ROTATION_TWEEN");
			iTween.StopByName(nextDisplayContainer, "ROTATION_TWEEN");
			iTween.RotateBy(currDisplay, iTween.Hash(new object[]
			{
				"amount",
				new Vector3(0.5f, 0f, 0f),
				"time",
				0.5f,
				"name",
				"ROTATION_TWEEN",
				"oncomplete",
				delegate(object o)
				{
					currDisplay.SetActive(false);
				}
			}));
			iTween.RotateBy(nextDisplayContainer, iTween.Hash(new object[]
			{
				"amount",
				new Vector3(0.5f, 0f, 0f),
				"time",
				0.5f,
				"name",
				"ROTATION_TWEEN"
			}));
			if (!string.IsNullOrEmpty(this.m_backgroundFlipSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_backgroundFlipSound));
			}
		}
		else
		{
			nextDisplayContainer.transform.localRotation = Quaternion.identity;
			currDisplay.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
			currDisplay.SetActive(false);
		}
		StorePackDef storePackDef = this.GetStorePackDef(id);
		this.GetCurrentDisplay().UpdatePackType(storePackDef);
		MeshRenderer currLogo = this.GetCurrentLogo();
		if (currLogo != null)
		{
			this.m_hasLogo = !string.IsNullOrEmpty(storePackDef.m_logoTextureName);
			currLogo.gameObject.SetActive(this.m_hasLogo);
			if (this.m_hasLogo)
			{
				AssetLoader.Get().LoadTexture(FileUtils.GameAssetPathToName(storePackDef.m_logoTextureName), delegate(string name, Object obj, object data)
				{
					Texture texture = obj as Texture;
					if (texture == null)
					{
						Debug.LogError(string.Format("Failed to load texture {0}!", name));
						return;
					}
					currLogo.material.mainTexture = texture;
				}, null, false);
				MeshRenderer glowLogo = this.GetCurrentGlowLogo();
				if (glowLogo != null)
				{
					AssetLoader.Get().LoadTexture(FileUtils.GameAssetPathToName(storePackDef.m_logoTextureGlowName), delegate(string name, Object obj, object data)
					{
						Texture texture = obj as Texture;
						if (texture == null)
						{
							Debug.LogError(string.Format("Failed to load texture {0}!", name));
							return;
						}
						glowLogo.material.mainTexture = texture;
					}, null, false);
				}
			}
		}
		this.AnimateBuyBar();
	}

	// Token: 0x0600353D RID: 13629 RVA: 0x00108BF0 File Offset: 0x00106DF0
	private void AnimateBuyBar()
	{
		Network.Bundle bundle;
		bool flag = StoreManager.Get().IsBoosterPreorderActive(this.m_selectedBoosterId, out bundle);
		GameObject gameObject = (!flag) ? this.m_packBuyPreorderContainer : this.m_packBuyContainer;
		if (this.m_selectedBoosterId == 0)
		{
			return;
		}
		iTween.Stop(gameObject);
		gameObject.transform.localRotation = Quaternion.identity;
		iTween.RotateBy(gameObject, iTween.Hash(new object[]
		{
			"amount",
			new Vector3(-1f, 0f, 0f),
			"time",
			this.m_backgroundFlipAnimTime,
			"delay",
			0.001f
		}));
	}

	// Token: 0x0600353E RID: 13630 RVA: 0x00108CAC File Offset: 0x00106EAC
	private void UpdateChinaKoreaInfoButton()
	{
		if (this.m_ChinaInfoButton == null)
		{
			return;
		}
		bool flag = StoreManager.Get().IsChinaCustomer() || StoreManager.Get().IsKoreanCustomer();
		bool active = flag && base.IsContentActive() && this.m_selectedBoosterId != 0;
		this.m_ChinaInfoButton.gameObject.SetActive(active);
	}

	// Token: 0x0600353F RID: 13631 RVA: 0x00108D1C File Offset: 0x00106F1C
	private void OnChinaKoreaInfoPressed(UIEvent e)
	{
		bool flag = StoreManager.Get().IsChinaCustomer();
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get((!flag) ? "GLUE_STORE_KOREAN_DISCLAIMER_HEADLINE" : "GLUE_STORE_CHINA_DISCLAIMER_HEADLINE");
		popupInfo.m_text = GameStrings.Get((!flag) ? "GLUE_STORE_KOREAN_DISCLAIMER_DETAILS" : "GLUE_STORE_CHINA_DISCLAIMER_DETAILS");
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x040020F5 RID: 8437
	private const string PREV_PLAYLIST_NAME = "StorePrevCurrentPlaylist";

	// Token: 0x040020F6 RID: 8438
	public StoreQuantityPrompt m_quantityPrompt;

	// Token: 0x040020F7 RID: 8439
	public GameObject m_packContainer;

	// Token: 0x040020F8 RID: 8440
	public GameObject m_packEmptyDisplay;

	// Token: 0x040020F9 RID: 8441
	public GeneralStorePacksContentDisplay m_packDisplay;

	// Token: 0x040020FA RID: 8442
	[CustomEditField(Sections = "Pack Buy Buttons")]
	public GameObject m_packBuyContainer;

	// Token: 0x040020FB RID: 8443
	[CustomEditField(Sections = "Pack Buy Buttons")]
	public MultiSliceElement m_packBuyButtonContainer;

	// Token: 0x040020FC RID: 8444
	[CustomEditField(Sections = "Pack Buy Buttons")]
	public GeneralStorePackBuyButton m_packBuyButtonPrefab;

	// Token: 0x040020FD RID: 8445
	[CustomEditField(Sections = "Pack Buy Buttons")]
	public MultiSliceElement m_packBuyFrameContainer;

	// Token: 0x040020FE RID: 8446
	[CustomEditField(Sections = "Pack Buy Buttons", ListTable = true)]
	public List<GeneralStorePacksContent.ToggleableButtonFrame> m_toggleableButtonFrames = new List<GeneralStorePacksContent.ToggleableButtonFrame>();

	// Token: 0x040020FF RID: 8447
	[CustomEditField(Sections = "Pack Buy Buttons/Preorder")]
	public GameObject m_packBuyPreorderContainer;

	// Token: 0x04002100 RID: 8448
	[CustomEditField(Sections = "Pack Buy Buttons/Preorder")]
	public GeneralStorePackBuyButton m_packBuyPreorderButton;

	// Token: 0x04002101 RID: 8449
	[CustomEditField(Sections = "Pack Buy Buttons/Preorder")]
	public UberText m_availableDateText;

	// Token: 0x04002102 RID: 8450
	[CustomEditField(Sections = "China Button")]
	public UIBButton m_ChinaInfoButton;

	// Token: 0x04002103 RID: 8451
	[CustomEditField(Sections = "Packs")]
	public int m_maxPackBuyButtons = 10;

	// Token: 0x04002104 RID: 8452
	[CustomEditField(Sections = "Packs")]
	public GeneralStorePacksContent.LogoAnimation m_logoAnimation;

	// Token: 0x04002105 RID: 8453
	[CustomEditField(Sections = "Animation")]
	public float m_packFlyOutAnimTime = 0.1f;

	// Token: 0x04002106 RID: 8454
	[CustomEditField(Sections = "Animation")]
	public float m_packFlyOutDelay = 0.005f;

	// Token: 0x04002107 RID: 8455
	[CustomEditField(Sections = "Animation")]
	public float m_packFlyInAnimTime = 0.2f;

	// Token: 0x04002108 RID: 8456
	[CustomEditField(Sections = "Animation")]
	public float m_packFlyInDelay = 0.01f;

	// Token: 0x04002109 RID: 8457
	[CustomEditField(Sections = "Animation")]
	public float m_shakeObjectDelayMultiplier = 0.7f;

	// Token: 0x0400210A RID: 8458
	[CustomEditField(Sections = "Animation")]
	public float m_backgroundFlipAnimTime = 0.5f;

	// Token: 0x0400210B RID: 8459
	[CustomEditField(Sections = "Animation")]
	public float m_maxPackFlyInXShake = 20f;

	// Token: 0x0400210C RID: 8460
	[CustomEditField(Sections = "Animation")]
	public float m_maxPackFlyOutXShake = 12f;

	// Token: 0x0400210D RID: 8461
	[CustomEditField(Sections = "Animation")]
	public float m_packFlyShakeTime = 2f;

	// Token: 0x0400210E RID: 8462
	[CustomEditField(Sections = "Animation")]
	public float m_backgroundFlipShake = 20f;

	// Token: 0x0400210F RID: 8463
	[CustomEditField(Sections = "Animation")]
	public float m_backgroundFlipShakeDelay;

	// Token: 0x04002110 RID: 8464
	[CustomEditField(Sections = "Animation")]
	public float m_PackYDegreeVariationMag = 2f;

	// Token: 0x04002111 RID: 8465
	[CustomEditField(Sections = "Animation/Appear")]
	public GameObject m_logoAnimationStartBone;

	// Token: 0x04002112 RID: 8466
	[CustomEditField(Sections = "Animation/Appear")]
	public GameObject m_logoAnimationEndBone;

	// Token: 0x04002113 RID: 8467
	[CustomEditField(Sections = "Animation/Appear")]
	public MeshRenderer m_logoMesh;

	// Token: 0x04002114 RID: 8468
	[CustomEditField(Sections = "Animation/Appear")]
	public MeshRenderer m_logoGlowMesh;

	// Token: 0x04002115 RID: 8469
	[CustomEditField(Sections = "Animation/Appear")]
	public Vector3 m_punchAmount;

	// Token: 0x04002116 RID: 8470
	[CustomEditField(Sections = "Animation/Appear")]
	public float m_logoHoldTime = 1f;

	// Token: 0x04002117 RID: 8471
	[CustomEditField(Sections = "Animation/Appear")]
	public float m_logoDisplayPunchTime = 0.5f;

	// Token: 0x04002118 RID: 8472
	[CustomEditField(Sections = "Animation/Appear")]
	public float m_logoIntroTime = 0.25f;

	// Token: 0x04002119 RID: 8473
	[CustomEditField(Sections = "Animation/Appear")]
	public float m_logoOutroTime = 0.25f;

	// Token: 0x0400211A RID: 8474
	[CustomEditField(Sections = "Animation/Appear")]
	public Vector3 m_logoAppearOffset;

	// Token: 0x0400211B RID: 8475
	[CustomEditField(Sections = "Animation/Preorder")]
	public GeneralStoreRewardsCardBack m_preorderCardBackReward;

	// Token: 0x0400211C RID: 8476
	[CustomEditField(Sections = "Sounds & Music", T = EditType.SOUND_PREFAB)]
	public string m_backgroundFlipSound;

	// Token: 0x0400211D RID: 8477
	public static readonly bool REQUIRE_REAL_MONEY_BUNDLE_OPTION = true;

	// Token: 0x0400211E RID: 8478
	private static readonly int MAX_QUANTITY_BOUGHT_WITH_GOLD = 50;

	// Token: 0x0400211F RID: 8479
	private int m_selectedBoosterId;

	// Token: 0x04002120 RID: 8480
	private List<GeneralStorePackBuyButton> m_packBuyButtons = new List<GeneralStorePackBuyButton>();

	// Token: 0x04002121 RID: 8481
	private int m_currentGoldPackQuantity = 1;

	// Token: 0x04002122 RID: 8482
	private int m_visiblePackCount;

	// Token: 0x04002123 RID: 8483
	private int m_lastBundleIndex;

	// Token: 0x04002124 RID: 8484
	private int m_currentDisplay = -1;

	// Token: 0x04002125 RID: 8485
	private Map<int, StorePackDef> m_storePackDefs = new Map<int, StorePackDef>();

	// Token: 0x04002126 RID: 8486
	private GeneralStorePacksContentDisplay m_packDisplay1;

	// Token: 0x04002127 RID: 8487
	private GeneralStorePacksContentDisplay m_packDisplay2;

	// Token: 0x04002128 RID: 8488
	private MeshRenderer m_logoMesh1;

	// Token: 0x04002129 RID: 8489
	private MeshRenderer m_logoMesh2;

	// Token: 0x0400212A RID: 8490
	private MeshRenderer m_logoGlowMesh1;

	// Token: 0x0400212B RID: 8491
	private MeshRenderer m_logoGlowMesh2;

	// Token: 0x0400212C RID: 8492
	private Coroutine m_logoAnimCoroutine;

	// Token: 0x0400212D RID: 8493
	private Coroutine m_packAnimCoroutine;

	// Token: 0x0400212E RID: 8494
	private Coroutine m_preorderCardBackAnimCoroutine;

	// Token: 0x0400212F RID: 8495
	private Vector3 m_savedLocalPosition;

	// Token: 0x04002130 RID: 8496
	private Vector3 m_availableDateTextOrigScale;

	// Token: 0x04002131 RID: 8497
	private bool m_animatingLogo;

	// Token: 0x04002132 RID: 8498
	private bool m_animatingPacks;

	// Token: 0x04002133 RID: 8499
	private bool m_hasLogo;

	// Token: 0x02000ADB RID: 2779
	[Serializable]
	public class ToggleableButtonFrame
	{
		// Token: 0x04004760 RID: 18272
		public GameObject m_Middle;

		// Token: 0x04004761 RID: 18273
		public GameObject m_IBar;
	}

	// Token: 0x02000ADC RID: 2780
	public enum LogoAnimation
	{
		// Token: 0x04004763 RID: 18275
		None,
		// Token: 0x04004764 RID: 18276
		Slam,
		// Token: 0x04004765 RID: 18277
		Fade
	}
}
