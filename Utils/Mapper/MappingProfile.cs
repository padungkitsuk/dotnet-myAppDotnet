using AutoMapper;
using MyBackend.Models.Customer;
using MyBackend.Models.Inspection;
using MyBackend.Models.Vehicle;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // คุณยังสามารถใส่คู่ที่ "พิเศษ" (ชื่อไม่ตรงกัน) ไว้ที่นี่ได้
        CreateMap<InspectionRequest, InspectionTransaction>();
        CreateMap<InspectionRequest, CustomerInfo>();
        CreateMap<InspectionRequest, VehicleInfo>();
    }
}