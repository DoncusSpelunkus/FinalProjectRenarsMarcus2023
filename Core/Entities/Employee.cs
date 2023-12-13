using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // If you want the ID to be auto-generated
    public int EmployeeId { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    
    public Warehouse Warehouse { get; set; }
    public int WarehouseId { get; set; }
    public IEnumerable<MoveLog> MoveLogs { get; set; }

    public IEnumerable<AdminLog> AdminLogs { get; set; }
}