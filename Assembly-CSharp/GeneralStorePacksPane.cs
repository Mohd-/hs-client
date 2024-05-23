using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AED RID: 2797
[CustomEditClass]
public class GeneralStorePacksPane : GeneralStorePane
{
	// Token: 0x170008F9 RID: 2297
	// (get) Token: 0x06006045 RID: 24645 RVA: 0x001CD038 File Offset: 0x001CB238
	// (set) Token: 0x06006046 RID: 24646 RVA: 0x001CD040 File Offset: 0x001CB240
	[CustomEditField(Sections = "Layout")]
	public Vector3 PackButtonSpacing
	{
		get
		{
			return this.m_packButtonSpacing;
		}
		set
		{
			this.m_packButtonSpacing = value;
			this.UpdatePackButtonPositions();
		}
	}

	// Token: 0x06006047 RID: 24647 RVA: 0x001CD04F File Offset: 0x001CB24F
	private void Awake()
	{
		this.m_packsContent = (this.m_parentContent as GeneralStorePacksContent);
		if (this.m_packsContent == null)
		{
			Debug.LogError("m_packsContent is not the correct type: GeneralStorePacksContent");
			return;
		}
	}

	// Token: 0x06006048 RID: 24648 RVA: 0x001CD07E File Offset: 0x001CB27E
	public override void StoreShown(bool isCurrent)
	{
		if (!this.m_paneInitialized)
		{
			this.m_paneInitialized = true;
			this.SetupPackButtons();
			this.SetupInitialSelectedPack();
		}
		this.UpdatePackButtonPositions();
		this.UpdatePackButtonRecommendedIndicators();
	}

	// Token: 0x06006049 RID: 24649 RVA: 0x001CD0AA File Offset: 0x001CB2AA
	public override void OnPurchaseFinished()
	{
		this.UpdatePackButtonRecommendedIndicators();
	}

	// Token: 0x0600604A RID: 24650 RVA: 0x001CD0B4 File Offset: 0x001CB2B4
	private void OnPackSelectorButtonClicked(GeneralStorePackSelectorButton btn, int boosterId)
	{
		if (!this.m_parentContent.IsContentActive())
		{
			return;
		}
		this.m_packsContent.SetBoosterId(boosterId, false);
		foreach (GeneralStorePackSelectorButton generalStorePackSelectorButton in this.m_packButtons)
		{
			generalStorePackSelectorButton.Unselect();
		}
		btn.Select();
		Options.Get().SetInt(Option.LAST_SELECTED_STORE_BOOSTER_ID, (int)btn.GetBoosterId());
		if (!string.IsNullOrEmpty(this.m_boosterSelectionSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_boosterSelectionSound));
		}
	}

	// Token: 0x0600604B RID: 24651 RVA: 0x001CD168 File Offset: 0x001CB368
	private void SetupPackButtons()
	{
		Map<int, StorePackDef> storePackDefs = this.m_packsContent.GetStorePackDefs();
		int boosterId = this.m_packsContent.GetBoosterId();
		foreach (KeyValuePair<int, StorePackDef> keyValuePair in storePackDefs)
		{
			int id = keyValuePair.Key;
			if (StoreManager.Get().GetAllBundlesForProduct(1, GeneralStorePacksContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, id, 0).Count != 0)
			{
				StorePackDef value = keyValuePair.Value;
				GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(value.m_buttonPrefab), false, false);
				GameUtils.SetParent(gameObject, this.m_paneContainer, true);
				SceneUtils.SetLayer(gameObject, this.m_paneContainer.layer);
				GeneralStorePackSelectorButton newPackButton = gameObject.GetComponent<GeneralStorePackSelectorButton>();
				newPackButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
				{
					this.OnPackSelectorButtonClicked(newPackButton, id);
				});
				newPackButton.SetBoosterId((BoosterDbId)id);
				if (id == boosterId)
				{
					newPackButton.Select();
				}
				this.m_packButtons.Add(newPackButton);
			}
		}
		this.UpdatePackButtonPositions();
	}

	// Token: 0x0600604C RID: 24652 RVA: 0x001CD2D0 File Offset: 0x001CB4D0
	private void SortPackButtons()
	{
		this.m_packButtons.Sort(delegate(GeneralStorePackSelectorButton lhs, GeneralStorePackSelectorButton rhs)
		{
			bool flag = lhs.IsRecommendedForNewPlayer();
			bool flag2 = rhs.IsRecommendedForNewPlayer();
			if (flag != flag2)
			{
				return (!flag) ? 1 : -1;
			}
			bool flag3 = lhs.IsPreorder();
			bool flag4 = rhs.IsPreorder();
			if (flag3 != flag4)
			{
				return (!flag3) ? 1 : -1;
			}
			bool flag5 = lhs.IsLatestExpansion();
			bool flag6 = rhs.IsLatestExpansion();
			if (flag5 != flag6)
			{
				return (!flag5) ? 1 : -1;
			}
			BoosterDbfRecord booster = lhs.GetBooster();
			BoosterDbfRecord booster2 = rhs.GetBooster();
			bool flag7 = booster != null && booster.ID == 1;
			bool flag8 = booster2 != null && booster2.ID == 1;
			if (flag7 != flag8)
			{
				return (!flag7) ? 1 : -1;
			}
			int num = (booster != null) ? booster.SortOrder : 0;
			int num2 = (booster2 != null) ? booster2.SortOrder : 0;
			return Mathf.Clamp(num2 - num, -1, 1);
		});
	}

	// Token: 0x0600604D RID: 24653 RVA: 0x001CD308 File Offset: 0x001CB508
	private void UpdatePackButtonPositions()
	{
		this.SortPackButtons();
		GeneralStorePackSelectorButton[] array = this.m_packButtons.ToArray();
		int i = 0;
		int num = 0;
		while (i < array.Length)
		{
			GeneralStorePackSelectorButton generalStorePackSelectorButton = array[i];
			bool flag = generalStorePackSelectorButton.IsPurchasable();
			generalStorePackSelectorButton.gameObject.SetActive(flag);
			if (flag)
			{
				generalStorePackSelectorButton.transform.localPosition = this.m_packButtonSpacing * (float)num++;
			}
			i++;
		}
	}

	// Token: 0x0600604E RID: 24654 RVA: 0x001CD37C File Offset: 0x001CB57C
	private void UpdatePackButtonRecommendedIndicators()
	{
		GeneralStorePackSelectorButton[] array = this.m_packButtons.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateRibbonIndicator();
		}
	}

	// Token: 0x0600604F RID: 24655 RVA: 0x001CD3B4 File Offset: 0x001CB5B4
	private bool ShouldResetPackSelection()
	{
		List<Network.Bundle> allBundlesForProduct = StoreManager.Get().GetAllBundlesForProduct(1, GeneralStorePacksContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, 0, 0);
		string @string = Options.Get().GetString(Option.SEEN_PACK_PRODUCT_LIST, string.Empty);
		List<string> list = new List<string>(@string.Split(new char[]
		{
			':'
		}));
		bool result = false;
		foreach (Network.Bundle bundle in allBundlesForProduct)
		{
			if (!list.Contains(bundle.ProductID))
			{
				list.Add(bundle.ProductID);
				result = true;
			}
		}
		Options.Get().SetString(Option.SEEN_PACK_PRODUCT_LIST, string.Join(":", list.ToArray()));
		return result;
	}

	// Token: 0x06006050 RID: 24656 RVA: 0x001CD484 File Offset: 0x001CB684
	private void SetupInitialSelectedPack()
	{
		BoosterDbId boosterDbId = BoosterDbId.INVALID;
		if (this.ShouldResetPackSelection())
		{
			Options.Get().SetInt(Option.LAST_SELECTED_STORE_BOOSTER_ID, 0);
		}
		else
		{
			boosterDbId = (BoosterDbId)Options.Get().GetInt(Option.LAST_SELECTED_STORE_BOOSTER_ID, 0);
		}
		foreach (GeneralStorePackSelectorButton generalStorePackSelectorButton in this.m_packButtons)
		{
			if (generalStorePackSelectorButton.GetBoosterId() == boosterDbId)
			{
				this.m_packsContent.SetBoosterId((int)boosterDbId, true);
				generalStorePackSelectorButton.Select();
				break;
			}
		}
	}

	// Token: 0x040047B6 RID: 18358
	[SerializeField]
	private Vector3 m_packButtonSpacing;

	// Token: 0x040047B7 RID: 18359
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public string m_boosterSelectionSound;

	// Token: 0x040047B8 RID: 18360
	private List<GeneralStorePackSelectorButton> m_packButtons = new List<GeneralStorePackSelectorButton>();

	// Token: 0x040047B9 RID: 18361
	private GeneralStorePacksContent m_packsContent;

	// Token: 0x040047BA RID: 18362
	private bool m_paneInitialized;
}
