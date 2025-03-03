﻿using Flunt.Notifications;

namespace MechShops.Domain;

public abstract class Entity : Notifiable<Notification>
{
    public int Id { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
