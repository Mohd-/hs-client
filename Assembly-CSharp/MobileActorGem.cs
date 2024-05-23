using System;
using UnityEngine;

// Token: 0x02000957 RID: 2391
public class MobileActorGem : MonoBehaviour
{
	// Token: 0x06005784 RID: 22404 RVA: 0x001A31E8 File Offset: 0x001A13E8
	private void Awake()
	{
		if (PlatformSettings.OS == OSCategory.iOS || PlatformSettings.OS == OSCategory.Android)
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				if (this.m_gemType == MobileActorGem.GemType.CardPlay)
				{
					base.gameObject.transform.localScale *= 1.6f;
					this.m_uberText.transform.localScale *= 0.9f;
					this.m_uberText.OutlineSize = 3.2f;
				}
				else if (this.m_gemType == MobileActorGem.GemType.CardHero_Attack)
				{
					base.gameObject.transform.localScale *= 1.6f;
					TransformUtil.SetLocalPosX(base.gameObject, base.gameObject.transform.localPosition.x - 0.075f);
					TransformUtil.SetLocalPosZ(base.gameObject, base.gameObject.transform.localPosition.z + 0.255f);
					this.m_uberText.transform.localScale *= 0.9f;
					this.m_uberText.OutlineSize = 3.2f;
				}
				else if (this.m_gemType == MobileActorGem.GemType.CardHero_Health)
				{
					base.gameObject.transform.localScale *= 1.6f;
					TransformUtil.SetLocalPosX(base.gameObject, base.gameObject.transform.localPosition.x + 0.05f);
					TransformUtil.SetLocalPosZ(base.gameObject, base.gameObject.transform.localPosition.z + 0.255f);
					this.m_uberText.transform.localPosition = new Vector3(0f, 0.154f, -0.0235f);
					this.m_uberText.OutlineSize = 3.6f;
				}
				else if (this.m_gemType == MobileActorGem.GemType.CardHero_Armor)
				{
					base.gameObject.transform.localScale *= 1.15f;
					TransformUtil.SetLocalPosX(base.gameObject, 0.06f);
					TransformUtil.SetLocalPosZ(base.gameObject, base.gameObject.transform.localPosition.z - 0.3f);
					this.m_uberText.transform.localScale *= 1.4f;
					this.m_uberText.FontSize = 50;
					this.m_uberText.CharacterSize = 8f;
					this.m_uberText.OutlineSize = 3.2f;
				}
				else if (this.m_gemType == MobileActorGem.GemType.CardHeroPower)
				{
					TransformUtil.SetLocalScaleXZ(base.gameObject, new Vector2(1.334f * base.gameObject.transform.localScale.x, 1.334f * base.gameObject.transform.localScale.z));
					TransformUtil.SetLocalScaleXY(this.m_uberText, new Vector2(1.5f * this.m_uberText.transform.localScale.x, 1.5f * this.m_uberText.transform.localScale.y));
					TransformUtil.SetLocalPosZ(this.m_uberText, this.m_uberText.transform.localPosition.z + 0.04f);
				}
			}
			else if (this.m_gemType == MobileActorGem.GemType.CardPlay)
			{
				base.gameObject.transform.localScale *= 1.3f;
				this.m_uberText.transform.localScale *= 0.9f;
				this.m_uberText.OutlineSize = 3.2f;
			}
		}
	}

	// Token: 0x04003E57 RID: 15959
	public UberText m_uberText;

	// Token: 0x04003E58 RID: 15960
	public MobileActorGem.GemType m_gemType;

	// Token: 0x02000958 RID: 2392
	public enum GemType
	{
		// Token: 0x04003E5A RID: 15962
		CardPlay,
		// Token: 0x04003E5B RID: 15963
		CardHero_Health,
		// Token: 0x04003E5C RID: 15964
		CardHero_Attack,
		// Token: 0x04003E5D RID: 15965
		CardHero_Armor,
		// Token: 0x04003E5E RID: 15966
		CardHeroPower
	}
}
