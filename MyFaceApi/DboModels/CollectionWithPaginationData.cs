using MyFaceApi.Api.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFaceApi.Api.DboModels
{
	public class CollectionWithPaginationData<T>
	{
		public PaginationMetadata PaginationMetadata { get; set; }
		public PagedList<T> Collection { get; set; }

	}
}
