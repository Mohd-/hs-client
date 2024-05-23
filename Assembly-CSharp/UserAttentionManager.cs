using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x0200016A RID: 362
public static class UserAttentionManager
{
	// Token: 0x14000009 RID: 9
	// (add) Token: 0x060013A7 RID: 5031 RVA: 0x000578DE File Offset: 0x00055ADE
	// (remove) Token: 0x060013A8 RID: 5032 RVA: 0x000578F5 File Offset: 0x00055AF5
	public static event Action<UserAttentionBlocker> OnBlockingStart;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x060013A9 RID: 5033 RVA: 0x0005790C File Offset: 0x00055B0C
	// (remove) Token: 0x060013AA RID: 5034 RVA: 0x00057923 File Offset: 0x00055B23
	public static event Action OnBlockingEnd;

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x060013AB RID: 5035 RVA: 0x0005793A File Offset: 0x00055B3A
	private static bool IsBlocked
	{
		get
		{
			return UserAttentionManager.s_blockedReasons != UserAttentionBlocker.NONE;
		}
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x00057948 File Offset: 0x00055B48
	public static bool IsBlockedBy(UserAttentionBlocker attentionCategory)
	{
		return attentionCategory != UserAttentionBlocker.NONE && (UserAttentionManager.s_blockedReasons & attentionCategory) == attentionCategory;
	}

	// Token: 0x060013AD RID: 5037 RVA: 0x0005796A File Offset: 0x00055B6A
	public static bool CanShowAttentionGrabber(string callerName)
	{
		return UserAttentionManager.CanShowAttentionGrabber(UserAttentionBlocker.NONE, callerName);
	}

	// Token: 0x060013AE RID: 5038 RVA: 0x00057974 File Offset: 0x00055B74
	public static bool CanShowAttentionGrabber(UserAttentionBlocker attentionCategory, string callerName)
	{
		bool flag = (UserAttentionManager.s_blockedReasons & ~attentionCategory) != UserAttentionBlocker.NONE;
		if (flag)
		{
			IEnumerable<string> enumerable = Enumerable.Select<UserAttentionBlocker, string>(Enumerable.Where<UserAttentionBlocker>(Enumerable.Cast<UserAttentionBlocker>(Enum.GetValues(typeof(UserAttentionBlocker))), (UserAttentionBlocker r) => r != attentionCategory && UserAttentionManager.IsBlockedBy(r)), (UserAttentionBlocker r) => r.ToString());
			string text = string.Join(", ", Enumerable.ToArray<string>(enumerable));
			Log.UserAttention.Print("UserAttentionManager attention grabber [{0}] blocked by: {1}", new object[]
			{
				callerName,
				text
			});
			return false;
		}
		return true;
	}

	// Token: 0x060013AF RID: 5039 RVA: 0x00057A24 File Offset: 0x00055C24
	public static void StartBlocking(UserAttentionBlocker attentionCategory)
	{
		if (UserAttentionManager.IsBlockedBy(attentionCategory))
		{
			return;
		}
		bool isBlocked = UserAttentionManager.IsBlocked;
		if (isBlocked)
		{
			string text = UserAttentionManager.DumpUserAttentionBlockers("StartBlocking");
			Error.AddDevFatal("UserAttentionBlocker.{0} already active, cannot StartBlocking {1}", new object[]
			{
				text,
				attentionCategory
			});
		}
		UserAttentionManager.s_blockedReasons |= attentionCategory;
		UserAttentionManager.DumpUserAttentionBlockers("StartBlocking[" + attentionCategory + "]");
		if (!isBlocked && UserAttentionManager.OnBlockingStart != null)
		{
			UserAttentionManager.OnBlockingStart.Invoke(attentionCategory);
		}
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x00057AB4 File Offset: 0x00055CB4
	public static void StopBlocking(UserAttentionBlocker attentionCategory)
	{
		bool isBlocked = UserAttentionManager.IsBlocked;
		UserAttentionManager.s_blockedReasons &= ~attentionCategory;
		if (isBlocked)
		{
			if (UserAttentionManager.s_blockedReasons == UserAttentionBlocker.NONE)
			{
				Log.UserAttention.Print("UserAttentionManager.StopBlocking[{0}] - all blockers cleared.", new object[]
				{
					attentionCategory
				});
				if (UserAttentionManager.OnBlockingEnd != null)
				{
					UserAttentionManager.OnBlockingEnd.Invoke();
				}
			}
			else
			{
				Log.UserAttention.Print("UserAttentionManager.StopBlocking[{0}]", new object[]
				{
					attentionCategory
				});
			}
		}
	}

	// Token: 0x1700030A RID: 778
	// (get) Token: 0x060013B1 RID: 5041 RVA: 0x00057B3C File Offset: 0x00055D3C
	private static string CurrentActiveBlockersString
	{
		get
		{
			IEnumerable<string> enumerable = Enumerable.Select<UserAttentionBlocker, string>(Enumerable.Where<UserAttentionBlocker>(Enumerable.Cast<UserAttentionBlocker>(Enum.GetValues(typeof(UserAttentionBlocker))), (UserAttentionBlocker r) => UserAttentionManager.IsBlockedBy(r)), (UserAttentionBlocker r) => r.ToString());
			return string.Join(", ", Enumerable.ToArray<string>(enumerable));
		}
	}

	// Token: 0x060013B2 RID: 5042 RVA: 0x00057BB4 File Offset: 0x00055DB4
	public static string DumpUserAttentionBlockers(string callerName)
	{
		string currentActiveBlockersString = UserAttentionManager.CurrentActiveBlockersString;
		Log.UserAttention.Print("UserAttentionManager:{0} current blockers: {1}", new object[]
		{
			callerName,
			currentActiveBlockersString
		});
		return currentActiveBlockersString;
	}

	// Token: 0x04000A1E RID: 2590
	private static UserAttentionBlocker s_blockedReasons;
}
