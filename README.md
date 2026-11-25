# TKM Cadastro de Cliente

Aplicativo moderno de cadastro e gerenciamento de clientes desenvolvido em C# com **WinUI 3** e **Windows App SDK** no **.NET 8**, projetado com interface Fluent Design nativa, suporte internacional inteligente a múltiplos documentos e total conformidade com privacidade de dados (LGPD e GDPR).

---

## 🌟 Principais Recursos

- **Módulo Inteligente de Documentos (InternationalDocumentManager)**:
  - 🇧🇷 **Brasil**: CPF (com algoritmo oficial de verificação), CNPJ e RG.
  - 🇵🇹 **Portugal**: NIF (com dígito de controle), NIPC e Cartão de Cidadão.
  - 🇺🇸 **Estados Unidos**: SSN (com verificação estrutural e regras de emissão) e EIN.
  - 🇨🇳 **China**: 居民身份证号码 (Resident Identity Card - 18 dígitos com verificação de data e checksum ISO 7064) e USCC.
  - 🇮🇳 **Índia**: Aadhaar (com algoritmo de checksum Verhoeff), PAN e GSTIN.
  - 🇪🇸 **Espanha**: DNI / NIE (com cálculo oficial de letra de controle) e CIF/NIF empresarial.
  - 🇸🇦 **Países de Língua Árabe**: Identificação Nacional com seleção por país específico (Arábia Saudita, Emirados Árabes, Egito, Catar, etc.) e Passaporte.
- **Sincronização Automática**:
  - Ao alterar o país no formulário ou o idioma principal do programa, o formulário ajusta automaticamente máscaras de digitação, placeholders, regras de validação e bandeira.
- **Interface Otimizada**:
  - Janela que abre maximizada adaptada a diferentes resoluções de monitor.
  - Formulário amplo em 2 colunas, projetado para exibir 100% dos campos visíveis sem corte e sem necessidade de barra de rolagem.
  - Digitação de documentos estritamente da esquerda para a direita (LTR) com posicionamento suave de cursor.
- **Multilíngue (7 Idiomas)**:
  - Português (Brasil - padrão), Português (Portugal), Inglês, Chinês Mandarim, Hindi, Espanhol e Árabe Padrão.
- **Temas Claro e Escuro**:
  - Paleta com cinza escuro e cinza claro, efeito Mica no Windows 11 e estados de hover e clique harmonizados.
- **Impressão Individual de Cadastro**:
  - Geração de ficha detalhada do cliente para impressão física ou exportação em PDF.
- **Backup e Restauração em Nuvem**:
  - Integração com Google Drive API v3 (OAuth 2.0) com retenção dos backups mais recentes.
- **Segurança de Dados**:
  - Hash SHA-256 para garantia de unicidade sem expor documentos em texto simples nos índices, e criptografia para armazenamento local em banco de dados SQLite.
- **Apoio ao Projeto**:
  - Botão integrado de apoio via Ko-fi ([ko-fi.com/tikemem](https://ko-fi.com/tikemem)).

---

## 📁 Estrutura da Solução

```text
CadastroClientesWinUI/
├── Assets/                        # Ícones e recursos visuais (app.ico)
├── Data/
│   └── DatabaseService.cs         # Gerenciamento do banco local SQLite e migrações
├── Dialogs/
│   ├── ClienteDialog.xaml / .cs   # Formulário inteligente de cadastro e edição
│   └── RestauracaoDialog.xaml/.cs # Diálogo de restauração de backups
├── InternationalDocumentManager/  # Módulo de identificação internacional
│   ├── CountryConfiguration.cs    # Definição dos países suportados
│   ├── DocumentTypes.cs           # Tipos de documentos por país
│   ├── DocumentMasks.cs           # Máscaras dinâmicas de preenchimento
│   ├── DocumentValidators.cs      # Algoritmos oficiais de validação
│   ├── Internationalization.cs    # Mapeamento de mensagens e instruções
│   └── DocumentSecurity.cs        # Criptografia e hashes de proteção
├── Models/                        # Modelos de dados e validações gerais
├── Services/                      # Serviços de localização, Drive e impressão
├── Theming/                       # Gerenciamento de tema claro e escuro
├── App.xaml / .cs                 # Ponto de entrada e recursos de temas
├── MainWindow.xaml / .cs          # Janela principal do sistema
└── Directory.Build.targets        # Empacotamento direto de recursos (makepri)
```

---

## 🚀 Como Compilar e Executar

### Pré-requisitos
- **Windows 10 (versão 1809 ou superior)** ou **Windows 11**
- **.NET 8 SDK** (versão 8.0 ou superior)
- Visual Studio 2022 (com carga de trabalho de desenvolvimento para desktop .NET) ou VS Code com extensão C#

### Compilação via Terminal
Abra o PowerShell na pasta raiz do projeto:

```powershell
# Restaurar dependências
dotnet restore

# Compilar para a plataforma x64
dotnet build -c Debug -p:Platform=x64

# Executar a aplicação
dotnet run -c Debug -p:Platform=x64
```

### Publicação (Self-Contained Executable)
Para gerar uma pasta pronta para distribuição sem exigir que o usuário final instale componentes externos:

```powershell
dotnet publish -c Release -r win-x64 -p:Platform=x64 -p:WindowsAppSDKSelfContained=true
```

---

## ☕ Apoie o Desenvolvimento

Se este projeto foi útil para você, considere apoiar o desenvolvimento contínuo através do Ko-fi:
👉 **[ko-fi.com/tikemem](https://ko-fi.com/tikemem)**
