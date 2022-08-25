# asp.net core 6 web-api using Entity Framework

You can create users, authenticate a user which returns a (fake) token, then use the token to add todos to that user.

### How to run
1. Import the `api.martin.pfx` certificate into your trusted root certificate authorities.
2. Add `127.0.0.1 api.martin` to your hosts file at `C:\windows\system32\drivers\etc\hosts`
3. Compile & run the project.

### How to test
1. Access the swagger page at https://api.martin/swagger/index.html
2. Create a user, by posting to `https://api.martin/User` there are 3 different encryption methods

```
1 = pbkdf2
2 = bcrypt
3 = md5
```
4. Authenticate the user, by posting to `https://api.martin/Auth/Login` which returns a fake token.
5. Add a todo, by sending a POST request to `https://api.martin/UserTodos` providing the fake token.
6. Get all todos, by sending a GET request to `https://api.martin/UserTodos` providing the fake token.

## Hashing
I use pbkdf2 for the password hashing which is the default for asp.net core 6, bcrypt and md5 may also be used for testing purposes, however md5 is highly insecure and should never be used to store sensitive data, as this algorithm has been "broken" for a very long time.
The benefits of using md5 is that it is very fast, however since it does not use salt, it is very easy to crack, so it should never be used for passwords.

## Encryption
I use AES encryption for the todo's title & description, using the key stored in appsettings.json, this is not a secure way of storing the key, but for testing purposes it is fine.