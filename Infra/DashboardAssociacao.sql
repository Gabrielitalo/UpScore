
DELIMITER $$
DROP PROCEDURE IF EXISTS DashboardAssociacao;

CREATE PROCEDURE DashboardAssociacao(
    IN p_DataInicial Datetime, 
    IN p_DataFinal Datetime)
BEGIN

Select Count(1) Into @TotalEmpresas
From CadEmpresas 
Where Ativo = 1;


Select C.Nome, Count(L.Id) Total
From CadEmpresas C 
Join CadEquipe Ce on (Ce.Fk_CadEmpresas = C.Id)
Left Join LogConsultas L on (L.Fk_CadEquipe = Ce.Id) and L.DataHoraConsulta between p_DataInicial and p_DataFinal
Group by C.Nome
Order by Count(L.Id) Desc;

Select C.Nome, Count(1) Total, Sum(TotalBeneficiarios) TotalBeneficiarios, Sum(Valor) Valor
From CadEmpresas C
Join MovLotesEmpresas M on (M.Fk_CadEmpresas = C.Id)
Where M.Data between p_DataInicial and p_DataFinal and 
Situacao = 1
Group by C.Nome
Order by Count(1) Desc;

END$$