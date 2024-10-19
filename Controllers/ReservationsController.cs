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

        // Listar todas as reservas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Cliente)
                .Include(r => r.Room)
                .ToListAsync();
        }

        // Obter uma reserva específica
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

            var reservasDoQuarto = _context.Reservations
                .Where(r => r.RoomId == reservation.RoomId)
                .AsEnumerable()
                .ToList();

            var isRoomReserved = reservasDoQuarto.Any(r =>
            {
                var checkInExistente = DateTime.Parse(r.CheckInDate);
                var checkOutExistente = DateTime.Parse(r.CheckOutDate);
                var novoCheckIn = DateTime.Parse(reservation.CheckInDate);
                var novoCheckOut = DateTime.Parse(reservation.CheckOutDate);
                return (novoCheckIn <= checkOutExistente && novoCheckOut >= checkInExistente);
            });

            if (isRoomReserved)
            {
                return BadRequest("Este quarto já está reservado nas datas selecionadas.");
            }

            reservation.Cliente = cliente;
            reservation.Room = room;

            // Cálculo para adultos adicionais (40 reais por adulto extra)
            decimal adicionalPorAdulto = 40;
            decimal valorFinalAdultos = room.PricePerNight + (reservation.NumeroDeAdultos > 1 ? (reservation.NumeroDeAdultos - 1) * adicionalPorAdulto : 0);

            // Cálculo para crianças com 50% de desconto
            decimal descontoPorCrianca = room.PricePerNight * 0.5m;
            decimal valorFinalCriancas = reservation.NumeroDeCriancas > 0 ? descontoPorCrianca * reservation.NumeroDeCriancas : 0;

            // Somar os dois valores para obter o preço total
            reservation.TotalPrice = valorFinalAdultos + valorFinalCriancas;

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Atualizar o status do quarto para ocupado
            room.IsOccupied = true;
            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }

        // Atualizar uma reserva existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest();
            }

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

            return NoContent();
        }

        // Excluir uma reserva
        // Excluir uma reserva
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            // Liberar o quarto
            var room = await _context.Rooms.FindAsync(reservation.RoomId);
            if (room != null)
            {
                room.IsOccupied = false;
                _context.Entry(room).State = EntityState.Modified;
            }

            // Remover a reserva
            _context.Reservations.Remove(reservation);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Logue o erro ou trate conforme necessário
                return StatusCode(500, "Erro ao excluir a reserva. Detalhes: " + ex.Message);
            }

            return NoContent();
        }


    }
}
