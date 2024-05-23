using System;
using UnityEngine;

// Token: 0x0200040F RID: 1039
public class StoreMobilePurchase
{
	// Token: 0x060034DA RID: 13530 RVA: 0x00107167 File Offset: 0x00105367
	public static void ClearProductList()
	{
		StoreMobilePurchase.ClearMobileStoreProducts();
	}

	// Token: 0x060034DB RID: 13531 RVA: 0x00107170 File Offset: 0x00105370
	public static void SetGameAccountIdAndRegion(ulong gameAccountId, int gameRegion)
	{
		Log.Yim.Print(string.Concat(new object[]
		{
			"SetGameAccountIdAndRegion(",
			gameAccountId,
			", ",
			gameRegion,
			")"
		}), new object[0]);
		StoreMobilePurchase.SetBattleNetGameAccountIdAndRegion(gameAccountId, gameRegion);
	}

	// Token: 0x060034DC RID: 13532 RVA: 0x001071C9 File Offset: 0x001053C9
	public static void AddProductById(string mobileProductId)
	{
		StoreMobilePurchase.AddProductToAllProductsList(mobileProductId);
	}

	// Token: 0x060034DD RID: 13533 RVA: 0x001071D1 File Offset: 0x001053D1
	public static void ValidateAllProducts(string gameObjectName)
	{
		StoreMobilePurchase.ValidateProducts(gameObjectName);
	}

	// Token: 0x060034DE RID: 13534 RVA: 0x001071D9 File Offset: 0x001053D9
	public static double GetProductPrice(string mobileProductId)
	{
		return StoreMobilePurchase.ProductPriceById(mobileProductId);
	}

	// Token: 0x060034DF RID: 13535 RVA: 0x001071E1 File Offset: 0x001053E1
	public static string GetLocalizedProductPrice(string mobileProductId)
	{
		return StoreMobilePurchase.LocalizedProductPriceById(mobileProductId);
	}

	// Token: 0x060034E0 RID: 13536 RVA: 0x001071E9 File Offset: 0x001053E9
	public static string GetLocalizedPrice(double price)
	{
		return StoreMobilePurchase.LocalizedPrice(price);
	}

	// Token: 0x060034E1 RID: 13537 RVA: 0x001071F1 File Offset: 0x001053F1
	public static void PurchaseProductById(string mobileProductId)
	{
		StoreMobilePurchase.RequestPurchaseByProductId(mobileProductId, string.Empty);
	}

	// Token: 0x060034E2 RID: 13538 RVA: 0x001071FE File Offset: 0x001053FE
	public static void PurchaseProductById(string mobileProductId, string transactionId)
	{
		StoreMobilePurchase.RequestPurchaseByProductId(mobileProductId, transactionId);
	}

	// Token: 0x060034E3 RID: 13539 RVA: 0x00107207 File Offset: 0x00105407
	public static bool IsWaitingForPurchase()
	{
		return StoreMobilePurchase.DeviceIsWaitingForPurchase();
	}

	// Token: 0x060034E4 RID: 13540 RVA: 0x0010720E File Offset: 0x0010540E
	public static StoreMobilePurchase.PURCHASE_STATUS_CODE GetStatusCodeOfLastPurchase()
	{
		return (StoreMobilePurchase.PURCHASE_STATUS_CODE)StoreMobilePurchase.GetLastPurchaseStatusCode();
	}

	// Token: 0x060034E5 RID: 13541 RVA: 0x00107215 File Offset: 0x00105415
	public static string GetStatusDescriptionOfLastPurchase()
	{
		return StoreMobilePurchase.GetLastPurchaseStatusDescription();
	}

	// Token: 0x060034E6 RID: 13542 RVA: 0x0010721C File Offset: 0x0010541C
	public static int NumReceiptsAvailable()
	{
		return StoreMobilePurchase.GetNumReceiptsAvailable();
	}

	// Token: 0x060034E7 RID: 13543 RVA: 0x00107223 File Offset: 0x00105423
	public static IntPtr GetNextReceipt()
	{
		return StoreMobilePurchase.NextReceipt();
	}

	// Token: 0x060034E8 RID: 13544 RVA: 0x0010722A File Offset: 0x0010542A
	public static void DismissNextReceipt(bool consume)
	{
		StoreMobilePurchase.DismissReceipt(consume);
	}

	// Token: 0x060034E9 RID: 13545 RVA: 0x00107232 File Offset: 0x00105432
	public static void FinishTransactionForId(string transactionId)
	{
		Debug.LogError("FinishTransactionForId should be not be called. Handled natively on mobile platforms.");
	}

	// Token: 0x060034EA RID: 13546 RVA: 0x0010723E File Offset: 0x0010543E
	public static void GamePurchaseStatusResponse(string transactionId, bool isSuccess)
	{
		StoreMobilePurchase.OnGamePurchaseStatusResponse(transactionId, isSuccess);
	}

	// Token: 0x060034EB RID: 13547 RVA: 0x00107247 File Offset: 0x00105447
	public static void ThirdPartyPurchaseStatus(string transactionId, Network.ThirdPartyPurchaseStatusResponse.PurchaseStatus status)
	{
		StoreMobilePurchase.OnThirdPartyPurchaseStatus(transactionId, (int)status);
	}

	// Token: 0x060034EC RID: 13548 RVA: 0x00107250 File Offset: 0x00105450
	public static void Reset()
	{
		if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD)
		{
			StoreMobilePurchase.OnReset();
		}
	}

	// Token: 0x060034ED RID: 13549 RVA: 0x00107271 File Offset: 0x00105471
	public static void TransactionStatusNotReady()
	{
		StoreMobilePurchase.StoreTransactionStatusNotReady();
	}

	// Token: 0x060034EE RID: 13550 RVA: 0x00107278 File Offset: 0x00105478
	public static void TransactionStatusReady()
	{
		Log.Yim.Print("TransactionStatusReady", new object[0]);
		StoreMobilePurchase.StoreTransactionStatusReady();
	}

	// Token: 0x060034EF RID: 13551 RVA: 0x00107294 File Offset: 0x00105494
	public static void WaitingOnThirdPartyReceipt(string mobileProductId)
	{
		StoreMobilePurchase.RequestThirdPartyReceipt(mobileProductId);
	}

	// Token: 0x060034F0 RID: 13552 RVA: 0x0010729C File Offset: 0x0010549C
	public static void ProcessNextPendingTransaction()
	{
		StoreMobilePurchase.SubmitNextPendingTransaction();
	}

	// Token: 0x060034F1 RID: 13553 RVA: 0x001072A3 File Offset: 0x001054A3
	public static string ThirdPartyUserId()
	{
		return StoreMobilePurchase.GetThirdPartyUserId();
	}

	// Token: 0x060034F2 RID: 13554 RVA: 0x001072AA File Offset: 0x001054AA
	private static void SetBattleNetGameAccountIdAndRegion(ulong gameAccountId, int gameRegion)
	{
	}

	// Token: 0x060034F3 RID: 13555 RVA: 0x001072AC File Offset: 0x001054AC
	private static void ClearMobileStoreProducts()
	{
	}

	// Token: 0x060034F4 RID: 13556 RVA: 0x001072AE File Offset: 0x001054AE
	private static void AddProductToAllProductsList(string mobileProductId)
	{
	}

	// Token: 0x060034F5 RID: 13557 RVA: 0x001072B0 File Offset: 0x001054B0
	private static void ValidateProducts(string gameObjectName)
	{
	}

	// Token: 0x060034F6 RID: 13558 RVA: 0x001072B2 File Offset: 0x001054B2
	private static void RequestPurchaseByProductId(string mobileProductId, string transactionId)
	{
	}

	// Token: 0x060034F7 RID: 13559 RVA: 0x001072B4 File Offset: 0x001054B4
	private static double ProductPriceById(string mobileProductId)
	{
		return -1.0;
	}

	// Token: 0x060034F8 RID: 13560 RVA: 0x001072BF File Offset: 0x001054BF
	private static string LocalizedProductPriceById(string mobileProductId)
	{
		return string.Empty;
	}

	// Token: 0x060034F9 RID: 13561 RVA: 0x001072C6 File Offset: 0x001054C6
	private static string LocalizedPrice(double price)
	{
		return string.Empty;
	}

	// Token: 0x060034FA RID: 13562 RVA: 0x001072CD File Offset: 0x001054CD
	private static bool DeviceIsWaitingForPurchase()
	{
		return false;
	}

	// Token: 0x060034FB RID: 13563 RVA: 0x001072D0 File Offset: 0x001054D0
	private static int GetLastPurchaseStatusCode()
	{
		return 3;
	}

	// Token: 0x060034FC RID: 13564 RVA: 0x001072D3 File Offset: 0x001054D3
	private static string GetLastPurchaseStatusDescription()
	{
		return string.Empty;
	}

	// Token: 0x060034FD RID: 13565 RVA: 0x001072DA File Offset: 0x001054DA
	private static int GetNumReceiptsAvailable()
	{
		return 0;
	}

	// Token: 0x060034FE RID: 13566 RVA: 0x001072DD File Offset: 0x001054DD
	private static IntPtr NextReceipt()
	{
		return IntPtr.Zero;
	}

	// Token: 0x060034FF RID: 13567 RVA: 0x001072E4 File Offset: 0x001054E4
	private static void DismissReceipt(bool consume)
	{
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x001072E6 File Offset: 0x001054E6
	private static int ReceiptForTransactionId(string transactionId, out IntPtr data)
	{
		data = IntPtr.Zero;
		return 0;
	}

	// Token: 0x06003501 RID: 13569 RVA: 0x001072F0 File Offset: 0x001054F0
	private static string ReceiptStringForTransactionId(string transactionId)
	{
		return string.Empty;
	}

	// Token: 0x06003502 RID: 13570 RVA: 0x001072F7 File Offset: 0x001054F7
	private static void FinishTransactionId(string transactionId)
	{
	}

	// Token: 0x06003503 RID: 13571 RVA: 0x001072F9 File Offset: 0x001054F9
	private static void OnGamePurchaseStatusResponse(string transactionId, bool isSuccess)
	{
	}

	// Token: 0x06003504 RID: 13572 RVA: 0x001072FB File Offset: 0x001054FB
	private static void OnThirdPartyPurchaseStatus(string transactionId, int status)
	{
	}

	// Token: 0x06003505 RID: 13573 RVA: 0x001072FD File Offset: 0x001054FD
	private static void OnReset()
	{
	}

	// Token: 0x06003506 RID: 13574 RVA: 0x001072FF File Offset: 0x001054FF
	private static void StoreTransactionStatusNotReady()
	{
	}

	// Token: 0x06003507 RID: 13575 RVA: 0x00107301 File Offset: 0x00105501
	private static void StoreTransactionStatusReady()
	{
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x00107303 File Offset: 0x00105503
	private static void RequestThirdPartyReceipt(string mobileProductId)
	{
	}

	// Token: 0x06003509 RID: 13577 RVA: 0x00107305 File Offset: 0x00105505
	private static void SubmitNextPendingTransaction()
	{
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x00107307 File Offset: 0x00105507
	private static string GetThirdPartyUserId()
	{
		return string.Empty;
	}

	// Token: 0x02000AFE RID: 2814
	public enum PURCHASE_STATUS_CODE
	{
		// Token: 0x0400481B RID: 18459
		PURCHASE_SUCCESSFUL,
		// Token: 0x0400481C RID: 18460
		PURCHASE_FAILED,
		// Token: 0x0400481D RID: 18461
		PURCHASE_RECOVERED,
		// Token: 0x0400481E RID: 18462
		PURCHASE_NOT_AVAILABLE
	}
}
