using Domain.Entity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class AdressValidator : AbstractValidator<Adress>
    {
        public AdressValidator() 
        {
            RuleFor(adress => adress.Street).NotEmpty().WithMessage("Street field is required.");
            RuleFor(adress => adress.Suite).NotEmpty().WithMessage("Suite field is required.");
            RuleFor(adress => adress.City).NotEmpty().WithMessage("City field is required.");
        }
    }
}
