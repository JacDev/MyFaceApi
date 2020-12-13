﻿using Microsoft.EntityFrameworkCore;
using MyFaceApi.Api.Domain.DatabasesInterfaces;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyFaceApi.Api.Infrastructure.Repository
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
	{
		private readonly IAppDbContext _dbContext;
		private readonly DbSet<TEntity> _dbSet;
		public Repository(IAppDbContext appDbContext)
		{
			_dbContext = appDbContext;
			_dbSet = _dbContext.Set<TEntity>();
		}

		public async Task<TEntity> AddAsync(TEntity entity)
		{
			var addedEntity = await _dbSet.AddAsync(entity);
			await _dbContext.SaveAsync();
			return addedEntity.Entity;
		}

		public void Remove(TEntity entityToDelete)
		{
			if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
			{
				_dbSet.Attach(entityToDelete);
			}
			_dbSet.Remove(entityToDelete);
		}

		public void Remove(object id)
		{
			TEntity entityToDelete = _dbSet.Find(id);
			Remove(entityToDelete);
		}
		public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
		{
			IQueryable<TEntity> query = _dbSet;

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
		public TEntity GetById(object id)
		{
			return _dbSet.Find(id);
		}

		public void Update(TEntity entityToUpdate)
		{
			_dbSet.Attach(entityToUpdate);
			_dbContext.Entry(entityToUpdate).State = EntityState.Modified;
		}
		public async Task SaveAsync()
		{
			await _dbContext.SaveAsync();
		}
	}
}
