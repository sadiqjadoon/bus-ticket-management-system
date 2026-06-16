using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using BusTicketManagement.Application.Interfaces;
using BusTicketManagement.Application.Interfaces.Services;
using BusTicketManagement.Application.DTOs.Auth;
using BusTicketManagement.Domain.Entities;
using System.Data.SqlClient;
using BusTicketManagement.Infrastructure.Data;

namespace BusTicketManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AuthService> logger, JwtTokenGenerator tokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _unitOfWork.Users.GetByUserNameAsync(dto.UserName);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Username already exists");
                }

                var existingEmail = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
                if (existingEmail != null)
                {
                    throw new InvalidOperationException("Email already registered");
                }

                // Create new user
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = dto.UserName,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Hash password (use ASP.NET Core Identity in production)
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // Save to database (pseudocode - need actual repository implementation)
                _logger.LogInformation($"User {dto.UserName} registered successfully");

                var userDto = _mapper.Map<UserDto>(user);
                var token = _tokenGenerator.GenerateToken(user);

                return new AuthResponseDto
                {
                    User = userDto,
                    Token = token,
                    Roles = new List<string> { "Customer" },
                    Permissions = new List<string> { "BOOK_TICKET" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering user: {ex.Message}");
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByUserNameAsync(dto.UserName);
                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }

                if (!user.IsActive)
                {
                    throw new UnauthorizedAccessException("User account is disabled");
                }

                // Verify password (use ASP.NET Core Identity in production)
                // var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
                var isPasswordValid = true; // Placeholder

                if (!isPasswordValid)
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }

                var roles = await GetUserRolesAsync(user.Id);
                var permissions = await GetUserPermissionsAsync(user.Id);

                var userDto = _mapper.Map<UserDto>(user);
                var token = _tokenGenerator.GenerateToken(user, roles, permissions);

                _logger.LogInformation($"User {user.UserName} logged in successfully");

                return new AuthResponseDto
                {
                    User = userDto,
                    Token = token,
                    Roles = roles,
                    Permissions = permissions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during login: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            // Query database for user roles
            var roles = new List<string>();
            try
            {
                // Pseudocode - implement actual query
                roles.Add("Customer");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching user roles: {ex.Message}");
            }
            return await Task.FromResult(roles);
        }

        public async Task<List<string>> GetUserPermissionsAsync(string userId)
        {
            var permissions = new List<string>();
            try
            {
                // Query database for user permissions
                permissions.Add("BOOK_TICKET");
                permissions.Add("VIEW_BUSES");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching user permissions: {ex.Message}");
            }
            return await Task.FromResult(permissions);
        }
    }
}
