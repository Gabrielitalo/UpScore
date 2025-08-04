using RecomeceAPI.Models.Serasa.Enum;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public sealed class RegI001 : ISearasaLayout
  {
    public List<LayoutRule> Fields()
    {
      return new List<LayoutRule>
            {
                new LayoutRule(1, 1, 3, LayoutRuleDataType.Alphanumeric, Mnemonic.TipoReg, "I001", "Registro I001 – REGISTRO DE PEDIDO."),
                new LayoutRule(2, 5, 2, LayoutRuleDataType.Alphanumeric, Mnemonic.Subtipo, "00", "Subtipo de registro de pedido \"00\""),
                new LayoutRule(3, 7, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta CONCENTRE DETALHE/RESUMO (\"D\" ou \"R\" ou \" \")."),
                new LayoutRule(4, 8, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature CONFIRMEI. (\"S\" ou (\"N\" ou \" \"))"),
                new LayoutRule(5, 9, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature CONSULTAS À SERASA"),
                new LayoutRule(6, 10, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature CONCENTRE SCORING"),
                new LayoutRule(7, 11, 4, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Modelo de score desejado:"),
                new LayoutRule(8, 15, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature CONFIRMAÇÃO DE TELEFONE"),
                new LayoutRule(9, 16, 4, LayoutRuleDataType.Numeric, Mnemonic.Filler, "", "Código do DDD do telefone que deseja confirmar (obrigatório se \"S\" na CONFIRMAÇÃO DE TELEFONE)"),
                new LayoutRule(10, 20, 10, LayoutRuleDataType.Numeric, Mnemonic.Filler, "", "Número do TELEFONE que deseja confirmar (obrigatório se \"S\" na CONFIRMAÇÃO DE TELEFONE)"),
                new LayoutRule(11, 30, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature LIMITE DE CRÉDITO"),
                new LayoutRule(12, 31, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature LOCALIZADOR DE ENDEREÇOS"),
                new LayoutRule(13, 32, 2, LayoutRuleDataType.Numeric, Mnemonic.Filler, "", "QUANTIDADE DE ENDEREÇOS a serem retornados no LOCALIZADOR DE ENDEREÇOS."),
                new LayoutRule(14, 34, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature ALERT SCORING (CNPJ)"),
                new LayoutRule(15, 35, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature ALERTA IDENTIDADE (CPF)"),
                new LayoutRule(16, 36, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature PARTICIPAÇÃO SOCIETÁRIA"),
                new LayoutRule(17, 37, 2, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta anotação de AÇÃO POR UF ESPECÍFICO (não disponível)"),
                new LayoutRule(18, 39, 2, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta anotação de PROTESTO POR UF ESPECÍFICO (não disponível)"),
                new LayoutRule(19, 41, 2, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta anotação de CONVEM DEVEDORES POR UF ESPECÍFICO (não disponível)"),
                new LayoutRule(20, 43, 2, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta anotação de PEFIN POR UF ESPECÍFICO (não disponível)"),
                new LayoutRule(21, 45, 2, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta anotação de REFIN POR UF ESPECÍFICO (não disponível)"),
                new LayoutRule(22, 47, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature FATURAMENTO ESTIMADO COM POSITIVO (CNPJ)"),
                new LayoutRule(23, 48, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Uso da SERASA"),
                new LayoutRule(24, 49, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature SÓCIOS E ADMINISTRADORES (CNPJ)"),
                new LayoutRule(25, 50, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature PARTICIPAÇÕES EM EMPRESAS (CNPJ)"),
                new LayoutRule(26, 51, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature INDICADOR DE OPERACIONALIDADE (CNPJ)"),
                new LayoutRule(27, 52, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Consulta feature IRM - INDICE RELACIONAMENTO MERCADO"),
                new LayoutRule(28, 53, 4, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Modelo de Alerta de Identidade desejado(PF)"),
                new LayoutRule(29, 57, 4, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Modelo de Limite de Crédito desejado"),
                new LayoutRule(30, 61, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Meio de acesso"),
                new LayoutRule(31, 62, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Deseja consultar \"Situação Cadastral do CNPJ\"?"),
                new LayoutRule(32, 63, 5, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Uso da SERASA"),
                new LayoutRule(33, 68, 47, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Mensagem de erro"),
                new LayoutRule(34, 115, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Uso da SERASA")
            };
    }
  }
}
