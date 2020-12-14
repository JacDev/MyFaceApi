using Pagination.Helpers;

namespace Pagination.DtoModels
{
	public class CollectionWithPaginationData<T>
	{
		public PaginationMetadata PaginationMetadata { get; set; }
		public PagedList<T> Collection { get; set; }
	}
}
