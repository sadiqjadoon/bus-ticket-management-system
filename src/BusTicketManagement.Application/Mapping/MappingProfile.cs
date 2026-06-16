using AutoMapper;
using BusTicketManagement.Domain.Entities;
using BusTicketManagement.Application.DTOs.Auth;
using BusTicketManagement.Application.DTOs.Bus;
using BusTicketManagement.Application.DTOs.Route;
using BusTicketManagement.Application.DTOs.Schedule;
using BusTicketManagement.Application.DTOs.Booking;
using BusTicketManagement.Application.DTOs.Seat;

namespace BusTicketManagement.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth/User Mappings
            CreateMap<User, UserDto>().ReverseMap();

            // Bus Mappings
            CreateMap<Bus, BusDto>().ReverseMap();
            CreateMap<Bus, CreateBusDto>().ReverseMap();
            CreateMap<Bus, UpdateBusDto>().ReverseMap();

            // Route Mappings
            CreateMap<Route, RouteDto>().ReverseMap();
            CreateMap<Route, CreateRouteDto>().ReverseMap();
            CreateMap<Route, UpdateRouteDto>().ReverseMap();

            // Schedule Mappings
            CreateMap<Schedule, ScheduleDto>().ReverseMap();
            CreateMap<Schedule, CreateScheduleDto>().ReverseMap();
            CreateMap<Schedule, UpdateScheduleDto>().ReverseMap();

            // Booking Mappings
            CreateMap<Booking, BookingDto>().ReverseMap();
            CreateMap<Booking, CreateBookingDto>().ReverseMap();

            // Seat Mappings
            CreateMap<Seat, SeatDto>().ReverseMap();

            // Payment Mappings
            CreateMap<Payment, PaymentDto>().ReverseMap();
        }
    }

    public class RouteInfoDto
    {
        public int RouteId { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public decimal Distance { get; set; }
        public int Duration { get; set; }
    }

    public class BusInfoDto
    {
        public int BusId { get; set; }
        public string BusNo { get; set; }
        public string BusType { get; set; }
        public int Capacity { get; set; }
    }
}
