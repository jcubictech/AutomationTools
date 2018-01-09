using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync
{
    public class ReservationInfo
    {
        public int Adult { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string UnitID { get; set; }
        public float Price { get; set; }
        public float ArnbnbFee { get; set; }
        public float CleaningFee { get; set; }
        public float TotalAmount { get; set; }
        public string Code { get; set; }

        /*
        public UserInfo User { get; set; }
        public ReservationItem Item { get; set; }
        //*/

        public string ReservationId { get; set; }
        public string ReservationCode { get; set; }
        public string Dates { get; set; }

        public string RoomId { get; set; }
        public string RoomTitle { get; set; }
        public string RoomAddress { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PriceString { get; set; }

        public string GuestFirstName { get; set; }
        public string GuestLastName { get; set; }
        public object GuestEmail { get; set; }

    }

    /*
    public class UserInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class ReservationItem
    {
        public string ReservationId { get; set; }
        public string ReservationCode { get; set; }
        public string Dates { get; set; }
        public string RoomId { get; set; }
        public string RoomTitle { get; set; }
        public string RoomAddress { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Price { get; set; }
        public object Email { get; set; }
    }
    //*/
}
