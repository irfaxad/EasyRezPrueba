using EasyRez.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.IO;
using static System.Net.WebRequestMethods;

namespace EasyRez.Pages
{
    public class ValidacionArchivoModel : PageModel
    {
        private readonly ILogger<ValidacionArchivoModel> _logger;
        protected string status = "";
        protected string partialPath = "\\Mi unidad\\Proyectos\\EasyRez\\Test\\Ejercicio\\EasyRez\\wwwroot\\files";

        [BindProperty]
        public SubirArchivosModel SubirArchivo { get; set; }

        public ValidacionArchivoModel(ILogger<ValidacionArchivoModel> logger)
        {
            _logger = logger;
        }

        /*public ActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
        }*/

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid && SubirArchivo.Archivo == null)
            {                
                return Page();
            }

            try
            {
                // Carga del archivo
                string path = Path.Combine(Directory.GetCurrentDirectory(), partialPath, SubirArchivo.Archivo.FileName);
                string pathDestino = Path.Combine(Directory.GetCurrentDirectory(), partialPath, "Procesado.txt");
                string ext = Path.GetExtension(path);
                string status = "";
                if (System.IO.File.Exists(path))
                {
                    @TempData["Message"] = "ALERTA: Ya existe el archivo en el servidor.";                    
                } 
                else
                {
                    if(ext == ".txt")
                    {
                        using var stream = new FileStream(path, FileMode.Create);
                        await SubirArchivo.Archivo.CopyToAsync(stream);
                        @TempData["Message"] = "OK: Se cargó el archivo en el servidor.";
                        status = "OK: Se cargó el archivo en el servidor.";
                        stream.Close();
                        StreamReader sr = null;

                        try
                        {
                            sr = new StreamReader(path);
                            List<string> linesDestino = new List<string>();
                            string strHeader = "";
                            int lineCount = System.IO.File.ReadAllLines(path).Length;
                            int line = 0;
                            float percentage;
                            if(lineCount > 0)
                            {
                            }

                            string data = sr.ReadLine();
                            while (data != null)
                            {
                                line++;
                                string tmpCadena = "";
                                if(line == 1)
                                {
                                    tmpCadena = ConversionHeader(data, line, path);
                                    if(tmpCadena == " ")
                                    {
                                        sr.Close();
                                        @TempData["Message"] = "ERROR: El archivo tiene líneas con longitudes no permitidas para su procesamiento.";
                                        status = "ERROR: El archivo tiene líneas con longitudes no permitidas para su procesamiento.";
                                        addStatus(status, path, 0, "ERROR");
                                        break;
                                    }
                                    strHeader = tmpCadena;                                    
                                } 
                                else
                                {
                                    tmpCadena = ConversionDetalle(data, line - 1, path);
                                    if (tmpCadena == " ")
                                    {
                                        sr.Close();
                                        @TempData["Message"] = "ERROR: El archivo tiene líneas con longitudes no permitidas para su procesamiento.";
                                        status = "ERROR: El archivo tiene líneas con longitudes no permitidas para su procesamiento.";
                                        addStatus(status, path, 0, "ERROR");
                                        break;
                                    }
                                    linesDestino.Add(string.Concat(strHeader, "|", tmpCadena));
                                                                    

                                    // TODO: Enviar resultado status a Front
                                }

                                percentage = line / lineCount;

                                // TODO: Enviar percentaje a barra de progreso

                                data = sr.ReadLine();
                            }

                            if (!System.IO.File.Exists(pathDestino))
                            {
                                using (StreamWriter sw = System.IO.File.CreateText(pathDestino))
                                {
                                    foreach(string lineText in linesDestino)
                                    {
                                        sw.WriteLine(lineText);
                                    }
                                }
                            } 
                            else
                            {
                                using (StreamWriter sw = System.IO.File.AppendText(pathDestino))
                                {
                                    foreach (string lineText in linesDestino)
                                    {
                                        sw.WriteLine(lineText);
                                    }
                                }
                            }

                        }
                        catch(Exception ex)
                        {
                            @TempData["Message"] = "ERROR: Hubo un problema al procesar tu archivo. " + ex.Message;
                            status = "ERROR: Hubo un problema al procesar tu archivo.";
                            addStatus(status, path, 0, "ERROR");                            
                        }
                        finally
                        {
                            sr.Close();
                        }
                    }
                    else
                    {
                        @TempData["Message"] = "ERROR: La extensión del archivo no es válida. No se cargó en el servidor.";
                    }

                }
            } 
            catch (Exception ex)
            {
                @TempData["Message"] = "ERROR: Hubo un problema al cargar tu archivo. " + ex.Message;
                Console.WriteLine(ex.Message);
            }

            
            return Page();
        }

        public string ConversionHeader(string text, int num, string tPath)
        {
            string file = Path.GetFileName(tPath);
            
            if (text.Length != 153)
            {
                status = "ERROR: Se encontró una línea con longitud incorrecta. No se puede procesar.";
                addStatus(status, file, num, "ERROR");
                return " ";
            }

            status = "";

            string hTR  = text.Substring(0,1);
            string hCS  = text.Substring(1,2);
            string hE   = text.Substring(3,5);
            string hFP  = text.Substring(8, 8);
            string hC   = text.Substring(16, 2);
            string hNR  = text.Substring(18, 6);
            string hIR  = text.Substring(24, 15);
            string hNA  = text.Substring(39, 6);
            string hIA  = text.Substring(45, 15);
            string hNB  = text.Substring(60, 6);
            string hIB  = text.Substring(66, 15);
            string hNC  = text.Substring(81, 6);
            string hA   = text.Substring(87, 1);
            string hF   = text.Substring(88, 65);

            if(hTR != "H")
            {
                status += "Header:TipoDeRegistro - Se encontró letra diferente a H\n";
            }
            if(hCS != "CP")
            {
                status += "Header:ClaveDeServicio - Se encontraron letras diferentes a CP\n";
            }
            if(!(Int32.TryParse(hE,out int ihE) && ihE > 0))
            {
                status += "Header:Emisora - El valor no tiene un número válido\n";
            }
            if(!((Int32.TryParse(hFP.Substring(0,4),out int ihFPy) && Int32.TryParse(hFP.Substring(4,2),out int ihFPm) && Int32.TryParse(hFP.Substring(6,2),out int ihFPd)) && (((ihFPm == 1 || ihFPm == 3 || ihFPm == 5 || ihFPm == 7 || ihFPm == 8 || ihFPm == 10 || ihFPm == 12) && ihFPd <= 31)
                    || ((ihFPm == 4 || ihFPm == 6 || ihFPm == 9 || ihFPm == 11) && ihFPd <= 30)
                    || (ihFPm == 2 && ihFPd <= 29)) && (ihFPy >= 1900 && ihFPy <= 2100)))
            {
                status += "Header:FechaDeProceso - La fecha no es válida\n";
            }
            if(hC != "00")
            {
                status += "Header:Consecutivo - Se encontró valor diferente a 00\n";
            }
            if(!(Int32.TryParse(hNR,out int ihNR) && ihNR > 0))
            {
                status += "Header:NumeroTotalDeRegistrosEnviados - Se encontró valor no válido\n";
            }
            if(!(Int32.TryParse(hIR, out int ihIR) && ihIR > 0))
            {
                status += "Header:ImporteTotalDeRegistrosEnviados - Se encontró valor no válido\n";
            }
            if(!(Int32.TryParse(hNA, out int ihNA) && ihNA > 0))
            {
                status += "Header:NumeroTotalDeAltasEnviados - Se encontró valor no válido\n";
            }
            if(!(Int32.TryParse(hIA, out int ihIA) && ihIA > 0))
            {
                status += "Header:ImporteTotalDeAltasEnviados - Se encontró valor no válido\n";
            }
            if(hNB != "000000")
            {
                status += "Header:NumeroTotalDeBajasEnviados - Se encontró valor diferente de 0\n";
            }
            if(hIB != "000000000000000")
            {
                status += "Header:ImporteTotalDeBajasEnviados - Se encontró valor diferente de 0\n";
            }
            if(hNC != "000000")
            {
                status += "Header:NumeroTotalDeCuentasAVerificar - Se encontró valor diferente de 0\n";
            }
            if(hA != " ")
            {
                status += "Header:Accion - Se encontró valor diferente a espacio\n";
            }
            if(hF != "                                                                 ")
            {
                status += "Header:Filler - Se encontró valor diferente a relleno de espacios permitido\n";
            }

            if (status == "")
            {
                status = "OK: Se procesó el archivo correctamente.";
            }

            if (!status.Contains("OK"))
            {
                addStatus(status, file, 1, "WARNING");
            }

            string[] arrHeader = { hTR, hCS, hE, hFP, hC, hNR, hIR, hNA, hIA, hNB, hIB, hNC, hA, hF };

            return String.Join("|", arrHeader);
        }

        public string ConversionDetalle(string text, int num, string tPath)
        {
            string file = Path.GetFileName(tPath);

            if (text.Length != 153)
            {
                status = "ERROR: Se encontró una línea con longitud incorrecta. No se puede procesar.";
                addStatus(status, file, num, "ERROR");
                return " ";
            }

            string dTR = text.Substring(0, 1);
            string dFA = text.Substring(1, 8);
            string dNE = text.Substring(9, 10);
            string dRS = text.Substring(19, 40);
            string dRL = text.Substring(59, 40);
            string dI = text.Substring(99, 15);
            string dCB = text.Substring(114, 3);
            string dTC = text.Substring(117, 2);
            string dNC = text.Substring(119, 18);
            string dTM = text.Substring(137, 1);
            string dA = text.Substring(138, 1);
            string dF = text.Substring(139, 5);
            string dMD = text.Substring(144, 2);
            string dRN = text.Substring(146, 7);
            string aNA = Path.GetFileName(tPath);
            string aCF = num.ToString().PadLeft(3,'0');

            if(dTR != "D")
            {
                status += "Detalle:TipoDeRegistro - Se encontró letra diferente a D\n";
            }
            if (!((Int32.TryParse(dFA.Substring(0, 4), out int idFAy) && Int32.TryParse(dFA.Substring(4, 2), out int idFAm) && Int32.TryParse(dFA.Substring(6, 2), out int idFAd)) && (((idFAm == 1 || idFAm == 3 || idFAm == 5 || idFAm == 7 || idFAm == 8 || idFAm == 10 || idFAm == 12) && idFAd <= 31)
                    || ((idFAm == 4 || idFAm == 6 || idFAm == 9 || idFAm == 11) && idFAd <= 30)
                    || (idFAm == 2 && idFAd <= 29)) && (idFAy >= 1900 && idFAy <= 2100)))
            {
                status += "Detalle:FechaDeAplicación - La fecha no es válida\n";
            }
            if(dNE != "          ")
            {
                status += "Detalle:NumeroDeEmpleado - Se encontraron caracteres diferentes a espacios permitidos\n";
            }
            if(!(Int32.TryParse(dRS,out int idRS) && idRS > 0))
            {
                status += "Detalle:ReferenciaDelServicio - Se encontró referencia no numérica o datos inválidos\n";
            }
            if (!(Int32.TryParse(dI, out int idI) && idI > 0))
            {
                status += "Detalle:Importe - Se encontró cantidad no válida\n";
            }
            if (!(Int32.TryParse(dCB, out int idCB) && idCB > 0))
            {
                status += "Detalle:CodigoDeBanco - Se encontró valor no válido\n";
            }
            if(!(dTC == "01" || dTC == "03" || dTC == "40"))
            {
                status += "Detalle:TipoDeCuenta - Se encontró valor distinto a Cheques, Tarjeta de Débito y CLABE\n";
            }
            if (!(Int32.TryParse(dNC, out int idNC) && idNC > 0))
            {
                status += "Detalle:NumeroDeCuenta - Se encontró cuenta no numérica o en ceros\n";
            }
            if(dTM != "1")
            {
                status += "Detalle:TipoDeMovimiento - Se encontró valor distinto a Alta Factura\n";
            }
            if(dA != "A")
            {
                status += "Detalle:Accion - Se encontró valor distinto a Alta\n";
            }
            if(dF != "     ")
            {
                status += "Detalle:Filler - Se encontraron caracteres diferentes al espaciado permitido\n";
            }
            if (!(Int32.TryParse(dMD, out int idMD) && idMD > 0))
            {
                status += "Detalle:MotivoDeDevolucion - Se encontró valor no numérico o en ceros\n";
            }
            if(!(Int32.TryParse(dRN, out int idRN)))
            {
                status += "Detalle:ReferenciaNumérica - Se encontró valor no numérico\n";
            }

            if(status == "")
            {
                status = "OK: Se procesó el archivo correctamente.";
            }

            if (!status.Contains("OK"))
            {
                addStatus(status, aNA, num, "WARNING");
            }

            string[] arrHeader = { dTR, dFA, dNE, dRS, dRL, dI, dCB, dTC, dNC, dTM, dA, dF, dMD, dRN, aNA, aCF };

            return string.Concat(String.Join("|", arrHeader),"*");
        }

        public void addStatus(string status, string filename, int line, string risk)
        {
            Status oStatus = new Status();
            oStatus.Stats = status;
            oStatus.Line = line.ToString();
            oStatus.Filename = Path.GetFileName(filename); ;
            oStatus.Risk = risk;

            StatusDataAccessLayer statusDataAccessLayer = new StatusDataAccessLayer();
            statusDataAccessLayer.AddStatus(oStatus);
        }
    }

    public class SubirArchivosModel
    {
        public string Description { get; set; }

        [Required(ErrorMessage = "Debes elegir un archivo para poder enviarlo.")]        
        public IFormFile Archivo { get; set; }
        public int Progreso { get; set; }

    }
}