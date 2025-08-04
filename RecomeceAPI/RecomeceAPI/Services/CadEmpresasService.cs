using DocumentFormat.OpenXml.Vml.Office;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.ASAAS;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.ASAAS;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class CadEmpresasService : IRepository<CadEmpresasModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly CustomerService _customerService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CadEmpresasService(DbContext dbContext,
      PaginationService paginationService, CustomerService customerService,
      GenericModelValidatorService genericModelValidatorService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _customerService = customerService;
      _genericModelValidatorService = genericModelValidatorService;
    }
    public void AddIdParameter(long id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }
    public void AddEntityParameters(CadEmpresasModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Ativo", entity.Ativo);
      _dbContext.Parametros.AddItem("@p_Bairro", entity.Bairro);
      _dbContext.Parametros.AddItem("@p_BancoDados", entity.BancoDados);
      _dbContext.Parametros.AddItem("@p_Cep", ExtensionService.CleanSpecialChars(entity.Cep));
      _dbContext.Parametros.AddItem("@p_ChaveAPIAsaas", entity.ChaveAPIAsaas);
      _dbContext.Parametros.AddItem("@p_Complemento", entity.Complemento);
      _dbContext.Parametros.AddItem("@p_CpfResponsavel", ExtensionService.CleanSpecialChars(entity.CpfResponsavel));
      _dbContext.Parametros.AddItem("@p_EmailResponsavel", entity.EmailResponsavel.Trim());
      _dbContext.Parametros.AddItem("@p_Fk_Cidades", entity.Cidades.Id);
      _dbContext.Parametros.AddItem("@p_IdAsaas", entity.IdAsaas);
      _dbContext.Parametros.AddItem("@p_IdEmpresaResponsavel", entity.IdEmpresaResponsavel);
      _dbContext.Parametros.AddItem("@p_Inscricao", ExtensionService.CleanSpecialChars(entity.Inscricao.Trim()));
      _dbContext.Parametros.AddItem("@p_Logradouro", entity.Logradouro);
      _dbContext.Parametros.AddItem("@p_Nome", entity.Nome);
      _dbContext.Parametros.AddItem("@p_NomeResponsavel", entity.NomeResponsavel);
      _dbContext.Parametros.AddItem("@p_Numero", entity.Numero);
      _dbContext.Parametros.AddItem("@p_Servidor", entity.Servidor);
      _dbContext.Parametros.AddItem("@p_TelefoneResponsavel", ExtensionService.CleanSpecialChars(entity.TelefoneResponsavel));
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo);
      _dbContext.Parametros.AddItem("@p_WhiteLabelConfig", HandleJsonService.GetJson(entity.WhiteLabelConfig));
    }
    public async Task<object> SaveAsync(CadEmpresasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      var customer = await CreateAsaas(entity);
      entity.IdAsaas = customer.Id;
      entity.IdEmpresaResponsavel = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany")); // Empresa responsável é a que está fazendo o cadastro
      string cmd = @"Insert Into CadEmpresas (Id, Ativo, Bairro, BancoDados, Cep, ChaveAPIAsaas, Complemento, CpfResponsavel, EmailResponsavel, Fk_Cidades, IdAsaas, IdEmpresaResponsavel, Inscricao, Logradouro, Nome, NomeResponsavel, Numero, Servidor, TelefoneResponsavel, Tipo, WhiteLabelConfig) Select @p_Id, @p_Ativo, @p_Bairro, @p_BancoDados, @p_Cep, @p_ChaveAPIAsaas, @p_Complemento, @p_CpfResponsavel, @p_EmailResponsavel, @p_Fk_Cidades, @p_IdAsaas, @p_IdEmpresaResponsavel, @p_Inscricao, @p_Logradouro, @p_Nome, @p_NomeResponsavel, @p_Numero, @p_Servidor, @p_TelefoneResponsavel, @p_Tipo, @p_WhiteLabelConfig; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<CustomerModel> CreateAsaas(CadEmpresasModel entity)
    {
      return await _customerService.Create(entity.Nome, entity.Inscricao);
    }
    public async Task<object> UpdateAsync(CadEmpresasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      if (string.IsNullOrEmpty(entity.IdAsaas))
      {
        var customer = await CreateAsaas(entity);
        entity.IdAsaas = customer.Id;
      }
      string cmd = @"Update CadEmpresas
      Set 
      Id = @p_Id, 
      Ativo = @p_Ativo, 
      Bairro = @p_Bairro, 
      BancoDados = @p_BancoDados, 
      Cep = @p_Cep, 
      ChaveAPIAsaas = @p_ChaveAPIAsaas, 
      Complemento = @p_Complemento, 
      CpfResponsavel = @p_CpfResponsavel, 
      EmailResponsavel = @p_EmailResponsavel, 
      Fk_Cidades = @p_Fk_Cidades, 
      IdAsaas = @p_IdAsaas, 
      IdEmpresaResponsavel = @p_IdEmpresaResponsavel, 
      Inscricao = @p_Inscricao, 
      Logradouro = @p_Logradouro, 
      Nome = @p_Nome, 
      NomeResponsavel = @p_NomeResponsavel, 
      Numero = @p_Numero, 
      Servidor = @p_Servidor, 
      TelefoneResponsavel = @p_TelefoneResponsavel, 
      Tipo = @p_Tipo,
      WhiteLabelConfig = @p_WhiteLabelConfig
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public WhiteLabelConfigModel GetDefaultConfig(long empresaId, string nome)
    {
      return new WhiteLabelConfigModel
      {
        EmpresaId = empresaId,
        Tipo = 0,
        Nome = nome,
        Logo = "default-logo.png",
        ModeloProposta = "default_proposal",
        ModeloContrato = "default_contract"
      };

    }
    public async Task UpdateWhiteLabelConfig(WhiteLabelConfigModel config)
    {
      var company = await GetByIdAsync(config.EmpresaId);
      if (config.Tipo == 0)
        config = GetDefaultConfig(company.Id, company.Nome);

      string cmd = @"Update CadEmpresas
      Set 
      WhiteLabelConfig = @p_WhiteLabelConfig
      Where Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_WhiteLabelConfig", HandleJsonService.GetJson(config));
      _dbContext.Parametros.AddItem("@p_Id", config.EmpresaId);
      await _dbContext.ExecAsync(cmd, 0);
    }
    public async Task<CadEmpresasModel> GetByInscricaoAsync(string inscricao)
    {
      string cmd = @"Select C.Id, C.Ativo, C.Bairro, C.BancoDados, C.Cep, C.ChaveAPIAsaas, C.Complemento, C.CpfResponsavel,
      C.EmailResponsavel, C.Fk_Cidades, C.IdAsaas, C.IdEmpresaResponsavel, C.Inscricao, C.Logradouro, C.Nome, C.NomeResponsavel, 
      C.Numero, C.Servidor, C.TelefoneResponsavel, C.Tipo, C.WhiteLabelConfig, 
      Ci.NomeCidade, E.Nome NomeEstado, E.Uf
      From CadEmpresas C 
      Left Join Cidades Ci on (Ci.Id = C.Fk_Cidades) 
      Left Join Estados E on (E.Id = Ci.Fk_Estados)
      Where C.Inscricao = @p_Inscricao;";
      _dbContext.Parametros.AddItem("@p_Inscricao", inscricao);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<CadEmpresasModel> GetByIdAsync(long id)
    {
      string cmd = @"Select C.Id, C.Ativo, C.Bairro, C.BancoDados, C.Cep, C.ChaveAPIAsaas, C.Complemento, C.CpfResponsavel,
      C.EmailResponsavel, C.Fk_Cidades, C.IdAsaas, C.IdEmpresaResponsavel, C.Inscricao, C.Logradouro, C.Nome, C.NomeResponsavel, 
      C.Numero, C.Servidor, C.TelefoneResponsavel, C.Tipo, C.WhiteLabelConfig, 
      Ci.NomeCidade, E.Nome NomeEstado, E.Uf
      From CadEmpresas C 
      Left Join Cidades Ci on (Ci.Id = C.Fk_Cidades) 
      Left Join Estados E on (E.Id = Ci.Fk_Estados)
      Where C.Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<CadEmpresasModel> GetCompanyUserLogged()
    {
      long id = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      return await GetByIdAsync(id);
    }
    public async Task<object> GetByIdDS(long id)
    {
      string cmd = @"Select C.Id, C.Ativo, C.Bairro, C.BancoDados, C.Cep, C.ChaveAPIAsaas, C.Complemento, C.CpfResponsavel, 
      C.EmailResponsavel, C.Fk_Cidades, C.IdAsaas, C.IdEmpresaResponsavel, C.Inscricao, C.Logradouro, C.Nome, C.NomeResponsavel, 
      C.Numero, C.Servidor, C.TelefoneResponsavel, Tipo, 
      Concat(Ci.NomeCidade, ' - ', E.UF) Cidade,
      C.WhiteLabelConfig
      From CadEmpresas C 
      Join Cidades Ci on (Ci.Id = C.Fk_Cidades) 
      Join Estados E on (E.Id = Ci.Fk_Estados)
      Where C.Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      string cmd = @"Select 
      C.Id, C.Ativo, C.Nome, C.EmailResponsavel, C.TelefoneResponsavel, C.NomeResponsavel, C.Inscricao, C.Tipo, C.Fk_Cidades,
      Concat(Ci.NomeCidade, ' - ', E.UF) Cidade
      From CadEmpresas C 
      Join Cidades Ci on (Ci.Id = C.Fk_Cidades) 
      Join Estados E on (E.Id = Ci.Fk_Estados)
      Where (C.Tipo <> 3) and
      ((C.IdEmpresaResponsavel = 0 and C.Id = @p_CadEmpresas) or (C.IdEmpresaResponsavel = @p_CadEmpresas))
      Order By C.Nome
      Limit @p_Pag, @p_rows
      ";
      _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> GetAllCompany()
    {
      string cmd = @"Select Id, Nome
      From CadEmpresas
      Where Ativo = 1 and Tipo <> 3
      Order by Nome Asc;
      ";
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From CadEmpresas Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CadEmpresasModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadEmpresasModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CadEmpresasModel ConvertDataRowToObj(DataRow dr)
    {

      var WhiteLabelConfig = new WhiteLabelConfigModel();
      try
      {
        HandleJsonService.SetByJson(dr.Field<string>("WhiteLabelConfig") ?? "", WhiteLabelConfig);
      }
      catch
      {

      }
      try
      {
        return new CadEmpresasModel
        {
          Id = dr.Field<long>("Id"),
          Ativo = ExtensionService.SafeConvert<byte>(dr["Ativo"]),
          Tipo = ExtensionService.SafeConvert<byte>(dr["Tipo"]),
          BancoDados = ExtensionService.SafeConvert<byte>(dr["BancoDados"]),
          Servidor = ExtensionService.SafeConvert<byte>(dr["Servidor"]),
          Cep = dr.Field<string>("Cep") ?? "",
          ChaveAPIAsaas = dr.Field<string>("ChaveAPIAsaas") ?? "",
          Complemento = dr.Field<string>("Complemento") ?? "",
          CpfResponsavel = dr.Field<string>("CpfResponsavel") ?? "",
          EmailResponsavel = dr.Field<string>("EmailResponsavel") ?? "",
          IdAsaas = dr.Field<string>("IdAsaas") ?? "",
          IdEmpresaResponsavel = dr.Field<long>("IdEmpresaResponsavel"),
          Inscricao = dr.Field<string>("Inscricao") ?? "",
          Logradouro = dr.Field<string>("Logradouro") ?? "",
          Nome = dr.Field<string>("Nome") ?? "",
          NomeResponsavel = dr.Field<string>("NomeResponsavel") ?? "",
          Numero = dr.Field<string>("Numero") ?? "",
          TelefoneResponsavel = dr.Field<string>("TelefoneResponsavel") ?? "",
          WhiteLabelConfig = WhiteLabelConfig,
          Cidades = new CidadesModel
          {
            Id = dr.Field<long>("Fk_Cidades"),
            NomeCidade = dr.Field<string>("NomeCidade") ?? "",
            Estados = new EstadosModel
            {
              Nome = dr.Field<string>("NomeEstado") ?? "",
              UF = dr.Field<string>("UF") ?? ""
            }
          },

        };
      }
      catch (Exception ex)
      {
        return new CadEmpresasModel();
      }

    }
    public CadEmpresasModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadEmpresasModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
  }
}