using Application.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace  API.Controllers;

        [ApiController]
        [Route("api/[controller]")]
        public class BackupController : ControllerBase
        {
            private readonly IBackupService _backupService;

            public BackupController(IBackupService backupService)
            {
                _backupService = backupService;
            }

            [HttpPost("export")]
            public async Task<IActionResult> ExportDataToTextFile([FromBody] int warehouseId)
            {
                try
                {
                    string fileName = $"warehouse_{warehouseId}_backup.txt";
                    await _backupService.ExportDataToTextFileAsync(fileName, warehouseId);
                    return Ok($"Backup file {fileName} created successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }
    
