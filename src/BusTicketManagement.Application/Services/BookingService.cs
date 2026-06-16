using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using BusTicketManagement.Application.Interfaces;
using BusTicketManagement.Application.Interfaces.Services;
using BusTicketManagement.Application.DTOs.Booking;
using BusTicketManagement.Domain.Entities;

namespace BusTicketManagement.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BookingService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BookingDto> GetBookingByIdAsync(int bookingId)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    throw new KeyNotFoundException($"Booking with ID {bookingId} not found");
                }
                return _mapper.Map<BookingDto>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching booking: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId, int pageNumber, int pageSize)
        {
            try
            {
                var bookings = await _unitOfWork.Bookings.GetUserBookingsAsync(userId, pageNumber, pageSize);
                return _mapper.Map<IEnumerable<BookingDto>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching user bookings: {ex.Message}");
                throw;
            }
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, string userId)
        {
            try
            {
                // Verify seat availability
                var seat = await _unitOfWork.Seats.GetByIdAsync(dto.SeatId);
                if (seat == null || seat.SeatStatus != "Available")
                {
                    throw new InvalidOperationException("Seat is not available");
                }

                // Generate booking code
                var bookingCode = $"BK{DateTime.UtcNow:yyyyMMddHHmmss}";

                var booking = _mapper.Map<Booking>(dto);
                booking.BookingCode = bookingCode;
                booking.UserId = userId;
                booking.BookingStatus = "Pending";
                booking.BookingDate = DateTime.UtcNow;
                booking.CreatedAt = DateTime.UtcNow;
                booking.UpdatedAt = DateTime.UtcNow;

                var bookingId = await _unitOfWork.Bookings.AddAsync(booking);

                // Update seat status
                seat.SeatStatus = "Booked";
                seat.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Seats.UpdateAsync(seat);

                // Create payment record
                var schedule = await _unitOfWork.Schedules.GetByIdAsync(dto.ScheduleId);
                var payment = new Payment
                {
                    BookingId = bookingId,
                    Amount = schedule.Fare,
                    PaymentStatus = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Payments.AddAsync(payment);

                _logger.LogInformation($"Booking {bookingCode} created for user {userId}");

                return await GetBookingByIdAsync(bookingId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating booking: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CancelBookingAsync(CancelBookingDto dto)
        {
            try
            {
                var booking = await _unitOfWork.Bookings.GetByIdAsync(dto.BookingId);
                if (booking == null)
                {
                    throw new KeyNotFoundException($"Booking with ID {dto.BookingId} not found");
                }

                if (booking.BookingStatus == "Cancelled")
                {
                    throw new InvalidOperationException("Booking is already cancelled");
                }

                // Update booking status
                booking.BookingStatus = "Cancelled";
                booking.CancellationDate = DateTime.UtcNow;
                booking.CancellationReason = dto.CancellationReason;
                booking.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Bookings.UpdateAsync(booking);

                // Release seat
                var seat = await _unitOfWork.Seats.GetByIdAsync(booking.SeatId);
                if (seat != null)
                {
                    seat.SeatStatus = "Available";
                    seat.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.Seats.UpdateAsync(seat);
                }

                // Update payment status
                var payment = await _unitOfWork.Payments.GetByBookingIdAsync(booking.BookingId);
                if (payment != null)
                {
                    payment.PaymentStatus = "Refunded";
                    payment.RefundDate = DateTime.UtcNow;
                    payment.RefundReason = dto.CancellationReason;
                    payment.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.Payments.UpdateAsync(payment);
                }

                _logger.LogInformation($"Booking {booking.BookingCode} cancelled");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error cancelling booking: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByScheduleAsync(int scheduleId)
        {
            try
            {
                var bookings = await _unitOfWork.Bookings.GetAllAsync();
                // Filter by scheduleId in memory (or implement in repository)
                return _mapper.Map<IEnumerable<BookingDto>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching schedule bookings: {ex.Message}");
                throw;
            }
        }
    }
}
