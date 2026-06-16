using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using BusTicketManagement.Application.Interfaces;
using BusTicketManagement.Application.Interfaces.Services;
using BusTicketManagement.Application.DTOs.Schedule;
using BusTicketManagement.Domain.Entities;

namespace BusTicketManagement.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScheduleService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ScheduleDto> GetScheduleByIdAsync(int scheduleId)
        {
            try
            {
                var schedule = await _unitOfWork.Schedules.GetByIdAsync(scheduleId);
                if (schedule == null)
                {
                    throw new KeyNotFoundException($"Schedule with ID {scheduleId} not found");
                }
                return _mapper.Map<ScheduleDto>(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching schedule: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<ScheduleDto>> SearchSchedulesAsync(SearchScheduleDto dto)
        {
            try
            {
                var schedules = await _unitOfWork.Schedules.SearchSchedulesAsync(
                    dto.SourceCity, 
                    dto.DestinationCity, 
                    dto.TravelDate);
                
                return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching schedules: {ex.Message}");
                throw;
            }
        }

        public async Task<int> CreateScheduleAsync(CreateScheduleDto dto)
        {
            try
            {
                var schedule = _mapper.Map<Schedule>(dto);
                schedule.AvailableSeats = schedule.TotalSeats; // Will be set from bus capacity
                schedule.ScheduleStatus = "Scheduled";
                schedule.CreatedAt = DateTime.UtcNow;
                schedule.UpdatedAt = DateTime.UtcNow;

                var scheduleId = await _unitOfWork.Schedules.AddAsync(schedule);
                _logger.LogInformation($"Schedule created with ID {scheduleId}");

                return scheduleId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating schedule: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateScheduleAsync(UpdateScheduleDto dto)
        {
            try
            {
                var schedule = await _unitOfWork.Schedules.GetByIdAsync(dto.ScheduleId);
                if (schedule == null)
                {
                    throw new KeyNotFoundException($"Schedule with ID {dto.ScheduleId} not found");
                }

                _mapper.Map(dto, schedule);
                schedule.UpdatedAt = DateTime.UtcNow;

                var result = await _unitOfWork.Schedules.UpdateAsync(schedule);
                if (result)
                {
                    _logger.LogInformation($"Schedule {dto.ScheduleId} updated successfully");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating schedule: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            try
            {
                var result = await _unitOfWork.Schedules.DeleteAsync(scheduleId);
                if (result)
                {
                    _logger.LogInformation($"Schedule {scheduleId} deleted successfully");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting schedule: {ex.Message}");
                throw;
            }
        }
    }
}
