using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Log
{
    //  todo : we need three methods one goes for the api ( fetch logs ) , and two for the application layer delete logs after 1 year , create logs 
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // If you want the ID to be auto-generated
    public int LogId { get; set; }
    public string ProductSKU { get; set; }
    public string FromLocationId { get; set; }
    public string ToLocationId { get; set; }
    public int Quantity { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; }
    public DateTime Timestamp { get; set; }
    public int WarehouseId { get; set; }
    public Employee Employee { get; set; }
    public Product Product { get; set; }
}