using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using BusTicketManagement.Application.Interfaces;
using BusTicketManagement.Application.Interfaces.Services;
using BusTicketManagement.Application.DTOs.Bus;
using BusTicketManagement.Domain.Entities;

namespace BusTicketManagement.Application.Services
{
    public class BusService : IBusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BusService> _logger;

        public BusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BusService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BusDto> GetBusByIdAsync(int busId)
        {
            try
            {
                var bus = await _unitOfWork.Buses.GetByIdAsync(busId);
                if (bus == null)
                {
                    throw new KeyNotFoundException($"Bus with ID {busId} not found");
                }
                return _mapper.Map<BusDto>(bus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching bus: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<BusDto>> GetAllBusesAsync(int pageNumber, int pageSize)
        {
            try
            {
                var buses = await _unitOfWork.Buses.GetActiveBusesAsync();
                return _mapper.Map<IEnumerable<BusDto>>(buses);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching buses: {ex.Message}");
                throw;
            }
        }

        public async Task<int> CreateBusAsync(CreateBusDto dto)
        {
            try
            {
                // Validate bus number is unique
                var existingBus = await _unitOfWork.Buses.GetByBusNoAsync(dto.BusNo);
                if (existingBus != null)
                {
                    throw new InvalidOperationException($"Bus number {dto.BusNo} already exists");
                }

                var bus = _mapper.Map<Bus>(dto);
                bus.IsActive = true;
                bus.CreatedAt = DateTime.UtcNow;
                bus.UpdatedAt = DateTime.UtcNow;

                var busId = await _unitOfWork.Buses.AddAsync(bus);
                _logger.LogInformation($"Bus {dto.BusNo} created with ID {busId}");

                return busId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating bus: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateBusAsync(UpdateBusDto dto)
        {
            try
            {
                var bus = await _unitOfWork.Buses.GetByIdAsync(dto.BusId);
                if (bus == null)
                {
                    throw new KeyNotFoundException($"Bus with ID {dto.BusId} not found");
                }

                _mapper.Map(dto, bus);
                bus.UpdatedAt = DateTime.UtcNow;

                var result = await _unitOfWork.Buses.UpdateAsync(bus);
                if (result)
                {
                    _logger.LogInformation($"Bus {dto.BusId} updated successfully");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating bus: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteBusAsync(int busId)
        {
            try
            {
                var bus = await _unitOfWork.Buses.GetByIdAsync(busId);
                if (bus == null)
                {
                    throw new KeyNotFoundException($"Bus with ID {busId} not found");
                }

                var result = await _unitOfWork.Buses.DeleteAsync(busId);
                if (result)
                {
                    _logger.LogInformation($"Bus {busId} deleted successfully");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting bus: {ex.Message}");
                throw;
            }
        }
    }
}
