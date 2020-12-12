using MyFaceApi.Api.Application.Helpers;

namespace MyFaceApi.Api.Application.DtoModels
{
	public class CollectionWithPaginationData<T>
	{
		public PaginationMetadata PaginationMetadata { get; set; }
		public PagedList<T> Collection { get; set; }
	}
}
