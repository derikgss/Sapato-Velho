-- ARQUIVO LEGADO / HISTORICO: NAO EXECUTE INDIVIDUALMENTE.
-- Use exclusivamente SapatoVelhoDB_Consolidado.sql no banco criado pelo provedor.
CREATE TABLE Pedidos(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    UsuariosID INT NOT NULL,
    DataPedido DATETIME NOT NULL DEFAULT GETDATE(),
    ValorPedido DECIMAL(10,2) NOT NULL,
    FormaPgt VARCHAR(100) NOT NULL,
    EnderecoEntrega VARCHAR(100) NOT NULL,

    CONSTRAINT FK_Pedidos_Usuarios 
        FOREIGN KEY (UsuariosID)
        REFERENCES Usuarios(ID)
    );
GO