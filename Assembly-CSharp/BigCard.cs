using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000847 RID: 2119
[CustomEditClass]
public class BigCard : MonoBehaviour
{
	// Token: 0x06005150 RID: 20816 RVA: 0x00183D64 File Offset: 0x00181F64
	public BigCard()
	{
		this.PLATFORM_SCALING_FACTOR = new PlatformDependentValue<float>(PlatformCategory.Screen)
		{
			PC = 1f,
			Tablet = 1f,
			Phone = 1.3f,
			MiniTablet = 1f
		};
		this.ENCHANTMENT_SCALING_FACTOR = new PlatformDependentValue<float>(PlatformCategory.Screen)
		{
			PC = 1f,
			Tablet = 1f,
			Phone = 1.5f,
			MiniTablet = 1f
		};
	}

	// Token: 0x06005152 RID: 20818 RVA: 0x00183E20 File Offset: 0x00182020
	private void Awake()
	{
		BigCard.s_instance = this;
		this.m_initialBannerHeight = this.m_EnchantmentBanner.GetComponent<Renderer>().bounds.size.z;
		this.m_initialBannerScale = this.m_EnchantmentBanner.transform.localScale;
		this.m_initialBannerBottomScale = this.m_EnchantmentBannerBottom.transform.localScale;
		this.m_enchantmentPool.SetCreateItemCallback(new Pool<BigCardEnchantmentPanel>.CreateItemCallback(this.CreateEnchantmentPanel));
		this.m_enchantmentPool.SetDestroyItemCallback(new Pool<BigCardEnchantmentPanel>.DestroyItemCallback(this.DestroyEnchantmentPanel));
		this.m_enchantmentPool.SetExtensionCount(1);
		this.m_enchantmentPool.SetMaxReleasedItemCount(2);
		this.ResetEnchantments();
	}

	// Token: 0x06005153 RID: 20819 RVA: 0x00183ED1 File Offset: 0x001820D1
	private void OnDestroy()
	{
		BigCard.s_instance = null;
	}

	// Token: 0x06005154 RID: 20820 RVA: 0x00183ED9 File Offset: 0x001820D9
	public static BigCard Get()
	{
		return BigCard.s_instance;
	}

	// Token: 0x06005155 RID: 20821 RVA: 0x00183EE0 File Offset: 0x001820E0
	public Card GetCard()
	{
		return this.m_card;
	}

	// Token: 0x06005156 RID: 20822 RVA: 0x00183EE8 File Offset: 0x001820E8
	public void Show(Card card)
	{
		this.m_card = card;
		if (GameState.Get() != null && !GameState.Get().GetGameEntity().NotifyOfCardTooltipDisplayShow(card))
		{
			return;
		}
		Zone zone = card.GetZone();
		if (UniversalInputManager.UsePhoneUI && zone is ZoneSecret)
		{
			this.LoadAndDisplayTooltipPhoneSecrets();
		}
		else
		{
			this.LoadAndDisplayBigCard();
		}
	}

	// Token: 0x06005157 RID: 20823 RVA: 0x00183F50 File Offset: 0x00182150
	public void Hide()
	{
		if (GameState.Get() != null)
		{
			GameState.Get().GetGameEntity().NotifyOfCardTooltipDisplayHide(this.m_card);
		}
		this.HideBigCard();
		this.HideTooltipPhoneSecrets();
		this.m_card = null;
	}

	// Token: 0x06005158 RID: 20824 RVA: 0x00183F8F File Offset: 0x0018218F
	public bool Hide(Card card)
	{
		if (this.m_card != card)
		{
			return false;
		}
		this.Hide();
		return true;
	}

	// Token: 0x06005159 RID: 20825 RVA: 0x00183FAC File Offset: 0x001821AC
	public void ShowSecretDeaths(Map<Player, DeadSecretGroup> deadSecretMap)
	{
		if (deadSecretMap == null)
		{
			return;
		}
		if (deadSecretMap.Count == 0)
		{
			return;
		}
		int num = 0;
		foreach (DeadSecretGroup deadSecretGroup in deadSecretMap.Values)
		{
			Card mainCard = deadSecretGroup.GetMainCard();
			List<Card> cards = deadSecretGroup.GetCards();
			List<Actor> list = new List<Actor>();
			for (int i = 0; i < cards.Count; i++)
			{
				Card card = cards[i];
				Actor actor = this.LoadPhoneSecret(card);
				list.Add(actor);
			}
			this.DisplayPhoneSecrets(mainCard, list, true);
			num++;
		}
	}

	// Token: 0x0600515A RID: 20826 RVA: 0x00184074 File Offset: 0x00182274
	private void LoadAndDisplayBigCard()
	{
		if (this.m_bigCardActor)
		{
			this.m_bigCardActor.Destroy();
		}
		string bigCardActor = ActorNames.GetBigCardActor(this.m_card.GetEntity());
		GameObject gameObject = AssetLoader.Get().LoadActor(bigCardActor, false, false);
		this.m_bigCardActor = gameObject.GetComponent<Actor>();
		this.SetupActor(this.m_card, this.m_bigCardActor);
		this.DisplayBigCard();
	}

	// Token: 0x0600515B RID: 20827 RVA: 0x001840E0 File Offset: 0x001822E0
	private void HideBigCard()
	{
		if (!this.m_bigCardActor)
		{
			return;
		}
		this.ResetEnchantments();
		iTween.Stop(this.m_bigCardActor.gameObject);
		this.m_bigCardActor.Destroy();
		this.m_bigCardActor = null;
		KeywordHelpPanelManager.Get().HideKeywordHelp();
	}

	// Token: 0x0600515C RID: 20828 RVA: 0x00184130 File Offset: 0x00182330
	private void DisplayBigCard()
	{
		Entity entity = this.m_card.GetEntity();
		bool flag = entity.GetController().IsFriendlySide();
		Zone zone = this.m_card.GetZone();
		Bounds bounds = this.m_bigCardActor.GetMeshRenderer().bounds;
		Vector3 position = this.m_card.GetActor().transform.position;
		Vector3 vector;
		vector..ctor(0f, 0f, 0f);
		Vector3 vector2;
		vector2..ctor(1.1f, 1.1f, 1.1f);
		float? overrideScale = default(float?);
		if (zone is ZoneHero)
		{
			if (flag)
			{
				vector..ctor(0f, 4f, 0f);
			}
			else
			{
				vector..ctor(0f, 4f, -bounds.size.z * 0.7f);
			}
		}
		else if (zone is ZoneHeroPower)
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				vector2..ctor(1.3f, 1f, 1.3f);
				if (flag)
				{
					vector..ctor(-3.5f, 8f, 3.5f);
				}
				else
				{
					vector..ctor(-3.5f, 8f, -3.35f);
				}
			}
			else if (flag)
			{
				vector..ctor(0f, 4f, 2.69f);
			}
			else
			{
				vector..ctor(0f, 4f, -2.6f);
			}
			overrideScale = new float?(0.6f);
		}
		else if (zone is ZoneWeapon)
		{
			vector2..ctor(1.65f, 1.65f, 1.65f);
			if (flag)
			{
				vector..ctor(0f, 0f, bounds.size.z * 0.9f);
			}
			else
			{
				vector..ctor(-1.57f, 0f, -1f);
			}
			vector2 *= this.PLATFORM_SCALING_FACTOR;
		}
		else if (zone is ZoneSecret)
		{
			vector2..ctor(1.65f, 1.65f, 1.65f);
			vector..ctor(bounds.size.x + 0.3f, 0f, 0f);
		}
		else if (zone is ZoneHand)
		{
			vector..ctor(bounds.size.x * 0.7f, 4f, -bounds.size.z * 0.8f);
			vector2..ctor(1.65f, 1.65f, 1.65f);
		}
		else
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				vector2..ctor(2f, 2f, 2f);
				if (this.ShowBigCardOnRight())
				{
					vector..ctor(bounds.size.x + 2.2f, 0f, 0f);
				}
				else
				{
					vector..ctor(-bounds.size.x - 2.2f, 0f, 0f);
				}
			}
			else
			{
				vector2..ctor(1.65f, 1.65f, 1.65f);
				if (this.ShowBigCardOnRight())
				{
					vector..ctor(bounds.size.x + 0.7f, 0f, 0f);
				}
				else
				{
					vector..ctor(-bounds.size.x - 0.7f, 0f, 0f);
				}
			}
			if (zone is ZonePlay)
			{
				vector += new Vector3(0f, 0.1f, 0f);
				vector2 *= this.PLATFORM_SCALING_FACTOR;
			}
		}
		Vector3 vector3;
		vector3..ctor(0.02f, 0.02f, 0.02f);
		Vector3 vector4 = position + vector;
		Vector3 position2 = vector4 + vector3;
		Vector3 localScale;
		localScale..ctor(1f, 1f, 1f);
		Transform parent = this.m_bigCardActor.transform.parent;
		this.m_bigCardActor.transform.localScale = vector2;
		this.m_bigCardActor.transform.position = position2;
		this.m_bigCardActor.transform.parent = null;
		if (zone is ZoneHand)
		{
			this.m_bigCardActor.SetEntity(entity);
			this.m_bigCardActor.UpdateTextComponents(entity);
		}
		else
		{
			this.UpdateEnchantments();
			if (UniversalInputManager.UsePhoneUI && this.m_EnchantmentBanner.activeInHierarchy)
			{
				float num = (this.m_enchantmentPool.GetActiveList().Count <= 1) ? 0.85f : 0.75f;
				vector2 *= num;
				this.m_bigCardActor.transform.localScale = vector2;
			}
		}
		this.FitInsideScreen();
		this.m_bigCardActor.transform.parent = parent;
		this.m_bigCardActor.transform.localScale = localScale;
		Vector3 position3 = this.m_bigCardActor.transform.position;
		this.m_bigCardActor.transform.position -= vector3;
		BigCard.KeywordArgs keywordArgs = default(BigCard.KeywordArgs);
		keywordArgs.card = this.m_card;
		keywordArgs.actor = this.m_bigCardActor;
		keywordArgs.showOnRight = this.ShowBigCardOnRight();
		if (zone is ZoneHand)
		{
			object[] array = new object[8];
			array[0] = "scale";
			array[1] = vector2;
			array[2] = "time";
			array[3] = this.m_LayoutData.m_ScaleSec;
			array[4] = "oncompleteparams";
			array[5] = keywordArgs;
			array[6] = "oncomplete";
			array[7] = delegate(object obj)
			{
				BigCard.KeywordArgs keywordArgs2 = (BigCard.KeywordArgs)obj;
				KeywordHelpPanelManager.Get().UpdateKeywordHelp(keywordArgs2.card, keywordArgs2.actor, keywordArgs2.showOnRight, default(float?), default(Vector3?));
			};
			Hashtable args = iTween.Hash(array);
			iTween.ScaleTo(this.m_bigCardActor.gameObject, args);
		}
		else
		{
			iTween.ScaleTo(this.m_bigCardActor.gameObject, vector2, this.m_LayoutData.m_ScaleSec);
			KeywordHelpPanelManager.Get().UpdateKeywordHelp(keywordArgs.card, keywordArgs.actor, keywordArgs.showOnRight, overrideScale, default(Vector3?));
		}
		iTween.MoveTo(this.m_bigCardActor.gameObject, position3, this.m_LayoutData.m_DriftSec);
		this.m_bigCardActor.transform.rotation = Quaternion.identity;
		this.m_bigCardActor.Show();
		if (UniversalInputManager.UsePhoneUI)
		{
			Vector3? overrideOffset = default(Vector3?);
			if (this.m_card.GetEntity().IsHeroPower())
			{
				overrideOffset = new Vector3?(new Vector3(0.6f, 0f, -1.3f));
			}
			KeywordHelpPanelManager.Get().UpdateKeywordHelp(this.m_card, this.m_bigCardActor, this.ShowKeywordOnRight(), overrideScale, overrideOffset);
		}
		if (entity.IsSilenced())
		{
			this.m_bigCardActor.ActivateSpell(SpellType.SILENCE);
		}
	}

	// Token: 0x0600515D RID: 20829 RVA: 0x00184878 File Offset: 0x00182A78
	private bool ShowBigCardOnRight()
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			return this.ShowBigCardOnRightTouch();
		}
		return this.ShowBigCardOnRightMouse();
	}

	// Token: 0x0600515E RID: 20830 RVA: 0x001848A4 File Offset: 0x00182AA4
	private bool ShowBigCardOnRightMouse()
	{
		if (this.m_card.GetEntity().IsHero() || this.m_card.GetEntity().IsHeroPower() || this.m_card.GetEntity().IsSecret())
		{
			return true;
		}
		if (this.m_card.GetEntity().GetCardId() == "TU4c_007")
		{
			return false;
		}
		ZonePlay zonePlay = this.m_card.GetZone() as ZonePlay;
		return !(zonePlay != null) || this.m_card.GetActor().GetMeshRenderer().bounds.center.x < zonePlay.GetComponent<BoxCollider>().bounds.center.x + 2.5f;
	}

	// Token: 0x0600515F RID: 20831 RVA: 0x0018497C File Offset: 0x00182B7C
	private bool ShowBigCardOnRightTouch()
	{
		if (this.m_card.GetEntity().IsHero() || this.m_card.GetEntity().IsHeroPower() || this.m_card.GetEntity().IsSecret())
		{
			return false;
		}
		if (this.m_card.GetEntity().GetCardId() == "TU4c_007")
		{
			return false;
		}
		ZonePlay zonePlay = this.m_card.GetZone() as ZonePlay;
		if (zonePlay != null)
		{
			float num = (!UniversalInputManager.UsePhoneUI) ? -2.5f : 0f;
			return this.m_card.GetActor().GetMeshRenderer().bounds.center.x < zonePlay.GetComponent<BoxCollider>().bounds.center.x + num;
		}
		return false;
	}

	// Token: 0x06005160 RID: 20832 RVA: 0x00184A70 File Offset: 0x00182C70
	private bool ShowKeywordOnRight()
	{
		if (this.m_card.GetEntity().IsHeroPower())
		{
			return true;
		}
		if (this.m_card.GetEntity().IsWeapon())
		{
			return true;
		}
		if (this.m_card.GetEntity().IsHero() || this.m_card.GetEntity().IsSecret())
		{
			return false;
		}
		if (this.m_card.GetEntity().GetCardId() == "TU4c_007")
		{
			return false;
		}
		ZonePlay zonePlay = this.m_card.GetZone() as ZonePlay;
		if (!(zonePlay != null))
		{
			return false;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			return this.m_card.GetActor().GetMeshRenderer().bounds.center.x > zonePlay.GetComponent<BoxCollider>().bounds.center.x;
		}
		return this.m_card.GetActor().GetMeshRenderer().bounds.center.x < zonePlay.GetComponent<BoxCollider>().bounds.center.x + 0.03f;
	}

	// Token: 0x06005161 RID: 20833 RVA: 0x00184BB8 File Offset: 0x00182DB8
	private void UpdateEnchantments()
	{
		this.ResetEnchantments();
		GameObject gameObject = this.m_bigCardActor.FindBone("EnchantmentTooltip");
		if (gameObject == null)
		{
			return;
		}
		Entity entity = this.m_card.GetEntity();
		List<Entity> enchantments = entity.GetEnchantments();
		List<BigCardEnchantmentPanel> activeList = this.m_enchantmentPool.GetActiveList();
		int count = enchantments.Count;
		int count2 = activeList.Count;
		int num = count - count2;
		if (num > 0)
		{
			this.m_enchantmentPool.AcquireBatch(num);
		}
		else if (num < 0)
		{
			this.m_enchantmentPool.ReleaseBatch(count, -num);
		}
		if (count == 0)
		{
			return;
		}
		for (int i = 0; i < activeList.Count; i++)
		{
			BigCardEnchantmentPanel bigCardEnchantmentPanel = activeList[i];
			bigCardEnchantmentPanel.SetEnchantment(enchantments[i]);
		}
		this.LayoutEnchantments(gameObject);
		SceneUtils.SetLayer(gameObject, GameLayer.Tooltip);
	}

	// Token: 0x06005162 RID: 20834 RVA: 0x00184CA0 File Offset: 0x00182EA0
	private void ResetEnchantments()
	{
		this.m_EnchantmentBanner.SetActive(false);
		this.m_EnchantmentBannerBottom.SetActive(false);
		this.m_EnchantmentBanner.transform.parent = base.transform;
		this.m_EnchantmentBannerBottom.transform.parent = base.transform;
		List<BigCardEnchantmentPanel> activeList = this.m_enchantmentPool.GetActiveList();
		foreach (BigCardEnchantmentPanel bigCardEnchantmentPanel in activeList)
		{
			bigCardEnchantmentPanel.transform.parent = base.transform;
			bigCardEnchantmentPanel.ResetScale();
			bigCardEnchantmentPanel.Hide();
		}
	}

	// Token: 0x06005163 RID: 20835 RVA: 0x00184D5C File Offset: 0x00182F5C
	private void LayoutEnchantments(GameObject bone)
	{
		float num = 0.1f;
		List<BigCardEnchantmentPanel> activeList = this.m_enchantmentPool.GetActiveList();
		BigCardEnchantmentPanel bigCardEnchantmentPanel = null;
		for (int i = 0; i < activeList.Count; i++)
		{
			BigCardEnchantmentPanel bigCardEnchantmentPanel2 = activeList[i];
			bigCardEnchantmentPanel2.Show();
			bigCardEnchantmentPanel2.transform.localScale *= this.PLATFORM_SCALING_FACTOR * this.ENCHANTMENT_SCALING_FACTOR;
			if (i == 0)
			{
				TransformUtil.SetPoint(bigCardEnchantmentPanel2.gameObject, new Vector3(0.5f, 0f, 1f), this.m_bigCardActor.GetMeshRenderer().gameObject, new Vector3(0.5f, 0f, 0f), new Vector3(0.01f, 0.01f, 0f));
			}
			else
			{
				TransformUtil.SetPoint(bigCardEnchantmentPanel2.gameObject, new Vector3(0f, 0f, 1f), bigCardEnchantmentPanel.gameObject, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
			}
			bigCardEnchantmentPanel = bigCardEnchantmentPanel2;
			bigCardEnchantmentPanel2.transform.parent = bone.transform;
			float height = bigCardEnchantmentPanel2.GetHeight();
			num += height;
		}
		this.m_EnchantmentBanner.SetActive(true);
		this.m_EnchantmentBannerBottom.SetActive(true);
		this.m_EnchantmentBannerBottom.transform.localScale = this.m_initialBannerBottomScale * this.PLATFORM_SCALING_FACTOR * this.ENCHANTMENT_SCALING_FACTOR;
		this.m_EnchantmentBanner.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		this.m_EnchantmentBannerBottom.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		TransformUtil.SetPoint(this.m_EnchantmentBanner, new Vector3(0.5f, 0f, 1f), this.m_bigCardActor.GetMeshRenderer().gameObject, new Vector3(0.5f, 0f, 0f), new Vector3(0f, 0f, 0.2f));
		this.m_EnchantmentBanner.transform.localScale = this.m_initialBannerScale * this.PLATFORM_SCALING_FACTOR * this.ENCHANTMENT_SCALING_FACTOR;
		TransformUtil.SetLocalScaleZ(this.m_EnchantmentBanner.gameObject, num / this.m_initialBannerHeight / this.m_initialBannerScale.z);
		this.m_EnchantmentBanner.transform.parent = bone.transform;
		TransformUtil.SetPoint(this.m_EnchantmentBannerBottom, Anchor.FRONT, this.m_EnchantmentBanner, Anchor.BACK);
		this.m_EnchantmentBannerBottom.transform.parent = bone.transform;
	}

	// Token: 0x06005164 RID: 20836 RVA: 0x00185034 File Offset: 0x00183234
	private void FitInsideScreen()
	{
		this.FitInsideScreenBottom();
		this.FitInsideScreenTop();
	}

	// Token: 0x06005165 RID: 20837 RVA: 0x00185044 File Offset: 0x00183244
	private bool FitInsideScreenBottom()
	{
		Bounds bounds = (!this.m_EnchantmentBanner.activeInHierarchy) ? this.m_bigCardActor.GetMeshRenderer().bounds : this.m_EnchantmentBannerBottom.GetComponent<Renderer>().bounds;
		Vector3 center = bounds.center;
		if (UniversalInputManager.UsePhoneUI)
		{
			center.z -= 0.4f;
		}
		Vector3 vector;
		vector..ctor(center.x, center.y, center.z - bounds.extents.z);
		Ray ray;
		ray..ctor(vector, vector - center);
		Camera camera = CameraUtils.FindFirstByLayer(GameLayer.Tooltip);
		Plane plane = CameraUtils.CreateBottomPlane(camera);
		float num = 0f;
		if (plane.Raycast(ray, ref num))
		{
			return false;
		}
		if (object.Equals(num, 0f))
		{
			return false;
		}
		TransformUtil.SetPosZ(this.m_bigCardActor.gameObject, this.m_bigCardActor.transform.position.z - num);
		return true;
	}

	// Token: 0x06005166 RID: 20838 RVA: 0x00185164 File Offset: 0x00183364
	private bool FitInsideScreenTop()
	{
		Bounds bounds = this.m_bigCardActor.GetMeshRenderer().bounds;
		Vector3 center = bounds.center;
		if (UniversalInputManager.UsePhoneUI && !(this.m_card.GetZone() is ZoneHeroPower))
		{
			center.z += 1f;
		}
		Vector3 vector;
		vector..ctor(center.x, center.y, center.z + bounds.extents.z);
		Ray ray;
		ray..ctor(vector, vector - center);
		Camera camera = CameraUtils.FindFirstByLayer(GameLayer.Tooltip);
		Plane plane = CameraUtils.CreateTopPlane(camera);
		float num = 0f;
		if (plane.Raycast(ray, ref num))
		{
			return false;
		}
		if (object.Equals(num, 0f))
		{
			return false;
		}
		TransformUtil.SetPosZ(this.m_bigCardActor.gameObject, this.m_bigCardActor.transform.position.z + num);
		return true;
	}

	// Token: 0x06005167 RID: 20839 RVA: 0x00185274 File Offset: 0x00183474
	private BigCardEnchantmentPanel CreateEnchantmentPanel(int index)
	{
		BigCardEnchantmentPanel bigCardEnchantmentPanel = Object.Instantiate<BigCardEnchantmentPanel>(this.m_EnchantmentPanelPrefab);
		bigCardEnchantmentPanel.name = string.Format("{0}{1}", typeof(BigCardEnchantmentPanel).ToString(), index);
		SceneUtils.SetRenderQueue(bigCardEnchantmentPanel.gameObject, this.m_RenderQueueEnchantmentPanel, false);
		return bigCardEnchantmentPanel;
	}

	// Token: 0x06005168 RID: 20840 RVA: 0x001852C5 File Offset: 0x001834C5
	private void DestroyEnchantmentPanel(BigCardEnchantmentPanel panel)
	{
		Object.Destroy(panel.gameObject);
	}

	// Token: 0x06005169 RID: 20841 RVA: 0x001852D4 File Offset: 0x001834D4
	private void LoadAndDisplayTooltipPhoneSecrets()
	{
		if (this.m_phoneSecretActors == null)
		{
			this.m_phoneSecretActors = new List<Actor>();
		}
		else
		{
			foreach (Actor actor in this.m_phoneSecretActors)
			{
				actor.Destroy();
			}
			this.m_phoneSecretActors.Clear();
		}
		Zone zone = this.m_card.GetZone();
		List<Card> cards = zone.GetCards();
		for (int i = 0; i < cards.Count; i++)
		{
			Actor actor2 = this.LoadPhoneSecret(cards[i]);
			this.m_phoneSecretActors.Add(actor2);
		}
		this.DisplayPhoneSecrets(this.m_card, this.m_phoneSecretActors, false);
	}

	// Token: 0x0600516A RID: 20842 RVA: 0x001853B4 File Offset: 0x001835B4
	private void HideTooltipPhoneSecrets()
	{
		if (this.m_phoneSecretActors == null)
		{
			return;
		}
		foreach (Actor actor in this.m_phoneSecretActors)
		{
			this.HidePhoneSecret(actor);
		}
		this.m_phoneSecretActors.Clear();
	}

	// Token: 0x0600516B RID: 20843 RVA: 0x00185428 File Offset: 0x00183628
	private Actor LoadPhoneSecret(Card card)
	{
		Entity entity = card.GetEntity();
		string bigCardActor = ActorNames.GetBigCardActor(entity);
		GameObject gameObject = AssetLoader.Get().LoadActor(bigCardActor, false, false);
		Actor component = gameObject.GetComponent<Actor>();
		this.SetupActor(card, component);
		return component;
	}

	// Token: 0x0600516C RID: 20844 RVA: 0x00185464 File Offset: 0x00183664
	private void DisplayPhoneSecrets(Card mainCard, List<Actor> actors, bool showDeath)
	{
		Vector3 vector;
		Vector3 vector2;
		Vector3 vector3;
		this.DetermineSecretLayoutOffsets(mainCard, actors, out vector, out vector2, out vector3);
		bool flag = GeneralUtils.IsOdd(actors.Count);
		Actor actor = mainCard.GetActor();
		Vector3 vector4 = actor.transform.position + vector;
		for (int i = 0; i < actors.Count; i++)
		{
			Actor actor2 = actors[i];
			Vector3 vector5;
			if (i == 0 && flag)
			{
				vector5 = vector4;
			}
			else
			{
				bool flag2 = GeneralUtils.IsOdd(i);
				bool flag3 = flag == flag2;
				float num = (!flag) ? Mathf.Floor(0.5f * (float)i) : Mathf.Ceil(0.5f * (float)i);
				float num2 = num * vector2.x;
				if (!flag)
				{
					num2 += 0.5f * vector2.x;
				}
				if (flag3)
				{
					num2 = -num2;
				}
				float num3 = num * vector2.z;
				vector5..ctor(vector4.x + num2, vector4.y, vector4.z + num3);
			}
			actor2.transform.position = actor.transform.position;
			actor2.transform.rotation = actor.transform.rotation;
			actor2.transform.localScale = BigCard.INVISIBLE_SCALE;
			float num4 = (!showDeath) ? this.m_SecretLayoutData.m_ShowAnimTime : this.m_SecretLayoutData.m_DeathShowAnimTime;
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				vector5 - vector3,
				"time",
				num4,
				"easeType",
				iTween.EaseType.easeOutExpo
			});
			iTween.MoveTo(actor2.gameObject, args);
			Hashtable args2 = iTween.Hash(new object[]
			{
				"position",
				vector5,
				"delay",
				num4,
				"time",
				this.m_SecretLayoutData.m_DriftSec,
				"easeType",
				iTween.EaseType.easeOutExpo
			});
			iTween.MoveTo(actor2.gameObject, args2);
			iTween.ScaleTo(actor2.gameObject, base.transform.localScale, num4);
			if (showDeath)
			{
				this.ShowPhoneSecretDeath(actor2);
			}
		}
	}

	// Token: 0x0600516D RID: 20845 RVA: 0x001856C8 File Offset: 0x001838C8
	private void DetermineSecretLayoutOffsets(Card mainCard, List<Actor> actors, out Vector3 initialOffset, out Vector3 spacing, out Vector3 drift)
	{
		Player controller = mainCard.GetController();
		bool flag = controller.IsFriendlySide();
		bool flag2 = controller.IsRevealed();
		int minCardThreshold = this.m_SecretLayoutData.m_MinCardThreshold;
		int maxCardThreshold = this.m_SecretLayoutData.m_MaxCardThreshold;
		BigCard.SecretLayoutOffsets minCardOffsets = this.m_SecretLayoutData.m_MinCardOffsets;
		BigCard.SecretLayoutOffsets maxCardOffsets = this.m_SecretLayoutData.m_MaxCardOffsets;
		float num = Mathf.InverseLerp((float)minCardThreshold, (float)maxCardThreshold, (float)actors.Count);
		if (flag2)
		{
			if (flag)
			{
				initialOffset = Vector3.Lerp(minCardOffsets.m_InitialOffset, maxCardOffsets.m_InitialOffset, num);
			}
			else
			{
				initialOffset = Vector3.Lerp(minCardOffsets.m_OpponentInitialOffset, maxCardOffsets.m_OpponentInitialOffset, num);
			}
			spacing = this.m_SecretLayoutData.m_Spacing;
		}
		else
		{
			if (flag)
			{
				initialOffset = Vector3.Lerp(minCardOffsets.m_HiddenInitialOffset, maxCardOffsets.m_HiddenInitialOffset, num);
			}
			else
			{
				initialOffset = Vector3.Lerp(minCardOffsets.m_HiddenOpponentInitialOffset, maxCardOffsets.m_HiddenOpponentInitialOffset, num);
			}
			spacing = this.m_SecretLayoutData.m_HiddenSpacing;
		}
		if (flag)
		{
			spacing.z = -spacing.z;
			drift = this.m_SecretLayoutData.m_DriftOffset;
		}
		else
		{
			drift = -this.m_SecretLayoutData.m_DriftOffset;
		}
	}

	// Token: 0x0600516E RID: 20846 RVA: 0x00185824 File Offset: 0x00183A24
	private void ShowPhoneSecretDeath(Actor actor)
	{
		Spell.StateFinishedCallback deathSpellStateFinished = delegate(Spell spell, SpellStateType prevStateType, object userData)
		{
			if (spell.GetActiveState() == SpellStateType.NONE)
			{
				return;
			}
			actor.Destroy();
		};
		Action<object> action = delegate(object obj)
		{
			Spell spell = actor.GetSpell(SpellType.DEATH);
			spell.AddStateFinishedCallback(deathSpellStateFinished);
			spell.Activate();
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			this.m_SecretLayoutData.m_TimeUntilDeathSpell,
			"oncomplete",
			action
		});
		iTween.Timer(actor.gameObject, args);
	}

	// Token: 0x0600516F RID: 20847 RVA: 0x001858A4 File Offset: 0x00183AA4
	private void HidePhoneSecret(Actor actor)
	{
		Actor actor2 = this.m_card.GetActor();
		iTween.MoveTo(actor.gameObject, actor2.transform.position, this.m_SecretLayoutData.m_HideAnimTime);
		iTween.ScaleTo(actor.gameObject, BigCard.INVISIBLE_SCALE, this.m_SecretLayoutData.m_HideAnimTime);
		Action<object> action = delegate(object obj)
		{
			actor.Destroy();
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			this.m_SecretLayoutData.m_HideAnimTime,
			"oncomplete",
			action
		});
		iTween.Timer(actor.gameObject, args);
	}

	// Token: 0x06005170 RID: 20848 RVA: 0x00185964 File Offset: 0x00183B64
	private void SetupActor(Card card, Actor actor)
	{
		Entity entity = card.GetEntity();
		if (this.ShouldActorUseEntity(entity))
		{
			actor.SetEntity(entity);
		}
		else
		{
			actor.SetEntityDef(entity.GetEntityDef());
		}
		actor.SetPremium(entity.GetPremiumType());
		actor.SetCard(card);
		actor.SetCardDef(card.GetCardDef());
		actor.UpdateAllComponents();
		actor.GetComponentInChildren<BoxCollider>().enabled = false;
		actor.name = "BigCard_" + actor.name;
		SceneUtils.SetLayer(actor, GameLayer.Tooltip);
	}

	// Token: 0x06005171 RID: 20849 RVA: 0x001859EB File Offset: 0x00183BEB
	private bool ShouldActorUseEntity(Entity entity)
	{
		return entity.IsHidden() || entity.IsHeroPower();
	}

	// Token: 0x0400380B RID: 14347
	public BigCardEnchantmentPanel m_EnchantmentPanelPrefab;

	// Token: 0x0400380C RID: 14348
	public GameObject m_EnchantmentBanner;

	// Token: 0x0400380D RID: 14349
	public GameObject m_EnchantmentBannerBottom;

	// Token: 0x0400380E RID: 14350
	public int m_RenderQueueEnchantmentBanner = 1;

	// Token: 0x0400380F RID: 14351
	public int m_RenderQueueEnchantmentPanel = 2;

	// Token: 0x04003810 RID: 14352
	public BigCard.LayoutData m_LayoutData;

	// Token: 0x04003811 RID: 14353
	public BigCard.SecretLayoutData m_SecretLayoutData;

	// Token: 0x04003812 RID: 14354
	private static readonly Vector3 INVISIBLE_SCALE = new Vector3(0.0001f, 0.0001f, 0.0001f);

	// Token: 0x04003813 RID: 14355
	private static BigCard s_instance;

	// Token: 0x04003814 RID: 14356
	private Card m_card;

	// Token: 0x04003815 RID: 14357
	private Actor m_bigCardActor;

	// Token: 0x04003816 RID: 14358
	private List<Actor> m_phoneSecretActors;

	// Token: 0x04003817 RID: 14359
	private float m_initialBannerHeight;

	// Token: 0x04003818 RID: 14360
	private Vector3 m_initialBannerScale;

	// Token: 0x04003819 RID: 14361
	private Vector3 m_initialBannerBottomScale;

	// Token: 0x0400381A RID: 14362
	private Pool<BigCardEnchantmentPanel> m_enchantmentPool = new Pool<BigCardEnchantmentPanel>();

	// Token: 0x0400381B RID: 14363
	private readonly PlatformDependentValue<float> PLATFORM_SCALING_FACTOR;

	// Token: 0x0400381C RID: 14364
	private readonly PlatformDependentValue<float> ENCHANTMENT_SCALING_FACTOR;

	// Token: 0x020008A7 RID: 2215
	[Serializable]
	public class LayoutData
	{
		// Token: 0x04003A45 RID: 14917
		public float m_ScaleSec = 0.15f;

		// Token: 0x04003A46 RID: 14918
		public float m_DriftSec = 10f;
	}

	// Token: 0x020008A8 RID: 2216
	[Serializable]
	public class SecretLayoutOffsets
	{
		// Token: 0x04003A47 RID: 14919
		public Vector3 m_InitialOffset = new Vector3(0.1f, 5f, 3.3f);

		// Token: 0x04003A48 RID: 14920
		public Vector3 m_OpponentInitialOffset = new Vector3(0.1f, 5f, -3.3f);

		// Token: 0x04003A49 RID: 14921
		public Vector3 m_HiddenInitialOffset = new Vector3(0f, 4f, 4f);

		// Token: 0x04003A4A RID: 14922
		public Vector3 m_HiddenOpponentInitialOffset = new Vector3(0f, 4f, -4f);
	}

	// Token: 0x020008A9 RID: 2217
	[Serializable]
	public class SecretLayoutData
	{
		// Token: 0x04003A4B RID: 14923
		public float m_ShowAnimTime = 0.15f;

		// Token: 0x04003A4C RID: 14924
		public float m_HideAnimTime = 0.15f;

		// Token: 0x04003A4D RID: 14925
		public float m_DeathShowAnimTime = 1f;

		// Token: 0x04003A4E RID: 14926
		public float m_TimeUntilDeathSpell = 1.5f;

		// Token: 0x04003A4F RID: 14927
		public float m_DriftSec = 5f;

		// Token: 0x04003A50 RID: 14928
		public Vector3 m_DriftOffset = new Vector3(0f, 0f, 0.05f);

		// Token: 0x04003A51 RID: 14929
		public Vector3 m_Spacing = new Vector3(2.1f, 0f, 0.7f);

		// Token: 0x04003A52 RID: 14930
		public Vector3 m_HiddenSpacing = new Vector3(2.4f, 0f, 0.7f);

		// Token: 0x04003A53 RID: 14931
		public int m_MinCardThreshold = 1;

		// Token: 0x04003A54 RID: 14932
		public int m_MaxCardThreshold = 5;

		// Token: 0x04003A55 RID: 14933
		public BigCard.SecretLayoutOffsets m_MinCardOffsets = new BigCard.SecretLayoutOffsets();

		// Token: 0x04003A56 RID: 14934
		public BigCard.SecretLayoutOffsets m_MaxCardOffsets = new BigCard.SecretLayoutOffsets();
	}

	// Token: 0x020008AA RID: 2218
	private struct KeywordArgs
	{
		// Token: 0x04003A57 RID: 14935
		public Card card;

		// Token: 0x04003A58 RID: 14936
		public Actor actor;

		// Token: 0x04003A59 RID: 14937
		public bool showOnRight;
	}
}
