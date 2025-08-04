ALTER TABLE `MovPropostasDuplicatas` 
ADD COLUMN `IdExtPagto` VARCHAR(255) NULL AFTER `Valor`,
ADD COLUMN `Situacao` INT NULL AFTER `IdExtPagto`,
ADD COLUMN `DataCriacaoExtPagto` DATETIME NULL AFTER `Situacao`;

ALTER TABLE `upscore`.`CadProdutos` 
DROP COLUMN `Fk_CadConsultas`,
ADD COLUMN `TipoConsulta` INT NULL DEFAULT 1 AFTER `LimpaNome`;