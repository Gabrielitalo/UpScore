namespace RecomeceAPI.Interfaces
{
  public interface ICryptService
  {
    string Crypt(string clearText);
    string Decrypt(string cipherText);
    string CryptBase64(string texto);
    string DecryptBase64(string dados);
  }
}
