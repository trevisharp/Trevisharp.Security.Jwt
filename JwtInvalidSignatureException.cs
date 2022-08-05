using System;

namespace Trevisharp.Security.Jwt.Exceptions;

public class JwtInvalidSignatureException : Exception
{
    public override string Message
        => "Token é inválido ou já expirou.";
}