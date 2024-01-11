namespace Application.IServices;

public interface IBackupService
{
     Task ExportDataToTextFileAsync(string fileName , int warehouseid);
}