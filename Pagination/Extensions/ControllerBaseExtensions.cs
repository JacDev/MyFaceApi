using Microsoft.AspNetCore.Mvc;
using Pagination.DtoModels;
using Pagination.Helpers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Pagination.Extensions
{
	public static class ControllerBaseExtensions
	{
		public static CollectionWithPaginationData<T> CreateCollectionWithPagination<T>(this ControllerBase controllerBase,
			PagedList<T> collection,
			PaginationParams paginationParams,
			string methodName,
			Dictionary<object, object> queries = null)
		{
			PaginationMetadata metadata = PaginationHelper.CreatePaginationMetadata(collection);
			metadata.PreviousPageLink = metadata.HasPrevious ?
				controllerBase.CreateLink(paginationParams, ResourceUriType.PreviousPage, methodName, queries) : null;

			metadata.NextPageLink = metadata.HasNext ?
				controllerBase.CreateLink(paginationParams, ResourceUriType.NextPage, methodName, queries) : null;

			return new CollectionWithPaginationData<T> { PaginationMetadata = metadata, Collection = collection };
		}
		private static string CreateLink(this ControllerBase controllerBase,
			PaginationParams paginationParams,
			ResourceUriType type,
			string nameOfMethodToRedirect,
			Dictionary<object, object> queries)
		{
			var pageNumber = type switch
			{
				ResourceUriType.PreviousPage => paginationParams.PageNumber - 1,
				ResourceUriType.NextPage => paginationParams.PageNumber + 1,
				_ => paginationParams.PageNumber,
			};

			var query = new
			{
				pageNumber,
				pageSize = paginationParams.PageSize
			};

			string link = controllerBase.Url.Link(nameOfMethodToRedirect, query);
			NameValueCollection myCol = new NameValueCollection();
			if (queries != null)
			{
				foreach (KeyValuePair<object, object> quer in queries)
				{
					myCol.Add(quer.Key.ToString(), quer.Value.ToString()); ;
				}
				link = link + '&' + ToQueryString(myCol);
			}


			return link;
		}
		private static string ToQueryString(NameValueCollection nvc)
		{
			var array = (
				from key in nvc.AllKeys
				from value in nvc.GetValues(key)
				select string.Format(
			"{0}={1}",
			HttpUtility.UrlEncode(key),
			HttpUtility.UrlEncode(value))
				).ToArray();
			return string.Join("&", array);
		}

	}
}
