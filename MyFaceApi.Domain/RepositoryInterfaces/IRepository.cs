using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Domain.RepositoryInterfaces
{
	public interface IRepository<TEntity> where TEntity : class
	{
		void Remove(TEntity entityToDelete);
		void Remove(object id);
		IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>,
				IOrderedQueryable<TEntity>> orderBy = null,
			string includeProperties = "");
		TEntity GetById(object id);
		Task<TEntity> AddAsync(TEntity entity);
		void Update(TEntity entityToUpdate);
		Task SaveAsync();
	}
}

