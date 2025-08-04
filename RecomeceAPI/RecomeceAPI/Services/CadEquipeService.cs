using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class CadEquipeService : IRepository<CadEquipeModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;
    private readonly MovAuditoriaService _movAuditoriaService;
    private readonly ICryptService _cyptService;
    private readonly CadEmpresasService _cadEmpresasService;

    public CadEquipeService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService,
      MovAuditoriaService movAuditoriaService,
      ICryptService cyptService,
      CadEmpresasService cadEmpresasService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
      _movAuditoriaService = movAuditoriaService;
      _cyptService = cyptService;
      _cadEmpresasService = cadEmpresasService;
    }
    public void AddIdParameter(long id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }
    public void AddEntityParameters(CadEquipeModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Ativo", entity.Ativo);
      _dbContext.Parametros.AddItem("@p_Bairro", entity.Bairro);
      _dbContext.Parametros.AddItem("@p_Cep", ExtensionService.CleanSpecialChars(entity.Cep));
      _dbContext.Parametros.AddItem("@p_Complemento", entity.Complemento);
      _dbContext.Parametros.AddItem("@p_Email", entity.Email);
      _dbContext.Parametros.AddItem("@p_Senha", _cyptService.Crypt(entity.Senha));
      _dbContext.Parametros.AddItem("@p_Fk_CadEmpresas", entity.CadEmpresas.Id);
      _dbContext.Parametros.AddItem("@p_Inscricao", ExtensionService.CleanSpecialChars(entity.Inscricao));
      _dbContext.Parametros.AddItem("@p_Logradouro", entity.Logradouro);
      _dbContext.Parametros.AddItem("@p_Nome", entity.Nome);
      _dbContext.Parametros.AddItem("@p_Numero", entity.Numero);
      _dbContext.Parametros.AddItem("@p_Telefone", ExtensionService.CleanSpecialChars(entity.Telefone));
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo);

    }
    public async Task<object> SaveAsync(CadEquipeModel entity)
    {
      if (entity.CadEmpresas.Id == 0)
        entity.CadEmpresas.Id = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));

      entity.Senha = ExtensionService.CleanSpecialChars(entity.Inscricao);

      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into CadEquipe (Id, Ativo, Bairro, Cep, Complemento, Email, Fk_CadEmpresas, Inscricao, Logradouro, Nome, Numero, Telefone, Tipo, Senha) Select @p_Id, @p_Ativo, @p_Bairro, @p_Cep, @p_Complemento, @p_Email, @p_Fk_CadEmpresas, @p_Inscricao, @p_Logradouro, @p_Nome, @p_Numero, @p_Telefone, @p_Tipo, @p_Senha; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      var isSave = await _dbContext.SaveAsync(cmd, 0);
      entity.Id = isSave;
      await _movAuditoriaService.PrepareSave(1, "CadEquipe", isSave, (string)CompareTwoObjectsService.CompareEquals(new CadEquipeModel(), entity));
      return isSave;
    }
    public async Task<object> UpdateAsync(CadEquipeModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update CadEquipe
      Set 
      Id = @p_Id, 
      Ativo = @p_Ativo, 
      Bairro = @p_Bairro, 
      Cep = @p_Cep, 
      Complemento = @p_Complemento, 
      Email = @p_Email, 
      Fk_CadEmpresas = @p_Fk_CadEmpresas, 
      Inscricao = @p_Inscricao, 
      Logradouro = @p_Logradouro, 
      Nome = @p_Nome, 
      Numero = @p_Numero, 
      Telefone = @p_Telefone, 
      Tipo = @p_Tipo
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<object> UpdatePasswordAsync(long id, string pass)
    {
      string cmd = @"Update CadEquipe
      Set Senha = @p_Senha
      Where Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_Id", id);
      _dbContext.Parametros.AddItem("@p_Senha", _cyptService.Crypt(pass));
      await _dbContext.ExecAsync(cmd, 0);
      return 1;
    }
    public async Task<object> CreateFromCompany(long companyId, int userType)
    {
      var company = await _cadEmpresasService.GetByIdAsync(companyId);

      _dbContext.Parametros.AddItem("@p_CadEmpresas", companyId);
      _dbContext.Parametros.AddItem("@p_TipoUsuario", userType);
      _dbContext.Parametros.AddItem("@p_Senha", _cyptService.Crypt(company.Inscricao));
      await _dbContext.ExecAsync("CriarUsuarioEmpresa", 1);
      return true;
    }
    public async Task<CadEquipeModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From CadEquipe Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<CadEquipeModel> Login(string email, string pass)
    {
      string cmd = @"Select * From CadEquipe Where Email = @p_Email and Senha = @p_Senha;";
      _dbContext.Parametros.AddItem("@p_Email", email);
      _dbContext.Parametros.AddItem("@p_Senha", _cyptService.Crypt(pass));
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string IdCompany = AppIdentity.GetClaimValue("IdCompany");

      string cmd = @"Select 
      C.Id, C.Ativo, C.Bairro, C.Cep, C.Complemento, C.Email, C.Fk_CadEmpresas, C.Inscricao, C.Logradouro, C.Nome, C.Numero, C.Telefone, C.Tipo,
      Concat(Ce.Inscricao, ' - ', Ce.Nome) Empresa
      From CadEquipe C 
      Join CadEmpresas Ce on (Ce.Id = C.Fk_CadEmpresas)
      Where (C.Fk_CadEmpresas = @p_CadEmpresas)
      Order By C.Nome
      Limit @p_Pag, @p_rows
      ";
      _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", IdCompany);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> GetAllCompanyAsync(long IdCompany)
    {

      string cmd = @"Select 
      C.Id, C.Ativo, C.Bairro, C.Cep, C.Complemento, C.Email, C.Fk_CadEmpresas, C.Inscricao, C.Logradouro, C.Nome, C.Numero, C.Telefone, C.Tipo,
      Concat(Ce.Inscricao, ' - ', Ce.Nome) Empresa
      From CadEquipe C 
      Join CadEmpresas Ce on (Ce.Id = C.Fk_CadEmpresas)
      Where (C.Fk_CadEmpresas = @p_CadEmpresas)
      Order By C.Nome
      ";
      _dbContext.Parametros.AddItem("@p_CadEmpresas", IdCompany);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<object> GetAllCurrentCompanyAsync()
    {
      var IdCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      return await GetAllCompanyAsync(IdCompany);
    }
    public async Task<object> GetByIdDS(long id)
    {
      string cmd = @"Select 
      C.Id, C.Ativo, C.Bairro, C.Cep, C.Complemento, C.Email, C.Fk_CadEmpresas, C.Inscricao, C.Logradouro, C.Nome, C.Numero, C.Telefone, C.Tipo,
      Concat(Ce.Inscricao, ' - ', Ce.Nome) Empresa
      From CadEquipe C 
      Join CadEmpresas Ce on (Ce.Id = C.Fk_CadEmpresas)
      Where C.Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From CadEquipe Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CadEquipeModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadEquipeModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CadEquipeModel ConvertDataRowToObj(DataRow dr)
    {
      try
      {
        return new CadEquipeModel
        {
          Id = dr.Field<long>("Id"),
          Ativo = ExtensionService.SafeConvert<byte>(dr["Ativo"]),
          Tipo = ExtensionService.SafeConvert<byte>(dr["Tipo"]),
          Bairro = dr.Field<string>("Bairro") ?? "",
          Cep = dr.Field<string>("Cep") ?? "",
          Complemento = dr.Field<string>("Complemento") ?? "",
          Email = dr.Field<string>("Email") ?? "",
          CadEmpresas = new CadEmpresasModel { Id = dr.Field<long>("Fk_CadEmpresas") },
          Inscricao = dr.Field<string>("Inscricao") ?? "",
          Logradouro = dr.Field<string>("Logradouro") ?? "",
          Nome = dr.Field<string>("Nome") ?? "",
          Numero = dr.Field<string>("Numero") ?? "",
          Senha = dr.Field<string>("Senha") ?? "",
          Telefone = dr.Field<string>("Telefone") ?? "",
        };
      }
      catch (Exception ex)
      {
        return new CadEquipeModel();
      }

    }
    public CadEquipeModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadEquipeModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
  }
}