using MyFaceApi.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Helpers
{
	public static class PaginationHelper
	{
		public static PaginationMetadata CreatePaginationMetadata<T>(PagedList<T> pagedList)
		{
			return new PaginationMetadata
			{
				TotalCount = pagedList.TotalCount,
				PageSize = pagedList.PageSize,
				CurrentPage = pagedList.CurrentPage,
				TotalPages = pagedList.TotalPages,
				PreviousPageLink = pagedList.PreviousPageLink,
				NextPageLink = pagedList.NextPageLink,
			};
		}
	}
}
