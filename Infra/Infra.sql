create database recomecedb;
use recomecedb;
SET GLOBAL log_bin_trust_function_creators = 1;
SET SQL_SAFE_UPDATES = 0;

Create Table if not exists Estados (
    Id BigInt NOT NULL AUTO_INCREMENT,
    CodigoIBGE Int ,
    UF Varchar(2),
    Nome Varchar(300),
    Capital varchar(300),
    PRIMARY KEY (Id)
);

Create Table if not exists Cidades (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_Estados BigInt ,
    CodigoIBGE int,
    NomeCidade Varchar(500),
    PRIMARY KEY (Id),
    CONSTRAINT Fk_Estados_CidadesIBGE FOREIGN KEY (Fk_Estados) REFERENCES Estados(Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists CadLicencas (
    Id BigInt NOT NULL AUTO_INCREMENT,
	Situacao int default 0 not null, -- 0 Inativo, 1 Ativo
    QtdUsuarios int,
	Valor Numeric(18,2),
    DataCriacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);

Create Table if not exists CadComissaoFranqueados (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Tipo tinyint   default 1, -- 1 Escritorio, 2 Individual
    Modo tinyint   default 1, -- 1 Normal, 2 Bonus
    Nome varchar(300) not null,
    Valor decimal(18,2),
    Comissao decimal(18,2) default 0,
    PRIMARY KEY (Id)
);

Create Table if not exists CadComissao (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Modo tinyint   default 1, -- 1 Normal, 2 Bonus
    Ativo tinyint   not null Default 1,
    Nome varchar(300) not null,
    Valor decimal(18,2),
    Comissao decimal(18,2) default 0,
    PRIMARY KEY (Id)
);

Create Table if not exists CadEmpresas (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_Cidades BigInt not null,
    IdEmpresaResponsavel BigInt 
    Ativo tinyint   not null Default 1,
    Tipo tinyInt not null default 1, -- 1 Escritorio, 2 Individual, 3 Associação
    Inscricao varchar(25) not null,
    Nome varchar(300) not null,
    NomeResponsavel varchar(300) not null,
    CpfResponsavel varchar(25),
    EmailResponsavel varchar(250),
    TelefoneResponsavel varchar(30),
    Logradouro Varchar(300),
    Numero Varchar(30),
    Bairro Varchar(150),
    Cep Varchar(20),
    Complemento Varchar(60),
    Servidor tinyint,
    BancoDados tinyint,
    IdAsaas Varchar(800),
    ChaveAPIAsaas Varchar(800),
    WhiteLabelConfig Longtext,
    CreatedOn datetime DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_Cidades_CadEmpresas FOREIGN KEY (Fk_Cidades) REFERENCES Cidades (Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists CadEmpresasContasBancarias (
    Id BigInt NOT NULL AUTO_INCREMENT,
    Fk_CadEmpresas BigInt not null,
    Ativo tinyint  default 1 not null,
    Padrao tinyint  default 0 not null,
    CodBanco varchar(10),
    Agencia varchar(20),
    Conta varchar(30),
    ChavePix varchar(1000),
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEmpresas_CadEmpresasContasBancarias FOREIGN KEY (Fk_CadEmpresas) REFERENCES CadEmpresas (Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists CadConsultas (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(60),
    Descricao VARCHAR(300),
    DataInicial Datetime,
    DataFinal Datetime,
    Custo decimal(10, 2),
    Venda decimal(10, 2),
    ProductURL VARCHAR(600),
    PRIMARY KEY (Id)
);


Insert Into CadConsultas(Nome, Descricao, DataInicial, Custo, Venda, Tipo)
Select 'Relatório Avançado PF', '', Now(), 5.53, 7.9, 1;
Insert Into CadConsultas(Nome, Descricao, DataInicial, Custo, Venda, Tipo)
Select 'Relatório Avançado PJ', '', Now(), 8.52, 8.9, 1;
Insert Into CadConsultas(Nome, Descricao, DataInicial, Custo, Venda, Tipo)
Select 'Relatório Avançado PF + SPC', '', Now(), 8.13, 10.9, 1;
Insert Into CadConsultas(Nome, Descricao, DataInicial, Custo, Venda, Tipo)
Select 'Relatório Avançado PJ + SPC', '', Now(), 11.52, 12.9, 1;
Insert Into CadConsultas(Nome, Descricao, DataInicial, Custo, Venda, ProductURL)
Select 'Boa Vista PF', '', Now(), 3, 4, '';
Insert Into CadConsultas(Nome, Descricao, DataInicial, Custo, Venda, ProductURL)
Select 'Boa Vista PJ', '', Now(), 3, 4, '';

Update CadConsultas Set ProductURL = 'optionalFeatures=COMPROMETIMENTO_RENDA' Where Id = 1;
Update CadConsultas Set ProductURL = 'optionalFeatures=FATURAMENTO_ESTIMADO_POSITIVO' Where Id = 2;
Update CadConsultas Set ProductURL = 'optionalFeatures=ANOTACOES_CONSULTAS_SPC,COMPROMETIMENTO_RENDA' Where Id = 3;
Update CadConsultas Set ProductURL = 'optionalFeatures=ANOTACOES_CONSULTAS_SPC,FATURAMENTO_ESTIMADO_POSITIVO' Where Id = 4;

Update CadConsultas Set Custo = 5.53 + 0.62  Where Id = 1;
Update CadConsultas Set Custo = 8.52  Where Id = 2;
Update CadConsultas Set Custo = 5.53 + 0.62 + 2.60  Where Id = 3;
Update CadConsultas Set Custo = 8.52 + 0.62 + 3  Where Id = 4;

Update CadConsultas Set Nome = 'Serasa Relatório Avançado CPF (PF)' Where Id = 1;
Update CadConsultas Set Nome = 'Serasa Relatório Avançado CNPJ (PJ)' Where Id = 2;
Update CadConsultas Set Nome = 'Serasa Relatório Avançado CPF + SPC (PF)' Where Id = 3;
Update CadConsultas Set Nome = 'Serasa Relatório Avançado CNPJ + SPC (PJ)' Where Id = 4;
Update CadConsultas Set Nome = 'Boa Vista CPF (PF)' Where Id = 5;
Update CadConsultas Set Nome = 'Boa Vista CNPJ (PJ)' Where Id = 6;
Update CadConsultas Set Nome = 'Serasa Relatório Básico CPF (PF)' Where Id = 7;
Update CadConsultas Set Nome = 'Serasa Relatório Básico CNPJ (PJ)' Where Id = 8;


Create Table if not exists CadProdutos (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_CadEmpresas BigInt not null,
    Fk_CadConsultas BigInt,
    Situacao tinyint   default 1,
	LimpaNome INT NULL DEFAULT 1,
    DataInicial datetime,
    DataFinal datetime default null,
    Codigo varchar(13),
    Descricao varchar(60),
    Custo decimal(10,2) default 0,
    TipoConsulta int default 1, -- 1 Avançado, 2 Básico 
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEmpresas_CadProdutos FOREIGN KEY (Fk_CadEmpresas) REFERENCES CadEmpresas (Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists CadProdutosFaixas (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_CadProdutos BigInt  not null,
    ValorDivida decimal(18,2) default 0,
    ValorServico decimal(18,2) default 0,
    ValorEntradaMinima decimal(18,2) default 0,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadProdutos_CadProdutosFaixas FOREIGN KEY (Fk_CadProdutos) REFERENCES CadProdutos (Id) ON DELETE No Action ON UPDATE CASCADE
);

Create Table if not exists CadProdutosFaixasPagamento (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_CadProdutosFaixas BigInt  not null,
    Tipo tinyint   default 1, -- 1 A vista, 2 Parcelado
    Modo tinyint   default 1, -- 1 Boleto, 2 Cartão de Crédito, 3 Pix
    ValorMaxParcelas tinyint   default 1, 
    Descricao varchar(300),
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadProdutosFaixas_CadProdutosFaixasPagamento FOREIGN KEY (Fk_CadProdutosFaixas) REFERENCES CadProdutosFaixas (Id) ON DELETE No Action ON UPDATE CASCADE
);

Create Table if not exists MovConsultasCompradas (
    Id BigInt NOT NULL AUTO_INCREMENT,
    Fk_CadEmpresas BigInt not null,
    Ativo tinyint default 1 not null, -- 0 Inativo, 1 Ativo
	Situacao int default 0 not null, -- 0 Gerado, 1 Pago
	QtdConsultas int,
	Valor Numeric(18,2),
	IdExtPagto VARCHAR(255),
    DataCriacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEmpresas_MovConsultasCompradas FOREIGN KEY (Fk_CadEmpresas) REFERENCES CadEmpresas (Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists CadEquipe (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_CadEmpresas BigInt not null,
    Ativo tinyint not null Default 1,
    Tipo tinyInt not null default 1, -- 0 Adm global, 1 Adm local, 2 vendedor, 3 financeiro
    Nome varchar(300) not null,
    Inscricao varchar(25),
    Email varchar(250),
    Telefone varchar(30),
    Logradouro Varchar(300),
    Numero Varchar(30),
    Bairro Varchar(150),
    Cep Varchar(20),
    Complemento Varchar(100),
    Senha varchar(800) not null,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEmpresas_CadEquipe FOREIGN KEY (Fk_CadEmpresas) REFERENCES CadEmpresas (Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists CadComissaoVendedores (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_CadEquipe BigInt  not null,
    Fk_CadComissao BigInt  not null,
    Ativo tinyint   not null Default 1,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEquipe_CadComissaoVendedores FOREIGN KEY (Fk_CadEquipe) REFERENCES CadEquipe (Id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT Fk_CadComissao_CadComissaoVendedores FOREIGN KEY (Fk_CadComissao) REFERENCES CadComissao (Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists CadClientes (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_CadEquipe BigInt  not null,
	Fk_Cidades Bigint BIGINT NULL DEFAULT 5562,
    Ativo tinyint   not null Default 1,
    Tipo tinyInt not null default 1,
    Inscricao varchar(45),
    Nome varchar(300) not null,
    Email varchar(250),
    Telefone varchar(30),
    Logradouro Varchar(300),
    Numero Varchar(30),
    Bairro Varchar(150),
    Cep Varchar(20),
    Complemento Varchar(100),
    Senha varchar(800) not null,
    EstadoCivil Varchar(100),
    Nacionalidade Varchar(100),
    Profissão Varchar(120),
    CreatedOn datetime DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEquipe_CadClientes FOREIGN KEY (Fk_CadEquipe) REFERENCES CadEquipe (Id) ON DELETE CASCADE ON UPDATE CASCADE
);

Create Table if not exists MovCadClientesSerasa (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_CadEquipe BigInt not null,
    Fk_CadClientes BigInt 
    Tipo tinyInt ,
	MarkID varchar(36),
    Situacao tinyInt  default 1, -- 1 Sucesso, 2 Falha 
    DataHoraConsulta Datetime,
    PendenciasComerciaisPefin Numeric(18,2),
    PendenciasBancariasRefin Numeric(18,2),
    ChequesSemFundos Numeric(18,2),
    Protestos Numeric(18,2),
    AcoesJudiciais Numeric(18,2),
    PartFalencias Numeric(18,2),
    DividasVencidas Numeric(18,2),
    Concordatas Numeric(18,2),
    AnotacoesSPC Numeric(18,2),
    Score Int,
    TotalNegativacao Numeric(18,2),
    ArquivoEnvio Varchar(2000),
    ArquivoRetorno Longtext,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEquipe_MovCadClientesSerasa FOREIGN KEY (Fk_CadEquipe) REFERENCES CadEquipe (Id) ON DELETE No Action ON UPDATE CASCADE,
    CONSTRAINT Fk_CadClientes_MovCadClientesSerasa FOREIGN KEY (Fk_CadClientes) REFERENCES CadClientes (Id) ON DELETE No Action ON UPDATE CASCADE
);

Create Table if not exists CadOrigens (
    Id Int NOT NULL AUTO_INCREMENT,
    Nome Varchar(300),
    PRIMARY KEY (Id)
);

Create Table if not exists MovPropostas (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_CadEquipe BigInt not null,
    Fk_CadProdutos BigInt 
    Fk_CadClientes BigInt 
    Situacao tinyInt , -- 1 Gerado, 2 Em Negociação, 3 Aprovado, 4 Recusado
    Termometro int default 1, -- 1 Frio, 2 Morno, 3 Quente
    NumeroContrato Varchar(60),
    ValorDivida Numeric(18,2),
    ValorContrato Numeric(18,2),
    ValorAprovado Numeric(18,2),
    ValorEntrada Numeric(18,2),
    PercDesconto Numeric(18,2),
    DataHoraCadastro Datetime,
    DataHoraFechamento Datetime,
    Observacao Varchar(800),
	ValorAvulso Numeric(18,2),
    CreatedOn datetime DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEquipe_MovPropostas FOREIGN KEY (Fk_CadEquipe) REFERENCES CadEquipe (Id) ON DELETE No Action ON UPDATE CASCADE,
    CONSTRAINT Fk_CadClientes_MovPropostas FOREIGN KEY (Fk_CadClientes) REFERENCES CadClientes (Id) ON DELETE No Action ON UPDATE CASCADE,
    CONSTRAINT Fk_CadProdutos_MovPropostas FOREIGN KEY (Fk_CadProdutos) REFERENCES CadProdutos (Id) ON DELETE No Action ON UPDATE CASCADE
);

Create Table if not exists MovPropostasBeneficiarios (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_CadClientes BigInt 
    Fk_MovPropostas BigInt 
    Score int,
    ValorDivida Numeric(18,2),
    ValorContrato Numeric(18,2),
    Tipo tinyInt , -- 1 Automatico, 2 Manual
    Observacao Varchar(800),
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadClientes_MovPropostasBeneficiarios FOREIGN KEY (Fk_CadClientes) REFERENCES CadClientes (Id) ON DELETE No Action ON UPDATE CASCADE,
    CONSTRAINT Fk_MovPropostas_MovPropostasBeneficiarios FOREIGN KEY (Fk_MovPropostas) REFERENCES MovPropostas (Id) ON DELETE No Action ON UPDATE CASCADE
);

Create Table if not exists MovPropostasDuplicatas (
    Id varchar(36) NOT NULL,
    Fk_MovPropostas BigInt,
    Indice tinyInt,
    CondPagamento tinyInt, -- 1 Boleto, 2 Cartão de Crédito, 3 Pix
    DataVencimento Datetime,
    Descricao varchar(1000),
    Valor Numeric(18,2),
	IdExtPagto VARCHAR(255),
    Situacao int default 1,
    DataCriacaoExtPagto Datetime, 
    PRIMARY KEY (Id),
    CONSTRAINT Fk_MovPropostas_MovPropostasDuplicatas FOREIGN KEY (Fk_MovPropostas) REFERENCES MovPropostas (Id) ON DELETE No Action ON UPDATE CASCADE
);

Create Table if not exists MovPropostasArquivos (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_MovPropostas BigInt,
    Fk_MovPropostasBeneficiarios BigInt,
    Tipo tinyInt, -- 1 Contrato, 2 - Ficha Finananceira
    Situacao tinyInt  default 1, -- 1 Gerado, 2 Conferido, 3 Pendente 
    DataCriacao Datetime,
    DataConferencia Datetime,
    Observacao Varchar(800),
    NomeArquivo Varchar(300),
    ArquivoBytes MEDIUMBLOB,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_MovPropostas_MovPropostasArquivos FOREIGN KEY (Fk_MovPropostas) REFERENCES MovPropostas (Id) ON DELETE No Action ON UPDATE CASCADE
);

Create Table if not exists MovContratos (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_MovPropostas BigInt,
    Situacao tinyInt,
	DataCriacao Datetime,
    DataFechamento Datetime,
    PRIMARY KEY (Id),
    CONSTRAINT Fk_MovPropostas_MovContratos FOREIGN KEY (Fk_MovPropostas) REFERENCES MovPropostas (Id) ON DELETE No Action ON UPDATE CASCADE
);

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

Create Table if not exists MovAuditoria (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    TipoAcao tinyInt , -- 1 Inclusão, 2 Alteração, 3 Exclusão
    TipoResponsavel tinyInt  default 1, -- 1 CadEquipe, 2 CadClientes
    IdResponsavel Bigint 
    DataInicial	Datetime,
    IdTabela Bigint 
    NomeTabela Varchar(100),
    Texto Varchar(8000),
    Observacao Varchar(800),
    PRIMARY KEY (Id)
);

Create Table if not exists LogAsaas(
    Id BigInt  NOT NULL AUTO_INCREMENT,
    DataHoraConsulta Datetime,
	TipoEvento varchar(60),
    ArquivoRetorno Longtext,
    PRIMARY KEY (Id)
);

Create Table if not exists LogSerasaConsultas (
    Id BigInt  NOT NULL AUTO_INCREMENT,
    Fk_CadEquipe BigInt null,
	Inscricao varchar(30),
	MarkID varchar(36),
    Situacao tinyInt  default 1, -- 1 Sucesso, 2 Falha 
    DataHoraConsulta Datetime,
    ArquivoEnvio Varchar(2000),
    ArquivoRetorno Longtext,
    PRIMARY KEY (Id)
);

Create Table if not exists MovContaCorrente (
    Id VARCHAR(36) NOT NULL, -- GUID
    Fk_CadEmpresas BigInt not null,
    Fk_Origem VARCHAR(36),
    OrigemTipo int,
	Situacao int default 0 not null, -- 0 Gerado, 1 Pago
    Tipo int, -- 1 Débito, 2 Crédito
	Valor Numeric(18,2),
	IdExtPagto VARCHAR(255),
    DataCriacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    DataPagamento DATETIME,
    Historico VARCHAR(500),
    PRIMARY KEY (Id),
    CONSTRAINT Fk_CadEmpresas_MovContaCorrente FOREIGN KEY (Fk_CadEmpresas) REFERENCES CadEmpresas (Id) ON DELETE CASCADE ON UPDATE CASCADE
);


Create Table if not exists LogConsultas (
    Id VARCHAR(36) NOT NULL,
    Fk_CadEquipe BigInt null,
    Fk_CadConsultas BigInt null,
	Legado int default 0,
	Inscricao varchar(30),
	MarkID varchar(36),
    Situacao tinyInt  default 1, -- 1 Sucesso, 2 Falha 
    DataHoraConsulta Datetime,
    Score decimal(10, 2),
    TotalNegativacao decimal(18, 2),
    ArquivoEnvio Varchar(2000),
    ArquivoRetorno Longtext,
    PRIMARY KEY (Id)
);

{
  "id": 0,
  "ativo": 1,
  "bairro": "Meireles",
  "bancoDados": "1",
  "cep": "60160-230",
  "chaveAPIAsaas": "$aact_MzkwODA2MWY2OGM3MWRlMDU2NWM3MzJlNzZmNGZhZGY6OjQzYTYzNTcyLTQ2NmYtNDFlMC05M2ZiLWZkYWU2MDNmMmNmZTo6JGFhY2hfZGI5MzdlOGMtZjI1ZS00ZjhkLWI2MTEtZDRkNjQ0NjlhYTQ2",
  "complemento": "loja 321",
  "cpfResponsavel": "",
  "emailResponsavel": "brunosouza@recomecebrasil.com.br",
  "cidades": {
    "id": 1194
  },
  "idAsaas": "",
  "idEmpresaResponsavel": 0,
  "inscricao": "46.569.656/0001-20",
  "logradouro": "Avenida Dom Luís",
  "nome": "RECOMECE NORDESTE REABILITAÇÃO DE CRÉDITO LTDA",
  "nomeResponsavel": "BRUNO RAFAEL DE SOUZA",
  "numero": "300",
  "servidor": "1",
  "telefoneResponsavel": "85999708185",
  "tipo": 1
}

{
  "id": 0,
  "ativo": 1,
  "bairro": "Belvedere",
  "cep": "35501320",
  "complemento": "Casa",
  "email": "gabrielitaloalv@gmail.com",
  "cadEmpresas": {
    "id": 1
  },
  "inscricao": "019.274.116-02",
  "logradouro": "Rua Januário de Souza Rocha",
  "nome": "Gabriel Italo Alves de Oliveira",
  "numero": "215",
  "senha": "1234",
  "telefone": "(37)98854-5038",
  "tipo": 1
}