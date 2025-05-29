using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.Data.SqlClient;
using MyProject.Data;

namespace backend.Repositories
{
    public class PatientQueueRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public PatientQueueRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(int newQueueNumber, int patientId,int roomId)> TakeQueueNumberAsync(PatientQueueRequestDto dto)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("TakeQueueNumber", conn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@FullName", dto.FullName);
            cmd.Parameters.AddWithValue("@IsHealthInsurance", dto.hasInsurance);
            cmd.Parameters.AddWithValue("@DOB", dto.dob);
            cmd.Parameters.AddWithValue("@Phone", dto.Phone);
            cmd.Parameters.AddWithValue("@CCCD", dto.cccd);

            // Output parameters
            var newQueueNumberParam = new SqlParameter("@NewQueueNumber", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            cmd.Parameters.Add(newQueueNumberParam);

            var patientIdParam = new SqlParameter("@PatientId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
             cmd.Parameters.Add(patientIdParam);
            var roomIdParam = new SqlParameter("@RoomId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            cmd.Parameters.Add(roomIdParam);

            await cmd.ExecuteNonQueryAsync();

            // Get output values
            int newQueueNumber = (int)newQueueNumberParam.Value;
            int patientId = (int)patientIdParam.Value;
           int roomId = roomIdParam.Value != DBNull.Value ? (int)roomIdParam.Value : -1;

            return (newQueueNumber, patientId,roomId);
        }
    }
}