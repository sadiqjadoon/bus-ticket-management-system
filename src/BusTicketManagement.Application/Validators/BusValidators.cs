using FluentValidation;
using BusTicketManagement.Application.DTOs.Bus;

namespace BusTicketManagement.Application.Validators
{
    public class CreateBusDtoValidator : AbstractValidator<CreateBusDto>
    {
        public CreateBusDtoValidator()
        {
            RuleFor(x => x.BusNo)
                .NotEmpty().WithMessage("Bus number is required")
                .MaximumLength(50).WithMessage("Bus number cannot exceed 50 characters");

            RuleFor(x => x.BusType)
                .NotEmpty().WithMessage("Bus type is required")
                .Must(x => x == "AC" || x == "NonAC").WithMessage("Bus type must be AC or NonAC");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Capacity cannot exceed 100 seats");

            RuleFor(x => x.RegistrationNo)
                .NotEmpty().WithMessage("Registration number is required")
                .MaximumLength(50).WithMessage("Registration number cannot exceed 50 characters");

            RuleFor(x => x.ManufacturerName)
                .MaximumLength(100).WithMessage("Manufacturer name cannot exceed 100 characters");

            RuleFor(x => x.ModelName)
                .MaximumLength(100).WithMessage("Model name cannot exceed 100 characters");
        }
    }

    public class UpdateBusDtoValidator : AbstractValidator<UpdateBusDto>
    {
        public UpdateBusDtoValidator()
        {
            Include(new CreateBusDtoValidator());

            RuleFor(x => x.BusId)
                .GreaterThan(0).WithMessage("Bus ID is required");
        }
    }
}
