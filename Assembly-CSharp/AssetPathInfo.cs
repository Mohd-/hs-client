using System;

// Token: 0x020002D8 RID: 728
public class AssetPathInfo
{
	// Token: 0x040016D6 RID: 5846
	public static readonly Map<AssetFamily, AssetFamilyPathInfo> FamilyInfo = new Map<AssetFamily, AssetFamilyPathInfo>
	{
		{
			AssetFamily.Actor,
			new AssetFamilyPathInfo
			{
				format = "Data/Actors/{0}.unity3d",
				sourceDir = "Assets/Game/Actors",
				exts = new string[]
				{
					"prefab"
				}
			}
		},
		{
			AssetFamily.AudioClip,
			new AssetFamilyPathInfo
			{
				format = "Data/AudioClips/{0}.unity3d",
				sourceDir = "Assets",
				exts = new string[]
				{
					"wav",
					"ogg"
				}
			}
		},
		{
			AssetFamily.Board,
			new AssetFamilyPathInfo
			{
				format = "Data/Boards/{0}.unity3d",
				sourceDir = "Assets/Game/Boards",
				exts = new string[]
				{
					"prefab"
				}
			}
		},
		{
			AssetFamily.CardBack,
			new AssetFamilyPathInfo
			{
				format = "Data/CardBacks/{0}.unity3d",
				sourceDir = "Assets/Game/CardBacks",
				exts = new string[]
				{
					"prefab"
				}
			}
		},
		{
			AssetFamily.CardPrefab,
			new AssetFamilyPathInfo
			{
				format = "Data/Cards/{0}_prefab.unity3d",
				sourceDir = "Assets/Game/Cards",
				exts = new string[]
				{
					"prefab"
				}
			}
		},
		{
			AssetFamily.CardPremium,
			new AssetFamilyPathInfo
			{
				format = "{0}",
				sourceDir = string.Empty,
				exts = new string[]
				{
					"mat"
				}
			}
		},
		{
			AssetFamily.CardTexture,
			new AssetFamilyPathInfo
			{
				format = "{0}",
				sourceDir = string.Empty,
				exts = new string[]
				{
					"psd",
					"png",
					"tif"
				}
			}
		},
		{
			AssetFamily.CardXML,
			new AssetFamilyPathInfo
			{
				format = "Data/Cards/{0}_xml.unity3d",
				sourceDir = "Assets/Game/Cards",
				exts = new string[]
				{
					"xml"
				}
			}
		},
		{
			AssetFamily.FontDef,
			new AssetFamilyPathInfo
			{
				format = "Data/Fonts/{0}.unity3d",
				sourceDir = "Assets/Game/Fonts",
				exts = new string[]
				{
					"prefab"
				}
			}
		},
		{
			AssetFamily.GameObject,
			new AssetFamilyPathInfo
			{
				format = "Data/GameObjects/{0}.unity3d",
				sourceDir = "Assets/Game/GameObjects",
				exts = new string[]
				{
					"prefab"
				}
			}
		},
		{
			AssetFamily.Movie,
			new AssetFamilyPathInfo
			{
				format = "Data/Movies/{0}.unity3d",
				sourceDir = "Assets/Game/Movies",
				exts = new string[]
				{
					"ogv"
				}
			}
		},
		{
			AssetFamily.Screen,
			new AssetFamilyPathInfo
			{
				format = "Data/UIScreens/{0}.unity3d",
				sourceDir = "Assets/Game/UIScreens",
				exts = new string[]
				{
					"prefab"
				}
			}
		},
		{
			AssetFamily.Sound,
			new AssetFamilyPathInfo
			{
				format = "Data/Sounds/{0}.unity3d",
				sourceDir = "Assets/Game/Sounds",
				exts = new string[]
				{
					"prefab"
				}
			}
		},
		{
			AssetFamily.Spell,
			new AssetFamilyPathInfo
			{
				format = "Data/Spells/{0}.unity3d",
				sourceDir = string.Empty,
				exts = new string[]
				{
					"prefab"
				}
			}
		},
		{
			AssetFamily.Texture,
			new AssetFamilyPathInfo
			{
				format = "Data/Textures/{0}.unity3d",
				sourceDir = "Assets/Game/Textures",
				exts = new string[]
				{
					"psd"
				}
			}
		}
	};
}
