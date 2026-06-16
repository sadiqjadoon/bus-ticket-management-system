using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using BusTicketManagement.Application.Interfaces;
using BusTicketManagement.Application.Interfaces.Services;
using BusTicketManagement.Application.DTOs.Route;
using BusTicketManagement.Domain.Entities;

namespace BusTicketManagement.Application.Services
{
    public class RouteService : IRouteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RouteService> _logger;

        public RouteService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RouteService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RouteDto> GetRouteByIdAsync(int routeId)
        {
            try
            {
                var route = await _unitOfWork.Routes.GetByIdAsync(routeId);
                if (route == null)
                {
                    throw new KeyNotFoundException($"Route with ID {routeId} not found");
                }
                return _mapper.Map<RouteDto>(route);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching route: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync(int pageNumber, int pageSize)
        {
            try
            {
                var routes = await _unitOfWork.Routes.GetAllAsync();
                return _mapper.Map<IEnumerable<RouteDto>>(routes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching routes: {ex.Message}");
                throw;
            }
        }

        public async Task<int> CreateRouteAsync(CreateRouteDto dto)
        {
            try
            {
                var routeCode = $"{dto.SourceCity.Substring(0, 3).ToUpper()}-{dto.DestinationCity.Substring(0, 3).ToUpper()}";
                
                var route = _mapper.Map<Route>(dto);
                route.RouteCode = routeCode;
                route.IsActive = true;
                route.CreatedAt = DateTime.UtcNow;
                route.UpdatedAt = DateTime.UtcNow;

                var routeId = await _unitOfWork.Routes.AddAsync(route);
                _logger.LogInformation($"Route {routeCode} created with ID {routeId}");

                return routeId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating route: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateRouteAsync(UpdateRouteDto dto)
        {
            try
            {
                var route = await _unitOfWork.Routes.GetByIdAsync(dto.RouteId);
                if (route == null)
                {
                    throw new KeyNotFoundException($"Route with ID {dto.RouteId} not found");
                }

                _mapper.Map(dto, route);
                route.UpdatedAt = DateTime.UtcNow;

                var result = await _unitOfWork.Routes.UpdateAsync(route);
                if (result)
                {
                    _logger.LogInformation($"Route {dto.RouteId} updated successfully");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating route: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteRouteAsync(int routeId)
        {
            try
            {
                var result = await _unitOfWork.Routes.DeleteAsync(routeId);
                if (result)
                {
                    _logger.LogInformation($"Route {routeId} deleted successfully");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting route: {ex.Message}");
                throw;
            }
        }
    }
}
