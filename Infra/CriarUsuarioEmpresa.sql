DELIMITER $$

DROP PROCEDURE IF EXISTS CriarUsuarioEmpresa $$

CREATE PROCEDURE CriarUsuarioEmpresa(
    IN p_CadEmpresas BIGINT,
    IN p_TipoUsuario INT,
    IN p_Senha VARCHAR(500)
)
BEGIN
    INSERT INTO CadEquipe (
        Fk_CadEmpresas, Ativo, Tipo, Nome, Inscricao, Email, Telefone,
        Logradouro, Numero, Bairro, Cep, Complemento, Senha
    )
    SELECT 
        CE.Id, 1, p_TipoUsuario, CE.NomeResponsavel, CE.CpfResponsavel, CE.EmailResponsavel,
        '', '', '', '', '', '', p_Senha
    FROM CadEmpresas CE
    WHERE CE.Id = p_CadEmpresas
      AND NOT EXISTS (
          SELECT 1
          FROM CadEquipe E
          WHERE E.Inscricao = CE.CpfResponsavel
      );
END $$

DELIMITER ;
