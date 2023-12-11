namespace Application.IServices;

public interface ITimeMapService
{
    string SignIn(int employeeId);
    string SignOut(int employeeId);
}