-- ARQUIVO LEGADO / HISTORICO: NAO EXECUTE INDIVIDUALMENTE.
-- Use exclusivamente SapatoVelhoDB_Consolidado.sql no banco criado pelo provedor.
USE SapatoVelhoDB;
GO

-- Produtos iniciais da loja (o nome precisa bater com o das páginas do Front-End).
-- Ajuste o estoque como quiser.
INSERT INTO Produtos (NomeProduto, Preco, Imagem, Estoque)
SELECT v.NomeProduto, v.Preco, v.Imagem, v.Estoque
FROM (VALUES
    ('Console SNES',         300.00, 'img/produtos/snes.jpeg',            5.00),
    ('Livro CSS na Web',      20.00, 'img/produtos/CssBook.jpeg',         5.00),
    ('Caixa de Ferramentas',  35.00, 'img/produtos/CaixaFerramenta.png',  5.00),
    ('Allstar Preto',         70.00, 'img/produtos/Tenis.jpeg',           5.00)
) AS v(NomeProduto, Preco, Imagem, Estoque)
WHERE NOT EXISTS (SELECT 1 FROM Produtos p WHERE p.NomeProduto = v.NomeProduto);
GO

-- (Opcional) Para tornar uma conta Admin e poder cadastrar/editar produtos pela API:
-- UPDATE Usuarios SET tipo_usuario = 1 WHERE Email = 'seu@email.com';
