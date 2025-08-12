
DELIMITER $$
DROP PROCEDURE IF EXISTS PcDashboard;

CREATE PROCEDURE PcDashboard(
    IN p_IdUser BIGINT,
    IN p_IdRole BIGINT,
    IN p_IdCompany BIGINT,
    IN p_IdVendedor BIGINT,
    IN p_ProductId BIGINT,
    IN p_DataInicial Datetime, 
    IN p_DataFinal Datetime)
BEGIN

Set @Modulo = 'Dashboard';
Set @DataAtual = Now();
Set @DataInicialTrimestre = DATE_SUB(p_DataInicial, INTERVAL 3 MONTH);
Set @DataInicialMesAntetior = DATE_SUB(p_DataInicial, INTERVAL 1 MONTH);
Set @DataFinalMesAntetior = DATE_SUB(p_DataFinal, INTERVAL 1 MONTH);

    Drop Table If Exists Tp_Equipe;
    CREATE TEMPORARY TABLE IF NOT EXISTS Tp_Equipe (
      Id BIGINT
    );

    Drop Table If Exists Tp_Produtos;
    CREATE TEMPORARY TABLE IF NOT EXISTS Tp_Produtos (
      Id BIGINT
    );
  
    If (p_IdVendedor > 0 and p_IdUser <> 159) Then
      Insert Into Tp_Equipe Select p_IdVendedor;
    End If;

    If ((p_IdVendedor = 0 and p_IdRole in (0, 1)) or p_IdUser = 159) Then
      Insert Into Tp_Equipe 
      Select Ce.Id
      From CadEquipe Ce
      Where (Ce.Fk_CadEmpresas = p_IdCompany);
    End If;

    If (p_IdVendedor = 0 and p_IdRole > 1) Then
      Insert Into Tp_Equipe 
      Select Ce.Id
      From CadEquipe Ce
      Where (Ce.Id = p_IdUser);
    End If;

    If (p_ProductId > 0) Then
      Insert Into Tp_Produtos Select p_ProductId;
    End If;

    If (p_ProductId = 0 and p_IdRole in (0, 1)) Then
      Insert Into Tp_Produtos 
      Select Id 
      From CadProdutos 
      Where Fk_CadEmpresas = p_IdCompany and Situacao = 1;
    End If;

    If(p_IdUser = 159) Then 
      Delete From Tp_Produtos;
      Insert Into Tp_Produtos Select 7;
    End If;

DROP TABLE IF EXISTS Retorno;
-- --------------------------------------------------------------------------------------------------------------------------
-- Buscando propostas
Drop Table If Exists Tp_MovPropostas;
CREATE TEMPORARY TABLE IF NOT EXISTS Tp_MovPropostas (
    Id bigint,
    Fk_CadEquipe bigint, 
    Fk_CadProdutos bigint, 
    Fk_CadClientes bigint, 
    Situacao tinyint, 
    Termometro int, 
    ValorDivida decimal(18,2), 
    ValorContrato decimal(18,2), 
    ValorAprovado decimal(18,2), 
    ValorEntrada decimal(18,2), 
    DataHoraCadastro datetime, 
    DataHoraFechamento datetime,
    Fk_CadOrigens int, 
    ValorAvulso decimal(18,2)
);

Insert Into Tp_MovPropostas
Select M.Id, M.Fk_CadEquipe, M.Fk_CadProdutos, M.Fk_CadClientes, M.Situacao, M.Termometro, M.ValorDivida, M.ValorContrato,
M.ValorAprovado, M.ValorEntrada, M.DataHoraCadastro, M.DataHoraFechamento, M.Fk_CadOrigens, M.ValorAvulso
From MovPropostas M
Join Tp_Equipe Te on (Te.Id = M.Fk_CadEquipe)
Join Tp_Produtos Tp on (Tp.Id = M.Fk_CadProdutos)
Join CadEquipe Ce on (Ce.Id = Te.Id)
Where (Ce.Fk_CadEmpresas = p_IdCompany) and  
((M.DataHoraCadastro between p_DataInicial and p_DataFinal) or ((M.Situacao >= 3) and (M.DataHoraFechamento between p_DataInicial and p_DataFinal)));

-- Caso não seja adm tem que manter somente o usuário corrente
If (p_IdRole > 1 and p_IdUser <> 159) Then 
    Delete From Tp_MovPropostas Where Fk_CadEquipe <> p_IdUser;
End If;
-- --------------------------------------------------------------------------------------------------------------------------
-- Buscando contratos
Drop Table If Exists Tp_Contratos;
CREATE TEMPORARY TABLE IF NOT EXISTS Tp_Contratos (
    Id bigint,
    Fk_MovPropostas bigint, 
    Situacao tinyint, 
    DataCriacao datetime, 
    DataFechamento datetime
);

Insert Into Tp_Contratos
Select Mc.Id, Mc.Fk_MovPropostas, Mc.Situacao, Mc.DataCriacao, Mc.DataFechamento
From MovContratos Mc 
Where ((Mc.DataCriacao between p_DataInicial and p_DataFinal) or ((Mc.Situacao >= 3) and (Mc.DataFechamento between p_DataInicial and p_DataFinal)));

-- --------------------------------------------------------------------------------------------------------------------------
-- Totalizador geral

Select Count(1), Sum(ValorAprovado) Into @TotalPropostas, @ValorTotalPropostas From Tp_MovPropostas;
Select Count(1), Sum(ValorAprovado) Into @TotalPropostasFechadas, @ValorTotalPropostasFechadas From Tp_MovPropostas Where (Situacao = 3);
Select Count(1), Sum(ValorAprovado) Into @TotalPropostasRecusadas, @ValorTotalPropostasRecusadas From Tp_MovPropostas Where (Situacao = 4);

Select Count(1), Sum(Tp.ValorAprovado) Into @TotalContratos, @ValorTotalContratos From Tp_Contratos Tc Join Tp_MovPropostas Tp on (Tp.Id = Tc.Fk_MovPropostas);
Select Count(1), Sum(Tp.ValorAprovado) Into @TotalContratosFechadas, @ValorTotalContratosFechadas From Tp_Contratos Tc Join Tp_MovPropostas Tp on (Tp.Id = Tc.Fk_MovPropostas) Where (Tc.Situacao = 3);
Select Count(1), Sum(Tp.ValorAprovado) Into @TotalContratosRecusadas, @ValorTotalContratosRecusadas From Tp_Contratos Tc Join Tp_MovPropostas Tp on (Tp.Id = Tc.Fk_MovPropostas) Where (Tc.Situacao = 4);

Select @TotalPropostas TotalPropostas, @TotalPropostasFechadas TotalPropostasFechadas, @TotalPropostasRecusadas TotalPropostasRecusadas,
@ValorTotalPropostas ValorTotalPropostas, @ValorTotalPropostasFechadas ValorTotalPropostasFechadas, @ValorTotalPropostasRecusadas ValorTotalPropostasRecusadas,
@TotalContratos TotalContratos, @TotalContratosFechadas TotalContratosFechadas, @TotalContratosRecusadas TotalContratosRecusadas,
@ValorTotalContratos ValorTotalContratos, @ValorTotalContratosFechadas ValorTotalContratosFechadas, @ValorTotalContratosRecusadas ValorTotalContratosRecusadas;

-- --------------------------------------------------------------------------------------------------------------------------
-- Totalizando as propostas

Select 
Situacao,
Case 
    When Situacao = 1 Then 'Gerado'
    When Situacao = 2 Then 'Em Negociação'
    When Situacao = 3 Then 'Aprovado'
    When Situacao = 4 Then 'Recusado'
Else
    'Sem Categoria'
End SituacaoDesc,
Count(1) Total,
Sum(ValorAprovado) ValorAprovado,
Sum(ValorDivida) ValorDivida
From Tp_MovPropostas
Group by Situacao
Order by Situacao Asc;
-- --------------------------------------------------------------------------------------------------------------------------
-- Totalizando contratos

Select 
Tc.Situacao,
Case 
    When Tc.Situacao = 1 Then 'Gerado'
    When Tc.Situacao = 2 Then 'Análise'
    When Tc.Situacao = 3 Then 'Aprovado'
    When Tc.Situacao = 4 Then 'Recusado'
Else
    'Sem Categoria'
End SituacaoDesc,
Count(1) Total,
Sum(Tp.ValorAprovado) ValorAprovado,
Sum(Tp.ValorDivida) ValorDivida
From Tp_Contratos Tc
Join Tp_MovPropostas Tp on (Tp.Id = Tc.Fk_MovPropostas)
Group by Tc.Situacao
Order by Tc.Situacao Asc;
-- --------------------------------------------------------------------------------------------------------------------------
-- Ranking de vendedores

Select Ce.Nome, Count(1) Total,
Sum(Tp.ValorAprovado) ValorAprovado
From Tp_Contratos Tc
Join Tp_MovPropostas Tp on (Tp.Id = Tc.Fk_MovPropostas)
Join CadEquipe Ce on (Ce.Id = Tp.Fk_CadEquipe)
Where (Tc.Situacao = 3)
Group by Nome
Order by Sum(Tp.ValorAprovado) Desc;


-- Ranking de dianósticos feitos
Select Ce.Nome, Count(1) Total,
Sum(Tp.ValorAvulso) ValorAvulso
From Tp_MovPropostas Tp 
Join CadEquipe Ce on (Ce.Id = Tp.Fk_CadEquipe)
Where Coalesce(Tp.ValorAvulso, 0) > 0
Group by Ce.Nome
Order by Sum(Tp.ValorAvulso) Desc;

-- Ranking de origem de vendas
Select Tp.Fk_CadOrigens, Coalesce(Co.Nome, 'Sem Categoria') Nome, Count(1) Total,
Sum(Tp.ValorAprovado) ValorAprovado
From Tp_Contratos Tc
Join Tp_MovPropostas Tp on (Tp.Id = Tc.Fk_MovPropostas)
Left Join CadOrigens Co on (Co.Id = Tp.Fk_CadOrigens)
Where (Tc.Situacao = 3)
Group by Tp.Fk_CadOrigens, Co.Nome
Order by Sum(Tp.ValorAprovado) Desc;


DROP TABLE IF EXISTS Tp_MovPropostas;
DROP TABLE IF EXISTS Tp_Contratos;

END$$