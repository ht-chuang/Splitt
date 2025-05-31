using System.ComponentModel.DataAnnotations;

namespace SplittDB.Attributes
{
    public class DecimalPrecisionAttribute : ValidationAttribute
    {
        private readonly int _precision;

        public DecimalPrecisionAttribute(int precision)
        {
            _precision = precision;
        }

        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Let [Required] handle null validation

            if (value is decimal decimalValue)
            {
                var scale = BitConverter.GetBytes(decimal.GetBits(decimalValue)[3])[2];
                return scale <= _precision;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot have more than {_precision} decimal places";
        }
    }
}