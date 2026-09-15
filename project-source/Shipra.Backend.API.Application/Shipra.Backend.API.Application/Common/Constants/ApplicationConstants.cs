namespace Shipra.Backend.API.Application.Common.Constants;

public class ApplicationConstants
{
  public const string UserId = "UserId";
  public const string ClientId = "ClientId";
  public const decimal ClientDefaultTransactionCharges = 3;
  public const string ClientProfileImagePath = "/Profiles/";
  public const string ClientProductImage = "/Products/";
  //public const string ProductImagePath = "Product/ProductFiles\";
  public const string ClientStoreImagePath = "/Stores/";
  public const string CarrierImagePath = "Carrier/";
  public const string EmployeeImagePath = "Employee/";
  public const string ClientUploadOrder = "/Uploads/Orders/";
  public const string ClientUploadReturnOrder = "/Uploads/ReturnOrders/";
  public const string CarrierPaymentSettlement = "/Uploads/Account/CarrierSettlement/";
  public const string ClientUploadOrderPOD = "/OrderPOD/";
  public const string ShipraLogo = "https://app.shipra.io/static/media/navLogo.05e36c6be73752c7567d.png";
  public const string ShipraSupportEmail = "support@shipra.io";

  public const string S3BucketUrl = "https://shipra.io.s3.ap-south-1.amazonaws.com/";
  public const string StoreImagePlaceHolder = "https://shipprastatic.s3.ap-south-1.amazonaws.com/Shipra/Others/placeholder/27-06-2024/8da0caa9-4d68-4ade-beef-613d18b74a50_store.png";
  public const string ProductPlaceHolder = "https://shipprastatic.s3.ap-south-1.amazonaws.com/Shipra/Others//13-05-2025/4f4db1e1-e702-4af8-aa77-dc8a3863ab67_placeholderProductImage.jpg";
  #region ordertypefile url
  public const string OrderTypeRegular = @"https://shippra.s3.ap-south-1.amazonaws.com/Shipra/Order/20-02-2024/2f9cca36-9e84-41da-b41d-a30f3fb734a5_Sample_Regular.xlsx";
  public const string OrderTypeFullfilable = @"https://shippra.s3.ap-south-1.amazonaws.com/Shipra/Order/20-02-2024/08911c2e-d7eb-47e5-9e70-08a16568c371_Sample_Fulfillable.xlsx";
  #endregion

  #region carrier settlement
  public const string CarrierSettlementFileUrl = @"https://shippraio.s3.ap-south-1.amazonaws.com/Shipra/Carrier//16-07-2025/d2342de5-6041-46b0-8993-508d81ecd612_sample-carriersettlement .xlsx";

  #endregion 

  #region return report
  public const string ReturnReportFileUrl = @"https://shippraio.s3.ap-south-1.amazonaws.com/Shipra/Others//23-06-2025/cf3b6b98-617c-4030-b232-324235e5f8b1_returnReport-sample.xlsx";

  #endregion
  public const int WeightDimensionalFactor = 5000;
  #region  User role
  public const string SuperAdmin = "Super Admin";
  public const string Admin = "Admin";
  public const string Employee = "Employee";
  public const string Driver = "Driver";
  public const string SalePerson = "Sale Person";
  public const string SaleChannel = "Sale Channel";

  #endregion
  #region microservices constant url key 
  public const string CognitoKey = "Cognito";
  public const string ShipraServiceKey = "ShipraService";
  public const string IntegrationKey = "Integration";
  public const string IntegrationWebHookKey = "IntegrationWebHook";
  public const string AdminControlPanKey = "Admin";
  public const string ShopifyConfigKey = "ShopifyConfig";
  public const string API = "API";
  #endregion

  //DropDownPlaceHolder
  public const string DropDownPlaceHolderName = "Please select the option";
  public const int DropDownPlaceHolderId = 0;
  //ClassNames
  public const string DangerClassName = "danger"; //red
  public const string WarningClassName = "warning"; //yellow
  public const string SuccessClassName = "success"; //green

  public static string GetS3ClientFolderPattern(string clientId, string folder)
  {
    return $"Clients/{clientId}{folder}{DateTime.Today.ToString("dd-MM-yyyy") + "/"}";
  }

  public static readonly List<string> SubscriptionSkipCommandQueryList = new List<string>()
{
    "GetClientSubscription"
};
}
#region notification
public static class ApplicationConstantConfig
{
  public static readonly string[] EventNames =
   {
        "onordercreated",
        "onordertrackingstatus",
        "onassigncarrier"
    };

  public static List<EventCommandPairModel> EventCommandPairs = new List<EventCommandPairModel>()
   {
        new EventCommandPairModel(){EventName = "onordercreated",CommandName = CommandAggregates.CreateOrderCommand ,AggregateName = CommandAggregates.CreateOrderCommand },
        new EventCommandPairModel(){EventName = "onordercreated",CommandName= CommandAggregates.CreateOrderForShopifyCommand ,AggregateName =CommandAggregates.CreateOrderForShopifyCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.SaleChannelOrderPostProcessorCommand ,AggregateName = CommandAggregates.SaleChannelOrderPostProcessorCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.UnAssignFromCarrierCommand ,AggregateName = CommandAggregates.UnAssignFromCarrierCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.AssignToCarrierCommand ,AggregateName = CommandAggregates.AssignToCarrierCommand},
        new EventCommandPairModel(){EventName = "onassigncarrier",CommandName= CommandAggregates.AssignToCarrierCommand ,AggregateName = CommandAggregates.AssignToCarrierCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.CreateCarrierPendingForReturnCommand ,AggregateName = CommandAggregates.CreateCarrierPendingForReturnCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.CreateDeliveryNoteDetailPendingForReturnStatusCommand ,AggregateName =CommandAggregates.CreateDeliveryNoteDetailPendingForReturnStatusCommand },
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.RevertDeliveryNoteDetailByOrderIdCommand ,AggregateName = CommandAggregates.RevertDeliveryNoteDetailByOrderIdCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.UpdateDeliveryNoteInOperationCommand ,AggregateName = CommandAggregates.UpdateDeliveryNoteInOperationCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.UpdateOrderStatusForCompleteOnDebriefCommand ,AggregateName = CommandAggregates.UpdateOrderStatusForCompleteOnDebriefCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.CreateDeliveryTaskAndAddToExistingNoteCommand ,AggregateName = CommandAggregates.CreateDeliveryTaskAndAddToExistingNoteCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.UpdateOrderStatusByDriverCommand ,AggregateName = CommandAggregates.UpdateOrderStatusByDriverCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.BatchUpdateOrderStatusCommand ,AggregateName = CommandAggregates.BatchUpdateOrderStatusCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.CreateMyCarrierReturnReportCommand ,AggregateName = CommandAggregates.CreateMyCarrierReturnReportCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.DeleteMyCarrierReturnReportCommand ,AggregateName =CommandAggregates.DeleteMyCarrierReturnReportCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.CreateCarrierReturnReportCommand ,AggregateName = CommandAggregates.CreateCarrierReturnReportCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.CreateDeliveryTaskCommand ,AggregateName = CommandAggregates.CreateDeliveryTaskCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName= CommandAggregates.UnAssignDeliveryTaskByIdCommand ,AggregateName = CommandAggregates.UnAssignDeliveryTaskByIdCommand},
        new EventCommandPairModel(){EventName = "onordertrackingstatus",CommandName=CommandAggregates.RevertDeliveryTaskByOrderNosCommand ,AggregateName =CommandAggregates.RevertDeliveryTaskByOrderNosCommand},
    };
}

public class EventCommandPairModel
{
  public string? EventName { get; set; }
  public string? CommandName { get; set; }
  public string? AggregateName { get; set; }

}
public static class CommandAggregates
{
  public const string CreateOrderCommand = "CreateOrderCommand";
  public const string CreateOrderForShopifyCommand = "CreateOrderForShopifyCommand";
  public const string SaleChannelOrderPostProcessorCommand = "SaleChannelOrderPostProcessorCommand";
  public const string UnAssignFromCarrierCommand = "UnAssignFromCarrierCommand";
  public const string AssignToCarrierCommand = "AssignToCarrierCommand";
  public const string CreateCarrierPendingForReturnCommand = "CreateCarrierPendingForReturnCommand";
  public const string CreateDeliveryNoteDetailPendingForReturnStatusCommand = "CreateDeliveryNoteDetailPendingForReturnStatusCommand";
  public const string RevertDeliveryNoteDetailByOrderIdCommand = "RevertDeliveryNoteDetailByOrderIdCommand";
  public const string UpdateDeliveryNoteInOperationCommand = "UpdateDeliveryNoteInOperationCommand";
  public const string UpdateOrderStatusForCompleteOnDebriefCommand = "UpdateOrderStatusForCompleteOnDebriefCommand";
  public const string CreateDeliveryTaskAndAddToExistingNoteCommand = "CreateDeliveryTaskAndAddToExistingNoteCommand";
  public const string UpdateOrderStatusByDriverCommand = "UpdateOrderStatusByDriverCommand";
  public const string BatchUpdateOrderStatusCommand = "BatchUpdateOrderStatusCommand";
  public const string CreateMyCarrierReturnReportCommand = "CreateMyCarrierReturnReportCommand";
  public const string DeleteMyCarrierReturnReportCommand = "DeleteMyCarrierReturnReportCommand";
  public const string CreateCarrierReturnReportCommand = "CreateCarrierReturnReportCommand";
  public const string CreateDeliveryTaskCommand = "CreateDeliveryTaskCommand";
  public const string UnAssignDeliveryTaskByIdCommand = "UnAssignDeliveryTaskByIdCommand";
  public const string RevertDeliveryTaskByOrderNosCommand = "RevertDeliveryTaskByOrderNosCommand";

}

#endregion
#region subscription plan skip commands and queries


#endregion
public class NotificationConstants
{
  public const string SavedSuccess = "The record saved successfully";
  public const string SavedError = "Error while saving record";
  public const string UpdateSuccess = "The record updated successfully";
  public const string UpdateError = "Error record saved successfully";
  public const string DeleteSuccess = "The record deleted successfully";
  public const string DeleteError = "Error while deleting record";
  public const string InvalidResponse = "The request has invalid response";
  public const string Success = "The action perform successfully";
  public const string Error = "Error while performing this action";
  public const string ErrorEntityNotFound = "Error entity record not found";
}
public class ApplicationEvents
{
  public const string onordertrackingstatus = "onordertrackingstatus"; 
  public const string onordercreated = "onordercreated"; 
}
public class ImageExtensionContants
{
  public const string JPG = ".jpg";
  public const string JPEG = ".jpeg";
  public const string GIF = ".gif";
  public const string PNG = ".png";
}
