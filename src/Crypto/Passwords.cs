using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Athena.SDK.Definitions;
using FluentValidation;

namespace Athena.SDK.Crypto
{
    public static class Passwords
    {
        public static byte[] EncodePassword(string value) => Encoding.UTF8.GetBytes(value);
        public static string DecodePassword(byte[] value) => Encoding.UTF8.GetString(value);
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int IterationsCount = 1000;
        private const int Offset = 1;
        private static int HashSize => Offset + SaltSize + KeySize;


        public static IValidator<UserCredentialsValidationParams> CreateValidator()
        {
            return new InlineValidator<UserCredentialsValidationParams>
            {
                v => v.RuleFor(x => x.Username)
                    .NotNull()
                    .MaximumLength(UserCredentialsDefinitions.UsernameMaxLength)
                    .MinimumLength(UserCredentialsDefinitions.UsernameMinLength),
                v => v.RuleFor(x => x.Password)
                    .NotNull()
                    .MaximumLength(UserCredentialsDefinitions.PasswordMaxLength)
                    .MinimumLength(UserCredentialsDefinitions.PasswordMinLength)
                    .Must(pwd => ComplexityRegex.IsMatch(pwd))
            };
        }

        public static byte[] HashPassword(string password)
        {
            byte[] salt;
            byte[] key;

            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            using (var bytes = new Rfc2898DeriveBytes(password, SaltSize, IterationsCount, HashAlgorithmName.SHA512))
            {
                salt = bytes.Salt;
                key = bytes.GetBytes(KeySize);
            }

            var dst = new byte[HashSize];
            Buffer.BlockCopy(salt, 0, dst, Offset, SaltSize);
            Buffer.BlockCopy(key, 0, dst, SaltSize + Offset, KeySize);
            return dst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedPassword">Base64 hashed string</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool VerifyHashedPassword(byte[] storedPassword, string password)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));
            if (storedPassword == null) throw new ArgumentNullException(nameof(storedPassword));

            if (storedPassword.Length != HashSize || storedPassword[0] != 0)
            {
                return false;
            }

            var storedSalt = new byte[SaltSize];
            var storedKey = new byte[KeySize];

            Buffer.BlockCopy(storedPassword, Offset, storedSalt, 0, SaltSize);
            Buffer.BlockCopy(storedPassword, Offset + SaltSize, storedKey, 0, KeySize);

            using var bytes = new Rfc2898DeriveBytes(password, storedSalt, 1000, HashAlgorithmName.SHA512);

            var generatedKey = bytes.GetBytes(KeySize);

            return storedKey.SequenceEqual(generatedKey);
        }

        private static readonly Regex ComplexityRegex =
            new Regex(
                @"^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?])(?=.*[a-z])[A-Za-z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]{7,128}$",
                RegexOptions.Compiled);

        public static bool VerifyAuthorizationCode(byte[] storedData, byte[] authCode)
        {
            return storedData.SequenceEqual(authCode);
        }
    }
}