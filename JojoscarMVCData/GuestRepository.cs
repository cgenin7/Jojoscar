using JojoscarMVCCommun;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace JojoscarMVCDBLayer
{
    public static class GuestRepository
    {
        public static List<GuestModel> GetGuests(int year)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                return dbContext.Guests.AsNoTracking().ToList();
            }
        }

        public static GuestModel GetGuest(int year, int guestId)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                return dbContext.Guests.AsNoTracking().FirstOrDefault(g => g.GuestID == guestId);
            }
        }

        public static void AddGuest(int year, GuestModel guestToAdd)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                dbContext.Guests.Add(guestToAdd);
                dbContext.SaveChanges();
            }
        }

        public static void DeleteGuest(int year, int guestId)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                dbContext.Database.ExecuteSqlCommand("exec DeleteGuest @guestId = {0}", guestId);
            }
        }

        public static void EditGuest(int year, GuestModel guestToModify)
        {
            using (var dbContext = new JojoscarDbContext(year))
            {
                dbContext.Entry(guestToModify).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
    }
}
