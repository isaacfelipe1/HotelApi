using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApi.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; } 
        public int RoomId { get; set; }
        public Room? Room { get; set; } 
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public string Status { get; set; }
        
        // Campos para especificar a quantidade de adultos e crianças
        public int NumeroDeAdultos { get; set; }
        public int NumeroDeCriancas0A5Anos { get; set; } 
        public int NumeroDeCriancas { get; set; }
        public bool IncluirCafeDaManha { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
