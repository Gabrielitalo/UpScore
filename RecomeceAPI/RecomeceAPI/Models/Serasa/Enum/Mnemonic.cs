using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomeceAPI.Models.Serasa.Enum
{
  public enum Mnemonic
  {
    [Description("FILLER")]
    Filler,
    [Description("NUM DOC")]
    NumDoc,
    [Description("TIPO PESSOA")]
    TipoPessoa,
    [Description("BASE CONS")]
    BaseCons,
    [Description("MODALIDADE")]
    Modalidade,
    [Description("VLR CONSUL")]
    VlrConsul,
    [Description("CENTRO CUST")]
    CentroCust,
    [Description("CODIFICADO")]
    Codificado,
    [Description("QTD REG")]
    QtdReg,
    [Description("CONVERSA")]
    Conversa,
    [Description("FUNÇÃO")]
    Funcao,
    [Description("TP CONSULTA")]
    TpConsulta,
    [Description("ATUALIZA")]
    Atualiza,
    [Description("IDENT_TERM")]
    IdentTerm,
    [Description("RESCLI")]
    ResCli,
    [Description("DELTS")]
    Delts,
    [Description("COBRA")]
    Cobra,
    [Description("PASSA")]
    Passa,
    [Description("CONS.COLLEC")]
    ConsCollec,
    [Description("CONSULTANTE")]
    Consultante,
    [Description("TIPO–REG")]
    TipoReg,
    [Description("COD1")]
    Cod1,
    [Description("CHAVE1")]
    Chave1,
    [Description("COD2")]
    Cod2,
    [Description("CHAVE2")]
    Chave2,
    [Description("COD3")]
    Cod3,
    [Description("CHAVE3")]
    Chave3,
    [Description("COD4")]
    Cod4,
    [Description("CHAVE4")]
    Chave4,
    [Description("SUBTIPO")]
    Subtipo,
    [Description("CÓDIGO")]
    Codigo,
    [Description("MENSAGEM")]
    Mensagem,
    [Description("NOME")]
    Nome,
    [Description("DATA_CONFIRM")]
    DataConfirm,
    [Description("COD_SIT_CAD")]
    CodSitCad,
    [Description("DATA_SIT")]
    DataSit,
    [Description("COD_RETORNO")]
    CodRetorno
  }

  // Método auxiliar para obter o valor da descrição
  public static class MnemonicExtensions
  {
    public static string GetDescription(this Mnemonic value)
    {
      var fieldInfo = value.GetType().GetField(value.ToString());
      var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

      return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }
  }
}
