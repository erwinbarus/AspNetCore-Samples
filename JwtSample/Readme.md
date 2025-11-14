Steps:



1. dotnet user-jwts key
   The signing key used to issue tokens is:
   <your-base64-key>
2. Update Your Program.cs to Use This Key

   ```
   IssuerSigningKey = new SymmetricSecurityKey(
   	Convert.FromBase64String("<your-base64-key>"))
   ```
3. Create a JWT.
   ```
   dotnet user-jwts create --claim scope=api --role admin
   ```
4. Curl Test.
   ```
   curl -i http://localhost:5166/hello -H "Authorization: Bearer <your-token>"
   ```
