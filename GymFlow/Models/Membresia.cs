using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace GymFlow.Models;

[Index("Nombre", Name = "UQ_Membresias_Nombre", IsUnique = true)]
public partial class Membresia
{
    [Key]
    public int MembresiaId { get; set; }

    [StringLength(60)]
    [Required]
    public string Nombre { get; set; } = null!;

    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Range(1, 24)]
    public short DuracionMeses { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    [Range(0.01, 99999999.99)]
    public decimal Precio { get; set; }

    public bool Activa { get; set; }

    public DateTime FechaCreacion { get; set; }

    [InverseProperty("Membresia")]
    [JsonIgnore]
    public virtual ICollection<Socio> Socios { get; set; } = new List<Socio>();
}
