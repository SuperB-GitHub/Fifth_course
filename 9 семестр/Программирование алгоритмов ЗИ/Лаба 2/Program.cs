using System;
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        using (RSA rsa = RSA.Create(4096))
        {
            string publicKey = rsa.ToXmlString(false);
            string privateKey = rsa.ToXmlString(true);  

            Console.WriteLine("=== Генерация ключей RSA ===\n");
            Console.WriteLine("Публичный ключ:");
            Console.WriteLine(publicKey);
            Console.WriteLine("\nПриватный ключ:");
            Console.WriteLine(privateKey);

            Console.WriteLine("\nВведите сообщение:");
            string originalMessage = Console.ReadLine()!;
            Console.WriteLine($"\n=== Исходное сообщение ===\n{originalMessage}");

            byte[] encryptedBytes;
            using (RSA rsaEncrypt = RSA.Create())
            {
                rsaEncrypt.FromXmlString(publicKey);
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(originalMessage);
                encryptedBytes = rsaEncrypt.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
            }

            Console.WriteLine($"\n=== Зашифрованное сообщение (Base64) ===\n{Convert.ToBase64String(encryptedBytes)}");

            string decryptedMessage;
            using (RSA rsaDecrypt = RSA.Create())
            {
                rsaDecrypt.FromXmlString(privateKey);
                byte[] decryptedBytes = rsaDecrypt.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
                decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);
            }

            Console.WriteLine($"\n=== Расшифрованное сообщение ===\n{decryptedMessage}");

            Console.WriteLine("\n=== Цифровая подпись ===");
            byte[] signature;
            using (RSA rsaSign = RSA.Create())
            {
                rsaSign.FromXmlString(privateKey);
                byte[] dataToSign = Encoding.UTF8.GetBytes(originalMessage);
                signature = rsaSign.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            Console.WriteLine($"Подпись (Base64): {Convert.ToBase64String(signature)}");

            bool isValid;
            using (RSA rsaVerify = RSA.Create())
            {
                rsaVerify.FromXmlString(publicKey);
                byte[] dataToVerify = Encoding.UTF8.GetBytes(originalMessage);
                isValid = rsaVerify.VerifyData(dataToVerify, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            Console.WriteLine($"Подпись корректна: {isValid}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}