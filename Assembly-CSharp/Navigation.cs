using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200011B RID: 283
public static class Navigation
{
	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06000D8E RID: 3470 RVA: 0x0003716D File Offset: 0x0003536D
	// (set) Token: 0x06000D8F RID: 3471 RVA: 0x0003717F File Offset: 0x0003537F
	public static bool NAVIGATION_DEBUG
	{
		get
		{
			return Vars.Key("Application.Navigation.Debug").GetBool(false);
		}
		set
		{
			VarsInternal.Get().Set("Application.Navigation.Debug", value.ToString());
		}
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x00037197 File Offset: 0x00035397
	public static void Clear()
	{
		Navigation.history.Clear();
		if (Navigation.NAVIGATION_DEBUG)
		{
			Navigation.DumpStack();
		}
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x000371B4 File Offset: 0x000353B4
	public static bool GoBack()
	{
		if (Navigation.history.Count == 0 || !Navigation.CanNavigate())
		{
			return false;
		}
		Navigation.NavigateBackHandler navigateBackHandler = Navigation.history.Peek();
		if (navigateBackHandler())
		{
			if (Navigation.history.Count > 0 && navigateBackHandler == Navigation.history.Peek())
			{
				Navigation.history.Pop();
			}
			if (Navigation.NAVIGATION_DEBUG)
			{
				Navigation.DumpStack();
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x00037234 File Offset: 0x00035434
	public static void Push(Navigation.NavigateBackHandler handler)
	{
		if (handler == null)
		{
			return;
		}
		Navigation.history.Push(handler);
		if (Navigation.NAVIGATION_DEBUG)
		{
			Navigation.DumpStack();
		}
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x00037258 File Offset: 0x00035458
	public static void PushUnique(Navigation.NavigateBackHandler handler)
	{
		if (handler == null)
		{
			return;
		}
		foreach (Navigation.NavigateBackHandler navigateBackHandler in Navigation.history)
		{
			if (navigateBackHandler == handler)
			{
				return;
			}
		}
		Navigation.history.Push(handler);
		if (Navigation.NAVIGATION_DEBUG)
		{
			Navigation.DumpStack();
		}
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x000372DC File Offset: 0x000354DC
	public static void PushIfNotOnTop(Navigation.NavigateBackHandler handler)
	{
		if (handler == null)
		{
			return;
		}
		if (Navigation.history.Count > 0 && Navigation.history.Peek() == handler)
		{
			if (Navigation.NAVIGATION_DEBUG)
			{
				Debug.LogFormat("Navigation - Did not push {0}, it already exists on the top of the stack!", new object[]
				{
					Navigation.StackEntryToString(handler)
				});
			}
			return;
		}
		Navigation.history.Push(handler);
		if (Navigation.NAVIGATION_DEBUG)
		{
			Navigation.DumpStack();
		}
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x00037354 File Offset: 0x00035554
	public static void Pop()
	{
		if (Navigation.history.Count == 0 || !Navigation.CanNavigate())
		{
			return;
		}
		Navigation.history.Pop();
		if (Navigation.NAVIGATION_DEBUG)
		{
			Navigation.DumpStack();
		}
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00037398 File Offset: 0x00035598
	public static void PopUnique(Navigation.NavigateBackHandler handler)
	{
		if (Navigation.history.Count == 0)
		{
			return;
		}
		if (Navigation.history.Contains(handler))
		{
			Navigation.history = new Stack<Navigation.NavigateBackHandler>(Enumerable.Reverse<Navigation.NavigateBackHandler>(Enumerable.Where<Navigation.NavigateBackHandler>(Navigation.history, (Navigation.NavigateBackHandler h) => h != handler)));
		}
		if (Navigation.NAVIGATION_DEBUG)
		{
			Navigation.DumpStack();
		}
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x0003740B File Offset: 0x0003560B
	public static bool BackStackContainsHandler(Navigation.NavigateBackHandler handler)
	{
		return Navigation.history.Contains(handler);
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x00037418 File Offset: 0x00035618
	public static bool BlockBackingOut()
	{
		return false;
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x0003741C File Offset: 0x0003561C
	private static bool CanNavigate()
	{
		if (GameUtils.IsAnyTransitionActive())
		{
			return false;
		}
		switch (GameMgr.Get().GetFindGameState())
		{
		case FindGameState.CLIENT_STARTED:
		case FindGameState.CLIENT_CANCELED:
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_QUEUE_CANCELED:
		case FindGameState.BNET_ERROR:
		case FindGameState.SERVER_GAME_CONNECTING:
		case FindGameState.SERVER_GAME_STARTED:
		case FindGameState.SERVER_GAME_CANCELED:
			return false;
		}
		return true;
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x00037480 File Offset: 0x00035680
	private static string StackEntryToString(Navigation.NavigateBackHandler entry)
	{
		return string.Format("{0}.{1} Target={2}", entry.Method.DeclaringType, entry.Method.Name, (entry != null && entry.Target != null) ? entry.Target.ToString() : ((!entry.Method.IsStatic) ? "null" : "<static>"));
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x000374F0 File Offset: 0x000356F0
	public static void DumpStack()
	{
		Debug.Log(string.Format("Navigation Stack Dump (count: {0})\n", Navigation.history.Count));
		int num = 0;
		foreach (Navigation.NavigateBackHandler entry in Navigation.history)
		{
			Debug.Log(string.Format("{0}: {1}\n", num, Navigation.StackEntryToString(entry)));
			num++;
		}
	}

	// Token: 0x0400073C RID: 1852
	private static Stack<Navigation.NavigateBackHandler> history = new Stack<Navigation.NavigateBackHandler>();

	// Token: 0x02000219 RID: 537
	// (Invoke) Token: 0x060020ED RID: 8429
	public delegate bool NavigateBackHandler();
}
