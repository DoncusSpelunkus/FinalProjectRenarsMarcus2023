namespace Application.ErrorHandler;

 // just custom exception class that handle errors caused by the logic layer :D 
public class ApplicationException : Exception
{
    public ApplicationException()
    {
    }
    
    public ApplicationException(string message)
        : base(message)
    {
    }

    public ApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}