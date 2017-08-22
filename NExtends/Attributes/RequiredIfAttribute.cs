using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NExtends.Attributes
{
	[AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class RequiredIfAttribute : RequiredAttribute
	{
		private String PropertyName { get; set; }
		private Object DesiredValue { get; set; }

		public RequiredIfAttribute()
		{
		}
		public RequiredIfAttribute(String propertyName, Object desiredvalue)
			: this()
		{
			PropertyName = propertyName;
			DesiredValue = desiredvalue;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			Object instance = validationContext.ObjectInstance;
			Type type = instance.GetType();
			Object propertyValue = type.GetTypeInfo().GetProperty(PropertyName).GetValue(instance, null);

			if (propertyValue.ToString() == DesiredValue.ToString())
			{
				return base.IsValid(value, validationContext);
			}

			return ValidationResult.Success;
		}
	}
}
