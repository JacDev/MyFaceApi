using Microsoft.EntityFrameworkCore;
using MyFaceApi.Api.Domain.DatabasesInterfaces;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Infrastructure.Repository
{

	public class OnlineUsersRepository<TEntity> : IOnlineUserRepository<TEntity> where TEntity : class
	{
		private readonly IOnlineUsersDbContext _dbContext;
		public OnlineUsersRepository(IOnlineUsersDbContext appDbContext)
		{
			_dbContext = appDbContext;
		}
		public async Task<TEntity> AddAsync(TEntity entity)
		{
			var addedEntity = await _dbContext.Set<TEntity>().AddAsync(entity);
			await _dbContext.SaveAsync();
			return addedEntity.Entity;
		}
		public void Remove(TEntity entityToDelete)
		{
			if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
			{
				_dbContext.Set<TEntity>().Attach(entityToDelete);
			}
			_dbContext.Set<TEntity>().Remove(entityToDelete);
		}

		public void Remove(object id)
		{
			TEntity entityToDelete = _dbContext.Set<TEntity>().Find(id);
			Remove(entityToDelete);
		}
		public TEntity GetById(object id)
		{
			return _dbContext.Set<TEntity>().Find(id);
		}
		public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
		{
			IQueryable<TEntity> query = _dbContext.Set<TEntity>();

			if (filter != null)
			{
				query = query.Where(filter);
			}
			if (includeProperties != null)
			{
				foreach (var includeProperty in includeProperties.Split
				(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProperty);
				}
			}
			if (orderBy != null)
			{
				return orderBy(query).ToList();
			}
			else
			{
				return query.ToList();
			}
		}
		public async Task SaveAsync()
		{
			await _dbContext.SaveAsync();
		}
	}

}
