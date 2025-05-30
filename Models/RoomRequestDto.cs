using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
public class RoomRequestDto
{
    public required string FullName { get; set; }
    public required string cccd { get; set; }
    public required DateTime dob { get; set; }
    public required string Phone { get; set; }
    public bool hasInsurance { get; set; }
}

}