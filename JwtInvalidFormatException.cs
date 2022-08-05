using System;

namespace Trevisharp.Security.Jwt.Exceptions;

public class JwtInvalidFormatException : Exception
{
    public override string Message 
        => "O token não está no format x.y.z requerido";
}