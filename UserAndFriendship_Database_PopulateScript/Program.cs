using Microsoft.Data.Sqlite;
using System;
using System.Text;


namespace UserAndFriendship_Database_PopulateScript
{
    class Program
    {
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }



        static void Main(string[] args)
        {
            Program program = new Program();

            SqliteConnection sqlite = new SqliteConnection("Data Source=D:/movie-recommendation/project/movierecommendationapp.db");
            Console.WriteLine(sqlite.DataSource);
            sqlite.Open();

            var cmd = sqlite.CreateCommand();
            cmd.CommandText = "begin";
            cmd.ExecuteNonQuery();

            for (int i = 1; i<= 138493; i++)
            {

                cmd.CommandText = "INSERT INTO Users(Id,Username,Password,DateCreated) " +
                "VALUES(@Id,@Username,@Password,@DateCreated)";
                cmd.Parameters.AddWithValue("@Id", i);
                cmd.Parameters.AddWithValue("@Username", i.ToString() + Faker.Internet.UserName());
                cmd.Parameters.AddWithValue("@Password", program.CreatePassword(8));
                cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);

                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                
                Console.WriteLine("Row inserted:" + i);
            }

            cmd.CommandText = "end";
            cmd.ExecuteNonQuery();
        }
    }
}
