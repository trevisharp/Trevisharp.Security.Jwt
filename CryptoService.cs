using System;
using System.Text.Json;
using System.Security.Cryptography;

using static System.Convert;
using static System.Text.Encoding;

namespace Trevisharp.Security.Jwt;

using Exceptions;

public class CryptoService
{
    public int InternalKeySize { get; set; }
    public TimeSpan UpdatePeriod { get; set; }

    private string getRandomString(int seed, int size)
    {
        int base64BitCount = size * 6;
        int bytesCount = (int)Math.Ceiling(base64BitCount / 8.0);
        byte[] randData = new byte[bytesCount];

        Random rand = new Random(seed);
        rand.NextBytes(randData);

        var randString = ToBase64String(randData);

        return randString;
    }

    private int getSeedByTime(TimeSpan period)
    {
        var start = new DateTime(1970, 1, 1);
        var now = DateTime.Now;
        var timeElapsed = now - start;

        var currentEpoch = (int)(timeElapsed / period);
        var millis = (int)period.TotalMilliseconds;
        var magicNumberA = 1432;
        var magicNumberB = 9732;

        var seed = unchecked(
            currentEpoch * magicNumberA + magicNumberB * millis);

        return seed;
    }

    private string getInternalKey()
    {
        int seed = getSeedByTime(this.UpdatePeriod);
        int size = this.InternalKeySize;
        string randomString = getRandomString(seed, size);
        return randomString;
    }
    
    public string GetToken<T>(T obj)
    {
        var json = JsonSerializer.Serialize<T>(obj);
        var token = getTokenFromPayload(json);
        return token;
    }

    public T Validate<T>(string token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        var parts = token.Split('.');
        if (parts.Length != 3)
            throw new JwtInvalidFormatException();
        
        string header = parts[0],
               payload = parts[1],
               signature = parts[2];
        
        var secret = getInternalKey();
        var key = header + payload + secret;
        var expectedSignature = generateSignature(key);
        expectedSignature = expectedSignature.Replace("=", "");

        if (expectedSignature != signature)
            throw new JwtInvalidSignatureException();
        
        try
        {
            var correctPayload = paddingCorrection(payload);
            var payloadBytes = FromBase64String(correctPayload);
            var json = ASCII.GetString(payloadBytes);
            var obj = JsonSerializer.Deserialize<T>(json);
            return obj;
        }
        catch
        {
            throw new JwtInvalidPayloadException();
        }
    }

    private string paddingCorrection(string base64str)
    {
        int len = base64str.Length;
        int last32BytesSize = len % 4;
        if (last32BytesSize == 0)
            return base64str;
        
        int missingEquals = 4 - last32BytesSize;
        while (missingEquals > 0)
        {
            base64str += "=";
            missingEquals--;
        }
        return base64str;
    }

    private string getTokenFromPayload(string payload)
    {
        var header = "{ \"alg\": \"HS256\" }";
        byte[] headerBytes = ASCII.GetBytes(header);
        var base64Header = ToBase64String(headerBytes);
        base64Header = base64Header.Replace("=", "");

        byte[] payloadBytes = ASCII.GetBytes(payload);
        var base64Payload = ToBase64String(payloadBytes);
        base64Payload = base64Payload.Replace("=", "");

        var secret = getInternalKey();
        var key = base64Header + base64Payload + secret;
        var signature = generateSignature(key);

        var token = base64Header + "." + base64Payload + "." + signature;
        token = token.Replace("=", "");
        return token;
    }

    private string generateSignature(string data)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] dataBytes = ASCII.GetBytes(data);
            var hash = sha.ComputeHash(dataBytes);
            return ToBase64String(hash);
        }
    }
}