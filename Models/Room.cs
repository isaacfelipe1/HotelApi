using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApi.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } // Ex: 101
        public string Type { get; set; } // Ex: Individual, Duplo, Suíte
        public decimal PricePerNight { get; set; } // Preço por noite
        public bool IsOccupied { get; set; } // Status: Disponível ou Ocupado
    }
}