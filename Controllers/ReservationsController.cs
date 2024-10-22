using HotelApi.Data;
using HotelApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Cliente)
                .Include(r => r.Room)
                .ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Cliente)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

[HttpPost]
public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
{
    var cliente = await _context.Clientes.FindAsync(reservation.ClienteId);
    var room = await _context.Rooms.FindAsync(reservation.RoomId);

    if (cliente == null)
    {
        return BadRequest("Cliente não encontrado.");
    }

    if (room == null)
    {
        return BadRequest("Quarto não encontrado.");
    }
    var checkInDate = DateTime.Parse(reservation.CheckInDate).Date.AddHours(12); 
    var checkOutDate = DateTime.Parse(reservation.CheckOutDate).Date.AddHours(12); 

    if (checkOutDate <= checkInDate)
    {
        return BadRequest("Data de check-out deve ser posterior à data de check-in.");
    }
    int totalDays = (checkOutDate - checkInDate).Days;
    var reservasDoQuarto = _context.Reservations
        .Where(r => r.RoomId == reservation.RoomId)
        .AsEnumerable()
        .ToList();

    var isRoomReserved = reservasDoQuarto.Any(r =>
    {
        var checkInExistente = DateTime.Parse(r.CheckInDate).Date.AddHours(12);
        var checkOutExistente = DateTime.Parse(r.CheckOutDate).Date.AddHours(12);
        return (checkInDate < checkOutExistente && checkOutDate > checkInExistente);
    });

    if (isRoomReserved)
    {
        return BadRequest("Este quarto já está reservado nas datas selecionadas.");
    }

    reservation.Cliente = cliente;
    reservation.Room = room;
    decimal valorTotalAdultos = room.PricePerNight * totalDays;

    
    decimal valorDescontoCriancas = (room.PricePerNight * 0.5m) * reservation.NumeroDeCriancas * totalDays;

    decimal valorFinalCriancas0A5 = 0;

    reservation.TotalPrice = valorTotalAdultos + valorDescontoCriancas;
    _context.Reservations.Add(reservation);
    await _context.SaveChangesAsync();

    room.IsOccupied = true;
    _context.Entry(room).State = EntityState.Modified;
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
}


  [HttpPut("{id}")]
public async Task<IActionResult> PutReservation(int id, Reservation reservation)
{
    if (id != reservation.Id)
    {
        return BadRequest("O ID da reserva não corresponde.");
    }

    var cliente = await _context.Clientes.FindAsync(reservation.ClienteId);
    var room = await _context.Rooms.FindAsync(reservation.RoomId);

    if (cliente == null)
    {
        return BadRequest("Cliente não encontrado.");
    }

    if (room == null)
    {
        return BadRequest("Quarto não encontrado.");
    }
    var checkInDate = DateTime.Parse(reservation.CheckInDate).Date.AddHours(12);
    var checkOutDate = DateTime.Parse(reservation.CheckOutDate).Date.AddHours(12);

    if (checkOutDate <= checkInDate)
    {
        return BadRequest("Data de check-out deve ser posterior à data de check-in.");
    }
    int totalDays = (checkOutDate - checkInDate).Days;

    var reservasDoQuarto = _context.Reservations
        .Where(r => r.RoomId == reservation.RoomId && r.Id != id)
        .AsEnumerable()
        .ToList();

    var isRoomReserved = reservasDoQuarto.Any(r =>
    {
        var checkInExistente = DateTime.Parse(r.CheckInDate).Date.AddHours(12);
        var checkOutExistente = DateTime.Parse(r.CheckOutDate).Date.AddHours(12);
        return (checkInDate < checkOutExistente && checkOutDate > checkInExistente);
    });

    if (isRoomReserved)
    {
        return BadRequest("Este quarto já está reservado nas datas selecionadas.");
    }

    reservation.Cliente = cliente;
    reservation.Room = room;
    decimal valorTotalAdultos = room.PricePerNight * totalDays;

 
    decimal valorDescontoCriancas = (room.PricePerNight * 0.5m) * reservation.NumeroDeCriancas * totalDays;

   
    decimal valorFinalCriancas0A5 = 0;

   
    reservation.TotalPrice = valorTotalAdultos + valorDescontoCriancas;

    _context.Entry(reservation).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!_context.Reservations.Any(e => e.Id == id))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }
    room.IsOccupied = true;
    _context.Entry(room).State = EntityState.Modified;
    await _context.SaveChangesAsync();

    return NoContent();
}
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

     
            var room = await _context.Rooms.FindAsync(reservation.RoomId);
            if (room != null)
            {
                room.IsOccupied = false;
                _context.Entry(room).State = EntityState.Modified;
            }

  
            _context.Reservations.Remove(reservation);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
              
                return StatusCode(500, "Erro ao excluir a reserva. Detalhes: " + ex.Message);
            }

            return NoContent();
        }


    }
}
