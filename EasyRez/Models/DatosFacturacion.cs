using ExpressiveAnnotations.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace EasyRez.Models
{
    public class DatosFacturacion
    {
        public int IdDatoFacturacion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Se debe indicar el Tipo de Persona.")]
        public int TipoPersona { get; set; }

        [Required(ErrorMessage = "El RFC es obligatorio.")]
        [AssertThat("validRFCNull()", ErrorMessage = "RFC no válido: faltan datos de captura para calcular el RFC.")]
        [AssertThat("validRFCLength()", ErrorMessage = "RFC no válido: la longitud del RFC es incorrecta.")]
        [AssertThat("validRFCDate()", ErrorMessage = "RFC no válido: la fecha en el RFC no es correcta.")]
        [AssertThat("validRFC()", ErrorMessage = "RFC no válido: las iniciales del RFC no coinciden con el nombre o razón social.")]
        public string? RFC { get; set; }

        [RequiredIf("TipoPersona == 120", ErrorMessage = "La Razón Social es obligatoria para Personas Morales.")]
        [StringLength(250, ErrorMessage = "La Razón Social no puede tener más de 250 caracteres.")]
        public string? RazonSocial { get; set; }

        [RequiredIf("TipoPersona == 119", ErrorMessage = "El Nombre es obligatorio para Personas Físicas.")]
        public string? Nombres { get; set; }

        [RequiredIf("TipoPersona == 119 && ApMaterno == null", ErrorMessage = "Al menos uno de los apellidos es obligatorio para Personas Físicas.")]
        public string? ApPaterno { get; set; }

        [RequiredIf("TipoPersona == 119 && ApPaterno == null", ErrorMessage = "Al menos uno de los apellidos es obligatorio para Personas Físicas.")]
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

        public bool validRFC()
        {
            string vNombres = this.Nombres;
            string vApPaterno = this.ApPaterno;
            string vApMaterno = this.ApMaterno;
            string vRazonSocial = this.RazonSocial;
            string vRFC = this.RFC;
            int vTipoPersona = this.TipoPersona;

            List<string> RFCgen = new List<string>();
            string RFCcomp = "";
            string[] arrNombres = { "" };
            string[] arrApPaterno = { "" };
            string[] arrApMaterno = { "" };

            // Conversión a mayúsculas, sin espacios ni caracteres especiales.
            vRFC = PurgeSpaces(PurgeText(vRFC));

            // Si es Persona Física
            if (vTipoPersona == 119 && (vNombres != null && (vApPaterno != null || vApMaterno != null)))
            {
                // Se depura texto de espacios y caracteres especiales, se convierte a mayúsculas, se quitan acentos, se transformó en array para identificar cada palabra, se eliminan conjunciones, artículos, preposiciones y se detectan nombres comunes José o María para su eliminación, excepto que sean nombres únicos o sólo existan ambos y ningún otro nombre.
                if (vNombres != null)
                {
                    arrNombres = DeleteCommonNames(DeleteConjunctions(TextToArray(PurgeText(vNombres))));
                }
                if(vApPaterno != null)
                {
                    arrApPaterno = DeleteCommonNames(DeleteConjunctions(TextToArray(PurgeText(vApPaterno))));
                }
                if(vApMaterno != null)
                {
                    arrApMaterno = DeleteCommonNames(DeleteConjunctions(TextToArray(PurgeText(vApMaterno))));
                }

                // Regla PF: Apellido Paterno tiene 2 letras o menos
                if (vApMaterno != null && arrApPaterno[0].Length <= 2)
                {
                    RFCgen.Add(GetLeftChars(arrApPaterno[0], 1));
                    RFCgen.Add(GetLeftChars(arrApMaterno[0], 1));
                    RFCgen.Add(GetLeftChars(arrNombres[0], 2));
                }
                // Regla PF: No hay Apellido Paterno
                else if (vApPaterno == null)
                {
                    RFCgen.Add(GetLeftChars(arrApMaterno[0], 1));
                    RFCgen.Add(FindFirstVowel(arrApMaterno[0]));
                    RFCgen.Add(GetLeftChars(arrNombres[0], 2));
                }
                // Regla PF: No hay Apellido Materno
                else if (vApMaterno == null)
                {
                    RFCgen.Add(GetLeftChars(arrApPaterno[0], 1));
                    RFCgen.Add(FindFirstVowel(arrApPaterno[0]));
                    RFCgen.Add(GetLeftChars(arrNombres[0], 2));
                }
                // Regla PF: No hay más vocales en Apellido Paterno
                else if (vApMaterno != null && FindFirstVowel(arrApPaterno[0]) == " ")
                {
                    RFCgen.Add(GetLeftChars(arrApPaterno[0], 2));
                    RFCgen.Add(GetLeftChars(arrApMaterno[0], 1));
                    RFCgen.Add(GetLeftChars(arrNombres[0], 1));
                }
                // No cayó en condiciones especiales, se conforma de manera normal
                else
                {
                    RFCgen.Add(GetLeftChars(arrApPaterno[0], 1));
                    RFCgen.Add(FindFirstVowel(arrApPaterno[0]));
                    RFCgen.Add(GetLeftChars(arrApMaterno[0], 1));
                    RFCgen.Add(GetLeftChars(arrNombres[0], 1));
                }

                // Se unen letras para generar RFC comparativo
                RFCcomp = string.Join("", RFCgen);
                // Regla PF: Si RFC genera palabra altisonante, se cambia última letra por X
                RFCcomp = DetectNastyWords(RFCcomp);
                // Se comparan 4 primeros caracteres de RFC capturado con RFC generado y se valida fecha en RFC
                if (RFCcomp.Equals(GetLeftChars(vRFC, 4)))
                {
                    return true;
                }
            }
            // Si es Persona Moral
            else if (vTipoPersona == 120 && vRazonSocial != null)
            {
                // Se depura texto de espacios y caracteres especiales (excepto aquellos escritos de manera independiente), se convierte a mayúsculas, se quitan acentos, se transformó en array para identificar cada palabra, se eliminan conjunciones, artículos, preposiciones, palabras y abreviaturas no permitidas.
                string[] arrRazonSocial = (DeleteSocietiesAndConjunctions(TextToArray(PurgeBusiness(vRazonSocial))));

                // Regla PM: Si la Razón Social sólo tiene 2 palabras
                if (arrRazonSocial.Length == 2)
                {
                    RFCgen.Add(GetLeftChars(arrRazonSocial[0], 1));
                    RFCgen.Add(GetLeftChars(arrRazonSocial[1], 2));
                }
                // Regla PM: Si la Razón Social sólo tiene 1 palabra con 2 letras
                else if (arrRazonSocial.Length == 1 && arrRazonSocial[0].Length == 2)
                {
                    RFCgen.Add(GetLeftChars(arrRazonSocial[0], 2));
                    RFCgen.Add("X");
                }
                // Regla PM: Si la Razón Social sólo tiene 1 palabra con 1 letra
                else if (arrRazonSocial.Length == 1 && arrRazonSocial[0].Length == 1)
                {
                    RFCgen.Add(GetLeftChars(arrRazonSocial[0], 1));
                    RFCgen.Add("X");
                    RFCgen.Add("X");
                }
                // Regla PM: Si la Razón Social sólo tiene 1 palabra pero hay suficientes letras
                else if (arrRazonSocial.Length == 1)
                {
                    RFCgen.Add(GetLeftChars(arrRazonSocial[0], 3));
                }
                // No cayó en condiciones especiales, se conforma de manera normal
                else
                {
                    RFCgen.Add(GetLeftChars(arrRazonSocial[0], 1));
                    RFCgen.Add(GetLeftChars(arrRazonSocial[1], 1));
                    RFCgen.Add(GetLeftChars(arrRazonSocial[2], 1));
                }

                // Se unen letras para generar RFC comparativo
                RFCcomp = string.Join("", RFCgen);
                // Regla PF: Si RFC genera palabra altisonante, se cambia última letra por X
                RFCcomp = DetectNastyWords(RFCcomp);
                // Se comparan 3 primeros caracteres de RFC capturado con RFC generado y se valida fecha en RFC
                if (RFCcomp.Equals(GetLeftChars(vRFC, 3)))
                {
                    return true;
                }

            }

            return false;
        }

        public bool validName()
        {
            string vNombres = this.Nombres;
            int vTipoPersona = this.TipoPersona;

            return vTipoPersona == 119 && vNombres != null;
        }
        public bool validLastname()
        {
            string vApPaterno = this.ApPaterno;
            string vApMaterno = this.ApMaterno;
            int vTipoPersona = this.TipoPersona;

            return vTipoPersona == 119 && (vApPaterno != null || vApMaterno != null);
        }

        public bool validBusinessName()
        {
            string vRazonSocial = this.RazonSocial;
            int vTipoPersona = this.TipoPersona;

            return vTipoPersona == 120 && vRazonSocial != null;
        }

        public bool validRFCLength()
        {
            string vRFC = this.RFC;
            int vTipoPersona = this.TipoPersona;
            vRFC = PurgeSpaces(PurgeText(vRFC));

            return (vTipoPersona == 119 && vRFC.Length == 13) || (vTipoPersona == 120 && vRFC.Length == 12);
        }

        public bool validRFCDate()
        {
            string vRFC = this.RFC;
            int vTipoPersona = this.TipoPersona;

            vRFC = PurgeSpaces(PurgeText(vRFC));

            return IsValidDate(vRFC, vTipoPersona);
        }

        public bool validRFCNull()
        {
            string vNombres = this.Nombres;
            string vApPaterno = this.ApPaterno;
            string vApMaterno = this.ApMaterno;
            string vRazonSocial = this.RazonSocial;
            string vRFC = this.RFC;
            int vTipoPersona = this.TipoPersona;

            if(vTipoPersona == 119 && (vNombres != null && (vApPaterno != null || vApMaterno != null)) && vRFC != null)
            {
                return true;
            }
            else if(vTipoPersona == 120 && vRazonSocial != null && vRFC != null)
            {
                return true;
            }

            return false;
        }

        // Función para convertir texto a arreglo para identificar palabras
        static string[] TextToArray(string text)
        {
            string[] resp = text.Split(' ');
            return resp;
        }

        // Función para convertir texto a mayúsculas, limpiar espacios al inicio y final y reemplazar acentos y caracteres no necesarios.
        static string PurgeText(string text)
        {
            return text.ToUpper().Trim().Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U").Replace("Ñ", "X").Replace("'", "").Replace(".", "").Replace("-"," ");
        }

        static string PurgeSpaces(string text)
        {
            return text.Replace(" ", "").Trim();
        }

        // Función para convertir texto a mayúsculas, limpiar espacios al inicio y final y reemplazar acentos y caracteres no necesarios.
        static string PurgeBusiness(string text)
        {
            string[] societies = { " DE CV", " DE C V", " CV", " C V", " MI\r\n", " S EN NC\r\n", " S EN C POR A\r\n", " S DE RL\r\n", " S EN C\r\n", " SA\r\n", " SNC\r\n", " A EN P\r\n", " SCL\r\n", " SCS\r\n", " SAPI\r\n", " S A\r\n", "S DE R L\r\n", " DE R L\r\n", " SAS\r\n", " S A S\r\n", " A C\r\n", " AC\r\n", " A R\r\n", " AR\r\n" };

            text = text.ToUpper().Trim().Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U").Replace("Ñ", "X").Replace("'", "").Replace(",", "").Replace(".", "");

            foreach (string society in societies)
            {
                text = text.Replace(society, "\r\n");
            }

            text = text.Replace("@ ", "ARROBA ").Replace(" % ", " PORCENTAJE ").Replace(" / ", " DIAGONAL ").Replace(" ( ", " PARENTESIS ").Replace(" ) ", " PARENTESIS ").Replace(" # ", " NUMERO ").Replace(" ! ", " ADMIRACION ").Replace(" $ ", " PESOS ").Replace(" \" ", " COMILLAS ").Replace(" - ", " GUION ").Replace(" + ", " SUMA ");

            text = text.Replace("@", "").Replace("%", "").Replace("/", "").Replace("(", "").Replace(")", "").Replace("#", "").Replace("!", "").Replace("$", "").Replace("\"", "").Replace("-", "").Replace("+", "");

            return text.Trim();
        }

        // Función para encontrar primera vocal a partir de la segunda letra.
        static string FindFirstVowel(string text)
        {
            char[] vowels = { 'A', 'E', 'I', 'O', 'U' };
            text = text.Remove(0, 1);
            int pos = text.IndexOfAny(vowels);
            if (pos == -1)
            {
                return " ";
            }
            string vowel = text[pos].ToString();
            return vowel;
        }

        // Función para identificar conjunciones y eliminarlas del arreglo de texto.
        static string[] DeleteConjunctions(string[] text)
        {
            string[] conjunctions = { "DA", "DAS", "DE", "DEL", "DER", "DI", "DIE", "EL", "LA", "LOS", "LAS", "LE", "LES", "MAC", "MC", "VAN", "VON", "Y" };

            foreach (string conjunction in conjunctions)
            {
                text = text.Where(e => e != conjunction).ToArray();
            }

            return text;
        }

        static string[] DeleteSocietiesAndConjunctions(string[] text)
        {
            string[] conjunctions = { "DA", "DAS", "DE", "DEL", "DER", "DI", "DIE", "EL", "LA", "LOS", "LAS", "LE", "LES", "MAC", "MC", "VAN", "VON", "Y", "COMPAXIA", "COMPA&IA", "COMPANIA", "COOPERATIVA", "CIA", "SOCIEDAD", "SOC", "COOP", "PARA", "POR", "AL", "E", "OF", "COMPANY", "MI", "EN", "CON", "SUS", "THE", "AND", "CO", "A", "SA", "SRL", "CV", "AC", "AR", "SRL", "NC", "CS" };

            foreach (string conjunction in conjunctions)
            {
                text = text.Where(e => e != conjunction).ToArray();
            }

            return text;
        }


        // Función para extraer caracteres a la izquierda del texto.
        static string GetLeftChars(string text, int count)
        {
            return text.Substring(0, count);
        }


        // Función para corregir RFC con palabras altisonantes.
        static string DetectNastyWords(string text)
        {
            string[] nastyWords = { "BUEI", "BUEY", "CACA", "CACO", "CAGA", "CAGO", "CAKA", "CAKO", "COGE", "COJA", "COJE", "COJI", "COJO", "CULO", "FETO", "GUEY", "JOTO", "KACA", "KACO", "KAGA", "KAGO", "KOGE", "KOJO", "KAKA", "KULO", "MAME", "MAMO", "MEAR", "MEAS", "MEON", "MION", "MOCO", "MULA", "PEDA", "PEDO", "PENE", "PUTA", "PUTO", "QULO", "RATA", "RUIN" };

            foreach (string word in nastyWords)
            {
                if (text == word)
                {
                    text = text.Substring(0, text.Length - 1);
                    text += "X";
                    break;
                }
            }

            return text;
        }

        // Función para eliminar nombres comunes José y María, excepto que éstos sean nombres únicos o sólo estén éstos y ningún otro.
        static string[] DeleteCommonNames(string[] text)
        {
            string[] commonNames = { "JOSE", "MARIA", "J", "M", "MA" };
            int countCommonNames = 0;

            if (text.Length == 1)
            {
                return text;
            }

            foreach (string name in text)
            {
                if (name == "JOSE" || name == "MARIA")
                {
                    countCommonNames++;
                }
            }

            if (text.Length == 2 && countCommonNames == 2)
            {
                return text;
            }

            foreach (string name in commonNames)
            {
                text = text.Where(e => e != name).ToArray();
            }

            return text;
        }

        // Función para validar la fecha dentro del RFC
        static bool IsValidDate(string text, int person)
        {
            string year = "";
            string month = "";
            string day = "";

            if (person == 119)
            {
                year = text.Substring(4, 2);
                month = text.Substring(6, 2);
                day = text.Substring(8, 2);
            }
            else if (person == 120)
            {
                year = text.Substring(3, 2);
                month = text.Substring(5, 2);
                day = text.Substring(7, 2);
            }

            if (Int32.TryParse(year, out int iY) && Int32.TryParse(month, out int iM) && Int32.TryParse(day, out int iD))
            {
                // Según el mes, la cantidad de días permitidos varía.
                if (((iM == 1 || iM == 3 || iM == 5 || iM == 7 || iM == 8 || iM == 10 || iM == 12) && iD <= 31)
                    || ((iM == 4 || iM == 6 || iM == 9 || iM == 11) && iD <= 30)
                    || (iM == 2 && iD <= 29))
                {
                    return true;
                }
            }

            // Si la conversión de string a números falló o los números están fuera de rango, devuelve falso.
            return false;
        }

    }    
}
