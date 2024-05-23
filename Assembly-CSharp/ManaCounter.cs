using System;
using UnityEngine;

// Token: 0x0200061E RID: 1566
public class ManaCounter : MonoBehaviour
{
	// Token: 0x06004466 RID: 17510 RVA: 0x00148DF0 File Offset: 0x00146FF0
	private void Awake()
	{
		this.m_textMesh = base.GetComponent<UberText>();
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_phoneGem = AssetLoader.Get().LoadActor("Resource_Large_phone", false, false);
			GameUtils.SetParent(this.m_phoneGem, this.m_phoneGemContainer, true);
		}
	}

	// Token: 0x06004467 RID: 17511 RVA: 0x00148E41 File Offset: 0x00147041
	private void Start()
	{
		this.m_textMesh.Text = GameStrings.Format("GAMEPLAY_MANA_COUNTER", new object[]
		{
			"0",
			"0"
		});
	}

	// Token: 0x06004468 RID: 17512 RVA: 0x00148E6E File Offset: 0x0014706E
	public void SetPlayer(Player player)
	{
		this.m_player = player;
	}

	// Token: 0x06004469 RID: 17513 RVA: 0x00148E77 File Offset: 0x00147077
	public Player GetPlayer()
	{
		return this.m_player;
	}

	// Token: 0x0600446A RID: 17514 RVA: 0x00148E7F File Offset: 0x0014707F
	public GameObject GetPhoneGem()
	{
		return this.m_phoneGem;
	}

	// Token: 0x0600446B RID: 17515 RVA: 0x00148E88 File Offset: 0x00147088
	public void UpdateText()
	{
		int tag = this.m_player.GetTag(GAME_TAG.RESOURCES);
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
		}
		int numAvailableResources = this.m_player.GetNumAvailableResources();
		string text;
		if (UniversalInputManager.UsePhoneUI && tag >= 10)
		{
			text = numAvailableResources.ToString();
		}
		else
		{
			text = GameStrings.Format("GAMEPLAY_MANA_COUNTER", new object[]
			{
				numAvailableResources,
				tag
			});
		}
		this.m_textMesh.Text = text;
		if (UniversalInputManager.UsePhoneUI && this.m_availableManaPhone != null)
		{
			this.m_availableManaPhone.Text = numAvailableResources.ToString();
			this.m_permanentManaPhone.Text = tag.ToString();
		}
	}

	// Token: 0x04002B5E RID: 11102
	public Player.Side m_Side;

	// Token: 0x04002B5F RID: 11103
	public GameObject m_phoneGemContainer;

	// Token: 0x04002B60 RID: 11104
	public UberText m_availableManaPhone;

	// Token: 0x04002B61 RID: 11105
	public UberText m_permanentManaPhone;

	// Token: 0x04002B62 RID: 11106
	private Player m_player;

	// Token: 0x04002B63 RID: 11107
	private UberText m_textMesh;

	// Token: 0x04002B64 RID: 11108
	private GameObject m_phoneGem;
}
