USE SapatoVelhoDB;
GO

-- A senha agora é guardada como hash (84 caracteres). Aumenta a coluna para folga.
ALTER TABLE Usuarios ALTER COLUMN Senha VARCHAR(255) NOT NULL;
GO
