using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Buildoc.Models
{
	public class TipoIncidente
	{
		[Key]
		public Guid Id { get; set; }
		public CategoriaEnum Categoria { get; set; }
		public string Titulo { get; set; }
		public string Descripcion { get; set; }
		public string Gravedad { get; set; }
		public bool Estado { get; set; }

		// Collection of Incidentes
		public virtual ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
        // Propiedad para obtener la descripción de la categoría
        public string CategoriaDescripcion => Categoria.GetDescription();

    }
    public static class EnumExtensions
	{
		public static string GetDescription(this Enum value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());

			DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

			return attribute == null ? value.ToString() : attribute.Description;
		}
	}

    public enum CategoriaEnum
	{
		[Description("Caidas")]
		Caidas,
        [Description("Accidentes operando maquinaria")]
        AccidentesOperandoMaquinaria,
        [Description("Accidemtes de demolicion")]
        AccidentesDeDemolicion,
        [Description("Accidentes por derrumbes de zanjas")]
        AccidentesPorDerrumbesDeZanjas,
        [Description("Incendios y exposiones")]
        IncendiosYExplosiones,
        [Description("Accidentes de vehiculos en obras")]
        AccidentesDeVehiculosEnObrasDeConstruccion,
        [Description("Exposicion a sustancias quimicas y toxinas")]
        ExposicionASustanciasQuimicasYToxinas,
        [Description("Gruas")]
        Gruas,
        [Description("Incidentes de atropello")]
        IncidentesDeAtropello,
        [Description("Electrocucion")]
        Electrocucion,
        [Description("Tropiezos y resbalones")]
        TropiezosYResbalones,
        [Description("Traumatismo craneoencefalico")]
        TraumatismoCraneoencefalico,
        [Description("Derrumbes del suelo")]
        DerrumbesDelSuelo,
        [Description("Lesion ocular")]
        LesionOcular,
        [Description("Lesion de la medula espinal")]
        LesionDeLaMedulaEspinal,
        [Description("Quemaduras")]
        Quemaduras
	}
}
