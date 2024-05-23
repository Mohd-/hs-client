using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class CardBackManager : MonoBehaviour
{
	// Token: 0x0600115A RID: 4442 RVA: 0x0004AA54 File Offset: 0x00048C54
	private void Awake()
	{
		CardBackManager.s_instance = this;
		this.InitCardBackData();
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x0004AA83 File Offset: 0x00048C83
	private void OnDestroy()
	{
		CardBackManager.s_instance = null;
		if (ApplicationMgr.Get() == null)
		{
			return;
		}
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x0004AAB4 File Offset: 0x00048CB4
	private void Start()
	{
		Options.Get().RegisterChangedListener(Option.CARD_BACK, new Options.ChangedCallback(this.OnCheatOptionChanged));
		Options.Get().RegisterChangedListener(Option.CARD_BACK2, new Options.ChangedCallback(this.OnCheatOptionChanged));
		if (!this.m_ResetDefaultRegistered)
		{
			SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneChangeResetDefaultCardBack));
			this.m_ResetDefaultRegistered = true;
		}
		this.InitCardBacks();
		base.StartCoroutine(this.RegisterForDefaultCardBackChangesWhenPossible());
		base.StartCoroutine(this.UpdateDefaultCardBackWhenReady());
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x0004AB3B File Offset: 0x00048D3B
	private void WillReset()
	{
		this.InitCardBackData();
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x0004AB43 File Offset: 0x00048D43
	public static CardBackManager Get()
	{
		return CardBackManager.s_instance;
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x0004AB4C File Offset: 0x00048D4C
	public int GetDeckCardBackID(long deck)
	{
		NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
		NetCache.DeckHeader deckHeader = netObject.Decks.Find((NetCache.DeckHeader obj) => obj.ID == deck);
		if (deckHeader == null)
		{
			Debug.LogWarning(string.Format("CardBackManager.GetDeckCardBackID() could not find deck with ID {0}", deck));
			return 0;
		}
		return deckHeader.CardBack;
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x0004ABB1 File Offset: 0x00048DB1
	public CardBack GetFriendlyCardBack()
	{
		return this.m_FriendlyCardBack;
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x0004ABB9 File Offset: 0x00048DB9
	public CardBack GetOpponentCardBack()
	{
		return this.m_OpponentCardBack;
	}

	// Token: 0x06001162 RID: 4450 RVA: 0x0004ABC1 File Offset: 0x00048DC1
	public void UpdateAllCardBacks()
	{
		base.StartCoroutine(this.UpdateAllCardBacksImpl());
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x0004ABD0 File Offset: 0x00048DD0
	public void SetGameCardBackIDs(int friendlyCardBackID, int opponentCardBackID)
	{
		int validCardBackID = this.GetValidCardBackID(friendlyCardBackID);
		this.LoadCardBack(FileUtils.GameAssetPathToName(this.m_cardBackData[validCardBackID].PrefabName), true);
		int validCardBackID2 = this.GetValidCardBackID(opponentCardBackID);
		this.LoadCardBack(FileUtils.GameAssetPathToName(this.m_cardBackData[validCardBackID2].PrefabName), false);
		this.UpdateAllCardBacks();
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x0004AC2D File Offset: 0x00048E2D
	public bool LoadCardBackByIndex(int cardBackIdx, CardBackManager.LoadCardBackData.LoadCardBackCallback callback, string actorName = "Card_Hidden")
	{
		return this.LoadCardBackByIndex(cardBackIdx, callback, false, actorName);
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x0004AC3C File Offset: 0x00048E3C
	public bool LoadCardBackByIndex(int cardBackIdx, CardBackManager.LoadCardBackData.LoadCardBackCallback callback, bool unlit, string actorName = "Card_Hidden")
	{
		if (!this.m_cardBackData.ContainsKey(cardBackIdx))
		{
			Log.CardbackMgr.Print("CardBackManager.LoadCardBackByIndex() - wrong cardBackIdx {0}", new object[]
			{
				cardBackIdx
			});
			return false;
		}
		CardBackManager.LoadCardBackData loadCardBackData = new CardBackManager.LoadCardBackData();
		loadCardBackData.m_CardBackIndex = cardBackIdx;
		loadCardBackData.m_Callback = callback;
		loadCardBackData.m_Unlit = unlit;
		loadCardBackData.m_Name = this.m_cardBackData[cardBackIdx].Name;
		AssetLoader assetLoader = AssetLoader.Get();
		assetLoader.LoadActor(actorName, new AssetLoader.GameObjectCallback(this.OnHiddenActorLoaded), loadCardBackData, false);
		return true;
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0004ACCC File Offset: 0x00048ECC
	public CardBackManager.LoadCardBackData LoadCardBackByIndex(int cardBackIdx, bool unlit = false, string actorName = "Card_Hidden")
	{
		if (!this.m_cardBackData.ContainsKey(cardBackIdx))
		{
			Log.CardbackMgr.Print("CardBackManager.LoadCardBackByIndex() - wrong cardBackIdx {0}", new object[]
			{
				cardBackIdx
			});
			return null;
		}
		CardBackManager.LoadCardBackData loadCardBackData = new CardBackManager.LoadCardBackData();
		loadCardBackData.m_CardBackIndex = cardBackIdx;
		loadCardBackData.m_Unlit = unlit;
		loadCardBackData.m_Name = this.m_cardBackData[cardBackIdx].Name;
		loadCardBackData.m_GameObject = AssetLoader.Get().LoadActor(actorName, false, false);
		if (loadCardBackData.m_GameObject == null)
		{
			Log.CardbackMgr.Print("CardBackManager.LoadCardBackByIndex() - failed to load Actor {0}", new object[]
			{
				actorName
			});
			return null;
		}
		string prefabName = this.m_cardBackData[cardBackIdx].PrefabName;
		GameObject gameObject = AssetLoader.Get().LoadCardBack(FileUtils.GameAssetPathToName(prefabName), true, false);
		if (gameObject == null)
		{
			Log.CardbackMgr.Print("CardBackManager.LoadCardBackByIndex() - failed to load CardBack {0}", new object[]
			{
				prefabName
			});
			return null;
		}
		CardBack componentInChildren = gameObject.GetComponentInChildren<CardBack>();
		if (componentInChildren == null)
		{
			Debug.LogWarning("CardBackManager.LoadCardBackByIndex() - cardback=null");
			return null;
		}
		loadCardBackData.m_CardBack = componentInChildren;
		Actor component = loadCardBackData.m_GameObject.GetComponent<Actor>();
		this.SetCardBack(component.m_cardMesh, loadCardBackData.m_CardBack, loadCardBackData.m_Unlit);
		component.SetCardbackUpdateIgnore(true);
		loadCardBackData.m_CardBack.gameObject.transform.parent = loadCardBackData.m_GameObject.transform;
		return loadCardBackData;
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x0004AE38 File Offset: 0x00049038
	public void AddNewCardBack(int cardBackID)
	{
		NetCache.NetCacheCardBacks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>();
		if (netObject == null)
		{
			Debug.LogWarning(string.Format("CardBackManager.AddNewCardBack({0}): trying to access NetCacheCardBacks before it's been loaded", cardBackID));
			return;
		}
		netObject.CardBacks.Add(cardBackID);
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x0004AE7C File Offset: 0x0004907C
	public int GetDefaultCardBackID()
	{
		NetCache.NetCacheCardBacks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>();
		if (netObject == null)
		{
			Debug.LogWarning("CardBackManager.GetDefaultCardBackID(): trying to access NetCacheCardBacks before it's been loaded");
			return 0;
		}
		return netObject.DefaultCardBack;
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x0004AEAC File Offset: 0x000490AC
	public string GetCardBackName(int cardBackId)
	{
		CardBackData cardBackData;
		if (this.m_cardBackData.TryGetValue(cardBackId, out cardBackData))
		{
			return cardBackData.Name;
		}
		return null;
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x0004AED4 File Offset: 0x000490D4
	public int GetNumCardBacksOwned()
	{
		NetCache.NetCacheCardBacks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>();
		if (netObject == null)
		{
			Debug.LogWarning("CardBackManager.GetNumCardBacksOwned(): trying to access NetCacheCardBacks before it's been loaded");
			return 0;
		}
		return netObject.CardBacks.Count;
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x0004AF0C File Offset: 0x0004910C
	public HashSet<int> GetCardBacksOwned()
	{
		NetCache.NetCacheCardBacks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>();
		if (netObject == null)
		{
			Debug.LogWarning("CardBackManager.GetCardBacksOwned(): trying to access NetCacheCardBacks before it's been loaded");
			return null;
		}
		return netObject.CardBacks;
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x0004AF3C File Offset: 0x0004913C
	public HashSet<int> GetCardBackIds(bool all = true)
	{
		HashSet<int> hashSet = new HashSet<int>();
		if (all)
		{
			foreach (KeyValuePair<int, CardBackData> keyValuePair in this.m_cardBackData)
			{
				if (keyValuePair.Value.Enabled)
				{
					hashSet.Add(keyValuePair.Key);
				}
			}
			return hashSet;
		}
		return this.GetCardBacksOwned();
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x0004AFC8 File Offset: 0x000491C8
	public bool IsCardBackOwned(int cardBackID)
	{
		NetCache.NetCacheCardBacks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>();
		if (netObject == null)
		{
			Debug.LogWarning(string.Format("CardBackManager.IsCardBackOwned({0}): trying to access NetCacheCardBacks before it's been loaded", cardBackID));
			return false;
		}
		return netObject.CardBacks.Contains(cardBackID);
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x0004B00C File Offset: 0x0004920C
	public bool IsCardBackEnabled(int cardBackID)
	{
		CardBackData cardBackData;
		return this.m_cardBackData.TryGetValue(cardBackID, out cardBackData) && cardBackData.Enabled;
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x0004B034 File Offset: 0x00049234
	public List<CardBackData> GetEnabledCardBacks()
	{
		List<CardBackData> list = Enumerable.ToList<CardBackData>(this.m_cardBackData.Values);
		return list.FindAll((CardBackData obj) => obj.Enabled);
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x0004B078 File Offset: 0x00049278
	public List<CardBackManager.OwnedCardBack> GetOrderedEnabledCardBacks(bool checkOwned)
	{
		List<CardBackData> enabledCardBacks = this.GetEnabledCardBacks();
		List<CardBackManager.OwnedCardBack> list = new List<CardBackManager.OwnedCardBack>();
		foreach (CardBackData cardBackData in enabledCardBacks)
		{
			bool flag = this.IsCardBackOwned(cardBackData.ID);
			if (!checkOwned || flag)
			{
				CardBackDbfRecord record = GameDbf.CardBack.GetRecord(cardBackData.ID);
				long seasonId = -1L;
				if (record.Source == "season")
				{
					seasonId = record.Data1;
				}
				list.Add(new CardBackManager.OwnedCardBack
				{
					m_cardBackId = cardBackData.ID,
					m_owned = flag,
					m_sortOrder = record.SortOrder,
					m_sortCategory = record.SortCategory,
					m_seasonId = seasonId
				});
			}
		}
		list.Sort(delegate(CardBackManager.OwnedCardBack lhs, CardBackManager.OwnedCardBack rhs)
		{
			if (lhs.m_owned != rhs.m_owned)
			{
				return (!lhs.m_owned) ? 1 : -1;
			}
			if (lhs.m_sortCategory != rhs.m_sortCategory)
			{
				return (lhs.m_sortCategory >= rhs.m_sortCategory) ? 1 : -1;
			}
			if (lhs.m_sortOrder != rhs.m_sortOrder)
			{
				return (lhs.m_sortOrder >= rhs.m_sortOrder) ? 1 : -1;
			}
			if (lhs.m_seasonId != rhs.m_seasonId)
			{
				return (lhs.m_seasonId <= rhs.m_seasonId) ? 1 : -1;
			}
			return Mathf.Clamp(lhs.m_cardBackId - rhs.m_cardBackId, -1, 1);
		});
		return list;
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x0004B194 File Offset: 0x00049394
	public int GetNumEnabledCardBacks()
	{
		return this.GetEnabledCardBacks().Count;
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x0004B1A4 File Offset: 0x000493A4
	public void SetCardBackTexture(Renderer renderer, int matIdx, bool friendlySide)
	{
		if (friendlySide && this.m_isFriendlyLoading)
		{
			base.StartCoroutine(this.SetTextureWhenLoaded(renderer, matIdx, friendlySide));
			return;
		}
		if (!friendlySide && this.m_isOpponentLoading)
		{
			base.StartCoroutine(this.SetTextureWhenLoaded(renderer, matIdx, friendlySide));
			return;
		}
		this.SetTexture(renderer, matIdx, friendlySide);
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x0004B200 File Offset: 0x00049400
	public void UpdateCardBack(Actor actor, CardBack cardBack)
	{
		if (actor.gameObject == null || actor.m_cardMesh == null || cardBack == null)
		{
			return;
		}
		this.SetCardBack(actor.m_cardMesh, cardBack);
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x0004B24C File Offset: 0x0004944C
	public void UpdateCardBackWithInternalCardBack(Actor actor)
	{
		if (actor.gameObject == null || actor.m_cardMesh == null)
		{
			return;
		}
		CardBack componentInChildren = actor.gameObject.GetComponentInChildren<CardBack>();
		if (componentInChildren == null)
		{
			return;
		}
		this.SetCardBack(actor.m_cardMesh, componentInChildren);
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x0004B2A4 File Offset: 0x000494A4
	public void UpdateCardBack(GameObject go, bool friendlySide)
	{
		if (go == null)
		{
			return;
		}
		if (friendlySide && this.m_isFriendlyLoading)
		{
			base.StartCoroutine(this.SetCardBackWhenLoaded(go, friendlySide));
			return;
		}
		if (!friendlySide && this.m_isOpponentLoading)
		{
			base.StartCoroutine(this.SetCardBackWhenLoaded(go, friendlySide));
			return;
		}
		this.SetCardBack(go, friendlySide);
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x0004B308 File Offset: 0x00049508
	public void UpdateDeck(GameObject go, bool friendlySide)
	{
		if (go == null)
		{
			return;
		}
		if (friendlySide && this.m_isFriendlyLoading)
		{
			base.StartCoroutine(this.SetDeckCardBackWhenLoaded(go, friendlySide));
			return;
		}
		if (!friendlySide && this.m_isOpponentLoading)
		{
			base.StartCoroutine(this.SetDeckCardBackWhenLoaded(go, friendlySide));
			return;
		}
		this.SetDeckCardBack(go, friendlySide);
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x0004B36C File Offset: 0x0004956C
	public void UpdateDragEffect(GameObject go, bool friendlySide)
	{
		if (go == null)
		{
			return;
		}
		if (friendlySide && this.m_isFriendlyLoading)
		{
			base.StartCoroutine(this.SetDragEffectsWhenLoaded(go, friendlySide));
			return;
		}
		if (!friendlySide && this.m_isOpponentLoading)
		{
			base.StartCoroutine(this.SetDragEffectsWhenLoaded(go, friendlySide));
			return;
		}
		this.SetDragEffects(go, friendlySide);
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x0004B3D0 File Offset: 0x000495D0
	public bool IsActorFriendly(Actor actor)
	{
		if (actor == null)
		{
			Log.Kyle.Print("CardBack IsActorFriendly: actor is null!", new object[0]);
			return true;
		}
		Entity entity = actor.GetEntity();
		if (entity != null)
		{
			Player controller = entity.GetController();
			if (controller != null && controller.GetSide() == Player.Side.OPPOSING)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x0004B429 File Offset: 0x00049629
	public CardBack GetCardBack(Actor actor)
	{
		if (this.IsActorFriendly(actor))
		{
			return this.m_FriendlyCardBack;
		}
		return this.m_OpponentCardBack;
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x0004B444 File Offset: 0x00049644
	private void InitCardBacks()
	{
		this.LoadDefaultCardBack();
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x0004B44C File Offset: 0x0004964C
	public void InitCardBackData()
	{
		List<CardBackData> list = new List<CardBackData>();
		List<CardBackDbfRecord> records = GameDbf.CardBack.GetRecords();
		foreach (CardBackDbfRecord cardBackDbfRecord in records)
		{
			list.Add(new CardBackData(cardBackDbfRecord.ID, EnumUtils.GetEnum<CardBackData.CardBackSource>(cardBackDbfRecord.Source), cardBackDbfRecord.Data1, cardBackDbfRecord.Name, cardBackDbfRecord.Enabled, cardBackDbfRecord.PrefabName));
		}
		this.m_cardBackData = new Map<int, CardBackData>();
		foreach (CardBackData cardBackData in list)
		{
			this.m_cardBackData[cardBackData.ID] = cardBackData;
		}
		this.m_LoadedCardBacks = new Map<string, CardBack>();
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x0004B550 File Offset: 0x00049750
	private IEnumerator SetTextureWhenLoaded(Renderer renderer, int matIdx, bool friendlySide)
	{
		if (friendlySide)
		{
			while (this.m_isFriendlyLoading)
			{
				yield return null;
			}
		}
		else
		{
			while (this.m_isOpponentLoading)
			{
				yield return null;
			}
		}
		this.SetTexture(renderer, matIdx, friendlySide);
		yield break;
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x0004B598 File Offset: 0x00049798
	private void SetTexture(Renderer renderer, int matIdx, bool friendlySide)
	{
		if (friendlySide && this.m_FriendlyCardBack == null)
		{
			return;
		}
		if (!friendlySide && this.m_OpponentCardBack == null)
		{
			return;
		}
		Texture cardBackTexture;
		if (friendlySide)
		{
			cardBackTexture = this.m_FriendlyCardBack.m_CardBackTexture;
		}
		else
		{
			cardBackTexture = this.m_OpponentCardBack.m_CardBackTexture;
		}
		if (cardBackTexture == null)
		{
			Debug.LogWarning(string.Format("CardBackManager SetTexture(): texture is null!   obj: {0}  friendly: {1}", renderer.gameObject.name, friendlySide));
			return;
		}
		if (renderer == null)
		{
			return;
		}
		renderer.materials[matIdx].mainTexture = cardBackTexture;
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0004B644 File Offset: 0x00049844
	private IEnumerator SetCardBackWhenLoaded(GameObject go, bool friendlySide)
	{
		if (friendlySide)
		{
			while (this.m_isFriendlyLoading)
			{
				yield return null;
			}
		}
		else
		{
			while (this.m_isOpponentLoading)
			{
				yield return null;
			}
		}
		this.SetCardBack(go, friendlySide);
		yield break;
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0004B67C File Offset: 0x0004987C
	private void SetCardBack(GameObject go, bool friendlySide)
	{
		CardBack cardBack;
		if (friendlySide)
		{
			if (this.m_FriendlyCardBack == null)
			{
				cardBack = this.m_DefaultCardBack;
				Debug.LogWarning("CardBackManager.SetCardBack() m_FriendlyCardBack=null");
			}
			else
			{
				cardBack = this.m_FriendlyCardBack;
			}
		}
		else if (this.m_OpponentCardBack == null)
		{
			cardBack = this.m_DefaultCardBack;
			Debug.LogWarning("CardBackManager.SetCardBack() m_OpponentCardBack=null");
		}
		else
		{
			cardBack = this.m_OpponentCardBack;
		}
		if (cardBack == null)
		{
			Debug.LogWarning("CardBackManager SetCardBack() cardBack=null");
			return;
		}
		this.SetCardBack(go, cardBack);
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0004B70F File Offset: 0x0004990F
	private void SetCardBack(GameObject go, CardBack cardBack)
	{
		this.SetCardBack(go, cardBack, false);
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x0004B71C File Offset: 0x0004991C
	private void SetCardBack(GameObject go, CardBack cardBack, bool unlit)
	{
		if (cardBack == null)
		{
			Debug.LogWarning("CardBackManager SetCardBack() cardback=null");
			return;
		}
		if (go == null)
		{
			Debug.LogWarning("CardBackManager SetCardBack() go=null");
			return;
		}
		Mesh cardBackMesh = cardBack.m_CardBackMesh;
		if (cardBackMesh != null)
		{
			MeshFilter component = go.GetComponent<MeshFilter>();
			if (component != null)
			{
				component.mesh = cardBackMesh;
			}
		}
		else
		{
			Debug.LogWarning("CardBackManager SetCardBack() mesh=null");
		}
		float num = 0f;
		if (!unlit && SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
		{
			num = 1f;
		}
		Material cardBackMaterial = cardBack.m_CardBackMaterial;
		Material cardBackMaterial2 = cardBack.m_CardBackMaterial1;
		if (cardBackMaterial != null)
		{
			int num2 = 1;
			if (cardBackMaterial2 != null)
			{
				num2 = 2;
			}
			Material[] array = new Material[num2];
			array[0] = Object.Instantiate<Material>(cardBackMaterial);
			if (cardBackMaterial2 != null)
			{
				array[1] = Object.Instantiate<Material>(cardBackMaterial2);
			}
			float num3 = Random.Range(0f, 1f);
			foreach (Material material in array)
			{
				if (!(material == null))
				{
					if (material.HasProperty("_Seed") && material.GetFloat("_Seed") == 0f)
					{
						material.SetFloat("_Seed", num3);
					}
					if (material.HasProperty("_LightingBlend"))
					{
						material.SetFloat("_LightingBlend", num);
					}
				}
			}
			go.GetComponent<Renderer>().materials = array;
		}
		else
		{
			Debug.LogWarning("CardBackManager SetCardBack() material=null");
		}
		Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(go);
		if (actor != null)
		{
			actor.UpdateMissingCardArt();
		}
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x0004B8E0 File Offset: 0x00049AE0
	private IEnumerator SetDragEffectsWhenLoaded(GameObject go, bool friendlySide)
	{
		if (friendlySide)
		{
			while (this.m_isFriendlyLoading)
			{
				yield return null;
			}
		}
		else
		{
			while (this.m_isOpponentLoading)
			{
				yield return null;
			}
		}
		this.SetDragEffects(go, friendlySide);
		yield break;
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x0004B918 File Offset: 0x00049B18
	private void SetDragEffects(GameObject go, bool friendlySide)
	{
		if (go == null)
		{
			return;
		}
		CardBackDragEffect componentInChildren = go.GetComponentInChildren<CardBackDragEffect>();
		if (componentInChildren == null)
		{
			return;
		}
		if (friendlySide && this.m_FriendlyCardBack)
		{
			return;
		}
		CardBack cardBack = this.m_FriendlyCardBack;
		if (!friendlySide && this.m_OpponentCardBack)
		{
			return;
		}
		if (!friendlySide)
		{
			cardBack = this.m_OpponentCardBack;
		}
		if (componentInChildren.m_EffectsRoot != null)
		{
			Object.Destroy(componentInChildren.m_EffectsRoot);
		}
		if (cardBack == null || cardBack.m_DragEffect == null)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(cardBack.m_DragEffect);
		componentInChildren.m_EffectsRoot = gameObject;
		gameObject.transform.parent = componentInChildren.gameObject.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x0004BA1C File Offset: 0x00049C1C
	private IEnumerator SetDeckCardBackWhenLoaded(GameObject go, bool friendlySide)
	{
		if (friendlySide)
		{
			while (this.m_isFriendlyLoading)
			{
				yield return null;
			}
		}
		else
		{
			while (this.m_isOpponentLoading)
			{
				yield return null;
			}
		}
		this.SetDeckCardBack(go, friendlySide);
		yield break;
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x0004BA54 File Offset: 0x00049C54
	private void SetDeckCardBack(GameObject go, bool friendlySide)
	{
		if (friendlySide)
		{
			if (this.m_FriendlyCardBack == null)
			{
				return;
			}
		}
		else if (this.m_OpponentCardBack == null)
		{
			return;
		}
		Texture cardBackTexture = this.m_FriendlyCardBack.m_CardBackTexture;
		if (!friendlySide)
		{
			cardBackTexture = this.m_OpponentCardBack.m_CardBackTexture;
		}
		if (cardBackTexture == null)
		{
			Debug.LogWarning(string.Format("CardBackManager SetDeckCardBack(): texture is null!", new object[0]));
			return;
		}
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.material.mainTexture = cardBackTexture;
		}
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x0004BB04 File Offset: 0x00049D04
	private void OnCheatOptionChanged(Option option, object prevValue, bool existed, object userData)
	{
		Log.Kyle.Print("Cheat Option Change Called", new object[0]);
		int @int = Options.Get().GetInt(option, 0);
		if (!this.m_cardBackData.ContainsKey(@int))
		{
			return;
		}
		bool friendlySide = true;
		if (option == Option.CARD_BACK2)
		{
			friendlySide = false;
		}
		this.LoadCardBack(FileUtils.GetAssetPath(this.m_cardBackData[@int].PrefabName), friendlySide);
		this.UpdateAllCardBacks();
	}

	// Token: 0x06001187 RID: 4487 RVA: 0x0004BB74 File Offset: 0x00049D74
	private IEnumerator RegisterForDefaultCardBackChangesWhenPossible()
	{
		if (this.m_DefaultCardBackChangeListenerRegistered)
		{
			yield break;
		}
		while (CollectionManager.Get() == null)
		{
			yield return null;
		}
		this.m_DefaultCardBackChangeListenerRegistered = CollectionManager.Get().RegisterDefaultCardbackChangedListener(new CollectionManager.DefaultCardbackChangedCallback(this.OnDefaultCardBackChanged));
		yield break;
	}

	// Token: 0x06001188 RID: 4488 RVA: 0x0004BB90 File Offset: 0x00049D90
	private IEnumerator UpdateDefaultCardBackWhenReady()
	{
		NetCache.NetCacheCardBacks netCacheCardBacks;
		while ((netCacheCardBacks = NetCache.Get().GetNetObject<NetCache.NetCacheCardBacks>()) == null)
		{
			yield return null;
		}
		if (!this.m_cardBackData.ContainsKey(netCacheCardBacks.DefaultCardBack))
		{
			Log.CardbackMgr.Print("No cardback for {0}, set default to 0", new object[]
			{
				netCacheCardBacks.DefaultCardBack
			});
			netCacheCardBacks.DefaultCardBack = 0;
		}
		this.OnDefaultCardBackChanged(netCacheCardBacks.DefaultCardBack, null);
		yield break;
	}

	// Token: 0x06001189 RID: 4489 RVA: 0x0004BBAC File Offset: 0x00049DAC
	private IEnumerator UpdateAllCardBacksImpl()
	{
		while (this.m_isFriendlyLoading || this.m_isOpponentLoading)
		{
			yield return null;
		}
		Actor[] actors = Object.FindObjectsOfType(typeof(Actor)) as Actor[];
		foreach (Actor actor in actors)
		{
			actor.UpdateCardBack();
		}
		CardBackDisplay[] cbDisplays = Object.FindObjectsOfType(typeof(CardBackDisplay)) as CardBackDisplay[];
		foreach (CardBackDisplay cbd in cbDisplays)
		{
			cbd.UpdateCardBack();
		}
		CardBackDragEffect[] dragFx = Object.FindObjectsOfType(typeof(CardBackDragEffect)) as CardBackDragEffect[];
		foreach (CardBackDragEffect fx in dragFx)
		{
			fx.SetEffect();
		}
		CardBackDeckDisplay[] deckDisplays = Object.FindObjectsOfType(typeof(CardBackDeckDisplay)) as CardBackDeckDisplay[];
		foreach (CardBackDeckDisplay deck in deckDisplays)
		{
			deck.UpdateDeckCardBacks();
		}
		yield break;
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x0004BBC8 File Offset: 0x00049DC8
	private void LoadCardBack(string cardBackPath, bool friendlySide)
	{
		if (this.m_LoadedCardBacks.ContainsKey(cardBackPath))
		{
			if (!(this.m_LoadedCardBacks[cardBackPath] == null))
			{
				if (friendlySide)
				{
					this.m_FriendlyCardBack = this.m_LoadedCardBacks[cardBackPath];
				}
				else
				{
					this.m_OpponentCardBack = this.m_LoadedCardBacks[cardBackPath];
				}
				return;
			}
			this.m_LoadedCardBacks.Remove(cardBackPath);
		}
		if (friendlySide)
		{
			if (this.m_FriendlyCardBackName == cardBackPath)
			{
				return;
			}
			this.m_isFriendlyLoading = true;
			this.m_FriendlyCardBackName = cardBackPath;
			this.m_FriendlyCardBack = null;
		}
		else
		{
			if (this.m_OpponentCardBackName == cardBackPath)
			{
				return;
			}
			this.m_isOpponentLoading = true;
			this.m_OpponentCardBackName = cardBackPath;
			this.m_OpponentCardBack = null;
		}
		CardBackManager.LoadCardBackData loadCardBackData = new CardBackManager.LoadCardBackData();
		loadCardBackData.m_FriendlySide = friendlySide;
		loadCardBackData.m_Path = cardBackPath;
		AssetLoader.Get().LoadCardBack(FileUtils.GameAssetPathToName(cardBackPath), new AssetLoader.GameObjectCallback(this.OnCardBackLoaded), loadCardBackData, false);
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x0004BCCC File Offset: 0x00049ECC
	private void OnCardBackLoaded(string name, GameObject go, object callbackData)
	{
		CardBackManager.LoadCardBackData loadCardBackData = callbackData as CardBackManager.LoadCardBackData;
		if (go == null)
		{
			Debug.LogWarning(string.Format("CardBackManager OnCardBackLoaded(): Failed to load CardBack: {0}", name));
			this.FailedLoad(loadCardBackData.m_FriendlySide);
			return;
		}
		go.transform.parent = base.transform;
		go.transform.position = new Vector3(1000f, -1000f, -1000f);
		CardBack component = go.GetComponent<CardBack>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("CardBackManager OnCardBackLoaded(): Failed to find CardBack component: {0}", loadCardBackData.m_Path));
			return;
		}
		if (component.m_CardBackMesh == null)
		{
			Debug.LogWarning(string.Format("CardBackManager OnCardBackLoaded(): cardBack.m_CardBackMesh in null! - {0}", loadCardBackData.m_Path));
			return;
		}
		if (component.m_CardBackMaterial == null)
		{
			Debug.LogWarning(string.Format("CardBackManager OnCardBackLoaded(): cardBack.m_CardBackMaterial in null! - {0}", loadCardBackData.m_Path));
			return;
		}
		if (component.m_CardBackTexture == null)
		{
			Debug.LogWarning(string.Format("CardBackManager OnCardBackLoaded(): cardBack.m_CardBackTexture in null! - {0}", loadCardBackData.m_Path));
			return;
		}
		this.m_LoadedCardBacks[loadCardBackData.m_Path] = component;
		if (loadCardBackData.m_FriendlySide)
		{
			this.m_isFriendlyLoading = false;
			if (component == null)
			{
				Debug.LogError(string.Format("CardBackManager OnCardBackLoaded(): Failed to find CardBack component for: {0}", name));
			}
			this.m_FriendlyCardBack = component;
		}
		else
		{
			this.m_isOpponentLoading = false;
			if (component == null)
			{
				Debug.LogError(string.Format("CardBackManager OnCardBackLoaded(): Failed to find CardBack component for: {0}", name));
			}
			this.m_OpponentCardBack = component;
		}
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x0004BE50 File Offset: 0x0004A050
	private void LoadDefaultCardBack()
	{
		CardBackManager.LoadCardBackData loadCardBackData = new CardBackManager.LoadCardBackData();
		loadCardBackData.m_FriendlySide = true;
		loadCardBackData.m_Path = this.m_cardBackData[0].PrefabName;
		AssetLoader.Get().LoadCardBack(FileUtils.GameAssetPathToName(loadCardBackData.m_Path), new AssetLoader.GameObjectCallback(this.OnDefaultCardBackLoaded), loadCardBackData, false);
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x0004BEA8 File Offset: 0x0004A0A8
	private void OnDefaultCardBackLoaded(string name, GameObject go, object callbackData)
	{
		CardBackManager.LoadCardBackData loadCardBackData = callbackData as CardBackManager.LoadCardBackData;
		if (go == null)
		{
			Debug.LogWarning(string.Format("CardBackManager OnDefaultCardBackLoaded(): Failed to load CardBack: {0}", name));
			return;
		}
		go.transform.parent = base.transform;
		go.transform.position = new Vector3(1000f, -1000f, -1000f);
		CardBack component = go.GetComponent<CardBack>();
		this.m_LoadedCardBacks[loadCardBackData.m_Path] = component;
		this.m_DefaultCardBack = component;
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x0004BF29 File Offset: 0x0004A129
	private void FailedLoad(bool friendlySide)
	{
		if (friendlySide)
		{
			this.m_FriendlyCardBack = null;
			this.m_FriendlyCardBackName = string.Empty;
			this.m_isFriendlyLoading = false;
		}
		else
		{
			this.m_OpponentCardBack = null;
			this.m_OpponentCardBackName = string.Empty;
			this.m_isOpponentLoading = false;
		}
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x0004BF68 File Offset: 0x0004A168
	private void OnHiddenActorLoaded(string name, GameObject go, object userData)
	{
		CardBackManager.LoadCardBackData loadCardBackData = (CardBackManager.LoadCardBackData)userData;
		int cardBackIndex = loadCardBackData.m_CardBackIndex;
		string prefabName = this.m_cardBackData[cardBackIndex].PrefabName;
		loadCardBackData.m_GameObject = go;
		AssetLoader.Get().LoadCardBack(FileUtils.GameAssetPathToName(prefabName), new AssetLoader.GameObjectCallback(this.OnHiddenActorCardBackLoaded), userData, false);
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x0004BFBC File Offset: 0x0004A1BC
	private void OnHiddenActorCardBackLoaded(string name, GameObject go, object userData)
	{
		CardBack componentInChildren = go.GetComponentInChildren<CardBack>();
		if (componentInChildren == null)
		{
			Debug.LogWarningFormat("CardBackManager OnHiddenActorCardBackLoaded() name={0}, gameobject={1}, cardback=null", new object[]
			{
				name,
				go.name
			});
			return;
		}
		CardBackManager.LoadCardBackData loadCardBackData = (CardBackManager.LoadCardBackData)userData;
		loadCardBackData.m_CardBack = componentInChildren;
		base.StartCoroutine(this.HiddenActorCardBackLoadedSetup(loadCardBackData));
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x0004C018 File Offset: 0x0004A218
	private IEnumerator HiddenActorCardBackLoadedSetup(CardBackManager.LoadCardBackData data)
	{
		yield return null;
		yield return null;
		Actor actor = data.m_GameObject.GetComponent<Actor>();
		this.SetCardBack(actor.m_cardMesh, data.m_CardBack, data.m_Unlit);
		data.m_CardBack.gameObject.transform.parent = data.m_GameObject.transform;
		data.m_Callback(data);
		yield break;
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x0004C044 File Offset: 0x0004A244
	private int GetValidCardBackID(int cardBackID)
	{
		if (!this.m_cardBackData.ContainsKey(cardBackID))
		{
			Log.CardbackMgr.Print("No cardback for {0}, use 0 instead", new object[]
			{
				cardBackID
			});
			return 0;
		}
		return cardBackID;
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x0004C084 File Offset: 0x0004A284
	private void OnDefaultCardBackChanged(int defaultCardBackID, object userData)
	{
		int validCardBackID = this.GetValidCardBackID(defaultCardBackID);
		bool flag = false;
		if (GameMgr.Get() != null && GameMgr.Get().IsSpectator())
		{
			flag = true;
		}
		if (!flag)
		{
			this.LoadCardBack(FileUtils.GameAssetPathToName(this.m_cardBackData[validCardBackID].PrefabName), true);
			this.UpdateAllCardBacks();
		}
	}

	// Token: 0x06001194 RID: 4500 RVA: 0x0004C0E0 File Offset: 0x0004A2E0
	private void OnSceneChangeResetDefaultCardBack(SceneMgr.Mode mode, Scene scene, object userData)
	{
		if (SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY)
		{
			return;
		}
		this.LoadCardBack(FileUtils.GameAssetPathToName(this.m_cardBackData[this.GetDefaultCardBackID()].PrefabName), true);
	}

	// Token: 0x04000941 RID: 2369
	private CardBack m_DefaultCardBack;

	// Token: 0x04000942 RID: 2370
	private CardBack m_FriendlyCardBack;

	// Token: 0x04000943 RID: 2371
	private string m_FriendlyCardBackName = string.Empty;

	// Token: 0x04000944 RID: 2372
	private bool m_isFriendlyLoading;

	// Token: 0x04000945 RID: 2373
	private CardBack m_OpponentCardBack;

	// Token: 0x04000946 RID: 2374
	private string m_OpponentCardBackName = string.Empty;

	// Token: 0x04000947 RID: 2375
	private bool m_isOpponentLoading;

	// Token: 0x04000948 RID: 2376
	private Map<int, CardBackData> m_cardBackData;

	// Token: 0x04000949 RID: 2377
	private Map<string, CardBack> m_LoadedCardBacks;

	// Token: 0x0400094A RID: 2378
	private bool m_ResetDefaultRegistered;

	// Token: 0x0400094B RID: 2379
	private bool m_DefaultCardBackChangeListenerRegistered;

	// Token: 0x0400094C RID: 2380
	private static CardBackManager s_instance;

	// Token: 0x02000448 RID: 1096
	public class LoadCardBackData
	{
		// Token: 0x040021FF RID: 8703
		public int m_CardBackIndex;

		// Token: 0x04002200 RID: 8704
		public GameObject m_GameObject;

		// Token: 0x04002201 RID: 8705
		public CardBack m_CardBack;

		// Token: 0x04002202 RID: 8706
		public CardBackManager.LoadCardBackData.LoadCardBackCallback m_Callback;

		// Token: 0x04002203 RID: 8707
		public string m_Name;

		// Token: 0x04002204 RID: 8708
		public string m_Path;

		// Token: 0x04002205 RID: 8709
		public bool m_FriendlySide;

		// Token: 0x04002206 RID: 8710
		public bool m_Unlit;

		// Token: 0x02000449 RID: 1097
		// (Invoke) Token: 0x060036A1 RID: 13985
		public delegate void LoadCardBackCallback(CardBackManager.LoadCardBackData cardBackData);
	}

	// Token: 0x02000458 RID: 1112
	public class OwnedCardBack
	{
		// Token: 0x04002244 RID: 8772
		public int m_cardBackId;

		// Token: 0x04002245 RID: 8773
		public bool m_owned;

		// Token: 0x04002246 RID: 8774
		public int m_sortOrder;

		// Token: 0x04002247 RID: 8775
		public int m_sortCategory;

		// Token: 0x04002248 RID: 8776
		public long m_seasonId = -1L;
	}
}
