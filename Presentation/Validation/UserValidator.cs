using Domain.Entity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage("Name field is required.");
            RuleFor(user => user.UserName).NotEmpty().WithMessage("Username field is required.");
            RuleFor(user => user.Email).NotEmpty().EmailAddress().WithMessage("Invalid email address.");
            RuleFor(user => user.Phone).NotEmpty().WithMessage("Phone field is required.");
            RuleFor(user => user.Adress).SetValidator(new AdressValidator());


        }

    }
}
