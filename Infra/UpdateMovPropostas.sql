DELIMITER $$
DROP PROCEDURE IF EXISTS UpdateMovPropostas;

CREATE PROCEDURE UpdateMovPropostas(IN p_MovPropostas Int)
BEGIN

Set @TotalDivida = 0; 
Set @TotalContrato = 0;

Select Sum(ValorDivida), Sum(ValorContrato) Into @TotalDivida, @TotalContrato 
From MovPropostasBeneficiarios
Where Fk_MovPropostas = p_MovPropostas;

Select @TotalDivida Divida, @TotalContrato Contrato;

Update MovPropostas
Set ValorDivida = @TotalDivida,
ValorContrato = @TotalContrato
Where Id = p_MovPropostas;

Update MovPropostas
Set ValorAprovado = @TotalContrato
Where Id = p_MovPropostas and ValorAprovado = 0;

END$$