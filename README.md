# Duvidas
 ## Como esse metodo funciona?

Código:
```csharp
private string CreateToken(User user) {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<String>("AppSettings:Issuer"),
                audience: configuration.GetValue<String>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
```
*Explicação:*
1. Objetivo do método: Criar um token JWT para um usuário autenticado.
2. Criação de Claims: Adiciona o nome de usuário do usuário autenticado como uma claim.
3. Chave Simétrica: Cria uma chave simétrica usando uma string de configuração.
4. O que é uma chave simétrica: É uma chave usada tanto para assinar quanto para verificar o token
5. Credenciais de Assinatura: Cria credenciais de assinatura usando a chave simétrica e o algoritmo HMAC SHA-512.
6. Descritor do Token: Cria um descritor de token JWT com informações como emissor, público, claims, data de expiração e credenciais de assinatura.
    - O emissor é o responsável por emitir o token.
    - O público é quem pode consumir o token.
    - claims são informações adicionais sobre o usuário.
    - Data de expiração define por quanto tempo o token é válido.
    - Credenciais de assinatura garantem que o token não foi alterado.
7. Retorno: Retorna o token JWT como uma string usando `JwtSecurityTokenHandler`.

## O que é Guid?
Um `Guid` (Globally Unique Identifier) é um identificador único globalmente utilizado para identificar de forma única objetos ou entidades em sistemas distribuídos. Ele é representado como uma sequência de 32 caracteres hexadecimais, divididos em cinco grupos separados por hífens, totalizando 36 caracteres. Por exemplo: `123e4567-e89b-12d3-a456-426614174000`.

## O que é Task?
Uma `Task` em C# é uma representação assíncrona de uma operação que pode ser executada de forma concorrente. Ela permite que você execute código de forma não bloqueante, o que significa que o fluxo do programa não é interrompido enquanto aguarda a conclusão da tarefa. As `Tasks` são frequentemente usadas para operações de I/O, como chamadas de rede ou acesso a banco de dados, onde o tempo de espera pode ser significativo.

## Sobre Tokens e Refresh Tokens
Tokens são usados para autenticar usuários em sistemas. Eles contêm informações sobre o usuário e são assinados para garantir sua integridade. Um token é geralmente válido por um período limitado, após o qual o usuário precisa se autenticar novamente.
Refresh tokens são usados para obter novos tokens sem exigir que o usuário faça login novamente. Eles têm uma vida útil mais longa e são armazenados de forma segura no cliente. Quando um token expira, o refresh token pode ser usado para solicitar um novo token, permitindo que o usuário continue acessando o sistema sem precisar inserir suas credenciais novamente.

Exemplo de uma situação:
Imagine que você está desenvolvendo um aplicativo de e-commerce. Quando um usuário faz login, você gera um token JWT que contém informações sobre o usuário, como seu ID e permissões. Esse token é enviado ao cliente e usado para autenticar solicitações subsequentes.
Quando o token expira, o cliente pode usar um refresh token para solicitar um novo token JWT sem que o usuário precise fazer login novamente. Isso melhora a experiência do usuário, pois evita que ele precise inserir suas credenciais repetidamente enquanto navega pelo aplicativo.
*Mas então como o refresh token fica armazenado no sistema?*
O refresh token é geralmente armazenado de forma segura no lado do cliente, como em um cookie seguro ou no armazenamento local do navegador. No lado do servidor, o refresh token pode ser armazenado em um banco de dados associado ao usuário, permitindo que o servidor valide o refresh token quando uma solicitação para um novo token for feita. Isso garante que apenas usuários autenticados possam obter novos tokens e mantém a segurança do sistema.