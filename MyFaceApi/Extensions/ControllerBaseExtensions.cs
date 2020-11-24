﻿using MyFaceApi.Api.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyFaceApi.Api.Extensions
{
	public static class ControllerBaseExtensions
	{
		public static string CreateMessagesResourceUriWithPaginationParams(this ControllerBase controllerBase,
			PaginationParams paginationParams,
			ResourceUriType type,
			string nameOfMethodToRedirect)
		{
			var pageNumber = type switch
			{
				ResourceUriType.PreviousPage => paginationParams.PageNumber - 1,
				ResourceUriType.NextPage => paginationParams.PageNumber + 1,
				_ => paginationParams.PageNumber,
			};

			return controllerBase.Url.Link(nameOfMethodToRedirect, new
			{
				pageNumber,
				pageSize = paginationParams.PageSize
			});
		}
	}
}