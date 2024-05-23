using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000ACC RID: 2764
[CustomEditClass]
public class GeneralStoreAdventurePane : GeneralStorePane
{
	// Token: 0x170008EC RID: 2284
	// (get) Token: 0x06005F53 RID: 24403 RVA: 0x001C8258 File Offset: 0x001C6458
	// (set) Token: 0x06005F54 RID: 24404 RVA: 0x001C8260 File Offset: 0x001C6460
	[CustomEditField(Sections = "Layout")]
	public Vector3 AdventureButtonSpacing
	{
		get
		{
			return this.m_adventureButtonSpacing;
		}
		set
		{
			this.m_adventureButtonSpacing = value;
			this.UpdateAdventureButtonPositions();
		}
	}

	// Token: 0x06005F55 RID: 24405 RVA: 0x001C8270 File Offset: 0x001C6470
	private void Awake()
	{
		this.m_adventureContent = (this.m_parentContent as GeneralStoreAdventureContent);
		if (this.m_adventureContent == null)
		{
			Debug.LogError("m_adventureContent is not the correct type: GeneralStoreAdventureContent");
		}
	}

	// Token: 0x06005F56 RID: 24406 RVA: 0x001C82AC File Offset: 0x001C64AC
	public override void StoreShown(bool isCurrent)
	{
		if (!this.m_paneInitialized)
		{
			this.m_paneInitialized = true;
			this.SetUpAdventureButtons();
			this.SetupInitialSelectedAdventure();
		}
		this.UpdateAdventureButtonPositions();
	}

	// Token: 0x06005F57 RID: 24407 RVA: 0x001C82E0 File Offset: 0x001C64E0
	protected override void OnRefresh()
	{
		foreach (GeneralStoreAdventureSelectorButton generalStoreAdventureSelectorButton in this.m_adventureButtons)
		{
			generalStoreAdventureSelectorButton.UpdateState();
		}
	}

	// Token: 0x06005F58 RID: 24408 RVA: 0x001C833C File Offset: 0x001C653C
	private void SetUpAdventureButtons()
	{
		Map<int, StoreAdventureDef> storeAdventureDefs = this.m_adventureContent.GetStoreAdventureDefs();
		foreach (KeyValuePair<int, StoreAdventureDef> keyValuePair in storeAdventureDefs)
		{
			AdventureDbId advType = (AdventureDbId)keyValuePair.Key;
			Network.Bundle bundle;
			bool flag;
			StoreManager.Get().GetAvailableAdventureBundle(advType, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle, out flag);
			if (flag)
			{
				string storeButtonPrefab = keyValuePair.Value.m_storeButtonPrefab;
				GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(storeButtonPrefab), true, false);
				if (!(gameObject == null))
				{
					GeneralStoreAdventureSelectorButton advButton = gameObject.GetComponent<GeneralStoreAdventureSelectorButton>();
					if (advButton == null)
					{
						Debug.LogError(string.Format("{0} does not contain GeneralStoreAdventureSelectorButton component.", storeButtonPrefab));
						Object.Destroy(gameObject);
					}
					else
					{
						GameUtils.SetParent(advButton, this.m_paneContainer, true);
						SceneUtils.SetLayer(advButton, this.m_paneContainer.layer);
						advButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
						{
							this.OnAdventureSelectorButtonClicked(advButton, advType);
						});
						advButton.SetAdventureType(advType);
						this.m_adventureButtons.Add(advButton);
					}
				}
			}
		}
		this.UpdateAdventureButtonPositions();
	}

	// Token: 0x06005F59 RID: 24409 RVA: 0x001C84C8 File Offset: 0x001C66C8
	private void OnAdventureSelectorButtonClicked(GeneralStoreAdventureSelectorButton btn, AdventureDbId adventureType)
	{
		if (!this.m_parentContent.IsContentActive())
		{
			return;
		}
		if (!btn.IsAvailable())
		{
			return;
		}
		this.m_adventureContent.SetAdventureType(adventureType, false);
		foreach (GeneralStoreAdventureSelectorButton generalStoreAdventureSelectorButton in this.m_adventureButtons)
		{
			generalStoreAdventureSelectorButton.Unselect();
		}
		btn.Select();
		Options.Get().SetInt(Option.LAST_SELECTED_STORE_ADVENTURE_ID, (int)btn.GetAdventureType());
		if (!string.IsNullOrEmpty(this.m_adventureSelectionSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_adventureSelectionSound));
		}
	}

	// Token: 0x06005F5A RID: 24410 RVA: 0x001C8588 File Offset: 0x001C6788
	private void UpdateAdventureButtonPositions()
	{
		GeneralStoreAdventureSelectorButton[] array = this.m_adventureButtons.ToArray();
		int i = 0;
		int num = 0;
		while (i < array.Length)
		{
			GeneralStoreAdventureSelectorButton generalStoreAdventureSelectorButton = array[i];
			generalStoreAdventureSelectorButton.transform.localPosition = this.m_adventureButtonSpacing * (float)num++;
			i++;
		}
	}

	// Token: 0x06005F5B RID: 24411 RVA: 0x001C85D8 File Offset: 0x001C67D8
	private void SetupInitialSelectedAdventure()
	{
		AdventureDbId adventureDbId = (AdventureDbId)Options.Get().GetInt(Option.LAST_SELECTED_STORE_ADVENTURE_ID, 0);
		Network.Bundle bundle = null;
		bool flag = false;
		StoreManager.Get().GetAvailableAdventureBundle(adventureDbId, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle, out flag);
		if (!flag)
		{
			adventureDbId = AdventureDbId.INVALID;
		}
		foreach (GeneralStoreAdventureSelectorButton generalStoreAdventureSelectorButton in this.m_adventureButtons)
		{
			if (generalStoreAdventureSelectorButton.GetAdventureType() == adventureDbId)
			{
				this.m_adventureContent.SetAdventureType(adventureDbId, false);
				generalStoreAdventureSelectorButton.Select();
				break;
			}
		}
	}

	// Token: 0x040046BB RID: 18107
	[SerializeField]
	private Vector3 m_adventureButtonSpacing;

	// Token: 0x040046BC RID: 18108
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public string m_adventureSelectionSound;

	// Token: 0x040046BD RID: 18109
	private List<GeneralStoreAdventureSelectorButton> m_adventureButtons = new List<GeneralStoreAdventureSelectorButton>();

	// Token: 0x040046BE RID: 18110
	private GeneralStoreAdventureContent m_adventureContent;

	// Token: 0x040046BF RID: 18111
	private bool m_paneInitialized;
}
