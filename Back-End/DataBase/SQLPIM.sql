-- ARQUIVO LEGADO / HISTORICO: NAO EXECUTE INDIVIDUALMENTE.
-- Use exclusivamente SapatoVelhoDB_Consolidado.sql no banco criado pelo provedor.
-- 1. Cria o banco de dados do projeto Sapato Velho
CREATE DATABASE SapatoVelhoDB;
GO

-- 2. Ativa o banco de dados para os próximos comandos
USE SapatoVelhoDB;
GO

-- 3. Cria a tabela com as colunas idênticas às que o Node.js espera receber
CREATE TABLE Usuarios (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Nome VARCHAR(150) NOT NULL,
    DataNascimento DATE,
    CPF VARCHAR(20) UNIQUE NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    Senha VARCHAR(100) NOT NULL,
    Endereco VARCHAR(200),
    CEP VARCHAR(20),
    tipo_usuario INT NOT NULL DEFAULT 3,
    Cidade VARCHAR(100)
);
GO