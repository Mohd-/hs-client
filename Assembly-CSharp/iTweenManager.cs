using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001DE RID: 478
public class iTweenManager : MonoBehaviour
{
	// Token: 0x06001D91 RID: 7569 RVA: 0x0008A1D8 File Offset: 0x000883D8
	public static iTweenManager Get()
	{
		if (iTweenManager.s_quitting)
		{
			return null;
		}
		if (iTweenManager.s_instance == null)
		{
			iTweenManager.s_instance = new GameObject
			{
				name = "iTweenManager"
			}.AddComponent<iTweenManager>();
		}
		return iTweenManager.s_instance;
	}

	// Token: 0x06001D92 RID: 7570 RVA: 0x0008A224 File Offset: 0x00088424
	public static void Add(iTween tween)
	{
		iTweenManager iTweenManager = iTweenManager.Get();
		if (iTweenManager != null)
		{
			iTweenManager.AddImpl(tween);
		}
	}

	// Token: 0x06001D93 RID: 7571 RVA: 0x0008A24A File Offset: 0x0008844A
	private void AddImpl(iTween tween)
	{
		this.m_tweenCollection.Add(tween);
		tween.Awake();
	}

	// Token: 0x06001D94 RID: 7572 RVA: 0x0008A260 File Offset: 0x00088460
	public static void Remove(iTween tween)
	{
		iTweenManager iTweenManager = iTweenManager.Get();
		if (iTweenManager != null)
		{
			iTweenManager.RemoveImpl(tween);
		}
	}

	// Token: 0x06001D95 RID: 7573 RVA: 0x0008A286 File Offset: 0x00088486
	private void RemoveImpl(iTween tween)
	{
		this.m_tweenCollection.Remove(tween);
		tween.destroyed = true;
	}

	// Token: 0x06001D96 RID: 7574 RVA: 0x0008A29B File Offset: 0x0008849B
	public void OnApplicationQuit()
	{
		iTweenManager.s_instance = null;
		iTweenManager.s_quitting = true;
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06001D97 RID: 7575 RVA: 0x0008A2B4 File Offset: 0x000884B4
	public void OnDestroy()
	{
		if (iTweenManager.s_instance == this)
		{
			iTweenManager.s_instance = null;
		}
	}

	// Token: 0x06001D98 RID: 7576 RVA: 0x0008A2CC File Offset: 0x000884CC
	public void Update()
	{
		iTween next;
		while ((next = this.m_tweenCollection.GetIterator().GetNext()) != null)
		{
			next.Upkeep();
			next.Update();
		}
		this.m_tweenCollection.CleanUp();
	}

	// Token: 0x06001D99 RID: 7577 RVA: 0x0008A310 File Offset: 0x00088510
	public void LateUpdate()
	{
		iTween next;
		while ((next = this.m_tweenCollection.GetIterator().GetNext()) != null)
		{
			next.Upkeep();
			next.LateUpdate();
		}
		this.m_tweenCollection.CleanUp();
	}

	// Token: 0x06001D9A RID: 7578 RVA: 0x0008A354 File Offset: 0x00088554
	public void FixedUpdate()
	{
		iTween next;
		while ((next = this.m_tweenCollection.GetIterator().GetNext()) != null)
		{
			next.Upkeep();
			next.FixedUpdate();
		}
		this.m_tweenCollection.CleanUp();
	}

	// Token: 0x06001D9B RID: 7579 RVA: 0x0008A398 File Offset: 0x00088598
	public iTween GetTweenForObject(GameObject obj)
	{
		iTween next;
		while ((next = this.m_tweenCollection.GetIterator().GetNext()) != null)
		{
			if (next.gameObject == obj)
			{
				return next;
			}
		}
		return null;
	}

	// Token: 0x06001D9C RID: 7580 RVA: 0x0008A3D8 File Offset: 0x000885D8
	public static iTween[] GetTweensForObject(GameObject obj)
	{
		iTweenManager iTweenManager = iTweenManager.Get();
		if (iTweenManager != null)
		{
			return iTweenManager.GetTweensForObjectImpl(obj);
		}
		return new iTween[0];
	}

	// Token: 0x06001D9D RID: 7581 RVA: 0x0008A408 File Offset: 0x00088608
	private iTween[] GetTweensForObjectImpl(GameObject obj)
	{
		List<iTween> list = new List<iTween>();
		iTween next;
		while ((next = this.m_tweenCollection.GetIterator().GetNext()) != null)
		{
			if (next.gameObject == obj)
			{
				list.Add(next);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06001D9E RID: 7582 RVA: 0x0008A458 File Offset: 0x00088658
	public static iTweenIterator GetIterator()
	{
		iTweenManager iTweenManager = iTweenManager.Get();
		if (iTweenManager == null)
		{
			return new iTweenIterator(null);
		}
		return iTweenManager.GetIteratorImpl();
	}

	// Token: 0x06001D9F RID: 7583 RVA: 0x0008A484 File Offset: 0x00088684
	private iTweenIterator GetIteratorImpl()
	{
		return this.m_tweenCollection.GetIterator();
	}

	// Token: 0x06001DA0 RID: 7584 RVA: 0x0008A494 File Offset: 0x00088694
	public static int GetTweenCount()
	{
		iTweenManager iTweenManager = iTweenManager.Get();
		if (iTweenManager == null)
		{
			return 0;
		}
		return iTweenManager.GetTweenCountImpl();
	}

	// Token: 0x06001DA1 RID: 7585 RVA: 0x0008A4BB File Offset: 0x000886BB
	private int GetTweenCountImpl()
	{
		return this.m_tweenCollection.Count;
	}

	// Token: 0x06001DA2 RID: 7586 RVA: 0x0008A4C8 File Offset: 0x000886C8
	public static void ForEach(iTweenManager.TweenOperation op, GameObject go = null, string name = null, string type = null, bool includeChildren = false)
	{
		iTweenManager iTweenManager = iTweenManager.Get();
		if (iTweenManager != null)
		{
			iTweenManager.ForEachImpl(op, go, name, type, includeChildren);
		}
	}

	// Token: 0x06001DA3 RID: 7587 RVA: 0x0008A4F3 File Offset: 0x000886F3
	public static void ForEachByGameObject(iTweenManager.TweenOperation op, GameObject go)
	{
		iTweenManager.ForEach(op, go, null, null, false);
	}

	// Token: 0x06001DA4 RID: 7588 RVA: 0x0008A4FF File Offset: 0x000886FF
	public static void ForEachByType(iTweenManager.TweenOperation op, string type)
	{
		iTweenManager.ForEach(op, null, null, type, false);
	}

	// Token: 0x06001DA5 RID: 7589 RVA: 0x0008A50B File Offset: 0x0008870B
	public static void ForEachByName(iTweenManager.TweenOperation op, string name)
	{
		iTweenManager.ForEach(op, null, name, null, false);
	}

	// Token: 0x06001DA6 RID: 7590 RVA: 0x0008A518 File Offset: 0x00088718
	private void ForEachImpl(iTweenManager.TweenOperation op, GameObject go = null, string name = null, string type = null, bool includeChildren = false)
	{
		iTween next;
		while ((next = this.m_tweenCollection.GetIterator().GetNext()) != null)
		{
			if (!(go != null) || !(next.gameObject != go))
			{
				if (name == null || name.Equals(next._name))
				{
					if (type != null)
					{
						string text = next.type + next.method;
						text = text.Substring(0, type.Length);
						if (!text.ToLower().Equals(type.ToLower()))
						{
							continue;
						}
					}
					op(next);
					if (go != null && includeChildren)
					{
						foreach (object obj in go.transform)
						{
							Transform transform = (Transform)obj;
							iTweenManager.ForEach(op, transform.gameObject, name, type, true);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001DA7 RID: 7591 RVA: 0x0008A648 File Offset: 0x00088848
	public static void ForEachInverted(iTweenManager.TweenOperation op, GameObject go, string name, string type, bool includeChildren = false)
	{
		iTweenManager iTweenManager = iTweenManager.Get();
		if (iTweenManager != null)
		{
			iTweenManager.ForEachInvertedImpl(op, go, name, type, includeChildren);
		}
	}

	// Token: 0x06001DA8 RID: 7592 RVA: 0x0008A674 File Offset: 0x00088874
	private void ForEachInvertedImpl(iTweenManager.TweenOperation op, GameObject go, string name, string type, bool includeChildren = false)
	{
		iTween next;
		while ((next = this.m_tweenCollection.GetIterator().GetNext()) != null)
		{
			if (!(go != null) || !(next.gameObject != go))
			{
				if (name == null || !name.Equals(next._name))
				{
					if (type != null)
					{
						string text = next.type + next.method;
						text = text.Substring(0, type.Length);
						if (text.ToLower().Equals(type.ToLower()))
						{
							continue;
						}
					}
					op(next);
					if (go != null && includeChildren)
					{
						foreach (object obj in go.transform)
						{
							Transform transform = (Transform)obj;
							iTweenManager.ForEachInverted(op, transform.gameObject, name, type, true);
						}
					}
				}
			}
		}
	}

	// Token: 0x0400105E RID: 4190
	private static iTweenManager s_instance;

	// Token: 0x0400105F RID: 4191
	private static bool s_quitting;

	// Token: 0x04001060 RID: 4192
	private iTweenCollection m_tweenCollection = new iTweenCollection();

	// Token: 0x020001DF RID: 479
	// (Invoke) Token: 0x06001DAA RID: 7594
	public delegate void TweenOperation(iTween tween);

	// Token: 0x02000AAE RID: 2734
	private class iTweenEntry
	{
		// Token: 0x04004630 RID: 17968
		private GameObject gameObject;

		// Token: 0x04004631 RID: 17969
		private iTween iTween;

		// Token: 0x04004632 RID: 17970
		private Hashtable args;
	}
}
