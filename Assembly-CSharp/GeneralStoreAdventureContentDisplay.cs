using System;
using UnityEngine;

// Token: 0x02000453 RID: 1107
public class GeneralStoreAdventureContentDisplay : MonoBehaviour
{
	// Token: 0x060036D7 RID: 14039 RVA: 0x0010DB08 File Offset: 0x0010BD08
	private void Awake()
	{
		if (this.m_leavingSoonButton != null)
		{
			this.m_leavingSoonButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.OnLeavingSoonButtonClicked();
			});
		}
	}

	// Token: 0x060036D8 RID: 14040 RVA: 0x0010DB40 File Offset: 0x0010BD40
	public void UpdateAdventureType(StoreAdventureDef advDef, AdventureDbfRecord advRecord)
	{
		if (advDef == null)
		{
			return;
		}
		AssetLoader.Get().LoadTexture(FileUtils.GameAssetPathToName(advDef.m_logoTextureName), delegate(string name, Object obj, object data)
		{
			Texture texture = obj as Texture;
			if (texture == null)
			{
				Debug.LogError(string.Format("Failed to load texture {0}!", name));
				return;
			}
			this.m_logo.material.mainTexture = texture;
		}, null, false);
		this.m_keyArt.material = advDef.m_keyArt;
		if (this.m_leavingSoonBanner != null)
		{
			this.m_leavingSoonBanner.SetActive(advRecord.LeavingSoon);
			if (advRecord.LeavingSoon)
			{
				this.m_leavingSoonInfoText = advRecord.LeavingSoonText;
			}
		}
	}

	// Token: 0x060036D9 RID: 14041 RVA: 0x0010DBD0 File Offset: 0x0010BDD0
	public void SetPreOrder(bool preorder)
	{
		if (this.m_rewardChest != null)
		{
			this.m_rewardChest.gameObject.SetActive(!preorder);
		}
		if (this.m_rewardsFrame != null)
		{
			this.m_rewardsFrame.SetActive(!preorder);
		}
		if (this.m_preorderFrame != null)
		{
			this.m_preorderFrame.SetActive(preorder);
		}
	}

	// Token: 0x060036DA RID: 14042 RVA: 0x0010DC40 File Offset: 0x0010BE40
	private void OnLeavingSoonButtonClicked()
	{
		DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo
		{
			m_headerText = GameStrings.Get("GLUE_STORE_ADVENTURE_LEAVING_SOON"),
			m_text = this.m_leavingSoonInfoText,
			m_showAlertIcon = true,
			m_responseDisplay = AlertPopup.ResponseDisplay.OK
		});
	}

	// Token: 0x0400221C RID: 8732
	public PegUIElement m_rewardChest;

	// Token: 0x0400221D RID: 8733
	public GameObject m_rewardsFrame;

	// Token: 0x0400221E RID: 8734
	public GameObject m_preorderFrame;

	// Token: 0x0400221F RID: 8735
	public GameObject m_leavingSoonBanner;

	// Token: 0x04002220 RID: 8736
	public UIBButton m_leavingSoonButton;

	// Token: 0x04002221 RID: 8737
	public MeshRenderer m_logo;

	// Token: 0x04002222 RID: 8738
	public MeshRenderer m_keyArt;

	// Token: 0x04002223 RID: 8739
	private string m_leavingSoonInfoText;
}
