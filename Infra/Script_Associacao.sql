-- Executado no banco da up
Insert Into CadEmpresas(Tipo, Ativo, Nome, Inscricao, IdAsaas, Fk_Cidades, Servidor, BancoDados, NomeResponsavel, CpfResponsavel, EmailResponsavel)
Select 3, 1, 'ABRC-ASSOCIACAO BRASILEIRA DE REABILITACAO DE CREDITO', '58696244000130', 'cus_000124754609', 4340, 1, 1, 'LEONARDO DE MACEDO MALHEIROS', '04261374935', 'abrc@abrc.com';


-- Executado no banco da up
Update CadEmpresas
Set IdEmpresaResponsavel = 0
Where Id = 13;

-- Executado no banco da up
Set @EmpresaId = 13;

Insert Into CadEquipe(Fk_CadEmpresas, Ativo, Tipo, Nome, Inscricao, Email, Telefone, Logradouro, Numero, Bairro, Cep, Complemento, Senha)
Select Id, 1, 5, NomeResponsavel, CpfResponsavel, EmailResponsavel, '', '', '', '', '', '', 'SlHMkI+dUfsZATOLc/7NpA=='
From CadEmpresas
Where Id = @EmpresaId;

-- Executado no banco da up
Drop Table CadRenovacaoLicencas;
Drop Table CadEmpresasLicencas;
Drop Table CadLicencas;


Create Table if not exists CadLicencas (
    Id BigInt NOT NULL AUTO_INCREMENT,
	Situacao int default 0 not null, -- 0 Inativo, 1 Ativo
    QtdUsuarios int,
	Valor Numeric(18,2),
    DataCriacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);


Drop Table MovLotesEmpresasContratos;
Drop Table MovLotesEmpresasBeneficiarios;
Drop Table MovLotesEmpresas;


Create Table if not exists MovLotesEmpresas (
    Id VARCHAR(36) NOT NULL,
    Fk_CadEmpresas BigInt not null,
    Tipo int default 1, -- 1 Envio, 2 Reenvio
    Situacao int,
    Data datetime,
    TotalBeneficiarios int,
    Valor Numeric(18,2),
	IdExtPagto VARCHAR(255),
    Serasa int default 0,
    Boavista int default 0,
    SPC int default 0,
    CenprotSP int default 0,
    CenprotNacional int default 0,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEmpresas_MovLotesEmpresas FOREIGN KEY (Fk_CadEmpresas) REFERENCES CadEmpresas (Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists MovLotesEmpresasBeneficiarios (
    Id VARCHAR(36) NOT NULL,
    Fk_MovLotesEmpresas VARCHAR(36) not null,
    Inscricao varchar(45),
    Nome varchar(300) not null,
    Situacao int,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_MovLotesEmpresas_MovLotesEmpresasBeneficiarios FOREIGN KEY (Fk_MovLotesEmpresas) REFERENCES MovLotesEmpresas (Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists MovLotesEmpresasContratos (
    Id BigInt NOT NULL AUTO_INCREMENT,
    Fk_MovLotesEmpresas VARCHAR(36) not null,
    Fk_MovContratos BigInt not null,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_MovLotesEmpresas_MovLotesEmpresasContratos FOREIGN KEY (Fk_MovLotesEmpresas) REFERENCES MovLotesEmpresas (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT Fk_MovContratos_MovLotesEmpresasContratos FOREIGN KEY (Fk_MovContratos) REFERENCES MovContratos (Id) ON DELETE No Action ON UPDATE CASCADE
);

Create Table if not exists MovLotesAssociacao (
    Id VARCHAR(36) NOT NULL,
    Fk_CadEmpresas BigInt not null,
    Situacao int,
    Data datetime,
    TotalBeneficiarios int,
    ValorTotal numeric(10,2),
    Serasa int default 0,
    Boavista int default 0,
    SPC int default 0,
    CenprotSP int default 0,
    CenprotNacional int default 0,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEmpresas_MovLotesAssociacao FOREIGN KEY (Fk_CadEmpresas) REFERENCES CadEmpresas (Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists MovLotesAssociacaoComponentes (
    Fk_MovLotesAssociacao VARCHAR(36) not null,
    Fk_MovLotesEmpresas VARCHAR(36) not null,
    CONSTRAINT Fk_MovLotesAssociacao_MovLotesAssociacaoComponentes FOREIGN KEY (Fk_MovLotesAssociacao) REFERENCES MovLotesAssociacao (Id) ON DELETE NO ACTION ON UPDATE CASCADE,
    CONSTRAINT Fk_MovLotesEmpresas_MovLotesAssociacaoComponentes FOREIGN KEY (Fk_MovLotesEmpresas) REFERENCES MovLotesEmpresas (Id) ON DELETE NO ACTION ON UPDATE CASCADE
);