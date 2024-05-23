using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class RewardUtils
{
	// Token: 0x06001360 RID: 4960 RVA: 0x000567B0 File Offset: 0x000549B0
	public static List<RewardData> GetRewards(List<NetCache.ProfileNotice> notices)
	{
		List<RewardData> list = new List<RewardData>();
		foreach (NetCache.ProfileNotice profileNotice in notices)
		{
			RewardData rewardData;
			switch (profileNotice.Type)
			{
			case NetCache.ProfileNotice.NoticeType.REWARD_BOOSTER:
			{
				NetCache.ProfileNoticeRewardBooster profileNoticeRewardBooster = profileNotice as NetCache.ProfileNoticeRewardBooster;
				BoosterPackRewardData boosterPackRewardData = new BoosterPackRewardData(profileNoticeRewardBooster.Id, profileNoticeRewardBooster.Count);
				rewardData = boosterPackRewardData;
				goto IL_158;
			}
			case NetCache.ProfileNotice.NoticeType.REWARD_CARD:
			{
				NetCache.ProfileNoticeRewardCard profileNoticeRewardCard = profileNotice as NetCache.ProfileNoticeRewardCard;
				CardRewardData cardRewardData = new CardRewardData(profileNoticeRewardCard.CardID, profileNoticeRewardCard.Premium, profileNoticeRewardCard.Quantity);
				rewardData = cardRewardData;
				goto IL_158;
			}
			case NetCache.ProfileNotice.NoticeType.REWARD_DUST:
			{
				NetCache.ProfileNoticeRewardDust profileNoticeRewardDust = profileNotice as NetCache.ProfileNoticeRewardDust;
				ArcaneDustRewardData arcaneDustRewardData = new ArcaneDustRewardData(profileNoticeRewardDust.Amount);
				rewardData = arcaneDustRewardData;
				goto IL_158;
			}
			case NetCache.ProfileNotice.NoticeType.REWARD_MOUNT:
			{
				NetCache.ProfileNoticeRewardMount profileNoticeRewardMount = profileNotice as NetCache.ProfileNoticeRewardMount;
				MountRewardData mountRewardData = new MountRewardData((MountRewardData.MountType)profileNoticeRewardMount.MountID);
				rewardData = mountRewardData;
				goto IL_158;
			}
			case NetCache.ProfileNotice.NoticeType.REWARD_FORGE:
			{
				NetCache.ProfileNoticeRewardForge profileNoticeRewardForge = profileNotice as NetCache.ProfileNoticeRewardForge;
				ForgeTicketRewardData forgeTicketRewardData = new ForgeTicketRewardData(profileNoticeRewardForge.Quantity);
				rewardData = forgeTicketRewardData;
				goto IL_158;
			}
			case NetCache.ProfileNotice.NoticeType.REWARD_GOLD:
			{
				NetCache.ProfileNoticeRewardGold profileNoticeRewardGold = profileNotice as NetCache.ProfileNoticeRewardGold;
				GoldRewardData goldRewardData = new GoldRewardData((long)profileNoticeRewardGold.Amount, new DateTime?(DateTime.FromFileTimeUtc(profileNoticeRewardGold.Date)));
				rewardData = goldRewardData;
				goto IL_158;
			}
			case NetCache.ProfileNotice.NoticeType.REWARD_CARD_BACK:
			{
				NetCache.ProfileNoticeRewardCardBack profileNoticeRewardCardBack = profileNotice as NetCache.ProfileNoticeRewardCardBack;
				CardBackRewardData cardBackRewardData = new CardBackRewardData(profileNoticeRewardCardBack.CardBackID);
				rewardData = cardBackRewardData;
				goto IL_158;
			}
			}
			continue;
			IL_158:
			if (rewardData != null)
			{
				rewardData.SetOrigin(profileNotice.Origin, profileNotice.OriginData);
				rewardData.AddNoticeID(profileNotice.NoticeID);
				RewardUtils.AddRewardDataToList(rewardData, list);
			}
		}
		return list;
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x00056980 File Offset: 0x00054B80
	public static void GetViewableRewards(List<RewardData> rewardDataList, HashSet<RewardVisualTiming> rewardTimings, ref List<RewardData> rewardsToShow, ref List<Achievement> completedQuests)
	{
		if (rewardsToShow == null)
		{
			rewardsToShow = new List<RewardData>();
		}
		if (completedQuests == null)
		{
			completedQuests = new List<Achievement>();
		}
		foreach (RewardData rewardData in rewardDataList)
		{
			Log.Rachelle.Print("RewardUtils.GetViewableRewards() - processing reward {0}", new object[]
			{
				rewardData
			});
			if (rewardData.Origin != NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT)
			{
				bool flag = false;
				switch (rewardData.RewardType)
				{
				case Reward.Type.ARCANE_DUST:
				case Reward.Type.BOOSTER_PACK:
				case Reward.Type.GOLD:
					flag = true;
					break;
				case Reward.Type.CARD:
				{
					CardRewardData cardRewardData = rewardData as CardRewardData;
					bool flag2 = cardRewardData.CardID.Equals("HERO_08") && cardRewardData.Premium == TAG_PREMIUM.NORMAL;
					if (flag2)
					{
						flag = false;
						rewardData.AcknowledgeNotices();
						CollectionManager.Get().AddCardReward(cardRewardData, false);
					}
					else
					{
						flag = true;
					}
					break;
				}
				case Reward.Type.CARD_BACK:
					flag = (NetCache.ProfileNotice.NoticeOrigin.SEASON != rewardData.Origin);
					break;
				case Reward.Type.FORGE_TICKET:
				{
					bool flag3 = false;
					if (rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.BLIZZCON && rewardData.OriginData == 2013L)
					{
						flag3 = true;
					}
					if (rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.OUT_OF_BAND_LICENSE)
					{
						Log.Rachelle.Print(string.Format("RewardUtils.GetViewableRewards(): auto-acking notices for out of band license reward {0}", rewardData), new object[0]);
						flag3 = true;
					}
					if (flag3)
					{
						rewardData.AcknowledgeNotices();
					}
					flag = false;
					break;
				}
				}
				IL_262:
				if (!flag)
				{
					continue;
				}
				rewardsToShow.Add(rewardData);
				continue;
				goto IL_262;
			}
			Achievement completedQuest = AchieveManager.Get().GetAchievement((int)rewardData.OriginData);
			if (completedQuest != null)
			{
				List<long> noticeIDs = rewardData.GetNoticeIDs();
				Achievement achievement = completedQuests.Find((Achievement obj) => completedQuest.ID == obj.ID);
				if (achievement != null)
				{
					foreach (long noticeID in noticeIDs)
					{
						achievement.AddRewardNoticeID(noticeID);
					}
				}
				else
				{
					foreach (long noticeID2 in noticeIDs)
					{
						completedQuest.AddRewardNoticeID(noticeID2);
					}
					if (rewardTimings.Contains(completedQuest.RewardTiming))
					{
						completedQuests.Add(completedQuest);
					}
				}
			}
		}
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x00056C6C File Offset: 0x00054E6C
	public static void SortRewards(ref List<Reward> rewards)
	{
		if (rewards == null)
		{
			return;
		}
		rewards.Sort(delegate(Reward r1, Reward r2)
		{
			if (r1.RewardType == r2.RewardType)
			{
				if (r1.RewardType != Reward.Type.CARD)
				{
					return 0;
				}
				CardRewardData cardRewardData = r1.Data as CardRewardData;
				CardRewardData cardRewardData2 = r2.Data as CardRewardData;
				EntityDef entityDef = DefLoader.Get().GetEntityDef(cardRewardData.CardID);
				EntityDef entityDef2 = DefLoader.Get().GetEntityDef(cardRewardData2.CardID);
				bool flag = TAG_CARDTYPE.HERO == entityDef.GetCardType();
				bool flag2 = TAG_CARDTYPE.HERO == entityDef2.GetCardType();
				if (flag == flag2)
				{
					return 0;
				}
				return (!flag) ? 1 : -1;
			}
			else
			{
				if (r1.RewardType == Reward.Type.CARD_BACK)
				{
					return -1;
				}
				if (r2.RewardType == Reward.Type.CARD_BACK)
				{
					return 1;
				}
				if (r1.RewardType == Reward.Type.CARD)
				{
					return -1;
				}
				if (r2.RewardType == Reward.Type.CARD)
				{
					return 1;
				}
				if (r1.RewardType == Reward.Type.BOOSTER_PACK)
				{
					return -1;
				}
				if (r2.RewardType == Reward.Type.BOOSTER_PACK)
				{
					return 1;
				}
				if (r1.RewardType == Reward.Type.MOUNT)
				{
					return -1;
				}
				if (r2.RewardType == Reward.Type.MOUNT)
				{
					return 1;
				}
				return 0;
			}
		});
	}

	// Token: 0x06001363 RID: 4963 RVA: 0x00056CA8 File Offset: 0x00054EA8
	public static void AddRewardDataToList(RewardData newRewardData, List<RewardData> existingRewardDataList)
	{
		CardRewardData duplicateCardDataReward = RewardUtils.GetDuplicateCardDataReward(newRewardData, existingRewardDataList);
		if (duplicateCardDataReward == null)
		{
			existingRewardDataList.Add(newRewardData);
		}
		else
		{
			CardRewardData other = newRewardData as CardRewardData;
			duplicateCardDataReward.Merge(other);
		}
	}

	// Token: 0x06001364 RID: 4964 RVA: 0x00056CE0 File Offset: 0x00054EE0
	public static string GetRewardText(RewardData rewardData)
	{
		string result;
		switch (rewardData.RewardType)
		{
		case Reward.Type.ARCANE_DUST:
		{
			ArcaneDustRewardData arcaneDustRewardData = rewardData as ArcaneDustRewardData;
			return GameStrings.Format("GLOBAL_HERO_LEVEL_REWARD_ARCANE_DUST", new object[]
			{
				arcaneDustRewardData.Amount
			});
		}
		case Reward.Type.BOOSTER_PACK:
		{
			BoosterPackRewardData boosterPackRewardData = rewardData as BoosterPackRewardData;
			BoosterDbfRecord record = GameDbf.Booster.GetRecord(boosterPackRewardData.Id);
			string text = record.Name;
			return GameStrings.Format("GLOBAL_HERO_LEVEL_REWARD_BOOSTER", new object[]
			{
				text
			});
		}
		case Reward.Type.CARD:
		{
			CardRewardData cardRewardData = rewardData as CardRewardData;
			EntityDef entityDef = DefLoader.Get().GetEntityDef(cardRewardData.CardID);
			if (cardRewardData.Premium == TAG_PREMIUM.GOLDEN)
			{
				result = GameStrings.Format("GLOBAL_HERO_LEVEL_REWARD_GOLDEN_CARD", new object[]
				{
					GameStrings.Get("GLOBAL_COLLECTION_GOLDEN"),
					entityDef.GetName()
				});
			}
			else
			{
				result = entityDef.GetName();
			}
			return result;
		}
		case Reward.Type.GOLD:
		{
			GoldRewardData goldRewardData = rewardData as GoldRewardData;
			return GameStrings.Format("GLOBAL_HERO_LEVEL_REWARD_GOLD", new object[]
			{
				goldRewardData.Amount
			});
		}
		}
		result = "UNKNOWN";
		return result;
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x00056E24 File Offset: 0x00055024
	public static void ShowReward(UserAttentionBlocker blocker, Reward reward, bool updateCacheValues, Vector3 rewardPunchScale, Vector3 rewardScale, AnimationUtil.DelOnShownWithPunch callback, object callbackData)
	{
		RewardUtils.ShowReward_Internal(blocker, reward, updateCacheValues, rewardPunchScale, rewardScale, string.Empty, null, callback, callbackData);
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x00056E48 File Offset: 0x00055048
	public static void ShowReward(UserAttentionBlocker blocker, Reward reward, bool updateCacheValues, Vector3 rewardPunchScale, Vector3 rewardScale, string callbackName = "", object callbackData = null, GameObject callbackGO = null)
	{
		RewardUtils.ShowReward_Internal(blocker, reward, updateCacheValues, rewardPunchScale, rewardScale, callbackName, callbackGO, null, callbackData);
	}

	// Token: 0x06001367 RID: 4967 RVA: 0x00056E68 File Offset: 0x00055068
	private static void ShowReward_Internal(UserAttentionBlocker blocker, Reward reward, bool updateCacheValues, Vector3 rewardPunchScale, Vector3 rewardScale, string gameObjectCallbackName, GameObject callbackGO, AnimationUtil.DelOnShownWithPunch onShowPunchCallback, object callbackData)
	{
		if (reward == null)
		{
			return;
		}
		if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "RewardUtils.ShowReward:" + ((!(reward == null) && reward.Data != null) ? string.Concat(new object[]
		{
			reward.Data.Origin,
			":",
			reward.Data.OriginData,
			":",
			reward.Data.RewardType
		}) : "null")))
		{
			return;
		}
		RenderUtils.SetAlpha(reward.gameObject, 0f);
		AnimationUtil.ShowWithPunch(reward.gameObject, RewardUtils.REWARD_HIDDEN_SCALE, rewardPunchScale, rewardScale, gameObjectCallbackName, false, callbackGO, callbackData, onShowPunchCallback);
		reward.Show(updateCacheValues);
		RewardUtils.ShowInnkeeperQuoteForReward(reward);
	}

	// Token: 0x06001368 RID: 4968 RVA: 0x00056F48 File Offset: 0x00055148
	private static CardRewardData GetDuplicateCardDataReward(RewardData newRewardData, List<RewardData> existingRewardData)
	{
		if (!(newRewardData is CardRewardData))
		{
			return null;
		}
		CardRewardData newCardRewardData = newRewardData as CardRewardData;
		RewardData rewardData = existingRewardData.Find(delegate(RewardData obj)
		{
			if (!(obj is CardRewardData))
			{
				return false;
			}
			CardRewardData cardRewardData = obj as CardRewardData;
			return cardRewardData.CardID.Equals(newCardRewardData.CardID) && cardRewardData.Premium.Equals(newCardRewardData.Premium) && cardRewardData.Origin.Equals(newCardRewardData.Origin) && cardRewardData.OriginData.Equals(newCardRewardData.OriginData);
		});
		return rewardData as CardRewardData;
	}

	// Token: 0x06001369 RID: 4969 RVA: 0x00056F90 File Offset: 0x00055190
	private static void ShowInnkeeperQuoteForReward(Reward reward)
	{
		if (reward == null)
		{
			return;
		}
		if (reward.RewardType == Reward.Type.CARD)
		{
			CardRewardData cardRewardData = reward.Data as CardRewardData;
			CardRewardData.InnKeeperTrigger innKeeperLine = cardRewardData.InnKeeperLine;
			if (innKeeperLine != CardRewardData.InnKeeperTrigger.CORE_CLASS_SET_COMPLETE)
			{
				if (innKeeperLine == CardRewardData.InnKeeperTrigger.SECOND_REWARD_EVER)
				{
					if (!Options.Get().GetBool(Option.HAS_BEEN_NUDGED_TO_CM, false))
					{
						NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_NUDGE_CM_X"), "VO_INNKEEPER2_NUDGE_COLLECTION_10", 0f, null);
						Options.Get().SetBool(Option.HAS_BEEN_NUDGED_TO_CM, true);
					}
				}
			}
			else
			{
				Notification innkeeperQuote = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_BASIC_DONE1_11"), "VO_INNKEEPER_BASIC_DONE1_11", 0f, null);
				if (!Options.Get().GetBool(Option.HAS_SEEN_ALL_BASIC_CLASS_CARDS_COMPLETE, false))
				{
					SceneMgr.Get().StartCoroutine(RewardUtils.NotifyOfExpertPacksNeeded(innkeeperQuote));
				}
			}
		}
	}

	// Token: 0x0600136A RID: 4970 RVA: 0x0005706C File Offset: 0x0005526C
	private static IEnumerator NotifyOfExpertPacksNeeded(Notification innkeeperQuote)
	{
		while (innkeeperQuote.GetAudio() == null)
		{
			yield return null;
		}
		yield return new WaitForSeconds(innkeeperQuote.GetAudio().clip.length);
		NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_BASIC_DONE2_12"), "VO_INNKEEPER_BASIC_DONE2_12", 0f, null);
		Options.Get().SetBool(Option.HAS_SEEN_ALL_BASIC_CLASS_CARDS_COMPLETE, true);
		yield break;
	}

	// Token: 0x040009FE RID: 2558
	public static readonly Vector3 REWARD_HIDDEN_SCALE = new Vector3(0.001f, 0.001f, 0.001f);

	// Token: 0x040009FF RID: 2559
	public static readonly float REWARD_HIDE_TIME = 0.25f;
}
