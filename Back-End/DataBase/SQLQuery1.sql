-- ARQUIVO LEGADO / HISTORICO: NAO EXECUTE INDIVIDUALMENTE.
-- Use exclusivamente SapatoVelhoDB_Consolidado.sql no banco criado pelo provedor.
CREATE TABLE Produtos (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    NomeProduto VARCHAR(100)NOT NULL,
    Preco DECIMAL(10,2) NOT NULL,
    Imagem VARCHAR (100) NOT NULL,
    Estoque DECIMAL(10,2) NOT NULL
    );
GO