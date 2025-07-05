using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreWebAPI.Model.Dto
{
    public class UserRegisterDto
    {
        //[Required]
        //[MinLength(3)]
        public string Username { get; set; }

        //[Required]
        //[EmailAddress]
        public string Email { get; set; }

        //[Required]
        //[StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

    }

    public class UserRegisterDtoValidator: AbstractValidator<UserRegisterDto>
    {
        public UserRegisterDtoValidator() {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}
