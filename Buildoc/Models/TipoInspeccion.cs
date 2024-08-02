﻿using System.ComponentModel.DataAnnotations;

namespace Buildoc.Models
{
    public class TipoInspeccion
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        [Required]
        public string Nombre { get; set; }
        [Display(Name = "Categoría")]
        [Required]
        public CategoriaInspeccion Categoria { get; set; }
        [Display(Name = "Descripción")]
        [Required]
        public string Descripcion { get; set; }


    }
}
