using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EasyRez.Models
{
    public class DatosFacturacion
    {
        public int IdDatoFacturacion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Se debe indicar el Tipo de Persona.")]
        public int TipoPersona { get; set; }

        [ValidaRFCAttribute(Nombres, ApPaterno, ApMaterno, RazonSocial, ErrorMessage = "RFC no válido.")]
        [Required(ErrorMessage = "El RFC es obligatorio.")]
        public string? RFC { get; set; }

        [StringLength(250, ErrorMessage = "La Razón Social no puede tener más de 250 caracteres.")]
        [Required(ErrorMessage = "La Razón Social es obligatoria.")]
        public string? RazonSocial { get; set; }

        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        
        public string? Nombres { get; set; }

        public string? ApPaterno { get; set; }

        public string? ApMaterno { get; set; }

        [EmailAddress(ErrorMessage = "Se debe ingresar un correo válido.")]
        [Required(ErrorMessage = "Se debe capturar un correo.")]
        public string? Correo { get; set; }
        public int MetodoPago { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Se debe seleccionar una opción.")]
        public int UsoCFDI { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Se debe seleccionar una opción.")]
        public int RegimenFiscal { get; set; }
        public int IdDireccion { get; set; }
        public bool EsSucursal { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Se debe seleccionar una opción.")]
        public int TipoEntidadTributaria { get; set; }
        public int IdEntidadTributaria { get; set; }

        [Display(Name = "Metodo Pago")]
        public string? StrMetodoPago { get; set; }

        [Display(Name = "Uso CFDI Clave")]
        public string? StrUsoCFDIClave { get; set; }

        [Display(Name = "Uso CFDI Descripcion")]
        public string? StrUsoCFDIDescripcion { get; set; }

        [Display(Name = "Tipo Persona")]
        public string? StrTipoPersona { get; set; }

        [Display(Name = "Regimen Fiscal Clave")]
        public string? StrRegimenFiscalClave { get; set; }

        [Display(Name = "Regimen Fiscal Descripción")]
        public string? StrRegimenFiscalDescripcion { get; set; }

        public DatosFacturacion()
        {
            IdDatoFacturacion = 0;
            StrMetodoPago = String.Empty;
            StrUsoCFDIClave = String.Empty;
            StrUsoCFDIDescripcion = String.Empty;
            StrTipoPersona = String.Empty;
            StrRegimenFiscalClave = String.Empty;
            StrRegimenFiscalDescripcion = String.Empty;
        }

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ValidaRFCAttribute : ValidationAttribute
    {
        public String Nombre { get; set; }
        public String ApPaterno { get; set; }
        public String ApMaterno { get; set; }
        public String RazonSocial { get; set; }

        public ValidaRFCAttribute(String nombre, String apPaterno, String apMaterno, String razonSocial)
        {
            Nombre = nombre;
            ApPaterno = apPaterno;
            ApMaterno = apMaterno;
            RazonSocial = razonSocial;
        }

        public override bool IsValid(object value)
        {
            Type objectType = value.GetType();

            PropertyInfo[] RFCfields = objectType.GetProperties().Where(propertyInfo => propertyInfo.Name == Nombre || propertyInfo.Name == ApPaterno || propertyInfo.Name == ApMaterno || propertyInfo.Name == RazonSocial).ToArray();

            if (RFCfields.Count() < 1)
            {
                throw new ApplicationException("ValidaRFCAttribute error on " + objectType.Name);
            }

            Boolean isValid = true;

            if (!Convert.ToString(RFCfields[0].GetValue(value, null)).Equals("PEPE"))
            {
                isValid = false;
            }

            return base.IsValid(value);
        }
    }
}
