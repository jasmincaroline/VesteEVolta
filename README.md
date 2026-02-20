#  VesteEVolta - Aluguel de Roupas

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Latest-316192?logo=postgresql)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## Descrição

![WhatsApp Image 2026-02-18 at 22 14 41 (1)](https://github.com/user-attachments/assets/2e6a6390-c1a1-4210-a34d-1e91d3aee842)


**VesteEVolta** é um sistema desenvolvido em ASP.NET Core para facilitar o aluguel de roupas entre usuários.  A plataforma permite que proprietários cadastrem suas peças para aluguel e que clientes possam pesquisar, alugar e avaliar roupas de forma prática e segura.


---


Desenvolvido por  **[Ester Souza](https://www.linkedin.com/in/estersouza/)** e  **[Jasmin Caroline](https://www.linkedin.com/in/jasmincaroline)** durante o **CodeRDiversity Program**, ministrado por  **[Camille Gachido](https://www.linkedin.com/in/camille-gachido/)**,   com apoio da **[Prosper Digital Skills](https://prosperdigitalskills.com)**  e patrocinado por **[RDI Software](https://www.rdisoftware.com/)**.




###  Principais Funcionalidades


-  **Gestão de Usuários** (Clientes e Proprietários)
-  **Catálogo de Roupas** com sistema de categorias
-  **Sistema de Aluguel** com controle de disponibilidade
-  **Processamento de Pagamentos**
-  **Sistema de Avaliações** de roupas
-  **Autenticação e Autorização** com JWT
-  **Geração de Relatórios** em PDF

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


---
<div align="center">

⭐ Se este projeto foi útil, considere dar uma estrela!

<br><br>

</div>
