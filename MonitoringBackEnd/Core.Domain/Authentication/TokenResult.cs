public class TokenResult
{
	public string AccessToken { get; set; }
	public string RefreshToken { get; set; }

	public TokenResult(string accessToken, string refreshToken)
	{
		AccessToken = accessToken;
		RefreshToken = refreshToken;
	}
}
