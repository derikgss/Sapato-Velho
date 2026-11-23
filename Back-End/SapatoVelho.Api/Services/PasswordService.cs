using Microsoft.AspNetCore.Identity;
using SapatoVelho.Api.Models;

namespace SapatoVelho.Api.Services;

public interface IPasswordService
{
    string Hash(User user, string password);
    bool Verify(User user, string hash, string password, out bool needsRehash);
}

public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _hasher = new();
    public string Hash(User user, string password) => _hasher.HashPassword(user, password);

    public bool Verify(User user, string hash, string password, out bool needsRehash)
    {
        needsRehash = false;
        // Compatibilidade temporária: senhas legadas em texto puro são convertidas
        // para hash no primeiro login bem-sucedido. Novos cadastros sempre usam hash.
        if (!hash.StartsWith("AQAAAA", StringComparison.Ordinal))
        {
            var legacyMatches = string.Equals(hash, password, StringComparison.Ordinal);
            needsRehash = legacyMatches;
            return legacyMatches;
        }
        var result = _hasher.VerifyHashedPassword(user, hash, password);
        needsRehash = result == PasswordVerificationResult.SuccessRehashNeeded;
        return result != PasswordVerificationResult.Failed;
    }
}
