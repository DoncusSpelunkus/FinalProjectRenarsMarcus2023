namespace Application.InfraInterfaces;

public interface ITimeMapRepository
{
     string SignIn(int employeeId);
     string SignOut(int employeeId);
}