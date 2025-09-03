using AutoMapper;
using MedicineAppApi.Models;
using MedicineAppApi.DTOs;

namespace MedicineAppApi.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserInfoDto>();

            // Category mappings
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Supplier mappings
            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierDto, Supplier>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateSupplierDto, Supplier>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Customer mappings
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateCustomerDto, Customer>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Medicine mappings
            CreateMap<Medicine, MedicineDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<CreateMedicineDto, Medicine>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateMedicineDto, Medicine>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // SupplierMedicine mappings
            CreateMap<SupplierMedicine, SupplierMedicineDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine.Name));
            CreateMap<CreateSupplierMedicineDto, SupplierMedicine>();
            CreateMap<UpdateSupplierMedicineDto, SupplierMedicine>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Invoice mappings
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}"));
            CreateMap<CreateInvoiceDto, Invoice>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateInvoiceDto, Invoice>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // InvoiceItem mappings
            CreateMap<InvoiceItem, InvoiceItemDto>()
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine.Name))
                .ForMember(dest => dest.MedicineBarcode, opt => opt.MapFrom(src => src.Medicine.Barcode ?? ""));
            CreateMap<CreateInvoiceItemDto, InvoiceItem>();
            CreateMap<UpdateInvoiceItemDto, InvoiceItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Payment mappings
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.InvoiceNo, opt => opt.MapFrom(src => src.Invoice.InvoiceNo))
                .ForMember(dest => dest.ReceivedByUserName, opt => opt.MapFrom(src => $"{src.ReceivedByUser.FirstName} {src.ReceivedByUser.LastName}"));
            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdatePaymentDto, Payment>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // StockMovement mappings
            CreateMap<StockMovement, StockMovementDto>()
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine.Name))
                .ForMember(dest => dest.MedicineBarcode, opt => opt.MapFrom(src => src.Medicine.Barcode ?? ""))
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}"));
            CreateMap<CreateStockMovementDto, StockMovement>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateStockMovementDto, StockMovement>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Purchase mappings
            CreateMap<Purchase, PurchaseDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}"));
            CreateMap<CreatePurchaseDto, Purchase>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdatePurchaseDto, Purchase>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // PurchaseItem mappings
            CreateMap<PurchaseItem, PurchaseItemDto>()
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine.Name))
                .ForMember(dest => dest.MedicineBarcode, opt => opt.MapFrom(src => src.Medicine.Barcode ?? ""));
            CreateMap<CreatePurchaseItemDto, PurchaseItem>();
            CreateMap<UpdatePurchaseItemDto, PurchaseItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
