namespace Application.IServices;

public interface IEmailService
{ 
    void SendTemporaryCredentials(string receiverEmail, string password);
}