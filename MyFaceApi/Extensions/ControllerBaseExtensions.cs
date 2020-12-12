using Microsoft.AspNetCore.Mvc;
using MyFaceApi.Api.Application.DtoModels;
using MyFaceApi.Api.Application.Helpers;

namespace MyFaceApi.Api.Extensions
{
	public static class ControllerBaseExtensions
	{
		public static CollectionWithPaginationData<T> CreateCollectionWithPagination<T>(this ControllerBase controllerBase,
			PagedList<T> collection, 
			PaginationParams paginationParams, 
			string methodName)
		{
			PaginationMetadata metadata = PaginationHelper.CreatePaginationMetadata(collection);
			metadata.PreviousPageLink = metadata.HasPrevious ?
				controllerBase.CreateLink(paginationParams, ResourceUriType.PreviousPage, methodName) : null;

			metadata.NextPageLink = metadata.HasNext ?
				controllerBase.CreateLink(paginationParams, ResourceUriType.NextPage, methodName) : null;

			return new CollectionWithPaginationData<T> { PaginationMetadata = metadata, Collection = collection };
		}
		private static string CreateLink(this ControllerBase controllerBase,
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
