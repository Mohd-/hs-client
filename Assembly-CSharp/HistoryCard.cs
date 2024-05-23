using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200033D RID: 829
public class HistoryCard : HistoryItem
{
	// Token: 0x06002B52 RID: 11090 RVA: 0x000D76DC File Offset: 0x000D58DC
	public void LoadMainCardActor()
	{
		string text;
		if (this.m_fatigue)
		{
			text = "Card_Hand_Fatigue";
		}
		else
		{
			text = ActorNames.GetHistoryActor(this.m_entity);
		}
		GameObject gameObject = AssetLoader.Get().LoadActor(text, false, false);
		if (gameObject == null)
		{
			Debug.LogWarningFormat("HistoryCard.LoadMainCardActor() - FAILED to load actor \"{0}\"", new object[]
			{
				text
			});
			return;
		}
		Actor component = gameObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarningFormat("HistoryCard.LoadMainCardActor() - ERROR actor \"{0}\" has no Actor component", new object[]
			{
				text
			});
			return;
		}
		this.m_mainCardActor = component;
		if (this.m_fatigue)
		{
			this.m_mainCardActor.GetPowersText().Text = GameStrings.Get("GAMEPLAY_FATIGUE_HISTORY_TEXT");
		}
		else
		{
			this.m_mainCardActor.SetPremium(this.m_entity.GetPremiumType());
		}
		this.m_mainCardActor.SetHistoryItem(this);
		this.InitDisplayedCreator();
	}

	// Token: 0x06002B53 RID: 11091 RVA: 0x000D77BC File Offset: 0x000D59BC
	private void InitDisplayedCreator()
	{
		if (this.m_entity == null)
		{
			return;
		}
		Entity displayedCreator = this.m_entity.GetDisplayedCreator();
		if (displayedCreator == null)
		{
			return;
		}
		GameObject gameObject = this.m_mainCardActor.FindBone("HistoryCreatedByBone");
		if (!gameObject)
		{
			Error.AddDevWarning("Missing Bone", "Missing {0} on {1}", new object[]
			{
				"HistoryCreatedByBone",
				this.m_mainCardActor
			});
			return;
		}
		string name = displayedCreator.GetName();
		this.m_createdByText.Text = GameStrings.Format("GAMEPLAY_HISTORY_CREATED_BY", new object[]
		{
			name
		});
		this.m_createdByText.transform.parent = this.m_mainCardActor.GetRootObject().transform;
		this.m_createdByText.gameObject.SetActive(true);
		TransformUtil.SetPoint(this.m_createdByText, new Vector3(0.5f, 0f, 1f), gameObject, new Vector3(0.5f, 0f, 0f));
		this.m_createdByText.gameObject.SetActive(false);
		this.m_haveDisplayedCreator = true;
	}

	// Token: 0x06002B54 RID: 11092 RVA: 0x000D78CE File Offset: 0x000D5ACE
	private void ShowDisplayedCreator()
	{
		this.m_createdByText.gameObject.SetActive(this.m_haveDisplayedCreator);
	}

	// Token: 0x06002B55 RID: 11093 RVA: 0x000D78E6 File Offset: 0x000D5AE6
	public bool HasBeenShown()
	{
		return this.m_hasBeenShown;
	}

	// Token: 0x06002B56 RID: 11094 RVA: 0x000D78EE File Offset: 0x000D5AEE
	public void MarkAsShown()
	{
		if (this.m_hasBeenShown)
		{
			return;
		}
		this.m_hasBeenShown = true;
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x000D7904 File Offset: 0x000D5B04
	public Collider GetTileCollider()
	{
		if (this.m_tileActor == null)
		{
			return null;
		}
		if (this.m_tileActor.GetMeshRenderer() == null)
		{
			return null;
		}
		Transform transform = this.m_tileActor.GetMeshRenderer().transform.FindChild("Collider");
		if (transform == null)
		{
			return null;
		}
		return transform.GetComponent<Collider>();
	}

	// Token: 0x06002B58 RID: 11096 RVA: 0x000D796B File Offset: 0x000D5B6B
	public bool IsHalfSize()
	{
		return this.m_halfSize;
	}

	// Token: 0x06002B59 RID: 11097 RVA: 0x000D7974 File Offset: 0x000D5B74
	public void LoadTile(HistoryTileInitInfo info)
	{
		this.m_childInfos = info.m_childInfos;
		if (info.m_fatigueTexture != null)
		{
			this.m_portraitTexture = info.m_fatigueTexture;
			this.m_fatigue = true;
		}
		else
		{
			this.m_entity = info.m_entity;
			this.m_portraitTexture = info.m_portraitTexture;
			this.m_portraitGoldenMaterial = info.m_portraitGoldenMaterial;
			this.m_fullTileMaterial = info.m_fullTileMaterial;
			this.m_halfTileMaterial = info.m_halfTileMaterial;
			this.m_splatAmount = info.m_splatAmount;
			this.m_dead = info.m_dead;
		}
		switch (info.m_type)
		{
		case HistoryInfoType.NONE:
		case HistoryInfoType.WEAPON_PLAYED:
		case HistoryInfoType.CARD_PLAYED:
		case HistoryInfoType.FATIGUE:
			this.LoadPlayTile();
			break;
		case HistoryInfoType.ATTACK:
			this.LoadAttackTile();
			break;
		case HistoryInfoType.TRIGGER:
			this.LoadTriggerTile();
			break;
		case HistoryInfoType.WEAPON_BREAK:
			this.LoadWeaponBreak();
			break;
		}
	}

	// Token: 0x06002B5A RID: 11098 RVA: 0x000D7A64 File Offset: 0x000D5C64
	public void NotifyMousedOver()
	{
		if (this.m_mousedOver)
		{
			return;
		}
		if (this == HistoryManager.Get().GetCurrentBigCard())
		{
			return;
		}
		this.LoadChildCardsFromInfos();
		this.m_mousedOver = true;
		SoundManager.Get().LoadAndPlay("history_event_mouseover", this.m_tileActor.gameObject);
		if (!this.m_mainCardActor)
		{
			this.LoadMainCardActor();
			SceneUtils.SetLayer(this.m_mainCardActor, GameLayer.Tooltip);
		}
		this.ShowTile();
	}

	// Token: 0x06002B5B RID: 11099 RVA: 0x000D7AE4 File Offset: 0x000D5CE4
	public void NotifyMousedOut()
	{
		if (!this.m_mousedOver)
		{
			return;
		}
		this.m_mousedOver = false;
		if (this.m_gameEntityMousedOver)
		{
			GameState.Get().GetGameEntity().NotifyOfHistoryTokenMousedOut();
			this.m_gameEntityMousedOver = false;
		}
		KeywordHelpPanelManager.Get().HideKeywordHelp();
		if (this.m_mainCardActor)
		{
			this.m_mainCardActor.DeactivateAllSpells();
			this.m_mainCardActor.Hide();
		}
		for (int i = 0; i < this.m_historyChildren.Count; i++)
		{
			if (!(this.m_historyChildren[i].m_mainCardActor == null))
			{
				this.m_historyChildren[i].m_mainCardActor.DeactivateAllSpells();
				this.m_historyChildren[i].m_mainCardActor.Hide();
			}
		}
		if (this.m_separator)
		{
			this.m_separator.Hide();
		}
		HistoryManager.Get().UpdateLayout();
	}

	// Token: 0x06002B5C RID: 11100 RVA: 0x000D7BE3 File Offset: 0x000D5DE3
	private void LoadPlayTile()
	{
		this.m_halfSize = false;
		this.LoadTileImpl("HistoryTile_Card");
		this.LoadArrowSeparator();
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x000D7BFD File Offset: 0x000D5DFD
	private void LoadAttackTile()
	{
		this.m_halfSize = true;
		this.LoadTileImpl("HistoryTile_Attack");
		this.LoadSwordsSeparator();
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x000D7C17 File Offset: 0x000D5E17
	private void LoadWeaponBreak()
	{
		this.m_halfSize = true;
		this.LoadTileImpl("HistoryTile_Attack");
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x000D7C2B File Offset: 0x000D5E2B
	private void LoadTriggerTile()
	{
		this.m_halfSize = true;
		this.LoadTileImpl("HistoryTile_Trigger");
		this.LoadArrowSeparator();
	}

	// Token: 0x06002B60 RID: 11104 RVA: 0x000D7C48 File Offset: 0x000D5E48
	private void LoadTileImpl(string actorName)
	{
		GameObject gameObject = AssetLoader.Get().LoadActor(actorName, false, false);
		if (gameObject == null)
		{
			Debug.LogWarningFormat("HistoryCard.LoadTileImpl() - FAILED to load actor \"{0}\"", new object[]
			{
				actorName
			});
			return;
		}
		Actor component = gameObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarningFormat("HistoryCard.LoadTileImpl() - ERROR actor \"{0}\" has no Actor component", new object[]
			{
				actorName
			});
			return;
		}
		this.m_tileActor = component;
		this.m_tileActor.transform.parent = base.transform;
		TransformUtil.Identity(this.m_tileActor.transform);
		this.m_tileActor.transform.localScale = HistoryManager.Get().transform.localScale;
		Material[] array = new Material[2];
		array[0] = this.m_tileActor.GetMeshRenderer().materials[0];
		if (this.m_halfSize)
		{
			if (this.m_halfTileMaterial != null)
			{
				array[1] = this.m_halfTileMaterial;
				this.m_tileActor.GetMeshRenderer().materials = array;
			}
			else
			{
				this.m_tileActor.GetMeshRenderer().materials[1].mainTexture = this.m_portraitTexture;
			}
		}
		else if (this.m_fullTileMaterial != null)
		{
			array[1] = this.m_fullTileMaterial;
			this.m_tileActor.GetMeshRenderer().materials = array;
		}
		else
		{
			this.m_tileActor.GetMeshRenderer().materials[1].mainTexture = this.m_portraitTexture;
		}
		Color color = Color.white;
		if (Board.Get() != null)
		{
			color = Board.Get().m_HistoryTileColor;
		}
		if (!this.m_fatigue)
		{
			if (this.m_entity.IsControlledByFriendlySidePlayer())
			{
				color *= this.FRIENDLY_COLOR;
			}
			else
			{
				color *= this.OPPONENT_COLOR;
			}
		}
		foreach (Renderer renderer in this.m_tileActor.GetMeshRenderer().GetComponentsInChildren<Renderer>())
		{
			if (!(renderer.tag == "FakeShadow"))
			{
				renderer.material.color = Board.Get().m_HistoryTileColor;
			}
		}
		this.m_tileActor.GetMeshRenderer().materials[0].color = color;
		this.m_tileActor.GetMeshRenderer().materials[1].color = Board.Get().m_HistoryTileColor;
		if (this.GetTileCollider() != null && !this.IsHalfSize())
		{
			HistoryManager.Get().SetBigTileSize(this.GetTileCollider().bounds.size.z);
		}
	}

	// Token: 0x06002B61 RID: 11105 RVA: 0x000D7EF8 File Offset: 0x000D60F8
	private void LoadSwordsSeparator()
	{
		this.LoadSeparator("History_Swords");
	}

	// Token: 0x06002B62 RID: 11106 RVA: 0x000D7F08 File Offset: 0x000D6108
	private void LoadArrowSeparator()
	{
		if (this.m_childInfos == null)
		{
			return;
		}
		if (this.m_childInfos.Count == 0)
		{
			return;
		}
		this.LoadSeparator("History_Arrow");
	}

	// Token: 0x06002B63 RID: 11107 RVA: 0x000D7F40 File Offset: 0x000D6140
	private void LoadSeparator(string actorName)
	{
		GameObject gameObject = AssetLoader.Get().LoadActor(actorName, false, false);
		if (gameObject == null)
		{
			Debug.LogWarning(string.Format("HistoryCard.LoadSeparator() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		Actor component = gameObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("HistoryCard.LoadSeparator() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		this.m_separator = component;
		MeshRenderer component2 = this.m_separator.GetRootObject().transform.FindChild("Blue").gameObject.GetComponent<MeshRenderer>();
		MeshRenderer component3 = this.m_separator.GetRootObject().transform.FindChild("Red").gameObject.GetComponent<MeshRenderer>();
		if (this.m_fatigue)
		{
			component3.enabled = true;
			component2.enabled = false;
		}
		else
		{
			bool flag = this.m_entity.IsControlledByFriendlySidePlayer();
			component2.enabled = flag;
			component3.enabled = !flag;
		}
		this.m_separator.transform.parent = base.transform;
		TransformUtil.Identity(this.m_separator.transform);
		if (this.m_separator.GetRootObject() != null)
		{
			TransformUtil.Identity(this.m_separator.GetRootObject().transform);
		}
		this.m_separator.Hide();
	}

	// Token: 0x06002B64 RID: 11108 RVA: 0x000D8088 File Offset: 0x000D6288
	private void LoadChildCardsFromInfos()
	{
		if (this.m_childInfos == null)
		{
			return;
		}
		foreach (HistoryInfo historyInfo in this.m_childInfos)
		{
			GameObject gameObject = AssetLoader.Get().LoadActor("HistoryChildCard", false, false);
			HistoryChildCard component = gameObject.GetComponent<HistoryChildCard>();
			this.m_historyChildren.Add(component);
			Entity duplicatedEntity = historyInfo.GetDuplicatedEntity();
			CardDef cardDef = duplicatedEntity.GetCard().GetCardDef();
			component.SetCardInfo(duplicatedEntity, cardDef.GetPortraitTexture(), cardDef.GetPremiumPortraitMaterial(), historyInfo.GetSplatAmount(), historyInfo.HasDied());
			component.transform.parent = base.transform;
			component.LoadMainCardActor();
		}
		this.m_childInfos = null;
	}

	// Token: 0x06002B65 RID: 11109 RVA: 0x000D8164 File Offset: 0x000D6364
	private void ShowTile()
	{
		if (!this.m_mousedOver)
		{
			this.m_mainCardActor.Hide();
			return;
		}
		this.m_mainCardActor.Show();
		this.ShowDisplayedCreator();
		base.InitializeMainCardActor();
		base.DisplaySpells();
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_mainCardActor.transform.position = new Vector3(base.transform.position.x + this.MOUSE_OVER_X_OFFSET, base.transform.position.y + 7.524521f, this.GetZOffsetForThisTilesMouseOverCard());
		}
		else
		{
			this.m_mainCardActor.transform.position = new Vector3(base.transform.position.x + this.MOUSE_OVER_X_OFFSET, base.transform.position.y + 7.524521f, base.transform.position.z + this.GetZOffsetForThisTilesMouseOverCard());
		}
		this.m_mainCardActor.transform.localScale = new Vector3(this.MOUSE_OVER_SCALE, 1f, this.MOUSE_OVER_SCALE);
		if (!this.m_gameEntityMousedOver)
		{
			this.m_gameEntityMousedOver = true;
			GameState.Get().GetGameEntity().NotifyOfHistoryTokenMousedOver(base.gameObject);
		}
		if (!this.m_fatigue)
		{
			KeywordHelpPanelManager.Get().UpdateKeywordHelpForHistoryCard(this.m_entity, this.m_mainCardActor, this.m_createdByText);
		}
		if (this.m_historyChildren.Count <= 0)
		{
			return;
		}
		float num = 1f;
		float num2 = 1f;
		if (this.m_historyChildren.Count > 4 && this.m_historyChildren.Count < 9)
		{
			num2 = 2f;
			num = 0.5f;
		}
		else if (this.m_historyChildren.Count >= 9)
		{
			num2 = 3f;
			num = 0.3f;
		}
		int num3 = Mathf.CeilToInt((float)this.m_historyChildren.Count / num2);
		float num4 = (float)num3 * this.X_SIZE_OF_MOUSE_OVER_CHILD;
		float num5 = 5f / num4;
		num5 = Mathf.Clamp(num5, 0.1f, num);
		int num6 = 0;
		int num7 = 1;
		for (int i = 0; i < this.m_historyChildren.Count; i++)
		{
			this.m_historyChildren[i].m_mainCardActor.Show();
			this.m_historyChildren[i].InitializeMainCardActor();
			this.m_historyChildren[i].DisplaySpells();
			float num8 = this.m_mainCardActor.transform.position.z;
			if (num2 == 2f)
			{
				if (num7 == 1)
				{
					num8 += 0.78f;
				}
				else
				{
					num8 -= 0.78f;
				}
			}
			else if (num2 == 3f)
			{
				if (num7 == 1)
				{
					num8 += 0.98f;
				}
				else if (num7 == 3)
				{
					num8 -= 0.93f;
				}
			}
			float num9 = this.m_mainCardActor.transform.position.x + this.X_SIZE_OF_MOUSE_OVER_CHILD * (1f + num5) / 2f;
			this.m_historyChildren[i].m_mainCardActor.transform.position = new Vector3(num9 + this.X_SIZE_OF_MOUSE_OVER_CHILD * (float)num6 * num5, this.m_mainCardActor.transform.position.y, num8);
			this.m_historyChildren[i].m_mainCardActor.transform.localScale = new Vector3(num5, num5, num5);
			num6++;
			if (num6 >= num3)
			{
				num6 = 0;
				num7++;
			}
		}
		if (this.m_separator != null)
		{
			float num10 = 0.4f;
			float num11 = this.X_SIZE_OF_MOUSE_OVER_CHILD / 2f;
			this.m_separator.Show();
			this.m_separator.transform.position = new Vector3(this.m_mainCardActor.transform.position.x + num11, this.m_mainCardActor.transform.position.y + num10, this.m_mainCardActor.transform.position.z);
		}
	}

	// Token: 0x06002B66 RID: 11110 RVA: 0x000D85E4 File Offset: 0x000D67E4
	private float GetZOffsetForThisTilesMouseOverCard()
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			float num = Mathf.Abs(-1.5726469f);
			HistoryManager historyManager = HistoryManager.Get();
			float num2 = num / (float)historyManager.GetNumHistoryTiles();
			int num3 = historyManager.GetNumHistoryTiles() - historyManager.GetIndexForTile(this) - 1;
			return -1.404475f + num2 * (float)num3;
		}
		if (this.m_entity != null && this.m_entity.IsSecret() && this.m_entity.IsHidden())
		{
			return -4.3f;
		}
		return -4.75f;
	}

	// Token: 0x06002B67 RID: 11111 RVA: 0x000D8670 File Offset: 0x000D6870
	public void LoadBigCard(HistoryBigCardInitInfo info)
	{
		this.m_entity = info.m_entity;
		this.m_portraitTexture = info.m_portraitTexture;
		this.m_portraitGoldenMaterial = info.m_portraitGoldenMaterial;
		this.m_bigCardFinishedCallback = info.m_finishedCallback;
		this.m_bigCardCountered = info.m_countered;
		this.m_bigCardWaitingForSecret = info.m_waitForSecretSpell;
		this.m_bigCardFromMetaData = info.m_fromMetaData;
		this.m_bigCardPostTransformedEntity = info.m_postTransformedEntity;
		this.LoadMainCardActor();
	}

	// Token: 0x06002B68 RID: 11112 RVA: 0x000D86E4 File Offset: 0x000D68E4
	public void LoadBigCardPostTransformedEntity()
	{
		if (this.m_bigCardPostTransformedEntity == null)
		{
			return;
		}
		this.m_entity = this.m_bigCardPostTransformedEntity;
		Card card = this.m_entity.GetCard();
		this.m_portraitTexture = card.GetPortraitTexture();
		this.m_portraitGoldenMaterial = card.GetGoldenMaterial();
		this.LoadMainCardActor();
	}

	// Token: 0x06002B69 RID: 11113 RVA: 0x000D8733 File Offset: 0x000D6933
	public HistoryManager.BigCardFinishedCallback GetBigCardFinishedCallback()
	{
		return this.m_bigCardFinishedCallback;
	}

	// Token: 0x06002B6A RID: 11114 RVA: 0x000D873B File Offset: 0x000D693B
	public void RunBigCardFinishedCallback()
	{
		if (this.m_bigCardFinishedCallbackHasRun)
		{
			return;
		}
		this.m_bigCardFinishedCallbackHasRun = true;
		if (this.m_bigCardFinishedCallback != null)
		{
			this.m_bigCardFinishedCallback();
		}
	}

	// Token: 0x06002B6B RID: 11115 RVA: 0x000D8766 File Offset: 0x000D6966
	public bool WasBigCardCountered()
	{
		return this.m_bigCardCountered;
	}

	// Token: 0x06002B6C RID: 11116 RVA: 0x000D876E File Offset: 0x000D696E
	public bool IsBigCardWaitingForSecret()
	{
		return this.m_bigCardWaitingForSecret;
	}

	// Token: 0x06002B6D RID: 11117 RVA: 0x000D8776 File Offset: 0x000D6976
	public bool IsBigCardFromMetaData()
	{
		return this.m_bigCardFromMetaData;
	}

	// Token: 0x06002B6E RID: 11118 RVA: 0x000D877E File Offset: 0x000D697E
	public Entity GetBigCardPostTransformedEntity()
	{
		return this.m_bigCardPostTransformedEntity;
	}

	// Token: 0x06002B6F RID: 11119 RVA: 0x000D8786 File Offset: 0x000D6986
	public bool HasBigCardPostTransformedEntity()
	{
		return this.m_bigCardPostTransformedEntity != null;
	}

	// Token: 0x06002B70 RID: 11120 RVA: 0x000D8794 File Offset: 0x000D6994
	public void ShowBigCard(Vector3[] pathToFollow)
	{
		this.m_mainCardActor.transform.localScale = new Vector3(1.03f, 1.03f, 1.03f);
		if (this.m_entity != null)
		{
			if (this.m_entity.GetCardType() == TAG_CARDTYPE.SPELL || this.m_entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
			{
				pathToFollow[0] = this.m_mainCardActor.transform.position;
				Hashtable args = iTween.Hash(new object[]
				{
					"path",
					pathToFollow,
					"time",
					1f,
					"oncomplete",
					"OnBigCardPathComplete",
					"oncompletetarget",
					base.gameObject
				});
				iTween.MoveTo(this.m_mainCardActor.gameObject, args);
				iTween.ScaleTo(base.gameObject, new Vector3(1f, 1f, 1f), 1f);
				SoundManager.Get().LoadAndPlay("play_card_from_hand_1");
			}
			else
			{
				this.ShowDisplayedCreator();
			}
		}
	}

	// Token: 0x06002B71 RID: 11121 RVA: 0x000D88AC File Offset: 0x000D6AAC
	private void OnBigCardPathComplete()
	{
		this.ShowDisplayedCreator();
	}

	// Token: 0x04001A35 RID: 6709
	private const float ABILITY_CARD_ANIMATE_TO_BIG_CARD_AREA_TIME = 1f;

	// Token: 0x04001A36 RID: 6710
	private const float BIG_CARD_SCALE = 1.03f;

	// Token: 0x04001A37 RID: 6711
	private const float MOUSE_OVER_Z_OFFSET_TOP = -1.404475f;

	// Token: 0x04001A38 RID: 6712
	private const float MOUSE_OVER_Z_OFFSET_BOTTOM = 0.1681719f;

	// Token: 0x04001A39 RID: 6713
	private const float MOUSE_OVER_Z_OFFSET_PHONE = -4.75f;

	// Token: 0x04001A3A RID: 6714
	private const float MOUSE_OVER_Z_OFFSET_SECRET_PHONE = -4.3f;

	// Token: 0x04001A3B RID: 6715
	private const float MOUSE_OVER_HEIGHT_OFFSET = 7.524521f;

	// Token: 0x04001A3C RID: 6716
	private const float MAX_WIDTH_OF_CHILDREN = 5f;

	// Token: 0x04001A3D RID: 6717
	private const string CREATED_BY_BONE_NAME = "HistoryCreatedByBone";

	// Token: 0x04001A3E RID: 6718
	public Actor m_tileActor;

	// Token: 0x04001A3F RID: 6719
	public UberText m_createdByText;

	// Token: 0x04001A40 RID: 6720
	private readonly Color OPPONENT_COLOR = new Color(0.7137f, 0.2f, 0.1333f, 1f);

	// Token: 0x04001A41 RID: 6721
	private readonly Color FRIENDLY_COLOR = new Color(0.6509f, 0.6705f, 0.9843f, 1f);

	// Token: 0x04001A42 RID: 6722
	private PlatformDependentValue<float> MOUSE_OVER_X_OFFSET = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 4.326718f,
		Tablet = 4.7f,
		Phone = 5.4f
	};

	// Token: 0x04001A43 RID: 6723
	private PlatformDependentValue<float> MOUSE_OVER_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 1f,
		Tablet = 1.35f,
		Phone = 1.35f
	};

	// Token: 0x04001A44 RID: 6724
	private PlatformDependentValue<float> X_SIZE_OF_MOUSE_OVER_CHILD = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 2.5f,
		Tablet = 2.5f,
		Phone = 2.5f
	};

	// Token: 0x04001A45 RID: 6725
	private Material m_fullTileMaterial;

	// Token: 0x04001A46 RID: 6726
	private Material m_halfTileMaterial;

	// Token: 0x04001A47 RID: 6727
	private bool m_mousedOver;

	// Token: 0x04001A48 RID: 6728
	private bool m_halfSize;

	// Token: 0x04001A49 RID: 6729
	private bool m_hasBeenShown;

	// Token: 0x04001A4A RID: 6730
	private Actor m_separator;

	// Token: 0x04001A4B RID: 6731
	private bool m_haveDisplayedCreator;

	// Token: 0x04001A4C RID: 6732
	private bool m_gameEntityMousedOver;

	// Token: 0x04001A4D RID: 6733
	private List<HistoryInfo> m_childInfos;

	// Token: 0x04001A4E RID: 6734
	private List<HistoryChildCard> m_historyChildren = new List<HistoryChildCard>();

	// Token: 0x04001A4F RID: 6735
	private bool m_bigCardFinishedCallbackHasRun;

	// Token: 0x04001A50 RID: 6736
	private HistoryManager.BigCardFinishedCallback m_bigCardFinishedCallback;

	// Token: 0x04001A51 RID: 6737
	private bool m_bigCardCountered;

	// Token: 0x04001A52 RID: 6738
	private bool m_bigCardWaitingForSecret;

	// Token: 0x04001A53 RID: 6739
	private bool m_bigCardFromMetaData;

	// Token: 0x04001A54 RID: 6740
	private Entity m_bigCardPostTransformedEntity;
}
