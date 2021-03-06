﻿// ReSharper disable VirtualMemberCallInConstructor

namespace Nofy.Core.Model
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
    using Nofy.Core.Helper;

    /// <summary>
	/// Represent notification object
	/// </summary>
	public class Notification
	{
		/// <summary>
		/// Create new instnace of Notification class 
		/// </summary>
		/// <param name="description"></param>
		/// <param name="entityType"></param>
		/// <param name="entityId"></param>
		/// <param name="recipientType"></param>
		/// <param name="recipientId"></param>
		/// <param name="summary"></param>
		/// <param name="category"></param>
		/// <param name="actions"></param>
		public Notification(string description,
			string entityType,
			string entityId,
			string recipientType,
			string recipientId,
			string summary,
			int? category,
			params NotificationAction[] actions)
		{
			this.Description = description?.EnsureLength(NotificationServiceConfiguration.DescriptionLimit);
			this.EntityType = entityType;
			this.EntityId = entityId;
			this.RecipientType = recipientType;
			this.RecipientId = recipientId;
			this.Actions = actions?.ToList() ?? new List<NotificationAction>();
			this.Status = NotificationStatus.UnRead;
			this.CreatedOn = DateTime.UtcNow;
			this.Summary = summary?.EnsureLength(NotificationServiceConfiguration.SummaryLimit);
			this.Category = category;
			this.Archived = false;
			var context = new ValidationContext(this, serviceProvider: null, items: null);
			var results = new List<ValidationResult>();

			var isValid = Validator.TryValidateObject(this, context, results, true);
			if (!isValid)
			{
				foreach (var validationResult in results)
				{
					throw new ValidationException(validationResult.ErrorMessage);
				}
			}
		}

		public Notification()
		{
		}

		/// <summary>
		/// Get or set the list of actions for notification
		/// </summary>
		public virtual ICollection<NotificationAction> Actions { get; private set; }

		/// <summary>
		/// Get or set bool value that indecate the notification archived status.
		/// </summary>
		public bool Archived { get; private set; }

		/// <summary>
		/// Get or set the date that notification archived on
		/// </summary>
		public DateTime? ArchivedOn { get; private set; }

		/// <summary>
		/// Get or set notification category
		/// </summary>
		public int? Category { get; private set; }

		/// <summary>
		/// Get or set the creation date .
		/// </summary>
		public DateTime CreatedOn { get; private set; }

		/// <summary>
		/// Get or set the notification description
		/// </summary>
		[MaxLength(NotificationValidation.MaxDescriptionLength)]
		public string Description { get; private set; }

		/// <summary>
		/// Get or set the related entity id of the notification
		/// </summary>
		[Required]
		[MaxLength(NotificationValidation.MaxEntityIdLength)]
		public string EntityId { get; private set; }

		/// <summary>
		/// Get or set the related entity type of the notification
		/// </summary>
		[MaxLength(NotificationValidation.MaxEntityTypeLength)]
		public string EntityType { get; private set; }

		/// <summary>
		/// Get or set the notificatoin id 
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Get or set the recipient of notification
		/// </summary>
		[MaxLength(NotificationValidation.MaxRecipientIdLength)]
		public string RecipientId { get; private set; }

		/// <summary>
		/// Get or set the type of recipient of notification
		/// </summary>
		[MaxLength(NotificationValidation.MaxRecipientTypeLength)]
		public string RecipientType { get; private set; }

		/// <summary>
		/// Get or set enum value that indecate the notification status.
		/// </summary>
		public NotificationStatus Status { get; private set; }

		/// <summary>
		/// Short summery 
		/// </summary>
		[MaxLength(NotificationValidation.MaxSummaryLength)]
		public string Summary { get; private set; }

		/// <summary>
		/// Add new action to notfication actions
		/// </summary>
		/// <param name="action"></param>
		public void AddAction(NotificationAction action)
		{
			this.Actions.Add(action);
		}

		/// <summary>
		/// Archive notification
		/// </summary>
		/// <returns></returns>
		public bool Archive()
		{
			if (this.Archived)
			{
				return false;
			}

			this.Archived = true;
			this.ArchivedOn = DateTime.UtcNow;
			return true;
		}

		/// <summary>
		/// Clone this notification to new "notification"
		/// </summary>
		/// <param name="notification"></param>
		public void CopyTo(Notification notification)
		{
			if (this.Id != notification.Id)
			{
				throw new NotificationException("Id of domain and data objects don't match.");
			}

			notification.ArchivedOn = this.ArchivedOn;
			notification.CreatedOn = this.CreatedOn;
			notification.Description = this.Description;
			notification.EntityType = this.EntityType;
			notification.EntityId = this.EntityId;
			notification.RecipientId = this.RecipientId;
			notification.RecipientType = this.RecipientType;
			notification.Status = this.Status;
			notification.Summary = this.Summary;
			notification.Category = this.Category;
		}

		/// <summary>
		/// Specify whether notification is archived.
		/// </summary>
		/// <returns></returns>
		public bool IsArchived()
		{
			return this.Archived;
		}

		/// <summary>
		/// Specify whether notification is read before or not
		/// </summary>
		/// <returns></returns>
		public bool IsRead()
		{
			return this.Status == NotificationStatus.Read;
		}

		/// <summary>
		/// Specify whether notification is unread before or not
		/// </summary>
		/// <returns></returns>
		public bool IsUnread()
		{
			return this.Status == NotificationStatus.UnRead;
		}

		/// <summary>
		/// Mark as read
		/// </summary>
		public bool MarkAsRead()
		{
			if (this.IsRead())
			{
				return false;
			}

			this.Status = NotificationStatus.Read;
			return true;
		}

		/// <summary>
		/// Mark as unread
		/// </summary>
		public bool MarkAsUnread()
		{
			if (this.IsUnread())
			{
				return false;
			}
			this.Status = NotificationStatus.UnRead;
			return true;
		}

		/// <summary>
		/// Revert notification from archive and mark it as read
		/// </summary>
		/// <returns></returns>
		public bool UnArchive()
		{
			if (!this.Archived)
			{
				return false;
			}

			this.Status = NotificationStatus.Read;
			this.Archived = false;
			this.ArchivedOn = null;
			return true;
		}
	}
}