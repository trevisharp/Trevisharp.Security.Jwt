using System;

namespace Trevisharp.Security.Jwt.Exceptions;

public class JwtInvalidPayloadException : Exception
{
    public override string Message 
        => "O payload estava em formato incorreto ou foi corrompido.";
}