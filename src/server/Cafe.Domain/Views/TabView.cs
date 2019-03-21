﻿using Baseline;
using Cafe.Domain.Entities;
using Cafe.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cafe.Domain.Views
{
    public class TabView
    {
        public Guid Id { get; set; }

        public int TableNumber { get; set; }

        public string CustomerName { get; set; }

        public string WaiterName { get; set; }

        public decimal ServedItemsValue { get; set; }

        public bool IsOpen { get; set; }

        public decimal TipValue { get; set; }

        public decimal TotalPaid { get; set; }

        public IList<MenuItem> OutstandingMenuItems { get; set; } = new List<MenuItem>();

        public IList<MenuItem> ServedMenuItems { get; set; } = new List<MenuItem>();

        public void Apply(TabOpened @event)
        {
            IsOpen = true;
            CustomerName = @event.CustomerName;
            WaiterName = @event.WaiterName;
            TableNumber = @event.TableNumber;
        }

        public void Apply(TabClosed @event)
        {
            IsOpen = false;
            TipValue = @event.TipValue;
        }

        public void Apply(MenuItemsOrdered @event)
        {
            foreach (var item in @event.MenuItems)
            {
                OutstandingMenuItems.Add(item);
            }
        }

        public void Apply(MenuItemsServed @event)
        {
            ServedMenuItems.AddRange(@event.MenuItems);
            ServedItemsValue += @event.MenuItems.Sum(i => i.Price);

            foreach (var servedItem in @event.MenuItems)
            {
                var outstanding = OutstandingMenuItems
                    .FirstOrDefault(b => b.Number == servedItem.Number);

                if (outstanding != null)
                {
                    OutstandingMenuItems.Remove(outstanding);
                }
            }
        }
    }
}