-- ARQUIVO LEGADO / HISTORICO: NAO EXECUTE INDIVIDUALMENTE.
-- Use exclusivamente SapatoVelhoDB_Consolidado.sql no banco criado pelo provedor.
USE SapatoVelhoDB;
GO

-- A senha agora é guardada como hash (84 caracteres). Aumenta a coluna para folga.
ALTER TABLE Usuarios ALTER COLUMN Senha VARCHAR(255) NOT NULL;
GO
