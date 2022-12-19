namespace Dashboard.Domain;

public class BearerToken
{
	public readonly string Token;
	public BearerToken(string token) => Token = token ?? throw new ArgumentNullException(token);
}
