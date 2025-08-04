namespace RecomeceAPI.Services.Common
{
  public static class DocumentoValidator
  {
    public static bool IsCpfOuCnpjValido(string documento)
    {
      if (string.IsNullOrWhiteSpace(documento))
        return false;

      documento = new string(documento.Where(char.IsDigit).ToArray());

      return documento.Length switch
      {
        11 => IsCpfValido(documento),
        14 => IsCnpjValido(documento),
        _ => false
      };
    }

    public static bool IsCpfValido(string cpf)
    {
      if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
        return false;

      var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
      var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

      var tempCpf = cpf[..9];
      var soma = tempCpf.Select((t, i) => (t - '0') * multiplicador1[i]).Sum();

      var resto = soma % 11;
      var digito1 = resto < 2 ? 0 : 11 - resto;

      tempCpf += digito1;
      soma = tempCpf.Select((t, i) => (t - '0') * multiplicador2[i]).Sum();

      resto = soma % 11;
      var digito2 = resto < 2 ? 0 : 11 - resto;

      return cpf.EndsWith($"{digito1}{digito2}");
    }

    public static bool IsCnpjValido(string cnpj)
    {
      if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1)
        return false;

      var multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
      var multiplicador2 = new[] { 6 }.Concat(multiplicador1).ToArray();

      var tempCnpj = cnpj[..12];
      var soma = tempCnpj.Select((t, i) => (t - '0') * multiplicador1[i]).Sum();

      var resto = soma % 11;
      var digito1 = resto < 2 ? 0 : 11 - resto;

      tempCnpj += digito1;
      soma = tempCnpj.Select((t, i) => (t - '0') * multiplicador2[i]).Sum();

      resto = soma % 11;
      var digito2 = resto < 2 ? 0 : 11 - resto;

      return cnpj.EndsWith($"{digito1}{digito2}");
    }
  }

}
