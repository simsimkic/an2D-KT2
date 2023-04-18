using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace SimsProject.WPF.View
{
    public class EnumDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string displayValue = string.Empty;

            try
            {
                Type type = value.GetType();

                if (!type.IsEnum)
                    throw new ArgumentException("Type provided must be an Enum.");

                string enumValue = value.ToString();

                MemberInfo memberInfo = type.GetMember(enumValue).FirstOrDefault();

                if (memberInfo != null)
                {
                    DisplayAttribute displayAttr = memberInfo.GetCustomAttribute<DisplayAttribute>();

                    if (displayAttr != null)
                        displayValue = displayAttr.Name;
                }

                if (string.IsNullOrEmpty(displayValue))
                {
                    // Split enum value by capital letters
                    displayValue = string.Concat(enumValue.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x.ToString() : x.ToString()));
                }

                return displayValue;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
