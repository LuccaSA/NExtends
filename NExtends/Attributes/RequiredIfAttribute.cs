using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			Object instance = context.ObjectInstance;
			Type type = instance.GetType();
			Object proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
			// TODO spécialiser ce test par type: Enum, IEquatable, Parse, ...
			if (proprtyvalue.ToString() == DesiredValue.ToString())
			{
				ValidationResult result = base.IsValid(value, context);
				return result;
			}
			return ValidationResult.Success;
		}
	}
}
