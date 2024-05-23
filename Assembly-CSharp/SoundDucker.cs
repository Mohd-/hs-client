using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000369 RID: 873
public class SoundDucker : MonoBehaviour
{
	// Token: 0x06002C6E RID: 11374 RVA: 0x000DC87B File Offset: 0x000DAA7B
	private void Awake()
	{
		this.InitDuckedCategoryDefs();
	}

	// Token: 0x06002C6F RID: 11375 RVA: 0x000DC883 File Offset: 0x000DAA83
	private void OnDestroy()
	{
		this.StopDucking();
	}

	// Token: 0x06002C70 RID: 11376 RVA: 0x000DC88B File Offset: 0x000DAA8B
	public override string ToString()
	{
		return string.Format("[SoundDucker: {0}]", base.name);
	}

	// Token: 0x06002C71 RID: 11377 RVA: 0x000DC89D File Offset: 0x000DAA9D
	public List<SoundDuckedCategoryDef> GetDuckedCategoryDefs()
	{
		return this.m_DuckedCategoryDefs;
	}

	// Token: 0x06002C72 RID: 11378 RVA: 0x000DC8A5 File Offset: 0x000DAAA5
	public bool IsDucking()
	{
		return this.m_ducking;
	}

	// Token: 0x06002C73 RID: 11379 RVA: 0x000DC8B0 File Offset: 0x000DAAB0
	public void StartDucking()
	{
		if (SoundManager.Get() == null)
		{
			return;
		}
		if (this.m_ducking)
		{
			return;
		}
		this.InitDuckedCategoryDefs();
		this.m_ducking = SoundManager.Get().StartDucking(this);
	}

	// Token: 0x06002C74 RID: 11380 RVA: 0x000DC8F1 File Offset: 0x000DAAF1
	public void StopDucking()
	{
		if (SoundManager.Get() == null)
		{
			return;
		}
		if (!this.m_ducking)
		{
			return;
		}
		this.m_ducking = false;
		SoundManager.Get().StopDucking(this);
	}

	// Token: 0x06002C75 RID: 11381 RVA: 0x000DC924 File Offset: 0x000DAB24
	private void InitDuckedCategoryDefs()
	{
		if (!this.m_DuckAllCategories)
		{
			return;
		}
		if (this.m_GlobalDuckDef == null)
		{
			return;
		}
		this.m_DuckedCategoryDefs = new List<SoundDuckedCategoryDef>();
		foreach (object obj in Enum.GetValues(typeof(SoundCategory)))
		{
			SoundCategory soundCategory = (SoundCategory)((int)obj);
			if (soundCategory != SoundCategory.NONE)
			{
				SoundDuckedCategoryDef soundDuckedCategoryDef = new SoundDuckedCategoryDef();
				SoundUtils.CopyDuckedCategoryDef(this.m_GlobalDuckDef, soundDuckedCategoryDef);
				soundDuckedCategoryDef.m_Category = soundCategory;
				this.m_DuckedCategoryDefs.Add(soundDuckedCategoryDef);
			}
		}
	}

	// Token: 0x04001B77 RID: 7031
	public bool m_DuckAllCategories = true;

	// Token: 0x04001B78 RID: 7032
	public SoundDuckedCategoryDef m_GlobalDuckDef;

	// Token: 0x04001B79 RID: 7033
	public List<SoundDuckedCategoryDef> m_DuckedCategoryDefs;

	// Token: 0x04001B7A RID: 7034
	private bool m_ducking;
}
