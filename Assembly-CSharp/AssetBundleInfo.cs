using System;
using UnityEngine;

// Token: 0x020002CF RID: 719
public class AssetBundleInfo
{
	// Token: 0x0600261D RID: 9757 RVA: 0x000B9F63 File Offset: 0x000B8163
	public static string BundlePathPlatformModifier()
	{
		return "win/";
	}

	// Token: 0x040016A3 RID: 5795
	public const int NumSharedDependencyBundles = 4;

	// Token: 0x040016A4 RID: 5796
	public const string SharedBundleName = "shared";

	// Token: 0x040016A5 RID: 5797
	public const int NUM_BUNDLES_DEFAULT = 1;

	// Token: 0x040016A6 RID: 5798
	public const int NUM_ACTOR_BUNDLES = 2;

	// Token: 0x040016A7 RID: 5799
	public const int NUM_CARD_BUNDLES = 3;

	// Token: 0x040016A8 RID: 5800
	public const int NUM_CARDBACKS_BUNDLES = 1;

	// Token: 0x040016A9 RID: 5801
	public const int NUM_CARDTEXTURES_BUNDLES = 3;

	// Token: 0x040016AA RID: 5802
	public const int NUM_PREMIUMMATERIALS_BUNDLES = 2;

	// Token: 0x040016AB RID: 5803
	public const int NUM_GAMEOBJECTS_BUNDLES = 2;

	// Token: 0x040016AC RID: 5804
	public const int NUM_MOVIE_BUNDLES = 1;

	// Token: 0x040016AD RID: 5805
	public const int NUM_SOUND_BUNDLES = 2;

	// Token: 0x040016AE RID: 5806
	public const int NUM_SOUNDPREFAB_BUNDLES = 1;

	// Token: 0x040016AF RID: 5807
	public const int NUM_SPELL_BUNDLES = 3;

	// Token: 0x040016B0 RID: 5808
	public const int NUM_DOWNLOADABLE_SOUND_LOCALE_BUNDLES = 3;

	// Token: 0x040016B1 RID: 5809
	public const int NUM_DOWNLOADABLE_SPELL_LOCALE_BUNDLES = 9;

	// Token: 0x040016B2 RID: 5810
	public static readonly bool UseSharedDependencyBundle = true;

	// Token: 0x040016B3 RID: 5811
	public static readonly Map<AssetFamily, AssetFamilyBundleInfo> FamilyInfo = new Map<AssetFamily, AssetFamilyBundleInfo>
	{
		{
			AssetFamily.Actor,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(GameObject),
				BundleName = "actors",
				NumberOfBundles = 2
			}
		},
		{
			AssetFamily.AudioClip,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(GameObject),
				BundleName = "sounds",
				NumberOfBundles = 2,
				NumberOfDownloadableLocaleBundles = 3
			}
		},
		{
			AssetFamily.Board,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(GameObject),
				BundleName = "boards"
			}
		},
		{
			AssetFamily.CardBack,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(GameObject),
				BundleName = "cardbacks",
				NumberOfBundles = 1,
				Updatable = true
			}
		},
		{
			AssetFamily.CardPrefab,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(GameObject),
				BundleName = "cards",
				NumberOfBundles = 3
			}
		},
		{
			AssetFamily.CardPremium,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(Material),
				BundleName = "premiummaterials",
				NumberOfBundles = 2
			}
		},
		{
			AssetFamily.CardTexture,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(Texture),
				BundleName = "cardtextures",
				NumberOfBundles = 3
			}
		},
		{
			AssetFamily.CardXML,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(TextAsset),
				BundleName = "cardxml",
				Updatable = true
			}
		},
		{
			AssetFamily.FontDef,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(GameObject),
				BundleName = "fonts"
			}
		},
		{
			AssetFamily.GameObject,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(GameObject),
				BundleName = "gameobjects",
				NumberOfBundles = 2
			}
		},
		{
			AssetFamily.Movie,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(MovieTexture),
				BundleName = "movies",
				NumberOfBundles = 1
			}
		},
		{
			AssetFamily.Screen,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(GameObject),
				BundleName = "uiscreens"
			}
		},
		{
			AssetFamily.Sound,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(GameObject),
				BundleName = "soundprefabs",
				NumberOfBundles = 1
			}
		},
		{
			AssetFamily.Spell,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(GameObject),
				BundleName = "spells",
				NumberOfBundles = 3
			}
		},
		{
			AssetFamily.Texture,
			new AssetFamilyBundleInfo
			{
				TypeOf = typeof(Texture),
				BundleName = "textures"
			}
		}
	};
}
