using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using MyFaceApi.Api.Application.DtoModels.Notification;
using MyFaceApi.Api.Application.Interfaces;
using MyFaceApi.Api.Domain.Entities;
using MyFaceApi.Api.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFaceApi.Api.Application.Helpers;
using Pagination.Helpers;
using System.Linq.Expressions;
using MyFaceApi.Api.Domain.Enums;
using LinqKit;

namespace MyFaceApi.Api.Application.Services
{
	public class NotificationService : INotificationService
	{
		private readonly IRepository<Notification> _notificationRepository;
		private readonly ILogger<NotificationService> _logger;
		private readonly IMapper _mapper;
		public NotificationService(IRepository<Notification> notificationRepository,
			ILogger<NotificationService> logger,
			IMapper mapper)
		{
			_notificationRepository = notificationRepository;
			_logger = logger;
			_mapper = mapper;
		}
		public async Task<NotificationDto> AddNotificationAsync(Guid toThoId, NotificationToAddDto notification)
		{
			_logger.LogDebug("Trying to add notification {notification}.", notification);
			if (notification is null)
			{
				throw new ArgumentNullException(nameof(notification));
			}
			if (toThoId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(toThoId));
			}
			try
			{
				Notification notificationToAdd = _mapper.Map<Notification>(notification);
				notificationToAdd.ToWhoId = toThoId;
				Notification addedNotification = await _notificationRepository.AddAsync(notificationToAdd);
				await _notificationRepository.SaveAsync();
				return _mapper.Map<NotificationDto>(addedNotification);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during adding the notification.");
				throw;
			}
		}
		public async Task DeleteNotificationAsync(Guid notificationId)
		{
			_logger.LogDebug("Trying to remove notification: {notification}.", notificationId);
			if (notificationId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(notificationId));
			}
			try
			{
				Notification notificationToRemove = _notificationRepository.GetById(notificationId);
				if(notificationToRemove != null)
				{
					_notificationRepository.Remove(notificationToRemove);
					await _notificationRepository.SaveAsync();
					_logger.LogDebug("Notification {notification} has been removed.", notificationToRemove);
				}				
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during removing the notification.");
				throw;
			}
		}
		public NotificationDto GetNotification(Guid notificationId)
		{
			_logger.LogDebug("Trying to get the notification: {notificationId}", notificationId);
			if (notificationId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				Notification notification = _notificationRepository.GetById(notificationId);
				return _mapper.Map<NotificationDto>(notification);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the notification.");
				throw;
			}
		}
		public PagedList<NotificationDto> GetUserNotifications(Guid userId, PaginationParams paginationParams, string fromWhoId = null, int notificationType = 0)
		{
			_logger.LogDebug("Trying to get user notifications: {userid}", userId);
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(Guid));
			}
			try
			{
				var predicate = PredicateBuilder.New<Notification>();
				predicate = predicate.And(x => x.ToWhoId == userId);
				
				if(fromWhoId!= null)
				{
					Guid.TryParse(fromWhoId, out Guid gFromWhoId);

					predicate = predicate.And(x => x.FromWho == gFromWhoId);

				}
				if(notificationType!=0)
				{
					predicate = predicate.And(x => x.NotificationType == (NotificationType)notificationType);
				}
				

				List<Notification> notificationsFromRepo = _notificationRepository
					.Get(predicate)
					.OrderByDescending(x => x.WhenAdded)
					.ToList();
				List<NotificationDto> notificationsToReturn =  _mapper.Map<List<NotificationDto>>(notificationsFromRepo);
				return PagedList<NotificationDto>.Create(notificationsToReturn,
							paginationParams.PageNumber,
							paginationParams.PageSize,
							(paginationParams.PageNumber - 1) * paginationParams.PageSize + paginationParams.Skip);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during getting the user notifications.");
				throw;
			}
		}
		public static Expression<TDelegate> AndAlso<TDelegate>(Expression<TDelegate> left, Expression<TDelegate> right)
		{
			return Expression.Lambda<TDelegate>(Expression.AndAlso(left, right), left.Parameters);
		}
		public async Task<bool> TryUpdateNotificationAsync(Guid notificationId, JsonPatchDocument<NotificationToUpdateDto> patchDocument)
		{
			_logger.LogDebug("Trying to update notification: {id}", notificationId);
			try
			{
				Notification notificationFromRepo = _notificationRepository.GetById(notificationId);
				if (notificationFromRepo == null)
				{
					return false;
				}
				NotificationToUpdateDto postToPatch = _mapper.Map<NotificationToUpdateDto>(notificationFromRepo);
				patchDocument.ApplyTo(postToPatch);
				if (!ValidatorHelper.ValidateModel(postToPatch))
				{
					return false;
				}

				_mapper.Map(postToPatch, notificationFromRepo);
				_notificationRepository.Update(notificationFromRepo);
				await _notificationRepository.SaveAsync();
				_logger.LogDebug("Notification {id} has been updated.", notificationId);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the notification.");
				throw;
			}
		}
		public async Task UpdateNotificationAsync(Notification notification)
		{
			_logger.LogDebug("Trying to update notification: {notification}", notification);
			try
			{
				_notificationRepository.Update(notification);
				await _notificationRepository.SaveAsync();
				_logger.LogDebug("Notification {notification} has been updated.", notification);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured during updating the notification.");
				throw;
			}
		}
	}
}
