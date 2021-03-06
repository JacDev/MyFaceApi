﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Pagination.Helpers
{
	public class PagedList<T> : List<T>
	{
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
		public bool HasPrevious => (CurrentPage > 1);
		public bool HasNext => (CurrentPage < TotalPages);

		public PagedList()
		{

		}
		public PagedList(List<T> items, int count, int pageNumber, int pageSize)
		{
			TotalCount = count;
			PageSize = pageSize;
			CurrentPage = pageNumber;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			if (items != null)
			{
				AddRange(items);
			}
		}
		public static PagedList<dest> CreateNewWithSameParams<source, dest>(PagedList<source> old, List<dest> newCollection)
		{
			return PagedList<dest>.Create(newCollection, old.CurrentPage, old.PageSize, 0);
		}
		public static PagedList<T> Create(List<T> source, int pageNumber, int pageSize, int skip)
		{
			if (source != null)
			{
				var count = source.Count;
				var items = source.Skip(skip).Take(pageSize).ToList();
				return new PagedList<T>(items, count, pageNumber, pageSize);
			}
			else
			{
				return new PagedList<T>(null, 0, 0, 0);
			}
		}
		public void AddMetadataParams(PaginationMetadata paginationMetadata)
		{
			TotalCount = paginationMetadata.TotalCount;
			PageSize = paginationMetadata.PageSize;
			CurrentPage = paginationMetadata.CurrentPage;
			TotalPages = paginationMetadata.TotalPages;
		}
	}
}
