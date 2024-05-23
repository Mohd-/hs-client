using System;
using UnityEngine;

// Token: 0x0200036C RID: 876
[CustomEditClass]
public class AdventureMissionDeckPickerDisplay : MonoBehaviour
{
	// Token: 0x06002C7F RID: 11391 RVA: 0x000DCB98 File Offset: 0x000DAD98
	private void Awake()
	{
		GameObject gameObject = AssetLoader.Get().LoadActor((!UniversalInputManager.UsePhoneUI) ? "DeckPickerTray" : "DeckPickerTray_phone", false, false);
		if (gameObject == null)
		{
			Debug.LogError("Unable to load DeckPickerTray.");
			return;
		}
		this.m_deckPickerTray = gameObject.GetComponent<DeckPickerTrayDisplay>();
		if (this.m_deckPickerTray == null)
		{
			Debug.LogError("DeckPickerTrayDisplay component not found in DeckPickerTray object.");
			return;
		}
		if (this.m_deckPickerTrayContainer != null)
		{
			GameUtils.SetParent(this.m_deckPickerTray, this.m_deckPickerTrayContainer, false);
		}
		this.m_deckPickerTray.AddDeckTrayLoadedListener(new DeckPickerTrayDisplay.DeckTrayLoaded(this.OnTrayLoaded));
		this.m_deckPickerTray.Init();
		this.m_deckPickerTray.SetPlayButtonText(GameStrings.Get("GLOBAL_PLAY"));
		AdventureConfig adventureConfig = AdventureConfig.Get();
		AdventureDbId selectedAdventure = adventureConfig.GetSelectedAdventure();
		AdventureModeDbId selectedMode = adventureConfig.GetSelectedMode();
		AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int)selectedAdventure, (int)selectedMode);
		this.m_deckPickerTray.SetHeaderText(adventureDataRecord.Name);
	}

	// Token: 0x06002C80 RID: 11392 RVA: 0x000DCCA0 File Offset: 0x000DAEA0
	private void OnTrayLoaded()
	{
		AdventureSubScene component = base.GetComponent<AdventureSubScene>();
		if (component != null)
		{
			component.SetIsLoaded(true);
		}
	}

	// Token: 0x04001B85 RID: 7045
	public GameObject m_deckPickerTrayContainer;

	// Token: 0x04001B86 RID: 7046
	private DeckPickerTrayDisplay m_deckPickerTray;
}
