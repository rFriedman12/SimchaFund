using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SimchaFundHW21.Data
{
    public class SimchaFundDatabaseManager
    {
        private string _connString;

        public SimchaFundDatabaseManager(string connString)
        {
            _connString = connString;
        }

        public List<Simcha> GetAllSimchas()
        {
            using var conn = new SqlConnection(_connString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Simchas s
                                LEFT JOIN Contributions c ON s.Id = c.SimchaId";
            conn.Open();

            var simchas = new List<Simcha>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int simchaId = (int)reader["Id"];
                Simcha simcha = simchas.FirstOrDefault(s => s.Id == simchaId);
                if (simcha == null)
                {
                    simcha = new Simcha
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Date = (DateTime)reader["Date"],
                        Contributions = new List<Contribution>()
                    };
                    simchas.Add(simcha);
                }

                int contributorId = reader.GetOrNull<int>("ContributorId");
                if (contributorId > 0)
                {
                    simcha.Contributions.Add(new Contribution
                    {
                        SimchaId = simcha.Id,
                        ContributorId = (int)reader["ContributorId"],
                        Amount = (decimal)reader["Amount"]
                    });
                }
            }
            return simchas;
        }

        public void AddSimcha(Simcha simcha)
        {
            using var conn = new SqlConnection(_connString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Simchas (Name, Date) VALUES (@name, @date)
                                SELECT SCOPE_IDENTITY()";            
            cmd.Parameters.AddWithValue("@name", simcha.Name);
            cmd.Parameters.AddWithValue("@date", simcha.Date);
            conn.Open();
            int simchaId = (int)(decimal)cmd.ExecuteScalar();
            simcha.Id = simchaId;
        }

        public List<Contributor> GetAllContributors()
        {
            using var conn = new SqlConnection(_connString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ctors.*, ctions.ContributorId, ctions.SimchaId, ctions.Amount AS ContributionAmount, 
                                d.Id AS DepositId, d.Amount AS DepositAmount, d.Date AS DepositDate, d.ContributorId 
                                FROM Contributors ctors
                                LEFT JOIN Contributions ctions ON ctors.Id = ctions.ContributorId
                                LEFT JOIN Deposits d ON d.contributorId = ctors.Id";
            conn.Open();

            var contributors = new List<Contributor>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int contributorId = (int)reader["Id"];
                Contributor contributor = contributors.FirstOrDefault(c => c.Id == contributorId);
                if (contributor == null)
                {
                    contributor = new Contributor
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        PhoneNumber = (string)reader["PhoneNumber"],
                        AlwaysInclude = (bool)reader["AlwaysInclude"],
                        Contributions = new List<Contribution>(),
                        Deposits = new List<Deposit>()
                    };
                }

                int simchaId = reader.GetOrNull<int>("SimchaId");
                if (simchaId > 0)
                {
                    contributor.Contributions.Add(new Contribution
                    {
                        ContributorId = contributor.Id,
                        SimchaId = simchaId,
                        Amount = (decimal)reader["ContributionAmount"]
                    });
                }

                int depositId = reader.GetOrNull<int>("DepositId");
                if (depositId > 0)
                {
                    contributor.Deposits.Add(new Deposit
                    {
                        ContributorId = contributor.Id,
                        Date = (DateTime)reader["DepositDate"],
                        Amount = (decimal)reader["DepositAmount"]
                    });
                }

                contributors.Add(contributor);
            }
            return contributors;
        }

        public int AddContributor(Contributor contributor)
        {
            using var conn = new SqlConnection(_connString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributors (FirstName, LastName, PhoneNumber, AlwaysInclude, DateCreated) 
                                VALUES (@firstName, @lastName, @phoneNumber, @alwaysInclude, @dateCreated) 
                                SELECT SCOPE_IDENTITY()";            
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@phoneNumber", contributor.PhoneNumber);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            cmd.Parameters.AddWithValue("@dateCreated", contributor.DateCreated);
            conn.Open();
            contributor.Id = (int)(decimal)cmd.ExecuteScalar();
            return contributor.Id;
        }

        public void AddDeposit(decimal amount, int contributorId)
        {
            using var conn = new SqlConnection(_connString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Deposits (Amount, Date, ContributorId) VALUES (@amount, @date, @contributorId)";
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@contributorId", contributorId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public int GetTotalContributors()
        {
            using var conn = new SqlConnection(_connString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT COUNT(*) FROM Contributors";
            conn.Open();
            return (int)cmd.ExecuteScalar();            
        }

        public Simcha GetSimchaById(int id)
        {
            using var conn = new SqlConnection(_connString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Name FROM Simchas WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            return new Simcha
            {
                Id = id,
                Name = (string)cmd.ExecuteScalar()
            };            
        }

        public List<int> GetContributorsForSimcha(int simchaId)
        {
            using var conn = new SqlConnection(_connString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ContributorId FROM Contributions WHERE SimchaId = @simchaId";
            cmd.Parameters.AddWithValue("simchaId", simchaId);
            conn.Open();

            List<int> contributors = new();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributors.Add((int)reader["ContributorId"]);
            }
            return contributors;
        }

        public void UpdateContributionsForSimcha(int simchaId, List<ContributorContribution> contributors)
        {
            using var conn = new SqlConnection(_connString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM Contributions WHERE SimchaId = @simchaId";
            conn.Open();
            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            cmd.ExecuteNonQuery();
            foreach(ContributorContribution contributor in contributors)
            {
                if (contributor.Include && contributor.Amount > 0)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = @"INSERT INTO Contributions (SimchaId, ContributorId, Amount) VALUES (@simchaId, @contributorId, @amount)";
                    cmd.Parameters.AddWithValue("@simchaId", simchaId);
                    cmd.Parameters.AddWithValue("@contributorId", contributor.ContributorId);
                    cmd.Parameters.AddWithValue("@amount", contributor.Amount);
                    cmd.ExecuteNonQuery();
                }
            }
            {//foreach (ContributorContribution contributor in contributors)
            //{
            //    if(!currentContributors.Contains(contributor.ContributorId))
            //    {
            //        if (contributor.Include)
            //        {
            //            cmd.Parameters.Clear();
            //            cmd.CommandText += @"INSERT INTO Contributions (SimchaId, ContributorId, Amount, Date) 
            //                             VALUES (@simchaId, @contributorId, @amount, @date)";
            //            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            //            cmd.Parameters.AddWithValue("@contributorId", contributor.ContributorId);
            //            cmd.Parameters.AddWithValue("@amount", contributor.Amount);
            //            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            //            cmd.ExecuteNonQuery();
            //        }                    
            //    }
            //    else
            //    {
            //        if (contributor.Include)
            //        {
            //            cmd.Parameters.Clear();
            //            cmd.CommandText += @"UPDATE Contributions SET Amount = @amount, Date = @date 
            //                                 WHERE ContributorId = @contributorId AND SimchaId = @simchaId";
            //            cmd.Parameters.AddWithValue("@amount", contributor.Amount);
            //            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            //            cmd.Parameters.AddWithValue("@contributorId", contributor.ContributorId);
            //            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            //            cmd.ExecuteNonQuery();
            //        }
            //        else
            //        {
            //            cmd.Parameters.Clear();
            //            cmd.CommandText += "DELETE FROM Contributions WHERE ContributorId = @contributorId AND SimchaId = @simchaId";
            //            cmd.Parameters.AddWithValue("contributorId", contributor.ContributorId);
            //            cmd.Parameters.AddWithValue("simchaId", simchaId);
            //            cmd.ExecuteNonQuery();
            //        }
            //    }}
            }                        
            
        }

        public decimal GetContribution(int contributorId, int simchaId)
        {
            using var conn = new SqlConnection(_connString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Amount FROM Contributions WHERE SimchaId = @simchaId AND ContributorId = @contributorId";
            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            cmd.Parameters.AddWithValue("@contributorId", contributorId);
            conn.Open();
            decimal contributionAmount = (decimal)cmd.ExecuteScalar();
            return contributionAmount;
        }
    }


    public static class Extensions
    {
        public static T GetOrNull<T>(this SqlDataReader reader, string column)
        {
            object value = reader[column];
            if (value == DBNull.Value)
            {
                return default(T);
            }

            return (T)value;
        }
    }
}
