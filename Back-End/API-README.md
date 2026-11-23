# Sapato Velho API — configuração para hospedagem

API ASP.NET Core 10 + Entity Framework Core + SQL Server. O projeto inclui cadastro/login, JWT, produtos, pedidos, estoque, perfis e consulta de conta.

## 1. Criar o banco no MonsterASP.NET

O banco `SapatoVelhoDB` já foi criado no painel. No print do painel, os dados apresentados foram:

- Servidor: `db81947.databaseasp.net`
- Porta: `1433`
- Login: `db81947`
- Banco: `db81947`
- Tipo: SQL Server (MSSQL)

Use os valores exatamente como aparecem no painel atual. A senha fica somente no painel do provedor; não a coloque no GitHub nem a envie em mensagens.

Abra **Databases → SapatoVelhoDB → Open WebMSSQL** (ou a opção de executar/importar SQL), selecione o banco e execute apenas:

`Back-End/DataBase/SapatoVelhoDB_Consolidado.sql`

Não execute `SQLPIM.sql` nem os `SQLQuery*.sql` antigos em sequência. Eles são scripts históricos, têm nomes de colunas incompatíveis com os modelos atuais e tentam criar um banco local chamado `SapatoVelhoDB`. O script consolidado cria/migra as tabelas no banco que o provedor já criou e inclui os quatro produtos do Front-End.

Tabelas: `Cargos`, `Usuarios`, `Produtos`, `Pedidos`, `ItensPedido`.

Cargos: `1 = Admin`, `2 = Funcionario`, `3 = Cliente`. Todo cadastro público recebe Cliente automaticamente.

## 2. Variáveis de ambiente da API

No painel da hospedagem do site ASP.NET, configure estas variáveis/Environment Variables (não salve credenciais no repositório):

- `ConnectionStrings__ConexaoPadrao` = `Server=db81947.databaseasp.net,1433;Database=db81947;User Id=db81947;Password=SUA_SENHA_DO_PAINEL;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;`
- `Jwt__Key` = uma chave aleatória longa, com no mínimo 32 bytes. Gere uma chave nova e privada; não use o exemplo nem a senha do banco.
- `Jwt__Issuer` = `SapatoVelho.Api`
- `Jwt__Audience` = `SapatoVelho.FrontEnd`
- `Jwt__ExpiraEmMinutos` = `120`

O hostname de **Local access** mostrado pelo MonsterASP é para sites hospedados dentro da mesma infraestrutura. Não use esse hostname em um computador externo. A string acima segue o hostname mostrado no painel; se a aba **Remote access** fornecer um host diferente, use o host indicado nela somente para conexões externas.

## 3. Publicar a API

1. Crie no MonsterASP um website do tipo ASP.NET Core/.NET 10 (se o plano não oferecer .NET 10, confirme com o suporte qual runtime está habilitado antes de mudar o TargetFramework).
2. Publique a pasta `Back-End/SapatoVelho.Api` como aplicação ASP.NET Core, usando o método de publicação indicado pelo painel (preferencialmente `dotnet publish -c Release`).
3. Configure as variáveis de ambiente acima no website e reinicie a aplicação.
4. Abra `https://SEU-SITE-DA-API/`. Deve retornar JSON com `"status":"online"`.
5. Abra `https://SEU-SITE-DA-API/swagger` para testar os endpoints.

## 4. Conectar o GitHub Pages à API

Em `Front-End/js/api.js`, altere a constante `API_URL` para o endereço HTTPS real da API, incluindo `/api`, por exemplo:

`const API_URL = "https://SEU-SITE-DA-API/api";`

Não deixe `localhost` nessa versão publicada: no navegador do visitante, `localhost` aponta para o computador dele, não para o servidor. O domínio do Front-End já está incluído em `Cors:AllowedOrigins` (`https://derikgss.github.io`). Se você usar outro domínio, adicione apenas a origem (sem caminho `/Sapato-Velho`) à configuração CORS e reinicie a API.

## 5. Endpoints implementados

| Método | Endpoint | Acesso |
|---|---|---|
| POST | `/api/usuarios/cadastro` | Público |
| POST | `/api/usuarios/login` | Público |
| GET / PUT / DELETE | `/api/usuarios/me` | Logado |
| GET | `/api/usuarios` e `/api/usuarios/{id}` | Admin/Funcionario |
| PUT | `/api/usuarios/{id}/cargo` | Admin |
| GET | `/api/produtos` e `/api/produtos/{id}` | Público |
| POST / PUT / DELETE | `/api/produtos` e `/api/produtos/{id}` | Admin/Funcionario |
| POST | `/api/pedidos` | Logado |
| GET | `/api/pedidos/meus` | Logado |
| GET | `/api/pedidos/{id}` | Dono ou equipe |
| GET | `/api/pedidos` | Admin/Funcionario |
| PUT | `/api/pedidos/{id}/status` | Admin/Funcionario |
| GET | `/api/cargos` | Admin |

O preço de cada item e o total do pedido são calculados no servidor. O estoque é validado e decrementado na mesma transação do pedido.

## 6. Primeiro administrador

Cadastre sua conta normalmente pelo site. Depois, no WebMSSQL, localize seu e-mail e execute uma única vez, substituindo pelo seu e-mail:

```sql
UPDATE dbo.Usuarios
SET CargoID = 1
WHERE Email = 'SEU_EMAIL';
```

Saia e entre novamente para receber um JWT com o perfil Admin. Nunca permita que o cadastro público escolha seu próprio CargoID.

## 7. Senhas e compatibilidade

Novas senhas são armazenadas com o `PasswordHasher` do ASP.NET Core (PBKDF2). Se houver contas antigas com senha em texto puro, o primeiro login válido converte a senha para hash; por segurança, essa compatibilidade deve ser removida depois que as contas antigas forem migradas.

## 8. Observações importantes

- GitHub Pages hospeda somente o Front-End estático. A API e o SQL Server precisam ficar no MonsterASP (ou em outros serviços de hospedagem).
- `appsettings.json` não contém senha nem chave JWT. Configure os segredos no painel do host.
- A API exige `ConnectionStrings__ConexaoPadrao` e `Jwt__Key`; sem eles, encerra a inicialização para não subir com configuração insegura.
- Antes de aceitar pagamentos reais, integre um gateway de pagamento. O campo FormaPagamento registra a escolha, mas não processa cobranças.
