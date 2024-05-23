using System;
using System.ComponentModel;

// Token: 0x020000C7 RID: 199
public enum Option
{
	// Token: 0x04000539 RID: 1337
	INVALID,
	// Token: 0x0400053A RID: 1338
	[Description("clientOptionsVersion")]
	CLIENT_OPTIONS_VERSION,
	// Token: 0x0400053B RID: 1339
	[Description("sound")]
	SOUND,
	// Token: 0x0400053C RID: 1340
	[Description("music")]
	MUSIC,
	// Token: 0x0400053D RID: 1341
	[Description("cursor")]
	CURSOR,
	// Token: 0x0400053E RID: 1342
	[Description("hud")]
	HUD,
	// Token: 0x0400053F RID: 1343
	[Description("streaming")]
	STREAMING,
	// Token: 0x04000540 RID: 1344
	[Description("soundvolume")]
	SOUND_VOLUME,
	// Token: 0x04000541 RID: 1345
	[Description("musicvolume")]
	MUSIC_VOLUME,
	// Token: 0x04000542 RID: 1346
	[Description("graphicswidth")]
	GFX_WIDTH,
	// Token: 0x04000543 RID: 1347
	[Description("graphicsheight")]
	GFX_HEIGHT,
	// Token: 0x04000544 RID: 1348
	[Description("graphicsfullscreen")]
	GFX_FULLSCREEN,
	// Token: 0x04000545 RID: 1349
	[Description("hasseencinematic")]
	HAS_SEEN_CINEMATIC,
	// Token: 0x04000546 RID: 1350
	[Description("graphicsquality")]
	GFX_QUALITY,
	// Token: 0x04000547 RID: 1351
	[Description("fakepackopening")]
	FAKE_PACK_OPENING,
	// Token: 0x04000548 RID: 1352
	[Description("fakepackcount")]
	FAKE_PACK_COUNT,
	// Token: 0x04000549 RID: 1353
	[Description("healthygamingdebug")]
	HEALTHY_GAMING_DEBUG,
	// Token: 0x0400054A RID: 1354
	[Description("laststate")]
	LAST_SCENE_MODE,
	// Token: 0x0400054B RID: 1355
	[Description("locale")]
	LOCALE,
	// Token: 0x0400054C RID: 1356
	[Description("idlekicker")]
	IDLE_KICKER,
	// Token: 0x0400054D RID: 1357
	[Description("idlekicktime")]
	IDLE_KICK_TIME,
	// Token: 0x0400054E RID: 1358
	[Description("backgroundsound")]
	BACKGROUND_SOUND,
	// Token: 0x0400054F RID: 1359
	[Description("preferredregion")]
	PREFERRED_REGION,
	// Token: 0x04000550 RID: 1360
	[Description("forceShowIks")]
	FORCE_SHOW_IKS,
	// Token: 0x04000551 RID: 1361
	[Description("peguidebug")]
	PEGUI_DEBUG,
	// Token: 0x04000552 RID: 1362
	[Description("nearbyplayers2")]
	NEARBY_PLAYERS,
	// Token: 0x04000553 RID: 1363
	[Description("wincameraclear")]
	GFX_WIN_CAMERA_CLEAR,
	// Token: 0x04000554 RID: 1364
	[Description("msaa")]
	GFX_MSAA,
	// Token: 0x04000555 RID: 1365
	[Description("fxaa")]
	GFX_FXAA,
	// Token: 0x04000556 RID: 1366
	[Description("targetframerate")]
	GFX_TARGET_FRAME_RATE,
	// Token: 0x04000557 RID: 1367
	[Description("vsync")]
	GFX_VSYNC,
	// Token: 0x04000558 RID: 1368
	[Description("cardback")]
	CARD_BACK,
	// Token: 0x04000559 RID: 1369
	[Description("cardback2")]
	CARD_BACK2,
	// Token: 0x0400055A RID: 1370
	[Description("localtutorialprogress")]
	LOCAL_TUTORIAL_PROGRESS,
	// Token: 0x0400055B RID: 1371
	[Description("connecttobnet")]
	CONNECT_TO_AURORA,
	// Token: 0x0400055C RID: 1372
	[Description("seasonEndThreshold")]
	SEASON_END_THRESHOLD,
	// Token: 0x0400055D RID: 1373
	[Description("reconnect")]
	RECONNECT,
	// Token: 0x0400055E RID: 1374
	[Description("reconnectTimeout")]
	RECONNECT_TIMEOUT,
	// Token: 0x0400055F RID: 1375
	[Description("reconnectRetryTime")]
	RECONNECT_RETRY_TIME,
	// Token: 0x04000560 RID: 1376
	[Description("changedcardsdata")]
	CHANGED_CARDS_DATA,
	// Token: 0x04000561 RID: 1377
	[Description("kelthuzadtaunts")]
	KELTHUZADTAUNTS,
	// Token: 0x04000562 RID: 1378
	[Description("winposx")]
	GFX_WIN_POSX,
	// Token: 0x04000563 RID: 1379
	[Description("winposy")]
	GFX_WIN_POSY,
	// Token: 0x04000564 RID: 1380
	[Description("preferredcdnindex")]
	PREFERRED_CDN_INDEX,
	// Token: 0x04000565 RID: 1381
	[Description("lastfaileddopversion")]
	LAST_FAILED_DOP_VERSION,
	// Token: 0x04000566 RID: 1382
	[Description("touchmode")]
	TOUCH_MODE,
	// Token: 0x04000567 RID: 1383
	[Description("gfxdevicewarning")]
	SHOWN_GFX_DEVICE_WARNING,
	// Token: 0x04000568 RID: 1384
	[Description("intro")]
	INTRO,
	// Token: 0x04000569 RID: 1385
	[Description("tutoriallostprogress")]
	TUTORIAL_LOST_PROGRESS,
	// Token: 0x0400056A RID: 1386
	[Description("errorScreen")]
	ERROR_SCREEN,
	// Token: 0x0400056B RID: 1387
	[Description("innkeepersSpecialViews")]
	IKS_VIEWS,
	// Token: 0x0400056C RID: 1388
	[Description("innkeepersSpecialLastDownloadTime")]
	IKS_LAST_DOWNLOAD_TIME,
	// Token: 0x0400056D RID: 1389
	[Description("innkeepersSpecialLastResponse")]
	IKS_LAST_DOWNLOAD_RESPONSE,
	// Token: 0x0400056E RID: 1390
	[Description("innkeepersSpecialCacheAge")]
	IKS_CACHE_AGE,
	// Token: 0x0400056F RID: 1391
	[Description("cheatHistory")]
	CHEAT_HISTORY,
	// Token: 0x04000570 RID: 1392
	[Description("preloadCardAssets")]
	PRELOAD_CARD_ASSETS,
	// Token: 0x04000571 RID: 1393
	[Description("collectionPremiumType")]
	COLLECTION_PREMIUM_TYPE,
	// Token: 0x04000572 RID: 1394
	[Description("devTimescale")]
	DEV_TIMESCALE,
	// Token: 0x04000573 RID: 1395
	[Description("innkeepersSpecialLastShownAd")]
	IKS_LAST_SHOWN_AD,
	// Token: 0x04000574 RID: 1396
	[Description("seenPackProductList")]
	SEEN_PACK_PRODUCT_LIST,
	// Token: 0x04000575 RID: 1397
	[Description("showStandardOnly")]
	SHOW_STANDARD_ONLY,
	// Token: 0x04000576 RID: 1398
	[Description("showSetRotationIntroVisuals")]
	SHOW_SET_ROTATION_INTRO_VISUALS,
	// Token: 0x04000577 RID: 1399
	[Description("serverOptionsVersion")]
	SERVER_OPTIONS_VERSION,
	// Token: 0x04000578 RID: 1400
	[Description("pagemouseovers")]
	PAGE_MOUSE_OVERS,
	// Token: 0x04000579 RID: 1401
	[Description("covermouseovers")]
	COVER_MOUSE_OVERS,
	// Token: 0x0400057A RID: 1402
	[Description("aimode")]
	AI_MODE,
	// Token: 0x0400057B RID: 1403
	[Description("practicetipporgress")]
	TIP_PRACTICE_PROGRESS,
	// Token: 0x0400057C RID: 1404
	[Description("playtipprogress")]
	TIP_PLAY_PROGRESS,
	// Token: 0x0400057D RID: 1405
	[Description("forgetipprogress")]
	TIP_FORGE_PROGRESS,
	// Token: 0x0400057E RID: 1406
	[Description("lastChosenPreconHero")]
	LAST_PRECON_HERO_CHOSEN,
	// Token: 0x0400057F RID: 1407
	[Description("lastChosenCustomDeck")]
	LAST_CUSTOM_DECK_CHOSEN,
	// Token: 0x04000580 RID: 1408
	[Description("selectedAdventure")]
	SELECTED_ADVENTURE,
	// Token: 0x04000581 RID: 1409
	[Description("selectedAdventureMode")]
	SELECTED_ADVENTURE_MODE,
	// Token: 0x04000582 RID: 1410
	[Description("lastselectedbooster")]
	LAST_SELECTED_STORE_BOOSTER_ID,
	// Token: 0x04000583 RID: 1411
	[Description("lastselectedadventure")]
	LAST_SELECTED_STORE_ADVENTURE_ID,
	// Token: 0x04000584 RID: 1412
	[Description("lastselectedhero")]
	LAST_SELECTED_STORE_HERO_ID,
	// Token: 0x04000585 RID: 1413
	[Description("seenTB")]
	LATEST_SEEN_TAVERNBRAWL_SEASON,
	// Token: 0x04000586 RID: 1414
	[Description("seenTBScreen")]
	LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD,
	// Token: 0x04000587 RID: 1415
	[Description("seenCrazyRulesQuote")]
	TIMES_SEEN_TAVERNBRAWL_CRAZY_RULES_QUOTE,
	// Token: 0x04000588 RID: 1416
	[Description("setRotationIntroProgress")]
	SET_ROTATION_INTRO_PROGRESS,
	// Token: 0x04000589 RID: 1417
	[Description("timesMousedOverSwitchFormatButton")]
	TIMES_MOUSED_OVER_SWITCH_FORMAT_BUTTON,
	// Token: 0x0400058A RID: 1418
	[Description("hasclickedtournament")]
	HAS_CLICKED_TOURNAMENT,
	// Token: 0x0400058B RID: 1419
	[Description("hasopenedbooster")]
	HAS_OPENED_BOOSTER,
	// Token: 0x0400058C RID: 1420
	[Description("hasseentournament")]
	HAS_SEEN_TOURNAMENT,
	// Token: 0x0400058D RID: 1421
	[Description("hasseencollectionmanager")]
	HAS_SEEN_COLLECTIONMANAGER,
	// Token: 0x0400058E RID: 1422
	[Description("justfinishedtutorial")]
	JUST_FINISHED_TUTORIAL,
	// Token: 0x0400058F RID: 1423
	[Description("showadvancedcollectionmanager")]
	SHOW_ADVANCED_COLLECTIONMANAGER,
	// Token: 0x04000590 RID: 1424
	[Description("hasseenpracticetray")]
	HAS_SEEN_PRACTICE_TRAY,
	// Token: 0x04000591 RID: 1425
	[Description("firstHubVisitPastTutorial")]
	HAS_SEEN_HUB,
	// Token: 0x04000592 RID: 1426
	[Description("firstdeckcomplete")]
	HAS_FINISHED_A_DECK,
	// Token: 0x04000593 RID: 1427
	[Description("hasseenforge")]
	HAS_SEEN_FORGE,
	// Token: 0x04000594 RID: 1428
	[Description("hasseenforgeherochoice")]
	HAS_SEEN_FORGE_HERO_CHOICE,
	// Token: 0x04000595 RID: 1429
	[Description("hasseenforgecardchoice")]
	HAS_SEEN_FORGE_CARD_CHOICE,
	// Token: 0x04000596 RID: 1430
	[Description("hasseenforgecardchoice2")]
	HAS_SEEN_FORGE_CARD_CHOICE2,
	// Token: 0x04000597 RID: 1431
	[Description("hasseenforgeplaymode")]
	HAS_SEEN_FORGE_PLAY_MODE,
	// Token: 0x04000598 RID: 1432
	[Description("hasseenforge1win")]
	HAS_SEEN_FORGE_1WIN,
	// Token: 0x04000599 RID: 1433
	[Description("hasseenforge2loss")]
	HAS_SEEN_FORGE_2LOSS,
	// Token: 0x0400059A RID: 1434
	[Description("hasseenforgeretire")]
	HAS_SEEN_FORGE_RETIRE,
	// Token: 0x0400059B RID: 1435
	[Description("hasseenmulligan")]
	HAS_SEEN_MULLIGAN,
	// Token: 0x0400059C RID: 1436
	[Description("hasSeenExpertAI")]
	HAS_SEEN_EXPERT_AI,
	// Token: 0x0400059D RID: 1437
	[Description("hasSeenExpertAIUnlock")]
	HAS_SEEN_EXPERT_AI_UNLOCK,
	// Token: 0x0400059E RID: 1438
	[Description("hasseendeckhelper")]
	HAS_SEEN_DECK_HELPER,
	// Token: 0x0400059F RID: 1439
	[Description("hasSeenPackOpening")]
	HAS_SEEN_PACK_OPENING,
	// Token: 0x040005A0 RID: 1440
	[Description("hasSeenPracticeMode")]
	HAS_SEEN_PRACTICE_MODE,
	// Token: 0x040005A1 RID: 1441
	[Description("hasSeenCustomDeckPicker")]
	HAS_SEEN_CUSTOM_DECK_PICKER,
	// Token: 0x040005A2 RID: 1442
	[Description("hasSeenAllBasicClassCardsComplete")]
	HAS_SEEN_ALL_BASIC_CLASS_CARDS_COMPLETE,
	// Token: 0x040005A3 RID: 1443
	[Description("hasBeenNudgedToCM")]
	HAS_BEEN_NUDGED_TO_CM,
	// Token: 0x040005A4 RID: 1444
	[Description("hasAddedCardsToDeck")]
	HAS_ADDED_CARDS_TO_DECK,
	// Token: 0x040005A5 RID: 1445
	[Description("tipCraftingUnlocked")]
	TIP_CRAFTING_UNLOCKED,
	// Token: 0x040005A6 RID: 1446
	[Description("hasPlayedExpertAI")]
	HAS_PLAYED_EXPERT_AI,
	// Token: 0x040005A7 RID: 1447
	[Description("hasDisenchanted")]
	HAS_DISENCHANTED,
	// Token: 0x040005A8 RID: 1448
	[Description("hasSeenShowAllCardsReminder")]
	HAS_SEEN_SHOW_ALL_CARDS_REMINDER,
	// Token: 0x040005A9 RID: 1449
	[Description("hasSeenCraftingInstruction")]
	HAS_SEEN_CRAFTING_INSTRUCTION,
	// Token: 0x040005AA RID: 1450
	[Description("hasCrafted")]
	HAS_CRAFTED,
	// Token: 0x040005AB RID: 1451
	[Description("inRankedPlayMode")]
	IN_RANKED_PLAY_MODE,
	// Token: 0x040005AC RID: 1452
	[Description("hasSeenTheCoin")]
	HAS_SEEN_THE_COIN,
	// Token: 0x040005AD RID: 1453
	[Description("hasseen100goldReminder")]
	HAS_SEEN_100g_REMINDER,
	// Token: 0x040005AE RID: 1454
	[Description("hasSeenGoldQtyInstruction")]
	HAS_SEEN_GOLD_QTY_INSTRUCTION,
	// Token: 0x040005AF RID: 1455
	[Description("hasSeenLevel3")]
	HAS_SEEN_LEVEL_3,
	// Token: 0x040005B0 RID: 1456
	[Description("hasLostInArena")]
	HAS_LOST_IN_ARENA,
	// Token: 0x040005B1 RID: 1457
	[Description("hasRunOutOfQuests")]
	HAS_RUN_OUT_OF_QUESTS,
	// Token: 0x040005B2 RID: 1458
	[Description("hasAckedArenaRewards")]
	HAS_ACKED_ARENA_REWARDS,
	// Token: 0x040005B3 RID: 1459
	[Description("hasSeenStealthTaunter")]
	HAS_SEEN_STEALTH_TAUNTER,
	// Token: 0x040005B4 RID: 1460
	[Description("friendslistrequestsectionhide")]
	FRIENDS_LIST_REQUEST_SECTION_HIDE,
	// Token: 0x040005B5 RID: 1461
	[Description("friendslistcurrentgamesectionhide")]
	FRIENDS_LIST_CURRENTGAME_SECTION_HIDE,
	// Token: 0x040005B6 RID: 1462
	[Description("friendslistfriendsectionhide")]
	FRIENDS_LIST_FRIEND_SECTION_HIDE,
	// Token: 0x040005B7 RID: 1463
	[Description("friendslistnearbysectionhide")]
	FRIENDS_LIST_NEARBYPLAYER_SECTION_HIDE,
	// Token: 0x040005B8 RID: 1464
	[Description("friendslistrecruitsectionhide")]
	FRIENDS_LIST_RECRUIT_SECTION_HIDE,
	// Token: 0x040005B9 RID: 1465
	[Description("hasSeenHeroicWarning")]
	HAS_SEEN_HEROIC_WARNING,
	// Token: 0x040005BA RID: 1466
	[Description("hasSeenNaxx")]
	HAS_SEEN_NAXX,
	// Token: 0x040005BB RID: 1467
	[Description("hasEnteredNaxx")]
	HAS_ENTERED_NAXX,
	// Token: 0x040005BC RID: 1468
	[Description("hasSeenNaxxClassChallenge")]
	HAS_SEEN_NAXX_CLASS_CHALLENGE,
	// Token: 0x040005BD RID: 1469
	[Description("bundleJustPurchaseInHub")]
	BUNDLE_JUST_PURCHASE_IN_HUB,
	// Token: 0x040005BE RID: 1470
	[Description("hasPlayedNaxx")]
	HAS_PLAYED_NAXX,
	// Token: 0x040005BF RID: 1471
	[Description("spectatoropenjoin")]
	SPECTATOR_OPEN_JOIN,
	// Token: 0x040005C0 RID: 1472
	[Description("hasstartedadeck")]
	HAS_STARTED_A_DECK,
	// Token: 0x040005C1 RID: 1473
	[Description("hasseencollectionmanagerafterpractice")]
	HAS_SEEN_COLLECTIONMANAGER_AFTER_PRACTICE,
	// Token: 0x040005C2 RID: 1474
	[Description("hasSeenBRM")]
	HAS_SEEN_BRM,
	// Token: 0x040005C3 RID: 1475
	[Description("hasSeenLOE")]
	HAS_SEEN_LOE,
	// Token: 0x040005C4 RID: 1476
	[Description("hasClickedManaTab")]
	HAS_CLICKED_MANA_TAB,
	// Token: 0x040005C5 RID: 1477
	[Description("hasseenforgemaxwin")]
	HAS_SEEN_FORGE_MAX_WIN,
	// Token: 0x040005C6 RID: 1478
	[Description("hasheardtgtpackvo")]
	HAS_HEARD_TGT_PACK_VO,
	// Token: 0x040005C7 RID: 1479
	[Description("hasseenloestaffdisappear")]
	HAS_SEEN_LOE_STAFF_DISAPPEAR,
	// Token: 0x040005C8 RID: 1480
	[Description("hasseenloestaffreappear")]
	HAS_SEEN_LOE_STAFF_REAPPEAR,
	// Token: 0x040005C9 RID: 1481
	[Description("hasSeenUnlockAllHeroesTransition")]
	HAS_SEEN_UNLOCK_ALL_HEROES_TRANSITION,
	// Token: 0x040005CA RID: 1482
	[Description("createdFirstDeckForClass")]
	SKIP_DECK_TEMPLATE_PAGE_FOR_CLASS_FLAGS,
	// Token: 0x040005CB RID: 1483
	[Description("hasSeenDeckTemplateScreen")]
	HAS_SEEN_DECK_TEMPLATE_SCREEN,
	// Token: 0x040005CC RID: 1484
	[Description("hasClickedDeckTemplateReplace")]
	HAS_CLICKED_DECK_TEMPLATE_REPLACE,
	// Token: 0x040005CD RID: 1485
	[Description("hasSeenDeckTemplateGhostCard")]
	HAS_SEEN_DECK_TEMPLATE_GHOST_CARD,
	// Token: 0x040005CE RID: 1486
	[Description("hasRemovedCardFromDeck")]
	HAS_REMOVED_CARD_FROM_DECK,
	// Token: 0x040005CF RID: 1487
	[Description("hasSeenDeleteDeckReminder")]
	HAS_SEEN_DELETE_DECK_REMINDER,
	// Token: 0x040005D0 RID: 1488
	[Description("hasClickedCollectionButtonForNewCard")]
	HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_CARD,
	// Token: 0x040005D1 RID: 1489
	[Description("hasClickedCollectionButtonForNewDeck")]
	HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_DECK,
	// Token: 0x040005D2 RID: 1490
	[Description("inWildMode")]
	IN_WILD_MODE,
	// Token: 0x040005D3 RID: 1491
	[Description("hasSeenWildModeVO")]
	HAS_SEEN_WILD_MODE_VO,
	// Token: 0x040005D4 RID: 1492
	[Description("hasSeenStandardModeTutorial")]
	HAS_SEEN_STANDARD_MODE_TUTORIAL,
	// Token: 0x040005D5 RID: 1493
	[Description("needsToMakeStandardDeck")]
	NEEDS_TO_MAKE_STANDARD_DECK,
	// Token: 0x040005D6 RID: 1494
	[Description("hasSeenInvalidRotatedCard")]
	HAS_SEEN_INVALID_ROTATED_CARD,
	// Token: 0x040005D7 RID: 1495
	[Description("showSwitchToWildOnPlayScreen")]
	SHOW_SWITCH_TO_WILD_ON_PLAY_SCREEN,
	// Token: 0x040005D8 RID: 1496
	[Description("showSwitchToWildOnCreateDeck")]
	SHOW_SWITCH_TO_WILD_ON_CREATE_DECK,
	// Token: 0x040005D9 RID: 1497
	[Description("showWildDisclaimerPopupOnCreateDeck")]
	SHOW_WILD_DISCLAIMER_POPUP_ON_CREATE_DECK,
	// Token: 0x040005DA RID: 1498
	[Description("hasSeenBasicDeckWarning")]
	HAS_SEEN_BASIC_DECK_WARNING,
	// Token: 0x040005DB RID: 1499
	[Description("glowCollectionButtonAfterSetRotation")]
	GLOW_COLLECTION_BUTTON_AFTER_SET_ROTATION
}
