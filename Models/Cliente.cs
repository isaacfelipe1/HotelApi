using System;

namespace HotelApi.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string? Profissao { get; set; }
        public string? Nacionalidade { get; set; }

        public string? DataNascimento { get; set; }

        public string? Sexo { get; set; }
        public string? RG { get; set; }
        public string? Residencia { get; set; }
        public string? CEP { get; set; }
        public string? Cidade { get; set; }
        public string? Pais { get; set; }
        public string? MotivoViagem { get; set; }
        public string? MeioTransporte { get; set; }
        public string? ProximoDestino { get; set; }
        public string? TelefoneResidencial { get; set; }
        public string? TelefoneComercial { get; set; }

        public DateTime? DataEntrada { get; set; }
        public DateTime? DataSaida { get; set; }

        public string? Acompanhante { get; set; }
        public decimal? ValorEstadia { get; set; }
        public string? Apartamento { get; set; }
    }
}
