using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using MyProject.Data;

namespace backend.Repositories
{
    public class ViewQueueRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ViewQueueRepository(SqlConnectionFactory connectionFactory, IHubContext<NotificationHub> hubContext)
        {
            _connectionFactory = connectionFactory;
            _hubContext = hubContext;
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

        public async Task<Room?> GetRoomById(int id)
        {
            Room? room = null;

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT Id, RoomName, Description, IsAvailable FROM v_Room WHERE Id = @id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            room = new Room
                            {
                                Id = reader.GetInt32(0),
                                RoomName = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2)!,
                                IsAvailable = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }
            return room;
        }
        public async Task<bool> UpdateNextPatientInQueueAsync(int roomId)
        {
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                await conn.OpenAsync();

                using var transaction = conn.BeginTransaction();

                using var cmd = new SqlCommand("UpdateNextPatientInQueue", conn, transaction)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@RoomId", roomId);

                var result = await cmd.ExecuteScalarAsync();

                if (result != null && Convert.ToInt32(result) == 1)
                {
                    var queue = new List<PatientQueue>();

                    // Thay vì lấy tất cả, gọi stored procedure lấy 1 In_process + 4 Waiting nhỏ nhất
                    using var getCmd = new SqlCommand("GetPatientQueueForRoom", conn, transaction)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };
                    getCmd.Parameters.AddWithValue("@RoomId", roomId);

                    using var reader = await getCmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var item = new PatientQueue
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PatientId = reader.IsDBNull(reader.GetOrdinal("PatientId")) ? 0 : reader.GetInt32(reader.GetOrdinal("PatientId")),
                            RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
                            QueueNumber = reader.GetInt32(reader.GetOrdinal("QueueNumber")),
                            PriorityLevel = reader.IsDBNull(reader.GetOrdinal("PriorityLevel")) ? 0 : reader.GetInt32(reader.GetOrdinal("PriorityLevel")),
                            Status = reader.GetString(reader.GetOrdinal("Status")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                        };
                        queue.Add(item);
                    }

                    reader.Close();
                    transaction.Commit();

                    // Gửi cập nhật hàng chờ qua SignalR cho tất cả client
                    await _hubContext.Clients.All.SendAsync("ReceivePatientQueueUpdate", queue);

                    return true;
                }


                transaction.Rollback();
                return false;
            }
            catch (SqlException ex)
            {
                // rollback nếu exception
                if (ex.Message.Contains("Không tìm thấy bệnh nhân phù hợp"))
                    return false;
                throw;
            }
        }
        public async Task<List<PatientQueue>> GetQueueByIdRoom(int roomId)
        {
            var queue = new List<PatientQueue>();

            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = new SqlCommand("GetPatientQueueForRoom", conn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@RoomId", roomId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var item = new PatientQueue
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    PatientId = reader.IsDBNull(reader.GetOrdinal("PatientId")) ? 0 : reader.GetInt32(reader.GetOrdinal("PatientId")),
                    RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
                    QueueNumber = reader.GetInt32(reader.GetOrdinal("QueueNumber")),
                    PriorityLevel = reader.IsDBNull(reader.GetOrdinal("PriorityLevel")) ? 0 : reader.GetInt32(reader.GetOrdinal("PriorityLevel")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
                queue.Add(item);
            }

            return queue;
        }


    }
}