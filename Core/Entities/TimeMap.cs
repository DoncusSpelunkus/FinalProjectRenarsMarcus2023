using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class TimeMap
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TimeMapId { get; set; }
    
    public int EmployeeId { get; set; }

    public DateTime SignInTime { get; set; }
    public DateTime SignOutTime { get; set; }

    public double WorkedHours { get; set; }
    
    public  Employee Employee { get; set; }
}