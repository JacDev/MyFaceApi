namespace MyFaceApi.Api.Application.Helpers
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
				HasNext = pagedList.HasNext,
				HasPrevious = pagedList.HasPrevious
			};
		}
	}
}
