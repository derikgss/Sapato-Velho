# API do Sapato Velho

ASP.NET Core + Entity Framework Core + SQL Server, com **senha em hash (PBKDF2)** e **login por token JWT**.

## Antes de executar
1. Abra o SQL Server Express (`localhost\SQLEXPRESS`).
2. Execute os scripts de `Back-End/DataBase` **nesta ordem**:
   `SQLPIM.sql` → `SQLQuery1.sql` → `SQLQuery2.sql` → `SQLQuery3.sql` → `SQLQuery4.sql` → `SQLQuery5.sql` → `SQLQuery6.sql`
   - `SQLQuery5.sql` aumenta a coluna `Senha` (para caber o hash).
   - `SQLQuery6.sql` cadastra os 4 produtos da loja (precisam existir para o checkout funcionar).
3. Confira a connection string em `SapatoVelho.Api/appsettings.json`.
4. Confira a chave do JWT (`Jwt:Key`, mínimo 32 caracteres). Em desenvolvimento ela está em
   `appsettings.Development.json`. **Antes de publicar no GitHub/produção**, use outra chave e guarde-a fora do código:
   ```
   cd SapatoVelho.Api
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "uma-chave-grande-e-aleatoria-com-mais-de-32-caracteres"
   ```
5. Execute o projeto (perfil **http**, porta `5141`). O Front-End espera a API em `http://localhost:5141`.

## Como o login funciona
`POST /api/usuarios/login` devolve `{ token, expiraEm, usuario }`. Nas rotas protegidas envie:
`Authorization: Bearer <token>`. O token vale 120 minutos (`Jwt:ExpiraEmMinutos`).

Contas antigas, com senha em texto puro, continuam entrando: no primeiro login a senha é convertida em hash.

## Perfis (coluna `tipo_usuario`)
1 = Admin, 2 = Funcionario, 3 = Cliente (todo cadastro novo é Cliente).
Para promover uma conta a Admin, rode no SQL:
`UPDATE Usuarios SET tipo_usuario = 1 WHERE Email = 'seu@email.com';`

## Endpoints
| Método | Rota | Acesso |
|---|---|---|
| GET | `/api/produtos`, `/api/produtos/{id}` | público |
| POST/PUT/DELETE | `/api/produtos`, `/api/produtos/{id}` | Admin/Funcionario |
| POST | `/api/usuarios/cadastro` | público |
| POST | `/api/usuarios/login` | público |
| GET/PUT/DELETE | `/api/usuarios/me` | logado |
| GET | `/api/usuarios`, `/api/usuarios/{id}` | Admin/Funcionario |
| POST | `/api/pedidos` | logado (usuário vem do token) |
| GET | `/api/pedidos/meus` | logado |
| GET | `/api/pedidos/{id}` | dono do pedido ou equipe |
| GET | `/api/pedidos` | Admin/Funcionario |

O Swagger fica em `/swagger` (Development), mas ele **não tem o botão de token**: para testar rotas protegidas use o
arquivo `SapatoVelho.Api.http` (Visual Studio) ou o próprio site.

## Exemplo de pedido
```json
{
  "formaPagamento": "PIX",
  "enderecoEntrega": "Rua Exemplo, 100",
  "itens": [ { "produtoId": 1, "quantidade": 2 } ]
}
```
O preço e o total são sempre calculados pela API a partir do banco.
