namespace MyFaceApi.Api.Application.Helpers
{
	public class PaginationMetadata
	{
		public string NextPageLink { get; set; }
		public string PreviousPageLink { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public bool HasPrevious { get; set; }
		public bool HasNext { get; set; }
	}
}
