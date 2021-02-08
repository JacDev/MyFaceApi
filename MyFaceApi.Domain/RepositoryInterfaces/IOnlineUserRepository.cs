using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Domain.RepositoryInterfaces
{
	public interface IOnlineUserRepository<TEntity> where TEntity : class
	{
		Task<TEntity> AddAsync(TEntity entity);

		TEntity GetById(object id);
		IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>,
				IOrderedQueryable<TEntity>> orderBy = null,
			string includeProperties = "");
		void Remove(TEntity entityToDelete);
		void Remove(object id);
		Task SaveAsync();
	}
}
