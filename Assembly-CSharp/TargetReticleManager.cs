using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200065A RID: 1626
public class TargetReticleManager : MonoBehaviour
{
	// Token: 0x06004595 RID: 17813 RVA: 0x0014E0DA File Offset: 0x0014C2DA
	private void Awake()
	{
		TargetReticleManager.s_instance = this;
	}

	// Token: 0x06004596 RID: 17814 RVA: 0x0014E0E2 File Offset: 0x0014C2E2
	private void OnDestroy()
	{
		TargetReticleManager.s_instance = null;
	}

	// Token: 0x06004597 RID: 17815 RVA: 0x0014E0EA File Offset: 0x0014C2EA
	public static TargetReticleManager Get()
	{
		return TargetReticleManager.s_instance;
	}

	// Token: 0x06004598 RID: 17816 RVA: 0x0014E0F4 File Offset: 0x0014C2F4
	public bool IsActive()
	{
		if (this.m_ReticleType == TARGET_RETICLE_TYPE.DefaultArrow)
		{
			return this.m_arrow != null && this.m_isActive;
		}
		if (this.m_ReticleType == TARGET_RETICLE_TYPE.HunterReticle)
		{
			return this.m_hunterReticle != null && this.m_isActive;
		}
		Debug.LogError("Unknown Target Reticle Type!");
		return false;
	}

	// Token: 0x06004599 RID: 17817 RVA: 0x0014E159 File Offset: 0x0014C359
	public bool IsLocalArrow()
	{
		return !this.m_isEnemyArrow;
	}

	// Token: 0x0600459A RID: 17818 RVA: 0x0014E164 File Offset: 0x0014C364
	public bool IsEnemyArrow()
	{
		return this.m_isEnemyArrow;
	}

	// Token: 0x0600459B RID: 17819 RVA: 0x0014E16C File Offset: 0x0014C36C
	public bool IsLocalArrowActive()
	{
		return !this.m_isEnemyArrow && this.IsActive();
	}

	// Token: 0x0600459C RID: 17820 RVA: 0x0014E181 File Offset: 0x0014C381
	public bool IsEnemyArrowActive()
	{
		return this.m_isEnemyArrow && this.IsActive();
	}

	// Token: 0x170004BD RID: 1213
	// (get) Token: 0x0600459D RID: 17821 RVA: 0x0014E196 File Offset: 0x0014C396
	public int ArrowSourceEntityID
	{
		get
		{
			return this.m_originLocationEntityID;
		}
	}

	// Token: 0x0600459E RID: 17822 RVA: 0x0014E1A0 File Offset: 0x0014C3A0
	public void ShowBullseye(bool show)
	{
		if (this.m_ReticleType == TARGET_RETICLE_TYPE.DefaultArrow)
		{
			if (this.m_ReticleType != TARGET_RETICLE_TYPE.DefaultArrow)
			{
				return;
			}
			if (!this.IsActive() || !this.m_showArrow)
			{
				return;
			}
			Transform transform = this.m_arrow.transform.FindChild("TargetArrow_TargetMesh");
			if (!transform)
			{
				return;
			}
			SceneUtils.EnableRenderers(transform.gameObject, show);
		}
		else if (this.m_ReticleType == TARGET_RETICLE_TYPE.HunterReticle)
		{
			if (this.m_hunterReticle == null)
			{
				return;
			}
			RenderToTexture component = this.m_hunterReticle.GetComponent<RenderToTexture>();
			if (component == null)
			{
				return;
			}
			Material renderMaterial = component.GetRenderMaterial();
			if (renderMaterial == null)
			{
				return;
			}
			if (show)
			{
				renderMaterial.color = Color.red;
			}
			else
			{
				renderMaterial.color = Color.white;
			}
		}
	}

	// Token: 0x0600459F RID: 17823 RVA: 0x0014E27C File Offset: 0x0014C47C
	public void CreateFriendlyTargetArrow(Entity originLocationEntity, Entity sourceEntity, bool showDamageIndicatorText, bool showArrow = true, string overrideText = null, bool useHandAsOrigin = false)
	{
		if (GameMgr.Get() == null || !GameMgr.Get().IsSpectator())
		{
			this.DisableCollidersForUntargetableCards(sourceEntity.GetCard());
		}
		Spell playSpell = sourceEntity.GetCard().GetPlaySpell(true);
		if (playSpell != null)
		{
			this.m_ReticleType = playSpell.m_TargetReticle;
		}
		else
		{
			this.m_ReticleType = TARGET_RETICLE_TYPE.DefaultArrow;
		}
		string damageIndicatorText = null;
		if (overrideText != null)
		{
			damageIndicatorText = overrideText;
		}
		else if (showDamageIndicatorText)
		{
			damageIndicatorText = sourceEntity.GetTargetingArrowText();
		}
		this.CreateTargetArrow(false, originLocationEntity.GetEntityId(), sourceEntity.GetEntityId(), damageIndicatorText, showArrow, useHandAsOrigin);
	}

	// Token: 0x060045A0 RID: 17824 RVA: 0x0014E31C File Offset: 0x0014C51C
	public void CreateEnemyTargetArrow(Entity originEntity)
	{
		this.m_ReticleType = TARGET_RETICLE_TYPE.DefaultArrow;
		this.CreateTargetArrow(true, originEntity.GetEntityId(), originEntity.GetEntityId(), null, true, false);
	}

	// Token: 0x060045A1 RID: 17825 RVA: 0x0014E346 File Offset: 0x0014C546
	public void DestroyEnemyTargetArrow()
	{
		this.DestroyTargetArrow(true, false);
	}

	// Token: 0x060045A2 RID: 17826 RVA: 0x0014E350 File Offset: 0x0014C550
	public void DestroyFriendlyTargetArrow(bool isLocallyCanceled)
	{
		this.EnableCollidersThatWereDisabled();
		this.DestroyTargetArrow(false, isLocallyCanceled);
	}

	// Token: 0x060045A3 RID: 17827 RVA: 0x0014E360 File Offset: 0x0014C560
	public void UpdateArrowPosition()
	{
		if (!this.IsActive())
		{
			return;
		}
		if (!this.m_showArrow)
		{
			this.UpdateArrowOriginPosition();
			this.UpdateDamageIndicator();
			return;
		}
		float num = 0f;
		bool flag = GameMgr.Get() != null && GameMgr.Get().IsSpectator();
		Vector3 point;
		if (this.m_isEnemyArrow || flag)
		{
			Vector3 vector = Vector3.zero;
			if (this.m_ReticleType == TARGET_RETICLE_TYPE.DefaultArrow)
			{
				vector = this.m_arrow.transform.position;
			}
			else if (this.m_ReticleType == TARGET_RETICLE_TYPE.HunterReticle)
			{
				vector = this.m_hunterReticle.transform.position;
			}
			else
			{
				Debug.LogError("Unknown Target Reticle Type!");
			}
			point.x = Mathf.Lerp(vector.x, this.m_remoteArrowPosition.x, 0.1f);
			point.y = Mathf.Lerp(vector.y, this.m_remoteArrowPosition.y, 0.1f);
			point.z = Mathf.Lerp(vector.z, this.m_remoteArrowPosition.z, 0.1f);
			Card card = (!this.m_isEnemyArrow) ? RemoteActionHandler.Get().GetFriendlyHeldCard() : RemoteActionHandler.Get().GetOpponentHeldCard();
			if (card != null)
			{
				this.m_targetArrowOrigin = card.transform.position;
			}
		}
		else
		{
			RaycastHit raycastHit;
			if (!UniversalInputManager.Get().GetInputHitInfo(Camera.main, GameLayer.DragPlane, out raycastHit))
			{
				return;
			}
			point = raycastHit.point;
			this.UpdateArrowOriginPosition();
		}
		if (!object.Equals(point.z - this.m_targetArrowOrigin.z, 0f))
		{
			float num2 = Mathf.Atan((point.x - this.m_targetArrowOrigin.x) / (point.z - this.m_targetArrowOrigin.z));
			num = 57.29578f * num2;
		}
		if (point.z < this.m_targetArrowOrigin.z)
		{
			num -= 180f;
		}
		if (this.m_ReticleType == TARGET_RETICLE_TYPE.DefaultArrow)
		{
			this.m_arrow.transform.localEulerAngles = new Vector3(0f, num, 0f);
			this.m_arrow.transform.position = point;
			float num3 = Mathf.Pow(this.m_targetArrowOrigin.x - point.x, 2f);
			float num4 = Mathf.Pow(this.m_targetArrowOrigin.z - point.z, 2f);
			float lengthOfArrow = Mathf.Sqrt(num3 + num4);
			this.UpdateTargetArrowLinks(lengthOfArrow);
		}
		else if (this.m_ReticleType == TARGET_RETICLE_TYPE.HunterReticle)
		{
			this.m_hunterReticle.transform.position = point;
		}
		else
		{
			Debug.LogError("Unknown Target Reticle Type!");
		}
		this.UpdateDamageIndicator();
	}

	// Token: 0x060045A4 RID: 17828 RVA: 0x0014E634 File Offset: 0x0014C834
	public void SetRemotePlayerArrowPosition(Vector3 newPosition)
	{
		this.m_remoteArrowPosition = newPosition;
	}

	// Token: 0x060045A5 RID: 17829 RVA: 0x0014E63D File Offset: 0x0014C83D
	private void DestroyCurrentArrow(bool isLocallyCanceled)
	{
		if (this.m_isEnemyArrow)
		{
			this.DestroyEnemyTargetArrow();
		}
		else
		{
			this.DestroyFriendlyTargetArrow(isLocallyCanceled);
		}
	}

	// Token: 0x060045A6 RID: 17830 RVA: 0x0014E65C File Offset: 0x0014C85C
	private void DisableCollidersForUntargetableCards(Card sourceCard)
	{
		List<Card> list = new List<Card>();
		Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
		foreach (Player player in playerMap.Values)
		{
			this.AddUntargetableCard(sourceCard, list, player.GetHeroPowerCard());
			this.AddUntargetableCard(sourceCard, list, player.GetWeaponCard());
			ZoneSecret secretZone = player.GetSecretZone();
			foreach (Card card in secretZone.GetCards())
			{
				this.AddUntargetableCard(sourceCard, list, card);
			}
		}
		foreach (Card card2 in list)
		{
			if (!(card2 == null))
			{
				Actor actor = card2.GetActor();
				if (!(actor == null))
				{
					actor.TurnOffCollider();
				}
			}
		}
	}

	// Token: 0x060045A7 RID: 17831 RVA: 0x0014E7A8 File Offset: 0x0014C9A8
	private void AddUntargetableCard(Card sourceCard, List<Card> cards, Card card)
	{
		if (sourceCard == card)
		{
			return;
		}
		cards.Add(card);
	}

	// Token: 0x060045A8 RID: 17832 RVA: 0x0014E7C0 File Offset: 0x0014C9C0
	private void EnableCollidersThatWereDisabled()
	{
		List<Card> list = new List<Card>();
		Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
		foreach (Player player in playerMap.Values)
		{
			list.Add(player.GetHeroPowerCard());
			list.Add(player.GetWeaponCard());
			ZoneSecret secretZone = player.GetSecretZone();
			foreach (Card card in secretZone.GetCards())
			{
				list.Add(card);
			}
		}
		foreach (Card card2 in list)
		{
			if (!(card2 == null))
			{
				Actor actor = card2.GetActor();
				if (!(actor == null))
				{
					card2.GetActor().TurnOnCollider();
				}
			}
		}
	}

	// Token: 0x060045A9 RID: 17833 RVA: 0x0014E90C File Offset: 0x0014CB0C
	private void CreateTargetArrow(bool isEnemyArrow, int originLocationEntityID, int sourceEntityID, string damageIndicatorText, bool showArrow, bool useHandAsOrigin = false)
	{
		if (this.IsActive())
		{
			Log.Rachelle.Print("Uh-oh... creating a targeting arrow but one is already active...", new object[0]);
			this.DestroyCurrentArrow(false);
		}
		this.m_isEnemyArrow = isEnemyArrow;
		this.m_sourceEntityID = sourceEntityID;
		this.m_originLocationEntityID = originLocationEntityID;
		this.m_showArrow = showArrow;
		this.m_useHandAsOrigin = useHandAsOrigin;
		this.UpdateArrowOriginPosition();
		bool flag = GameMgr.Get() != null && GameMgr.Get().IsSpectator();
		if (this.m_isEnemyArrow || flag)
		{
			this.m_remoteArrowPosition = this.m_targetArrowOrigin;
			this.m_arrow.transform.position = this.m_targetArrowOrigin;
		}
		this.ActivateArrow(true);
		this.ShowBullseye(false);
		this.ShowDamageIndicator(!this.m_isEnemyArrow);
		this.UpdateArrowPosition();
		if (!this.m_isEnemyArrow)
		{
			base.StartCoroutine(this.SetDamageText(damageIndicatorText));
			if (!flag)
			{
				PegCursor.Get().Hide();
			}
		}
	}

	// Token: 0x060045AA RID: 17834 RVA: 0x0014EA04 File Offset: 0x0014CC04
	public void PreloadTargetArrows()
	{
		this.m_targetArrowLinks = new List<GameObject>();
		AssetLoader.Get().LoadActor("Target_Arrow_Bullseye", new AssetLoader.GameObjectCallback(this.LoadArrowCallback), null, false);
		AssetLoader.Get().LoadActor("TargetDamageIndicator", new AssetLoader.GameObjectCallback(this.LoadDamageIndicatorCallback), null, false);
		AssetLoader.Get().LoadActor("Target_Arrow_Link", new AssetLoader.GameObjectCallback(this.LoadLinkCallback), null, false);
		AssetLoader.Get().LoadActor("HunterReticle", new AssetLoader.GameObjectCallback(this.LoadHunterReticleCallback), null, false);
	}

	// Token: 0x060045AB RID: 17835 RVA: 0x0014EA94 File Offset: 0x0014CC94
	private void DestroyTargetArrow(bool destroyEnemyArrow, bool isLocallyCanceled)
	{
		if (!this.IsActive())
		{
			return;
		}
		if (destroyEnemyArrow != this.m_isEnemyArrow)
		{
			Log.Rachelle.Print(string.Format("trying to destroy {0} arrow but the active arrow is {1}", (!destroyEnemyArrow) ? "friendly" : "enemy", (!this.m_isEnemyArrow) ? "friendly" : "enemy"), new object[0]);
			return;
		}
		if (isLocallyCanceled)
		{
			Entity entity = GameState.Get().GetEntity(this.m_sourceEntityID);
			if (entity != null)
			{
				Card card = entity.GetCard();
				card.NotifyTargetingCanceled();
			}
		}
		this.m_originLocationEntityID = -1;
		this.m_sourceEntityID = -1;
		if (!this.m_isEnemyArrow)
		{
			RemoteActionHandler.Get().NotifyOpponentOfTargetEnd();
			PegCursor.Get().Show();
		}
		this.ActivateArrow(false);
		this.ShowDamageIndicator(false);
	}

	// Token: 0x060045AC RID: 17836 RVA: 0x0014EB68 File Offset: 0x0014CD68
	private void LoadArrowCallback(string actorName, GameObject actorObject, object callbackData)
	{
		this.m_arrow = actorObject;
		SceneUtils.EnableRenderers(this.m_arrow.gameObject, false);
		this.ShowBullseye(false);
	}

	// Token: 0x060045AD RID: 17837 RVA: 0x0014EB89 File Offset: 0x0014CD89
	private void LoadLinkCallback(string actorName, GameObject actorObject, object callbackData)
	{
		base.StartCoroutine(this.OnLinkLoaded(actorObject));
	}

	// Token: 0x060045AE RID: 17838 RVA: 0x0014EB9C File Offset: 0x0014CD9C
	private void LoadDamageIndicatorCallback(string actorName, GameObject actorObject, object callbackData)
	{
		this.m_damageIndicator = actorObject;
		this.m_damageIndicator.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		this.m_damageIndicator.transform.localScale = new Vector3(TargetReticleManager.DAMAGE_INDICATOR_SCALE, TargetReticleManager.DAMAGE_INDICATOR_SCALE, TargetReticleManager.DAMAGE_INDICATOR_SCALE);
		this.ShowDamageIndicator(false);
	}

	// Token: 0x060045AF RID: 17839 RVA: 0x0014EC10 File Offset: 0x0014CE10
	private void LoadHunterReticleCallback(string actorName, GameObject actorObject, object callbackData)
	{
		this.m_hunterReticle = actorObject;
		this.m_hunterReticle.transform.parent = base.transform;
		this.m_hunterReticle.SetActive(false);
	}

	// Token: 0x060045B0 RID: 17840 RVA: 0x0014EC48 File Offset: 0x0014CE48
	private IEnumerator OnLinkLoaded(GameObject linkActorObject)
	{
		while (this.m_arrow == null)
		{
			yield return null;
		}
		for (int i = 0; i < 14; i++)
		{
			GameObject newLink = Object.Instantiate<GameObject>(linkActorObject);
			newLink.transform.parent = this.m_arrow.transform;
			this.m_targetArrowLinks.Add(newLink);
		}
		linkActorObject.transform.parent = this.m_arrow.transform;
		this.m_targetArrowLinks.Add(linkActorObject);
		SceneUtils.EnableRenderers(this.m_arrow.gameObject, false);
		yield break;
	}

	// Token: 0x060045B1 RID: 17841 RVA: 0x0014EC74 File Offset: 0x0014CE74
	private int NumberOfRequiredLinks(float lengthOfArrow)
	{
		int num = (int)Mathf.Floor(lengthOfArrow / 1.2f) + 1;
		if (num == 1)
		{
			num = 0;
		}
		return num;
	}

	// Token: 0x060045B2 RID: 17842 RVA: 0x0014EC9C File Offset: 0x0014CE9C
	private void UpdateTargetArrowLinks(float lengthOfArrow)
	{
		this.m_numActiveLinks = this.NumberOfRequiredLinks(lengthOfArrow);
		int count = this.m_targetArrowLinks.Count;
		Transform transform = this.m_arrow.transform.FindChild("TargetArrow_ArrowMesh");
		if (this.m_numActiveLinks == 0)
		{
			transform.localEulerAngles = new Vector3(300f, 180f, 0f);
			for (int i = 0; i < count; i++)
			{
				SceneUtils.EnableRenderers(this.m_targetArrowLinks[i].gameObject, false);
			}
			return;
		}
		float num = -lengthOfArrow / 2f;
		float num2 = -1.5f / (num * num);
		for (int j = 0; j < count; j++)
		{
			if (!(this.m_targetArrowLinks[j] == null))
			{
				if (j >= this.m_numActiveLinks)
				{
					SceneUtils.EnableRenderers(this.m_targetArrowLinks[j].gameObject, false);
				}
				else
				{
					float num3 = -(1.2f * (float)(j + 1)) + this.m_linkAnimationZOffset;
					float num4 = num2 * Mathf.Pow(num3 - num, 2f) + 1.5f;
					float num5 = 2f * num2 * (num3 - num);
					float num6 = Mathf.Atan(num5);
					float num7 = 180f - num6 * 57.29578f;
					SceneUtils.EnableRenderers(this.m_targetArrowLinks[j].gameObject, true);
					this.m_targetArrowLinks[j].transform.localPosition = new Vector3(0f, num4, num3);
					this.m_targetArrowLinks[j].transform.eulerAngles = new Vector3(num7, this.m_arrow.transform.localEulerAngles.y, 0f);
					float num8 = 1f;
					if (j == 0)
					{
						if (num3 > -1.2f)
						{
							num8 = num3 / -1.2f;
							num8 = Mathf.Pow(num8, 6f);
						}
					}
					else if (j == this.m_numActiveLinks - 1)
					{
						num8 = this.m_linkAnimationZOffset / 1.2f;
						num8 *= num8;
					}
					this.SetLinkAlpha(this.m_targetArrowLinks[j], num8);
				}
			}
		}
		float num9 = num2 * Mathf.Pow(transform.localPosition.z - num, 2f) + 1.5f;
		float num10 = 2f * num2 * (transform.localPosition.z - num);
		float num11 = Mathf.Atan(num10);
		float num12 = num11 * 57.29578f;
		if (num12 < 0f)
		{
			num12 += 360f;
		}
		transform.localPosition = new Vector3(0f, num9, transform.localPosition.z);
		transform.localEulerAngles = new Vector3(num12, 180f, 0f);
		this.m_linkAnimationZOffset += Time.deltaTime * 0.5f;
		if (this.m_linkAnimationZOffset > 1.2f)
		{
			this.m_linkAnimationZOffset -= 1.2f;
		}
	}

	// Token: 0x060045B3 RID: 17843 RVA: 0x0014EFB4 File Offset: 0x0014D1B4
	private void SetLinkAlpha(GameObject linkGameObject, float alpha)
	{
		alpha = Mathf.Clamp(alpha, 0f, 1f);
		Renderer[] components = linkGameObject.GetComponents<Renderer>();
		foreach (Renderer renderer in components)
		{
			Color color = renderer.material.color;
			color.a = alpha;
			renderer.material.color = color;
		}
	}

	// Token: 0x060045B4 RID: 17844 RVA: 0x0014F018 File Offset: 0x0014D218
	private void UpdateDamageIndicator()
	{
		if (this.m_damageIndicator == null)
		{
			return;
		}
		Vector3 position = Vector3.zero;
		if (TargetReticleManager.SHOW_DAMAGE_INDICATOR_ON_ENTITY)
		{
			position = this.m_targetArrowOrigin;
			position.z += TargetReticleManager.DAMAGE_INDICATOR_Z_OFFSET;
		}
		else
		{
			if (this.m_ReticleType == TARGET_RETICLE_TYPE.DefaultArrow)
			{
				position = this.m_arrow.transform.position;
			}
			else if (this.m_ReticleType == TARGET_RETICLE_TYPE.HunterReticle)
			{
				position = this.m_hunterReticle.transform.position;
			}
			else
			{
				Debug.LogError("Unknown Target Reticle Type!");
			}
			position.z += TargetReticleManager.DAMAGE_INDICATOR_Z_OFFSET;
		}
		this.m_damageIndicator.transform.position = position;
	}

	// Token: 0x060045B5 RID: 17845 RVA: 0x0014F0E6 File Offset: 0x0014D2E6
	private void ShowDamageIndicator(bool show)
	{
		if (!this.m_damageIndicator || !this.m_damageIndicator.activeInHierarchy)
		{
			return;
		}
		SceneUtils.EnableRenderers(this.m_damageIndicator.gameObject, show);
	}

	// Token: 0x060045B6 RID: 17846 RVA: 0x0014F11C File Offset: 0x0014D31C
	private IEnumerator SetDamageText(string damageText)
	{
		while (this.m_damageIndicator == null)
		{
			yield return null;
		}
		if (string.IsNullOrEmpty(damageText))
		{
			this.ShowDamageIndicator(false);
			yield break;
		}
		UberText damageTextMesh = this.m_damageIndicator.transform.GetComponentInChildren<UberText>();
		damageTextMesh.Text = damageText;
		yield break;
	}

	// Token: 0x060045B7 RID: 17847 RVA: 0x0014F148 File Offset: 0x0014D348
	private void UpdateArrowOriginPosition()
	{
		Entity entity = GameState.Get().GetEntity(this.m_originLocationEntityID);
		if (entity == null)
		{
			Log.Rachelle.Print(string.Format("entity with ID {0} does not exist... can't update arrow origin position!", this.m_originLocationEntityID), new object[0]);
			this.DestroyCurrentArrow(false);
			return;
		}
		this.m_targetArrowOrigin = entity.GetCard().transform.position;
		if (this.m_useHandAsOrigin)
		{
			this.m_targetArrowOrigin = InputManager.Get().GetFriendlyHand().transform.position;
		}
		if (entity.IsHero() && !this.m_isEnemyArrow)
		{
			this.m_targetArrowOrigin.z = this.m_targetArrowOrigin.z + 1f;
		}
	}

	// Token: 0x060045B8 RID: 17848 RVA: 0x0014F204 File Offset: 0x0014D404
	private void ActivateArrow(bool active)
	{
		this.m_isActive = active;
		SceneUtils.EnableRenderers(this.m_arrow.gameObject, false);
		this.m_hunterReticle.SetActive(false);
		if (!active)
		{
			return;
		}
		if (this.m_ReticleType == TARGET_RETICLE_TYPE.DefaultArrow)
		{
			SceneUtils.EnableRenderers(this.m_arrow.gameObject, active && this.m_showArrow);
		}
		else if (this.m_ReticleType == TARGET_RETICLE_TYPE.HunterReticle)
		{
			this.m_hunterReticle.SetActive(active && this.m_showArrow);
		}
		else
		{
			Debug.LogError("Unknown Target Reticle Type!");
		}
	}

	// Token: 0x060045B9 RID: 17849 RVA: 0x0014F2A0 File Offset: 0x0014D4A0
	public void ShowArrow(bool show)
	{
		this.m_showArrow = show;
		SceneUtils.EnableRenderers(this.m_arrow.gameObject, false);
		this.m_hunterReticle.SetActive(false);
		if (!show)
		{
			return;
		}
		if (this.m_ReticleType == TARGET_RETICLE_TYPE.DefaultArrow)
		{
			SceneUtils.EnableRenderers(this.m_arrow.gameObject, show);
		}
		else if (this.m_ReticleType == TARGET_RETICLE_TYPE.HunterReticle)
		{
			this.m_hunterReticle.SetActive(show);
		}
		else
		{
			Debug.LogError("Unknown Target Reticle Type!");
		}
	}

	// Token: 0x04002C8D RID: 11405
	private const int MAX_TARGET_ARROW_LINKS = 15;

	// Token: 0x04002C8E RID: 11406
	private const float LINK_Y_LENGTH = 1f;

	// Token: 0x04002C8F RID: 11407
	private const float LENGTH_BETWEEN_LINKS = 1.2f;

	// Token: 0x04002C90 RID: 11408
	private const float LINK_PARABOLA_HEIGHT = 1.5f;

	// Token: 0x04002C91 RID: 11409
	private const float LINK_ANIMATION_SPEED = 0.5f;

	// Token: 0x04002C92 RID: 11410
	private const float FRIENDLY_HERO_ORIGIN_Z_OFFSET = 1f;

	// Token: 0x04002C93 RID: 11411
	private const float LINK_FADE_OFFSET = -1.2f;

	// Token: 0x04002C94 RID: 11412
	private TARGET_RETICLE_TYPE m_ReticleType;

	// Token: 0x04002C95 RID: 11413
	private static readonly PlatformDependentValue<bool> SHOW_DAMAGE_INDICATOR_ON_ENTITY = new PlatformDependentValue<bool>(PlatformCategory.Input)
	{
		Mouse = false,
		Touch = true
	};

	// Token: 0x04002C96 RID: 11414
	private static readonly PlatformDependentValue<float> DAMAGE_INDICATOR_SCALE = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 2.5f,
		Tablet = 3.75f
	};

	// Token: 0x04002C97 RID: 11415
	private static readonly PlatformDependentValue<float> DAMAGE_INDICATOR_Z_OFFSET = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 0.75f,
		Tablet = -1.2f
	};

	// Token: 0x04002C98 RID: 11416
	private static TargetReticleManager s_instance;

	// Token: 0x04002C99 RID: 11417
	private bool m_isEnemyArrow;

	// Token: 0x04002C9A RID: 11418
	private bool m_isActive;

	// Token: 0x04002C9B RID: 11419
	private bool m_showArrow = true;

	// Token: 0x04002C9C RID: 11420
	private int m_originLocationEntityID = -1;

	// Token: 0x04002C9D RID: 11421
	private int m_sourceEntityID = -1;

	// Token: 0x04002C9E RID: 11422
	private int m_numActiveLinks;

	// Token: 0x04002C9F RID: 11423
	private float m_linkAnimationZOffset;

	// Token: 0x04002CA0 RID: 11424
	private Vector3 m_targetArrowOrigin;

	// Token: 0x04002CA1 RID: 11425
	private Vector3 m_remoteArrowPosition;

	// Token: 0x04002CA2 RID: 11426
	private GameObject m_arrow;

	// Token: 0x04002CA3 RID: 11427
	private GameObject m_damageIndicator;

	// Token: 0x04002CA4 RID: 11428
	private GameObject m_hunterReticle;

	// Token: 0x04002CA5 RID: 11429
	private List<GameObject> m_targetArrowLinks;

	// Token: 0x04002CA6 RID: 11430
	private bool m_useHandAsOrigin;
}
