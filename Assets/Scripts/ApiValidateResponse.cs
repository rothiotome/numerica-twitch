using System;

[Serializable]
public class ApiValidateResponse
{
   public string client_id;
   public string login;
   public string[] scopes;
   public string user_id;
   public string expires_in;
}
