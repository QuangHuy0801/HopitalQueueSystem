using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.Data.SqlClient;
using MyProject.Data;

namespace backend.Repositories
{
    public class ViewQueueRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ViewQueueRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Room>> GetRoom()
        {
            var rooms = new List<Room>();

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT Id, RoomName, Description, IsAvailable FROM v_Room";
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var room = new Room
                        {
                            Id = reader.GetInt32(0),
                            RoomName = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2)!,
                            IsAvailable = reader.GetInt32(3)
                        };
                        rooms.Add(room);
                    }
                }
            }

            return rooms;
        }
    }
}