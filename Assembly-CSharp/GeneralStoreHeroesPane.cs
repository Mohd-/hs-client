using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000AD4 RID: 2772
[CustomEditClass]
public class GeneralStoreHeroesPane : GeneralStorePane
{
	// Token: 0x170008ED RID: 2285
	// (get) Token: 0x06005F99 RID: 24473 RVA: 0x001C9C35 File Offset: 0x001C7E35
	// (set) Token: 0x06005F9A RID: 24474 RVA: 0x001C9C3D File Offset: 0x001C7E3D
	[CustomEditField(Sections = "Layout")]
	public Vector3 UnpurchasedHeroButtonSpacing
	{
		get
		{
			return this.m_unpurchasedHeroButtonSpacing;
		}
		set
		{
			this.m_unpurchasedHeroButtonSpacing = value;
			this.PositionAllHeroButtons();
		}
	}

	// Token: 0x170008EE RID: 2286
	// (get) Token: 0x06005F9B RID: 24475 RVA: 0x001C9C4C File Offset: 0x001C7E4C
	// (set) Token: 0x06005F9C RID: 24476 RVA: 0x001C9C54 File Offset: 0x001C7E54
	[CustomEditField(Sections = "Layout")]
	public Vector3 PurchasedHeroButtonSpacing
	{
		get
		{
			return this.m_purchasedHeroButtonSpacing;
		}
		set
		{
			this.m_purchasedHeroButtonSpacing = value;
			this.PositionAllHeroButtons();
		}
	}

	// Token: 0x06005F9D RID: 24477 RVA: 0x001C9C64 File Offset: 0x001C7E64
	private void Awake()
	{
		this.m_heroesContent = (this.m_parentContent as GeneralStoreHeroesContent);
		this.PopulateHeroes();
		this.m_purchaseAnimationBlocker.SetActive(false);
		StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new StoreManager.SuccessfulPurchaseAckCallback(this.OnItemPurchased));
		CheatMgr.Get().RegisterCheatHandler("herobuy", new CheatMgr.ProcessCheatCallback(this.OnHeroPurchased_cheat), null, null, null);
	}

	// Token: 0x06005F9E RID: 24478 RVA: 0x001C9CCC File Offset: 0x001C7ECC
	private void OnDestroy()
	{
		CheatMgr.Get().UnregisterCheatHandler("herobuy", new CheatMgr.ProcessCheatCallback(this.OnHeroPurchased_cheat));
		StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new StoreManager.SuccessfulPurchaseAckCallback(this.OnItemPurchased));
	}

	// Token: 0x06005F9F RID: 24479 RVA: 0x001C9D0B File Offset: 0x001C7F0B
	public override void PrePaneSwappedIn()
	{
		this.SetupInitialSelectedHero();
	}

	// Token: 0x06005FA0 RID: 24480 RVA: 0x001C9D13 File Offset: 0x001C7F13
	public void RefreshHeroAvailability()
	{
	}

	// Token: 0x06005FA1 RID: 24481 RVA: 0x001C9D18 File Offset: 0x001C7F18
	private void PopulateHeroes()
	{
		SpecialEventManager specialEventManager = SpecialEventManager.Get();
		List<HeroDbfRecord> records = GameDbf.Hero.GetRecords();
		foreach (HeroDbfRecord heroDbfRecord in records)
		{
			Network.Bundle bundle = null;
			if (StoreManager.Get().GetHeroBundleByCardMiniGuid(heroDbfRecord.CardId, out bundle))
			{
				if (specialEventManager.IsEventActive(bundle.ProductEvent, false))
				{
					GeneralStoreHeroesSelectorButton generalStoreHeroesSelectorButton = this.CreateNewHeroButton(heroDbfRecord, bundle);
					generalStoreHeroesSelectorButton.SetSortOrder(heroDbfRecord.StoreSortOrder);
				}
			}
		}
		this.PositionAllHeroButtons();
	}

	// Token: 0x06005FA2 RID: 24482 RVA: 0x001C9DC8 File Offset: 0x001C7FC8
	private GeneralStoreHeroesSelectorButton CreateNewHeroButton(HeroDbfRecord hero, Network.Bundle heroBundle)
	{
		bool flag = !StoreManager.Get().CanBuyBundle(heroBundle);
		if (flag)
		{
			return this.CreatePurchasedHeroButton(hero, heroBundle);
		}
		return this.CreateUnpurchasedHeroButton(hero, heroBundle);
	}

	// Token: 0x06005FA3 RID: 24483 RVA: 0x001C9DFC File Offset: 0x001C7FFC
	private GeneralStoreHeroesSelectorButton CreateUnpurchasedHeroButton(HeroDbfRecord hero, Network.Bundle heroBundle)
	{
		GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(this.m_heroUnpurchasedFrame), true, false);
		GeneralStoreHeroesSelectorButton component = gameObject.GetComponent<GeneralStoreHeroesSelectorButton>();
		if (component == null)
		{
			Debug.LogError("Prefab does not contain GeneralStoreHeroesSelectorButton component.");
			Object.Destroy(gameObject);
			return null;
		}
		GameUtils.SetParent(component, this.m_paneContainer, true);
		SceneUtils.SetLayer(component, this.m_paneContainer.layer);
		this.m_unpurchasedHeroesButtons.Add(component);
		this.SetupHeroButton(hero, component);
		return component;
	}

	// Token: 0x06005FA4 RID: 24484 RVA: 0x001C9E7C File Offset: 0x001C807C
	public GeneralStoreHeroesSelectorButton CreatePurchasedHeroButton(HeroDbfRecord hero, Network.Bundle heroBundle)
	{
		GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(this.m_heroPurchasedFrame), true, false);
		GeneralStoreHeroesSelectorButton component = gameObject.GetComponent<GeneralStoreHeroesSelectorButton>();
		if (component == null)
		{
			Debug.LogError("Prefab does not contain GeneralStoreHeroesSelectorButton component.");
			Object.Destroy(gameObject);
			return null;
		}
		GameUtils.SetParent(component, this.m_purchasedButtonContainer, true);
		SceneUtils.SetLayer(component, this.m_purchasedButtonContainer.layer);
		this.m_purchasedHeroesButtons.Add(component);
		this.SetupHeroButton(hero, component);
		return component;
	}

	// Token: 0x06005FA5 RID: 24485 RVA: 0x001C9EFC File Offset: 0x001C80FC
	private void SetupHeroButton(HeroDbfRecord hero, GeneralStoreHeroesSelectorButton heroButton)
	{
		string cardId2 = hero.CardId;
		heroButton.SetHeroDbfRecord(hero);
		heroButton.SetPurchased(false);
		heroButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.SelectHero(heroButton);
		});
		DefLoader.Get().LoadFullDef(cardId2, delegate(string cardId, FullDef fullDef, object data)
		{
			heroButton.UpdatePortrait(fullDef.GetEntityDef(), fullDef.GetCardDef());
			heroButton.UpdateName(fullDef.GetEntityDef().GetName());
		});
	}

	// Token: 0x06005FA6 RID: 24486 RVA: 0x001C9F6C File Offset: 0x001C816C
	private void UpdatePurchasedSectionLayout()
	{
		if (this.m_purchasedHeroesButtons.Count == 0)
		{
			this.m_purchasedButtonContainer.SetActive(false);
			this.m_purchasedSection.gameObject.SetActive(false);
			return;
		}
		this.m_purchasedButtonContainer.SetActive(true);
		this.m_purchasedSection.gameObject.SetActive(true);
		if (this.m_purchasedSectionMidMeshes.Count < this.m_purchasedHeroesButtons.Count)
		{
			int num = this.m_purchasedHeroesButtons.Count - this.m_purchasedSectionMidMeshes.Count;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = (GameObject)GameUtils.Instantiate(this.m_purchasedSectionMidTemplate, this.m_purchasedSection.gameObject, true);
				gameObject.SetActive(true);
				this.m_purchasedSectionMidMeshes.Add(gameObject);
			}
		}
		this.m_purchasedSection.ClearSlices();
		this.m_purchasedSection.AddSlice(this.m_purchasedSectionTop);
		foreach (GameObject obj in this.m_purchasedSectionMidMeshes)
		{
			this.m_purchasedSection.AddSlice(obj);
		}
		this.m_purchasedSection.AddSlice(this.m_purchasedSectionBottom);
		this.m_purchasedSection.UpdateSlices();
	}

	// Token: 0x06005FA7 RID: 24487 RVA: 0x001CA0C8 File Offset: 0x001C82C8
	private void SelectHero(GeneralStoreHeroesSelectorButton button)
	{
		foreach (GeneralStoreHeroesSelectorButton generalStoreHeroesSelectorButton in this.m_unpurchasedHeroesButtons)
		{
			generalStoreHeroesSelectorButton.Unselect();
		}
		foreach (GeneralStoreHeroesSelectorButton generalStoreHeroesSelectorButton2 in this.m_purchasedHeroesButtons)
		{
			generalStoreHeroesSelectorButton2.Unselect();
		}
		button.Select();
		Options.Get().SetInt(Option.LAST_SELECTED_STORE_HERO_ID, button.GetHeroDbId());
		this.m_heroesContent.SelectHero(button.GetHeroDbfRecord(), true);
		if (!string.IsNullOrEmpty(this.m_heroSelectionSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_heroSelectionSound));
		}
	}

	// Token: 0x06005FA8 RID: 24488 RVA: 0x001CA1B8 File Offset: 0x001C83B8
	private void SetupInitialSelectedHero()
	{
		if (this.m_initializeFirstHero)
		{
			return;
		}
		this.m_initializeFirstHero = true;
		int @int = Options.Get().GetInt(Option.LAST_SELECTED_STORE_HERO_ID, -1);
		if (@int == -1)
		{
			return;
		}
		List<GeneralStoreHeroesSelectorButton> list = new List<GeneralStoreHeroesSelectorButton>();
		list.AddRange(this.m_unpurchasedHeroesButtons);
		list.AddRange(this.m_purchasedHeroesButtons);
		foreach (GeneralStoreHeroesSelectorButton generalStoreHeroesSelectorButton in list)
		{
			if (generalStoreHeroesSelectorButton.GetHeroDbId() == @int)
			{
				this.m_heroesContent.SelectHero(generalStoreHeroesSelectorButton.GetHeroDbfRecord(), false);
				generalStoreHeroesSelectorButton.Select();
				break;
			}
		}
	}

	// Token: 0x06005FA9 RID: 24489 RVA: 0x001CA278 File Offset: 0x001C8478
	private void PositionAllHeroButtons()
	{
		this.PositionUnpurchasedHeroButtons();
		this.PositionPurchasedHeroButtons(true);
	}

	// Token: 0x06005FAA RID: 24490 RVA: 0x001CA288 File Offset: 0x001C8488
	private void PositionUnpurchasedHeroButtons()
	{
		this.m_unpurchasedHeroesButtons.Sort(delegate(GeneralStoreHeroesSelectorButton lhs, GeneralStoreHeroesSelectorButton rhs)
		{
			int sortOrder = lhs.GetSortOrder();
			int sortOrder2 = rhs.GetSortOrder();
			if (sortOrder < sortOrder2)
			{
				return -1;
			}
			if (sortOrder > sortOrder2)
			{
				return 1;
			}
			return 0;
		});
		for (int i = 0; i < this.m_unpurchasedHeroesButtons.Count; i++)
		{
			this.m_unpurchasedHeroesButtons[i].transform.localPosition = this.m_unpurchasedHeroButtonSpacing * (float)i;
		}
	}

	// Token: 0x06005FAB RID: 24491 RVA: 0x001CA2FC File Offset: 0x001C84FC
	private void PositionPurchasedHeroButtons(bool sortAndSetSectionPos = true)
	{
		if (sortAndSetSectionPos)
		{
			this.m_purchasedHeroesButtons.Sort(delegate(GeneralStoreHeroesSelectorButton lhs, GeneralStoreHeroesSelectorButton rhs)
			{
				int sortOrder = lhs.GetSortOrder();
				int sortOrder2 = rhs.GetSortOrder();
				if (sortOrder < sortOrder2)
				{
					return -1;
				}
				if (sortOrder > sortOrder2)
				{
					return 1;
				}
				return 0;
			});
			this.m_purchasedSection.transform.localPosition = this.m_unpurchasedHeroButtonSpacing * (float)(this.m_unpurchasedHeroesButtons.Count - 1) + this.m_purchasedSectionOffset;
		}
		for (int i = 0; i < this.m_purchasedHeroesButtons.Count; i++)
		{
			this.m_purchasedHeroesButtons[i].transform.localPosition = this.m_purchasedHeroButtonSpacing * (float)i;
		}
		this.UpdatePurchasedSectionLayout();
	}

	// Token: 0x06005FAC RID: 24492 RVA: 0x001CA3B0 File Offset: 0x001C85B0
	private IEnumerator AnimateShowPurchase(int btnIndex)
	{
		this.m_purchaseAnimationBlocker.SetActive(true);
		this.m_scrollUpdate.Pause(true);
		if (GeneralStore.Get().GetMode() != GeneralStoreMode.HEROES)
		{
			GeneralStore.Get().SetMode(GeneralStoreMode.HEROES);
			yield return new WaitForSeconds(1f);
		}
		GeneralStoreHeroesSelectorButton removeBtn = this.m_unpurchasedHeroesButtons[btnIndex];
		float scrollPos = (float)btnIndex / (float)(this.m_unpurchasedHeroesButtons.Count + this.m_purchasedHeroesButtons.Count - 1);
		this.m_scrollUpdate.SetScroll(scrollPos, iTween.EaseType.easeInOutCirc, 0.2f, false, true);
		yield return new WaitForSeconds(0.21f);
		GameObject animateBtnObj = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(this.m_heroAnimationFrame), true, false);
		GeneralStoreHeroesSelectorButton animateBtn = animateBtnObj.GetComponent<GeneralStoreHeroesSelectorButton>();
		SceneUtils.SetLayer(animateBtn, GameLayer.PerspectiveUI);
		animateBtn.transform.position = removeBtn.transform.position;
		animateBtn.UpdatePortrait(removeBtn);
		animateBtn.UpdateName(removeBtn);
		removeBtn.gameObject.SetActive(false);
		PlayMakerFSM animation = animateBtn.GetComponent<PlayMakerFSM>();
		FsmVector3 startPos = animation.FsmVariables.FindFsmVector3("PopStartPos");
		FsmVector3 midPos = animation.FsmVariables.FindFsmVector3("PopMidPos");
		FsmVector3 endPos = animation.FsmVariables.FindFsmVector3("PopEndPos");
		startPos.Value = removeBtn.transform.position;
		midPos.Value = removeBtn.transform.position + this.m_purchaseAnimationMidPointWorldOffset;
		endPos.Value = this.m_purchaseAnimationEndBone.transform.position;
		Camera cam = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		if (cam != null)
		{
			FsmGameObject camShake = animation.FsmVariables.FindFsmGameObject("CameraObjectShake");
			camShake.Value = cam.gameObject;
		}
		FsmString animName = animation.FsmVariables.FindFsmString("PopOutAnimationName");
		animName.Value = this.m_purchaseAnimationName;
		animation.SendEvent("PopOut");
		yield return new WaitForSeconds(0.5f);
		this.m_heroesContent.PlayCurrentHeroPurchaseEmote();
		yield return null;
		FsmBool animComplete = animation.FsmVariables.FindFsmBool("AnimationComplete");
		while (!animComplete.Value)
		{
			yield return null;
		}
		GeneralStoreHeroesSelectorButton newBtn = this.CreatePurchasedHeroButton(removeBtn.GetHeroDbfRecord(), null);
		newBtn.Select();
		this.m_unpurchasedHeroesButtons.Remove(removeBtn);
		this.PositionPurchasedHeroButtons(false);
		yield return new WaitForSeconds(0.25f);
		while (!UniversalInputManager.Get().GetMouseButtonDown(0))
		{
			yield return null;
		}
		animation.SendEvent("EchoHero");
		yield return null;
		animComplete = animation.FsmVariables.FindFsmBool("AnimationComplete");
		while (!animComplete.Value)
		{
			yield return null;
		}
		for (int i = this.m_currentPurchaseRemovalIdx; i < this.m_unpurchasedHeroesButtons.Count; i++)
		{
			GeneralStoreHeroesSelectorButton btn = this.m_unpurchasedHeroesButtons[i];
			iTween.MoveTo(btn.gameObject, iTween.Hash(new object[]
			{
				"position",
				this.m_unpurchasedHeroButtonSpacing * (float)i,
				"islocal",
				true,
				"easetype",
				iTween.EaseType.easeInOutCirc,
				"time",
				0.25f
			}));
		}
		iTween.MoveTo(this.m_purchasedSection.gameObject, iTween.Hash(new object[]
		{
			"position",
			this.m_unpurchasedHeroButtonSpacing * (float)(this.m_unpurchasedHeroesButtons.Count - 1) + this.m_purchasedSectionOffset,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeInOutCirc,
			"time",
			0.25f
		}));
		if (!string.IsNullOrEmpty(this.m_buttonsSlideUpSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_buttonsSlideUpSound));
		}
		yield return new WaitForSeconds(0.25f);
		Object.Destroy(removeBtn.gameObject);
		Object.Destroy(animateBtnObj);
		animateBtnObj = null;
		this.m_scrollUpdate.Pause(false);
		this.m_purchaseAnimationBlocker.SetActive(false);
		yield break;
	}

	// Token: 0x06005FAD RID: 24493 RVA: 0x001CA3DC File Offset: 0x001C85DC
	private void OnItemPurchased(Network.Bundle bundle, PaymentMethod purchaseMethod, object userData)
	{
		if (bundle == null || bundle.Items == null)
		{
			return;
		}
		foreach (Network.BundleItem bundleItem in bundle.Items)
		{
			if (bundleItem != null && bundleItem.Product == 6)
			{
				int productData = bundleItem.ProductData;
				this.OnHeroPurchased(productData);
				break;
			}
		}
	}

	// Token: 0x06005FAE RID: 24494 RVA: 0x001CA468 File Offset: 0x001C8668
	private void OnHeroPurchased(int heroCardDbId)
	{
		int num = this.m_unpurchasedHeroesButtons.FindIndex((GeneralStoreHeroesSelectorButton e) => e.GetHeroCardDbId() == heroCardDbId);
		if (num == -1)
		{
			Debug.LogError(string.Format("Hero Card DB ID {0} does not exist in button list.", heroCardDbId));
			return;
		}
		this.RunHeroPurchaseAnimation(num);
	}

	// Token: 0x06005FAF RID: 24495 RVA: 0x001CA4C3 File Offset: 0x001C86C3
	private void RunHeroPurchaseAnimation(int btnIndex)
	{
		this.m_currentPurchaseRemovalIdx = btnIndex;
		base.StartCoroutine(this.AnimateShowPurchase(btnIndex));
	}

	// Token: 0x06005FB0 RID: 24496 RVA: 0x001CA4DC File Offset: 0x001C86DC
	private bool OnHeroPurchased_cheat(string func, string[] args, string rawArgs)
	{
		if (args.Length == 0)
		{
			return true;
		}
		int num = -1;
		if (int.TryParse(args[0], ref num) && num >= 0 && num < this.m_unpurchasedHeroesButtons.Count)
		{
			this.RunHeroPurchaseAnimation(num);
		}
		return true;
	}

	// Token: 0x040046FF RID: 18175
	[CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
	public string m_heroUnpurchasedFrame;

	// Token: 0x04004700 RID: 18176
	[CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
	public string m_heroPurchasedFrame;

	// Token: 0x04004701 RID: 18177
	[CustomEditField(Sections = "Prefabs", T = EditType.GAME_OBJECT)]
	public string m_heroAnimationFrame;

	// Token: 0x04004702 RID: 18178
	[SerializeField]
	private Vector3 m_unpurchasedHeroButtonSpacing = new Vector3(0f, 0f, 0.285f);

	// Token: 0x04004703 RID: 18179
	[SerializeField]
	private Vector3 m_purchasedHeroButtonSpacing = new Vector3(0f, 0f, 0.092f);

	// Token: 0x04004704 RID: 18180
	[CustomEditField(Sections = "Layout")]
	public float m_unpurchasedHeroButtonHeight = 0.0275f;

	// Token: 0x04004705 RID: 18181
	[CustomEditField(Sections = "Layout")]
	public float m_purchasedHeroButtonHeight;

	// Token: 0x04004706 RID: 18182
	[CustomEditField(Sections = "Layout")]
	public float m_purchasedHeroButtonHeightPadding = 0.01f;

	// Token: 0x04004707 RID: 18183
	[CustomEditField(Sections = "Layout")]
	public float m_maxPurchasedHeightAdd;

	// Token: 0x04004708 RID: 18184
	[CustomEditField(Sections = "Layout/Purchased Section")]
	public GameObject m_purchasedSectionTop;

	// Token: 0x04004709 RID: 18185
	[CustomEditField(Sections = "Layout/Purchased Section")]
	public GameObject m_purchasedSectionBottom;

	// Token: 0x0400470A RID: 18186
	[CustomEditField(Sections = "Layout/Purchased Section")]
	public GameObject m_purchasedSectionMidTemplate;

	// Token: 0x0400470B RID: 18187
	[CustomEditField(Sections = "Layout/Purchased Section")]
	public MultiSliceElement m_purchasedSection;

	// Token: 0x0400470C RID: 18188
	[CustomEditField(Sections = "Layout/Purchased Section")]
	public GameObject m_purchasedButtonContainer;

	// Token: 0x0400470D RID: 18189
	[CustomEditField(Sections = "Layout/Purchased Section")]
	public Vector3 m_purchasedSectionOffset = new Vector3(0f, 0f, 0.145f);

	// Token: 0x0400470E RID: 18190
	[CustomEditField(Sections = "Scroll")]
	public UIBScrollable m_scrollUpdate;

	// Token: 0x0400470F RID: 18191
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public string m_heroSelectionSound;

	// Token: 0x04004710 RID: 18192
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public string m_buttonsSlideUpSound;

	// Token: 0x04004711 RID: 18193
	[CustomEditField(Sections = "Purchase Flow")]
	public GameObject m_purchaseAnimationBlocker;

	// Token: 0x04004712 RID: 18194
	[CustomEditField(Sections = "Animations")]
	public GameObject m_purchaseAnimationEndBone;

	// Token: 0x04004713 RID: 18195
	[CustomEditField(Sections = "Animations")]
	public Vector3 m_purchaseAnimationMidPointWorldOffset = new Vector3(0f, 0f, -7.5f);

	// Token: 0x04004714 RID: 18196
	[CustomEditField(Sections = "Animations")]
	public string m_purchaseAnimationName = "HeroSkin_HeroHolderPopOut";

	// Token: 0x04004715 RID: 18197
	private List<GeneralStoreHeroesSelectorButton> m_unpurchasedHeroesButtons = new List<GeneralStoreHeroesSelectorButton>();

	// Token: 0x04004716 RID: 18198
	private List<GeneralStoreHeroesSelectorButton> m_purchasedHeroesButtons = new List<GeneralStoreHeroesSelectorButton>();

	// Token: 0x04004717 RID: 18199
	private GeneralStoreHeroesContent m_heroesContent;

	// Token: 0x04004718 RID: 18200
	private bool m_initializeFirstHero;

	// Token: 0x04004719 RID: 18201
	private List<GameObject> m_purchasedSectionMidMeshes = new List<GameObject>();

	// Token: 0x0400471A RID: 18202
	private int m_currentPurchaseRemovalIdx;
}
