﻿namespace Pagination.Helpers
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
				HasNext = pagedList.HasNext,
				HasPrevious = pagedList.HasPrevious
			};
		}
	}
}
