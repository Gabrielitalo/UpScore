USE recomecedb;
SET GLOBAL log_bin_trust_function_creators = 1;
SET SQL_SAFE_UPDATES = 0;

DELIMITER //

DROP PROCEDURE IF EXISTS GetLogSerasaConsultas;

CREATE PROCEDURE GetLogSerasaConsultas(
    IN p_Pag INT,
    IN p_IdUser BIGINT,
    IN p_IdRole BIGINT,
    IN p_IdCompany BIGINT,
    IN p_Nome VARCHAR(255),
    IN p_Inscricao VARCHAR(255),
    IN p_DataInicial DATETIME,
    IN p_DataFinal DATETIME
)
BEGIN
    DECLARE v_Limite INT DEFAULT 20;
    DECLARE v_Offset INT;

    SET v_Offset = (p_Pag - 1) * v_Limite;

  WITH UltimoMovCad AS (
        SELECT Mcs.*,
               ROW_NUMBER() OVER (PARTITION BY Mcs.MarkID ORDER BY Mcs.Id DESC) AS rn
        FROM LogConsultas Mcs
        Join CadEquipe Ce on (Ce.Id = Mcs.Fk_CadEquipe)
        Where (Ce.Fk_CadEmpresas = p_IdCompany)
    )

    SELECT 
        L.Id, 
        L.Inscricao, 
        L.MarkID, 
        L.DataHoraConsulta,
        L.Legado,
        C.Tipo,
        C.Id ConsultaId,
        C.Nome NomeConsulta,
        COALESCE(Cc.Nome, '') AS Nome,
        COALESCE(L.Score, '') AS Score
    FROM LogConsultas L
    Join CadConsultas C on (C.Id = L.Fk_CadConsultas)
    Join CadEquipe Ce on (Ce.Id = L.Fk_CadEquipe) and (Ce.Fk_CadEmpresas = p_IdCompany)
    -- LEFT JOIN UltimoMovCad Mc ON Mc.MarkID = L.MarkID AND Mc.rn = 1
    LEFT JOIN CadClientes Cc ON Cc.Inscricao = L.Inscricao
    WHERE ((p_IdRole > 1 and L.Fk_CadEquipe = p_IdUser) or (p_IdRole in (0, 1))) and
      ((
        COALESCE(p_Inscricao, '') <> ''
        AND L.Inscricao = p_Inscricao
      )
      OR
      (
        COALESCE(p_Inscricao, '') = ''
        AND L.Situacao = 1
        AND (L.DataHoraConsulta BETWEEN p_DataInicial AND p_DataFinal)
        AND Cc.Nome LIKE 
            CASE 
                WHEN COALESCE(p_Nome, '') = '' THEN Cc.Nome
                ELSE CONCAT('%', p_Nome, '%') 
            END
      ))
    ORDER BY L.DataHoraConsulta DESC
    LIMIT v_Limite OFFSET v_Offset;
END //

DELIMITER ;
