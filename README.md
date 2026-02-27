# AgroSolutions - Microsserviço de Propriedades e Talhões

## 📋 Descrição

Microsserviço para gerenciamento de propriedades rurais e seus talhões (áreas de plantio), implementado seguindo o padrão **Domain-Driven Design (DDD)** com arquitetura em camadas.

## 🏗️ Arquitetura

O projeto está organizado em 4 camadas principais:

### Core (Domínio)
Contém a lógica de negócio principal, entidades de domínio, value objects e interfaces.

**Entidades:**
- `Property` - Propriedade Rural (agregado raiz)
- `Field` - Talhão de plantio

**Value Objects:**
- `Address` - Endereço completo com validações
- `Coordinates` - Coordenadas GPS (latitude/longitude)

**Enums:**
- `FieldStatus` - Status do talhão (Active/Inactive)

### Application (Aplicação)
Camada de serviços de aplicação, DTOs e contratos.

**Services:**
- `PropertyAppService` - CRUD de propriedades com regras de negócio
- `FieldAppService` - CRUD de talhões com validações

**DTOs:**
- DTOs para Property (Create, Update, Response)
- DTOs para Field (Create, Update, Response)

### Infrastructure (Infraestrutura)
Implementação de persistência, repositórios e serviços de infraestrutura.

**Componentes:**
- `ApplicationDbContext` - Entity Framework Core DbContext
- `PropertyRepository` e `FieldRepository` - Implementações dos repositórios
- `CurrentUserService` - Extração de informações do usuário do JWT
- Configurações do Entity Framework (PropertyConfiguration, FieldConfiguration)

### API (Apresentação)
Controllers, middlewares e configuração da API.

**Controllers:**
- `PropertiesController` - Endpoints para propriedades
- `FieldsController` - Endpoints para talhões

**Middlewares:**
- `ExceptionHandlingMiddleware` - Tratamento global de exceções
- `CorrelationMiddleware` - Rastreamento de requisições

## 🔐 Autenticação

O microsserviço utiliza **JWT (JSON Web Tokens)** compartilhado com o microsserviço de usuários. As configurações JWT devem ser idênticas em ambos os serviços para garantir a interoperabilidade.

## 📡 Endpoints

### Propriedades (`/api/properties`)

- `POST /api/properties` - Criar nova propriedade
- `GET /api/properties` - Listar propriedades do usuário autenticado
- `GET /api/properties/{id}` - Buscar propriedade por ID
- `PUT /api/properties/{id}` - Atualizar propriedade
- `DELETE /api/properties/{id}` - Deletar propriedade

### Talhões (`/api/fields`)

- `POST /api/fields` - Criar novo talhão
- `GET /api/fields/property/{propertyId}` - Listar talhões de uma propriedade
- `GET /api/fields/{id}` - Buscar talhão por ID
- `PUT /api/fields/{id}` - Atualizar talhão
- `DELETE /api/fields/{id}` - Deletar talhão
- `PATCH /api/fields/{id}/activate` - Ativar talhão
- `PATCH /api/fields/{id}/deactivate` - Desativar talhão

## 🔒 Regras de Negócio

### Propriedades
- Usuário só pode criar propriedades para si mesmo
- Usuário só pode visualizar, editar ou deletar suas próprias propriedades
- Não é possível deletar uma propriedade com talhões ativos
- A área total deve ser sempre maior que zero

### Talhões
- Talhões só podem ser criados em propriedades do usuário autenticado
- A área do talhão não pode exceder a área disponível da propriedade
- Usuário só pode visualizar ou editar talhões de suas propriedades
- Coordenadas GPS devem ser válidas (latitude: -90 a 90, longitude: -180 a 180)

## ⚙️ Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "ConnectionString": "Host=localhost;Port=5432;Database=agrosolutions_properties;Username=postgres;Password=senha"
  },
  "Jwt": {
    "Key": "sua-chave-secreta-jwt-compartilhada",
    "Issuer": "AgroSolutions",
    "Audience": "AgroSolutions.Users"
  }
}
```

**⚠️ IMPORTANTE:** As configurações JWT (Key, Issuer, Audience) devem ser **idênticas** às configuradas no microsserviço de usuários.

## 🚀 Como Executar

### Pré-requisitos
- .NET 8.0 SDK
- PostgreSQL

### Passos

1. Clone o repositório
2. Configure o `appsettings.json` com sua connection string e configurações JWT
3. Execute as migrations:
```bash
cd Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../AgroSolutions.Properties.Fields
dotnet ef database update --startup-project ../AgroSolutions.Properties.Fields
```

4. Execute o projeto:
```bash
cd AgroSolutions.Properties.Fields
dotnet run
```

A API estará disponível em `https://localhost:7xxx` (a porta será exibida no console).

## 📚 Swagger

A documentação interativa da API está disponível em `/swagger` quando executado em modo de desenvolvimento.

## 🧪 Estrutura do Banco de Dados

### Tabela: Property
- Id (INT, PK)
- Name (VARCHAR(200))
- AddressStreet, AddressNumber, AddressComplement, AddressCity, AddressState, AddressZipCode, AddressCountry
- Latitude, Longitude (DOUBLE PRECISION)
- TotalArea (DECIMAL(18,2))
- SoilType (VARCHAR(100))
- UserId (INT)
- CreatedAt (TIMESTAMPTZ)

### Tabela: Field
- Id (INT, PK)
- Name (VARCHAR(200))
- CropType (VARCHAR(100))
- Area (DECIMAL(18,2))
- PlantingDate (TIMESTAMPTZ, nullable)
- Latitude, Longitude (DOUBLE PRECISION)
- Status (INT)
- PropertyId (INT, FK)
- CreatedAt (TIMESTAMPTZ)

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0**
- **Entity Framework Core 8.0**
- **PostgreSQL** (Npgsql)
- **JWT Bearer Authentication**
- **Swagger/OpenAPI**

## 📝 Padrões e Práticas

- **Domain-Driven Design (DDD)**
- **Repository Pattern**
- **Dependency Injection**
- **SOLID Principles**
- **Value Objects** para encapsulamento de lógica de validação
- **Middleware Pipeline** para cross-cutting concerns
- **Async/Await** em todas as operações de I/O

---

Desenvolvido para o Hackathon 8NETT - AgroSolutions 🌱
