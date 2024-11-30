using System;
using System.ComponentModel;
using System.Reflection;

namespace BudgetManager.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

            if (attribute == null)
            {
                return enumValue.ToString();
            }
            else
            {
                return attribute.Description;
            }
        }
    }
}
