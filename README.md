#  VesteEVolta - Aluguel de Roupas

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Latest-316192?logo=postgresql)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

##  Descrição

**VesteEVolta** é um sistema desenvolvido em ASP.NET Core para facilitar o aluguel de roupas entre usuários. A plataforma permite que proprietários cadastrem suas peças para aluguel e que clientes possam pesquisar, alugar e avaliar roupas de forma prática e segura.

O projeto foi desenvolvido como parte do **Programa CodeRDiversity**.

###  Principais Funcionalidades

-  **Autenticação e Autorização** com JWT
-  **Gestão de Usuários** (Clientes e Proprietários)
-  **Catálogo de Roupas** com sistema de categorias
-  **Sistema de Aluguel** com controle de disponibilidade
-  **Processamento de Pagamentos**
-  **Sistema de Avaliações** de roupas
-  **Geração de Relatórios** em PDF
- **Sistema de Denúncias**

---

##  Tecnologias Utilizadas

### Backend
- **.NET 10.0** - Framework principal
- **ASP.NET Core Web API** - Framework para criação de APIs RESTful
- **Entity Framework Core 10.0** - ORM para acesso ao banco de dados
- **Npgsql** - Provider para PostgreSQL

### Banco de Dados
- **PostgreSQL** - Banco de dados relacional

### Segurança
- **JWT (JSON Web Tokens)** - Autenticação e autorização
- **ASP.NET Core Identity** - Hash de senhas

### Outras Bibliotecas
- **QuestPDF 2026.2.1** - Geração de relatórios em PDF
- **NUnit** - Framework de testes unitários
- **Moq** - Mock de dependências para testes

---

##  Arquitetura do Projeto

O projeto segue uma **arquitetura em camadas** (Layered Architecture) para garantir manutenibilidade, escalabilidade e separação de responsabilidades:

```
┌─────────────────────────────────┐
│       Controllers               │  ← Camada de Apresentação
│  (Recebe requisições HTTP)      │
└────────────┬────────────────────┘
             │
┌────────────▼────────────────────┐
│         Services                │  ← Camada de Negócio
│  (Regras de negócio)            │
└────────────┬────────────────────┘
             │
┌────────────▼────────────────────┐
│       Repositories              │  ← Camada de Acesso a Dados
│  (Comunicação com BD)           │
└────────────┬────────────────────┘
             │
┌────────────▼────────────────────┐
│         Models                  │  ← Camada de Dados
│  (Entidades do BD)              │
└─────────────────────────────────┘
```

### Descrição das Camadas

#### 1. **Controllers** (Camada de Apresentação)
Responsáveis por receber as requisições HTTP, validar a entrada, chamar os serviços apropriados e retornar as respostas.

#### 2. **Services** (Camada de Lógica de Negócio)
Contém toda a lógica de negócio da aplicação. Processa os dados, aplica regras de negócio e coordena operações entre diferentes repositórios.

#### 3. **Repositories** (Camada de Acesso a Dados)
Abstraem o acesso ao banco de dados. Implementam operações CRUD e queries complexas usando Entity Framework Core.

#### 4. **Models** (Camada de Dados)
Define as entidades do banco de dados e o contexto do Entity Framework.

#### 5. **DTOs** (Data Transfer Objects)
Objetos usados para transferência de dados entre as camadas, evitando exposição direta das entidades do banco.

#### 6. **Validators**
Validações personalizadas de regras de negócio.

#### 7. **Converters**
Conversores customizados para serialização/deserialização JSON (ex: DateOnly).

---

##  Estrutura de Pastas

```
VesteEVolta/
├──  VesteEVolta.sln                  # Solução do projeto
├──  coverlet.runsettings             # Configuração de cobertura de testes
├──  README.md                        # Documentação do projeto
│
├──  Veste_e_Volta/                   # Projeto principal da API
│   ├──  Program.cs                   # Ponto de entrada da aplicação
│   ├──  appsettings.json             # Configurações da aplicação
│   ├──  appsettings.Development.json # Configurações de desenvolvimento
│   ├──  VesteEVolta.csproj           # Arquivo do projeto
│   │
│   ├──  Controllers/                 # Controladores da API
│   │   ├── AuthController.cs          # Autenticação (login/registro)
│   │   ├── UsersController.cs         # Gestão de usuários
│   │   ├── ClothingsController.cs     # Gestão de roupas
│   │   ├── CategoriesController.cs    # Gestão de categorias
│   │   ├── RentallController.cs       # Gestão de aluguéis
│   │   ├── PaymentController.cs       # Processamento de pagamentos
│   │   ├── RatingController.cs        # Sistema de avaliações
│   │   └── ReportController.cs        # Sistema de denúncias
│   │
│   ├──  Services/                    # Lógica de negócio
│   │   ├── IClothingService.cs        # Interface
│   │   ├── ClothingService.cs         # Implementação
│   │   ├── IRentalService.cs
│   │   ├── RentalService.cs
│   │   ├── IPaymentService.cs
│   │   ├── PaymentService.cs
│   │   ├── IRatingService.cs
│   │   ├── RatingService.cs
│   │   ├── IReportService.cs
│   │   └── ReportService.cs
│   │
│   ├──  Repositories/                # Acesso a dados
│   │   ├── IClothingRepository.cs
│   │   ├── ClothingRepository.cs
│   │   ├── IRentalRepository.cs
│   │   ├── RentalRepository.cs
│   │   ├── IPaymentRepository.cs
│   │   ├── PaymentRepository.cs
│   │   ├── IRatingRepository.cs
│   │   ├── RatingRepository.cs
│   │   ├── IReportRepository.cs
│   │   └── ReportRepository.cs
│   │
│   ├──  Models/                      # Entidades do banco
│   │   ├── PostgresContext.cs         # Contexto do EF Core
│   │   ├── TbUser.cs                  # Usuário
│   │   ├── TbOwner.cs                 # Proprietário
│   │   ├── TbCustomer.cs              # Cliente
│   │   ├── TbClothing.cs              # Roupa
│   │   ├── TbCategory.cs              # Categoria
│   │   ├── TbRental.cs                # Aluguel
│   │   ├── TbPayment.cs               # Pagamento
│   │   ├── TbRating.cs                # Avaliação
│   │   └── TbReport.cs                # Denúncia
│   │
│   ├──  DTO/                         # Objetos de transferência
│   │   ├── AuthLoginDto.cs
│   │   ├── AuthRegisterDto.cs
│   │   ├── ClothingResponseDto.cs
│   │   ├── CreateClothingDto.cs
│   │   ├── ClothingUpdateDto.cs
│   │   ├── CategoryRequestDto.cs
│   │   ├── CategoryResponseDto.cs
│   │   ├── RentalDTO.cs
│   │   ├── RentalResponseDTO.cs
│   │   ├── PaymentDto.cs
│   │   ├── RatingDto.cs
│   │   ├── CreateReportDto.cs
│   │   ├── UpdateReportStatusDto.cs
│   │   ├── UserResponseDto.cs
│   │   └── UserUpdateDto.cs
│   │
│   ├──  Validators/                  # Validações personalizadas
│   │   └── UserValidator.cs
│   │
│   ├──  Converters/                  # Conversores JSON
│   │   └── JsonDateOnlyConverter.cs
│   │
│   └──  Database/                    # Scripts SQL
│       └── 001_initial_schema.sql     # Schema inicial
│
└──  Veste_e_Volta.Tests/            # Projeto de testes
    ├── Veste_e_Volta.Tests.csproj
    ├──  Controllers/                 # Testes de controllers
    ├──  Services/                    # Testes de services
    └── UserValidatorTests.cs           # Testes de validadores
```

---

##  Configuração do Ambiente

### Pré-requisitos

Certifique-se de ter instalado:

- **[.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** ou superior
- **[PostgreSQL 14+](https://www.postgresql.org/download/)** 
- **[Git](https://git-scm.com/downloads)**
- Um editor de código como **[Visual Studio 2022](https://visualstudio.microsoft.com/)**, **[VS Code](https://code.visualstudio.com/)** ou **[JetBrains Rider](https://www.jetbrains.com/rider/)**

### 1. Clonar o Repositório

```bash
git clone https://github.com/seu-usuario/VesteEVolta.git
cd VesteEVolta
```

### 2. Configurar o Banco de Dados PostgreSQL

#### Criar o Banco de Dados

```sql
CREATE DATABASE vestevolta;
```

#### Executar o Script de Schema

Execute o script `001_initial_schema.sql` localizado em `Veste_e_Volta/Database/`:

```bash
psql -U postgres -d vestevolta -f Veste_e_Volta/Database/001_initial_schema.sql
```

Ou utilize uma ferramenta gráfica como **pgAdmin** ou **DBeaver** para executar o script.

### 3. Configurar o `appsettings.json`

Edite o arquivo `Veste_e_Volta/appsettings.json` com suas credenciais do PostgreSQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=vestevolta;Username=seu_usuario;Password=sua_senha"
  },
  "Jwt": {
    "Key": "SUA_CHAVE_SECRETA_AQUI_MINIMO_32_CARACTERES",
    "Issuer": "VesteEVolta",
    "Audience": "VesteEVolta"
  }
}
```

** Importante:**
- A chave JWT (`Key`) deve ter no mínimo 32 caracteres
- **Nunca** commite credenciais reais no repositório
- Para produção, use variáveis de ambiente ou Azure Key Vault

### 4. String de Conexão

Formato da string de conexão PostgreSQL:

```
Host=<host>;Port=<porta>;Database=<nome_do_banco>;Username=<usuario>;Password=<senha>
```

**Exemplo local:**
```
Host=localhost;Port=5432;Database=vestevolta;Username=postgres;Password=root
```

---

##  Como Rodar o Projeto

### Modo Desenvolvimento

#### Via .NET CLI

```bash
cd Veste_e_Volta
dotnet restore
dotnet build
dotnet run
```

#### Via Visual Studio

1. Abra o arquivo `VesteEVolta.sln`
2. Defina `Veste_e_Volta` como projeto de inicialização
3. Pressione `F5` ou clique em "Iniciar"

### Acessar a API

Após iniciar, a API estará disponível em:

- **HTTP:** `http://localhost:5000`
- **HTTPS:** `https://localhost:5001`


##  Migrations (Entity Framework Core)

### Verificar se há Migrations Pendentes

```bash
cd Veste_e_Volta
dotnet ef migrations list
```

### Criar uma Nova Migration

```bash
dotnet ef migrations add NomeDaMigration
```

### Aplicar Migrations ao Banco

```bash
dotnet ef database update
```

### Reverter a Última Migration

```bash
dotnet ef migrations remove
```

### Gerar Script SQL de uma Migration

```bash
dotnet ef migrations script --output migration.sql
```

**Nota:** Este projeto usa migrations através de scripts SQL diretos. Se preferir usar EF Migrations, remova o script `001_initial_schema.sql` e crie migrations a partir dos Models.

---

##  Como Executar os Testes

### Executar Todos os Testes

```bash
dotnet test
```

### Executar Testes com Saída Detalhada

```bash
dotnet test --verbosity detailed
```

### Executar Testes com Cobertura de Código

```bash
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### Executar Testes de um Projeto Específico

```bash
dotnet test Veste_e_Volta.Tests/Veste_e_Volta.Tests.csproj
```

### Gerar Relatório de Cobertura (HTML)

```bash
# Instalar ferramenta reportgenerator (uma vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório HTML
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html

# Abrir relatório
start coveragereport/index.html  # Windows
```

---

##  Padrões e Boas Práticas Utilizadas

### Arquiteturais
-  **Layered Architecture** - Separação em camadas (Controllers, Services, Repositories)
-  **Repository Pattern** - Abstração do acesso a dados
-  **Dependency Injection** - Inversão de controle
-  **DTO Pattern** - Transferência de dados entre camadas

### Código
-  **SOLID Principles**
  - **S**ingle Responsibility
  - **O**pen/Closed
  - **L**iskov Substitution
  - **I**nterface Segregation
  - **D**ependency Inversion
-  **Clean Code** - Código legível e manutenível
-  **Async/Await** - Operações assíncronas para melhor performance

### Segurança
-  **JWT Authentication** - Tokens seguros para autenticação
-  **Password Hashing** - Senhas hasheadas com ASP.NET Identity
-  **Authorization** - Controle de acesso baseado em claims
-  **Input Validation** - Validação de entrada de dados

### Testes
-  **Unit Tests** - Testes unitários com NUnit
-  **Mocking** - Uso de Moq para isolar dependências
-  **AAA Pattern** - Arrange, Act, Assert

### API
-  **RESTful Design** - Endpoints seguindo princípios REST
-  **HTTP Status Codes** - Uso correto de códigos de status
-  **Content Negotiation** - Suporte a JSON

---

##  Exemplos de Endpoints

###  Autenticação

#### Registrar Novo Usuário

```http
POST /auth/register
Content-Type: application/json

{
  "Name": "Maria Silva",
  "Telephone": "(11) 98765-4321",
  "Email": "maria@email.com",
  "Password": "Senha@123",
  "ProfileType": "Owner"
}
```

**Resposta (200 OK):**
```json
{
  "message": "Usuário registrado com sucesso.",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

#### Login

```http
POST /auth/login
Content-Type: application/json

{
  "Email": "maria@email.com",
  "Password": "Senha@123"
}
```

**Resposta (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Maria Silva",
  "email": "maria@email.com",
  "profileType": "Owner"
}
```

---

###  Roupas

#### Listar Todas as Roupas (com filtros opcionais)

```http
GET /clothes?status=available&minPrice=50&maxPrice=200&categoryId=uuid-da-categoria
Authorization: Bearer {seu-token-jwt}
```

**Resposta (200 OK):**
```json
[
  {
    "Id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "Description": "Vestido de festa longo, cor azul marinho",
    "RentPrice": 150.00,
    "AvailabilityStatus": "available",
    "OwnerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "Categories": [
      {
        "CategoryId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
        "Name": "Festa"
      }
    ]
  }
]
```

#### Cadastrar Nova Roupa

```http
POST /clothes
Authorization: Bearer {seu-token-jwt}
Content-Type: application/json

{
  "Description": "Terno social masculino, cor preto",
  "RentPrice": 200.00,
  "AvailabilityStatus": "available",
  "CategoryIds": [
    "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "6fa85f64-5717-4562-b3fc-2c963f66afa9"
  ]
}
```

**Resposta (201 Created):**
```json
{
  "Id": "7fa85f64-5717-4562-b3fc-2c963f66afb1",
  "Description": "Terno social masculino, cor preto",
  "RentPrice": 200.00,
  "AvailabilityStatus": "available",
  "OwnerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "Categories": [...]
}
```

#### Atualizar Roupa

```http
PUT /clothes/{id}
Authorization: Bearer {seu-token-jwt}
Content-Type: application/json

{
  "Description": "Terno social masculino, cor preto (AJUSTADO)",
  "RentPrice": 180.00,
  "AvailabilityStatus": "available"
}
```

#### Deletar Roupa

```http
DELETE /clothes/{id}
Authorization: Bearer {seu-token-jwt}
```

**Resposta (204 No Content)**

---

###  Aluguéis

#### Criar Novo Aluguel

```http
POST /rentals
Authorization: Bearer {seu-token-jwt}
Content-Type: application/json

{
  "ClothingId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "StartDate": "2026-03-01",
  "EndDate": "2026-03-05"
}
```

**Resposta (201 Created):**
```json
{
  "RentalId": "8fa85f64-5717-4562-b3fc-2c963f66afb2",
  "ClothingId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "CustomerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "StartDate": "2026-03-01",
  "EndDate": "2026-03-05",
  "TotalPrice": 600.00,
  "Status": "pending"
}
```

#### Listar Meus Aluguéis

```http
GET /rentals/my-rentals
Authorization: Bearer {seu-token-jwt}
```

---

###  Avaliações

#### Criar Avaliação

```http
POST /ratings
Authorization: Bearer {seu-token-jwt}
Content-Type: application/json

{
  "ClothingId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "Rating": 5,
  "Comment": "Roupa impecável! Muito bem cuidada."
}
```

**Resposta (201 Created):**
```json
{
  "RatingId": "9fa85f64-5717-4562-b3fc-2c963f66afb3",
  "ClothingId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "CustomerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "Rating": 5,
  "Comment": "Roupa impecável! Muito bem cuidada.",
  "CreatedAt": "2026-02-19T10:30:00Z"
}
```

---

###  Pagamentos

#### Processar Pagamento

```http
POST /payments
Authorization: Bearer {seu-token-jwt}
Content-Type: application/json

{
  "RentalId": "8fa85f64-5717-4562-b3fc-2c963f66afb2",
  "PaymentMethod": "credit_card",
  "Amount": 600.00
}
```

**Resposta (200 OK):**
```json
{
  "PaymentId": "afa85f64-5717-4562-b3fc-2c963f66afb4",
  "RentalId": "8fa85f64-5717-4562-b3fc-2c963f66afb2",
  "Amount": 600.00,
  "PaymentMethod": "credit_card",
  "PaymentStatus": "completed",
  "PaymentDate": "2026-02-19T10:35:00Z"
}
```

---

###  Relatórios

#### Gerar Relatório em PDF

```http
POST /reports
Authorization: Bearer {seu-token-jwt}
Content-Type: application/json

{
  "ReportType": "rental_summary",
  "StartDate": "2026-01-01",
  "EndDate": "2026-02-19"
}
```

**Resposta (200 OK):**
```
Content-Type: application/pdf
Content-Disposition: attachment; filename="relatorio.pdf"

[Binary PDF Data]
```

---

###  Denúncias

#### Criar Denúncia(Report)

```http
POST /reports/complaint
Authorization: Bearer {seu-token-jwt}
Content-Type: application/json

{
  "ReportedUserId": "bfa85f64-5717-4562-b3fc-2c963f66afb5",
  "Reason": "Roupa devolvida danificada",
  "Description": "A roupa foi devolvida com manchas e rasgos."
}
```

**Resposta (201 Created):**
```json
{
  "ReportId": "cfa85f64-5717-4562-b3fc-2c963f66afb6",
  "ReportedUserId": "bfa85f64-5717-4562-b3fc-2c963f66afb5",
  "ReporterId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "Reason": "Roupa devolvida danificada",
  "Description": "A roupa foi devolvida com manchas e rasgos.",
  "Status": "pending",
  "CreatedAt": "2026-02-19T10:40:00Z"
}
```

---

<div align="center">

**Desenvolvido com ❤️ pela equipe VesteEVolta**

⭐ Se este projeto foi útil, considere dar uma estrela!

</div>
This project was developed by **[Ester Souza](https://www.linkedin.com/in/estersouza/)** and **[Jasmin Caroline](https://www.linkedin.com/in/jasmincaroline)** during the **CodeRDiversity Program**,  
taught by **[Camille Gachido](https://www.linkedin.com/in/camille-gachido/)**,  
powered by **[Prosper Digital Skills](https://prosperdigitalskills.com)**,  
and sponsored by **[RDI Software](https://www.rdisoftware.com/)**.
