# Sapato Velho

Projeto de e-commerce acadêmico com Front-End em HTML/CSS/JavaScript, API em ASP.NET Core e banco SQL Server.

## Estrutura

- `Front-End/`: páginas estáticas publicadas no GitHub Pages.
- `Back-End/SapatoVelho.Api/`: API ASP.NET Core 10.
- `Back-End/DataBase/SapatoVelhoDB_Consolidado.sql`: script único para criar/migrar o banco SQL Server.
- `Back-End/API-README.md`: instruções de banco, variáveis de ambiente, hospedagem e endpoints.

## Importante para publicação

GitHub Pages hospeda apenas o Front-End. A API e o SQL Server precisam estar em hospedagem separada. Antes de publicar, configure a connection string e a chave JWT como variáveis privadas no painel do host e altere `Front-End/js/api.js` para o endereço HTTPS real da API.

Siga `Back-End/API-README.md` e execute somente `Back-End/DataBase/SapatoVelhoDB_Consolidado.sql`. Os arquivos `SQLPIM.sql` e `SQLQuery*.sql` são scripts antigos mantidos como histórico; não os execute individualmente.
