using MyFaceApi.Api.Repository.Helpers;

namespace MyFaceApi.Api.DboModels
{
	public class CollectionWithPaginationData<T>
	{
		public PaginationMetadata PaginationMetadata { get; set; }
		public PagedList<T> Collection { get; set; }

	}
}
