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

    // Ajuste o horário de check-in e check-out para 12:00
    var checkInDate = DateTime.Parse(reservation.CheckInDate).Date.AddHours(12); // Check-in sempre às 12h
    var checkOutDate = DateTime.Parse(reservation.CheckOutDate).Date.AddHours(12); // Check-out sempre às 12h

    // Verifica se o check-out é posterior ao check-in
    if (checkOutDate <= checkInDate)
    {
        return BadRequest("Data de check-out deve ser posterior à data de check-in.");
    }

    // Calcular a quantidade de dias
    int totalDays = (checkOutDate - checkInDate).Days;

    // Verificar se o quarto já está reservado nas datas selecionadas
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

    // Cálculo do valor total com base na quantidade de dias e no preço do quarto
    decimal valorTotalAdultos = room.PricePerNight * totalDays;

    // Se houver crianças a partir de 6 anos, aplicar 50% de desconto na diária delas
    decimal valorDescontoCriancas = (room.PricePerNight * 0.5m) * reservation.NumeroDeCriancas * totalDays;

    // Crianças de 0 a 5 anos não pagam
    decimal valorFinalCriancas0A5 = 0;

    // Somar valores para calcular o preço total
    reservation.TotalPrice = valorTotalAdultos + valorDescontoCriancas;

    // Salvar a reserva e marcar o quarto como ocupado
    _context.Reservations.Add(reservation);
    await _context.SaveChangesAsync();

    // Atualizar o status do quarto para ocupado
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

    // Ajuste o horário de check-in e check-out para 12:00
    var checkInDate = DateTime.Parse(reservation.CheckInDate).Date.AddHours(12);
    var checkOutDate = DateTime.Parse(reservation.CheckOutDate).Date.AddHours(12);

    // Verifica se o check-out é posterior ao check-in
    if (checkOutDate <= checkInDate)
    {
        return BadRequest("Data de check-out deve ser posterior à data de check-in.");
    }

    // Calcular a quantidade de dias
    int totalDays = (checkOutDate - checkInDate).Days;

    // Verificar se o quarto já está reservado nas datas selecionadas (excluindo a reserva atual)
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

    // Cálculo do valor total com base na quantidade de dias e no preço do quarto
    decimal valorTotalAdultos = room.PricePerNight * totalDays;

    // Se houver crianças a partir de 6 anos, aplicar 50% de desconto na diária delas
    decimal valorDescontoCriancas = (room.PricePerNight * 0.5m) * reservation.NumeroDeCriancas * totalDays;

    // Crianças de 0 a 5 anos não pagam
    decimal valorFinalCriancas0A5 = 0;

    // Somar valores para calcular o preço total
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

    // Atualizar o status do quarto
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
