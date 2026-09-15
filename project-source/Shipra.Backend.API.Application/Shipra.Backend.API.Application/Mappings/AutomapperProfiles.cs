using AutoMapper;
using Shipra.Backend.API.Application.DTOs.ActiveCarrierPickupLoctionUseCase;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.CountryUseCase.Request;
using Shipra.Backend.API.Application.DTOs.CountryUseCase.Response;
using Shipra.Backend.API.Application.DTOs.DeliveryNoteUseCase.Response;
using Shipra.Backend.API.Application.DTOs.DriverExpenseUseCase;
using Shipra.Backend.API.Application.DTOs.DriverUseCase;
using Shipra.Backend.API.Application.DTOs.EmployeeUseCase;
using Shipra.Backend.API.Application.DTOs.MiscUseCase;
using Shipra.Backend.API.Application.DTOs.NotificationUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.Response;
using Shipra.Backend.API.Application.DTOs.ProductCategoryUseCase;
using Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Request;
using Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Response;
using Shipra.Backend.API.Application.DTOs.ProductStationUseCase.Response;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Response;
using Shipra.Backend.API.Application.DTOs.SettingOperationDashboardUseCase.Response;
using Shipra.Backend.API.Application.DTOs.StoresUseCase.Responses;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.DocumentAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.NotificationAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderBoxAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Mappings;

public class AutomapperProfiles : Profile
{
  public AutomapperProfiles()
  {

    //#region example mappings
    //    #region requests
    //        CreateMap<ExampleCommand, Customer>();
    //    #endregion
    //    #region responses -- DTO
    //        CreateMap<Customer, ExampleResponseDTO>();
    //    #endregion
    //#endregion

    #region product mappings
    #region requests 
    #endregion
    #region responses -- DTO
    CreateMap<Product, ProductResponseModel>()
      //.ForMember(x => x.ClientId, cd => cd.MapFrom(map => map.ClientId!.Value))
      .ForMember(x => x.ProductId, cd => cd.MapFrom(map => map.ProductId!.Value));

    CreateMap<ProductOption, ProductOptionReuqestModel>().ForMember(x => x.ProductOptionsId, cd => cd.MapFrom(map => map.ProductOptionsId!.Value));

    CreateMap<ProductStock, ProductStockReuqestModel>().ForMember(x => x.ProductId, cd => cd.MapFrom(map => map.ProductId!.Value!));
    CreateMap<ProductMedia, ProductMediaResponseModal>();
    CreateMap<ProductMediaResponseModal, ProductMedia>();

    CreateMap<ImageGallery, ImageGalleryResponseDto>();


    #endregion
    #endregion

    #region product station mapping
    #region requests
    CreateMap<ProductStationResposeModel, ProductStation>();
    #endregion
    #region responses -- DTO
    CreateMap<ProductStation, ProductStationResposeModel>();
    CreateMap<ProductCategoryLookup, ProductCategoryResponseModel>();
    CreateMap<ProductCategoryResponseModel, ProductCategoryLookup>();
    #endregion
    #endregion
    #region misc setting
    CreateMap<DocumentSize, DocumentSizeResponseModel>();
    CreateMap<DocumentTemplate, DocumentTemplateResponseModel>();
    #endregion

    #region product station transfer and transferproduct requests
    CreateMap<ProductStationTransferResponseModel, ProductStationTransfer>();

    #endregion

    #region product station transfer and transferproduct responses
    CreateMap<ProductStationTransfer, ProductStationTransferResponseModel>().ForMember(x => x.ProductStaionTransferId, cd => cd.MapFrom(map => map.ProductStaionTransferId!.Value));

    CreateMap<TransferProduct, TransferProductRequestModel>().ForMember(x => x.ProductStaionTransferId, cd => cd.MapFrom(map => map.ProductStaionTransferId!.Value));
    #endregion

    #region stores requests
    CreateMap<StoreResponseModel, Store>();
    #endregion

    #region stores responses
    CreateMap<Store, StoreResponseModel>();
    CreateMap<StoreAddress, AddressResponseDTO>()
      .ForMember(x => x.AddressId, cd => cd.MapFrom(map => map.StoreAddressId))
      .ForMember(x => x.Country, cd => cd.MapFrom(map => map.CountryId))
      .ForMember(x => x.City, cd => cd.MapFrom(map => map.CityId))
      .ForMember(x => x.Area, cd => cd.MapFrom(map => map.AreaId))
      .ForMember(x => x.Province, cd => cd.MapFrom(map => map.ProvinceId))
      .ForMember(x => x.State, cd => cd.MapFrom(map => map.StateId))
      .ForMember(x => x.PinCode, cd => cd.MapFrom(map => map.PinCodeId));
    #endregion

    #region ActiveCarrierPickupLocation 
       CreateMap<ActiveCarrierPickupLocation, PickupLocationResponseModel>()
        .ForMember(dest => dest.address, opt => opt.MapFrom(src => src));
       CreateMap<ActiveCarrierPickupLocation, CarrierPickupAddressModel>()
        .ForMember(dest => dest.addressId, opt => opt.MapFrom(src => src.ActiveCarrierPickupLocationId))
        .ForMember(dest => dest.country, opt => opt.MapFrom(src => src.CountryId))
        .ForMember(dest => dest.city, opt => opt.MapFrom(src => src.CityId))
        .ForMember(dest => dest.area, opt => opt.MapFrom(src => src.AreaId))
        .ForMember(dest => dest.streetAddress, opt => opt.MapFrom(src => src.StreetAddress))
        .ForMember(dest => dest.streetAddress2, opt => opt.MapFrom(src => src.StreetAddress2))
        .ForMember(dest => dest.houseNo, opt => opt.MapFrom(src => src.HouseNo))
        .ForMember(dest => dest.buildingName, opt => opt.MapFrom(src => src.BuildingName))
        .ForMember(dest => dest.landmark, opt => opt.MapFrom(src => src.Landmark))
        .ForMember(dest => dest.province, opt => opt.MapFrom(src => src.ProvinceId))
        .ForMember(dest => dest.pinCode, opt => opt.MapFrom(src => src.PinCodeId))
        .ForMember(dest => dest.state, opt => opt.MapFrom(src => src.StateId))
        .ForMember(dest => dest.fullAddress, opt => opt.MapFrom(src => src.FullAddress))
        .ForMember(dest => dest.zip, opt => opt.MapFrom(src => src.Zip))
        .ForMember(dest => dest.latitude, opt => opt.MapFrom(src => src.Latitude))
        .ForMember(dest => dest.longitude, opt => opt.MapFrom(src => src.Longitude));
    #endregion
    #region country requests
        CreateMap<CountryResponseModel, Country>();
    #endregion

    #region country responses
    CreateMap<Country, CountryResponseModel>();
    #endregion


    #region city requests
    CreateMap<CityResponseModel, City>();
    #endregion

    #region city responses
    CreateMap<City, CityResponseModel>();
    #endregion

    #region zone requests
    CreateMap<Zone, ZoneRequestModel>();
    #endregion

    #region zone requests
    CreateMap<Carrier, CarrierResponseDTO>();
    CreateMap<ProductCategory, ProductCategoryResponseModel>();
    CreateMap<ProductCategoryResponseModel, ProductCategory>();
    #endregion

    #region client mappings 
    CreateMap<Client, ClientResponseModel>().ForMember(x => x.ClientId, cd => cd.MapFrom(map => map.ClientId!.Value));
    CreateMap<Client, ClientKeysResponseModel>();
    CreateMap<ClientAddress, AddressResponseDTO>().ForMember(x => x.AddressId, cd => cd.MapFrom(map => map.ClientAddressId))
      .ForMember(x => x.Country, cd => cd.MapFrom(map => map.CountryId))
      .ForMember(x => x.City, cd => cd.MapFrom(map => map.CityId))
      .ForMember(x => x.Area, cd => cd.MapFrom(map => map.AreaId))
      .ForMember(x => x.Province, cd => cd.MapFrom(map => map.ProvinceId))
      .ForMember(x => x.State, cd => cd.MapFrom(map => map.StateId))
      .ForMember(x => x.Zip, cd => cd.MapFrom(map => map.Zip))
      .ForMember(x => x.PinCode, cd => cd.MapFrom(map => map.PinCodeId));
    #endregion

    #region order mappings
    CreateMap<OrderAddress, OrderAddressValidateAgainstCarrierModel>();
    CreateMap<OrderAddressValidateAgainstCarrierModel, OrderAddress>();

    CreateMap<Order, OrderResponseModel>()
      .ForMember(x => x.OrderId, cd => cd.MapFrom(map => map.OrderId!.Value))
      .ForMember(x => x.ActualAmount, cd => cd.MapFrom(map => 0));

    #region for special meaning only used for contract and active carrier for assign order
    CreateMap<ActiveCarrier, ActiveCarrierContractResponseModel>();
    CreateMap<ShipraContractCarrier, ActiveCarrierContractResponseModel>();

    #endregion
    CreateMap<ClientOrderBox, ClientOrderBoxResponseModel>()
        .ForMember(x => x.IsDefault,
                   cd => cd.MapFrom(map => map.IsDefault.HasValue ? map.IsDefault.Value : false));
     
    CreateMap<OrderItem, OrderItemModel>()
    .ForMember(x => x.OrderItemId, cd => cd.MapFrom(map => map.OrderItemId!.Value))
    .ForMember(x => x.Price, cd => cd.MapFrom(map => map.Price ?? 0));

    CreateMap<OrderAddress, OrderAddressResponseModel>()
            .ForMember(dest => dest.OrderAddressId, opt => opt.MapFrom(src => src.OrderAddressId))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
            .ForMember(dest => dest.CustomerFullAddress, opt => opt.MapFrom(src => src.CustomerFullAddress))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Mobile1, opt => opt.MapFrom(src => src.Mobile1))
            .ForMember(dest => dest.Mobile2, opt => opt.MapFrom(src => src.Mobile2))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.CountryId))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CityId))
            .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.AreaId))
            .ForMember(dest => dest.StreetAddress, opt => opt.MapFrom(src => src.StreetAddress))
            .ForMember(dest => dest.StreetAddress2, opt => opt.MapFrom(src => src.StreetAddress2))
            .ForMember(dest => dest.HouseNo, opt => opt.MapFrom(src => src.HouseNo))
            .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.BuildingName))
            .ForMember(dest => dest.Landmark, opt => opt.MapFrom(src => src.Landmark))
            .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.ProvinceId))
            .ForMember(dest => dest.PinCode, opt => opt.MapFrom(src => src.PinCodeId))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.StateId))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude));


    CreateMap<OrderNote, OrderNoteModel>().ForMember(x => x.OrderNoteId, cd => cd.MapFrom(map => map.OrderNoteId!.Value)); ;
    CreateMap<OrderBox, OrderBoxesResponseModel>().ForMember(x => x.OrderBoxId, cd => cd.MapFrom(map => map.OrderBoxId!.Value)).ForMember(x => x.OrderId, cd => cd.MapFrom(map => map.OrderId!.Value));
    #endregion
    #region employee mappingsUserTypeId
    CreateMap<Employee, EmployeResponseModel>()
      .ForMember(x => x.EmployeeId, cd => cd.MapFrom(map => map.EmployeeId!.Value))
      .ForMember(dest => dest.EmployeeTypeId, opt => opt.MapFrom(src => src.EmployeeTypeId ?? (int)EnumEmployeeType.Employee));
    CreateMap<EmployeeAddress, AddressResponseDTO>()
      .ForMember(x => x.AddressId, cd => cd.MapFrom(map => map.EmployeeAddressId))
      .ForMember(x => x.Country, cd => cd.MapFrom(map => map.CountryId))
      .ForMember(x => x.City, cd => cd.MapFrom(map => map.CityId))
      .ForMember(x => x.Area, cd => cd.MapFrom(map => map.AreaId))
      .ForMember(x => x.Province, cd => cd.MapFrom(map => map.ProvinceId))
      .ForMember(x => x.State, cd => cd.MapFrom(map => map.StateId))
      .ForMember(x => x.Zip, cd => cd.MapFrom(map => map.Zip))
      .ForMember(x => x.PinCode, cd => cd.MapFrom(map => map.PinCodeId));
    #endregion

    #region deliverytask mapping
    CreateMap<DeliveryTask, DeliveryTaskResponseModel>().ForMember(x => x.DeliveryTaskId, cd => cd.MapFrom(map => map.DeliveryTaskId!.Value));
    CreateMap<DeliveryNote, DeliveryNoteResponseModel>()
      .ForMember(x => x.DeliveryNoteId, cd => cd.MapFrom(map => map.DeliveryNoteId!.Value))
      .ForMember(x => x.DriverId, cd => cd.MapFrom(map => map.DriverId!.Value));
    CreateMap<DeliveryNoteDetail, DeliveryNoteDetailResponseModel>().ForMember(x => x.DeliveryNoteDetailId, cd => cd.MapFrom(map => map.DeliveryNoteDetailId!.value));
    #endregion

    #region driver expense mapping 
    //CreateMap<Expense, ExpenseResponseModel>()
    //  .ForMember(x => x.ExpenseId, cd => cd.MapFrom(map => map.ExpenseId!.Value))
    //  .ForMember(x => x.ClientId, cd => cd.MapFrom(map => map.ClientId!.Value))
    //  .ForMember(x => x.DriverId, cd => cd.MapFrom(map => map.DriverId!.Value))
    //  .ForMember(x => x.DriverReceivableId, cd => cd.MapFrom(map => map.DriverReceivableId!.Value));
    #endregion

    #region driver expense mapping 
    CreateMap<DriverReceivable, DriverReceivableResponseModel>()
      .ForMember(x => x.DriverId, cd => cd.MapFrom(map => map.DriverId!.Value))
      .ForMember(x => x.DriverReceivableId, cd => cd.MapFrom(map => map.DriverReceivableId!.Value));
    CreateMap<DefaultShipmentDashboard, SettingOperationDashboardResponseModel>();
    CreateMap<Expense, DriverExpenseResponseModel>()
      .ForMember(x => x.DriverId, cd => cd.MapFrom(map => map.DriverId!.Value))
      .ForMember(x => x.DriverReceivableId, cd => cd.MapFrom(map => map.DriverReceivableId!.Value))
      .ForMember(x => x.ExpenseId, cd => cd.MapFrom(map => map.ExpenseId!.Value));
    ;
    #endregion

    #region driver mapping
    CreateMap<Driver, DriverResponseModel>()
    .ForMember(x => x.DriverId, cd => cd.MapFrom(map => map.DriverId!.Value))
    .ForMember(x => x.ClientId, cd => cd.MapFrom(map => map.ClientId!.Value));
    #endregion  
    #region notification
    CreateMap<NotificationConfig, NotificationConfigResponseModel>().ForMember(x => x.NotificationConfigId, cd => cd.MapFrom(map => map.NotificationConfigId!.Value));
    #endregion
  }
}
