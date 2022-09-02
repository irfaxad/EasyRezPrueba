using System.Data;
using System.Data.SqlClient;

namespace EasyRez.Models
{
    public class DatosFacturacionDataAccessLayer
    {
        // string connectionString = "Data Source=DESKTOP-4MPVROB\\SQLEXPRESS;Initial Catalog=EasyRez;User ID=EasyRez;Password=EasyRez2022;Application Name=EasyRez";

        string connectionString = "Data Source=(local);Initial Catalog=EasyRez;Integrated Security=true";

        public IEnumerable<DatosFacturacion> GetAllDatosFacturacion(int IdTipoEntidadTributaria)
        {
            List<DatosFacturacion> lstDatosFacturacion = new List<DatosFacturacion>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spObtenerDatosFacturacion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoEntidadTributaria", IdTipoEntidadTributaria);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    DatosFacturacion datosFacturacion = new DatosFacturacion();
                    datosFacturacion.IdDatoFacturacion = Convert.ToInt32(sdr["IdDatoFacturacion"]);
                    datosFacturacion.TipoPersona = Convert.ToInt32(sdr["IdTipoPersona"]);
                    datosFacturacion.RFC = sdr["RFC"].ToString();
                    datosFacturacion.RazonSocial = sdr["RazonSocial"].ToString();
                    datosFacturacion.MetodoPago = Convert.ToInt32(sdr["IdMetodoPago"]);
                    datosFacturacion.UsoCFDI = Convert.ToInt32(sdr["UsoCFDI"]);
                    datosFacturacion.RegimenFiscal = Convert.ToInt32(sdr["RegimenFiscal"]);
                    datosFacturacion.IdDireccion = Convert.ToInt32(sdr["IdDireccion"]);
                    datosFacturacion.IdEntidadTributaria = Convert.ToInt32(sdr["IdEntidadTributaria"]);

                    datosFacturacion.StrMetodoPago = sdr["MetodoPago"].ToString();
                    datosFacturacion.StrUsoCFDIClave = sdr["UsoCFDIClave"].ToString();
                    datosFacturacion.StrUsoCFDIDescripcion = sdr["UsoCFDIDescripcion"].ToString();
                    datosFacturacion.StrTipoPersona = sdr["TipoPersona"].ToString();
                    datosFacturacion.StrRegimenFiscalClave = sdr["RegimenFiscal_Clave"].ToString();
                    datosFacturacion.StrRegimenFiscalDescripcion = sdr["RegimenFiscal_Descripción"].ToString();
                    datosFacturacion.Correo = sdr["Correo"].ToString();
                    lstDatosFacturacion.Add(datosFacturacion);
                }
                con.Close();
            }
            return lstDatosFacturacion;
        }

        public void AgregarEditarDatosFacturacion(DatosFacturacion datosFacturacion)
        {
            string txtRazonSocial = "";
            string nombres = datosFacturacion.Nombres ?? "";
            string apPaterno = datosFacturacion.ApPaterno ?? "";
            string apMaterno = datosFacturacion.ApMaterno ?? "";
            string razonSocial = datosFacturacion.RazonSocial ?? "";

            if(datosFacturacion.TipoPersona == 119)
            {
                txtRazonSocial = nombres + " " + apPaterno + " " + apMaterno;
            }
            else if(datosFacturacion.TipoPersona == 120)
            {
                txtRazonSocial = razonSocial;
            }
            
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spAgregarEditarDatosFacturacion", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdDatoFacturacion", datosFacturacion.IdDatoFacturacion);
                cmd.Parameters.AddWithValue("@TipoPersona", datosFacturacion.TipoPersona);
                cmd.Parameters.AddWithValue("@RFC", datosFacturacion.RFC);
                cmd.Parameters.AddWithValue("@RazonSocial", txtRazonSocial);
                cmd.Parameters.AddWithValue("@UsoCFDI", datosFacturacion.UsoCFDI);
                cmd.Parameters.AddWithValue("@RegimenFiscal", datosFacturacion.RegimenFiscal);
                cmd.Parameters.AddWithValue("@EsSucursal", datosFacturacion.EsSucursal);
                cmd.Parameters.AddWithValue("@TipoEntidadTributaria", datosFacturacion.TipoEntidadTributaria);
                cmd.Parameters.AddWithValue("@Correo", datosFacturacion.Correo);
                //cmd.Parameters.AddWithValue("@MetodoPago", datosFacturacion.MetodoPago);
                //cmd.Parameters.AddWithValue("@IdDireccion", datosFacturacion.IdDireccion);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public DatosFacturacion ObtenerDatosFacturacionData(int? IdDatoFacturacion)
        {
            DatosFacturacion datosFacturacion = new DatosFacturacion();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spObtenerDatosFacturacion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdDatoFacturacion", IdDatoFacturacion);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {
                    datosFacturacion.IdDatoFacturacion = Convert.ToInt32(sdr["IdDatoFacturacion"]);
                    datosFacturacion.TipoPersona = Convert.ToInt32(sdr["IdTipoPersona"]);
                    datosFacturacion.RFC = sdr["RFC"].ToString();
                    datosFacturacion.RazonSocial = sdr["RazonSocial"].ToString();
                    datosFacturacion.MetodoPago = Convert.ToInt32(sdr["IdMetodoPago"]);
                    datosFacturacion.UsoCFDI = Convert.ToInt32(sdr["UsoCFDI"]);
                    datosFacturacion.RegimenFiscal = Convert.ToInt32(sdr["RegimenFiscal"]);
                    datosFacturacion.IdDireccion = Convert.ToInt32(sdr["IdDireccion"]);
                    datosFacturacion.EsSucursal = Convert.ToBoolean(sdr["EsSucursal"]);
                    datosFacturacion.IdEntidadTributaria = Convert.ToInt32(sdr["IdEntidadTributaria"]);

                    datosFacturacion.StrMetodoPago = sdr["MetodoPago"].ToString();
                    datosFacturacion.StrUsoCFDIClave = sdr["UsoCFDIClave"].ToString();
                    datosFacturacion.StrUsoCFDIDescripcion = sdr["UsoCFDIDescripcion"].ToString();
                    datosFacturacion.StrTipoPersona = sdr["TipoPersona"].ToString();
                    datosFacturacion.StrRegimenFiscalClave = sdr["RegimenFiscal_Clave"].ToString();
                    datosFacturacion.StrRegimenFiscalDescripcion = sdr["RegimenFiscal_Descripción"].ToString();
                    datosFacturacion.Correo = sdr["Correo"].ToString();
                    datosFacturacion.TipoEntidadTributaria = Convert.ToInt32(sdr["TipoEntidadTributaria"]);
                    //datosFacturacion.
                }
            }
            return datosFacturacion;
        }

    }
}
