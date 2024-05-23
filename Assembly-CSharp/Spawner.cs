using System;
using UnityEngine;

// Token: 0x02000557 RID: 1367
public class Spawner : MonoBehaviour
{
	// Token: 0x06003E8F RID: 16015 RVA: 0x0012E93E File Offset: 0x0012CB3E
	protected virtual void Awake()
	{
		if (this.spawnOnAwake)
		{
			this.Spawn();
		}
	}

	// Token: 0x06003E90 RID: 16016 RVA: 0x0012E954 File Offset: 0x0012CB54
	public GameObject Spawn()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.prefab);
		gameObject.transform.parent = base.transform.parent;
		TransformUtil.CopyLocal(gameObject, base.transform);
		SceneUtils.SetLayer(gameObject, base.gameObject.layer);
		if (this.destroyOnSpawn)
		{
			Object.Destroy(base.gameObject);
		}
		return gameObject;
	}

	// Token: 0x06003E91 RID: 16017 RVA: 0x0012E9B8 File Offset: 0x0012CBB8
	public T Spawn<T>() where T : MonoBehaviour
	{
		if (this.prefab.GetComponent<T>() != null)
		{
			return this.Spawn().GetComponent<T>();
		}
		Debug.Log(string.Format("The prefab for spawner {0} does not have component {1}", base.gameObject.name, typeof(T).Name));
		return (T)((object)null);
	}

	// Token: 0x0400281D RID: 10269
	public GameObject prefab;

	// Token: 0x0400281E RID: 10270
	public bool spawnOnAwake;

	// Token: 0x0400281F RID: 10271
	public bool destroyOnSpawn = true;
}
