/*
  SAPATO VELHO - banco consolidado para MonsterASP.NET / SQL Server
  Execute este arquivo UMA VEZ no banco SapatoVelhoDB já criado no painel.
  NÃO execute CREATE DATABASE: o provedor já criou o banco db81947.
  O script é idempotente para tabelas, cargos e produtos.
*/
SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Cargos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cargos (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cargos PRIMARY KEY,
        Nome VARCHAR(50) NOT NULL
    );
    CREATE UNIQUE INDEX UX_Cargos_Nome ON dbo.Cargos(Nome);
END;
GO

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
        Nome VARCHAR(150) NOT NULL,
        DataNascimento DATE NULL,
        CPF VARCHAR(20) NOT NULL,
        Email VARCHAR(100) NOT NULL,
        Senha VARCHAR(255) NOT NULL,
        Endereco VARCHAR(200) NULL,
        CEP VARCHAR(20) NULL,
        Cidade VARCHAR(100) NULL,
        CargoID INT NOT NULL CONSTRAINT DF_Usuarios_CargoID DEFAULT (3),
        CONSTRAINT FK_Usuarios_Cargos FOREIGN KEY (CargoID) REFERENCES dbo.Cargos(ID)
    );
    CREATE UNIQUE INDEX UX_Usuarios_CPF ON dbo.Usuarios(CPF);
    CREATE UNIQUE INDEX UX_Usuarios_Email ON dbo.Usuarios(Email);
END;
GO

IF OBJECT_ID(N'dbo.Produtos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Produtos (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Produtos PRIMARY KEY,
        NomeProduto VARCHAR(100) NOT NULL,
        Preco DECIMAL(10,2) NOT NULL,
        Imagem VARCHAR(255) NOT NULL,
        Estoque INT NOT NULL CONSTRAINT DF_Produtos_Estoque DEFAULT (0),
        CONSTRAINT CK_Produtos_Preco CHECK (Preco >= 0),
        CONSTRAINT CK_Produtos_Estoque CHECK (Estoque >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.Pedidos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pedidos (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Pedidos PRIMARY KEY,
        UsuariosID INT NOT NULL,
        DataPedido DATETIME2(0) NOT NULL CONSTRAINT DF_Pedidos_DataPedido DEFAULT (SYSUTCDATETIME()),
        ValorPedido DECIMAL(10,2) NOT NULL,
        FormaPgt VARCHAR(100) NOT NULL,
        EnderecoEntrega VARCHAR(200) NOT NULL,
        Status VARCHAR(30) NOT NULL CONSTRAINT DF_Pedidos_Status DEFAULT ('Recebido'),
        CONSTRAINT FK_Pedidos_Usuarios FOREIGN KEY (UsuariosID) REFERENCES dbo.Usuarios(ID),
        CONSTRAINT CK_Pedidos_Valor CHECK (ValorPedido >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.ItensPedido', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ItensPedido (
        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ItensPedido PRIMARY KEY,
        PedidosID INT NOT NULL,
        ProdutosID INT NOT NULL,
        Quantidade INT NOT NULL,
        PrecoUnidade DECIMAL(10,2) NOT NULL,
        CONSTRAINT FK_ItensPedido_Pedidos FOREIGN KEY (PedidosID) REFERENCES dbo.Pedidos(ID) ON DELETE CASCADE,
        CONSTRAINT FK_ItensPedido_Produtos FOREIGN KEY (ProdutosID) REFERENCES dbo.Produtos(ID),
        CONSTRAINT CK_ItensPedido_Quantidade CHECK (Quantidade > 0),
        CONSTRAINT CK_ItensPedido_Preco CHECK (PrecoUnidade >= 0)
    );
END;
GO

/* Se tabelas antigas já existiam, ajusta o campo de senha e a ligação de cargo. */
IF COL_LENGTH('dbo.Usuarios', 'Senha') IS NOT NULL
    ALTER TABLE dbo.Usuarios ALTER COLUMN Senha VARCHAR(255) NOT NULL;
GO

/* Dados de referência: 1=Admin, 2=Funcionario, 3=Cliente. */
IF NOT EXISTS (SELECT 1 FROM dbo.Cargos WHERE Nome = 'Admin')
    INSERT INTO dbo.Cargos (Nome) VALUES ('Admin');
IF NOT EXISTS (SELECT 1 FROM dbo.Cargos WHERE Nome = 'Funcionario')
    INSERT INTO dbo.Cargos (Nome) VALUES ('Funcionario');
IF NOT EXISTS (SELECT 1 FROM dbo.Cargos WHERE Nome = 'Cliente')
    INSERT INTO dbo.Cargos (Nome) VALUES ('Cliente');
GO

/* Migra a tabela Usuarios criada pelos scripts antigos (que usavam tipo_usuario). */
IF COL_LENGTH('dbo.Usuarios', 'CargoID') IS NULL
    ALTER TABLE dbo.Usuarios ADD CargoID INT NOT NULL CONSTRAINT DF_Usuarios_CargoID_Migracao DEFAULT (3) WITH VALUES;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Usuarios_Cargos')
    ALTER TABLE dbo.Usuarios ADD CONSTRAINT FK_Usuarios_Cargos FOREIGN KEY (CargoID) REFERENCES dbo.Cargos(ID);
GO

/* Alinha tipos antigos com os modelos atuais da API. */
IF COL_LENGTH('dbo.Produtos', 'Estoque') IS NOT NULL
    ALTER TABLE dbo.Produtos ALTER COLUMN Estoque INT NOT NULL;
IF COL_LENGTH('dbo.Produtos', 'Imagem') IS NOT NULL
    ALTER TABLE dbo.Produtos ALTER COLUMN Imagem VARCHAR(255) NOT NULL;
IF OBJECT_ID(N'dbo.ItensPedido', N'U') IS NOT NULL
    ALTER TABLE dbo.ItensPedido ALTER COLUMN Quantidade INT NOT NULL;
IF COL_LENGTH('dbo.Pedidos', 'EnderecoEntrega') IS NOT NULL
    ALTER TABLE dbo.Pedidos ALTER COLUMN EnderecoEntrega VARCHAR(200) NOT NULL;
IF COL_LENGTH('dbo.Pedidos', 'Status') IS NULL
    ALTER TABLE dbo.Pedidos ADD Status VARCHAR(30) NOT NULL CONSTRAINT DF_Pedidos_Status_Migracao DEFAULT ('Recebido') WITH VALUES;
GO

/* Produtos usados nas páginas atuais do Front-End. */
INSERT INTO dbo.Produtos (NomeProduto, Preco, Imagem, Estoque)
SELECT v.NomeProduto, v.Preco, v.Imagem, v.Estoque
FROM (VALUES
    ('Console SNES',         CAST(300.00 AS DECIMAL(10,2)), 'img/produtos/snes.jpeg',           5),
    ('Livro CSS na Web',     CAST(20.00 AS DECIMAL(10,2)),  'img/produtos/CssBook.jpeg',        5),
    ('Caixa de Ferramentas', CAST(35.00 AS DECIMAL(10,2)),  'img/produtos/CaixaFerramenta.png', 5),
    ('Allstar Preto',        CAST(70.00 AS DECIMAL(10,2)),  'img/produtos/Tenis.jpeg',          5)
) AS v(NomeProduto, Preco, Imagem, Estoque)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Produtos p WHERE p.NomeProduto = v.NomeProduto);
GO

/* Verificação final */
SELECT 'Cargos' AS Tabela, COUNT(*) AS Registros FROM dbo.Cargos
UNION ALL SELECT 'Usuarios', COUNT(*) FROM dbo.Usuarios
UNION ALL SELECT 'Produtos', COUNT(*) FROM dbo.Produtos
UNION ALL SELECT 'Pedidos', COUNT(*) FROM dbo.Pedidos
UNION ALL SELECT 'ItensPedido', COUNT(*) FROM dbo.ItensPedido;
GO
