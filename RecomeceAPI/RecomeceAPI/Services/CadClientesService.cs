using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class CadClientesService : IRepository<CadClientesModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CadClientesService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
    }
    public void AddIdParameter(long id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }
    public void AddEntityParameters(CadClientesModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Ativo", entity.Ativo);
      _dbContext.Parametros.AddItem("@p_Bairro", entity.Bairro);
      _dbContext.Parametros.AddItem("@p_Cep", ExtensionService.CleanSpecialChars(entity.Cep));
      _dbContext.Parametros.AddItem("@p_Complemento", entity.Complemento);
      _dbContext.Parametros.AddItem("@p_Email", entity.Email);
      _dbContext.Parametros.AddItem("@p_EstadoCivil", entity.EstadoCivil);
      _dbContext.Parametros.AddItem("@p_Fk_CadEquipe", entity.CadEquipe.Id);
      _dbContext.Parametros.AddItem("@p_Inscricao", ExtensionService.CleanSpecialChars(entity.Inscricao));
      _dbContext.Parametros.AddItem("@p_Logradouro", entity.Logradouro);
      _dbContext.Parametros.AddItem("@p_Nacionalidade", entity.Nacionalidade);
      _dbContext.Parametros.AddItem("@p_Nome", entity.Nome);
      _dbContext.Parametros.AddItem("@p_Numero", entity.Numero);
      _dbContext.Parametros.AddItem("@p_Profissao", entity.Profissao);
      _dbContext.Parametros.AddItem("@p_Senha", entity.Senha);
      _dbContext.Parametros.AddItem("@p_Telefone", ExtensionService.CleanSpecialChars(entity.Telefone));
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo);
      _dbContext.Parametros.AddItem("@p_Cidades", entity.Cidades.Id);
    }
    public async Task<object> SaveAsync(CadClientesModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into CadClientes (Id, Ativo, Bairro, Cep, Complemento, Email, EstadoCivil, Fk_CadEquipe, Inscricao, Logradouro, Nacionalidade, Nome, Numero, Profissao, Senha, Telefone, Tipo) Select @p_Id, @p_Ativo, @p_Bairro, @p_Cep, @p_Complemento, @p_Email, @p_EstadoCivil, @p_Fk_CadEquipe, @p_Inscricao, @p_Logradouro, @p_Nacionalidade, @p_Nome, @p_Numero, @p_Profissao, @p_Senha, @p_Telefone, @p_Tipo; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }

    public async Task<object> SimpleSave(CadClientesModel entity)
    {
      entity.Ativo = 0;
      entity.Tipo = entity.Inscricao.Length == 11 ? (byte)1 : (byte)2;
      entity.Nacionalidade = "Brasileira";
      entity.Senha = "nada";
      entity.CadEquipe.Id = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));
      return await SaveAsync(entity);
    }
    public async Task<object> UpdateAsync(CadClientesModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update CadClientes
      Set Ativo = @p_Ativo, 
      Bairro = @p_Bairro, 
      Cep = @p_Cep, 
      Complemento = @p_Complemento, 
      Email = @p_Email, 
      EstadoCivil = @p_EstadoCivil, 
      Fk_CadEquipe = @p_Fk_CadEquipe, 
      Inscricao = @p_Inscricao, 
      Logradouro = @p_Logradouro, 
      Nacionalidade = @p_Nacionalidade, 
      Nome = @p_Nome, 
      Numero = @p_Numero, 
      Profissao = @p_Profissao, 
      Fk_Cidades = @p_Cidades, 
      Telefone = @p_Telefone, 
      Tipo = @p_Tipo
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<CadClientesModel> GetByIdAsync(long id)
    {
      string cmd = @"Select C.Id, C.Ativo, C.Bairro, C.Cep, C.Complemento, C.Email, C.EstadoCivil, 
      C.Fk_CadEquipe, C.Inscricao, C.Logradouro, C.Nacionalidade, C.Nome, 
      C.Numero, C.Profissao, C.Telefone, C.Tipo, C.Fk_Cidades,
      Ci.NomeCidade, E.Nome NomeEstado, E.Uf
      From CadClientes C
      Left Join Cidades Ci on (Ci.Id = C.Fk_Cidades) 
      Left Join Estados E on (E.Id = Ci.Fk_Estados)
      Where C.Id = @p_Id";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<object> GetByIdDS(long id)
    {
      string cmd = @"Select C.Id, C.Ativo, C.Bairro, C.Cep, C.Complemento, C.Email, C.EstadoCivil, 
      C.Fk_CadEquipe, C.Inscricao, C.Logradouro, C.Nacionalidade, C.Nome, 
      C.Numero, C.Profissao, C.Telefone, C.Tipo, C.Fk_Cidades,
      Concat(Ci.NomeCidade, ' - ', E.UF) Cidade
      From CadClientes C
      Left Join Cidades Ci on (Ci.Id = C.Fk_Cidades) 
      Left Join Estados E on (E.Id = Ci.Fk_Estados)
      Where C.Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<CadClientesModel> GetByInscAsync(string insc)
    {
      insc = ExtensionService.CleanSpecialChars(insc);
      var entity = new CadClientesModel();
      string cmd = @"Select C.Id, C.Ativo, C.Bairro, C.Cep, C.Complemento, C.Email, C.EstadoCivil, 
      C.Fk_CadEquipe, C.Inscricao, C.Logradouro, C.Nacionalidade, C.Nome, 
      C.Numero, C.Profissao, C.Telefone, C.Tipo, C.Fk_Cidades,
      Ci.NomeCidade, E.Nome NomeEstado, E.Uf
      From CadClientes C
      Join CadEquipe Ce on (Ce.Id = C.Fk_CadEquipe)
      Left Join Cidades Ci on (Ci.Id = C.Fk_Cidades) 
      Left Join Estados E on (E.Id = Ci.Fk_Estados) 
      Where C.Inscricao = @p_Inscricao and Ce.Fk_CadEmpresas = @p_CadEmpresa
      Limit 1;";
      _dbContext.Parametros.AddItem("@p_Inscricao", insc);
      _dbContext.Parametros.AddItem("@p_CadEmpresa", AppIdentity.GetIdCompanyValue());
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      try
      {
        entity = SetByDataSet(result);
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(ex);
      }
      return entity;
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From CadClientes Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage, string nome, string insc)
    {
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      long idRole = Convert.ToInt64(AppIdentity.GetClaimValue("IdRole"));
      if (idRole > 1)
        return await GetAllSaler(page, itemsPerPage, nome, insc);

      if (string.IsNullOrEmpty(nome)) nome = "";
      if (string.IsNullOrEmpty(insc))
        insc = "";
      else
        insc = ExtensionService.CleanSpecialChars(insc);

      string cmd = @"SELECT * 
      FROM CadClientes Cc
      Join CadEquipe Ce on (Ce.Id = Cc.Fk_CadEquipe)
      WHERE (Ce.Fk_CadEmpresas = @p_CadEmpresas) and  
        (
        (@p_Inscricao <> '' AND Cc.Inscricao = @p_Inscricao)
        OR
        (
          @p_Inscricao = '' AND
          (@p_Nome = '' OR Cc.Nome LIKE CONCAT('%', @p_Nome, '%'))
        )
      )
      Order by Cc.Nome
      LIMIT @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      _dbContext.Parametros.AddItem("@p_Nome", nome);
      _dbContext.Parametros.AddItem("@p_Inscricao", insc);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<PaginationModel> GetAllSaler(int page, int itemsPerPage, string nome, string insc)
    {
      long idUser = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));
      if (string.IsNullOrEmpty(nome)) nome = "";
      if (string.IsNullOrEmpty(insc))
        insc = "";
      else
        insc = ExtensionService.CleanSpecialChars(insc);

      string cmd = @"SELECT * 
      FROM CadClientes Cc
      Join CadEquipe Ce on (Ce.Id = Cc.Fk_CadEquipe)
      WHERE (Ce.Id = @p_CadEquipe) and
        (
        (@p_Inscricao <> '' AND Cc.Inscricao = @p_Inscricao)
        OR
        (
          @p_Inscricao = '' AND
          (@p_Nome = '' OR Cc.Nome LIKE CONCAT('%', @p_Nome, '%'))
        )
      )
      Order by Cc.Nome
      LIMIT @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      _dbContext.Parametros.AddItem("@p_Nome", nome);
      _dbContext.Parametros.AddItem("@p_Inscricao", insc);
      _dbContext.Parametros.AddItem("@p_CadEquipe", idUser);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From CadClientes Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CadClientesModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadClientesModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CadClientesModel ConvertDataRowToObj(DataRow dr)
    {
      return new CadClientesModel
      {
        Id = dr.Field<long>("Id"),
        Ativo = ExtensionService.SafeConvert<byte>(dr["Ativo"]),
        Tipo = ExtensionService.SafeConvert<byte>(dr["Tipo"]),
        Bairro = dr.Field<string>("Bairro") ?? "",
        Cep = dr.Field<string>("Cep") ?? "",
        Complemento = dr.Field<string>("Complemento") ?? "",
        Email = dr.Field<string>("Email") ?? "",
        EstadoCivil = dr.Field<string>("EstadoCivil") ?? "",
        CadEquipe = new CadEquipeModel { Id = dr.Field<long>("Fk_CadEquipe") },
        Inscricao = dr.Field<string>("Inscricao") ?? "",
        Logradouro = dr.Field<string>("Logradouro") ?? "",
        Nacionalidade = dr.Field<string>("Nacionalidade") ?? "",
        Nome = dr.Field<string>("Nome") ?? "",
        Numero = dr.Field<string>("Numero") ?? "",
        Profissao = dr.Field<string>("Profissao") ?? "",
        Telefone = dr.Field<string>("Telefone") ?? "",
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
    public CadClientesModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadClientesModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }

    public async Task<List<NotificationModel>> IsFullClienAsync(long id)
    {
      List<NotificationModel> list = new List<NotificationModel>();
      var entity = await GetByIdAsync(id);

      var camposObrigatorios = new List<string> { "Nome", "Email", "Telefone", "Bairro", "Cep", "EstadoCivil", "Inscricao", "Logradouro", "Numero", "Profissao" };
      list = CompareTwoObjectsService.ValidateRequiredFields(entity, camposObrigatorios);

      if (entity.Cidades.Id == 0 && entity.Cidades.Id == 5562)
        list.Add(NotificationService.Validation("Necessário preencher o campo de cidade"));

      return list;
    }
    public async Task<object> Autocomplete(string texto)
    {
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      string cmd = @"Select C.Id, C.Nome Texto
      From CadClientes C 
      Join CadEquipe E on (E.Id = C.Fk_CadEquipe) 
      Where (E.Fk_CadEmpresas = @p_CadEmpresas) and
      (C.Nome like @p_texto)
      Limit 10;";
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      _dbContext.Parametros.AddItem("@p_texto", $"%{texto}%", 1);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
  }
}