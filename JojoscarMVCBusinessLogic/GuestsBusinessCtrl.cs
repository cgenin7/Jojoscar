using JojoscarMVCCommun;
using JojoscarMVCDBLayer;
using System.Collections.Generic;
using System.Linq;

namespace JojoscarMVCBusinessLogic
{
    public class GuestsBusinessCtrl
    {
        public static List<GuestModel> GetGuests(int year)
        {
            return GuestRepository.GetGuests(year).OrderBy(g => g.AccessCode).ToList();
        }

        public static GuestModel GetGuest(int year, int guestId)
        {
            return GuestRepository.GetGuest(year, guestId);
        }

        public static void AddGuest(int year, GuestModel guest)
        {
            GuestRepository.AddGuest(year, guest);
        }

        public static void DeleteGuest(int year, int guestId)
        {
            GuestRepository.DeleteGuest(year, guestId);
        }

        public static void EditGuest(int year, GuestModel guest)
        {
            GuestRepository.EditGuest(year, guest);
        }
    }
}
