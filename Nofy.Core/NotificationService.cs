﻿namespace Nofy.Core
{
	using System;
	using System.Collections.Generic;
	using Nofy.Core.Helper;
	using Nofy.Core.Model;

	/// <inheritdoc />
	/// <summary>
	/// Manage notifications for a repository 
	/// </summary>
	public class NotificationService : IDisposable
	{
		private readonly object lockKey = new object();

		//Notification repository
		private readonly INotificationRepository notificationRepository;

		//Temporary list of notification 
		private readonly List<Notification> notifications = new List<Notification>();

		/// <summary>
		/// Initialize new instance of notification service
		/// </summary>
		/// <param name="notificationRepository">Service's repository</param>
		public NotificationService(INotificationRepository notificationRepository)
		{
			this.notificationRepository = notificationRepository;
			this.Config = new NotificationServiceConfiguration
			{
				BatchLimit = 0
			};
		}



		/// <summary>
		/// Service configurations
		/// </summary>
		public NotificationServiceConfiguration Config { get; set; }

		/// <inheritdoc />
		/// <summary>
		/// Dispose all resources
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Archive notification by id
		/// </summary>
		/// <param name="id">Notification id</param>
		public int Archive(int id)
		{
			return this.notificationRepository.Archive(id);
		}

		/// <summary>
		/// Get notification by id
		/// </summary>
		/// <param name="notificationId">Notification id</param>
		/// <returns></returns>
		public Notification GetNotification(int notificationId)
		{
			return this.notificationRepository.GetNotification(notificationId);
		}

		/// <summary>
		/// Get paginated list of notifications for list of recipients 
		/// </summary>
		/// <param name="recipients">List of recipients </param>
		/// <param name="pageIndex">Page index</param>
		/// <param name="pageSize">Page size : maximum number of notification to be returned </param>
		/// <param name="showArchived">Include archived notification in result</param>
		/// <param name="title"></param>
		/// <returns></returns>
		public PaginatedData<Notification> GetNotifications(
			IEnumerable<NotificationRecipient> recipients,
			int pageIndex,
			int pageSize = 10,
			bool showArchived = false, 
			string title = "")
		{
			return this.notificationRepository.GetNotifications(recipients, pageIndex, pageSize, showArchived, title);
		}

		/// <summary>
		/// Return number of unread notifications for specific recipients.
		/// </summary>
		/// <param name="recipients"></param>
		/// <returns></returns>
		public int GetNotificationCounter(IEnumerable<NotificationRecipient> recipients)
		{
			return this.notificationRepository.NotReadNotificationCount(recipients);
		}


		/// <summary>
		/// Update notification status to read.
		/// </summary>
		/// <param name="notificationId"></param>
		public int MarkAsRead(int notificationId)
		{
			return this.notificationRepository.MarkAsRead(notificationId);
		}

		/// <summary>
		/// Update notification status to unread.
		/// </summary>
		/// <param name="notificationId"></param>
		public int MarkAsUnread(int notificationId)
		{
			return this.notificationRepository.MarkAsUnread(notificationId);
		}

		/// <summary>
		/// Publish notifications to user
		/// </summary>
		/// <param name="notification"></param>
		public void Publish(Notification notification)
		{
			lock (this.lockKey)
			{
				this.notifications.Add(notification);
			}

			lock (this.lockKey)
			{
				if (this.notifications.Count > this.Config.BatchLimit)
				{
					this.SaveBatch();
				}
			}
		}

		/// <summary>
		/// Un-archive notification by id
		/// </summary>
		/// <param name="id">Notification id</param>
		public int UnArchive(int id)
		{
			return this.notificationRepository.UnArchive(id);
		}

		/// <summary>
		/// Dispose resources
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.SaveBatch();
			}
		}

		/// <summary>
		/// Bulk save from temporary list to the repository 
		/// </summary>
		private void SaveBatch()
		{
			lock (this.lockKey)
			{
				if (this.notifications.Count == 0)
				{
					return;
				}

				this.notificationRepository.AddRange(this.notifications);
				this.notifications.Clear();
			}
		}
	}
}