using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office.Word;
using MySqlX.XDevAPI;
using RecomeceAPI.DTOs;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services.Serasa;
using System.Data;

namespace RecomeceAPI.Services
{
  public class MovPropostasService : IRepository<MovPropostasModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;
    private readonly CadClientesService _cadClientesService;
    private readonly MovCadClientesSerasaService _movCadClientesSerasaService;
    private readonly RestSerasaService _restSerasaService;
    private readonly CadProdutosService _cadProdutosService;
    private readonly CadProdutosFaixasService _cadProdutosFaixasService;

    public MovPropostasService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService,
      CadClientesService cadClientesService,
      MovCadClientesSerasaService movCadClientesSerasaService,
      RestSerasaService restSerasaService,
      CadProdutosService cadProdutosService,
      CadProdutosFaixasService cadProdutosFaixasService
      )
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
      _cadClientesService = cadClientesService;
      _movCadClientesSerasaService = movCadClientesSerasaService;
      _restSerasaService = restSerasaService;
      _cadProdutosService = cadProdutosService;
      _cadProdutosFaixasService = cadProdutosFaixasService;
    }
    public void AddIdParameter(long id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }
    public void AddEntityParameters(MovPropostasModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_DataHoraCadastro", entity.DataHoraCadastro);
      _dbContext.Parametros.AddItem("@p_DataHoraFechamento", entity.DataHoraFechamento);
      _dbContext.Parametros.AddItem("@p_Fk_CadClientes", entity.CadClientes.Id);
      _dbContext.Parametros.AddItem("@p_Fk_CadEquipe", entity.CadEquipe.Id);
      _dbContext.Parametros.AddItem("@p_Fk_CadProdutos", entity.CadProdutos.Id);
      _dbContext.Parametros.AddItem("@p_NumeroContrato", entity.NumeroContrato);
      _dbContext.Parametros.AddItem("@p_Observacao", entity.Observacao);
      _dbContext.Parametros.AddItem("@p_PercDesconto", entity.PercDesconto);
      _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao);
      _dbContext.Parametros.AddItem("@p_Termometro", entity.Termometro);
      _dbContext.Parametros.AddItem("@p_ValorAprovado", entity.ValorAprovado);
      _dbContext.Parametros.AddItem("@p_ValorContrato", entity.ValorContrato);
      _dbContext.Parametros.AddItem("@p_ValorDivida", entity.ValorDivida);
      _dbContext.Parametros.AddItem("@p_ValorEntrada", entity.ValorEntrada);
      _dbContext.Parametros.AddItem("@p_ValorAvulso", entity.ValorAvulso);

    }
    public async Task<object> SaveAsync(MovPropostasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into MovPropostas (Id, DataHoraCadastro, DataHoraFechamento, Fk_CadClientes, Fk_CadEquipe, Fk_CadProdutos, NumeroContrato, Observacao, PercDesconto, Situacao, Termometro, ValorAprovado, ValorContrato, ValorDivida, ValorEntrada, ValorAvulso) Select @p_Id, @p_DataHoraCadastro, @p_DataHoraFechamento, @p_Fk_CadClientes, @p_Fk_CadEquipe, @p_Fk_CadProdutos, @p_NumeroContrato, @p_Observacao, @p_PercDesconto, @p_Situacao, @p_Termometro, @p_ValorAprovado, @p_ValorContrato, @p_ValorDivida, @p_ValorEntrada, @p_ValorAvulso; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public string NewContractNumber()
    {
      var dtAtual = DateTime.Now;
      var random = new Random();
      int codigoAleatorio = random.Next(1000, 9999); // Número aleatório de 4 dígitos

      return $"{dtAtual:yyyyMM}-{codigoAleatorio}";
    }
    public async Task<object> NewSerasaProposal(long clientID, long productID)
    {
      long idUser = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));

      var entity = new MovPropostasModel
      {
        Id = 0,
        DataHoraCadastro = DateTime.Now,
        CadClientes = new CadClientesModel { Id = clientID },
        CadEquipe = new CadEquipeModel { Id = Convert.ToInt64(idUser) },
        CadProdutos = new CadProdutosModel { Id = productID },
        NumeroContrato = NewContractNumber(),
        Observacao = "",
        PercDesconto = 0,
        Situacao = 1, // Gerado
        Termometro = 1,
        ValorAprovado = 0,
        ValorContrato = 0,
        ValorDivida = 0,
        ValorEntrada = 0
      };

      return await SaveAsync(entity);
    }
    public async Task<object> UpdateAsync(MovPropostasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostas
      Set 
      DataHoraCadastro = @p_DataHoraCadastro, 
      DataHoraFechamento = @p_DataHoraFechamento, 
      Fk_CadClientes = @p_Fk_CadClientes, 
      Fk_CadEquipe = @p_Fk_CadEquipe, 
      Fk_CadProdutos = @p_Fk_CadProdutos, 
      NumeroContrato = @p_NumeroContrato, 
      Observacao = @p_Observacao, 
      PercDesconto = @p_PercDesconto, 
      Situacao = @p_Situacao, 
      Termometro = @p_Termometro, 
      ValorAprovado = @p_ValorAprovado, 
      ValorContrato = @p_ValorContrato, 
      ValorDivida = @p_ValorDivida, 
      ValorEntrada = @p_ValorEntrada
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<object> UpdateTotals(MovPropostasModel entity)
    {
      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostas
      Set ValorAprovado = @p_ValorAprovado, 
      ValorContrato = @p_ValorContrato, 
      ValorDivida = @p_ValorDivida
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<object> UpdateStatus(long proposalId, int status)
    {
      if (await IsExists(proposalId) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostas
      Set Situacao = @p_Situacao,
      ValorAprovado = Case When @p_Situacao = 3 and ValorAprovado = 0 Then ValorContrato Else ValorAprovado End,
      DataHoraFechamento = Now()
      Where Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_Situacao", status);
      _dbContext.Parametros.AddItem("@p_Id", proposalId);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public async Task<object> UpdateValorAprovado(long proposalId, decimal valorAprovado)
    {
      if (await IsExists(proposalId) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostas
      Set ValorAprovado = @p_ValorAprovado
      Where Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_ValorAprovado", valorAprovado);
      _dbContext.Parametros.AddItem("@p_Id", proposalId);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public async Task<object> UpdateValorAvulso(long proposalId, decimal valorAvulso)
    {
      if (await IsExists(proposalId) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostas
      Set ValorAvulso = @p_ValorAvulso
      Where Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_ValorAvulso", valorAvulso);
      _dbContext.Parametros.AddItem("@p_Id", proposalId);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public async Task<object> UpdateOwner(long proposalId, long clientId)
    {
      if (await IsExists(proposalId) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostas
      Set Fk_CadClientes = @p_CadClientes
      Where Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_CadClientes", clientId);
      _dbContext.Parametros.AddItem("@p_Id", proposalId);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public async Task<object> UpdateDate(long proposalId, string date)
    {
      if (await IsExists(proposalId) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostas
      Set DataHoraCadastro = @p_DataHoraCadastro
      Where Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_DataHoraCadastro", date);
      _dbContext.Parametros.AddItem("@p_Id", proposalId);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public async Task<object> UpdateOrigem(long proposalId, int origemId)
    {
      if (await IsExists(proposalId) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostas
      Set Fk_CadOrigens = @p_CadOrigens
      Where Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_CadOrigens", origemId);
      _dbContext.Parametros.AddItem("@p_Id", proposalId);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public async Task<object> UpdateVendedor(long proposalId, long equipeId)
    {
      if (await IsExists(proposalId) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostas
      Set Fk_CadEquipe = @p_CadEquipe
      Where Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_CadEquipe", equipeId);
      _dbContext.Parametros.AddItem("@p_Id", proposalId);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public async Task<object> SumTotals(MovPropostasModel entity)
    {
      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostas
      Set ValorAprovado += @p_ValorAprovado, 
      ValorContrato += @p_ValorContrato, 
      ValorDivida += @p_ValorDivida
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<MovPropostasModel> GetByIdAsync(long id)
    {
      string cmd = @"Select P.*, Cp.LimpaNome, Cp.Descricao NomeProduto From MovPropostas P Join CadProdutos Cp on (Cp.Id = P.Fk_CadProdutos) Where P.Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<object> GetOrigens()
    {
      string cmd = @"Select Id, Nome From CadOrigens Order by Nome Desc";
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<object> GetByIdDetails(long id)
    {
      string cmd = @"SELECT 
      JSON_OBJECT(
          'Id', p.Id,
          'NumeroContrato', p.NumeroContrato,
          'DataCadastro', p.DataHoraCadastro,
          'ValorDivida', p.ValorDivida,
          'ValorContrato', p.ValorContrato,
          'ValorAprovado', p.ValorAprovado,
          'Desconto', p.ValorContrato - p.ValorAprovado,
          'Situacao', p.Situacao,
          'ValorAvulso', p.ValorAvulso,
          'CadOrigensId', p.Fk_CadOrigens,
          'OrigemDesc', Co.Nome,
          'TitularId', Ct.Id,
          'TitularNome', Ct.Nome,
          'TitularInscricao', Ct.Inscricao,
          'VendedorNome', Ce.Nome,
          'VendedorId', Ce.Id,
          'ProdutoId', Cp.Id,
          'Observacao', p.Observacao,
          'Beneficiarios', (
              SELECT JSON_ARRAYAGG(
                  JSON_OBJECT(
                      'Id', b.Id,
                      'Score', b.Score, 
                      'ValorDivida', b.ValorDivida,
                      'ValorContrato', b.ValorContrato,
                      'IdCliente', Cc.Id,
                      'Nome', Cc.Nome,
                      'Inscricao', Cc.Inscricao,
                      'MarkID', b.MarkID,
                      'IsTitular', Case When b.Fk_CadClientes = p.Fk_CadClientes Then 1 Else 0 End
                  )
              )
              FROM MovPropostasBeneficiarios b
              Join CadClientes Cc on (Cc.Id = b.Fk_CadClientes)
              WHERE b.Fk_MovPropostas = p.Id
          )
      ) AS Proposta
      FROM MovPropostas p
      Join CadProdutos Cp on (Cp.Id = p.Fk_CadProdutos)
      Join CadEquipe Ce on (Ce.Id = p.Fk_CadEquipe)
      Join CadClientes Ct on (Ct.Id = p.Fk_CadClientes)
      Left Join CadOrigens Co on (Co.Id = p.Fk_CadOrigens)
      WHERE p.Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public string GetConStrGetAll(long vendedor)
    {
      string cmd = "";
      long idRole = Convert.ToInt64(AppIdentity.GetClaimValue("IdRole"));

      if (idRole == 0)
        cmd = @"      SELECT P.Id, P.DataHoraCadastro, P.DataHoraFechamento, P.Fk_CadClientes, P.Fk_CadEquipe, P.Fk_CadProdutos, P.NumeroContrato, P.Observacao, P.PercDesconto, 
      P.Situacao, P.Termometro, P.ValorAprovado, P.ValorContrato, P.ValorDivida, P.ValorAvulso, Cc.Nome, Ce.Nome Vendedor, Cp.LimpaNome
      FROM MovPropostas P
      JOIN CadClientes Cc ON (Cc.Id = P.Fk_CadClientes)
      JOIN CadEquipe Ce ON (Ce.Id = P.Fk_CadEquipe)      
      Join CadProdutos Cp on (Cp.Id = P.Fk_CadProdutos)
      WHERE 
      (
        @p_Inscricao <> '' AND Cc.Inscricao = @p_Inscricao
      )
      OR
      (
        @p_Inscricao = '' AND
        P.DataHoraCadastro BETWEEN @p_DataInicial AND @p_DataFinal AND
        (@p_Nome = '' OR Cc.Nome LIKE CONCAT('%', @p_Nome, '%')) AND
        (@p_Situacao = 0 OR P.Situacao = @p_Situacao) and 
        (@p_Vendedor = 0 OR P.Fk_CadEquipe = @p_Vendedor)
      )
      Order by P.DataHoraCadastro Desc
      Limit @p_Pag, @p_rows;";
      else if (idRole == 1)
        cmd = @"      SELECT P.Id, P.DataHoraCadastro, P.DataHoraFechamento, P.Fk_CadClientes, P.Fk_CadEquipe, P.Fk_CadProdutos, P.NumeroContrato, P.Observacao, P.PercDesconto, 
      P.Situacao, P.Termometro, P.ValorAprovado, P.ValorContrato, P.ValorDivida, P.ValorAvulso, Cc.Nome, Ce.Nome Vendedor, Cp.LimpaNome
      FROM MovPropostas P
      JOIN CadClientes Cc ON (Cc.Id = P.Fk_CadClientes)
      JOIN CadEquipe Ce ON (Ce.Id = P.Fk_CadEquipe) and (Ce.Fk_CadEmpresas = @p_CadEmpresas)
      Join CadProdutos Cp on (Cp.Id = P.Fk_CadProdutos)
      WHERE 
      (
        @p_Inscricao <> '' AND Cc.Inscricao = @p_Inscricao
      )
      OR
      (
        @p_Inscricao = '' AND
        P.DataHoraCadastro BETWEEN @p_DataInicial AND @p_DataFinal AND
        (@p_Nome = '' OR Cc.Nome LIKE CONCAT('%', @p_Nome, '%')) AND
        (@p_Situacao = 0 OR P.Situacao = @p_Situacao) and 
        (@p_Vendedor = 0 OR P.Fk_CadEquipe = @p_Vendedor)
      )
      Order by P.DataHoraCadastro Desc
      Limit @p_Pag, @p_rows;";
      else if (idRole > 1)
        cmd = @"      SELECT P.Id, P.DataHoraCadastro, P.DataHoraFechamento, P.Fk_CadClientes, P.Fk_CadEquipe, P.Fk_CadProdutos, P.NumeroContrato, P.Observacao, P.PercDesconto, 
      P.Situacao, P.Termometro, P.ValorAprovado, P.ValorContrato, P.ValorDivida, P.ValorAvulso, Cc.Nome, Ce.Nome Vendedor, Cp.LimpaNome
      FROM MovPropostas P
      JOIN CadClientes Cc ON (Cc.Id = P.Fk_CadClientes)
      JOIN CadEquipe Ce ON (Ce.Id = P.Fk_CadEquipe) and (Ce.Id = @p_CadEquipe) 
      Join CadProdutos Cp on (Cp.Id = P.Fk_CadProdutos)
      WHERE 
      (
        @p_Inscricao <> '' AND Cc.Inscricao = @p_Inscricao
      )
      OR
      (
        @p_Inscricao = '' AND
        P.DataHoraCadastro BETWEEN @p_DataInicial AND @p_DataFinal AND
        (@p_Nome = '' OR Cc.Nome LIKE CONCAT('%', @p_Nome, '%')) AND
        (@p_Situacao = 0 OR P.Situacao = @p_Situacao)       
      )
      Order by P.DataHoraCadastro Desc
      Limit @p_Pag, @p_rows;";

      return cmd;
    }
    public async Task<PaginationModel> GetAllAsync(string nome, string insc, int situacao, string dataInicial, string dataFinal, int page, long vendedor)
    {
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      long idUser = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));
      int itemsPerPage = 30;
      if (string.IsNullOrEmpty(nome)) nome = "";
      if (string.IsNullOrEmpty(insc))
        insc = "";
      else
        insc = ExtensionService.CleanSpecialChars(insc);


      string cmd = GetConStrGetAll(vendedor);
      if (dataInicial == "0001-01-01 00:00:00")
        cmd = cmd.Replace("P.DataHoraCadastro BETWEEN @p_DataInicial AND @p_DataFinal AND", "");
      _dbContext.Parametros.AddItem("@p_Nome", nome);
      _dbContext.Parametros.AddItem("@p_Inscricao", insc);
      _dbContext.Parametros.AddItem("@p_Situacao", situacao);
      _dbContext.Parametros.AddItem("@p_DataInicial", dataInicial);
      _dbContext.Parametros.AddItem("@p_DataFinal", dataFinal);
      _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      _dbContext.Parametros.AddItem("@p_CadEquipe", idUser);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      _dbContext.Parametros.AddItem("@p_Vendedor", vendedor);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<PaginationModel> GetAllFromClient(long clientId)
    {
      int itemsPerPage = 30;

      string cmd = @"
      Select Distinct Mp.Id, Mp.NumeroContrato, Mp.ValorAvulso, Mp.DataHoraCadastro, Mpb.Score, Mpb.ValorDivida, Mpb.MarkID, Cc.Nome Tituar
      From MovPropostasBeneficiarios Mpb
      Join MovPropostas Mp on (Mp.Id = Mpb.Fk_MovPropostas)
      Join CadClientes Cc on (Cc.Id = Mp.Fk_CadClientes)
      Where (Mpb.Fk_CadClientes = @p_CadClientes)
      Order by Mp.DataHoraCadastro Desc;";
      _dbContext.Parametros.AddItem("@p_CadClientes", clientId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, 1, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From MovPropostas Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<MovPropostasModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovPropostasModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public MovPropostasModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovPropostasModel
      {
        Id = dr.Field<long>("Id"),
        DataHoraCadastro = dr.Field<DateTime>("DataHoraCadastro"),
        DataHoraFechamento = dr.Field<DateTime>("DataHoraFechamento"),
        CadClientes = new CadClientesModel { Id = dr.Field<long>("Fk_CadClientes") },
        CadEquipe = new CadEquipeModel { Id = dr.Field<long>("Fk_CadEquipe") },
        CadProdutos = new CadProdutosModel { Id = dr.Field<long>("Fk_CadProdutos"), LimpaNome = dr.Field<int>("LimpaNome"), Descricao = dr.Field<string>("NomeProduto") },
        NumeroContrato = dr.Field<string>("NumeroContrato") ?? "",
        Observacao = dr.Field<string>("Observacao") ?? "",
        PercDesconto = dr.Field<decimal>("PercDesconto"),
        Situacao = ExtensionService.SafeConvert<byte>(dr["Situacao"]),
        Termometro = dr.Field<int>("Termometro"),
        ValorAprovado = dr.Field<decimal>("ValorAprovado"),
        ValorContrato = dr.Field<decimal>("ValorContrato"),
        ValorDivida = dr.Field<decimal>("ValorDivida"),
        ValorEntrada = dr.Field<decimal>("ValorEntrada"),
        ValorAvulso = dr.Field<decimal>("ValorAvulso")
      };
    }
    public MovPropostasModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovPropostasModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }

    public async Task<(long proposalId, MovCadClientesSerasaModel serasa)> NewProposal(long productID, string insc)
    {
      var query = await QuerySerasa(insc);
      if (query.Id == 0)
        return (0, query);

      var proposalID = await NewSerasaProposal(query.CadClientes.Id, productID);
      return (ExtensionService.SafeConvert<long>(proposalID, 0), query);
    }
    public async Task UpdateMovPropostas(long proposalId)
    {
      _dbContext.Parametros.AddItem("@p_MovPropostas", proposalId);
      await _dbContext.QueryAsync("UpdateMovPropostas", 1);
    }
    public async Task<MovCadClientesSerasaModel> QuerySerasa(string insc)
    {
      string markID = await _movCadClientesSerasaService.HandleRequestsSerasa(insc, "", "", true);
      if (markID == "401")
        return new MovCadClientesSerasaModel { StatusConsulta = 401 };

      var query = await _movCadClientesSerasaService.GetByMarkIdAsync(markID);
      return query;
    }
    public Task<PaginationModel> GetAllAsync(int page, int itensPerPage)
    {
      throw new NotImplementedException();
    }
    public async Task<object> GetDashboard(string dataInicial, string dataFinal, long vendedor, long productId)
    {
      long idUser = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));
      long idRole = Convert.ToInt64(AppIdentity.GetClaimValue("IdRole"));
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));

      _dbContext.Parametros.AddItem("@p_IdUser", idUser);
      _dbContext.Parametros.AddItem("@p_IdRole", idRole);
      _dbContext.Parametros.AddItem("@p_IdCompany", idCompany);
      _dbContext.Parametros.AddItem("@p_DataInicial", dataInicial);
      _dbContext.Parametros.AddItem("@p_DataFinal", dataFinal);
      _dbContext.Parametros.AddItem("@p_IdVendedor", vendedor);
      _dbContext.Parametros.AddItem("@p_ProductId", productId);
      DataSet result = await _dbContext.QueryAsync("PcDashboard", 1);
      return result.Tables;
    }
    public async Task<object> NewCustomSale(MovPropostasModel entity)
    {
      var data = entity.DataHoraCadastro.Date; // garante só a data (zera a hora)
      var hora = DateTime.Now.TimeOfDay;       // pega só a hora atual
      entity.DataHoraCadastro = data + hora;

      if (entity.Id == 0)
      {
        long idUser = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));
        entity.CadEquipe.Id = idUser;
        entity.ValorAprovado = entity.ValorContrato;
        entity.Situacao = 1;
        return await SaveAsync(entity);
      }
      else
      {
        return await UpdateAsync(entity);
      }
    }
  }
}