using System.ComponentModel.DataAnnotations;

namespace NewsBackend.Application.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class ValidEnumValueAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public ValidEnumValueAttribute(Type enumType)
    {
        _enumType = enumType;
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        return Enum.IsDefined(_enumType, value);
    }
}