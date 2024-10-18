using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApi.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } 
        public string Type { get; set; }
        public decimal PricePerNight { get; set; } 
        public bool IsOccupied { get; set; } 
       

    }
}