ALTER TABLE CadConsultas
ADD COLUMN `Tipo` INT NULL DEFAULT 1 AFTER `ProductURL`;


Insert Into CadConsultas(Nome, Descricao, DataInicial, Custo, Venda, ProductURL, Tipo)
Select 'Boa Vista PF', 'Acerta Essencial Positivo', Now(), 3, 4, '', 2;
Insert Into CadConsultas(Nome, Descricao, DataInicial, Custo, Venda, ProductURL, Tipo)
Select 'Boa Vista PJ', 'Define Risco Positivo', Now(), 3, 4, '', 2;

Insert Into CadConsultas(Nome, Descricao, DataInicial, Custo, Venda, ProductURL, Tipo)
Select 'Relatório Básico PF', 'Identificação Cadastral, Participações sociais, Dívidas com bancos e empresas (Pefin/Refin), Cheques devolvidos (sem fundo, sustado e cancelado), Protestos', Now(), 0.54, 4, '', 1;
Insert Into CadConsultas(Nome, Descricao, DataInicial, Custo, Venda, ProductURL, Tipo)
Select 'Relatório Básico PJ', 'Identificação Cadastral, Participações sociais, Dívidas com bancos e empresas (Pefin/Refin), Cheques devolvidos (sem fundo, sustado e cancelado), Protestos', Now(), 0.55, 4, '', 1;

--Atualizar a GetLogSerasaConsultas

Update CadConsultas
Set ProductURL = 'person-information-report/v1/creditreport?reportName=RELATORIO_AVANCADO_TOP_SCORE_PF_PME&optionalFeatures=COMPROMETIMENTO_RENDA'
Where Id = 1;

Update CadConsultas
Set ProductURL = 'business-information-report/v1/reports?reportName=RELATORIO_AVANCADO_PJ_PME&optionalFeatures=FATURAMENTO_ESTIMADO_POSITIVO'
Where Id = 2;

Update CadConsultas
Set ProductURL = 'person-information-report/v1/creditreport?reportName=RELATORIO_AVANCADO_TOP_SCORE_PF_PME&optionalFeatures=ANOTACOES_CONSULTAS_SPC,COMPROMETIMENTO_RENDA'
Where Id = 3;

Update CadConsultas
Set ProductURL = 'business-information-report/v1/reports?reportName=RELATORIO_AVANCADO_PJ_PMEoptionalFeatures=ANOTACOES_CONSULTAS_SPC,FATURAMENTO_ESTIMADO_POSITIVO'
Where Id = 4;

Update CadConsultas
Set ProductURL = 'person-information-report/v1/creditreport?reportName=RELATORIO_BASICO_PF_PME'
Where Id = 7;

Update CadConsultas
Set ProductURL = 'business-information-report/v1/reports?reportName=RELATORIO_BASICO_PJ_PME'
Where Id = 8;