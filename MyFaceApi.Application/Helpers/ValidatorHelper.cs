﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFaceApi.Api.Application.Helpers
{
	public static class ValidatorHelper
	{
		public static bool ValidateModel<T>(T data)
		{
			var context = new ValidationContext(typeof(T), serviceProvider: null, items: null);
			var results = new List<ValidationResult>();
			var isValid = Validator.TryValidateObject(data, context, results);
			return isValid;
		}
	}
}
