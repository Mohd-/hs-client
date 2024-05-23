using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class SceneUtils
{
	// Token: 0x06001DC5 RID: 7621 RVA: 0x0008ABAC File Offset: 0x00088DAC
	public static void SetLayer(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (object obj in go.transform)
		{
			Transform transform = (Transform)obj;
			SceneUtils.SetLayer(transform.gameObject, layer);
		}
	}

	// Token: 0x06001DC6 RID: 7622 RVA: 0x0008AC1C File Offset: 0x00088E1C
	public static void SetLayer(Component c, int layer)
	{
		SceneUtils.SetLayer(c.gameObject, layer);
	}

	// Token: 0x06001DC7 RID: 7623 RVA: 0x0008AC2A File Offset: 0x00088E2A
	public static void SetLayer(GameObject go, GameLayer layer)
	{
		SceneUtils.SetLayer(go, (int)layer);
	}

	// Token: 0x06001DC8 RID: 7624 RVA: 0x0008AC33 File Offset: 0x00088E33
	public static void SetLayer(Component c, GameLayer layer)
	{
		SceneUtils.SetLayer(c.gameObject, (int)layer);
	}

	// Token: 0x06001DC9 RID: 7625 RVA: 0x0008AC44 File Offset: 0x00088E44
	public static void ReplaceLayer(GameObject parentObject, GameLayer newLayer, GameLayer oldLayer)
	{
		if (parentObject.layer == (int)oldLayer)
		{
			parentObject.layer = (int)newLayer;
		}
		foreach (object obj in parentObject.transform)
		{
			Transform transform = (Transform)obj;
			SceneUtils.ReplaceLayer(transform.gameObject, newLayer, oldLayer);
		}
	}

	// Token: 0x06001DCA RID: 7626 RVA: 0x0008ACC0 File Offset: 0x00088EC0
	public static string LayerMaskToString(LayerMask mask)
	{
		if (mask == 0)
		{
			return "[NO LAYERS]";
		}
		StringBuilder stringBuilder = new StringBuilder("[");
		foreach (object obj in Enum.GetValues(typeof(GameLayer)))
		{
			GameLayer gameLayer = (GameLayer)((int)obj);
			if ((mask & gameLayer.LayerBit()) != 0)
			{
				stringBuilder.Append(gameLayer);
				stringBuilder.Append(", ");
			}
		}
		if (stringBuilder.Length == 1)
		{
			return "[NO LAYERS]";
		}
		stringBuilder.Remove(stringBuilder.Length - 2, 2);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	// Token: 0x06001DCB RID: 7627 RVA: 0x0008ADA8 File Offset: 0x00088FA8
	public static void SetRenderQueue(GameObject go, int renderQueue, bool includeInactive = false)
	{
		foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>(includeInactive))
		{
			if (!(renderer.material == null))
			{
				renderer.material.renderQueue = renderQueue;
			}
		}
	}

	// Token: 0x06001DCC RID: 7628 RVA: 0x0008ADF8 File Offset: 0x00088FF8
	public static GameObject FindChild(GameObject parentObject, string name)
	{
		if (parentObject.name.Equals(name, 5))
		{
			return parentObject;
		}
		foreach (object obj in parentObject.transform)
		{
			Transform transform = (Transform)obj;
			GameObject gameObject = SceneUtils.FindChild(transform.gameObject, name);
			if (gameObject)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x06001DCD RID: 7629 RVA: 0x0008AE90 File Offset: 0x00089090
	public static GameObject FindChildBySubstring(GameObject parentObject, string substr)
	{
		if (parentObject.name.IndexOf(substr, 5) >= 0)
		{
			return parentObject;
		}
		foreach (object obj in parentObject.transform)
		{
			Transform transform = (Transform)obj;
			GameObject gameObject = SceneUtils.FindChildBySubstring(transform.gameObject, substr);
			if (gameObject)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x06001DCE RID: 7630 RVA: 0x0008AF28 File Offset: 0x00089128
	public static Transform FindFirstChild(Transform parent)
	{
		using (IEnumerator enumerator = parent.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return (Transform)enumerator.Current;
			}
		}
		return null;
	}

	// Token: 0x06001DCF RID: 7631 RVA: 0x0008AF8C File Offset: 0x0008918C
	public static bool IsAncestorOf(GameObject ancestor, GameObject descendant)
	{
		return SceneUtils.IsAncestorOf(ancestor.transform, descendant.transform);
	}

	// Token: 0x06001DD0 RID: 7632 RVA: 0x0008AFA0 File Offset: 0x000891A0
	public static bool IsAncestorOf(Component ancestor, Component descendant)
	{
		Transform transform = descendant.transform;
		while (transform != null)
		{
			if (transform == ancestor.transform)
			{
				return true;
			}
			transform = transform.parent;
		}
		return false;
	}

	// Token: 0x06001DD1 RID: 7633 RVA: 0x0008AFE0 File Offset: 0x000891E0
	public static bool IsDescendantOf(GameObject descendant, GameObject ancestor)
	{
		return SceneUtils.IsDescendantOf(descendant.transform, ancestor.transform);
	}

	// Token: 0x06001DD2 RID: 7634 RVA: 0x0008AFF3 File Offset: 0x000891F3
	public static bool IsDescendantOf(GameObject descendant, Component ancestor)
	{
		return SceneUtils.IsDescendantOf(descendant.transform, ancestor.transform);
	}

	// Token: 0x06001DD3 RID: 7635 RVA: 0x0008B006 File Offset: 0x00089206
	public static bool IsDescendantOf(Component descendant, GameObject ancestor)
	{
		return SceneUtils.IsDescendantOf(descendant.transform, ancestor.transform);
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x0008B01C File Offset: 0x0008921C
	public static bool IsDescendantOf(Component descendant, Component ancestor)
	{
		if (descendant == ancestor)
		{
			return true;
		}
		foreach (object obj in ancestor.transform)
		{
			Transform ancestor2 = (Transform)obj;
			if (SceneUtils.IsDescendantOf(descendant, ancestor2))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001DD5 RID: 7637 RVA: 0x0008B09C File Offset: 0x0008929C
	public static T FindComponentInParents<T>(Component child) where T : Component
	{
		if (child == null)
		{
			return (T)((object)null);
		}
		Transform parent = child.transform.parent;
		while (parent != null)
		{
			T component = parent.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			parent = parent.parent;
		}
		return (T)((object)null);
	}

	// Token: 0x06001DD6 RID: 7638 RVA: 0x0008B100 File Offset: 0x00089300
	public static T FindComponentInParents<T>(GameObject child) where T : Component
	{
		if (child == null)
		{
			return (T)((object)null);
		}
		return SceneUtils.FindComponentInParents<T>(child.transform);
	}

	// Token: 0x06001DD7 RID: 7639 RVA: 0x0008B120 File Offset: 0x00089320
	public static T FindComponentInThisOrParents<T>(Component start) where T : Component
	{
		if (start == null)
		{
			return (T)((object)null);
		}
		Transform transform = start.transform;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return (T)((object)null);
	}

	// Token: 0x06001DD8 RID: 7640 RVA: 0x0008B17F File Offset: 0x0008937F
	public static T FindComponentInThisOrParents<T>(GameObject start) where T : Component
	{
		if (start == null)
		{
			return (T)((object)null);
		}
		return SceneUtils.FindComponentInThisOrParents<T>(start.transform);
	}

	// Token: 0x06001DD9 RID: 7641 RVA: 0x0008B1A0 File Offset: 0x000893A0
	public static T GetComponentInChildrenOnly<T>(GameObject go) where T : Component
	{
		if (go != null)
		{
			foreach (object obj in go.transform)
			{
				Transform transform = (Transform)obj;
				T componentInChildren = transform.GetComponentInChildren<T>();
				if (componentInChildren != null)
				{
					return componentInChildren;
				}
			}
		}
		return (T)((object)null);
	}

	// Token: 0x06001DDA RID: 7642 RVA: 0x0008B234 File Offset: 0x00089434
	public static T GetComponentInChildrenOnly<T>(Component c) where T : Component
	{
		if (c == null)
		{
			return (T)((object)null);
		}
		return SceneUtils.GetComponentInChildrenOnly<T>(c.gameObject);
	}

	// Token: 0x06001DDB RID: 7643 RVA: 0x0008B254 File Offset: 0x00089454
	public static T[] GetComponentsInChildrenOnly<T>(Component c) where T : Component
	{
		if (c == null)
		{
			return new T[0];
		}
		return SceneUtils.GetComponentsInChildrenOnly<T>(c.gameObject);
	}

	// Token: 0x06001DDC RID: 7644 RVA: 0x0008B274 File Offset: 0x00089474
	public static T[] GetComponentsInChildrenOnly<T>(GameObject go) where T : Component
	{
		return SceneUtils.GetComponentsInChildrenOnly<T>(go, false);
	}

	// Token: 0x06001DDD RID: 7645 RVA: 0x0008B27D File Offset: 0x0008947D
	public static T[] GetComponentsInChildrenOnly<T>(Component c, bool includeInactive) where T : Component
	{
		if (c == null)
		{
			return new T[0];
		}
		return SceneUtils.GetComponentsInChildrenOnly<T>(c.gameObject, includeInactive);
	}

	// Token: 0x06001DDE RID: 7646 RVA: 0x0008B2A0 File Offset: 0x000894A0
	public static T[] GetComponentsInChildrenOnly<T>(GameObject go, bool includeInactive) where T : Component
	{
		if (go != null)
		{
			List<T> list = new List<T>();
			foreach (object obj in go.transform)
			{
				Transform transform = (Transform)obj;
				T[] componentsInChildren = transform.GetComponentsInChildren<T>(includeInactive);
				list.AddRange(componentsInChildren);
			}
			return list.ToArray();
		}
		return new T[0];
	}

	// Token: 0x06001DDF RID: 7647 RVA: 0x0008B330 File Offset: 0x00089530
	public static GameObject FindTopParent(Component c)
	{
		Transform transform = c.transform;
		while (transform.parent != null)
		{
			transform = transform.parent;
		}
		return transform.gameObject;
	}

	// Token: 0x06001DE0 RID: 7648 RVA: 0x0008B367 File Offset: 0x00089567
	public static GameObject FindTopParent(GameObject go)
	{
		return SceneUtils.FindTopParent(go.transform);
	}

	// Token: 0x06001DE1 RID: 7649 RVA: 0x0008B374 File Offset: 0x00089574
	public static GameObject FindChildByTag(GameObject go, string tag)
	{
		Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>();
		if (componentsInChildren == null)
		{
			return null;
		}
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.CompareTag(tag))
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	// Token: 0x06001DE2 RID: 7650 RVA: 0x0008B3BE File Offset: 0x000895BE
	public static void EnableRenderers(Component c, bool enable)
	{
		SceneUtils.EnableRenderers(c.gameObject, enable);
	}

	// Token: 0x06001DE3 RID: 7651 RVA: 0x0008B3CC File Offset: 0x000895CC
	public static void EnableRenderers(GameObject go, bool enable)
	{
		SceneUtils.EnableRenderers(go, enable, false);
	}

	// Token: 0x06001DE4 RID: 7652 RVA: 0x0008B3D6 File Offset: 0x000895D6
	public static void EnableRenderers(Component c, bool enable, bool includeInactive)
	{
		SceneUtils.EnableRenderers(c.gameObject, enable, false);
	}

	// Token: 0x06001DE5 RID: 7653 RVA: 0x0008B3E8 File Offset: 0x000895E8
	public static void EnableRenderers(GameObject go, bool enable, bool includeInactive)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(includeInactive);
		if (componentsInChildren == null)
		{
			return;
		}
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = enable;
		}
	}

	// Token: 0x06001DE6 RID: 7654 RVA: 0x0008B425 File Offset: 0x00089625
	public static void EnableColliders(Component c, bool enable)
	{
		SceneUtils.EnableColliders(c.gameObject, enable);
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x0008B434 File Offset: 0x00089634
	public static void EnableColliders(GameObject go, bool enable)
	{
		Collider[] componentsInChildren = go.GetComponentsInChildren<Collider>();
		if (componentsInChildren == null)
		{
			return;
		}
		foreach (Collider collider in componentsInChildren)
		{
			collider.enabled = enable;
		}
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x0008B470 File Offset: 0x00089670
	public static void EnableRenderersAndColliders(Component c, bool enable)
	{
		SceneUtils.EnableRenderersAndColliders(c.gameObject, enable);
	}

	// Token: 0x06001DE9 RID: 7657 RVA: 0x0008B480 File Offset: 0x00089680
	public static void EnableRenderersAndColliders(GameObject go, bool enable)
	{
		Collider component = go.GetComponent<Collider>();
		if (component != null)
		{
			component.enabled = enable;
		}
		Renderer component2 = go.GetComponent<Renderer>();
		if (component2 != null)
		{
			component2.enabled = enable;
		}
		foreach (object obj in go.transform)
		{
			Transform transform = (Transform)obj;
			SceneUtils.EnableRenderersAndColliders(transform.gameObject, enable);
		}
	}

	// Token: 0x06001DEA RID: 7658 RVA: 0x0008B520 File Offset: 0x00089720
	public static void ResizeBoxCollider(GameObject go, Component worldCorner1, Component worldCorner2)
	{
		SceneUtils.ResizeBoxCollider(go.GetComponent<Collider>(), worldCorner1.transform.position, worldCorner2.transform.position);
	}

	// Token: 0x06001DEB RID: 7659 RVA: 0x0008B550 File Offset: 0x00089750
	public static void ResizeBoxCollider(GameObject go, GameObject worldCorner1, Component worldCorner2)
	{
		SceneUtils.ResizeBoxCollider(go.GetComponent<Collider>(), worldCorner1.transform.position, worldCorner2.transform.position);
	}

	// Token: 0x06001DEC RID: 7660 RVA: 0x0008B580 File Offset: 0x00089780
	public static void ResizeBoxCollider(GameObject go, Component worldCorner1, GameObject worldCorner2)
	{
		SceneUtils.ResizeBoxCollider(go.GetComponent<Collider>(), worldCorner1.transform.position, worldCorner2.transform.position);
	}

	// Token: 0x06001DED RID: 7661 RVA: 0x0008B5B0 File Offset: 0x000897B0
	public static void ResizeBoxCollider(GameObject go, GameObject worldCorner1, GameObject worldCorner2)
	{
		SceneUtils.ResizeBoxCollider(go.GetComponent<Collider>(), worldCorner1.transform.position, worldCorner2.transform.position);
	}

	// Token: 0x06001DEE RID: 7662 RVA: 0x0008B5E0 File Offset: 0x000897E0
	public static void ResizeBoxCollider(Component c, Component worldCorner1, Component worldCorner2)
	{
		SceneUtils.ResizeBoxCollider(c, worldCorner1.transform.position, worldCorner2.transform.position);
	}

	// Token: 0x06001DEF RID: 7663 RVA: 0x0008B60C File Offset: 0x0008980C
	public static void ResizeBoxCollider(Component c, GameObject worldCorner1, Component worldCorner2)
	{
		SceneUtils.ResizeBoxCollider(c, worldCorner1.transform.position, worldCorner2.transform.position);
	}

	// Token: 0x06001DF0 RID: 7664 RVA: 0x0008B638 File Offset: 0x00089838
	public static void ResizeBoxCollider(Component c, Component worldCorner1, GameObject worldCorner2)
	{
		SceneUtils.ResizeBoxCollider(c, worldCorner1.transform.position, worldCorner2.transform.position);
	}

	// Token: 0x06001DF1 RID: 7665 RVA: 0x0008B664 File Offset: 0x00089864
	public static void ResizeBoxCollider(Component c, GameObject worldCorner1, GameObject worldCorner2)
	{
		SceneUtils.ResizeBoxCollider(c, worldCorner1.transform.position, worldCorner2.transform.position);
	}

	// Token: 0x06001DF2 RID: 7666 RVA: 0x0008B690 File Offset: 0x00089890
	public static void ResizeBoxCollider(GameObject go, Bounds bounds)
	{
		SceneUtils.ResizeBoxCollider(go.GetComponent<Collider>(), bounds.min, bounds.max);
	}

	// Token: 0x06001DF3 RID: 7667 RVA: 0x0008B6B8 File Offset: 0x000898B8
	public static void ResizeBoxCollider(Component c, Bounds bounds)
	{
		SceneUtils.ResizeBoxCollider(c.GetComponent<Collider>(), bounds.min, bounds.max);
	}

	// Token: 0x06001DF4 RID: 7668 RVA: 0x0008B6DE File Offset: 0x000898DE
	public static void ResizeBoxCollider(GameObject go, Vector3 worldCorner1, Vector3 worldCorner2)
	{
		SceneUtils.ResizeBoxCollider(go.GetComponent<Collider>(), worldCorner1, worldCorner2);
	}

	// Token: 0x06001DF5 RID: 7669 RVA: 0x0008B6F0 File Offset: 0x000898F0
	public static void ResizeBoxCollider(Component c, Vector3 worldCorner1, Vector3 worldCorner2)
	{
		Vector3 vector = c.transform.InverseTransformPoint(worldCorner1);
		Vector3 vector2 = c.transform.InverseTransformPoint(worldCorner2);
		Vector3 vector3 = Vector3.Min(vector, vector2);
		Vector3 vector4 = Vector3.Max(vector, vector2);
		BoxCollider component = c.GetComponent<BoxCollider>();
		component.center = 0.5f * (vector3 + vector4);
		component.size = vector4 - vector3;
	}

	// Token: 0x06001DF6 RID: 7670 RVA: 0x0008B758 File Offset: 0x00089958
	public static Transform CreateBone(GameObject template)
	{
		string text = string.Format("{0}Bone", template.name);
		GameObject gameObject = new GameObject(text);
		gameObject.transform.parent = template.transform.parent;
		TransformUtil.CopyLocal(gameObject, template);
		return gameObject.transform;
	}

	// Token: 0x06001DF7 RID: 7671 RVA: 0x0008B7A0 File Offset: 0x000899A0
	public static Transform CreateBone(Component template)
	{
		return SceneUtils.CreateBone(template.gameObject);
	}

	// Token: 0x06001DF8 RID: 7672 RVA: 0x0008B7AD File Offset: 0x000899AD
	public static void SetHideFlags(Object obj, HideFlags flags)
	{
	}
}
