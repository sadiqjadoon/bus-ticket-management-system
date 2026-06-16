using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using BusTicketManagement.Application.Interfaces;
using BusTicketManagement.Application.Interfaces.Services;
using BusTicketManagement.Application.DTOs.Seat;

namespace BusTicketManagement.Application.Services
{
    public class SeatService : ISeatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SeatService> _logger;

        public SeatService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SeatService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ScheduleSeatsDto> GetScheduleSeatsAsync(int scheduleId)
        {
            try
            {
                var schedule = await _unitOfWork.Schedules.GetByIdAsync(scheduleId);
                if (schedule == null)
                {
                    throw new KeyNotFoundException($"Schedule with ID {scheduleId} not found");
                }

                var seats = await _unitOfWork.Seats.GetScheduleSeatsAsync(scheduleId);
                var availableCount = await _unitOfWork.Seats.GetAvailableSeatsCountAsync(scheduleId);

                return new ScheduleSeatsDto
                {
                    ScheduleId = scheduleId,
                    TotalSeats = schedule.TotalSeats,
                    AvailableSeats = availableCount,
                    Seats = _mapper.Map<List<SeatDto>>(seats)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching schedule seats: {ex.Message}");
                throw;
            }
        }

        public async Task<SeatDto> GetSeatByIdAsync(int seatId)
        {
            try
            {
                var seat = await _unitOfWork.Seats.GetByIdAsync(seatId);
                if (seat == null)
                {
                    throw new KeyNotFoundException($"Seat with ID {seatId} not found");
                }
                return _mapper.Map<SeatDto>(seat);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching seat: {ex.Message}");
                throw;
            }
        }
    }
}
