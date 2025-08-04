USE upscore;
SET GLOBAL log_bin_trust_function_creators = 1;
SET SQL_SAFE_UPDATES = 0;

Select *
From CadEmpresas;

Select *
From CadEquipe;

Set @EmpresaId = 4;

Update CadEmpresas
Set IdEmpresaResponsavel = 0
Where Id = @EmpresaId;

Insert Into CadEquipe(Fk_CadEmpresas, Ativo, Tipo, Nome, Inscricao, Email, Telefone, Logradouro, Numero, Bairro, Cep, Complemento, Senha)
Select Id, 1, 4, NomeResponsavel, CpfResponsavel, EmailResponsavel, '', '', '', '', '', '', 'SlHMkI+dUfsZATOLc/7NpA=='
From CadEmpresas
Where Id = @EmpresaId;
