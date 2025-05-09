using AutoMapper;
using HospitalManagement.API.Models;
using HospitalManagement.API.Models.DTOs;

namespace HospitalManagement.API.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<Doctor, DoctorDto>().ReverseMap();
            CreateMap<Patient, PatientDto>().ReverseMap();
            CreateMap<Appointment, AppointmentDto>().ReverseMap();
            CreateMap<Prescription, PrescriptionDto>().ReverseMap();
            CreateMap<Billing, BillingDto>().ReverseMap();
            CreateMap<Staff, StaffDto>().ReverseMap();
            CreateMap<AddDoctorDto, Doctor>().ReverseMap();
            CreateMap<AddPatientDto, Patient>().ReverseMap();
            CreateMap<AddAppointmentDto, Appointment>().ReverseMap();
            CreateMap<AddPrescriptionDto, Prescription>().ReverseMap();
            CreateMap<AddBillingDto, Billing>().ReverseMap();
        }
    }
}
