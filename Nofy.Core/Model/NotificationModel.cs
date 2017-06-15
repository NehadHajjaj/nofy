﻿namespace Nofy.Core.Model
{
	using System;
	using System.Collections.Generic;

	public class NotificationModel
	{
		public virtual ICollection<NotificationActionModel> Actions { get; set; }
		public DateTime? ArchivedOn { get; set; }
		public int? Category { get; set; }
		public DateTime CreatedOn { get; set; }
		public string Description { get; set; }
		public string EntityId { get; set; }
		public string EntityType { get; set; }

		public int Id { get; set; }
		public string RecipientId { get; set; }
		public string RecipientType { get; set; }
		public NotificationStatus Status { get; set; }
		public string Summary { get; set; }



	}
}