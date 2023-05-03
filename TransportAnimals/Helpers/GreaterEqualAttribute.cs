using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection.Emit;

namespace TransportAnimals.Helpers
{
    /// <summary>
    ///     Used for specifying a min constraint
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class GreaterEqualAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Constructor that takes integer minimum
        /// </summary>
        /// <param name="minimum">The minimum value, exclusive</param>
        public GreaterEqualAttribute(long minimum)
        {
            Minimum = minimum;
            OperandType = typeof(long);
        }

        /// <summary>
        ///     Constructor that takes integer minimum
        /// </summary>
        /// <param name="minimum">The minimum value, exclusive</param>
        public GreaterEqualAttribute(double minimum)
        {
            Minimum = minimum;
            OperandType = typeof(double);
        }

        /// <summary>
        ///     Allows for specifying range for arbitrary types. The minimum strings
        ///     will be converted to the target type.
        /// </summary>
        /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
        /// <param name="minimum">The minimum allowable value.</param>
        [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
        public GreaterEqualAttribute(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type,
            string minimum)
        {
            OperandType = type;
            Minimum = minimum;
        }

        /// <summary>
        ///     Gets the minimum value for the range
        /// </summary>
        public object Minimum { get; private set; }

        /// <summary>
        ///     Gets the type of the <see cref="Minimum" /> value (e.g. Int32, Double, or some custom
        ///     type)
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type OperandType { get; }

        /// <summary>
        /// Determines whether string values for <see cref="Minimum"/>  are parsed in the invariant
        /// culture rather than the current culture in effect at the time of the validation.
        /// </summary>
        public bool ParseLimitsInInvariantCulture { get; set; }

        /// <summary>
        /// Determines whether any conversions necessary from the value being validated to <see cref="OperandType"/> as set
        /// by the <c>type</c> parameter of the <see cref="MinAttribute(Type, string, string)"/> constructor are carried
        /// out in the invariant culture rather than the current culture in effect at the time of the validation.
        /// </summary>
        /// <remarks>This property has no effects with the constructors with <see cref="int"/> or <see cref="double"/>
        /// parameters, for which the invariant culture is always used for any conversions of the validated value.</remarks>
        public bool ConvertValueInInvariantCulture { get; set; }

        private Func<object, object?>? Conversion { get; set; }

        private void Initialize(IComparable minimum, Func<object, object?> conversion)
        {
            Minimum = minimum;
            Conversion = conversion;
        }

        /// <summary>
        ///     Returns true if the value falls between min and max, inclusive.
        /// </summary>
        /// <param name="value">The value to test for validity.</param>
        /// <returns><c>true</c> means the <paramref name="value" /> is valid</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public override bool IsValid(object? value)
        {
            SetupConversion();
            // Automatically pass if value is null or empty. RequiredAttribute should be used to assert a value is not empty.
            if (value == null || (value as string)?.Length == 0)
            {
                return true;
            }

            object? convertedValue;

            try
            {
                convertedValue = Conversion!(value);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

            var min = (IComparable)Minimum;
            return min.CompareTo(convertedValue) <= 0;
        }

        /// <summary>
        ///     Override of <see cref="ValidationAttribute.FormatErrorMessage" />
        /// </summary>
        /// <remarks>This override exists to provide a formatted message describing the minimum and maximum values</remarks>
        /// <param name="name">The user-visible name to include in the formatted message.</param>
        /// <returns>A localized string describing the minimum and maximum values</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, $"The value of {name} must be greater than or equal to {Minimum}", name, Minimum);
        }

        /// <summary>
        ///     Validates the properties of this attribute and sets up the conversion function.
        ///     This method throws exceptions if the attribute is not configured properly.
        ///     If it has once determined it is properly configured, it is a NOP.
        /// </summary>
        private void SetupConversion()
        {
            if (Conversion == null)
            {
                object minimum = Minimum;

                if (minimum == null)
                {
                    throw new InvalidOperationException("Must set minimum value");
                }

                // Careful here -- OperandType could be int or double if they used the long form of the ctor.
                // But the min would still be strings.  Do use the type of the min operands to condition
                // the following code.
                Type operandType = minimum.GetType();

                if (operandType == typeof(long))
                {
                    Initialize((long)minimum, v => Convert.ToInt64(v, CultureInfo.InvariantCulture));
                }
                else if (operandType == typeof(double))
                {
                    Initialize((double)minimum, v => Convert.ToDouble(v, CultureInfo.InvariantCulture));
                }
                else
                {
                    Type type = OperandType;
                    if (type == null)
                    {
                        throw new InvalidOperationException("Must set operand type");
                    }
                    Type comparableType = typeof(IComparable);
                    if (!comparableType.IsAssignableFrom(type))
                    {
                        throw new InvalidOperationException("Must type is assignable IComparable");
                    }

                    TypeConverter converter = GetOperandTypeConverter();
                    IComparable min = (IComparable)(ParseLimitsInInvariantCulture
                        ? converter.ConvertFromInvariantString((string)minimum)!
                        : converter.ConvertFromString((string)minimum))!;

                    Func<object, object?> conversion;
                    if (ConvertValueInInvariantCulture)
                    {
                        conversion = value => value.GetType() == type
                            ? value
                            : converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                    }
                    else
                    {
                        conversion = value => value.GetType() == type ? value : converter.ConvertFrom(value);
                    }

                    Initialize(min, conversion);
                }
            }
        }

        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode",
            Justification = "The ctor that allows this code to be called is marked with RequiresUnreferencedCode.")]
        private TypeConverter GetOperandTypeConverter() =>
            TypeDescriptor.GetConverter(OperandType);
    }
}
