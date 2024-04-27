using FastEndpoints;
using FluentValidation;
using QuickTest.Data.Dtos;
using QuickTest.Data.Wafer;
using QuickTest.Data.Wafer.Enums;

namespace QuickTest.Data.Validators;

public class CreateWaferPadRequestValidator:Validator<CreateWaferPadRequest> {
    public CreateWaferPadRequestValidator() {
        RuleFor(x => x.WaferArea).NotNull().WithMessage("Wafer area must be provided.");
        RuleFor(x => x.WaferSize).NotNull().WithMessage("Wafer size must be provided.");
        RuleFor(x => x.PadLocation).NotNull().WithMessage("Pad location must be provided.");
        RuleFor(x => x.SvgObject).NotNull().WithMessage("Svg object must be provided.");
        RuleFor(request => request).Custom((request, context) => {
            switch (request.WaferArea.Name) {
                case nameof(WaferArea.Middle):
                case nameof(WaferArea.Edge): {
                    if (request.PadNumber<1 && request.PadNumber > 6) {
                        context.AddFailure("Middle and Edge pads can only have a pad number of 1-6.");
                    }
                    break;
                }
            }
        });
    }
}