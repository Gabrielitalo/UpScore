using RecomeceAPI.DTOs;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class MovPropostasDuplicatasService : IRepository<MovPropostasDuplicatasModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;
    private readonly MovPropostasService _movPropostasService;

    public MovPropostasDuplicatasService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService,
      MovPropostasService movPropostasService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
      _movPropostasService = movPropostasService;
    }
    public void AddIdParameter(string id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }
    public void AddEntityParameters(MovPropostasDuplicatasModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_CondPagamento", entity.CondPagamento);
      _dbContext.Parametros.AddItem("@p_DataVencimento", entity.DataVencimento);
      _dbContext.Parametros.AddItem("@p_Descricao", entity.Descricao);
      _dbContext.Parametros.AddItem("@p_Fk_MovPropostas", entity.MovPropostas.Id);
      _dbContext.Parametros.AddItem("@p_Indice", entity.Indice);
      _dbContext.Parametros.AddItem("@p_Valor", entity.Valor);

    }
    public async Task<object> SaveAsync(MovPropostasDuplicatasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into MovPropostasDuplicatas (Id, CondPagamento, DataVencimento, Descricao, Fk_MovPropostas, Indice, Valor) Select @p_Id, @p_CondPagamento, @p_DataVencimento, @p_Descricao, @p_Fk_MovPropostas, @p_Indice, @p_Valor; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(MovPropostasDuplicatasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostasDuplicatas
      Set 
      Id = @p_Id, 
      CondPagamento = @p_CondPagamento, 
      DataVencimento = @p_DataVencimento, 
      Descricao = @p_Descricao, 
      Fk_MovPropostas = @p_Fk_MovPropostas, 
      Indice = @p_Indice, 
      Valor = @p_Valor
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<MovPropostasDuplicatasModel> GetByIdAsync(string id)
    {
      string cmd = @"Select * From MovPropostasDuplicatas Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(long proposalId)
    {
      string cmd = @"Select * From MovPropostasDuplicatas Where Fk_MovPropostas = @p_MovPropostas Order by Indice;";
      _dbContext.Parametros.AddItem("@p_MovPropostas", proposalId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, 0, 50);
    }
    public async Task<bool> CheckPayments(long proposalId)
    {
      var response = await GetAllPropossalAsync(proposalId);
      if (response.Count > 0)
        return true;
      else
        return false;
    }
    public async Task<List<MovPropostasDuplicatasModel>> GetAllPropossalAsync(long proposalId)
    {
      string cmd = @"Select * From MovPropostasDuplicatas Where Fk_MovPropostas = @p_MovPropostas Order by Indice;";
      _dbContext.Parametros.AddItem("@p_MovPropostas", proposalId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return GetListByDataSet(result);
    }
    public async Task<object> DeleteAsync(string id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From MovPropostasDuplicatas Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public async Task<object> CleanProposal(long proposalId)
    {
      string cmd = @"Delete From MovPropostasDuplicatas Where Fk_MovPropostas = @p_MovPropostas;";
      _dbContext.Parametros.AddItem("@p_MovPropostas", proposalId);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<MovPropostasDuplicatasModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovPropostasDuplicatasModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public MovPropostasDuplicatasModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovPropostasDuplicatasModel
      {
        Id = dr.Field<string>("Id") ?? "",
        CondPagamento = ExtensionService.SafeConvert<byte>(dr["CondPagamento"]),
        DataVencimento = dr.Field<DateTime>("DataVencimento"),
        Descricao = dr.Field<string>("Descricao") ?? "",
        MovPropostas = new MovPropostasModel { Id = dr.Field<long>("Fk_MovPropostas") },
        Indice = ExtensionService.SafeConvert<byte>(dr["Indice"]),
        Valor = dr.Field<decimal>("Valor")
      };
    }
    public MovPropostasDuplicatasModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovPropostasDuplicatasModel();
    }

    public async Task<bool> IsExists(string id)
    {
      var item = await GetByIdAsync(id);
      return string.IsNullOrEmpty(item.Id);
    }
    public async Task<object> GeneratePayments(long proposalId, CondPagtoDTO dto)
    {
      if (dto == null)
        return NotificationService.Validation("Dados de pagamento não informados.");

      if (dto.ValorEntrada < 0)
        return NotificationService.Validation("Valor de entrada não pode ser negativo.");

      if (dto.ValorEntrada > 0 && dto.CondPagtoEntrada <= 0)
        return NotificationService.Validation("Condição de pagamento da entrada é obrigatória.");

      var proposal = await _movPropostasService.GetByIdAsync(proposalId);
      if (proposal.Id == 0)
        return NotificationService.Validation("Proposta não encontrada.");

      decimal valorTotal = proposal.ValorAprovado > 0 ? proposal.ValorAprovado : proposal.ValorContrato;

      if (dto.ValorEntrada > valorTotal)
        return NotificationService.Validation("Valor de entrada não pode ser maior que o valor total do contrato.");

      if (dto.ValorEntrada > 0 && dto.DataEntrada == DateTime.MinValue)
        return NotificationService.Validation("Data da entrada é obrigatória.");

      decimal valorFaltante = valorTotal - dto.ValorEntrada;

      await CleanProposal(proposalId);
      if (dto.ValorEntrada > 0)
      {
        // Grava a entrada, mesmo que seja zero
        var entry = new MovPropostasDuplicatasModel
        {
          Indice = 0,
          MovPropostas = proposal,
          CondPagamento = ExtensionService.SafeConvert<byte>(dto.CondPagtoEntrada),
          Valor = dto.ValorEntrada,
          DataVencimento = dto.DataEntrada,
          Descricao = "Entrada"
        };
        await SaveAsync(entry);
      }

      // Se o valor de entrada não for total, então parcela é obrigatória
      if (valorFaltante > 0)
      {
        if (dto.NumeroParcela <= 0)
          return NotificationService.Validation("Número de parcelas deve ser maior que zero se houver valor faltante.");

        if (dto.DataPrimeiraParcela == DateTime.MinValue)
          return NotificationService.Validation("Data da primeira parcela é obrigatória.");

        if (dto.CondPagtoParcela <= 0)
          return NotificationService.Validation("Condição de pagamento da parcela é obrigatória.");

        decimal payMonth = valorFaltante / dto.NumeroParcela;
        decimal totalParcelado = 0;

        for (int i = 0; i < dto.NumeroParcela; i++)
        {
          decimal valorParcela = Math.Round(valorFaltante / dto.NumeroParcela, 2);

          // Ajusta a última parcela para garantir que soma total = valorFaltante
          if (i == dto.NumeroParcela - 1)
            valorParcela = valorFaltante - totalParcelado;

          totalParcelado += valorParcela;

          var month = new MovPropostasDuplicatasModel
          {
            Indice = ExtensionService.SafeConvert<byte>(i + 1),
            MovPropostas = proposal,
            CondPagamento = ExtensionService.SafeConvert<byte>(dto.CondPagtoParcela),
            Valor = valorParcela,
            DataVencimento = dto.DataPrimeiraParcela.AddMonths(i),
            Descricao = $"Parcela {i + 1} de {dto.NumeroParcela}"
          };

          await SaveAsync(month);
        }

      }
      return NotificationService.Success();
    }

    public Task<object> DeleteAsync(long id)
    {
      throw new NotImplementedException();
    }

    public Task<MovPropostasDuplicatasModel> GetByIdAsync(long id)
    {
      throw new NotImplementedException();
    }

    public Task<bool> IsExists(long id)
    {
      throw new NotImplementedException();
    }

    public void AddIdParameter(long id)
    {
      throw new NotImplementedException();
    }

    public Task<PaginationModel> GetAllAsync(int page, int itensPerPage)
    {
      throw new NotImplementedException();
    }
  }
}