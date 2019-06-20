﻿using Doug.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Doug.Repositories
{
    public interface IUserRepository
    {
        void AddUser(string userId);
        List<User> GetUsers();
        User GetUser(string userId);
        void RemoveCredits(string userId, int amount);
        void AddCredits(string userId, int amount);
        void AddItem(string userId, string itemId);
        void RemoveItem(string userId, int inventoryPosition);
    }

    public class UserRepository : IUserRepository
    {
        private readonly DougContext _db;

        public UserRepository(DougContext dougContext)
        {
            _db = dougContext;
        }

        public void AddCredits(string userId, int amount)
        {
            var user = _db.Users.Single(usr => usr.Id == userId);

            user.Credits += amount;

            _db.SaveChanges();
        }

        public void AddItem(string userId, string itemId)
        {
            var user = _db.Users
                .Include(usr => usr.InventoryItems)
                .Single(usr => usr.Id == userId);

            var slots = user.InventoryItems.Select(itm => itm.InventoryPosition).ToList();

            var slot = 0;
            if (slots.Any())
            {
                var maxPos = slots.Any() ? slots.Max() : 0;
                var freeSlots = Enumerable.Range(0, maxPos).Except(slots).ToList();

                slot = freeSlots.Any() ? freeSlots.First() : maxPos + 1;
            }

            user.InventoryItems.Add(new InventoryItem(userId, itemId) { InventoryPosition = slot });
            _db.SaveChanges();
        }

        public void RemoveItem(string userId, int inventoryPosition)
        {
            var user = _db.Users.Single(usr => usr.Id == userId);
            var item = user.InventoryItems.Single(itm => itm.InventoryPosition == inventoryPosition);
            user.InventoryItems.Remove(item);
            _db.SaveChanges();
        }

        public void AddUser(string userId)
        {
            if (!_db.Users.Any(user => user.Id == userId)) {

                var user = new User()
                {
                    Id = userId,
                    Credits = 10
                };

                _db.Users.Add(user);
                _db.SaveChanges();
            }
        }

        public User GetUser(string userId)
        {
            return _db.Users
                .Include(user => user.InventoryItems)
                .Single(user => user.Id == userId);
        }

        public List<User> GetUsers()
        {
            return _db.Users.ToList();
        }

        public void RemoveCredits(string userId, int amount)
        {
            var user = _db.Users.Single(usr => usr.Id == userId);

            user.Credits -= amount;
            _db.SaveChanges();
        }
    }
}
