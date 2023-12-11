using Application.InfraInterfaces;
using Application.IServices;

namespace Application.Services;

public class TimeMapService : ITimeMapService
{
    private readonly ITimeMapRepository _timeMapRepository;

    public TimeMapService(ITimeMapRepository timeMapRepository)
    {
        _timeMapRepository = timeMapRepository;
    }
    
    public string SignIn(int employeeId)
    {
        return _timeMapRepository.SignIn(employeeId);
    }

    public string SignOut(int employeeId)
    {
        return _timeMapRepository.SignOut(employeeId);
    }
}