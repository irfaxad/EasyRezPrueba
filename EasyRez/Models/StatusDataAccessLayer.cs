using System.Data;
using System.Data.SqlClient;

namespace EasyRez.Models
{
    public class StatusDataAccessLayer
    {
        // string connectionString = "Data Source=DESKTOP-4MPVROB\\SQLEXPRESS;Initial Catalog=EasyRez;User ID=EasyRez;Password=EasyRez2022;Application Name=EasyRez";

        string connectionString = "Data Source=(local);Initial Catalog=EasyRez;Integrated Security=true";

        public IEnumerable<Status> GetAllStatus()
        {            
            List<Status> lstStatus= new List<Status>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Uploadlog ORDER BY dateLoad DESC";
                
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {
                    Status status = new Status();
                    status.Stats = sdr["status"].ToString();
                    status.Line = sdr["line"].ToString();
                    status.Filename = sdr["filename"].ToString();
                    status.Risk = sdr["risk"].ToString();
                    status.DateLoad = sdr["dateLoad"].ToString();
                    lstStatus.Add(status);
                }
                con.Close();
            }
            return lstStatus;
        }

        public void AddStatus(Status status)
        {
            string sql = "INSERT INTO Uploadlog (line, status, filename, risk) VALUES (@line, @status, @filename, @risk)";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@line", status.Line);
                cmd.Parameters.AddWithValue("@status", status.Stats);
                cmd.Parameters.AddWithValue("@filename", status.Filename);
                cmd.Parameters.AddWithValue("@risk", status.Risk);                

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}
