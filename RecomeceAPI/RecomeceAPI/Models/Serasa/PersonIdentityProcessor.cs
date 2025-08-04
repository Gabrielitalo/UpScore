using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomeceAPI.Models.Serasa
{
  public class PersonIdentityProcessor
  {
    public string TempString { get; private set; }
    public string Nome { get; private set; }
    public string DataNasc { get; private set; }
    public string Situacao { get; private set; }
    public string DataSituacao { get; private set; }
    public string NomeMae { get; private set; }

    public PersonIdentityProcessor(string tempString)
    {
      TempString = tempString;
    }

    public void SetPersonIdentity()
    {
      if (TempString.StartsWith("I10000"))
      {
        // Extrai os dados de identificação da pessoa
        string peopleData = TempString.Substring(0, 109);
        TempString = TempString.Substring(115);

        Nome = peopleData.Substring(6, 70).Trim();
        DataNasc = peopleData.Substring(76, 8).Trim();
        Situacao = peopleData.Substring(84, 1).Trim();
        DataSituacao = peopleData.Substring(85, 8).Trim();

        // Verifica se o próximo segmento contém o nome da mãe
        if (TempString.StartsWith("I10100"))
        {
          NomeMae = TempString.Substring(6, 60).Trim();
          TempString = TempString.Substring(115);
        }
      }

      // Ignora a string se for "I10200"
      if (TempString.StartsWith("I10200"))
      {
        TempString = TempString.Substring(115);
      }
    }
  }
}
