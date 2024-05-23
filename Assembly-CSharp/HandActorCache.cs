using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200076F RID: 1903
public class HandActorCache
{
	// Token: 0x06004C25 RID: 19493 RVA: 0x0016AFC8 File Offset: 0x001691C8
	public void Initialize()
	{
		foreach (TAG_CARDTYPE tag_CARDTYPE in this.ACTOR_CARD_TYPES)
		{
			foreach (object obj in Enum.GetValues(typeof(TAG_PREMIUM)))
			{
				TAG_PREMIUM tag_PREMIUM = (TAG_PREMIUM)((int)obj);
				string heroSkinOrHandActor = ActorNames.GetHeroSkinOrHandActor(tag_CARDTYPE, tag_PREMIUM);
				HandActorCache.ActorKey callbackData = this.MakeActorKey(tag_CARDTYPE, tag_PREMIUM);
				AssetLoader.Get().LoadActor(heroSkinOrHandActor, new AssetLoader.GameObjectCallback(this.OnActorLoaded), callbackData, false);
			}
		}
	}

	// Token: 0x06004C26 RID: 19494 RVA: 0x0016B084 File Offset: 0x00169284
	public bool IsInitializing()
	{
		foreach (TAG_CARDTYPE cardType in this.ACTOR_CARD_TYPES)
		{
			foreach (object obj in Enum.GetValues(typeof(TAG_PREMIUM)))
			{
				TAG_PREMIUM premiumType = (TAG_PREMIUM)((int)obj);
				HandActorCache.ActorKey key = this.MakeActorKey(cardType, premiumType);
				if (!this.m_actorMap.ContainsKey(key))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004C27 RID: 19495 RVA: 0x0016B138 File Offset: 0x00169338
	public Actor GetActor(EntityDef entityDef, TAG_PREMIUM premium)
	{
		HandActorCache.ActorKey key = this.MakeActorKey(entityDef, premium);
		Actor result;
		if (!this.m_actorMap.TryGetValue(key, out result))
		{
			Debug.LogError(string.Format("HandActorCache.GetActor() - FAILED to get actor with cardType={0} premiumType={1}", entityDef.GetCardType(), premium));
			return null;
		}
		return result;
	}

	// Token: 0x06004C28 RID: 19496 RVA: 0x0016B184 File Offset: 0x00169384
	public void AddActorLoadedListener(HandActorCache.ActorLoadedCallback callback)
	{
		this.AddActorLoadedListener(callback, null);
	}

	// Token: 0x06004C29 RID: 19497 RVA: 0x0016B190 File Offset: 0x00169390
	public void AddActorLoadedListener(HandActorCache.ActorLoadedCallback callback, object userData)
	{
		HandActorCache.ActorLoadedListener actorLoadedListener = new HandActorCache.ActorLoadedListener();
		actorLoadedListener.SetCallback(callback);
		actorLoadedListener.SetUserData(userData);
		if (this.m_loadedListeners.Contains(actorLoadedListener))
		{
			return;
		}
		this.m_loadedListeners.Add(actorLoadedListener);
	}

	// Token: 0x06004C2A RID: 19498 RVA: 0x0016B1CF File Offset: 0x001693CF
	public bool RemoveActorLoadedListener(HandActorCache.ActorLoadedCallback callback)
	{
		return this.RemoveActorLoadedListener(callback, null);
	}

	// Token: 0x06004C2B RID: 19499 RVA: 0x0016B1DC File Offset: 0x001693DC
	public bool RemoveActorLoadedListener(HandActorCache.ActorLoadedCallback callback, object userData)
	{
		HandActorCache.ActorLoadedListener actorLoadedListener = new HandActorCache.ActorLoadedListener();
		actorLoadedListener.SetCallback(callback);
		actorLoadedListener.SetUserData(userData);
		return this.m_loadedListeners.Remove(actorLoadedListener);
	}

	// Token: 0x06004C2C RID: 19500 RVA: 0x0016B209 File Offset: 0x00169409
	private HandActorCache.ActorKey MakeActorKey(EntityDef entityDef, TAG_PREMIUM premium)
	{
		return this.MakeActorKey(entityDef.GetCardType(), premium);
	}

	// Token: 0x06004C2D RID: 19501 RVA: 0x0016B218 File Offset: 0x00169418
	private HandActorCache.ActorKey MakeActorKey(TAG_CARDTYPE cardType, TAG_PREMIUM premiumType)
	{
		return new HandActorCache.ActorKey
		{
			m_cardType = cardType,
			m_premiumType = premiumType
		};
	}

	// Token: 0x06004C2E RID: 19502 RVA: 0x0016B23C File Offset: 0x0016943C
	private void OnActorLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogWarning(string.Format("HandActorCache.OnActorLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		Actor component = go.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("HandActorCache.OnActorLoaded() - ERROR \"{0}\" has no Actor component", name));
			return;
		}
		go.transform.position = new Vector3(-99999f, -99999f, 99999f);
		HandActorCache.ActorKey key = (HandActorCache.ActorKey)callbackData;
		this.m_actorMap.Add(key, component);
		this.FireActorLoadedListeners(name, component);
	}

	// Token: 0x06004C2F RID: 19503 RVA: 0x0016B2C8 File Offset: 0x001694C8
	private void FireActorLoadedListeners(string name, Actor actor)
	{
		HandActorCache.ActorLoadedListener[] array = this.m_loadedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire(name, actor);
		}
	}

	// Token: 0x0400330A RID: 13066
	private readonly TAG_CARDTYPE[] ACTOR_CARD_TYPES = new TAG_CARDTYPE[]
	{
		TAG_CARDTYPE.MINION,
		TAG_CARDTYPE.SPELL,
		TAG_CARDTYPE.WEAPON,
		TAG_CARDTYPE.HERO
	};

	// Token: 0x0400330B RID: 13067
	private Map<HandActorCache.ActorKey, Actor> m_actorMap = new Map<HandActorCache.ActorKey, Actor>();

	// Token: 0x0400330C RID: 13068
	private List<HandActorCache.ActorLoadedListener> m_loadedListeners = new List<HandActorCache.ActorLoadedListener>();

	// Token: 0x02000770 RID: 1904
	// (Invoke) Token: 0x06004C31 RID: 19505
	public delegate void ActorLoadedCallback(string name, Actor actor, object userData);

	// Token: 0x02000782 RID: 1922
	private class ActorLoadedListener : EventListener<HandActorCache.ActorLoadedCallback>
	{
		// Token: 0x06004C6A RID: 19562 RVA: 0x0016BBC8 File Offset: 0x00169DC8
		public void Fire(string name, Actor actor)
		{
			this.m_callback(name, actor, this.m_userData);
		}
	}

	// Token: 0x02000783 RID: 1923
	private class ActorKey
	{
		// Token: 0x06004C6C RID: 19564 RVA: 0x0016BBE8 File Offset: 0x00169DE8
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			HandActorCache.ActorKey other = obj as HandActorCache.ActorKey;
			return this.Equals(other);
		}

		// Token: 0x06004C6D RID: 19565 RVA: 0x0016BC0C File Offset: 0x00169E0C
		public bool Equals(HandActorCache.ActorKey other)
		{
			return other != null && this.m_cardType == other.m_cardType && this.m_premiumType == other.m_premiumType;
		}

		// Token: 0x06004C6E RID: 19566 RVA: 0x0016BC44 File Offset: 0x00169E44
		public override int GetHashCode()
		{
			int num = 23;
			num = num * 17 + this.m_cardType.GetHashCode();
			return num * 17 + this.m_premiumType.GetHashCode();
		}

		// Token: 0x06004C6F RID: 19567 RVA: 0x0016BC81 File Offset: 0x00169E81
		public static bool operator ==(HandActorCache.ActorKey a, HandActorCache.ActorKey b)
		{
			return object.ReferenceEquals(a, b) || (a != null && b != null && a.Equals(b));
		}

		// Token: 0x06004C70 RID: 19568 RVA: 0x0016BCA6 File Offset: 0x00169EA6
		public static bool operator !=(HandActorCache.ActorKey a, HandActorCache.ActorKey b)
		{
			return !(a == b);
		}

		// Token: 0x04003346 RID: 13126
		public TAG_CARDTYPE m_cardType;

		// Token: 0x04003347 RID: 13127
		public TAG_PREMIUM m_premiumType;
	}
}
