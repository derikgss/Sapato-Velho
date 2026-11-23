-- ARQUIVO LEGADO / HISTORICO: NAO EXECUTE INDIVIDUALMENTE.
-- Use exclusivamente SapatoVelhoDB_Consolidado.sql no banco criado pelo provedor.
CREATE TABLE ItensPedido(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    PedidosID INT NOT NULL,
    ProdutosID INT NOT NULL,
    Quantidade DECIMAL(10,2)NOT NULL,
    PrecoUnidade DECIMAL(10,2)NOT NULL,

    CONSTRAINT FK_ItensPedido_Pedidos
      FOREIGN KEY (PedidosID) 
      REFERENCES Pedidos(ID),

     CONSTRAINT FK_ItensPedido_Ped
      FOREIGN KEY (ProdutosID)
      REFERENCES Produtos(ID)
);

GO