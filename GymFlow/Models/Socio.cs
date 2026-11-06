using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GymFlow.Models;

[Index("Correo", Name = "UQ_Socios_Correo", IsUnique = true)]
[Index("Dni", Name = "UQ_Socios_Dni", IsUnique = true)]
public partial class Socio
{
    [Key]
    public int SocioId { get; set; }

    [StringLength(8)]
    [Unicode(false)]
    [Required]
    public string Dni { get; set; } = null!;

    [StringLength(80)]
    [Required]
    public string Nombres { get; set; } = null!;

    [StringLength(100)]
    [Required]
    public string Apellidos { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    [Required]
    [EmailAddress]
    public string Correo { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public DateTime FechaRegistro { get; set; }

    public int MembresiaId { get; set; }

    [ForeignKey("MembresiaId")]
    [InverseProperty("Socios")]
    public virtual Membresia? Membresia { get; set; }
}
