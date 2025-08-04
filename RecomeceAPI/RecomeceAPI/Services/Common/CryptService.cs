namespace RecomeceAPI.Services.Common
{
  using RecomeceAPI.Interfaces;
  using System;
  using System.IO;
  using System.Security.Cryptography;
  using System.Text;

  public class CryptService : ICryptService
  {
    private static readonly string key_crypt = "FuturoJaSeFoi";
    private static readonly byte[] saltArray = Encoding.ASCII.GetBytes("security hidden");
    private static readonly int iterations = 10000;

    private Aes _aes;
    private bool _disposed = false;

    public CryptService()
    {
      _aes = Aes.Create();
    }

    public string Crypt(string clearText)
    {
      if (_disposed)
        throw new ObjectDisposedException(nameof(CryptService));

      try
      {
        string EncryptionKey = key_crypt;
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (var pdb = new Rfc2898DeriveBytes(EncryptionKey, saltArray, iterations, HashAlgorithmName.SHA256))
        {
          _aes.Key = pdb.GetBytes(32);
          _aes.IV = pdb.GetBytes(16);
          using (MemoryStream ms = new MemoryStream())
          {
            using (CryptoStream cs = new CryptoStream(ms, _aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
              cs.Write(clearBytes, 0, clearBytes.Length);
            }
            clearText = Convert.ToBase64String(ms.ToArray());
          }
        }
      }
      catch
      {
        clearText = "";
      }

      return clearText;
    }

    public string Decrypt(string cipherText)
    {
      if (_disposed)
        throw new ObjectDisposedException(nameof(CryptService));

      try
      {
        string EncryptionKey = key_crypt;
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (var pdb = new Rfc2898DeriveBytes(EncryptionKey, saltArray, iterations, HashAlgorithmName.SHA256))
        {
          _aes.Key = pdb.GetBytes(32);
          _aes.IV = pdb.GetBytes(16);
          using (MemoryStream ms = new MemoryStream())
          {
            using (CryptoStream cs = new CryptoStream(ms, _aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
              cs.Write(cipherBytes, 0, cipherBytes.Length);
            }
            cipherText = Encoding.Unicode.GetString(ms.ToArray());
          }
        }
      }
      catch
      {
        cipherText = "";
      }

      return cipherText;
    }

    public string CryptBase64(string texto)
    {
      try
      {
        byte[] textoAsBytes = Encoding.ASCII.GetBytes(texto);
        return Convert.ToBase64String(textoAsBytes);
      }
      catch (Exception)
      {
        throw;
      }
    }

    public string DecryptBase64(string dados)
    {
      try
      {
        byte[] dadosAsBytes = Convert.FromBase64String(dados);
        return Encoding.ASCII.GetString(dadosAsBytes);
      }
      catch (Exception)
      {
        throw;
      }
    }

    // Implementação do padrão IDisposable
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
        {
          // Liberar recursos gerenciados
          _aes?.Dispose();
        }

        _disposed = true;
      }
    }

    ~CryptService()
    {
      Dispose(false);
    }
  }

}
